using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedInterfaces;
using System.Data.SqlClient;
using System.Data;


namespace GenesisTrialTest
{
    public class SqlDataPreserver : IDataPreserver
    {
        IDatabaseStorage _storage;
        public SqlDataPreserver(IDatabaseStorage storage)
        {
            _storage = storage;
        }
        public void Save(IEnumerable<ChangeableData> data)
        {
            SaveData(data);
        }

        private void SaveData(IEnumerable<ChangeableData> data, int? parentId = null)
        {
            if (data != null)
            {
                StoreData(data, parentId);
                data.Where(n => n.HasChilds).All(x =>
                {
                    SaveData(x.Childs, GetRecordId(x));
                    return true;
                });
            }
        }
        private void StoreData(IEnumerable<ChangeableData> data, int ? parentId = null)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\dotNet\projects\Practice\Genesis\GenesisTrialTest\GenesisTrialTest\DB\LocalDatabase.mdf;Integrated Security=True";
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\DB\LocalDatabase.mdf;Integrated Security=True";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                using (SqlTransaction transaction = connection.BeginTransaction())
                {
                    using (var bulkCopy = new SqlBulkCopy(connection, SqlBulkCopyOptions.Default, transaction))
                    {
                        bulkCopy.BatchSize = 1000;
                        bulkCopy.DestinationTableName = "dbo." + data.First().GroupName;
                        bulkCopy.ColumnMappings.Add("Name", "Name");
                        bulkCopy.ColumnMappings.Add("Value", "Value");
                        if(parentId != null)
                            bulkCopy.ColumnMappings.Add("Parent_id", "Parent_id");
                        try
                        {
                            bulkCopy.WriteToServer(data.AsDataTable(parentId));
                        }
                        catch (Exception ex)
                        {
                            transaction.Rollback();
                            connection.Close();
                            return;
                        }
                    }
                    transaction.Commit();
                }
            }
        }

        private int ? GetRecordId(ChangeableData data)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\dotNet\projects\Practice\Genesis\GenesisTrialTest\GenesisTrialTest\DB\LocalDatabase.mdf;Integrated Security=True";
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\DB\LocalDatabase.mdf;Integrated Security=True";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string commandFormat = "SELECT id FROM [{0}] WHERE Value='{1}'";
                using (SqlCommand command = new SqlCommand(String.Format(commandFormat, data.GroupName, data.Value), connection))
                {
                    using (SqlDataReader reader = command.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            Console.WriteLine(String.Format("{0}", reader["id"]));
                            return reader.GetInt32(0);
                        }
                        return null;
                    }
                }
            }

            throw new Exception("refactoring is needed");
        }

        public void ClearTable(string tableName)
        {
            string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\dotNet\projects\Practice\Genesis\GenesisTrialTest\GenesisTrialTest\DB\LocalDatabase.mdf;Integrated Security=True";
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\DB\LocalDatabase.mdf;Integrated Security=True";
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                string commandFormat = "DELETE FROM {0}";
                using (SqlCommand command = new SqlCommand(String.Format(commandFormat, tableName), connection))
                {
                    command.ExecuteNonQuery();
                }
            }
        }
    }

    public static class IEnumerableExtension
    {
        public static DataTable AsDataTable(this IEnumerable<ChangeableData> data, int ? parentId = null) 
        {
            
            var table = new DataTable();

            table.Columns.Add("Name", typeof(string));
            table.Columns.Add("Value", typeof(string));
            if (parentId != null)
                table.Columns.Add("Parent_id", typeof(int));

            foreach (var item in data)
            {
                DataRow row = table.NewRow();
                row["Name"] = item.Name;
                row["Value"] = String.IsNullOrEmpty(item.Value) ? "empty": item.Value;
                if (parentId != null)
                    row["Parent_id"] = parentId;
                table.Rows.Add(row);
            }
            return table;
        }

    
    }
}
