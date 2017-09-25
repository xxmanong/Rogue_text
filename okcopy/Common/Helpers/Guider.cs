using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okcopy.Common
{
    /// <summary>
    /// GUID帮助类
    /// </summary>
    public class Guider
    {
        /// <summary>
        /// 创建GUID字符串（去横线并转大写）
        /// </summary>
        /// <returns>GUID字符串（去横线并转大写）</returns>
        public static string Create()
        {
            return Guid.NewGuid().ToString().Replace("-", "").ToUpper();
        }
        /// <summary>
        /// 取行数据，进行条件查询
        /// </summary>
        /// <param name="dt"></param>
        /// <param name="condition"></param>
        /// <returns></returns>
        public static string conString(DataTable dt, string condition)
        {
            List<string> ls = new List<string>();  //存放你一整列所有的值 
            string str = string.Empty;           //存放组合成的查询条件(说说ID)
            foreach (DataRow dr in dt.Rows)
            {
                ls.Add(dr[condition].ToString());
            }
            for (int i = 0; i < ls.Count(); i++)
            {
                if (i == ls.Count() - 1)
                {
                    str += ls[i];
                }
                else
                {
                    str += ls[i] + ",";
                }
            }

            return str;
        }
    }
}
