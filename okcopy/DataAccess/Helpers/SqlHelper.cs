using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okcopy.DataAccess
{

    public class SqlHelper : DbFactory
    {
        //#region DbName
        ///// <summary>
        ///// 库名
        ///// </summary>
        //public static string DbName
        //{
        //    get { return "Hishop"; }
        //}
        //#endregion

        #region structs

        /// <summary>
        /// 
        /// </summary>
        public SqlHelper()
            : base()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="cnstr"></param>
        public SqlHelper(string cnstr) :
            base(DbContextType.MSSql, cnstr)
        {

        }

        #endregion

        #region override

        protected override DbConnection CreateConnection()
        {
            var sr = new System.Configuration.AppSettingsReader();

            var str = sr.GetValue("mssql", typeof(string)) as String;
            if (str == null) throw new Exception("缺少mssql:ecms连接字符窜");

            return CreateConnection(DbContextType.MSSql, str);
        }

        #endregion

        /// <summary>
        /// 插入或更新
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        public int InsertOrUpdate(IDbTableModel model)
        {
            return InsertOrUpdate(model.GetTableName(), model, model.GetPrimaryKeys());
        }


        /// <summary>
        /// 插入或更新
        /// </summary>
        /// <param name="tableName">表名</param>
        /// <param name="rowData">行数据</param>
        /// <param name="primaryKeys">
        /// 用于比较记录是否存在的字段名称
        /// <para>字段名前面加*号表示该字段值非序列生成，可以加入Insert中</para>
        /// </param>
        /// <returns></returns>
        public int InsertOrUpdate(string tableName, object rowData, params string[] primaryKeys)
        {

            StringBuilder sb = new StringBuilder();

            var aFields = getFieldNames(rowData);
            var mFields = aFields.Except(primaryKeys).ToArray(); // 非主键列

            sb.AppendFormat("MERGE INTO {0} A ", tableName);
            sb.AppendFormat("USING(SELECT {0}) B ", convertFields(aFields, "@{0} {0}"));
            sb.AppendFormat("ON({0}) ", convertFields(primaryKeys, "A.{0}=B.{0}", " AND "));
            sb.Append("WHEN MATCHED THEN ");
            sb.AppendFormat("UPDATE SET {0} ", convertFields(mFields, "A.{0}=B.{0}"));
            sb.Append("WHEN NOT MATCHED THEN ");
            sb.AppendFormat("INSERT ({0}) VALUES({1})", convertFields(mFields), convertFields(mFields, "B.{0}"));
            sb.Append(";");

            return Execute(sb.ToString(), rowData);

        }

        private string[] getFieldNames(object rowData)
        {

            if (rowData is IDictionary)
            {
                List<string> names = new List<string>();
                var em = ((IDictionary)rowData).GetEnumerator();

                while (em.MoveNext())
                {
                    names.Add(Convert.ToString(em.Key));
                }

                return names.ToArray();
            }
            else
            {
                return rowData.GetType().GetProperties()
                    .ToDictionary(p => p.Name).Keys.ToArray();
            }
        }


        private string convertFields(string[] fields, string format = "{0}", string separator = ",")
        {
            return string.Join(separator, fields.ToDictionary(key => string.Format(format, key.TrimStart('*'))).Keys);
        }



    }
}
