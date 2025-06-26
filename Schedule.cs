using NBAdbToolboxHistoric;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NBAdbToolboxSchedule
{
    public class Schedule
    {
        //public async Task<List<Game>> GetJSON(DateTime lastDate)
        public async Task<List<Game>> GetJSON(List<int> gameList)
        {
            string pbpLink = "https://cdn.nba.com/static/json/staticData/scheduleLeagueV2_1.json";
            string json = "";
            ScheduleLeagueV2 Schedule = new ScheduleLeagueV2();
            List<Game> GameList = new List<Game>();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
                client.Timeout = TimeSpan.FromSeconds(3.5);
                try
                {
                    json = await client.GetStringAsync(pbpLink);
                    Schedule = JsonConvert.DeserializeObject<ScheduleLeagueV2>(json);
                }
                catch
                {

                }

            }
            foreach (GameDates date in Schedule.LeagueSchedule.GameDates)
            {
                foreach(Game game in date.Games)
                {
                    //if(game.GameDateTimeEst.Date >= lastDate.Date && game.GameDateTimeEst <= DateTime.Now)
                    //{
                    //    GameList.Add(game);
                    //}
                    int gameId = int.Parse(game.GameId);
                    if (!gameList.Contains(Int32.Parse(game.GameId)) && game.GameId.Substring(2, 1) != "1" && game.GameId.Substring(2, 1) != "3" && game.GameDateTimeEst <= DateTime.Now)
                    {
                        GameList.Add(game);
                    }
                    GameList = GameList.OrderBy(g => Int32.Parse(g.GameId)).ToList();
                }
            }
            return GameList;
        }
    }

    public class Meta
    {
        public int Version { get; set; }
        public string Request { get; set; }
        public DateTime Time { get; set; }
    }

    public class Broadcaster
    {
        public string BroadcasterScope { get; set; }
        public string BroadcasterMedia { get; set; }
        public int BroadcasterId { get; set; }
        public string BroadcasterDisplay { get; set; }
        public string BroadcasterAbbreviation { get; set; }
        public string BroadcasterDescription { get; set; }
        public string TapeDelayComments { get; set; }
        public string BroadcasterVideoLink { get; set; }
        public int RegionId { get; set; }
        public int BroadcasterTeamId { get; set; }
    }

    public class Broadcasters
    {
        public List<Broadcaster> NationalTvBroadcasters { get; set; }
        public List<Broadcaster> NationalRadioBroadcasters { get; set; }
        public List<Broadcaster> NationalOttBroadcasters { get; set; }
        public List<Broadcaster> HomeTvBroadcasters { get; set; }
        public List<Broadcaster> HomeRadioBroadcasters { get; set; }
        public List<Broadcaster> HomeOttBroadcasters { get; set; }
        public List<Broadcaster> AwayTvBroadcasters { get; set; }
        public List<Broadcaster> AwayRadioBroadcasters { get; set; }
        public List<Broadcaster> AwayOttBroadcasters { get; set; }
        public List<Broadcaster> IntlRadioBroadcasters { get; set; }
        public List<Broadcaster> IntlTvBroadcasters { get; set; }
        public List<Broadcaster> IntlOttBroadcasters { get; set; }
    }

    public class Team
    {
        public int TeamId { get; set; }
        public string TeamName { get; set; }
        public string TeamCity { get; set; }
        public string TeamTricode { get; set; }
        public string TeamSlug { get; set; }
        public int Wins { get; set; }
        public int Losses { get; set; }
        public int Score { get; set; }
        public int Seed { get; set; }
    }

    public class PointsLeader
    {
        public int PersonId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public int TeamId { get; set; }
        public string TeamCity { get; set; }
        public string TeamName { get; set; }
        public string TeamTricode { get; set; }
        public double Points { get; set; }
    }

    public class Game
    {
        public string GameId { get; set; }
        public string GameCode { get; set; }
        public int GameStatus { get; set; }
        public string GameStatusText { get; set; }
        public int GameSequence { get; set; }
        public DateTime GameDateEst { get; set; }
        public DateTime GameTimeEst { get; set; }
        public DateTime GameDateTimeEst { get; set; }
        public DateTime GameDateUTC { get; set; }
        public DateTime GameTimeUTC { get; set; }
        public DateTime GameDateTimeUTC { get; set; }
        public DateTime AwayTeamTime { get; set; }
        public DateTime HomeTeamTime { get; set; }
        public string Day { get; set; }
        public int MonthNum { get; set; }
        public int WeekNumber { get; set; }
        public string WeekName { get; set; }
        public bool IfNecessary { get; set; }
        public string SeriesGameNumber { get; set; }
        public string GameLabel { get; set; }
        public string GameSubLabel { get; set; }
        public string SeriesText { get; set; }
        public string ArenaName { get; set; }
        public string ArenaState { get; set; }
        public string ArenaCity { get; set; }
        public string PostponedStatus { get; set; }
        public string BranchLink { get; set; }
        public string GameSubtype { get; set; }
        public Broadcasters Broadcasters { get; set; }
        public Team HomeTeam { get; set; }
        public Team AwayTeam { get; set; }
        public List<PointsLeader> PointsLeaders { get; set; }
    }

    public class GameDates
    {
        public string GameDate { get; set; }
        public List<Game> Games { get; set; }
    }

    public class LeagueSchedule
    {
        public string SeasonYear { get; set; }
        public string LeagueId { get; set; }
        public List<GameDates> GameDates { get; set; }
    }

    public class ScheduleLeagueV2
    {
        public Meta Meta { get; set; }
        public LeagueSchedule LeagueSchedule { get; set; }
    }
}
