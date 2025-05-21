using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
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
        public int count = 0;
        public TimeSpan requestDuration = TimeSpan.Zero;
        public float AvgDuration = 0;
        public HashSet<int> Missing2019Games = new HashSet<int> {  21900194, 21900200, 21900203, 21900204, 21900208, 21900244, 21900306, 21900307, 21900308, 21900309, 21900310, 21900311, 21900312, 21900313, 21900314, 21900315, 21900316, 21900317, 21900318, 21900319, 21900320, 21900321, 21900322, 21900323, 21900324, 21900325, 21900326, 21900327, 21900328, 21900329, 21900619, 21900668, 21900669, 21900670, 21900671, 21900672, 21900673, 21900674, 21900675, 21900676, 21900677, 21900678, 21900679, 21900680, 21900681, 21900682, 21900683, 21900684, 21900685, 21900686, 21900687, 21900688, 21900689, 21900690, 21900691, 21900692, 21900693, 21900694, 21900695, 21900696, 21900697, 21900698, 21900699, 21900700, 21900701, 21900702, 21900703, 21900704, 21900705, 21900706, 21900708, 21900709, 21900710, 21900711, 21900712, 21900713, 21900714, 21900715, 21900716, 21900717, 21900718, 21900719, 21900721, 21900722, 21900723, 21900724, 21900725, 21900726, 21900727, 21900728, 21900729, 21900730, 21900731, 21900732, 21900733, 21900734, 21900735, 21900736, 21900737, 21900738, 21900739, 21900740, 21900741, 21900742, 21900743, 21900744, 21900745, 21900746, 21900747, 21900748, 21900749, 21900750, 21900751, 21900752, 21900753, 21900754, 21900755, 21900756, 21900757, 21900758, 21900759, 21900760, 21900761, 21900762, 21900763, 21900764, 21900765, 21900766, 21900767, 21900768, 21900769, 21900770, 21900771, 21900772, 21900773, 21900774, 21900775, 21900776, 21900778, 21900779, 21900780, 21900781, 21900782, 21900783, 21900784, 21900785, 21900786, 21900787, 21900788, 21900789, 21900790, 21900791, 21900792, 21900793, 21900794, 21900795, 21900796, 21900797, 21900798, 21900799, 21900800, 21900801, 21900802, 21900803, 21900804, 21900805, 21900806, 21900807, 21900808, 21900809, 21900810, 21900811, 21900812, 21900813, 21900814, 21900815, 21900816, 21900817, 21900818, 21900819, 21900822, 21900823, 21900824, 21900825, 21900826, 21900827, 21900828, 21900829, 21900830, 21900831, 21900832, 21900833, 21900834, 21900835, 21900836, 21900837, 21900838, 21900845, 21900847, 21900853, 21900854, 21900855, 21900856, 21900857, 21900858, 21900859, 21900860, 21900861, 21900862, 21900863, 21900865, 21900867, 21900873, 21900874, 21900875, 21900876, 21900884, 21900885, 21900886, 21900887, 21900895, 21900905, 21900907, 21900909, 21900910, 21900911, 21900912, 21900913, 21900914, 21900915, 21900916, 21900927, 21900928, 21900929, 21900930, 21900931, 21900932, 21900933, 21900934, 21900935, 21900936, 21900937, 21900938, 21900939, 21900940, 21900941, 21900942, 21900943, 21900944, 21900945, 21900947, 21900955, 21900965, 21900967, 21901308, 21901309, 21901310, 21901311, 21901312, 21901313, 21901314, 21901315, 21901316, 21901317, 21901318, 41900101, 41900131, 41900141, 41900166, 41900171,
                41900211 };
        public async Task<Root>GetJSON(int GameID, int season)
        {
            string boxLink = "https://cdn.nba.com/static/json/liveData/boxscore/boxscore_00" + GameID + ".json";
            string json = "";
            Root root = new Root();
            if (season == 2019 && Missing2019Games.Contains(GameID))
            {
                return root;
            }
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
                client.Timeout = TimeSpan.FromSeconds(3.5);
                try
                {
                    json = await client.GetStringAsync(boxLink);
                    root = JsonConvert.DeserializeObject<Root>(json);
                }
                catch (HttpRequestException ex)
                {
                    ErrorOutput(ex);
                }
                catch (NullReferenceException nah)
                {
                    ErrorOutput(nah);
                }
                catch (TaskCanceledException thread)
                {
                    ErrorOutput(thread);
                }
            }
            
            return root;
        }
        public void ErrorOutput(Exception e)
        {
            var st = new System.Diagnostics.StackTrace(e, true);
            var frame = st.GetFrame(0); // Get the top stack frame where the exception occurred

            // Output file name, method name, and line number
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

            // If there's an inner exception, show that too
            if (e.InnerException != null)
            {
                Console.WriteLine($"Inner exception: {e.InnerException.Message}");
            }

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
