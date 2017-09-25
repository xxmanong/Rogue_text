using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okcopy.DataAccess
{


    /// <summary>
    /// 
    /// </summary>
    public class DbPagingResult
    {

        #region properties


        /// <summary>
        /// 
        /// </summary>
        public long total { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbArray data { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public DbObject summary { get; set; }

        #endregion

        #region structs

        /// <summary>
        /// 
        /// </summary>
        public DbPagingResult()
        {

        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="total"></param>
        /// <param name="data"></param>
        public DbPagingResult(long total, DbArray data)
        {
            this.total = total;
            this.data = data;
        }

        #endregion


    }
}
