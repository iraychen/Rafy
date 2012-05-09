﻿/*******************************************************
 * 
 * 作者：胡庆访
 * 创建时间：20120425
 * 说明：此文件只包含一个类，具体内容见类型注释。
 * 运行环境：.NET 4.0
 * 版本号：1.0.0
 * 
 * 历史记录：
 * 创建文件 胡庆访 20120425
 * 
*******************************************************/

using System;
using System.Data;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using OEA.ORM;
using System.Data.SqlClient;
using System.Data.Common;
using hxy.Common.Data;
using OEA.Library;
using OEA.MetaModel;

namespace OEA.ORM
{
    public class Db : IDb
    {
        private ConnectionManager _connectionManager;

        /// <summary>
        /// 数制库访问控制器
        /// </summary>
        public IDBAccesser DBA { get; private set; }

        #region 构造器

        /// <summary>
        /// 根据配置文件，构造一个数据库控制器。
        /// </summary>
        /// <param name="connectionStringSettingName"></param>
        /// <returns></returns>
        public static IDb Create(string connectionStringSettingName)
        {
            var setting = DbSetting.FindOrCreate(connectionStringSettingName);
            return new Db(setting);
        }

        /// <summary>
        /// 根据配置文件，构造一个数据库控制器。
        /// </summary>
        /// <param name="connectionStringSettingName"></param>
        /// <returns></returns>
        public static IDb Create(DbSetting dbSetting)
        {
            return new Db(dbSetting);
        }

        private Db(DbSetting dbSetting)
        {
            if (dbSetting == null) throw new ArgumentNullException("dbSetting");

            this._connectionManager = ConnectionManager.GetManager(dbSetting);

            this.DBA = new DBAccesser(
                this._connectionManager.Connection,
                dbSetting.ProviderName
                );
        }

        #endregion

        public int Insert(IEntity item)
        {
            var table = TableFor(item);
            return table.Insert(this, item);
        }

        public int Update(IEntity item)
        {
            var table = TableFor(item);
            return table.Update(this, item);
        }

        public int Delete(IEntity item)
        {
            var table = TableFor(item);
            return table.Delete(this, item);
        }

        public int Delete(Type type, IQuery query)
        {
            var table = TableFor(type);
            return table.Delete(this, query);
        }

        public IList Select(IQuery typedQuery)
        {
            IList list = new ArrayList();
            DoSelect(typedQuery.EntityType, typedQuery, list);
            return list;
        }

        protected virtual void DoSelect(Type type, IQuery query, IList list)
        {
            var table = TableFor(type);
            table.Select(this, query, list);
        }

        public IList Select(Type type, string sql)
        {
            var table = TableFor(type);
            IList list = new ArrayList();
            using (var reader = this.DBA.QueryDataReaderNormal(sql, CommandType.Text))
            {
                table.FillByIndex(reader, list);
            }
            return list;
        }

        public IQuery Query(Type entityType)
        {
            return new DbQuery(entityType);
        }

        private DbTable TableFor(IEntity entity)
        {
            return entity.GetRepository().GetORMTable() as DbTable;
        }

        private DbTable TableFor(Type type)
        {
            return RepositoryFactoryHost.Factory.Create(type).GetORMTable() as DbTable;
        }

        #region Dispose Pattern

        ~Db()
        {
            this.Dispose(false);
        }

        public virtual void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                this._connectionManager.Dispose();
                this.DBA.Dispose();
            }
        }

        #endregion
    }
}