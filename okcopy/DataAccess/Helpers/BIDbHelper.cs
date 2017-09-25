using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace okcopy.DataAccess
{

    /// <summary>
    /// 数据分析帮助类
    /// </summary>
    public class BIDbHelper : SqlHelper
    {

        protected override DbConnection CreateConnection()
        {
            var sr = new System.Configuration.AppSettingsReader();

            var str = sr.GetValue("bidb", typeof(string)) as String;
            if (str == null) throw new Exception("缺少mssql:bi连接字符窜");

            return CreateConnection(DbContextType.MSSql, str);
        }
    }

}
