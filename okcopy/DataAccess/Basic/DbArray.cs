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
    [Serializable]
    public class DbArray : List<DbObject> 
    {
        /// <summary>
        /// 
        /// </summary>
        public DbArray()
            : base() { }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="items"></param>
        public DbArray(IEnumerable<DbObject> items)
            : base(items) { }
         
    }
}
