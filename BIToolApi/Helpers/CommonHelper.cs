using MimeKit;
using MySqlConnector;
using System.ComponentModel;
using System.Data;

namespace BITool.Helpers
{
    public static class CommonHelper
    {
        public static DataTable ToDataTable<T>(this IEnumerable<T> data)
        {
            PropertyDescriptorCollection properties = TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        public static IEnumerable<MySqlBulkCopyColumnMapping> GetMySqlColumnMapping(this DataTable dataTable)
        {
            List<MySqlBulkCopyColumnMapping> colMappings = new List<MySqlBulkCopyColumnMapping>();
            int i = 0;
            foreach (DataColumn col in dataTable.Columns)
            {
                colMappings.Add(new MySqlBulkCopyColumnMapping(i, col.ColumnName));
                i++;
            }
            return colMappings;
        }

        public static bool IsNullOrEmpty(this string input) => string.IsNullOrEmpty(input);

        public static bool IsNotNullOrEmpty(this string input) => !string.IsNullOrEmpty(input);

        public static List<MailboxAddress> ToMailAddresses(this string input, string delimeter = ",")
        {
            if (input.IsNullOrEmpty())
                return new List<MailboxAddress>();
            return input.Split(delimeter, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => MailboxAddress.Parse(p))
                .ToList();
        }

        public static T ConvertFromDBVal<T>(object obj)
        {
            if (obj == null || obj == DBNull.Value)
            {
                return default(T); // returns the default value for the type
            }
            else
            {
                return (T)obj;
            }
        }

        public static string GetFileContentType(this string extension)
        {
            return extension switch
            {
                ".pdf" => "application/pdf",
                ".txt" => "text/plain",
                ".bmb" => "image/bmb",
                ".svg" => "image/svg+xml",
                ".gif" => "image/gif",
                ".png" => "image/png",
                ".jpg" => "image/jpg",
                ".jpeg" => "image/jpeg",
                ".webp" => "image/webp",
                ".xls" => "application/vnd.ms-excel",
                ".xlsx" => "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                ".csv" => "text/csv",
                ".html" => "text/html",
                ".xml" => "text/xml",
                ".zip" => "text/xml",
                _ => "application/octet-stream"
            };
        }
    }
}