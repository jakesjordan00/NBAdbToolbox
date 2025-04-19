using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace NBAdbToolboxCurrent
{
    public class DataCurrent
    {
        public async Task<Root>GetJSON(int GameID, int season)
        {
            string boxLink = "https://cdn.nba.com/static/json/liveData/boxscore/boxscore_00" + GameID + ".json";
            string json = "";
            Root root = new Root();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
                client.Timeout = TimeSpan.FromSeconds(3);

                try
                {
                    json = await client.GetStringAsync(boxLink);
                    json = json.Replace("None", "null");
                    root = JsonConvert.DeserializeObject<Root>(json);
                }
                catch (HttpRequestException ex)
                {
                    
                }
                catch(NullReferenceException nah)
                {

                }
            }
            return root;
        }
    }
    public class Root
    {
        public Meta meta { get; set; }
        public Game game { get; set; }
    }

    public class Meta
    {
        public int version { get; set; }
        public int code { get; set; }
        public string request { get; set; }
        public string time { get; set; }
    }

    public class Game
    {
        public string gameId { get; set; }
        public string gameTimeLocal { get; set; }
        public string gameTimeUTC { get; set; }
        public string gameTimeHome { get; set; }
        public string gameTimeAway { get; set; }
        public string gameEt { get; set; }
        public int duration { get; set; }
        public string gameCode { get; set; }
        public string gameStatusText { get; set; }
        //public int gameStatus { get; set; }
        public int regulationPeriods { get; set; }
        public int period { get; set; }
        public string gameClock { get; set; }
        public int attendance { get; set; }
        public string sellout { get; set; }
        public Arena arena { get; set; }
        public List<Official> officials { get; set; }
        public Team homeTeam { get; set; }
        public Team awayTeam { get; set; }
    }

    public class Arena
    {
        public int arenaId { get; set; }
        public string arenaName { get; set; }
        public string arenaCity { get; set; }
        public string arenaState { get; set; }
        public string arenaCountry { get; set; }
        public string arenaTimezone { get; set; }
    }

    public class Official
    {
        public int personId { get; set; }
        public string name { get; set; }
        public string nameI { get; set; }
        public string firstName { get; set; }
        public string familyName { get; set; }
        public string jerseyNum { get; set; }
        public string assignment { get; set; }
    }

    public class Team
    {
        public int teamId { get; set; }
        public string teamName { get; set; }
        public string teamCity { get; set; }
        public string teamTricode { get; set; }
        public int score { get; set; }
        public string inBonus { get; set; }
        public int timeoutsRemaining { get; set; }
        public List<Period> periods { get; set; }
        public List<Player> players { get; set; }
        public Statistics statistics { get; set; }
    }

    public class Period
    {
        public int period { get; set; }
        public string periodType { get; set; }
        public int score { get; set; }
    }

    public class Player
    {
        public string status { get; set; }
        public int order { get; set; }
        public int personId { get; set; }
        public string jerseyNum { get; set; }
        public string position { get; set; }
        public string starter { get; set; }
        public string oncourt { get; set; }
        public string played { get; set; }
        public Statistics statistics { get; set; }
        public string notPlayingReason { get; set; }
        public string notPlayingDescription { get; set; }
        public string name { get; set; }
        public string nameI { get; set; }
        public string firstName { get; set; }
        public string familyName { get; set; }
    }

    public class Statistics
    {
        public int assists { get; set; }
        public double assistsTurnoverRatio { get; set; }
        public int benchPoints { get; set; }
        public int biggestLead { get; set; }
        public string biggestLeadScore { get; set; }
        public int biggestScoringRun { get; set; }
        public string biggestScoringRunScore { get; set; }
        public int blocks { get; set; }
        public int blocksReceived { get; set; }
        public int fastBreakPointsAttempted { get; set; }
        public int fastBreakPointsMade { get; set; }
        public double fastBreakPointsPercentage { get; set; }
        public int fieldGoalsAttempted { get; set; }
        public double fieldGoalsEffectiveAdjusted { get; set; }
        public int fieldGoalsMade { get; set; }
        public double fieldGoalsPercentage { get; set; }
        public int foulsOffensive { get; set; }
        public int foulsDrawn { get; set; }
        public int foulsPersonal { get; set; }
        public int foulsTeam { get; set; }
        public int foulsTechnical { get; set; }
        public int foulsTeamTechnical { get; set; }
        public int freeThrowsAttempted { get; set; }
        public int freeThrowsMade { get; set; }
        public double freeThrowsPercentage { get; set; }
        public int leadChanges { get; set; }
        public string minutes { get; set; }
        public string minutesCalculated { get; set; }
        public double minus { get; set; }
        public double plus { get; set; }
        public double plusMinusPoints { get; set; }
        public int points { get; set; }
        public int pointsAgainst { get; set; }
        public int pointsFastBreak { get; set; }
        public int pointsFromTurnovers { get; set; }
        public int pointsInThePaint { get; set; }
        public int pointsInThePaintAttempted { get; set; }
        public int pointsInThePaintMade { get; set; }
        public double pointsInThePaintPercentage { get; set; }
        public int pointsSecondChance { get; set; }
        public int reboundsDefensive { get; set; }
        public int reboundsOffensive { get; set; }
        public int reboundsPersonal { get; set; }
        public int reboundsTeam { get; set; }
        public int reboundsTeamDefensive { get; set; }
        public int reboundsTeamOffensive { get; set; }
        public int reboundsTotal { get; set; }
        public int secondChancePointsAttempted { get; set; }
        public int secondChancePointsMade { get; set; }
        public double secondChancePointsPercentage { get; set; }
        public int steals { get; set; }
        public int teamFieldGoalAttempts { get; set; }
        public int threePointersAttempted { get; set; }
        public int threePointersMade { get; set; }
        public double threePointersPercentage { get; set; }
        public string timeLeading { get; set; }
        public int timesTied { get; set; }
        public double trueShootingAttempts { get; set; }
        public double trueShootingPercentage { get; set; }
        public int turnovers { get; set; }
        public int turnoversTeam { get; set; }
        public int turnoversTotal { get; set; }
        public int twoPointersAttempted { get; set; }
        public int twoPointersMade { get; set; }
        public double twoPointersPercentage { get; set; }
    }

}
