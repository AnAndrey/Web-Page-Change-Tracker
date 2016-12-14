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
    public class SqlDataFetcher : IDataFetcher
    {
        string _rootTableName = String.Empty;
        SqlRepository _repository = new SqlRepository("");

        public SqlDataFetcher(string rootTableName)
        {
            _rootTableName = rootTableName;

        }
        public IEnumerable<ChangeableData> GetData()
        {
            IEnumerable<ChangeableData> data = null;
            string commandFormat = "SELECT * FROM [{0}]";
            DataTable table = _repository.GetDataSet(String.Format(commandFormat, typeof(CourtRegion).Name));
            if (IsValidChangeableDataInTable(table))
            {
                data = table.Rows.Cast<DataRow>().Select<DataRow, ChangeableData>(x =>
                {
                    return new CourtRegion(x["Value"].ToString(), x["Name"].ToString()) { Childs = GetDistricts(x["id"].ToString()) };
                });
            }
            return data;
        }
        private IEnumerable<ChangeableData> GetDistricts(string parent_id)
        {
            IEnumerable<ChangeableData> data = null;

            string commandFormat = "SELECT * FROM [{0}] WHERE Parent_id='{1}'";
            DataTable table = _repository.GetDataSet(String.Format(commandFormat, typeof(CourtDistrcits).Name, parent_id));
            if (IsValidChangeableDataInTable(table))
            {
                data = table.Rows.Cast<DataRow>().Select<DataRow, ChangeableData>(x =>
                {
                    return new CourtRegion(x["Value"].ToString(), x["Name"].ToString()) { Childs = GetLocation(x["id"].ToString()) };
                });
            }
            return data;
        }

        private IEnumerable<ChangeableData> GetLocation(string parent_id)
        {
            IEnumerable<ChangeableData> data = null;

            string commandFormat = "SELECT * FROM [{0}] WHERE Parent_id='{1}'";
            DataTable table = _repository.GetDataSet(String.Format(commandFormat, typeof(CourtLocation).Name, parent_id));
            if (IsValidChangeableDataInTable(table))
            {
                data = table.Rows.Cast<DataRow>().Select<DataRow, ChangeableData>(x =>
                {
                    return new CourtRegion(x["Value"].ToString(), x["Name"].ToString()) { Childs = null };
                });
            }
            return data;
        }

        private bool IsValidChangeableDataInTable(DataTable table)
        {
            if (table == null || table.HasErrors)
            {
#warning Change exceptions to custom ones

                Console.WriteLine("HasErrors");
                throw new Exception("HasErrors");

            }
            if (!table.Columns.Contains("Name"))
            {
                Console.WriteLine("Unknown table structure, noname");

                throw new Exception("Unknown table structure, noname");
            }
            if (!table.Columns.Contains("Value"))
            {
                Console.WriteLine("Unknown table structure, no Value");

                throw new Exception("Unknown table structure, no Value");
            }
            return true;
        }


       
    }

    internal static class DataTableExtensions
    {
        internal static IEnumerable<ChangeableData> TopChangableDataEnumarable(this DataTable table)
        {

        }
    }
}
