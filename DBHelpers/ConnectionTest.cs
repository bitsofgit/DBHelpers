using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Diagnostics;
using System.Threading;
using System.Data.SqlClient;
using System.IO;
using System.Net;
using System.Runtime.CompilerServices;
using System.Timers;

namespace DBHelpers
{
    public class ConnectionInfo : IConnectionInfo
    {
        public string CommandText { get; set; }
        public string ConnectionString { get; set; }
        public long MSToOpenConnection { get; set; }
        public long MSToExecute { get; set; }
        public long MSTotalTime { get; set; }
        public string Result { get; set; }
        public string ErrorMessage { get; set; }
    }

    public interface IConnectionInfo
    {
        string CommandText { get; set; }
        string ConnectionString { get; set; }
        long MSToOpenConnection { get; set; }
        long MSToExecute { get; set; }
        long MSTotalTime { get; set; }
        string Result { get; set; }
        string ErrorMessage { get; set; }
    }

    public class ConnectionTest
    {
        public IConnectionInfo ExecuteScalar(IConnectionInfo info)
        {
            var sw = new Stopwatch();
            sw.Start();
            try
            {
                var swConn = new Stopwatch();
                swConn.Start();
                using (var conn = new SqlConnection(info.ConnectionString))
                {
                    conn.Open();
                    swConn.Stop();
                    info.MSToOpenConnection = swConn.ElapsedMilliseconds;

                    var cmd = new SqlCommand(info.CommandText, conn) { CommandType = CommandType.Text };

                    var swExec = new Stopwatch();
                    swExec.Start();
                    info.Result = cmd.ExecuteScalar().ToString();
                    swExec.Stop();
                    info.MSToExecute = swExec.ElapsedMilliseconds;
                }
            }
            catch (Exception ex)
            {
                info.ErrorMessage = ex.Message;
            }
            sw.Stop();
            info.MSTotalTime = sw.ElapsedMilliseconds;

            return info;
        }
    }
}
