using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SP.PowerBI.Interfaces
{
    public interface IExcelService
    {
        List<List<object>> TableToEntities(string space, DataTable dt);
        List<object> TableToEntity(DataTable dt, Type entityType);
        DataTable ExcelToDataTable(string sheetName = null);
    }
}
