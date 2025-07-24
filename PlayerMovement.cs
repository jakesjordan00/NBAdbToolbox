using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NBAdbToolboxPlayerMovement
{
    public class PlayerMovement
    {
        public int count = 0;
        public TimeSpan requestDuration = TimeSpan.Zero;
        public float AvgDuration = 0;

        public async Task<PlayerMovementRoot> GetPlayerMovementAsync()
        {
            PlayerMovementRoot root = null;
            string url = "https://stats.nba.com/js/data/playermovement/NBA_Player_Movement.json";

            var stopwatch = Stopwatch.StartNew();

            using (HttpClient client = new HttpClient())
            {
                try
                {
                    var response = await client.GetStringAsync(url);
                    root = JsonConvert.DeserializeObject<PlayerMovementRoot>(response);
                }
                catch (Exception ex)
                {
                    ErrorOutput(ex);
                }
            }

            return root;
        }

        public void ErrorOutput(Exception e)
        {
            var st = new System.Diagnostics.StackTrace(e, true);
            var frame = st.GetFrame(0);

            if (frame != null)
            {
                string fileName = frame.GetFileName();
                int lineNumber = frame.GetFileLineNumber();
                string methodName = frame.GetMethod().Name;

                Console.WriteLine($"Exception in file: {fileName}");
                Console.WriteLine($"Method: {methodName}");
                Console.WriteLine($"Line: {lineNumber}");
            }

            Console.WriteLine($"Error message: {e.Message}");
            Console.WriteLine($"Stack trace: {e.StackTrace}");

            if (e.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {e.InnerException.Message}");
            }
        }

    }

    public class PlayerMovementRoot
    {
        public NBAPlayerMovement NBA_Player_Movement { get; set; }
    }

    public class NBAPlayerMovement
    {
        public List<Transaction> rows { get; set; }
        public List<Column> columns { get; set; }
    }

    public class Transaction
    {
        public string Transaction_Type { get; set; }
        public DateTime TRANSACTION_DATE { get; set; }
        public string TRANSACTION_DESCRIPTION { get; set; }
        public long TEAM_ID { get; set; }
        public string TEAM_SLUG { get; set; }
        public long PLAYER_ID { get; set; }
        public string PLAYER_SLUG { get; set; }
        public long Additional_Sort { get; set; }
        public string GroupSort { get; set; }
    }

    public class Column
    {
        public string Name { get; set; }
        public string DataType { get; set; }
    }
}
