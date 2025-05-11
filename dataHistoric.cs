using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Data.SqlClient;
using System.Data;
using System.Data.SqlTypes;
using System.Net.Http;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Text;

namespace NBAdbToolboxHistoric
{
    public class DataHistoric
    {
        public async Task<Root> ReadFile(int season, int iterations, string filePath)
        {
            // Pre-allocate close to the expected size (300MB)
            // This prevents multiple resize operations when dealing with large files
            StringBuilder seasonFileBuilder = new StringBuilder(320 * 1024 * 1024);

            try
            {
                for (int i = 0; i < iterations; i++)
                {
                    string path = Path.Combine(filePath, $"{season}p{i}.json");

                    // Use Task.Run to run synchronous File.ReadAllText on a background thread
                    string seasonFilePart = await Task.Run(() => File.ReadAllText(path));
                    seasonFileBuilder.Append(seasonFilePart);
                }
                //Parse the complete JSON in one operation
                return await Task.Run(() => JsonConvert.DeserializeObject<Root>(seasonFileBuilder.ToString()));
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error reading season {season}: {ex.Message}");
                throw; // Re-throw to let caller handle it
            }
        }
    }


    public class Root
    {
        public Season season { get; set; }
    }

    public class Season
    {
        public int season_id { get; set; }
        public Games games { get; set; }
    }

    public class Games
    {
        public List<Game> regularSeason { get; set; }
        public List<Game> playoffs { get; set; }
    }

    public class Game
    {
        public string game_id { get; set; }
        public PlayByPlay playByPlay { get; set; }
        public Box box { get; set; }
    }

    public class PlayByPlay
    {
        public string gameId { get; set; }
        public int videoAvailable { get; set; }
        public List<Action> actions { get; set; }
    }

    public class Action
    {
        public int actionNumber { get; set; }
        public string clock { get; set; }
        public int period { get; set; }
        public long teamId { get; set; }
        public string teamTricode { get; set; }
        public long personId { get; set; }
        public string playerName { get; set; }
        public string playerNameI { get; set; }
        public int xLegacy { get; set; }
        public int yLegacy { get; set; }
        public int shotDistance { get; set; }
        public string shotResult { get; set; }
        public int isFieldGoal { get; set; }
        public string scoreHome { get; set; }
        public string scoreAway { get; set; }
        public int pointsTotal { get; set; }
        public string location { get; set; }
        public string description { get; set; }
        public string actionType { get; set; }
        public string subType { get; set; }
        public int videoAvailable { get; set; }
        public int shotValue { get; set; }
        public int actionId { get; set; }
    }

    public class Box
    {
        public string gameId { get; set; }
        public string gameCode { get; set; }
        public int gameStatus { get; set; }
        public string gameStatusText { get; set; }
        public int period { get; set; }
        public string gameClock { get; set; }
        public string gameTimeUTC { get; set; }
        public string gameEt { get; set; }
        public int awayTeamId { get; set; }
        public int homeTeamId { get; set; }
        public string duration { get; set; }
        public int attendance { get; set; }
        public int sellout { get; set; }
        public string seriesGameNumber { get; set; }
        public string gameLabel { get; set; }
        public string gameSubLabel { get; set; }
        public string seriesText { get; set; }
        public string ifNecessary { get; set; }
        public bool? isNeutral { get; set; }
        public Arena arena { get; set; }
        public List<Official> officials { get; set; }
        public Broadcasters broadcasters { get; set; }
        public Team homeTeam { get; set; }
        public Team awayTeam { get; set; }
        public List<Player> homeTeamPlayers { get; set; }
        public List<Player> awayTeamPlayers { get; set; }

    }

    public class Arena
    {
        public int arenaId { get; set; }
        public string arenaName { get; set; }
        public string arenaCity { get; set; }
        public string arenaState { get; set; }
        public string arenaCountry { get; set; }
        public string arenaTimezone { get; set; }
        public string arenaStreetAddress { get; set; }
        public string arenaPostalCode { get; set; }
    }

    public class Official
    {
        public int personId { get; set; }
        public string name { get; set; }
        public int jerseyNum { get; set; }
    }

