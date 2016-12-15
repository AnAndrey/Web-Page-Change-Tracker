using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SharedInterfaces;
using System.Data;
using System.Data.SqlClient;

namespace GenesisTrialTest
{
    public class SqlRepository: IRepository
    {
        string ConnectionString;
        public SqlRepository(string connectionString)
        {
            ConnectionString = connectionString;
            ConnectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\dotNet\projects\Practice\Genesis\GenesisTrialTest-Copy(2)\GenesisTrialTest\DB\LocalDatabase.mdf;Integrated Security=True";
        }
        public DataTable GetDataSet(string sqlCommand)
        {
            
            //string connectionString = @"Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=|DataDirectory|\DB\LocalDatabase.mdf;Integrated Security=True";
            using (var connection = new SqlConnection(ConnectionString))
            {
                connection.Open();
                //string commandFormat = "SELECT * FROM [{0}] WHERE Parent_id='{1}'";
                using (SqlDataAdapter adapter = new SqlDataAdapter(sqlCommand, connection))
                {
                    var table = new DataTable();

                    adapter.Fill(table);
                    return table;
                }
            }
        }
    }
}
