using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace DBHelpers
{
    public class AGLatencyTest
    {
        public IDBInfo TestLatency(IDBInfo item)
        {
            var result = IsReplicaInSync(item.PrimaryConnStr, item.ReplicaConnStr, item.Query).Result;
            item.InSync = result.InSync;
            item.PrimaryVal = result.PrimaryVal;
            item.ReplicaVal = result.ReplicaVal;

            return item;
        }

        private async Task<(bool InSync, string PrimaryVal, string ReplicaVal)> IsReplicaInSync(string primaryConnStr, string replicaConnStr, string query)
        {
            // make async call at the same time to both dbs
            var primaryTask = GetFromDBAsync(primaryConnStr, query);
            var replicaTask = GetFromDBAsync(replicaConnStr, query);

            // wait here to get both values
            string[] values = await Task.WhenAll(primaryTask, replicaTask);

            // compare dates
            if (string.IsNullOrWhiteSpace(values[0]) || string.IsNullOrWhiteSpace(values[1]))
                throw new Exception("Error getting one of primary or replica value.");
            else
                return (values[0] == values[1], values[0], values[1]);
        }

        private async Task<string> GetFromDBAsync(string connStr, string query)
        {
            Object obj = null;

            using (var conn = new SqlConnection(connStr))
            {
                conn.Open();
                var cmd = new SqlCommand(query, conn) { CommandType = CommandType.Text };
                obj = await cmd.ExecuteScalarAsync();
            }

            if (obj != null)
            {
                return obj.ToString();
            }
            return null;
        }
    }

    public class DBInfo : IDBInfo
    {
        public string PrimaryKey { get; set; }
        public string PrimaryConnStr { get; set; }
        public string ReplicaKey { get; set; }
        public string ReplicaConnStr { get; set; }
        public string Query { get; set; }
        public bool InSync { get; set; }
        public string PrimaryVal { get; set; }
        public string ReplicaVal { get; set; }
    }

    public interface IDBInfo
    {
        string PrimaryKey { get; set; }
        string PrimaryConnStr { get; set; }
        string ReplicaKey { get; set; }
        string ReplicaConnStr { get; set; }
        string Query { get; set; }
        bool InSync { get; set; }
        string PrimaryVal { get; set; }
        string ReplicaVal { get; set; }
    }
}
