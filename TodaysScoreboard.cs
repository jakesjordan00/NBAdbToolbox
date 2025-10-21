using NBAdbToolboxSchedule;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NBAdbToolboxTodaysScoreboard
{
    public class TodaysScoreboard
    {

        public async Task<List<Game>> GetScoreboard()
        {
            //If Version = 1, 2024
            //If Version = 2, 2025
            string pbpLink = "https://cdn.nba.com/static/json/liveData/scoreboard/todaysScoreboard_00.json";
            string json = "";
            Root Scoreboard = new Root();
            List<Game> GameList = new List<Game>();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
                client.Timeout = TimeSpan.FromSeconds(3.5);
                try
                {
                    json = await client.GetStringAsync(pbpLink);
                    Scoreboard = JsonConvert.DeserializeObject<Root>(json);
                }
                catch
                {
                    Scoreboard = null;
                }

            }
            List<NBAdbToolboxTodaysScoreboard.Game> Games = new List<Game>();
            foreach(NBAdbToolboxTodaysScoreboard.Game game in Scoreboard.scoreboard.Games)
            {
                if(game.Period != 0)
                {
                    Games.Add(game);
                }
            }
            return Games;
        }
    }

    public class Root
    {
        public Meta Meta { get; set; }
        public Scoreboard scoreboard { get; set; }
    }

    public class Scoreboard
    {
        public List<Game> Games { get; set; }
    }

    public class Game
    {
        public string GameId { get; set; }
        public int Period { get; set; }
        public string GameClock { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
    }
    public class Team
    {
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Score { get; set; }
    }

}
