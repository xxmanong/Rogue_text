using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business
{
    using okcopy.DataAccess;
    using okcopy.Common;

    public  class DataAnlysis
    {

        public DbArray query(string PlatID)
        {
            DbArray arr = null;

            using (var db = new SqlHelper())
            { 
              arr =  db.Query("  select * from OpenOrder where PlatID=@PlatID ", new { PlatID });
            }

            DbObject oj = new DbObject();
             
            return arr;
        }


    }
}
