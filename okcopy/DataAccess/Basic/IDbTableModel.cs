using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace okcopy.DataAccess
{

    public interface IDbTableModel
    {
        string GetTableName();
        string[] GetPrimaryKeys();
    }
}
