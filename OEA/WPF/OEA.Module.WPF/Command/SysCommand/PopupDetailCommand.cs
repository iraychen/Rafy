﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20120415
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20120415
 * 
*******************************************************/


using System.Windows;
using OEA.Library;
using OEA.MetaModel;
using OEA.MetaModel.View;
using OEA.MetaModel.Attributes;
using OEA.Module.WPF;
using OEA.Module.WPF.Behaviors;
using OEA.Module.WPF.Controls;
using System;
using System.Windows.Controls;

namespace OEA.WPF.Command
{
    public abstract class PopupDetailCommand : ListViewCommand
    {
        protected WindowButton PopupEditingDialog(EntityViewMeta evm, Entity tmpEntity, Action<ViewDialog> windowSetter)
        {
            //弹出窗体显示详细面板
            var detailView = AutoUI.ViewFactory.CreateDetailObjectView(evm);
            detailView.Data = tmpEntity;

            //使用一个 StackPanel，保证详细面板不会自动变大
            var ctrl = detailView.Control;
            ctrl.VerticalAlignment = VerticalAlignment.Top;
            ctrl = new StackPanel { Children = { ctrl } };

            var result = App.Current.Windows.ShowDialog(ctrl, w =>
            {
                w.Buttons = ViewDialogButtons.YesNo;
                w.SizeToContent = SizeToContent.Height;
                w.MinHeight = 200;
                w.MinWidth = 200;
                w.Width = 400 * detailView.ColumnsCount;
                w.ValidateOperations += (o, e) =>
                {
                    var broken = tmpEntity.ValidationRules.Validate();
                    if (broken.Count > 0)
                    {
                        App.Current.MessageBox.Show("属性错误", broken.ToString());
                        e.Cancel = true;
                    }
                };

                windowSetter(w);

                this.OnWindowShowing(w);
            });

            return result;
        }

        public event EventHandler<WindowShowingEventArgs> WindowShowing;

        protected virtual void OnWindowShowing(Window win)
        {
            var handler = this.WindowShowing;
            if (handler != null) handler(this, new WindowShowingEventArgs(win));
        }

        public class WindowShowingEventArgs : EventArgs
        {
            public WindowShowingEventArgs(Window window)
            {
                this.Window = window;
            }

            public Window Window { get; private set; }
        }
    }
}