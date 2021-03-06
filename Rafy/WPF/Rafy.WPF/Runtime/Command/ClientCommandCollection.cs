﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20120414
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20120414
 * 
*******************************************************/

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections.ObjectModel;
using Rafy;

namespace Rafy.WPF.Command
{
    /// <summary>
    /// LogicalView 中的 ClientCommand 集合
    /// </summary>
    public class ClientCommandCollection : SealableCollection<ClientCommand>
    {
        /// <summary>
        /// 找到某个类型的运行时命令
        /// </summary>
        /// <typeparam name="TCommandType"></typeparam>
        /// <returns></returns>
        public TCommandType Find<TCommandType>()
            where TCommandType : ClientCommand
        {
            var cmd = this.FirstOrDefault(c => c is TCommandType);
            return cmd as TCommandType;
        }

        /// <summary>
        /// 找到某个类型的运行时命令
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public ClientCommand Find(Type commandType)
        {
            var cmd = this.FirstOrDefault(c => commandType.IsInstanceOfType(c));
            return cmd;
        }

        /// <summary>
        /// 找到某个类型的运行时命令
        /// </summary>
        /// <param name="commandType"></param>
        /// <returns></returns>
        public ClientCommand this[Type commandType]
        {
            get
            {
                var c = this.Find(commandType);
                if (c == null) throw new InvalidOperationException("该视图不存在这个命令：" + commandType.FullName);
                return c;
            }
        }
    }
}