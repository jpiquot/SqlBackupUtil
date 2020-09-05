using System.Collections.Generic;
using System.Data;
using System.IO;

namespace SqlBackup
{
    public static class DataRowHelper
    {
        public static Dictionary<string, object> ToDictionary(this DataRow row)
        {
            var values = new Dictionary<string, object>();
            if (row != null)
            {
                foreach (DataColumn? column in row.Table.Columns)
                {
                    if (column == null)
                    {
                        throw new InvalidDataException(Properties.Resources.InvalidColumn);
                    }
                    values.Add(column.ColumnName, row[column.Ordinal]);
                }
            }
            return values;
        }
    }
}