    public class Broadcasters
    {
        public List<Broadcaster> nationalBroadcasters { get; set; }
        public List<Broadcaster> nationalRadioBroadcasters { get; set; }
        public List<Broadcaster> nationalOttBroadcasters { get; set; }
        public List<Broadcaster> homeTvBroadcasters { get; set; }
        public List<Broadcaster> homeRadioBroadcasters { get; set; }
        public List<Broadcaster> HomeOttBroadcasters { get; set; }
        public List<Broadcaster> awayTvBroadcasters { get; set; }
        public List<Broadcaster> awayRadioBroadcasters { get; set; }
        public List<Broadcaster> awayOttBroadcasters { get; set; }
    }

    public class Broadcaster
    {
        public int broadcasterId { get; set; }
        public string broadcastDisplay { get; set; }
        public string broadcasterDisplay { get; set; }
        public string broadcasterVideoLink { get; set; }
        public string broadcasterDescription { get; set; }
        public int broadcasterTeamId { get; set; }
    }

    public class Team
    {
        public int teamId { get; set; }
        public string teamName { get; set; }
        public string teamCity { get; set; }
        public string teamTricode { get; set; }
        public string teamSlug { get; set; }
        public int teamWins { get; set; }
        public int teamLosses { get; set; }
        public int score { get; set; }
        public string inBonus { get; set; }
        public int timeoutsRemaining { get; set; }
        public int seed { get; set; }

        public List<Period> periods { get; set; }
        public Statistics statistics { get; set; }
        public List<Player> players { get; set; }
        public List<Inactive> inactives { get; set; }
        public List<Lineups> lineups { get; set; }
    }

    public class Period
    {
        public int period { get; set; }
        public string periodType { get; set; }
        public int score { get; set; }
    }
    public class HomeTeamPlayers
    {
        public List<Player> players;
    }
    public class AwayTeamPlayers
    {
        public List<Player> players;
    }

    public class Statistics
    {
        public string minutes { get; set; }
        public int fieldGoalsMade { get; set; }
        public int fieldGoalsAttempted { get; set; }
        public double fieldGoalsPercentage { get; set; }
        public int threePointersMade { get; set; }
        public int threePointersAttempted { get; set; }
        public double threePointersPercentage { get; set; }
        public int freeThrowsMade { get; set; }
        public int freeThrowsAttempted { get; set; }
        public double freeThrowsPercentage { get; set; }
        public int reboundsOffensive { get; set; }
        public int reboundsDefensive { get; set; }
        public int reboundsTotal { get; set; }
        public int assists { get; set; }
        public int steals { get; set; }
        public int blocks { get; set; }
        public int turnovers { get; set; }
        public int foulsPersonal { get; set; }
        public int points { get; set; }
        public int plusMinusPoints { get; set; }
    }

    public class Player
    {
        public int personId { get; set; }
        public string firstName { get; set; }
        public string familyName { get; set; }
        public string nameI { get; set; }
        public string playerSlug { get; set; }
        public string position { get; set; }
        public string comment { get; set; }
        public string jerseyNum { get; set; }
        public Statistics statistics { get; set; }
    }
    public class Inactive
    {
        public int personId { get; set; }
        public string firstName { get; set; }
        public string familyName { get; set; }
        public string jerseyNum { get; set; }
    }

    public class PlayerStats
    {
        public int points { get; set; }
        public int rebounds { get; set; }
        public int assists { get; set; }
        public int steals { get; set; }
        public int blocks { get; set; }
        public int fieldGoalsMade { get; set; }
        public int fieldGoalsAttempted { get; set; }
        public int threePointersMade { get; set; }
        public int threePointersAttempted { get; set; }
        public int freeThrowsMade { get; set; }
        public int freeThrowsAttempted { get; set; }
        public int plusMinus { get; set; }
        public string minutes { get; set; }
    }

    public class Lineups
    {
        public string unit { get; set; }
        public string minutes { get; set; }
        public int fieldGoalsMade { get; set; }
        public int fieldGoalsAttempted { get; set; }
        public double fieldGoalsPercentage { get; set; }
        public int threePointersMade { get; set; }
        public int threePointersAttempted { get; set; }
        public double threePointersPercentage { get; set; }
        public int freeThrowsMade { get; set; }
        public int freeThrowsAttempted { get; set; }
        public double freeThrowsPercentage { get; set; }
        public int reboundsOffensive { get; set; }
        public int reboundsDefensive { get; set; }
        public int reboundsTotal { get; set; }
        public int assists { get; set; }
        public int steals { get; set; }
        public int blocks { get; set; }
        public int turnovers { get; set; }
        public int foulsPersonal { get; set; }
        public int points { get; set; }
    }
}
