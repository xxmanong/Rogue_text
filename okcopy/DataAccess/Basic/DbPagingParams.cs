using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okcopy.DataAccess
{
    /// <summary>
    /// 分页参数
    /// </summary>
    public class DbPagingParams
    {

        public DbPagingParams()
        {
            // defaults;
            PageIndex = 0;
            PageSize = 20;
            SortOrder = "asc";
        }

        /// <summary>
        /// 
        /// </summary>
        public string SqlString { get; set; }
        /// <summary>
        /// 页码
        /// </summary>
        public int PageIndex { get; set; }
        /// <summary>
        /// 每页显示行数
        /// </summary>
        public int PageSize { get; set; }
        /// <summary>
        /// 排序字段
        /// </summary>
        public string SortField { get; set; }
        /// <summary>
        /// 排序方式(asc/desc)
        /// </summary>
        public string SortOrder { get; set; }
        /// <summary>
        /// 汇总字段
        /// </summary>
        public string[] SumamryFields { get; set; }

    }
}
