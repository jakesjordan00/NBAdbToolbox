using NBAdbToolbox;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
#nullable enable

namespace NBAdbToolboxCurrentPBP
{
    public class DataCurrentPBP
    {
        public async Task<Root>GetJSON(int GameID, int season)
        {
            string pbpLink = "https://cdn.nba.com/static/json/liveData/playbyplay/playbyplay_00" + GameID + ".json";
            string json = "";
            Root root = new Root();
            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
                client.Timeout = TimeSpan.FromSeconds(3);
                if(season == 2019)
                {
                    client.Timeout = TimeSpan.FromSeconds(1);
                }
                try
                {
                    json = await client.GetStringAsync(pbpLink);
                    json = json.Replace("None", "null");
                    root = JsonConvert.DeserializeObject<Root>(json);
                }
                catch (HttpRequestException ex)
                {

                }
                catch (NullReferenceException nah)
                {

                }
                catch (TaskCanceledException thread)
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
        public List<Action> actions { get; set; }
    }

    public class Action
    {
        public int? actionNumber { get; set; }
        public string? clock { get; set; }
        public string? timeActual { get; set; }
        public int? period { get; set; }
        public string? periodType { get; set; }
        public string? actionType { get; set; }
        public string? subType { get; set; }
        public List<object>? qualifiers { get; set; }
        public int? personId { get; set; }
        public int? possession { get; set; }
        public string? scoreHome { get; set; }
        public string? scoreAway { get; set; }
        public string? edited { get; set; }
        public int? orderNumber { get; set; }
        public bool? isTargetScoreLastPeriod { get; set; }
        public int? isFieldGoal { get; set; }
        public string? description { get; set; }
        public List<object>? personIdsFilter { get; set; }
        public int? teamId { get; set; }
        public string? teamTricode { get; set; }
        public string? descriptor { get; set; }
        public string? jumpBallRecoveredName { get; set; }
        public int? jumpBallRecoverdPersonId { get; set; }
        public string? playerName { get; set; }
        public string? playerNameI { get; set; }
        public string? jumpBallWonPlayerName { get; set; }
        public int? jumpBallWonPersonId { get; set; }
        public string? jumpBallLostPlayerName { get; set; }
        public int? jumpBallLostPersonId { get; set; }
        public string? area { get; set; }
        public string? areaDetail { get; set; }
        public int? officialId { get; set; }
        public int? turnoverTotal { get; set; }
        public double? x { get; set; }
        public double? y { get; set; }
        public string? side { get; set; }
        public double shotDistance { get; set; }
        public int? xLegacy { get; set; }
        public int? yLegacy { get; set; }
        public string? shotResult { get; set; }
        public int? pointsTotal { get; set; }
        public string? assistPlayerNameInitial { get; set; }
        public int? assistPersonId { get; set; }
        public int? assistTotal { get; set; }
        public int? shotActionNumber { get; set; }
        public int? reboundTotal { get; set; }
        public int? reboundDefensiveTotal { get; set; }
        public int? reboundOffensiveTotal { get; set; }
        public int? foulPersonalTotal { get; set; }
        public int? foulTechnicalTotal { get; set; }
        public string? foulDrawnPlayerName { get; set; }
        public int? foulDrawnPersonId { get; set; }
        public string? stealPlayerName { get; set; }
        public int? stealPersonId { get; set; }
        public string? blockPlayerName { get; set; }
        public int? blockPersonId { get; set; }
    }

}
