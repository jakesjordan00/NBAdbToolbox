using NBAdbToolboxCurrent;
using NBAdbToolboxCurrentPBP;
using NBAdbToolboxHistoric;
using NBAdbToolboxSchedule;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Data.SqlTypes;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Media;
using System.Net;
using System.Net.Http;
using System.Runtime;
using System.Runtime.ConstrainedExecution;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.ComTypes;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using static NBAdbToolbox.Main;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TextBox;

namespace NBAdbToolbox
{
    public partial class Main : Form
    {
        #region Global Declarations
        NBAdbToolboxHistoric.Root root = new NBAdbToolboxHistoric.Root();
        NBAdbToolboxCurrent.Root rootC = new NBAdbToolboxCurrent.Root();
        NBAdbToolboxCurrentPBP.Root rootCPBP = new NBAdbToolboxCurrentPBP.Root();
        NBAdbToolboxSchedule.ScheduleLeagueV2 schedule = new NBAdbToolboxSchedule.ScheduleLeagueV2();
        public bool dbConnection = false; //Determine whether or not we have a connection to the Database in dbconfig file
        public bool isConnected = false; //Server Connection status variable
        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\..")); //File path for project

        public string settingsPath = Path.Combine(projectRoot, @"Content", "settings.json");
        private Settings settings;
        private Settings settingsControl;

        public string configPath = ""; //dbconfig file
        private DbConfig config;
        private DbConfig configControl;


        public int screenWidth = Screen.PrimaryScreen.WorkingArea.Width; //Screen size/Display variables
        public int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;



        //Connection String items
        public static string cString = "";
        public SqlConnectionStringBuilder bob = new SqlConnectionStringBuilder();
        public SqlConnection SQLdb = new SqlConnection(cString);





        //SQL building items
        public static string buildFile = File.ReadAllText(Path.Combine(projectRoot, "Content", "build.sql"));   //Creates tables
        //Splits procedures off from table creation. For some reason, it won't let me do them at once, even if the formatting I have works straight in SQL. it really doesnt like 'go'
        public static string procedures = buildFile.Substring(buildFile.IndexOf("~") + 1).Replace("*/", "");
        //Each procedure is a list item
        public static List<string> procs = new List<string>();


        #region pnlWelcome - Welcome Panel
        //pnlWelcome items
        public Panel pnlWelcome = new Panel();
        private string iconFile = "";               //Icon file name
        private string imagePath = "";              //Icon file path
        private string imagePathDb = "";            //Db icon file path
        #region Labels and Controls
        public Label lblStatus = new Label();      //Header label
        public Label lblServer = new Label();      //Server
        public Label lblServerName = new Label();  //Server name
        public Label lblCStatus = new Label { Tag = "Server Connection" };     //Connection string status
        public Label lblDB = new Label();          //Database
        public Label lblDbName = new Label();      //Database name
        public Label lblDbStat = new Label();      //Need to create database/Database created label
        public Button btnEdit = new Button();      //Edit config file
        public Button btnBuild = new Button();     //Build Database
        public PictureBox picStatus = new PictureBox();//Connection string icon
        public PictureBox picDbStatus = new PictureBox();//Db Status icon


        public Label lblSettings = new Label();
        public PictureBox picSettings = new PictureBox();//Db Status icon
        public Panel pnlSettings = new Panel
        {
            Visible = false
        };
        public Label lblBrowseConfig = new Label
        {
            Text = "Change Config Folder:",
            Visible = true
        };
        public Label lblChangeConfig = new Label
        {
            Text = "Change Connection/Db:",
            Visible = true
        };
        public Label lblConfigFiles = new Label
        {
            Text = "Default Connection:",
            Visible = true
        };
        public Label lblBackground = new Label
        {
            Text = "Background:",
            Visible = true
        };
        public Label lblSound = new Label
        {
            Text = "Sound Options:",
            Visible = true
        };
        public FolderBrowserDialog dlgDefaultPath = new FolderBrowserDialog();
        public Button btnBrowseConfig = new Button
        {
            Text = "Browse...",
            Visible = true
        };
        public ComboBox boxChangeConfig = new ComboBox
        {
            Visible = true
        };
        public ComboBox boxConfigFiles = new ComboBox
        {
            Visible = true
        };
        public ComboBox boxBackground = new ComboBox
        {
            Visible = true,
            Items = { "Court Default", "Court Dark" }
        };
        public ComboBox boxSoundOptions = new ComboBox
        {
            Visible = true,
            Items = { "Default", "Muted" }
        };


        #endregion
        #region Variables
        #endregion


        #endregion




        #region pnlDbUtil - Database Utilities
        public Panel pnlDbUtil = new Panel();
        public Label lblGameUtil = new Label { Text = "Game", Visible = false, Tag = "Table" };
        public Label lblTeamUtil = new Label { Text = "Team", Visible = false, Tag = "Table" };
        public Label lblArenaUtil = new Label { Text = "Arena", Visible = false, Tag = "Table" };
        public Label lblPlayerUtil = new Label { Text = "Player", Visible = false, Tag = "Table" };
        public Label lblOfficialUtil = new Label { Text = "Official", Visible = false, Tag = "Table" };
        public Label lblTbUtil = new Label { Text = "TeamBox", Visible = false, Tag = "Table" };
        public Label lblPbUtil = new Label { Text = "PlayerBox", Visible = false, Tag = "Table" };
        public Label lblPbpUtil = new Label { Text = "PlayByPlay", Visible = false, Tag = "Table" };

        public Label lblEmpty = new Label { Text = "Unpopulated", Visible = false, Tag = "Year", Name = "Unpopulated" };



        public Panel pnlDbOverview = new Panel();
        #region Labels and Controls
        public Label lblDbUtil = new Label { Text = "Database Utilities" };
        public Label lblDbOverview = new Label();
        public Label lblDbOvName = new Label();
        public Label lblDbOvExpand = new Label();
        public Label lblDbOptions = new Label();
        public Label lblDbSelectSeason = new Label();
        public Label lblPopulate = new Label
        {
            Text = "Populate Season Data"
        };
        public Label lblDownloadSeasonData = new Label
        {
            Text = "Download Data Files"
        };
        public Label lblDlSelectSeason = new Label();

        public ListBox listSeasons = new ListBox();
        public ListBox listDownloadSeasonData = new ListBox();
        public Button btnPopulate = new Button();
        public Button btnDownloadSeasonData = new Button();
        public Label lblRefresh = new Label
        {
            Text = "Refresh Current Season"
        };
        public Button btnRefresh = new Button();

        //pnlDbUtil sub panel Positions and sizes
        public int leftPanelPos = 0;
        public int midPanelPos = 0;
        public int rightPanelPos = 0;
        public int fullHeight = 0;
        public int dimW = 0;
        public int dimH = 0;
        public int dimH2 = 0;
        #endregion
        #region Variables
        public HashSet<int> Missing2019Games = new HashSet<int> {  21900194, 21900200, 21900203, 21900204, 21900208, 21900244, 21900306, 21900307, 21900308, 21900309, 21900310, 21900311, 21900312, 21900313, 21900314, 21900315, 21900316, 21900317, 21900318, 21900319, 21900320, 21900321, 21900322, 21900323, 21900324, 21900325, 21900326, 21900327, 21900328, 21900329, 21900619, 21900668, 21900669, 21900670, 21900671, 21900672, 21900673, 21900674, 21900675, 21900676, 21900677, 21900678, 21900679, 21900680, 21900681, 21900682, 21900683, 21900684, 21900685, 21900686, 21900687, 21900688, 21900689, 21900690, 21900691, 21900692, 21900693, 21900694, 21900695, 21900696, 21900697, 21900698, 21900699, 21900700, 21900701, 21900702, 21900703, 21900704, 21900705, 21900706, 21900708, 21900709, 21900710, 21900711, 21900712, 21900713, 21900714, 21900715, 21900716, 21900717, 21900718, 21900719, 21900721, 21900722, 21900723, 21900724, 21900725, 21900726, 21900727, 21900728, 21900729, 21900730, 21900731, 21900732, 21900733, 21900734, 21900735, 21900736, 21900737, 21900738, 21900739, 21900740, 21900741, 21900742, 21900743, 21900744, 21900745, 21900746, 21900747, 21900748, 21900749, 21900750, 21900751, 21900752, 21900753, 21900754, 21900755, 21900756, 21900757, 21900758, 21900759, 21900760, 21900761, 21900762, 21900763, 21900764, 21900765, 21900766, 21900767, 21900768, 21900769, 21900770, 21900771, 21900772, 21900773, 21900774, 21900775, 21900776, 21900778, 21900779, 21900780, 21900781, 21900782, 21900783, 21900784, 21900785, 21900786, 21900787, 21900788, 21900789, 21900790, 21900791, 21900792, 21900793, 21900794, 21900795, 21900796, 21900797, 21900798, 21900799, 21900800, 21900801, 21900802, 21900803, 21900804, 21900805, 21900806, 21900807, 21900808, 21900809, 21900810, 21900811, 21900812, 21900813, 21900814, 21900815, 21900816, 21900817, 21900818, 21900819, 21900822, 21900823, 21900824, 21900825, 21900826, 21900827, 21900828, 21900829, 21900830, 21900831, 21900832, 21900833, 21900834, 21900835, 21900836, 21900837, 21900838, 21900845, 21900847, 21900853, 21900854, 21900855, 21900856, 21900857, 21900858, 21900859, 21900860, 21900861, 21900862, 21900863, 21900865, 21900867, 21900873, 21900874, 21900875, 21900876, 21900884, 21900885, 21900886, 21900887, 21900895, 21900905, 21900907, 21900909, 21900910, 21900911, 21900912, 21900913, 21900914, 21900915, 21900916, 21900927, 21900928, 21900929, 21900930, 21900931, 21900932, 21900933, 21900934, 21900935, 21900936, 21900937, 21900938, 21900939, 21900940, 21900941, 21900942, 21900943, 21900944, 21900945, 21900947, 21900955, 21900965, 21900967, 21901308, 21901309, 21901310, 21901311, 21901312, 21901313, 21901314, 21901315, 21901316, 21901317, 21901318, 41900101, 41900131, 41900141, 41900166, 41900171,
                41900211 };
        #endregion
        //pnlDbUtil Options section
        public int overviewHeight = 0;
        #endregion

        #region PnlLoad
        public Panel pnlLoad = new Panel();
        #region Labels and Controls
        public Label lblSeasonStatusLoad = new Label
        {
            Text = "Currently loading: ",
            Visible = false
        };
        public Label lblSeasonStatusLoadInfo = new Label
        {
            Text = "",
            Visible = false
        };
        public Label lblCurrentGame = new Label
        {
            Text = "Current game: ",
            Visible = false
        };
        public Label lblCurrentGameCount = new Label
        {
            Text = "0",
            Visible = false
        };
        public Label lblWorkingOn = new Label
        {
            Text = "",
            Visible = false
        };
        public Label lblWorkingOnInfo = new Label
        {
            Text = "",
            Visible = false
        };
        public Label gpm = new Label
        {
            Text = "Games per minute/Est.Time remaining:",
            Visible = false
        };
        public Label gpmValue = new Label
        {
            Text = "",
            Visible = false
        };
        PictureBox picLoad = new PictureBox
        {
            Image = Image.FromFile(Path.Combine(projectRoot, @"Content\Images\Loading", "kawhi1.png")),
            Visible = false,
            SizeMode = PictureBoxSizeMode.Zoom,
            BackColor = Color.Transparent
        };
        #endregion

        #endregion



        #region Time Tracking
        public Stopwatch stopwatchInsert = new Stopwatch();
        public Stopwatch stopwatchRead = new Stopwatch();
        public Stopwatch stopwatchAlterDelete = new Stopwatch();
        public TimeSpan timeElapsedRead = new TimeSpan(0);
        public string elapsedStringRead = "";
        public Stopwatch stopwatch;
        public DateTime start = DateTime.Now;
        public Dictionary<string, (int, string)> timeUnitsRead = new Dictionary<string, (int value, string sep)>();
        Stopwatch stopwatchFull = Stopwatch.StartNew();
        DateTime startFull = DateTime.Now;

        #endregion



        public static DataHistoric historic = new DataHistoric();
        public static DataCurrent currentData = new DataCurrent();
        public static DataCurrentPBP currentDataPBP = new DataCurrentPBP();
        public static Schedule leagueSchedule = new Schedule();
        public static List<NBAdbToolboxSchedule.Game> scheduleGames = new List<NBAdbToolboxSchedule.Game>();
        public float loadFontSize = 0;
        public string completionMessage = "";

        public int noBoxCount = 0;
        public int noPBPCount = 0;

        public int picLoadLoc = 0;

        //Header Panels
        public Panel pnlNav = new Panel();
        public Panel pnlScoreboard = new Panel();

        public HashSet<(int SeasonID, (int Games, int Loaded, int Team, int Arena, int Player, int Official, int Game, int PlayerBox, int TeamBox, int PlayByPlay, int StartingLineups, int TeamBoxLineups,
            int HistoricLoaded, int CurrentLoaded))> seasonInfo
            = new HashSet<(int, (int, int, int, int, int, int, int, int, int, int, int, int, int, int))>();

        public HashSet<(int SeasonID, (int Games, int Loaded, int Team, int Arena, int Player, int Official, int Game, int PlayerBox, int TeamBox, int PlayByPlay, int StartingLineups, int TeamBoxLineups))> seasonControl
            = new HashSet<(int, (int, int, int, int, int, int, int, int, int, int, int, int))>()
            {
                (2024, (1320, 1, 30, 36, 587, 80, 1320, 1320, 1320, 1320, 1320, 1320)),
                //(2024, (1321, 1, 30, 36, 587, 80, 1321, 1321, 1321, 1321)),
                (2023, (1319, 1, 30, 34, 595, 80, 1319, 1319, 1319, 1319, 1319, 1319)),
                (2022, (1320, 1, 30, 35, 554, 82, 1320, 1320, 1320, 1320, 1320, 1320)),
                (2021, (1323, 1, 30, 30, 633, 84, 1323, 1323, 1323, 1323, 1323, 1323)),
                (2020, (1171, 1, 30, 30, 550, 79, 1171, 1171, 1171, 1171, 1171, 1171)),
                (2019, (1142, 1, 30, 34, 549, 74, 1142, 1142, 1142, 1142, 1142, 1142)),
                (2018, (1312, 1, 30, 32, 557, 68, 1312, 1312, 1312, 1312, 1312, 1312)),
                (2017, (1311, 1, 30, 32, 559, 71, 1311, 1311, 1311, 1311, 1311, 1311)),
                (2016, (1309, 1, 30, 31, 493, 67, 1309, 1309, 1309, 1309, 1309, 1309)),
                (2015, (1316, 1, 30, 31, 484, 66, 1316, 1316, 1316, 1316, 1316, 1316)),
                (2014, (1311, 1, 30, 31, 503, 67, 1311, 1311, 1311, 1311, 1311, 1311)),
                (2013, (1319, 1, 30, 30, 495, 66, 1319, 1319, 1319, 1319, 1319, 1319)),
                (2012, (1314, 1, 30, 30, 485, 68, 1314, 1314, 1314, 1314, 1314, 1314)),
                (2011, (1074, 1, 30, 29, 483, 66, 1074, 1074, 1074, 1074, 1074, 1074)),
                (2010, (1311, 1, 30, 30, 469, 64, 1311, 1311, 1311, 1311, 1311, 1311)),
                (2009, (1312, 1, 30, 30, 469, 66, 1312, 1312, 1312, 1312, 1312, 1312)),
                (2008, (1315, 1, 30, 30, 461, 61, 1315, 1315, 1315, 1315, 1315, 1315)),
                (2007, (1316, 1, 30, 30, 469, 58, 1316, 1316, 1316, 1315, 1316, 1315)),
                (2006, (1309, 1, 30, 31, 471, 60, 1309, 1309, 1309, 1308, 1309, 1308)),
                (2005, (1319, 1, 30, 32, 470, 63, 1319, 1319, 1319, 1319, 1319, 1319)),
                (2004, (1314, 1, 30, 29, 469, 62, 1314, 1314, 1314, 1314, 1314, 1314)),
                (2003, (1271, 1, 29, 29, 548, 60, 1271, 1271, 1271, 1270, 1271, 1270)),
                (2002, (1277, 1, 29, 29, 620, 39, 1277, 1277, 1277, 1277, 1277, 1277)),
                (2001, (1260, 1, 29, 28, 650, 56, 1260, 1260, 1260, 1260, 1260, 1260)),
                (2000, (1260, 1, 29, 29, 652, 51, 1260, 1260, 1260, 1260, 1260, 1260)),
                (1999, (1264, 1, 29, 32, 646, 37, 1264, 1264, 1264, 1264, 1264, 1264)),
                (1998, (791, 1, 29, 33, 633, 37, 791, 791, 791, 790, 1320, 1320)),
                (1997, (1260, 1, 29, 34, 608, 40, 1260, 1260, 1260, 1260, 1320, 1320)),
                (1996, (1261, 1, 29, 33, 715, 40, 1261, 1261, 1261, 1259, 1320, 1320))
            };
        public HashSet<(int SeasonID, (int Games, int Loaded, int Team, int Arena, int Player, int Official, int Game, int PlayerBox, int TeamBox, int PlayByPlay, int StartingLineups, int TeamBoxLineups))> seasonCurrentControl
            = new HashSet<(int, (int, int, int, int, int, int, int, int, int, int, int, int))>()
            {
                (2024, (1320, 1, 30, 36, 587, 80, 1320, 1320, 1320, 1320, 1320, 1320)),
                //(2024, (1321, 1, 30, 36, 587, 80, 1321, 1321, 1321, 1321, 1321, 1321)),
                (2023, (1319, 1, 30, 34, 595, 81, 1319, 1319, 1319, 1319, 1319, 1319)),
                (2022, (1320, 1, 30, 35, 554, 83, 1320, 1320, 1320, 1320, 1320, 1320)),
                (2021, (1323, 1, 30, 30, 633, 84, 1323, 1323, 1323, 1323, 1323, 1323)),
                (2020, (1171, 1, 30, 31, 550, 79, 1171, 1171, 1171, 1171, 1171, 1171)),
                (2019, (1142, 1, 30, 37, 549, 74, 1142, 1142, 1142, 1142, 1142, 1142)),
            };


        public int currentImageIterator = 0;
        public bool currentReverse = false;
        public string currentBoxUpdate = "";




        public string json = "";
        PictureBox bgCourt = new PictureBox //Create Background image
        {
            SizeMode = PictureBoxSizeMode.StretchImage,
            Dock = DockStyle.Fill
        };
        public string Theme = "Default";

        //Utility
        #region Utilitiy
        private float aspectRatio;
        private bool isResizing = false;
        public int imageIteration = 1;
        public int iterator = 0;
        public bool reverse = false;
        #endregion

        #region Data Load

        public int GameID = 0;
        public int SeasonID = 0;
        public int RegularSeasonGames = 0;
        public int PostseasonGames = 0;
        public int TotalGames = 0;
        public int TotalGamesCD = 0;
        public List<int> gamesRS = new List<int>();
        public List<int> gamesPS = new List<int>();

        #endregion

        #endregion

        public float screenFontSize = 1;

        public string settingsJSON = "";
        public bool defaultConfig = false;
        public bool isBuildEnabled = true;

        public bool dbOverviewFirstOpen = true;
        //Settings

        public bool isPopulating = false;
        public bool missingPbp = false;
        public string pbpInsertFailSafe = "";

        public Main()
        {
            InitializeComponent();
            GetSettings("Main");
            //Set screen size
            #region Set screen size
            #endregion
            lblDbUtil.ForeColor = ThemeColor;

            //Add initial controls before we  attempts connection
            AddControls("Init");

            //Check for dbconfig - Verify Server/Database Connectivity
            InitializeDbConfig("Main");

            //Add controls to panel
            InitializeElements();
            //Set Colors
            SetTheme("Main");



            float fontSize = ((float)(screenFontSize * pnlWelcome.Height * .08) / (96 / 12)) * (72 / 12);




            #region Populate Database
            btnPopulate.Click += async (s, e) =>
            {
                SqlConnection MainConnection = new SqlConnection(cString);
                int selectedSeasons = listSeasons.SelectedItems.Count;
                string dialog = "Seasons selected: ";
                List<int> seasons = new List<int>();

                for (int i = 0; i < listSeasons.SelectedItems.Count; i++)
                {
                    seasons.Add(Int32.Parse(listSeasons.SelectedItems[i].ToString()));
                    dialog += seasons[i] + ", ";
                }
                if (selectedSeasons > 0)
                {
                    dialog = dialog.Remove(dialog.Length - 2);
                }
                var popup = new PopulatePopup(dialog, seasons);
                if (popup.ShowDialog() == DialogResult.OK)
                {
                    isPopulating = true;
                    int historic = 0;
                    int current = 0;
                    string source = "";
                    InitializeDbLoad();
                    int buildID = 0;
                    using (SqlCommand BuildLogCheck = new SqlCommand("BuildLogCheck", MainConnection))
                    {
                        BuildLogCheck.CommandType = CommandType.StoredProcedure;
                        MainConnection.Open();
                        using (SqlDataReader reader = BuildLogCheck.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                buildID = reader.GetInt32(0);
                            }
                        }
                        MainConnection.Close();
                    }
                    stopwatchFull.Restart();
                    int seasonIterator = 0;
                    foreach (int season in seasons)
                    {
                        missingPbp = false;
                        SeasonID = season;
                        seasonIterator++;
                        Task DeleteSeasonData = Task.Run(() =>
                        {
                            AlterDeleteExisting();
                            stopwatchAlterDelete.Stop();
                            stopwatchDelete.Stop();
                        });
                        PopulateDb_1_PreSelection();
                        //Historic Data
                        #region Historic Data
                        if (popup.historic || (!popup.historic && !popup.current && season < 2019))
                        {
                            source = "Historic";
                            historic = 1;
                            lblSeasonStatusLoad.Text = SeasonID + " Historic data file";
                            stopwatchRead.Restart();
                            root = null;
                            await Task.Run(async () =>      //This sets the root variable to our big file
                            {
                                await ReadSeasonFile(popup.historic, popup.current);
                            });
                            PopulateDb_2_AfterHistoricRead();
                            await DeleteSeasonData;
                            PopulateDb_3_AfterDelete_BeforeGames();
                            foreach (NBAdbToolboxHistoric.Game game in root.season.games.regularSeason)
                            {
                                sqlBuilder.Clear();
                                Task SecondInsert;
                                GameID = Int32.Parse(game.game_id);
                                lblCurrentGameCount.Text = game.game_id;
                                await Task.Run(async () =>      //This inserts the games from season file into db
                                {
                                    await InsertGameWithLoading(game, season, imageIteration, "Regular Season");
                                });
                                //After Insert is complete, use multithreading to insert the bulk of our data - PlayByPlay, PlayerBox (+StartingLineups) and TeamBox (+TeamBoxLineups)
                                #region Second Insert
                                if (GameID == 20600975 || GameID == 20700753 || GameID == 20300778 || GameID == 29800661 || GameID == 29600332 || GameID == 29600370)
                                {
                                    game.box = null;
                                    game.playByPlay = null;
                                    SecondInsert = Task.CompletedTask;
                                }
                                else
                                {
                                    SecondInsert = Task.Run(async () =>
                                    {
                                        await InsertPlayByPlayWithRetry(game, "RS");
                                    });
                                }
                                lastSeason = season;

                                UpdateLoadingImage(imageIteration);
                                #endregion
                                PopulateDb_4_AfterHistoricGame("RS");
                                if(iterator == RegularSeasonGames)
                                {
                                    await SecondInsert;
                                }
                            }
                            if (missingPbp)
                            {
                                lblSeasonStatusLoad.Text = "Catching any missing data";
                                foreach (var game in root.season.games.regularSeason.Where(g => g.playByPlay != null))
                                {
                                    HistoricPlayByPlayStaging(game.playByPlay);
                                    await InsertPlayByPlayWithRetry(game, "Retry");
                                    UpdateLoadingImage(imageIteration);
                                    PopulateDb_4_AfterHistoricGame("Retry");
                                }
                            }
                            root.season.games.regularSeason = null;
                            missingPbp = false;
                            lblSeasonStatusLoad.Text = "Inserting " + SeasonID + " Postseason";

                            foreach (NBAdbToolboxHistoric.Game game in root.season.games.playoffs)
                            {
                                sqlBuilder.Clear();
                                GameID = Int32.Parse(game.game_id);
                                lblCurrentGameCount.Text = game.game_id;
                                await Task.Run(async () =>      //This inserts the games from season file into db
                                {
                                    await InsertGameWithLoading(game, season, imageIteration, "Postseason");
                                });
                                #region Second Insert
                                Task SecondInsert = Task.Run(async () =>
                                {
                                    await InsertPlayByPlayWithRetry(game, "PS");
                                });
                                

                                UpdateLoadingImage(imageIteration);

                                #endregion
                                PopulateDb_4_AfterHistoricGame("PS");
                                if (iterator == TotalGames)
                                {
                                    await SecondInsert;
                                }
                            }
                            if (missingPbp)
                            {
                                lblSeasonStatusLoad.Text = "Catching any missing data";
                                foreach (var game in root.season.games.playoffs.Where(g => g.playByPlay != null))
                                {
                                    HistoricPlayByPlayStaging(game.playByPlay);
                                    await InsertPlayByPlayWithRetry(game, "Retry");
                                    UpdateLoadingImage(imageIteration);
                                    PopulateDb_4_AfterHistoricGame("Retry");
                                }
                            }
                            root.season.games.playoffs = null;
                            missingPbp = false;
                        }
                        #endregion
                        //Current Data
                        #region Current Data
                        else if (popup.current || (!popup.historic && !popup.current && season > 2018))
                        {
                            source = "Current";
                            current = 1;
                            PopulateDb_5_BeforeCurrentRead();
                            await Task.Run(async () =>      //We need to read the big file to get our game list
                            {
                                await ReadSeasonFile(popup.historic, popup.current);
                            });
                            PopulateDb_6_AfterCurrentReadRoot();
                            await DeleteSeasonData;
                            PopulateDb_7_AfterCurrentDelete();
                            for (int i = 0; i < RegularSeasonGames; i++)
                            {
                                GameID = gamesRS[i];
                                await CurrentGameGPS(gamesRS[i], "");
                                if (!boxMissing)
                                {
                                    Task CalculateTeamBoxLineup = TeamBoxLineupCalculation(GameID);
                                    if (i % 5 == 0 || i == RegularSeasonGames - 1)
                                    {
                                        await CalculateTeamBoxLineup;
                                        string execute = LineupCalc.ToString();
                                        LineupCalc.Clear();
                                        Task CalculateTeamBoxLineupsInsert = CalculatedTeamBoxLineupInsert(execute);
                                        if (i == RegularSeasonGames - 1)
                                        {
                                            await CalculateTeamBoxLineupsInsert;
                                        }
                                    }
                                }
                                root.season.games.regularSeason[i].box = null;
                                root.season.games.regularSeason[i].playByPlay = null;
                                PopulateDb_8_AfterCurrentGame(gamesRS[i].ToString());
                            }
                            for (int i = 0; i < PostseasonGames; i++)
                            {
                                GameID = gamesPS[i];
                                lblCurrentGameCount.Text = gamesPS[i].ToString();
                                await CurrentGameGPS(gamesPS[i], "");
                                if (!boxMissing)
                                {
                                    Task CalculateTeamBoxLineup = TeamBoxLineupCalculation(GameID);
                                    if (i % 5 == 0 || i == PostseasonGames - 1)
                                    {
                                        await CalculateTeamBoxLineup;
                                        string execute = LineupCalc.ToString();
                                        LineupCalc.Clear();
                                        Task CalculateTeamBoxLineupsInsert = CalculatedTeamBoxLineupInsert(execute);
                                        if (i == PostseasonGames - 1)
                                        {
                                            await CalculateTeamBoxLineupsInsert;
                                        }
                                    }
                                }
                                //Task TeamBoxLineupTask = TeamBoxLineupCalculation(GameID);
                                root.season.games.playoffs[i].box = null;
                                root.season.games.playoffs[i].playByPlay = null;
                                PopulateDb_8_AfterCurrentGame(gamesPS[i].ToString());
                            }

                            await DeleteSeasonData;
                            PopulateDb_7_AfterCurrentDelete();


                            sqlBuilder.Clear();
                            sqlBuilderParallel.Clear();
                            playByPlayBuilder.Clear();
                        }
                        #endregion

                        PopulateDb_9_AfterSeasonInserts(buildID, current, historic, source, seasonIterator, selectedSeasons);
                        //int stop = gameBytes.Count;
                    }
                    stopwatchFull.Stop();
                    PopulateDb_10_Completion();
                    isPopulating = false;
                    CheckDataFiles(); //GetSeasons();
                    if (dbOverviewOpened)
                    {
                        lblDbOptions.Invoke((MethodInvoker)(() =>
                        {
                            lblDbOptions.Text = "Loading Season info";
                            CenterElement(pnlDbUtil, lblDbOptions);
                        }));
                        await Task.Run(() => GetSeasonInfo());
                        lblDbOptions.Invoke((MethodInvoker)(() =>
                        {
                            lblDbOptions.Text = "Options";
                            CenterElement(pnlDbUtil, lblDbOptions);
                        }));
                        dbOverviewFirstOpen = false;
                        DbOverviewVisibility(dbOverviewOpened, "Populate");
                    }
                    else
                    {
                        dbOverviewFirstOpen = true;
                    }
                }
            };
            #endregion
            //Panel Formatting
            #region Panel Formatting


            AddControlsAfterConnection();

            //Scoreboard
            pnlScoreboard.Height = this.Height / 18;
            pnlScoreboard.Width = this.Width - pnlDbUtil.Width;
            pnlScoreboard.Parent = bgCourt;
            pnlScoreboard.Top = pnlNav.Bottom;
            pnlScoreboard.Left = pnlDbUtil.Right;

            #endregion
            #region Edit Connection & Build DB
            //if (dbConnection)
            //{
            //    CheckDataFiles(); //GetSeasons();
            //}
            CheckDataFiles(); //GetSeasons();

            //Edit Button Actions
            btnEdit.Click += (s, e) =>
            {
                string server = config?.Server ?? "";
                string alias = config?.Alias ?? "";
                bool? create = config?.Create;
                bool? def = config?.Default;
                string database = config?.Database ?? "";
                bool? windowsAuth = config?.UseWindowsAuth;
                string username = config?.Username ?? "";
                string password = config?.Password ?? "";
                bool fileExist = true;

                if (!File.Exists(configPath))
                {
                    fileExist = false;
                }
                var popup = new EditPopup("create", fileExist, server, alias, create, def, database, windowsAuth, username, password);
                if (popup.ShowDialog() == DialogResult.OK)
                {
                    config = new DbConfig
                    {
                        Server = popup.Server,
                        Alias = popup.Alias,
                        Create = popup.CreateDatabase,
                        Default = popup.DefaultDb,
                        Database = popup.Database,
                        UseWindowsAuth = popup.UseWindowsAuth,
                        Username = popup.Username,
                        Password = popup.Password
                    };
                    defaultConfig = false;
                    if (configPath == "" || popup.DefaultDb == true)
                    {

                    }
                    //configPath = Path.Combine(settings.ConfigPath, 
                    InitializeDbConfig("btnEdit");
                    RefreshDefaultConfigPath("Edit");
                }
            };

            btnBuild.Click += (s, e) =>
            {
                if (config.Create == true)
                {
                    CreateDB(cString);
                    bob.InitialCatalog = config.Database;
                    cString = bob.ToString();
                    btnPopulate.Enabled = true;
                    ButtonChangeState(btnPopulate, true);
                }
            };
            #endregion

            #region Database Overview

            DbOverviewClick(pnlDbOverview, lblDbOvExpand, pnlDbOverview);
            DbOverviewClick(lblDbOverview, lblDbOvExpand, pnlDbOverview);
            DbOverviewClick(lblDbOvExpand, lblDbOvExpand, pnlDbOverview);
            DbOverviewClick(lblDbOvName, lblDbOvExpand, pnlDbOverview);
            #endregion

            btnBrowseConfig.Click += (s, e) =>
            {
                dlgDefaultPath.Description = "Select folder for configuration files";
                dlgDefaultPath.SelectedPath = settings.ConfigPath;

                if (dlgDefaultPath.ShowDialog() == DialogResult.OK)
                {
                    settings.ConfigPath = dlgDefaultPath.SelectedPath;
                    File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
                }
                lblSettings.Focus();
            };
            if (Directory.Exists(settings.ConfigPath))
            {
                RefreshDefaultConfigPath("Main");
            }
            boxChangeConfig.SelectedIndexChanged += async (s, e) =>
            {
                if (boxChangeConfig.SelectedItem != null)
                {
                    string selectedFile = boxChangeConfig.SelectedItem.ToString() + ".json";
                    configPath = Path.Combine(settings.ConfigPath, selectedFile);
                    //Load the selected config
                    config = JsonConvert.DeserializeObject<DbConfig>(File.ReadAllText(configPath));
                    defaultConfig = false;
                    InitializeDbConfig("boxChangeConfig");
                    if (dbOverviewOpened)
                    {
                        lblDbOptions.Invoke((MethodInvoker)(() =>
                        {
                            lblDbOptions.Text = "Loading Season info";
                            CenterElement(pnlDbUtil, lblDbOptions);
                        }));
                        await Task.Run(() => GetSeasonInfo());
                        lblDbOptions.Invoke((MethodInvoker)(() =>
                        {
                            lblDbOptions.Text = "Options";
                            CenterElement(pnlDbUtil, lblDbOptions);
                        }));
                        dbOverviewFirstOpen = false;
                        DbOverviewVisibility(dbOverviewOpened, "Change");
                    }
                    else
                    {
                        dbOverviewFirstOpen = true;
                    }
                }
                lblSettings.Focus();
            };
            boxChangeConfig.DropDownClosed += (s, e) =>
            {
                lblSettings.Focus();
            };
            boxConfigFiles.SelectedIndexChanged += async (s, e) =>
            {
                if (isRefreshing) return;
                if (boxConfigFiles.SelectedItem != null)
                {
                    string selectedFile = boxConfigFiles.SelectedItem.ToString() + ".json";
                    configPath = Path.Combine(settings.ConfigPath, selectedFile);
                    //Load the selected config
                    config = JsonConvert.DeserializeObject<DbConfig>(File.ReadAllText(configPath));
                    defaultConfig = false;
                    InitializeDbConfig("boxDefaultConfig");
                    if (dbOverviewOpened)
                    {
                        lblDbOptions.Invoke((MethodInvoker)(() =>
                        {
                            lblDbOptions.Text = "Loading Season info";
                            CenterElement(pnlDbUtil, lblDbOptions);
                        }));
                        await Task.Run(() => GetSeasonInfo());
                        lblDbOptions.Invoke((MethodInvoker)(() =>
                        {
                            lblDbOptions.Text = "Options";
                            CenterElement(pnlDbUtil, lblDbOptions);
                        }));
                        dbOverviewFirstOpen = false;
                        DbOverviewVisibility(dbOverviewOpened, "Change");
                    }
                    else
                    {
                        dbOverviewFirstOpen = true;
                    }
                }
                RefreshDefaultConfigPath("Change");
                lblSettings.Focus();
            };
            boxConfigFiles.DropDownClosed += (s, e) =>
            {
                lblSettings.Focus();
            };
            boxBackground.SelectedIndexChanged += (s, e) =>
            {
                if (boxBackground.SelectedItem.ToString() != settings.BackgroundImage)
                {
                    settings.BackgroundImage = boxBackground.SelectedItem.ToString();
                    ClearImage(bgCourt);
                    bgCourt.Image = Image.FromFile(Path.Combine(projectRoot, @"Content\Images", settings.BackgroundImage + ".png"));
                    WriteSettings();
                    GetThemeColors();
                    SetTheme("Change");
                    DbOverviewVisibility(dbOverviewOpened, "Background");

                }
                lblSettings.Focus();
            };
            boxBackground.DropDownClosed += (s, e) =>
            {
                lblSettings.Focus();
            };
            SettingsClick(lblSettings, picSettings, fontSize);
            SettingsClick(picSettings, picSettings, fontSize);


            btnDownloadSeasonData.Click += async (s, e) =>
            {
                if (listDownloadSeasonData.SelectedItems.Count == 0)
                {
                    MessageBox.Show("Please select seasons to download", "No Selection", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                await DownloadSeasonFiles();
            };

            btnRefresh.Click += async (s, e) =>
            {
                await RefreshClick();
                if (dbOverviewOpened)
                {
                    lblDbOptions.Invoke((MethodInvoker)(() =>
                    {
                        lblDbOptions.Text = "Loading Season info";
                        CenterElement(pnlDbUtil, lblDbOptions);
                    }));
                    await Task.Run(() => GetSeasonInfo());
                    CheckDataFiles(); //GetSeasons();
                    lblDbOptions.Invoke((MethodInvoker)(() =>
                    {
                        lblDbOptions.Text = "Options";
                        CenterElement(pnlDbUtil, lblDbOptions);
                    }));
                    dbOverviewFirstOpen = false;
                    DbOverviewVisibility(dbOverviewOpened, "Refresh");
                }
                else
                {
                    dbOverviewFirstOpen = true;
                }
                RefreshCompletion();
            };

        }
        private async Task InsertPlayByPlayWithRetry(NBAdbToolboxHistoric.Game game, string sender)
        {
            int retryAttempts = 3;
            int currentAttempt = 0;
            bool success = false;
            string pbpInsert = "";
            bool primaryKeyError = false;

            while (!success && currentAttempt < retryAttempts)
            {
                try
                {
                    currentAttempt++;
                    if (currentAttempt == 1)
                    {
                        pbpInsert = playByPlayBuilder.ToString();
                        pbpInsertFailSafe = playByPlayBuilder.ToString();
                    }
                    else if (pbpInsert != pbpInsertFailSafe && !primaryKeyError && pbpInsertFailSafe != "")
                    {
                        pbpInsert = pbpInsertFailSafe;
                    }

                    using SqlConnection bigInsertsPBP = new SqlConnection(cString);
                    using SqlCommand PBPInsert = new SqlCommand(pbpInsert, bigInsertsPBP);
                    PBPInsert.CommandType = CommandType.Text;
                    PBPInsert.CommandTimeout = 120;
                    playByPlayBuilder.Clear();

                    await bigInsertsPBP.OpenAsync();
                    await PBPInsert.ExecuteNonQueryAsync();
                    success = true;
                }
                catch (SqlException ex) when (ex.Number == 1205)
                {
                    Console.WriteLine($"Deadlock detected (attempt {currentAttempt}/{retryAttempts}): {ex.Message}");
                    await Task.Delay(500 * currentAttempt);
                }
                catch (SqlException ex) when (ex.Number == 2627)
                {
                    primaryKeyError = true;
                    Console.WriteLine($"Duplicate Primary Key: {ex.Message}");
                }
                catch (Exception e)
                {
                    Console.WriteLine($"Error in SecondInsert: {e.Message}");
                    pbpInsert = pbpInsertFailSafe;
                    await Task.Delay(500 * currentAttempt);
                }
            }

            if (success)
            {
                game.box = null;
                game.playByPlay = null;
            }
            else if(sender != "Retry")
            {
                missingPbp = true;
            }
        }
        public StringBuilder LineupCalc = new StringBuilder();
        private async Task TeamBoxLineupCalculation(int game)
        {
            using (SqlConnection conn = new SqlConnection(bob.ToString()))
            using (SqlCommand SQLSeasons = new SqlCommand("TeamBoxLineupCalc", conn))
            {
                SQLSeasons.CommandType = CommandType.StoredProcedure;
                SQLSeasons.Parameters.AddWithValue("@SeasonID", SeasonID);
                SQLSeasons.Parameters.AddWithValue("@GameID", game);
                conn.Open();
                using (SqlDataReader sdr = SQLSeasons.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        string insert = sdr.GetString(0);
                        string min = sdr.GetString(1);
                        double sec = double.Parse(min.Substring(min.IndexOf("."))) * 60;
                        min = min.Substring(0, min.IndexOf("."));
                        insert = insert.Replace("minutesplaceholder", min + ":" + sec) + "\n";
                        LineupCalc.Append(insert);
                    }
                }
            }
        }
        private async Task CalculatedTeamBoxLineupInsert(string execute)
        {
            try
            {
                using (SqlConnection tbl = new SqlConnection(bob.ToString()))
                using (SqlCommand tblInsert = new SqlCommand(execute, tbl))
                {
                    tblInsert.CommandType = CommandType.Text;
                    tbl.Open();
                    tblInsert.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public void RefreshCompletion()
        {
            ButtonChangeState(btnRefresh, true);
            ButtonChangeState(btnPopulate, true);
            ButtonChangeState(btnEdit, true);
            ButtonChangeState(btnDownloadSeasonData, true);

        }
        public void InitializeDbLoad()
        {
            #region Set UI status lables and images
            imagePath = Path.Combine(projectRoot, @"Content\Images\Loading", "kawhi1.png");
            using (var img = Image.FromFile(imagePath))
            {
                // Create and assign the Bitmap
                var bitmap = new Bitmap(img);

                // Dispose of previous image to prevent memory leaks
                if (picLoad.Image != null)
                {
                    var oldImage = picLoad.Image;
                    picLoad.Image = null;
                    oldImage.Dispose();
                }
                picLoad.Image = bitmap;
                picLoad.Width = pnlLoad.Height;
                picLoad.Height = pnlLoad.Height;
                picLoad.Left = (pnlLoad.ClientSize.Width - picLoad.Width) / 2;
                picLoad.Top = 0;
            }
            gpm.Visible = true;
            gpmValue.Visible = true;
            lblCurrentGameCount.Visible = true;
            lblSeasonStatusLoadInfo.Visible = true;
            picLoad.Visible = true;
            #region ChangeLabel

            ChangeLabel(ThemeColor, lblCurrentGame, pnlLoad, new List<string> {
                        "Current game: ", //Text
                        "Regular", //FontStyle
                        ((float)(screenFontSize * pnlLoad.Height * .05) / (96 / 12) * (72 / 12)).ToString(), //FontSize
                        ".", //Width
                        "true", //AutoSize
                        "4", //Left
                        ".", //Top
                        ThemeColor.ToString(), //Color
                        "true", //Visible
                        "." //Height
                    }); //Current game: 
            gpm.Top = lblCurrentGame.Bottom;
            gpmValue.Top = gpm.Bottom;
            ChangeLabel(ThemeColor, lblSeasonStatusLoad, pnlLoad, new List<string> {
                        "Checking util.BuildLog", //Text
                        "Bold", //FontStyle
                        ((float)(screenFontSize * pnlLoad.Height * .08) / (96 / 12) * (72 / 12)).ToString(), //FontSize
                        ".", //Width
                        "true", //AutoSize
                        "0", //Left
                        ".", //Top
                        ThemeColor.ToString(), //Color
                        "true", //Visible
                        "." //Height
                    }); //Currently Loading: 
            ChangeLabel(ThemeColor, lblWorkingOn, pnlLoad, new List<string> {
                        "", //Text
                        "Regular", //FontStyle
                        ((float)(screenFontSize * pnlLoad.Height * .03) / (96 / 12) * (72 / 12)).ToString(), //FontSize
                        ".", //Width
                        "true", //AutoSize
                        (pnlLoad.Width - lblWorkingOn.Width).ToString(), //Left
                        lblSeasonStatusLoadInfo.Top.ToString(), //Top
                        ThemeColor.ToString(), //Color
                        "true", //Visible
                        "." //Height
                    }); //No text yet
            #endregion

            #endregion
        }
        public string GetLowestTable(int seasonID)
        {
            var season = seasonInfo.FirstOrDefault(s => s.SeasonID == seasonID);
            if (season.Equals(default))
            {
                return "Game";
            }

            var data = season.Item2;
            int games = data.Games;

            //check if all values equal Games
            if (data.Game == games && data.PlayerBox == games && data.TeamBox == games && data.PlayByPlay == games)
            {
                return "Game";
            }

            //find the lowest value among the four tables
            var tableValues = new Dictionary<string, int>
            {
                {"Game", data.Game},
                {"PlayerBox", data.PlayerBox},
                {"TeamBox", data.TeamBox},
                {"PlayByPlay", data.PlayByPlay}
            };

            int minValue = tableValues.Values.Min();
            return tableValues.FirstOrDefault(kvp => kvp.Value == minValue).Key;
        }
        public async Task RefreshClick()
        {
            RefreshDb_PreSelection();
            SqlConnection connection = new SqlConnection(bob.ToString());
            int buildID = 0;
            using (SqlCommand BuildLogCheck = new SqlCommand("BuildLogCheck", connection))
            {
                BuildLogCheck.CommandType = CommandType.StoredProcedure;
                connection.Open();
                using (SqlDataReader reader = BuildLogCheck.ExecuteReader())
                {
                    while (reader.Read())
                    {
                        buildID = reader.GetInt32(0);
                    }
                }
                connection.Close();
            }
            stopwatchFull.Restart();
            stopwatchRead.Restart();
            DateTime lastDate = DateTime.MinValue;
            string query = "";
            List<int> gameList = new List<int>();
            //query = "select Max(g.Date) Date from Game g where g.SeasonID = 2024";
            query = "select distinct GameID from " + GetLowestTable(2024) + " where SeasonID = 2024 order by GameID";
            using (SqlCommand lastDateQuery = new SqlCommand(query))
            {
                lastDateQuery.Connection = connection;
                lastDateQuery.CommandType = CommandType.Text;
                connection.Open();
                using (SqlDataReader sdr = lastDateQuery.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        //lastDate = sdr.GetDateTime(0);
                        gameList.Add(sdr.GetInt32(0));
                    }
                    gameList.RemoveAt(gameList.Count - 1);
                }
                connection.Close();
            }
            //scheduleGames = await leagueSchedule.GetJSON(lastDate);
            scheduleGames = await leagueSchedule.GetJSON(gameList);

            string values = " where GameID in(";
            List<int> games = new List<int>();
            foreach (NBAdbToolboxSchedule.Game game in scheduleGames)
            {
                values += Int32.Parse(game.GameId) + ", ";
                games.Add(Int32.Parse(game.GameId));
                TotalGames++;
            }
            stopwatchRead.Stop();
            TotalGamesCD = TotalGames - 1;

            RefreshDb_DeleteMissing(values, connection);
            InitializeDbLoad();
            PopulateDb_7_AfterCurrentDelete();
            stopwatchInsert.Restart();
            for (int i = 0; i < games.Count; i++)
            {
                GameID = games[i];
                await CurrentGameGPS(games[i], "Refresh");
                await TeamBoxLineupCalculation(GameID);
                PopulateDb_8_AfterCurrentGame(games[i].ToString());
            }
            PopulateDb_9_AfterSeasonInserts(buildID, 1, 0, "Current Refresh", 1, 1);
            stopwatchFull.Stop();
            PopulateDb_10_Completion();
            scheduleGames.Clear();
        }


        public async Task DownloadSeasonFiles()
        {
            string gitToken = "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";
            //Disable controls during download
            btnDownloadSeasonData.Enabled = false;
            ButtonChangeState(btnDownloadSeasonData, false);
            listDownloadSeasonData.Enabled = false;

            //Create progress bar
            ProgressBar progressBar = new ProgressBar
            {
                Width = listDownloadSeasonData.Width,
                Height = 20,
                Left = listDownloadSeasonData.Left,
                Top = btnDownloadSeasonData.Bottom + 10,
                Visible = true
            };
            pnlDbUtil.Controls.Add(progressBar);

            //Create status label
            Label lblDownloadStatus = new Label
            {
                Text = "Preparing download...",
                AutoSize = true,
                Left = progressBar.Left,
                Top = progressBar.Bottom + 5,
                ForeColor = ThemeColor,
                Visible = true
            };
            pnlDbUtil.Controls.Add(lblDownloadStatus);

            //Calculate total files to download
            int totalFiles = 0;
            foreach (string season in listDownloadSeasonData.SelectedItems)
            {
                int seasonID = int.Parse(season);
                int iter = (seasonID <= 2012 || seasonID == 2019 || seasonID == 2020) ? 3 : 4;
                totalFiles += iter;
            }

            progressBar.Maximum = totalFiles;
            progressBar.Value = 0;

            string historicDataPath = Path.Combine(projectRoot, @"Content\Historic Data\");
            int filesDownloaded = 0;

            using (HttpClient client = new HttpClient())
            {
                client.DefaultRequestHeaders.UserAgent.ParseAdd("Mozilla/5.0");
                client.DefaultRequestHeaders.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("token", gitToken);
                foreach (string season in listDownloadSeasonData.SelectedItems)
                {
                    int seasonID = int.Parse(season);
                    int iter = (seasonID <= 2012 || seasonID == 2019 || seasonID == 2020) ? 3 : 4;

                    for (int i = 0; i < iter; i++)
                    {
                        string fileName = $"{seasonID}p{i}.json";
                        string localPath = Path.Combine(historicDataPath, fileName);
                        string url = $"https://raw.githubusercontent.com/jakesjordan00/NBAdbToolbox/master/Content/Historic%20Data/{fileName}";

                        lblDownloadStatus.Text = $"Downloading {fileName}...";
                        lblDownloadStatus.Refresh();

                        try
                        {
                            //Download file
                            byte[] fileData = await client.GetByteArrayAsync(url);

                            //Save to disk
                            File.WriteAllBytes(localPath, fileData);

                            filesDownloaded++;
                            progressBar.Value = filesDownloaded;

                        }
                        catch (HttpRequestException ex)
                        {
                            MessageBox.Show($"Failed to download {fileName}: {ex.Message}", "Download Error",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                        catch (Exception ex)
                        {
                            MessageBox.Show($"Error saving {fileName}: {ex.Message}", "Save Error",
                                          MessageBoxButtons.OK, MessageBoxIcon.Error);
                        }
                    }
                }
            }

            //Cleanup
            pnlDbUtil.Controls.Remove(progressBar);
            pnlDbUtil.Controls.Remove(lblDownloadStatus);
            progressBar.Dispose();
            lblDownloadStatus.Dispose();

            //Re-enable controls
            btnDownloadSeasonData.Enabled = true;
            listDownloadSeasonData.Enabled = true;
            CheckDataFiles();
            

            //Check if all files downloaded
            if (listDownloadSeasonData.Items.Count == 0)
            {
                allFilesDownloaded = true;
                ButtonChangeState(btnDownloadSeasonData, false);
                MessageBox.Show("All seasons downloaded successfully!", "Download Complete",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                ButtonChangeState(btnDownloadSeasonData, true);
                MessageBox.Show($"Downloaded {filesDownloaded} files successfully", "Download Complete",
                              MessageBoxButtons.OK, MessageBoxIcon.Information);
            }

            //Play completion sound
            if (settings.Sound != "Muted")
            {
                PlayCompletionSound("Season");
            }
        }

        #region Theme
        public Color ThemeColor = new Color();
        public Color SubThemeColor = new Color();
        public Color SuccessColor = new Color();
        public Color LastSuccessColor = new Color();
        public Color WarningColor = Color.FromArgb(255, 230, 180, 30);
        public Color LastWarningColor = new Color();
        public Color ErrorColor = new Color();
        public Color LastErrorColor = new Color();
        public void GetThemeColors()
        {
            if (settings.BackgroundImage == "Court Dark")
            {
                Theme = "Dark";
                ThemeColor = Color.Ivory;
                SubThemeColor = Color.Black;
                SuccessColor = Color.FromArgb(255, 100, 220, 100);
                LastSuccessColor = Color.FromArgb(255, 20, 100, 20);
                ErrorColor = Color.FromArgb(255, 200, 50, 50);
                LastErrorColor = Color.FromArgb(255, 120, 30, 30);
            }
            else if (settings.BackgroundImage == "Court Default")
            {
                Theme = "Default";
                ThemeColor = Color.Black;
                SubThemeColor = Color.Ivory;
                SuccessColor = Color.FromArgb(255, 20, 100, 20);
                LastSuccessColor = Color.FromArgb(255, 100, 220, 100);
                ErrorColor = Color.FromArgb(255, 120, 30, 30);
                LastErrorColor = Color.FromArgb(255, 200, 50, 50);
            }
            else
            {
                Theme = "Default";
                ThemeColor = Color.Black;
                SubThemeColor = Color.Ivory;
            }
        }
        public void SetTheme(string sender)
        {

            //get controls by type
            List<Label> labels = GetAllControlsOfType<Label>(this);
            List<ComboBox> comboBoxes = GetAllControlsOfType<ComboBox>(this);
            List<Button> buttons = GetAllControlsOfType<Button>(this);
            List<ListBox> listBoxes = GetAllControlsOfType<ListBox>(this);

            //update labels
            foreach (Label lbl in labels)
            {
                if (lbl.ForeColor.ToArgb() == Color.Black.ToArgb() || lbl.ForeColor.ToArgb() == Color.Ivory.ToArgb())
                {
                    lbl.ForeColor = ThemeColor;
                }
                else if (lbl.ForeColor == SuccessColor || lbl.ForeColor == LastSuccessColor)
                {
                    lbl.ForeColor = SuccessColor;
                }
                else if (lbl.ForeColor == WarningColor || lbl.ForeColor == LastWarningColor)
                {
                    lbl.ForeColor = WarningColor;
                }
                else if (lbl.ForeColor == ErrorColor || lbl.ForeColor == LastErrorColor)
                {
                    lbl.ForeColor = ErrorColor;
                }
                if (lbl.Tag != null && lbl.Tag.ToString() != "Server Connection")
                {
                    lbl.BackColor = SubThemeColor;
                }
            }

            //update comboboxes
            foreach (ComboBox combo in comboBoxes)
            {
                combo.ForeColor = ThemeColor;
                combo.BackColor = SubThemeColor;
            }

            //update buttons
            foreach (Button btn in buttons)
            {
                btn.ForeColor = SubThemeColor;
                btn.BackColor = ThemeColor;
                
                if (!btn.Enabled)
                {
                    btn.BackColor = Color.Gainsboro;
                    // Use FlatStyle to have more control
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 2;
                    btn.FlatAppearance.BorderColor = Color.Black;
                }
                else
                {
                    btn.ForeColor = SubThemeColor;
                    btn.BackColor = ThemeColor;
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 2;
                    btn.FlatAppearance.BorderColor = Color.DodgerBlue;
                }
            }
            //update buttons
            foreach (ListBox lb in listBoxes)
            {
                lb.ForeColor = ThemeColor;
                lb.BackColor = SubThemeColor;
            }
        }
        #endregion

        //generic recursive method to get all controls of a specific type
        public List<T> GetAllControlsOfType<T>(Control parent) where T : Control
        {
            List<T> controls = new List<T>();

            foreach (Control control in parent.Controls)
            {
                if (control is T)
                {
                    controls.Add((T)control);
                }

                //recursively check child controls (panels, etc)
                if (control.HasChildren)
                {
                    controls.AddRange(GetAllControlsOfType<T>(control));
                }
            }
            return controls;
        }




        #region Settings
        private bool ConfigChanged(string sender)
        {
            if (sender == "btnEdit")
            {
                return true;
            }
            else if (configControl == null)
            {
                return false;
            }
            else 
            {
                return config.Server != configControl.Server ||
                       config.Alias != configControl.Alias ||
                       config.Database != configControl.Database ||
                       config.Create != configControl.Create ||
                       config.Default != configControl.Default ||
                       config.UseWindowsAuth != configControl.UseWindowsAuth ||
                       config.Username != configControl.Username ||
                       config.Password != configControl.Password;
            }
        }
        public void GetSettings(string sender) //Gets settings file values. If file doesnt exist, set defaults and create
        {
            if (File.Exists(settingsPath)) //If our file exists
            {
                if (sender == "Main")
                {
                    settingsJSON = "";
                    settingsJSON = File.ReadAllText(settingsPath);
                    settings = JsonConvert.DeserializeObject<Settings>(settingsJSON);
                }
                settingsControl = JsonConvert.DeserializeObject<Settings>(settingsJSON);
                DefaultSettings();
                //Set Background Image
                bgCourt.Image = Image.FromFile(Path.Combine(projectRoot, @"Content\Images", settings.BackgroundImage + ".png"));
                string backupDir = Path.Combine(projectRoot, @"Content\Configuration");
                string backupPath = Path.Combine(projectRoot, @"Content\Configuration\", settings.DefaultConfig);

                //Set Default dbconfig.json filepath
                if (File.Exists(Path.Combine(settings.ConfigPath, settings.DefaultConfig)))
                {
                    configPath = Path.Combine(settings.ConfigPath, settings.DefaultConfig);
                }
                else if (Directory.Exists(backupDir))
                {
                    string[] backupDirFiles = Directory.GetFiles(backupDir, "*.json").OrderByDescending(f => File.GetLastWriteTime(f)).ToArray();
                    string[] backupDirFileNames = Directory.GetFiles(backupDir, "*.json").OrderByDescending(f => File.GetLastWriteTime(f))
                    .Select(f => Path.GetFileName(f)).ToArray();
                    if (File.Exists(Path.Combine(backupDir, settings.DefaultConfig)))
                    {
                        configPath = Path.Combine(backupDir, settings.DefaultConfig);
                        settings.ConfigPath = Path.Combine(projectRoot, @"Content\Configuration");
                    }
                    else if (backupDirFiles.Count() > 0)
                    {
                        configPath = backupDirFiles[0];
                        settings.ConfigPath = backupDir;
                        settings.DefaultConfig = backupDirFileNames[0];
                    }
                    else if (!File.Exists(Path.Combine(backupDir, settings.DefaultConfig)))
                    {

                    }
                }
                else
                {
                    Directory.CreateDirectory(Path.Combine(projectRoot, @"Content\Configuration"));
                    defaultConfig = true;
                }
            }
            else if (!File.Exists(settingsPath)) //If our file doesnt exist, just set to defaults
            {
                DefaultSettings();
                defaultConfig = true;
            }
            if (SettingsChanged()) //If the settings have changed, write update to file
            {
                WriteSettings();
            }
            GetThemeColors();
        }
        public void DefaultSettings()
        {
            if (settings.ConfigPath == null)
            {
                settings.ConfigPath = Path.Combine(projectRoot, @"Content\Configuration");
            }
            if (settings.DefaultConfig == null)
            {
                settings.DefaultConfig = "dbconfig.json";
            }
            if (settings.BackgroundImage == null)
            {
                settings.BackgroundImage = "Court Default";
            }
            if (settings.WindowSize == null)
            {
                settings.WindowSize = "Default";
            }
            if (settings.Sound == null)
            {
                settings.Sound = "Default";
            }

        }
        public void WriteSettings()
        {
            File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
            settingsControl = JsonConvert.DeserializeObject<Settings>(JsonConvert.SerializeObject(settings));
        }
        private bool SettingsChanged()
        {
            return settings.ConfigPath != settingsControl.ConfigPath ||
                   settings.DefaultConfig != settingsControl.DefaultConfig ||
                   settings.BackgroundImage != settingsControl.BackgroundImage ||
                   settings.WindowSize != settingsControl.WindowSize ||
                   settings.Sound != settingsControl.Sound;
        }
        #endregion

        #region Config
        public string configName = "";
        public void InitializeDbConfig(string sender)
        {
            if (!File.Exists(configPath) || defaultConfig) //If our file doesnt exist
            {
                if(sender == "btnEdit")
                {
                    configName = "";
                    string db = "";
                    if (config.Database != null)
                    {
                        db = " - " + config.Database;
                    }
                    if (config.Alias != null)
                    {
                        configName = config.Alias + db;
                    }
                    else
                    {
                        configName = config.Server + db;
                    }
                    WriteConfig(sender);
                }
                else
                {
                    NoConnection();
                }
            }
            else if (File.Exists(configPath)) //If our file does exist
            {
                GetConfig(sender);
                lblStatus.Text = "Welcome Back!";
                lblStatus.ForeColor = ThemeColor;
                btnEdit.Text = "Edit Server/Db connection";
                btnEdit.Width = (int)(lblStatus.Width / 1.5);

                //Set label text
                if (config.Alias != null)
                {
                    lblServerName.Text = config.Alias;
                    ToolTip tip = new ToolTip();
                    tip.BackColor = SubThemeColor;
                    tip.ForeColor = ThemeColor;
                    tip.SetToolTip(lblServerName, config.Server);
                    tip.IsBalloon = true;
                }
                else
                {
                    lblServerName.Text = config.Server;
                }
                lblDbName.Text = config.Database;

                ClearImage(picStatus);
                if (isConnected)
                {
                    lblCStatus.Text = "Connected";
                    lblCStatus.ForeColor = SuccessColor;
                    // Load image
                    imagePath = Path.Combine(projectRoot, @"Content\Images", "Success.png");
                    picStatus.Image = Image.FromFile(imagePath);
                }
                else
                {
                    lblCStatus.Text = "Disconnected";
                    lblCStatus.ForeColor = ErrorColor;
                    // Load image
                    imagePath = Path.Combine(projectRoot, @"Content\Images", "Error.png");
                    picStatus.Image = Image.FromFile(imagePath);
                }
            }

        }
        public void NoConnection()
        {
            lblStatus.Text = "Set Connection Configuration";
            btnEdit.Text = "Create Server/Db connection";
            lblServerName.Text = "Not Connected.";
            lblCStatus.Text = "Disconnected";
            lblDbName.Text = "";
            lblDbStat.Text = "Could not connect using master or Db.";
            imagePath = Path.Combine(projectRoot, @"Content\Images", "Error.png");
            picStatus.Image = Image.FromFile(imagePath);
            if (!Directory.Exists(settings.ConfigPath))
            {
                settings.ConfigPath = Path.Combine(projectRoot, @"Content\Configuration");
            }
            UIController("NoConnection");

        }
        public void GetConfig(string sender) //Gets config file
        {
            if (sender == "Main")
            {
                json = "";
                json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<DbConfig>(json);
            }
            configControl = JsonConvert.DeserializeObject<DbConfig>(json);

            configName = "";
            string db = "";
            if (config.Database != null)
            {
                db = " - " + config.Database;
            }
            if (config.Alias != null)
            {
                configName = config.Alias + db;
            }
            else
            {
                configName = config.Server + db;
            }
            //Build connection string
            bob.DataSource = config.Server;
            if (config.UseWindowsAuth == true)
            {
                bob.IntegratedSecurity = true;
            }
            else
            {
                bob.UserID = config.Username;
                bob.Password = config.Password;
                bob.IntegratedSecurity = false;
            }
            cString = bob.ToString();
            if (config.Server != "" && ((config.Username != "" && config.Password != "") || config.UseWindowsAuth == true))
            {
                isConnected = TestConnectionString(cString);
            }
            if (isConnected) //If the connection string works for master
            {
                if (config.Create == false) //and if the config file says we dont need to create database, 
                {
                    bob.InitialCatalog = config.Database; //have the connection string use the database
                    dbConnection = TestConnectionString(bob.ToString());
                }
            }
            else //If the connection string doesnt work on master for whatever reason and our config file says we have a db, double check the db only connection string to make sure.
            {
                if (config.Create == false) //same as above
                {
                    bob.InitialCatalog = config.Database; //same as above
                    dbConnection = TestConnectionString(bob.ToString());
                }
            }
            if (config.Create == true)
            {
                dbConnection = false;
            }

            if (isConnected && dbConnection) //If both server and db connectionstrings work
            {
                DbExists();
            }
            else if (isConnected && !dbConnection) //If ONLY server connection string works
            {
                DbMissing();
            }
            else if (!isConnected && !dbConnection) //If neither work
            {
                BadConnection();
            }
            WriteConfig(sender);

            UIController("GetConfig");
        }
        public bool TestConnectionString(string connectionString) //Test Server connection
        {
            try
            {
                SqlConnectionStringBuilder builder = new SqlConnectionStringBuilder(connectionString);
                builder.ConnectTimeout = 3; //set 3 second timeout

                using (SqlConnection conn = new SqlConnection(builder.ToString()))
                {
                    conn.Open();
                    btnBuild.Enabled = true;
                    ButtonChangeState(btnBuild, true);
                    lblServerName.ForeColor = SuccessColor;
                    btnEdit.Text = "Edit Server/Db connection";
                    conn.Dispose();
                    return true; //connected successfully
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        public void DbExists() //If the Database exists: Set variables, disable build btn, enable populate
        {
            config.Create = false;
            lblDbOverview.Text = "Database Overview";
            lblDbOvName.Text = config.Database;
            lblDbName.Text = config.Database;
            lblDbStat.Text = "Database connected";
            imagePathDb = Path.Combine(projectRoot, @"Content\Images", "Success.png");
            ClearImage(picDbStatus);
            picDbStatus.Image = Image.FromFile(imagePathDb);
            cString = bob.ToString();
            UIController("DbExists");
        }
        public void DbMissing()
        {
            listSeasons.Items.Clear();
            seasonInfo.Clear();
            config.Create = true;
            lblDbName.Text = config.Database;
            lblDbStat.Text = "Need to create Database";
            imagePathDb = Path.Combine(projectRoot, @"Content\Images", "Warning.png");
            ClearImage(picDbStatus);
            picDbStatus.Image = Image.FromFile(imagePathDb);
            UIController("DbMissing");
            DbOverviewVisibility(false, "DbMissing");
        }
        public void BadConnection()
        {
            listSeasons.Items.Clear();
            seasonInfo.Clear();
            lblStatus.Text = "Invalid Connection!";
            UIController("BadConnection");
        }
        public void WriteConfig(string sender) //Writes Config File and Settings file
        {
            if (config.Default == true && sender != "boxChangeConfig")
            {
                settings.DefaultConfig = configName + ".json";
            }
            if (SettingsChanged()) //If the settings have changed, write update to file
            {
                WriteSettings();
            }
            if (ConfigChanged(sender) || settings.ConfigPath != settingsControl.ConfigPath) //If the config file has changed, write update to file
            {
                configPath = Path.Combine(settings.ConfigPath, configName + ".json");
                File.WriteAllText(Path.Combine(settings.ConfigPath, configName + ".json"), JsonConvert.SerializeObject(config, Formatting.Indented));
                configControl = JsonConvert.DeserializeObject<DbConfig>(JsonConvert.SerializeObject(config));
                ClearLoadInfo();
            }
        }
        #endregion

        public void ClearLoadInfo()
        {
            if (picLoad.Image != null)
            {
                picLoad.Image.Dispose();
                picLoad.Image = null;
            }
            gpmValue.Visible = false;
            gpm.Visible = false;
            lblWorkingOn.Visible = false;
            lblCurrentGameCount.Visible = false;
            lblCurrentGame.Visible = false;
            lblSeasonStatusLoadInfo.Visible = false;
            lblSeasonStatusLoad.Visible = false;

            AddPanelElement(pnlLoad, gpmValue);
            AddPanelElement(pnlLoad, gpm);
            AddPanelElement(pnlLoad, lblWorkingOn);
            AddPanelElement(pnlLoad, lblCurrentGameCount);
            AddPanelElement(pnlLoad, lblCurrentGame);
            AddPanelElement(pnlLoad, lblSeasonStatusLoadInfo);
            AddPanelElement(pnlLoad, lblSeasonStatusLoad);
            AddPanelElement(pnlLoad, picLoad);
        }

        public void ButtonChangeState(Button btn, bool enabled)
        {
            btn.Enabled = enabled;
            btn.ForeColor = SubThemeColor;
            btn.BackColor = ThemeColor;
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 2;
            btn.FlatAppearance.BorderColor = Color.DodgerBlue;
            if(btn.Text == "Populate Db")
            {

            }
            if (!enabled)
            {
                btn.FlatAppearance.BorderColor = Color.Black;
                btn.BackColor = Color.Gainsboro;
            }
            btn.AutoSize = true;
        }
        private bool isRefreshing = false;
        public void UIController(string sender)//If an event occurs that will change the state of the UI, it must run through here
        {
            float fontSize = ((float)(screenFontSize * pnlWelcome.Height * .08) / (96 / 12)) * (72 / 12);
            if (sender == "NoConnection")
            {
                ButtonChangeState(btnBuild, false);
                isBuildEnabled = false;
                ButtonChangeState(btnPopulate, false);
                listSeasons.Items.Clear();

                lblServerName.ForeColor = ErrorColor;
                lblCStatus.ForeColor = ErrorColor;
                lblDbName.ForeColor = ErrorColor;
                lblDbName.BackColor = Color.Transparent;
                lblDbStat.ForeColor = ErrorColor;
                lblDbStat.BackColor = Color.Transparent;
            }
            else if (sender == "GetConfig")
            {
                lblCStatus.Text = isConnected ? "Connected" : "Disconnected";
                lblCStatus.ForeColor = isConnected ? SuccessColor : ErrorColor;
                iconFile = isConnected ? "Success.png" : "Error.png";
                imagePath = Path.Combine(projectRoot, @"Content\Images", iconFile);
                ClearImage(picStatus);
                picStatus.Image = Image.FromFile(imagePath);
            }

            else if (sender == "DbExists")
            {
                ButtonChangeState(btnBuild, false);
                isBuildEnabled = false;
                ButtonChangeState(btnPopulate, true);
                ButtonChangeState(btnRefresh, true);
                lblDbOvName.Visible = true;

                lblDbOvName.ForeColor = SuccessColor;
                lblDbName.ForeColor = SuccessColor;
                lblDbName.BackColor = Color.Transparent;
                lblDbStat.ForeColor = SuccessColor;
                lblDbStat.BackColor = Color.Transparent;
                picDbStatus.BackColor = Color.Transparent;
            }
            else if (sender == "DbMissing")
            {
                ButtonChangeState(btnBuild, true);
                isBuildEnabled = true;
                ButtonChangeState(btnPopulate, false);
                lblDbOvName.Visible = false;
                listSeasons.Items.Clear();
                lblDbOvName.ForeColor = WarningColor;
                lblDbOvName.BackColor = Color.FromArgb(100, 0, 0, 0);
                lblDbName.ForeColor = WarningColor;
                lblDbName.BackColor = Color.FromArgb(100, 0, 0, 0);
                lblDbName.AutoSize = true;
                lblDbStat.ForeColor = WarningColor;
                lblDbStat.BackColor = Color.FromArgb(100, 0, 0, 0);
                lblDbStat.AutoSize = true;
                picDbStatus.Left = lblDbName.Right;
            }
            else if (sender == "BadConnection")
            {
                ButtonChangeState(btnBuild, true);
                isBuildEnabled = true;
                ButtonChangeState(btnPopulate, false);
                lblDbOvName.Visible = false;
                listSeasons.Items.Clear();

                CenterElement(pnlWelcome, lblStatus);
                lblDbName.ForeColor = ErrorColor;
                lblDbName.BackColor = Color.Transparent;
                lblDbName.AutoSize = true;
                lblDbStat.ForeColor = ErrorColor;
                lblDbStat.BackColor = Color.Transparent;
                lblDbName.AutoSize = true;
                picDbStatus.Left = lblDbName.Right;
            }

            //Do big Status label first
            //lblStatus.Height = (int)(pnlWelcome.Height * .1);
            //lblStatus.Font = SetFontSize("Segoe UI", ((float)(lblStatus.Height) / (96 / 12)) * (72 / 12), FontStyle.Bold, pnlWelcome, lblStatus);
            //If CStatus = Disconnected, make font smaller
            if (!isConnected)
            {
                lblStatus.Height = (int)(pnlWelcome.Height * .1);
                lblCStatus.Font = SetFontSize("Segoe UI", ((float)(screenFontSize * lblServer.Height * .9) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, (int)(pnlWelcome.Width * .7), lblCStatus);
            }
            else //If we're connected, use normal sized font
            {
                lblStatus.Height = (int)(pnlWelcome.Height * .1);
                lblCStatus.Font = SetFontSize("Segoe UI", ((float)(screenFontSize * lblServer.Height) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, (int)(pnlWelcome.Width * .7), lblCStatus);
            }

            if (sender == "GetConfig")
            {
                lblCStatus.AutoSize = true;
                lblStatus.Left = (pnlWelcome.ClientSize.Width - lblStatus.Width) / 2;
                lblCStatus.Left = pnlWelcome.Width - (lblCStatus.Width + picStatus.Width);
                picStatus.Left = lblCStatus.Right;
                lblDbName.AutoSize = true;
                lblDbStat.AutoSize = true;
                picDbStatus.Left = lblDbName.Right;
                lblDbUtil.Text = "Database Utilities";
                lblDbUtil.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, (int)(pnlDbUtil.Width * .7), lblDbUtil);
                lblDbUtil.AutoSize = true;
                lblDbUtil.Left = (pnlDbUtil.Width - lblDbUtil.Width) / 2;
                CheckDataFiles(); //GetSeasons();
            }
            if (allFilesDownloaded)
            {
                ButtonChangeState(btnDownloadSeasonData, false);
            }


        }
        public void RefreshDefaultConfigPath(string sender)
        {
            isRefreshing = true;
            boxConfigFiles.Items.Clear();
            boxChangeConfig.Items.Clear();
            string[] configFiles = Directory.GetFiles(settings.ConfigPath);
            string settingsDefConfig = "";
            int intSettingsDefConfig = 0;
            for (int i = 0; i < configFiles.Length; i++)
            {
                string currentFileName = Path.GetFileName(configFiles[i]).Replace(".json", "");
                boxConfigFiles.Items.Add(Path.GetFileName(configFiles[i]).Replace(".json", ""));
                boxChangeConfig.Items.Add(Path.GetFileName(configFiles[i]).Replace(".json", ""));
                //select if it's the default
                if (currentFileName == settings.DefaultConfig.Replace(".json", ""))
                {
                    if (sender == "Main")
                    {
                        boxConfigFiles.SelectedIndex = i;
                    }
                    settingsDefConfig = settings.DefaultConfig.Replace(".json", "");
                    intSettingsDefConfig = i;
                }
                if (sender != "Main" && currentFileName == configName)
                {
                    if (config.Default == true)
                    {
                        boxConfigFiles.SelectedIndex = i;
                    }
                    else
                    {
                        boxConfigFiles.SelectedIndex = intSettingsDefConfig;
                    }
                }
            }
            boxChangeConfig.SelectedIndex = boxConfigFiles.SelectedIndex;
            isRefreshing = false;
        }


        //Create Database using build.sql file
        public void CreateDB(string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string createAlter = "use master create database " + config.Database + "; " +
                "alter database " + config.Database + " set auto_shrink off; " +
                "alter database " + config.Database + " set auto_update_statistics on; " +
                "alter database " + config.Database + " set auto_update_statistics_async on; " +
                "alter database " + config.Database + " set recovery simple; " +
                "alter database " + config.Database + " modify file (name = " + config.Database + ", size = 1024mb, filegrowth = 256mb); " +
                "alter database " + config.Database + " modify file (name = " + config.Database + "_log, size = 1024mb, filegrowth = 256mb);";


                using (SqlCommand InsertData = new SqlCommand(createAlter))
                {
                    InsertData.Connection = conn;
                    InsertData.CommandType = CommandType.Text;
                    conn.Open();
                    try
                    {
                        InsertData.ExecuteScalar();
                        config.Create = false;
                        bob.InitialCatalog = config.Database;
                    }
                    catch (SqlException ex)
                    {

                    }
                    conn.Close();
                }
                connectionString = bob.ToString();
            }
            dbConnection = TestConnectionString(connectionString);
            if (dbConnection)
            {
                if (ConfigChanged("dbConnection")) //If the config file has changed, write update to file
                {
                    WriteConfig("FirstDbConTest");
                }
            }
            //CheckServer(connectionString, "create");
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand CreateTables = new SqlCommand(buildFile))
                {
                    CreateTables.Connection = conn;
                    CreateTables.CommandType = CommandType.Text;
                    conn.Open();
                    try
                    {
                        CreateTables.ExecuteScalar();
                        config.Create = false;
                        ButtonChangeState(btnBuild, false);
                        FormatProcedures();
                    }
                    catch (SqlException ex)
                    {

                    }
                    conn.Close();
                }
                for (int i = 0; i < procs.Count; i++)
                {
                    using (SqlCommand CreateProcedures = new SqlCommand(procs[i]))
                    {
                        CreateProcedures.Connection = conn;
                        CreateProcedures.CommandType = CommandType.Text;
                        conn.Open();
                        try
                        {
                            CreateProcedures.ExecuteScalar();
                        }
                        catch (SqlException ex)
                        {

                        }
                        conn.Close();
                    }
                }
                using (SqlCommand InsertSeasons = new SqlCommand("SeasonInsert"))
                {
                    InsertSeasons.Connection = conn;
                    InsertSeasons.CommandType = CommandType.StoredProcedure;
                    conn.Open();
                    try
                    {
                        InsertSeasons.ExecuteScalar();
                    }
                    catch (SqlException ex)
                    {

                    }
                    conn.Close();
                }
                if (ConfigChanged("SeasonInsert")) //If the config file has changed, write update to file
                {
                    WriteConfig("After SeasonInsert");
                }
                DbExists();
            }
        }
        //Formats procedures from build.sql file
        public void FormatProcedures()
        {
            string search = "~~~";
            List<int> indices = new List<int>();
            int startIndex = 0;
            for (int i = 0; i <= procedures.Length - search.Length; i++)
            {
                if (procedures.Substring(i, search.Length) == search)
                {
                    indices.Add(i);
                    procs.Add(procedures.Substring(startIndex, i - startIndex));
                    startIndex = i + search.Length;
                }
            }
        }




        public void RefreshDb_PreSelection()
        {
            TotalGames = 0;
            TotalGamesCD = 0;
            iterator = 0;
            imageIteration = 1;
            reverse = false;
            scheduleGames.Clear();
            SeasonID = 2024;
            completionMessage += SeasonID + ": ";
            ButtonChangeState(btnPopulate, true);
            ButtonChangeState(btnEdit, false);
            listSeasons.Enabled = false;
            ButtonChangeState(btnRefresh, false);
            stopwatch = Stopwatch.StartNew();
            start = DateTime.Now;
            timeElapsedRead = TimeSpan.Zero;
            elapsedStringRead = "";
            ChangeLabel(ThemeColor, lblSeasonStatusLoad, pnlLoad, new List<string> {
            "Finding missing games...", //Text
            "Bold", //FontStyle
            ((float)(screenFontSize * pnlLoad.Height * .075) / (96 / 12) * (72 / 12)).ToString(), //FontSize
            ".", //Width
            "true", //AutoSize
            "0", //Left
            ".", //Top
            ThemeColor.ToString(), //Color
            "true", //Visible
            "." //Height
            }); //Hitting endpoints and inserting

        }
        public void RefreshDb_DeleteMissing(string values, SqlConnection connection)
        {
            ChangeLabel(ThemeColor, lblSeasonStatusLoad, pnlLoad, new List<string> {
            "Deleting any incomplete games...", //Text
            "Bold", //FontStyle
            ((float)(screenFontSize * pnlLoad.Height * .075) / (96 / 12) * (72 / 12)).ToString(), //FontSize
            ".", //Width
            "true", //AutoSize
            "0", //Left
            ".", //Top
            ThemeColor.ToString(), //Color
            "true", //Visible
            "." //Height
            }); //Hitting endpoints and inserting
            stopwatchDelete.Restart();
            string delete = "delete from ";
            string cmd = "";
            if (scheduleGames.Count > 0)
            {
                values = values.Remove(values.Length - 3) + ")";
                cmd = delete + "StartingLineups" + values + "\n"
                    + delete + "TeamBoxLineups" + values + "\n"
                    + delete + "PlayByPlay" + values + "\n"
                    + delete + "PlayerBox" + values + "\n"
                    + delete + "TeamBox" + values + "\n"
                    + delete + "GameExt" + values + "\n"
                    + delete + "Game" + values;
                try
                {
                    using (SqlCommand deleteExisting = new SqlCommand(cmd))
                    {
                        deleteExisting.Connection = connection;
                        deleteExisting.CommandType = CommandType.Text;
                        connection.Open();
                        deleteExisting.ExecuteNonQuery();
                        connection.Close();
                    }
                }
                catch
                {

                }
            }
            stopwatchDelete.Stop();
        }

        #region Populate Database
        public void PopulateDb_1_PreSelection()
        {
            RegularSeasonGames = 0;
            PostseasonGames = 0;
            TotalGames = 0;
            TotalGamesCD = 0;
            sqlBuilder.Capacity = 4 * 1024;
            playByPlayBuilder.Capacity = 245 * 1024;
            sqlBuilderParallel.Capacity = 40 * 1024;
            ButtonChangeState(btnPopulate, false);
            ButtonChangeState(btnEdit, false);
            ButtonChangeState(btnRefresh, false);
            listSeasons.Enabled = false;
            iterator = 0;
            imageIteration = 1;
            reverse = false;
            completionMessage += SeasonID + ": ";
            lblStatus.Text = "Loading " + SeasonID + " season...";
            CenterElement(pnlWelcome, lblStatus);
            stopwatch = Stopwatch.StartNew();
            start = DateTime.Now;
            timeElapsedRead = TimeSpan.Zero;
            elapsedStringRead = "";
        }
        #region Historic
        public void PopulateDb_2_AfterHistoricRead()
        {
            stopwatchRead.Stop();
            timeElapsedRead = stopwatchRead.Elapsed;
            timeUnitsRead = new Dictionary<string, (int value, string sep)>
                            {
                                { "h", (timeElapsedRead.Hours, ":") },
                                { "m", (timeElapsedRead.Minutes, ":") },
                                { "s", (timeElapsedRead.Seconds, ".") },
                                { "ms", (timeElapsedRead.Milliseconds, "") }
                            };
            elapsedStringRead = CheckTime(timeUnitsRead);
            //End season read
            lblStatus.Text = "Deleting existing " + SeasonID + " data...";
            CenterElement(pnlWelcome, lblStatus);
            ChangeLabel(ThemeColor, lblSeasonStatusLoad, pnlLoad, new List<string> {
            "Deleting any " + SeasonID + " data, one sec...", //Text
            "Bold", //FontStyle
            ((float)(screenFontSize * pnlLoad.Height * .075) / (96 / 12) * (72 / 12)).ToString(), //FontSize
            ".", //Width
            "true", //AutoSize
            "0", //Left
            ".", //Top
            ThemeColor.ToString(), //Color
            "true", //Visible
            "." //Height
            }); //Hitting endpoints and inserting 

        }
        public void PopulateDb_3_AfterDelete_BeforeGames()
        {
            lblStatus.Text = SeasonID + " parsed. Inserting data...";
            CenterElement(pnlWelcome, lblStatus);
            lblSeasonStatusLoad.Text = "Inserting " + SeasonID + " Regular Season";
            stopwatchInsert.Restart();
            RegularSeasonGames = root.season.games.regularSeason.Count;
            PostseasonGames = root.season.games.playoffs.Count;
            TotalGames = RegularSeasonGames + PostseasonGames;
            sqlBuilder.Clear();
            playByPlayBuilder.Clear();

        }
        public void PopulateDb_4_AfterHistoricGame(string sender)
        {
            ImageDriver();
            if(sender == "Retry")
            {
                iterator--;
            }
            int gamesLeft = TotalGames - iterator;
            double gamesPerSec = iterator / stopwatchInsert.Elapsed.TotalSeconds;
            double gamesPerMin = Math.Round(gamesPerSec * 60, 2);
            double estimatedSeconds = gamesLeft / gamesPerSec;
            TimeSpan timeRemaining = TimeSpan.FromSeconds(estimatedSeconds);
            string time = timeRemaining.ToString();
            time = time.Remove(time.Length - 3);
            gpmValue.Text = gamesPerMin + "\n" + time;
        }
        #endregion
        #region Current
        public void PopulateDb_5_BeforeCurrentRead()
        {
            gamesRS = new List<int>();
            gamesPS = new List<int>();
            lblSeasonStatusLoad.Text = "Grabbing " + SeasonID + " GameIDs";
            lblSeasonStatusLoad.AutoSize = true;
            stopwatchRead.Restart();
        }
        public void PopulateDb_6_AfterCurrentReadRoot()
        {
            stopwatchRead.Stop();
            timeElapsedRead = stopwatchRead.Elapsed;
            timeUnitsRead = new Dictionary<string, (int value, string sep)>
                            {
                                { "h", (timeElapsedRead.Hours, ":") },
                                { "m", (timeElapsedRead.Minutes, ":") },
                                { "s", (timeElapsedRead.Seconds, ".") },
                                { "ms", (timeElapsedRead.Milliseconds, "") }
                            };
            elapsedStringRead = CheckTime(timeUnitsRead);
            for (int i = 0; i < root.season.games.regularSeason.Count; i++)
            {
                RegularSeasonGames++;
                GameID = Int32.Parse(root.season.games.regularSeason[i].game_id);
                gamesRS.Add(GameID);
            }
            for (int i = 0; i < root.season.games.playoffs.Count; i++)
            {
                PostseasonGames++;
                GameID = Int32.Parse(root.season.games.playoffs[i].game_id);
                gamesPS.Add(GameID);
            }
            TotalGames = RegularSeasonGames + PostseasonGames;
            TotalGamesCD = TotalGames - 1;
            iterator = 0;
            imageIteration = 1;
            reverse = false;

            lblStatus.Text = "Deleting existing " + SeasonID + " data...";
            CenterElement(pnlWelcome, lblStatus);
            ChangeLabel(ThemeColor, lblSeasonStatusLoad, pnlLoad, new List<string> {
                            "Deleting any " + SeasonID + " data, one sec...", //Text
                            "Bold", //FontStyle
                            ((float)(screenFontSize * pnlLoad.Height * .075) / (96 / 12) * (72 / 12)).ToString(), //FontSize
                            ".", //Width
                            "true", //AutoSize
                            "0", //Left
                            ".", //Top
                            ThemeColor.ToString(), //Color
                            "true", //Visible
                            "." //Height
                            }); //Hitting endpoints and inserting 
        }
        public void PopulateDb_6_AfterCurrentReadSchedule()
        {
            stopwatchRead.Stop();
            timeElapsedRead = stopwatchRead.Elapsed;
            timeUnitsRead = new Dictionary<string, (int value, string sep)>
                            {
                                { "h", (timeElapsedRead.Hours, ":") },
                                { "m", (timeElapsedRead.Minutes, ":") },
                                { "s", (timeElapsedRead.Seconds, ".") },
                                { "ms", (timeElapsedRead.Milliseconds, "") }
                            };
            elapsedStringRead = CheckTime(timeUnitsRead);
            foreach (NBAdbToolboxSchedule.Game game in scheduleGames)
            {
                if (game.GameId.Substring(2, 1) == "2")
                {
                    RegularSeasonGames++;
                    GameID = Int32.Parse(game.GameId);
                    gamesRS.Add(GameID);
                }
                else if (game.GameId.Substring(2, 1) != "1" && game.GameId.Substring(2, 1) != "3" && game.GameId.Substring(2, 1) != "6")
                {
                    PostseasonGames++;
                    GameID = Int32.Parse(game.GameId);
                    gamesPS.Add(GameID);
                }
                if (game.GameId.Substring(2, 1) == "6")
                {
                    RegularSeasonGames++;
                    GameID = Int32.Parse(game.GameId);
                    gamesRS.Add(GameID);
                }

            }
            TotalGames = RegularSeasonGames + PostseasonGames;
            TotalGamesCD = TotalGames - 1;
            iterator = 0;
            imageIteration = 1;
            reverse = false;

            lblStatus.Text = "Deleting existing " + SeasonID + " data...";
            CenterElement(pnlWelcome, lblStatus);
            ChangeLabel(ThemeColor, lblSeasonStatusLoad, pnlLoad, new List<string> {
                            "Deleting any " + SeasonID + " data, one sec...", //Text
                            "Bold", //FontStyle
                            ((float)(screenFontSize * pnlLoad.Height * .075) / (96 / 12) * (72 / 12)).ToString(), //FontSize
                            ".", //Width
                            "true", //AutoSize
                            "0", //Left
                            ".", //Top
                            ThemeColor.ToString(), //Color
                            "true", //Visible
                            "." //Height
                            }); //Hitting endpoints and inserting 
        }
        public void PopulateDb_7_AfterCurrentDelete()
        {
            stopwatchInsert.Restart();
            lblStatus.Text = "Loading " + SeasonID + "...";
            CenterElement(pnlWelcome, lblStatus);
            ChangeLabel(ThemeColor, lblSeasonStatusLoad, pnlLoad, new List<string> {
            "Hitting endpoints and inserting " + SeasonID + " data", //Text
            "Bold", //FontStyle
            ((float)(screenFontSize * pnlLoad.Height * .075) / (96 / 12) * (72 / 12)).ToString(), //FontSize
            ".", //Width
            "true", //AutoSize
            "0", //Left
            ".", //Top
            ThemeColor.ToString(), //Color
            "true", //Visible
            "." //Height
            }); //Hitting endpoints and inserting
        }
        public void PopulateDb_8_AfterCurrentGame(string currentGame)
        {
            UpdateLoadingImage(imageIteration);
            ImageDriver();
            lblCurrentGameCount.Text = currentGame;
            int gamesLeft = TotalGames - iterator;
            double gamesPerSec = iterator / stopwatchInsert.Elapsed.TotalSeconds;
            double gamesPerMin = Math.Round(gamesPerSec * 60, 2);
            double estimatedSeconds = gamesLeft / gamesPerSec;
            TimeSpan timeRemaining = TimeSpan.FromSeconds(estimatedSeconds);
            string time = timeRemaining.ToString();
            time = time.Remove(time.Length - 3);
            gpmValue.Text = gamesPerMin + "\n" + time;

        }
        public void PopulateDb_9_AfterSeasonInserts(int BuildID, int current, int historic, string source, int seasonIterator, int selectedSeasons)
        {
            //Measures time taken to Read and Insert data, by season and in total
            #region All Time Tracking operations

            //Measures total time taken for data read and insert.
            #region Total time taken for given Season
            stopwatch.Stop();
            TimeSpan timeElapsedSeason = stopwatch.Elapsed;
            Dictionary<string, (int, string)> timeUnitsSeason = new Dictionary<string, (int value, string sep)>
                        {
                        { "h", (timeElapsedSeason.Hours, ":") },
                        { "m", (timeElapsedSeason.Minutes, ":") },
                        { "s", (timeElapsedSeason.Seconds, ".") },
                        { "ms", (timeElapsedSeason.Milliseconds, "") }
                        };
            string elapsedStringSeason = CheckTime(timeUnitsSeason);
            #endregion

            //Measures time taken to insert data
            #region Time taken to insert data for given season
            stopwatchInsert.Stop();
            TimeSpan timeElapsedInsert = stopwatchInsert.Elapsed;
            Dictionary<string, (int, string)> timeUnitsInsert = new Dictionary<string, (int value, string sep)>
                        {
                        { "h", (timeElapsedInsert.Hours, ":") },
                        { "m", (timeElapsedInsert.Minutes, ":") },
                        { "s", (timeElapsedInsert.Seconds, ".") },
                        { "ms", (timeElapsedInsert.Milliseconds, "") }
                        };
            string elapsedStringInsert = CheckTime(timeUnitsInsert);
            DateTime end = DateTime.Now;
            #endregion

            #endregion

            #region Build Log Insert
            try
            {
                using (SqlConnection Main = new SqlConnection(bob.ToString()))
                using (SqlCommand BuildLogInsert = new SqlCommand("BuildLogInsert", Main))
                {
                    BuildLogInsert.CommandType = CommandType.StoredProcedure;
                    BuildLogInsert.Parameters.AddWithValue("@BuildID", BuildID);
                    BuildLogInsert.Parameters.AddWithValue("@Season", SeasonID);
                    BuildLogInsert.Parameters.AddWithValue("@Hr", timeElapsedSeason.Hours);
                    BuildLogInsert.Parameters.AddWithValue("@Min", timeElapsedSeason.Minutes);
                    BuildLogInsert.Parameters.AddWithValue("@Sec", timeElapsedSeason.Seconds);
                    BuildLogInsert.Parameters.AddWithValue("@Ms", timeElapsedSeason.Milliseconds);
                    BuildLogInsert.Parameters.AddWithValue("@FullTime", elapsedStringSeason);
                    BuildLogInsert.Parameters.AddWithValue("@HrR", timeElapsedRead.Hours);
                    BuildLogInsert.Parameters.AddWithValue("@MinR", timeElapsedRead.Minutes);
                    BuildLogInsert.Parameters.AddWithValue("@SecR", timeElapsedRead.Seconds);
                    BuildLogInsert.Parameters.AddWithValue("@MsR", timeElapsedRead.Milliseconds);
                    BuildLogInsert.Parameters.AddWithValue("@ReadTime", elapsedStringRead);
                    BuildLogInsert.Parameters.AddWithValue("@HrI", timeElapsedInsert.Hours);
                    BuildLogInsert.Parameters.AddWithValue("@MinI", timeElapsedInsert.Minutes);
                    BuildLogInsert.Parameters.AddWithValue("@SecI", timeElapsedInsert.Seconds);
                    BuildLogInsert.Parameters.AddWithValue("@MsI", timeElapsedInsert.Milliseconds);
                    BuildLogInsert.Parameters.AddWithValue("@InsertTime", elapsedStringInsert);
                    BuildLogInsert.Parameters.AddWithValue("@DatetimeStarted", start);
                    BuildLogInsert.Parameters.AddWithValue("@DatetimeComplete", end);
                    BuildLogInsert.Parameters.AddWithValue("@Historic", historic);
                    BuildLogInsert.Parameters.AddWithValue("@Current", current);
                    BuildLogInsert.Parameters.AddWithValue("@Source", source);
                    Main.Open();
                    BuildLogInsert.ExecuteNonQuery();
                }
            }
            catch (Exception ex)
            {

            }

            #endregion
            #region Completion Message
            completionMessage += elapsedStringSeason + ". ";
            if(source == "Current Refresh")
            {
                completionMessage += iterator + " games.";
            }
            else
            {
                completionMessage += iterator + " games, " + RegularSeasonGames + "/" + PostseasonGames + "\n";
            }
            int lblLeft = lblWorkingOn.Width;
            ChangeLabel(ThemeColor, lblWorkingOn, pnlLoad, new List<string>
                        {
             /*Text*/       completionMessage,
             /*FontStyle*/  "Regular",
             /*FontSize*/   ((float)(screenFontSize * pnlLoad.Height * .03) / (96 / 12) * (72 / 12)).ToString(),
             /*Width*/      ".",
             /*Autosize*/   "true",
             /*Left*/       (pnlLoad.Width - lblWorkingOn.Width).ToString(),
             /*Top*/        "0",
             /*Color*/      ThemeColor.ToString(),
             /*Visible*/    "true",
             /*Height*/     "."
                        });
            lblWorkingOn.Left = pnlLoad.Width - lblWorkingOn.Width;
            if(source == "Current")
            {
                lblWorkingOn.Top = lblSeasonStatusLoad.Bottom;
            }
            else 
            {
                lblWorkingOn.Top = 0;
            }
            #endregion

            if (seasonIterator != selectedSeasons && settings.Sound != "Muted")
            {
                PlayCompletionSound("Season");
            }
            teamsDone = false;
            arenasDone = false;
            playersDone = false;
            arenaList.Clear();
            officialList.Clear();
            playerList.Clear();
            teamList.Clear();
            timeElapsedRead = TimeSpan.Zero;
            elapsedStringRead = "";
            iterator = 0;
            root = null;
            rootC = null;
            rootCPBP = null;
            schedule = null;
            SeasonID = 0;
            GameID = 0;
            RegularSeasonGames = 0;
            PostseasonGames = 0;
            TotalGames = 0;
            TotalGamesCD = 0;
        }

        #endregion
        public void PopulateDb_10_Completion()
        {
            TimeSpan timeElapsedFull = stopwatchFull.Elapsed;
            Dictionary<string, (int, string)> timeUnitsFull = new Dictionary<string, (int value, string sep)>
                        {
                        { "h", (timeElapsedFull.Hours, ":") },
                        { "m", (timeElapsedFull.Minutes, ":") },
                        { "s", (timeElapsedFull.Seconds, ".") },
                        { "ms", (timeElapsedFull.Milliseconds, "") }
                        };
            string elapsedStringFull = CheckTime(timeUnitsFull);
            if (settings.Sound != "Muted")
            {
                PlayCompletionSound("Run");
            }
            #region Enable buttons and clear label text
            ButtonChangeState(btnPopulate, true);
            ButtonChangeState(btnEdit, true);
            ButtonChangeState(btnRefresh, true);
            listSeasons.Enabled = true;

            lblStatus.Text = "Welcome Back!";
            CenterElement(pnlWelcome, lblStatus);

            //lblCurrentGame

            ChangeLabel(ThemeColor, lblSeasonStatusLoadInfo, pnlLoad, new List<string> { "", ".", ".", ".", ".", ".", ".", ".", "false", "." });
            //...............................................................Text,  FontStyle, FontSize, Width, AutoSize, Left, Top, Color, Visible, Height
            ChangeLabel(ThemeColor, lblCurrentGameCount, pnlLoad, new List<string> { "", ".", ".", ".", ".", ".", ".", ".", "false", "." });
            //...........................................................Text,  FontStyle, FontSize, Width, AutoSize, Left, Top, Color, Visible, Height
            ChangeLabel(SuccessColor, lblCurrentGame, pnlLoad, new List<string> {
                        "Full Load: " + elapsedStringFull,
                        "Bold",
                        ((float)(screenFontSize * pnlLoad.Height * .06) / (96 / 12) * (72 / 12)).ToString(),
                        ".",
                        "true",
                        ".",
                        ".",
                        SuccessColor.ToString(),
                        "true",
                        "." }
            );//Done! Check your SQL db
            gpm.Top = lblCurrentGame.Bottom;
            gpmValue.Top = gpm.Bottom;
            ChangeLabel(SuccessColor, lblSeasonStatusLoad, pnlLoad, new List<string> {
                        "Done! Check your SQL db",
                        "Regular",
                        ((float)(screenFontSize * pnlLoad.Height * .08) / (96 / 12) * (72 / 12)).ToString(),
                        ".",
                        "true",
                        ".",
                        ".",
                        SuccessColor.ToString(),
                        "true",
                        "." }
            );//Done! Check your SQL db

            float workingOnTop = ((float)(screenFontSize * pnlLoad.Height * .033) / (96 / 12) * (72 / 12));
            ChangeLabel(ThemeColor, lblWorkingOn, pnlLoad, new List<string> {
                        ".", //Text
                        "Bold", //FontStyle
                        workingOnTop.ToString(), //FontSize
                        ".", //Width
                        "true", //AutoSize
                        ".", //Left
                        "0", //Top
                        ".", //Color
                        "true",//Visible
                        "." } //Height
            );//No text
            lblWorkingOn.Left = pnlLoad.Width - lblWorkingOn.Width;
            //working on top = 8.3655 with 2001 as last season = .033
            if (pnlLoad.Height - lblWorkingOn.Height <= 0)
            {
                workingOnTop = ((float)(screenFontSize * pnlLoad.Height * .03) / (96 / 12) * (72 / 12));
            }
            //working on top = 8.11199951 = .032
            ChangeLabel(ThemeColor, lblWorkingOn, pnlLoad, new List<string> {
                        ".", //Text
                        "Bold", //FontStyle
                        workingOnTop.ToString(), //FontSize
                        ".", //Width
                        "true", //AutoSize
                        ".", //Left
                        "0", //Top
                        ".", //Color
                        "true",//Visible
                        "." } //Height
            );//No text
            // 8.11199951 works for 99
            //Clear out image if it exists
            if (picLoad.Image != null)
            {
                picLoad.Image.Dispose();
                picLoad.Image = null;
            }
            //Show Success image
            imagePath = Path.Combine(projectRoot, @"Content\Images", "Success.png");
            if (File.Exists(imagePath))
            {
                try
                {
                    using (var img = Image.FromFile(imagePath))
                    {
                        picLoad.Image = new Bitmap(img);
                    }

                    // Update layout calculations here - assuming they need to happen each time
                    picLoad.Width = (int)(pnlLoad.Height * .5);
                    picLoad.Height = (int)(pnlLoad.Height * .5);
                    picLoad.Top = gpm.Bottom + (int)(picLoad.Height * .25);
                    picLoad.Left = ((pnlLoad.ClientSize.Width - picLoad.Width) / 2) - picLoad.Width;
                }
                catch
                {
                    // Handle loading failure gracefully
                }
            }
            #endregion

        }
        #endregion

        #region Initializations
        public void AddControls(string sender)
        {
            if(sender == "Init")
            {
                screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
                screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
                this.Width = (int)(screenWidth / 1.5);
                this.Height = (int)(screenHeight / 1.2);
                aspectRatio = (float)((screenWidth / 1.5) / (screenHeight / 1.2));
                this.MaximumSize = new Size((int)(screenWidth * 1.1), (int)(screenHeight * 1.1));
                this.StartPosition = FormStartPosition.Manual;
                this.Left = (screenWidth - this.Width) / 2;
                this.Top = (screenHeight - this.Height) / 2;
                this.FormBorderStyle = FormBorderStyle.FixedDialog;
                this.MaximizeBox = true;
                this.MinimizeBox = true;
            }
            if (screenWidth >= 2240)
            {
                screenFontSize = 1;
            }
            else if (screenWidth >= 1920 && screenWidth < 2240)
            {
                screenFontSize = 1.05f;
            }
            else if (screenWidth >= 1536 && screenWidth < 1920)
            {
                screenFontSize = 1.1f;
            }
            else if (screenWidth >= 1366 && screenWidth < 1536)
            {
                screenFontSize = 1.3f;
            }
            else if (screenWidth >= 1280 && screenWidth < 1366)
            {
                screenFontSize = 2;
            }
            #region Set and declare variables
            lblServer.Text = "Server: ";
            lblDB.Text = "Database: ";
            isConnected = false;
            bgCourt.Width = this.Width;
            bgCourt.Height = this.Height;
            #endregion

            #region Set Welcome and Database Utilities panel sizes
            pnlWelcome.BorderStyle = BorderStyle.FixedSingle;
            pnlWelcome.Width = (int)(this.ClientSize.Width / 2.5);
            pnlWelcome.Height = (int)(this.ClientSize.Height / 2.5);
            pnlWelcome.Left = (this.ClientSize.Width - pnlWelcome.Width) / 2;
            pnlWelcome.Top = (this.ClientSize.Height - pnlWelcome.Height) / 2;
            pnlWelcome.BackColor = Color.Transparent;

            //DbUtil
            pnlDbUtil.Height = this.Height;
            pnlDbUtil.Dock = DockStyle.Left;
            pnlDbUtil.Width = pnlWelcome.Left;
            #endregion

            float fontSize = ((float)(screenFontSize * pnlWelcome.Height * .08) / (96 / 12)) * (72 / 12);
            pnlDbUtil.BorderStyle = BorderStyle.None;
            pnlDbUtil.Paint += (s, e) =>
            {
                Control p = (Control)s;
                using (Pen pen = new Pen(Color.White, 1))
                {
                    e.Graphics.DrawLine(pen, p.Width - 1, 0, p.Width - 1, p.Height);
                }
            };
            lblDbUtil.Height = (int)(pnlWelcome.Height * .1);
            //Will be used primarily for table panel expansion later, but need for DbOptions
            fullHeight = (int)(pnlDbUtil.Height * .5);
            dimW = pnlDbUtil.Width / 3;
            dimH = (int)(fullHeight * .25);
            dimH2 = (int)(fullHeight * .5);


            ChangeLabel(ThemeColor, lblDbOverview, pnlDbUtil, new List<string> {
                "Database Overview",
                "Bold",
                (((float)(screenFontSize * pnlWelcome.Height * .05) / (96 / 12)) * (72 / 12)).ToString(),
                ".",
                "true",
                pnlDbUtil.Left.ToString(),
                (lblDbUtil.Bottom + (int)(lblDbUtil.Height * .3)).ToString(),
                ThemeColor.ToString(),
                "true",
                ((int)(lblDbUtil.Height * .8)).ToString()
            });
            ChangeLabel(ThemeColor, lblDbOvExpand, pnlDbUtil, new List<string> {
                "+",
                "Bold",
                (((float)(screenFontSize * pnlWelcome.Height * .05) / (96 / 12)) * (72 / 12)).ToString(),
                ".",
                "true",
                ".",
                lblDbOverview.Top.ToString(),
                ThemeColor.ToString(),
                "true",
                ((int)(lblDbUtil.Height * .8)).ToString()
            });
            lblDbOvExpand.Left = (pnlDbUtil.Left + pnlDbUtil.Width - lblDbOvExpand.Width);
            lblDbOvExpand.Height = lblDbOverview.Height;
            overviewHeight = lblDbOverview.Height;

            ChangeLabel(ThemeColor, lblDbOptions, pnlDbUtil, new List<string> {
                "Options",
                "Bold",
                fontSize.ToString(),
                ".",
                "true",
                ".",
                (lblDbUtil.Height * 3).ToString(),
                ThemeColor.ToString(),
                "true",
                ((int)(lblDbUtil.Height * .9)).ToString()
            });

            ChangeLabel(ThemeColor, lblPopulate, lblDbOptions, new List<string> {
                ".",
                "Bold",
                fontSize.ToString(),
                ".",
                "true",
                pnlDbUtil.Left.ToString(),
                lblDbOptions.Bottom.ToString(),
                ThemeColor.ToString(),
                "true",
                "."
            });
            ChangeLabel(ThemeColor, lblDownloadSeasonData, lblDbOptions, new List<string> {
                ".",
                "Bold",
                fontSize.ToString(),
                ".",
                "true",
                ".",
                lblDbOptions.Bottom.ToString(),
                ThemeColor.ToString(),
                "true",
                "."
            });

            ChangeLabel(ThemeColor, lblDbSelectSeason, lblDbOptions, new List<string> {
                "Season Select",
                "Bold",
                fontSize.ToString(),
                ".",
                "true",
                pnlDbUtil.Left.ToString(),
                lblPopulate.Bottom.ToString(),
                ThemeColor.ToString(),
                "true",
                "."
            });
            lblDbSelectSeason.Left = (int)(listSeasons.Width * .03);
            ChangeLabel(ThemeColor, lblDlSelectSeason, lblDbOptions, new List<string> {
                "Season Select",
                "Bold",
                fontSize.ToString(),
                ".",
                "true",
                ".",
                lblDownloadSeasonData.Bottom.ToString(),
                ThemeColor.ToString(),
                "true",
                "."
            });


            listSeasons.SelectionMode = SelectionMode.MultiExtended; 
            listSeasons.KeyDown += (sender, e) => SelectAllListItems(sender, e, listSeasons);
            listSeasons.DrawMode = DrawMode.OwnerDrawFixed;

            listSeasons.DrawItem += (sender, e) =>
            {
                if (e.Index < 0) return;

                string season = listSeasons.Items[e.Index].ToString();
                bool isDownloaded = downloadedSeasons.Contains(season);

                //Draw background - gray out if files don't exist
                if (!isDownloaded)
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), e.Bounds);
                }
                else if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), e.Bounds);
                }

                //Draw text
                Color textThemeColor = SubThemeColor;
                if (settings.BackgroundImage == "Court Dark")
                {
                    textThemeColor = ThemeColor;
                }
                Color textColor = !isDownloaded ? ErrorColor :
                ((e.State & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.HighlightText : textThemeColor);

                e.Graphics.DrawString(season, listSeasons.Font, new SolidBrush(textColor), e.Bounds);
                e.DrawFocusRectangle();
            };

            //Prevent selection of seasons without files
            listSeasons.SelectedIndexChanged += (s, e) =>
            {
                for (int i = listSeasons.SelectedIndices.Count - 1; i >= 0; i--)
                {
                    string season = listSeasons.Items[listSeasons.SelectedIndices[i]].ToString();
                    if (!downloadedSeasons.Contains(season))
                    {
                        listSeasons.SetSelected(listSeasons.SelectedIndices[i], false);
                    }
                }
            };

            listDownloadSeasonData.SelectionMode = SelectionMode.MultiExtended;
            listDownloadSeasonData.KeyDown += (sender, e) => SelectAllListItems(sender, e, listDownloadSeasonData);
            listDownloadSeasonData.DrawMode = DrawMode.OwnerDrawFixed;
            listDownloadSeasonData.DrawItem += (sender, e) =>
            {
                if (e.Index < 0) return;
                string season = listDownloadSeasonData.Items[e.Index].ToString();

                //Draw background
                if ((e.State & DrawItemState.Selected) == DrawItemState.Selected)
                {
                    e.Graphics.FillRectangle(new SolidBrush(SystemColors.Highlight), e.Bounds);
                }
                else
                {
                    e.Graphics.FillRectangle(new SolidBrush(Color.Black), e.Bounds);
                }

                //Draw text
                Color textThemeColor = SubThemeColor;
                if (settings.BackgroundImage == "Court Dark")
                {
                    textThemeColor = ThemeColor;
                }
                Color textColor = (e.State & DrawItemState.Selected) == DrawItemState.Selected ? SystemColors.HighlightText : textThemeColor;

                e.Graphics.DrawString(season, listDownloadSeasonData.Font, new SolidBrush(textColor), e.Bounds);
                e.DrawFocusRectangle();
            };


            btnPopulate.Text = "Populate Db";
            btnPopulate.Font = SetFontSize("Segoe UI", (float)(fontSize / 2.7), FontStyle.Bold, (int)(listSeasons.Width * .8), btnPopulate); //6.5
            btnDownloadSeasonData.Height = btnPopulate.Height;
            btnPopulate.Width = (int)(listSeasons.Width * .9);
            btnPopulate.AutoSize = true;
            btnDownloadSeasonData.Text = "Download";
            btnDownloadSeasonData.Font = SetFontSize("Segoe UI", (float)(fontSize / 2.7), FontStyle.Bold, (int)(listSeasons.Width * .8), btnPopulate); //6.5
            btnDownloadSeasonData.Width = (int)(listSeasons.Width * .9);



        }

        public void AddControlsAfterConnection()
        {
            float fontSize = ((float)(screenFontSize * pnlWelcome.Height * .08) / (96 / 12)) * (72 / 12);

            pnlDbUtil.Parent = bgCourt;


            //Panel Formatting
            pnlLoad.Top = pnlWelcome.Bottom;
            pnlLoad.Left = pnlWelcome.Left;
            pnlLoad.Width = pnlWelcome.Width;
            pnlLoad.Height = pnlWelcome.Top;
            pnlLoad.BackColor = Color.Transparent;
            picLoad.Parent = pnlLoad;
            pnlLoad.Parent = bgCourt;
            picLoad.SizeMode = PictureBoxSizeMode.Zoom;
            picLoad.Width = pnlLoad.Height;
            picLoad.Height = pnlLoad.Height;
            picLoad.Left = (pnlLoad.ClientSize.Width - picLoad.Width) / 2;
            picLoad.BackColor = Color.Transparent;



            lblDbUtil.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, (int)(pnlDbUtil.Width * .7), lblDbUtil);
            lblDbUtil.AutoSize = true;
            lblDbUtil.Left = (pnlDbUtil.Width - lblDbUtil.Width) / 2;



            //lblSeasonStatusLoadInfo  
            lblSeasonStatusLoad.Left = 0;
            fontSize = ((float)(screenFontSize * pnlLoad.Height * .08) / (96 / 12)) * (72 / 12);
            lblSeasonStatusLoad.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, (int)(pnlLoad.Width * .7), lblSeasonStatusLoad);
            lblSeasonStatusLoad.AutoSize = true;
            lblSeasonStatusLoadInfo.Left = lblSeasonStatusLoad.Right - (int)(pnlLoad.Width * .01);
            lblSeasonStatusLoadInfo.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, (int)(pnlLoad.Width * .7), lblSeasonStatusLoadInfo);
            lblSeasonStatusLoadInfo.AutoSize = true;



            lblCurrentGame.Left = 4;
            fontSize = ((float)(screenFontSize * pnlLoad.Height * .05) / (96 / 12)) * (72 / 12);
            lblCurrentGame.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, (int)(pnlLoad.Width * .7), lblCurrentGame);
            lblCurrentGame.AutoSize = true;
            lblCurrentGameCount.Left = lblCurrentGame.Right - (int)(pnlLoad.Width * .02);
            lblCurrentGameCount.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, (int)(pnlLoad.Width * .7), lblCurrentGameCount);
            lblCurrentGameCount.AutoSize = true;
            lblCurrentGame.Top = lblSeasonStatusLoad.Bottom;
            lblCurrentGameCount.Top = lblSeasonStatusLoad.Bottom;

            fontSize = ((float)(screenFontSize * pnlLoad.Height * .045) / (96 / 12)) * (72 / 12);
            gpm.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, (int)(pnlLoad.Width * .7), gpm);
            gpm.AutoSize = true;
            gpm.Top = lblCurrentGame.Bottom;
            gpm.Left = 4;
            fontSize = ((float)(screenFontSize * pnlLoad.Height * .05) / (96 / 12)) * (72 / 12);
            gpmValue.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, (int)(pnlLoad.Width * .7), gpmValue);
            gpmValue.AutoSize = true;
            gpmValue.Top = gpm.Bottom;
            gpmValue.Left = 4;
            gpmValue.ForeColor = ThemeColor;


            //Navbar
            pnlNav.Height = this.Height / 20;
            pnlNav.Dock = DockStyle.Top;
            pnlNav.Parent = bgCourt;
            pnlNav.Width = this.Width;
            pnlNav.BorderStyle = BorderStyle.None;
            pnlNav.Paint += (s, e) =>
            {
                Control p = (Control)s;
                using (Pen pen = new Pen(Color.Black, 3))
                {
                    e.Graphics.DrawLine(pen, 0, p.Height - 1, p.Width, p.Height - 1);
                }
            };

            //Welcome
            pnlWelcome.Parent = bgCourt; //Set Panel parent as the image
            int spacer = (int)(pnlWelcome.Height * .01);




            //Set Label Properties ***************************************************************************

            //To set font, i'll need the name, ideal size or pt, and its Style.
            //In addition, i also need the parent element and the child or the element we're working with
            lblStatus.Height = (int)(pnlWelcome.Height * .1);
            lblStatus.Font = SetFontSize("Segoe UI", ((float)(lblStatus.Height) / (96 / 12)) * (72 / 12), FontStyle.Bold, (int)(pnlWelcome.Width * .7), lblStatus);
            //Auto-size and center
            CenterElement(pnlWelcome, lblStatus);





            //Server label properties
            lblServer.Left = 5;
            lblServer.Top = lblStatus.Bottom;
            lblServer.Height = (int)(pnlWelcome.Height * .067);
            fontSize = ((float)((screenFontSize * pnlWelcome.Height * .05)) / (96 / 12)) * (72 / 12);
            lblServer.AutoSize = true;
            //lblServer.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, pnlWelcome, lblServer);

            lblServer.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
            lblServerName.Left = lblServer.Right - 10;
            lblServerName.Height = (int)(pnlWelcome.Height * .067);
            lblServerName.Top = lblServer.Top;
            lblServerName.Height = lblServer.Height;
            lblServerName.AutoSize = true;
            lblServerName.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);

            //Database label Properties
            lblDB.Left = 5;
            lblDB.Top = lblServer.Bottom;
            lblDB.Height = (int)(pnlWelcome.Height * .06);
            lblDB.AutoSize = true;
            fontSize = ((float)((screenFontSize * pnlWelcome.Height * .04)) / (96 / 12)) * (72 / 12);
            lblDB.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
            lblDbName.Left = lblDB.Right - 10;
            lblDbName.Top = lblServer.Bottom;
            lblDbName.AutoSize = true;
            lblDbName.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
            picDbStatus.Width = lblDB.Height;
            picDbStatus.Height = lblDB.Height;
            picDbStatus.SizeMode = PictureBoxSizeMode.Zoom;
            picDbStatus.Top = lblDB.Top;
            picDbStatus.Left = lblDbName.Right;

            lblDbStat.Left = 5;
            lblDbStat.Top = lblDB.Bottom;
            lblDbStat.Height = (int)(pnlWelcome.Height * .067);
            lblDbStat.AutoSize = true;
            lblDbStat.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, (int)(pnlWelcome.Width * .7), lblDbStat);





            fontSize = ((float)(screenFontSize * lblDB.Height) / (96 / 12)) * (72 / 12);
            btnEdit.Font = SetFontSize("Segoe UI", (float)(fontSize * .67), FontStyle.Bold, (int)(pnlWelcome.Width * .7), btnEdit); //12F
            CenterElement(pnlWelcome, btnEdit);
            btnEdit.Top = lblDbStat.Bottom + spacer; //subject to change
            btnEdit.TextAlign = ContentAlignment.BottomCenter;
            btnEdit.AutoSize = true;







            fontSize = ((float)(screenFontSize * lblServer.Height) / (96 / 12)) * (72 / 12) / 2;
            if (!isConnected)
            {
                lblCStatus.Font = SetFontSize("Segoe UI", ((float)(lblServer.Height * .9) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, (int)(pnlWelcome.Width * .7), lblCStatus);
            }
            else //If we're connected, use normal sized font
            {
                lblCStatus.Font = SetFontSize("Segoe UI", ((float)(lblServer.Height) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, (int)(pnlWelcome.Width * .7), lblCStatus);
            }
            //lblCStatus.Height = lblServer.Height / 2;
            picStatus.SizeMode = PictureBoxSizeMode.Zoom;
            int topY = lblDB.Bottom + 20; //Vertical position            
            //picStatus.Top = lblStatus.Top+ (int)(lblCStatus.Height / 10);
            lblCStatus.AutoSize = true;
            lblCStatus.Height = lblServer.Height / 2;
            picStatus.Width = lblCStatus.Height;
            picStatus.Height = lblCStatus.Height;
            int totalWidth = picStatus.Width + lblCStatus.Width; //Measure combined width  
            picStatus.Left = pnlWelcome.Width - totalWidth; //Position image on the left      
            lblCStatus.Left = picStatus.Right - 2; //Position label on the right
            pnlWelcome.Controls.SetChildIndex(picStatus, 1);

            //Build Database Button
            btnBuild.Text = "Build Database";
            btnBuild.Font = SetFontSize("Segoe UI", (float)(fontSize * 1.3), FontStyle.Bold, (int)(pnlWelcome.Width * .7), btnBuild);
            btnBuild.AutoSize = true;
            CenterElement(pnlWelcome, btnBuild);
            btnBuild.Top = btnEdit.Bottom + spacer; //subject to change
            if (!isBuildEnabled)
            {
                btnBuild.Enabled = false;
                ButtonChangeState(btnBuild, false);
            }
            else
            {
                btnBuild.Enabled = true;
                ButtonChangeState(btnBuild, true);
            }




            pnlDbOverview.Parent = pnlDbUtil;
            pnlDbOverview.Width = pnlDbUtil.Width;
            pnlDbOverview.Top = lblDbOverview.Top;
            pnlDbOverview.Height = (int)(lblDbOverview.Height * 1.3);
            pnlDbOverview.BorderStyle = BorderStyle.None;
            pnlDbOverview.Paint += (s, e) =>
            {
                Control p = (Control)s;
                using (Pen pen = new Pen(Color.White, 2))
                {
                    e.Graphics.DrawLine(pen, 0, p.Height - 1, p.Width, p.Height - 1);
                }
            };
            pnlDbOverview.AutoScroll = true;

            string database = config?.Database ?? "";
            ChangeLabel(ThemeColor, lblDbOvName, pnlDbUtil, new List<string> {
                database,
                "Bold",
                (((float)(screenFontSize * pnlWelcome.Height * .05) / (96 / 12)) * (72 / 12)).ToString(),
                ".",
                "true",
                lblDbOverview.Left.ToString(),
                lblDbOverview.Top.ToString(),
                ".",
                "true",
                ((int)(lblDbUtil.Height * .8)).ToString()
            });
            lblDbOvName.Font = SetFontSize("Segoe UI", (((float)(screenFontSize * pnlWelcome.Height * .05) / (96 / 12)) * (72 / 12)), FontStyle.Bold,
                (int)(screenFontSize * (lblDbOvExpand.Right - lblDbOverview.Right)), lblDbOvName);
            lblDbOvName.Left = lblDbOverview.Right;
            if (!dbConnection)
            {
                lblDbOvName.BackColor = Color.FromArgb(100, 0, 0, 0);
            }
            else
            {
                lblDbOvName.BackColor = Color.Transparent;
            }
            ChangeLabel(ThemeColor, lblDbOvExpand, pnlDbUtil, new List<string> {
            "+",
            "Bold",
            (((float)(screenFontSize * pnlWelcome.Height * .05) / (96 / 12)) * (72 / 12)).ToString(),
            ".",
            "true",
            ".",
            lblDbOverview.Top.ToString(),
            ThemeColor.ToString(),
            "true",
            ((int)(lblDbUtil.Height * .8)).ToString()
            });
            lblDbOvExpand.Left = (pnlDbUtil.Left + pnlDbUtil.Width - (lblDbOvExpand.Width + SystemInformation.VerticalScrollBarWidth));




            lblSettings.Text = "Settings";
            lblSettings.Top = btnBuild.Bottom + spacer; //subject to change
            lblSettings.Font = SetFontSize("Segoe UI", (float)(fontSize), FontStyle.Bold, (int)(pnlWelcome.Width * .7), btnBuild);
            lblSettings.AutoSize = true;
            lblSettings.Left = (pnlWelcome.ClientSize.Width - lblSettings.Width) / 2;
            picSettings.Width = lblSettings.Height;
            picSettings.Height = lblSettings.Height;
            picSettings.SizeMode = PictureBoxSizeMode.Zoom;
            picSettings.Image = Image.FromFile(Path.Combine(projectRoot, @"Content\Images", "Settings.png"));
            picSettings.Left = lblSettings.Right;
            picSettings.Top = lblSettings.Top + lblSettings.Height / 6;
            picSettings.BackColor = Color.FromArgb(0, 0, 0, 0);

            pnlSettings.Top = lblSettings.Bottom + spacer;
            pnlSettings.Width = pnlWelcome.Width;
            pnlSettings.Height = pnlWelcome.Height - pnlSettings.Top;
            pnlSettings.AutoScroll = true;





            lblBrowseConfig.Font = SetFontSize("Segoe UI", (float)(fontSize * .9), FontStyle.Bold, (int)(pnlWelcome.Width * .7), lblBrowseConfig);
            lblBrowseConfig.Top = 0;
            lblBrowseConfig.Left = 0;
            btnBrowseConfig.Font = SetFontSize("Segoe UI", (float)(fontSize * .8), FontStyle.Bold, (int)(pnlWelcome.Width * .7), btnBrowseConfig);
            btnBrowseConfig.Width = btnEdit.Width / 2;
            btnBrowseConfig.Top = 0;
            lblBrowseConfig.Top = btnBrowseConfig.Top + (btnBrowseConfig.Height / 5);
            lblBrowseConfig.AutoSize = true;
            btnBrowseConfig.AutoSize = true;
            btnBrowseConfig.Left = lblBrowseConfig.Right;
            lblBrowseConfig.ForeColor = ThemeColor;


            boxChangeConfig.DropDownStyle = ComboBoxStyle.DropDownList; //Makes it non-editable
            boxChangeConfig.Font = SetFontSize("Segoe UI", (float)(fontSize * .7), FontStyle.Regular, (int)(pnlWelcome.Width * .7), boxChangeConfig);
            boxChangeConfig.Top = btnBrowseConfig.Bottom + spacer;
            boxChangeConfig.Width = (int)(btnEdit.Width * .7);

            lblChangeConfig.Left = 0;
            lblChangeConfig.Top = boxChangeConfig.Top;
            lblChangeConfig.AutoSize = true;
            lblChangeConfig.Font = SetFontSize("Segoe UI", (float)(fontSize * .9), FontStyle.Bold, (int)(pnlWelcome.Width * .7), lblChangeConfig);
            lblChangeConfig.ForeColor = ThemeColor;
            boxChangeConfig.Left = lblChangeConfig.Right;

            boxConfigFiles.DropDownStyle = ComboBoxStyle.DropDownList; //Makes it non-editable
            boxConfigFiles.Font = SetFontSize("Segoe UI", (float)(fontSize * .7), FontStyle.Regular, (int)(pnlWelcome.Width * .7), boxConfigFiles);
            boxConfigFiles.Top = boxChangeConfig.Bottom + spacer;
            boxConfigFiles.Width = (int)(btnEdit.Width * .7);

            lblConfigFiles.Left = 0;
            lblConfigFiles.Top = boxConfigFiles.Top;
            lblConfigFiles.AutoSize = true;
            lblConfigFiles.Font = SetFontSize("Segoe UI", (float)(fontSize * .9), FontStyle.Bold, (int)(pnlWelcome.Width * .7), lblConfigFiles);
            lblConfigFiles.ForeColor = ThemeColor;
            boxConfigFiles.Left = lblConfigFiles.Right;

            boxBackground.DropDownStyle = ComboBoxStyle.DropDownList; //Makes it non-editable
            boxBackground.Font = SetFontSize("Segoe UI", (float)(fontSize * .7), FontStyle.Regular, (int)(pnlWelcome.Width * .7), boxBackground);
            boxBackground.Top = boxConfigFiles.Bottom + spacer;
            boxBackground.Width = (int)(btnEdit.Width * .4);

            lblBackground.Left = 0;
            lblBackground.Top = boxConfigFiles.Bottom + spacer;
            lblBackground.AutoSize = true;
            lblBackground.Font = SetFontSize("Segoe UI", (float)(fontSize * .9), FontStyle.Bold, (int)(pnlWelcome.Width * .7), lblBackground);
            lblBackground.ForeColor = ThemeColor;
            boxBackground.Left = lblBackground.Right;
            boxBackground.SelectedItem = settings.BackgroundImage;



            boxSoundOptions.DropDownStyle = ComboBoxStyle.DropDownList; //Makes it non-editable
            boxSoundOptions.Font = SetFontSize("Segoe UI", (float)(fontSize * .7), FontStyle.Regular, (int)(pnlWelcome.Width * .7), boxSoundOptions);
            boxSoundOptions.Top = boxBackground.Bottom + spacer;
            boxSoundOptions.Width = (int)(btnEdit.Width * .4);

            lblSound.Left = 0;
            lblSound.Top = boxBackground.Bottom + spacer;
            lblSound.AutoSize = true;
            lblSound.Font = SetFontSize("Segoe UI", (float)(fontSize * .9), FontStyle.Bold, (int)(pnlWelcome.Width * .7), lblSound);
            lblSound.ForeColor = ThemeColor;
            boxSoundOptions.Left = lblSound.Right;
            boxSoundOptions.SelectedItem = settings.Sound;

            lblDbOvName.FlatStyle = FlatStyle.Flat;


            lblDbOptions.AutoSize = true;
            lblDbOptions.Left = (pnlDbUtil.ClientSize.Width - lblDbOptions.Width) / 2;



            lblPopulate.Font = SetFontSize("Segoe UI", (float)(fontSize), FontStyle.Bold, (int)(lblDbUtil.Width / 1.5), lblPopulate);
            lblPopulate.AutoSize = true;
            lblDownloadSeasonData.Font = SetFontSize("Segoe UI", (float)(fontSize), FontStyle.Bold, (int)(lblDbUtil.Width / 1.5), lblDownloadSeasonData);
            lblDownloadSeasonData.AutoSize = true;
            lblRefresh.Font = SetFontSize("Segoe UI", (float)(fontSize), FontStyle.Bold, (int)(lblDbUtil.Width / 1.5), lblRefresh);
            lblRefresh.AutoSize = true;
            if (listSeasons.Items.Count > 0)
            {
                btnRefresh.Text = "Refresh " + listSeasons.Items[0].ToString() + " data";
            }
            else
            {
                btnRefresh.Enabled = false;
                ButtonChangeState(btnRefresh, false);
            }
            btnRefresh.Font = SetFontSize("Segoe UI", (float)(fontSize), FontStyle.Bold, (int)(lblRefresh.Width * .8), btnRefresh);
            btnDownloadSeasonData.Text = "Populate Db";
            btnDownloadSeasonData.Font = SetFontSize("Segoe UI", (float)(fontSize), FontStyle.Bold, (int)(listSeasons.Width * .8), btnDownloadSeasonData);
            btnDownloadSeasonData.Text = "Download";
            ArrangeOverviewControls();
        }

        public void ArrangeOverviewControls()
        {
            int spacer = (int)(pnlDbUtil.Height * .01);
            lblDbOptions.Top = pnlDbOverview.Bottom;
            lblPopulate.Top = lblDbOptions.Bottom;
            lblDownloadSeasonData.Top = lblDbOptions.Bottom;
            lblDbSelectSeason.Top = lblPopulate.Bottom;
            lblDlSelectSeason.Top = lblPopulate.Bottom;
            listSeasons.Top = lblDbSelectSeason.Bottom;
            listDownloadSeasonData.Top = lblDbSelectSeason.Bottom;

            btnPopulate.Top = listSeasons.Bottom;
            btnDownloadSeasonData.Top = listDownloadSeasonData.Bottom;
            lblRefresh.Top = btnPopulate.Bottom + (int)(btnPopulate.Height / 2);
            btnRefresh.Top = lblRefresh.Bottom;
            btnPopulate.Left = (int)(listSeasons.Width * .05);

            btnRefresh.Left = (int)((lblRefresh.Width - btnRefresh.Width) * .5);
            listSeasons.Left = btnRefresh.Left;
            lblDbSelectSeason.Left = (int)((listSeasons.Width - listSeasons.Width) * .5) + listSeasons.Left;
            btnPopulate.Left = btnRefresh.Left;

            lblDownloadSeasonData.Left = lblDbOptions.Left + (int)(lblDbOptions.Width / 2);
            listDownloadSeasonData.Left = lblDownloadSeasonData.Left + listSeasons.Left;
            lblDlSelectSeason.Left = lblDownloadSeasonData.Left + lblDbSelectSeason.Left;
            btnDownloadSeasonData.Left = listDownloadSeasonData.Left;
        }
        public void InitializeElements()
        {
            //Children elements should go above the parents, background image should be last added. AddPanelElement(pnlDbOverview, lblGameUtil);
            AddPanelElement(pnlDbOverview, lblTeamUtil);
            AddPanelElement(pnlDbOverview, lblArenaUtil);
            AddPanelElement(pnlDbOverview, lblPlayerUtil);
            AddPanelElement(pnlDbOverview, lblOfficialUtil);
            AddPanelElement(pnlDbOverview, lblTbUtil);
            AddPanelElement(pnlDbOverview, lblPbUtil);
            AddPanelElement(pnlDbOverview, lblPbpUtil);
            AddPanelElement(pnlDbOverview, lblEmpty);
            AddPanelElement(pnlLoad, gpmValue);
            AddPanelElement(pnlLoad, gpm);
            AddPanelElement(pnlLoad, lblWorkingOn);
            AddPanelElement(pnlLoad, lblCurrentGameCount);
            AddPanelElement(pnlLoad, lblCurrentGame);
            AddPanelElement(pnlLoad, lblSeasonStatusLoadInfo);
            AddPanelElement(pnlLoad, lblSeasonStatusLoad);
            AddPanelElement(pnlLoad, picLoad);
            AddPanelElement(pnlDbUtil, btnDownloadSeasonData);
            AddPanelElement(pnlDbUtil, lblDownloadSeasonData);
            AddPanelElement(pnlDbUtil, lblDlSelectSeason);
            AddPanelElement(pnlDbUtil, listDownloadSeasonData);
            AddPanelElement(pnlDbUtil, btnRefresh);
            AddPanelElement(pnlDbUtil, lblRefresh);
            AddPanelElement(pnlDbUtil, btnPopulate);
            AddPanelElement(pnlDbUtil, listSeasons);
            AddPanelElement(pnlDbUtil, lblDbSelectSeason);
            AddPanelElement(pnlDbUtil, lblPopulate);
            AddPanelElement(pnlDbUtil, lblDbOptions);
            AddPanelElement(pnlDbUtil, lblDbOvExpand);
            AddPanelElement(pnlDbUtil, lblDbOvName);
            AddPanelElement(pnlDbUtil, lblDbOverview);
            AddPanelElement(pnlDbUtil, pnlDbOverview);
            AddPanelElement(pnlDbUtil, lblDbUtil);
            AddPanelElement(pnlSettings, boxSoundOptions);
            AddPanelElement(pnlSettings, lblSound);
            AddPanelElement(pnlSettings, boxBackground);
            AddPanelElement(pnlSettings, lblBackground);
            AddPanelElement(pnlSettings, boxConfigFiles);
            AddPanelElement(pnlSettings, lblConfigFiles);
            AddPanelElement(pnlSettings, boxChangeConfig);
            AddPanelElement(pnlSettings, lblChangeConfig);
            AddPanelElement(pnlSettings, btnBrowseConfig);
            AddPanelElement(pnlSettings, lblBrowseConfig);
            AddPanelElement(pnlWelcome, pnlSettings);
            AddPanelElement(pnlWelcome, picSettings);
            AddPanelElement(pnlWelcome, lblSettings);
            AddPanelElement(pnlWelcome, lblDbStat);
            AddPanelElement(pnlWelcome, btnBuild);
            AddPanelElement(pnlWelcome, lblCStatus);
            AddPanelElement(pnlWelcome, picStatus);
            AddPanelElement(pnlWelcome, picDbStatus);
            AddPanelElement(pnlWelcome, lblDbName);
            AddPanelElement(pnlWelcome, lblDB);
            AddPanelElement(pnlWelcome, lblServerName);
            AddPanelElement(pnlWelcome, lblServer);
            AddPanelElement(pnlWelcome, btnEdit);
            AddPanelElement(pnlWelcome, lblStatus);
            AddMainElement(this, pnlLoad);   //Adding Welcome panel
            AddMainElement(this, pnlWelcome);   //Adding Welcome panel
            AddMainElement(this, pnlScoreboard);   //Adding Database Utilities panel
            AddMainElement(this, pnlNav);   //Adding Database Utilities panel
            AddMainElement(this, pnlDbUtil);   //Adding Database Utilities panel
            AddMainElement(this, bgCourt); //Ading background image
        }

        #endregion


        #region Panel, Element, Alignment and Font Formatting
        //Add an element to a panel
        public void AddPanelElement(Panel panel, Control control)
        {
            this.Controls.Add(control);
            control.Parent = panel;
        }
        //Add element to Main program
        public void AddMainElement(Main main, Control control)
        {
            this.Controls.Add(control);
            control.Parent = main;
            control.BackColor = Color.Transparent;
        }
        //Center element within panel
        public void CenterElement(Panel panel, Control control)
        {
            control.AutoSize = true;
            control.Left = (panel.ClientSize.Width - control.Width) / 2;
        }

        //Set Dynamic Font size
        public Font SetFontSize(string font, Single size, FontStyle style, int target, Control child)
        {
            Font newFont = new Font(font, size, style);
            int targetWidth = target;
            //if (parent.Text == "Options")
            //{
            //    targetWidth = (int)(parent.Width * 0.8);
            //}
            //else
            //{
            //    targetWidth = (int)(parent.Width * 0.7);
            //}
            float bestSize = GetBestFitFontSize(child, child.Text, newFont, targetWidth);
            return new Font(newFont.FontFamily, bestSize, newFont.Style);
        }
        private float GetBestFitFontSize(Control control, string text, Font baseFont, int targetWidth)
        {
            using (Graphics g = control.CreateGraphics())
            {
                float fontSize = baseFont.Size;

                while (fontSize > 1)
                {
                    Font newFont = new Font(baseFont.FontFamily, fontSize, baseFont.Style);
                    SizeF size = g.MeasureString(text, newFont);

                    if (size.Width <= targetWidth)
                        return fontSize;

                    fontSize -= 0.5f;
                }

                return baseFont.Size; // fallback
            }
        }
        #endregion


        #region Utility Functions
        //Formats Time elapsed string for season load
        public string CheckTime(Dictionary<string, (int, string)> timeUnits)
        {
            string returnString = "";
            foreach (KeyValuePair<string, (int, string)> pair in timeUnits)
            {
                if (pair.Value.Item1 != 0 && pair.Value.Item1 < 10 && pair.Key != "ms")              //If value is Hour, Min or Sec and single digit
                {
                    returnString += "0" + pair.Value.Item1 + pair.Value.Item2;
                }
                else if (pair.Value.Item1 != 0 && pair.Value.Item1 < 100 && pair.Key == "ms")        //If value is ms and double digit
                {
                    returnString += "0" + pair.Value.Item1;
                }
                else if (pair.Value.Item1 != 0 && pair.Value.Item1 >= 100 && pair.Key == "ms")       //If value ms and triple digits (normal)
                {
                    returnString += pair.Value.Item1;
                }
                else if (pair.Value.Item1 != 0 && pair.Value.Item1 >= 10 && pair.Key != "ms")        //If value Hour, Min or sec and double digit (normal)
                {
                    returnString += pair.Value.Item1 + pair.Value.Item2;
                }
                else if (pair.Value.Item1 == 0 && pair.Key != "ms")
                {
                    returnString += "00" + pair.Value.Item2;
                }
                else if (pair.Value.Item1 == 0 && pair.Key == "ms")
                {
                    returnString += "000";
                }

            }
            return returnString;
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)//Enables Esc key to close program
        {
            if (keyData == Keys.Escape)
            {
                this.Close(); // or Application.Exit();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        public void ClearImage(PictureBox pic)
        {
            if (pic.Image != null)
            {
                pic.Image.Dispose();
                pic.Image = null;
            }
        }
        private void SelectAllListItems(object sender, KeyEventArgs e, ListBox list)//Allows us to select all seasons to populate
        {
            // Check if Ctrl+A was pressed
            if (e.Control && e.KeyCode == Keys.A)
            {
                // Select all items
                for (int i = 0; i < list.Items.Count; i++)
                {
                    list.SetSelected(i, true);
                }
                
                // Mark the event as handled to prevent further processing
                e.Handled = true;
            }            
        }

        public HashSet<(int GameID, string builder, double mb, long len)> gameBytes = new HashSet<(int, string builder, double, long)>();
        
        public void ChangeLabel(Color color, Label label, Control parent, List<string> structions)
        {
            float fontSize = 0;
            FontStyle style = FontStyle.Regular;
            /*Text*/
            if (structions[0] != ".")
            {
                label.Text = structions[0];
            }
            else
            {
                label.Text = label.Text;
            }
            /*FontStyle*/
            if (structions[1] == "Bold")
            {
                style = FontStyle.Bold;
            }
            else if (structions[1] == "Italic")
            {
                style = FontStyle.Italic;
            }
            else if (structions[1] == "Strikeout")
            {
                style = FontStyle.Strikeout;
            }
            else if (structions[1] == "Underline")
            {
                style = FontStyle.Underline;
            }
            /*FontSize*/
            if (structions[2] != ".")
            {
                fontSize = float.Parse(structions[2]);
                if (label.Text == "Season Select")
                {
                    label.Font = SetFontSize("Segoe UI", fontSize, style, (int)(parent.Width * 1.1), label);
                }
                else
                {
                    label.Font = SetFontSize("Segoe UI", fontSize, style, (int)(parent.Width * .8), label);
                }
            }

            /*Width*/
            if (structions[3] != ".")
            {
                label.Width = Int32.Parse(structions[3]);
            }
            else
            {
                label.Width = label.Width;
            }

            /*Autosize*/
            if (structions[4] == "true")
            {
                label.AutoSize = true;
            }
            else if (structions[4] == "false")
            {
                label.AutoSize = false;
            }
            /*Left*/
            if (structions[5] != ".")
            {
                label.Left = Int32.Parse(structions[5]);
            }
            /*Top*/
            if (structions[6] != ".")
            {
                label.Top = Int32.Parse(structions[6]);
            }


            /*Color*/
            if (structions[7] != ".")
            {
                label.ForeColor = color;

            }
            /*Visiblity*/
            if (structions[8] == "true")
            {
                label.Visible = true;
            }
            else if (structions[8] == "false")
            {
                label.Visible = false;
            }
            /*Height*/
            if (structions[9] != ".")
            {
                label.Height = Int32.Parse(structions[9]);
            }
        }
        public void ImageDriver()
        {
            if (reverse)
            {
                imageIteration--;
            }
            else
            {
                imageIteration++;
            }
            if (imageIteration == 22)
            {
                reverse = true;
            }
            if (imageIteration == 1)
            {
                reverse = false;
            }
            iterator++;
        }
        public void PlayCompletionSound(string sender)
        {
            try
            {
                string soundPath = Path.Combine(projectRoot, "Content", "Sounds", "Completed " + sender + ".wav");
                if (File.Exists(soundPath))
                {
                    using (SoundPlayer player = new SoundPlayer(soundPath))
                    {
                        player.Play(); //asynchronous
                    }
                }
                else
                {
                    Console.WriteLine($"Sound file not found: {soundPath}");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error playing sound: {ex.Message}");
            }
        }

        private void UpdateLoadingImage(int imageNumber)
        {
            //Release previous image
            if (picLoad.Image != null)
            {
                picLoad.Image.Dispose();
                picLoad.Image = null;
            }

            //Load new image directly (no caching)
            string imagePath = Path.Combine(projectRoot, @"Content\Images\Loading", $"kawhi{imageNumber}.png");
            if (File.Exists(imagePath))
            {
                try
                {
                    using (var img = Image.FromFile(imagePath))
                    {
                        picLoad.Image = new Bitmap(img);
                    }
                }
                catch
                {
                    //Handle loading failure gracefully
                }
            }
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


        #region Db Overview
        public bool dbOverviewOpened = false;
        private Dictionary<int, Label> yearLabels = new Dictionary<int, Label>();
        private List<Label> tableHeaders = new List<Label>();
        private Label lblUnpopulated;
        private List<Panel> gridLines = new List<Panel>();
        public int popCount = 0;
        public int lineHeight = 0;
        public int lineWidth = 0;
        public bool seasonDataWarning = false;
        public bool seasonDataError = false;
        public bool seasonDataSuccess = false;
        private Dictionary<string, Label> dataLabels = new Dictionary<string, Label>();
        public void DbOverviewClick(Control control, Label growShrink, Control parent)
        {
            control.Click += async (s, e) =>
            {
                if (control.Focused || parent.Focused || dbOverviewOpened)
                {
                    this.ActiveControl = null;
                    growShrink.Text = "+";
                    dbOverviewOpened = false;
                    DbOverviewVisibility(false, "Click Close");
                }
                else
                {
                    if (dbOverviewFirstOpen)
                    {
                        lblDbOptions.Invoke((MethodInvoker)(() =>
                        {
                            lblDbOptions.Text = "Loading Season info";
                            CenterElement(pnlDbUtil, lblDbOptions);
                        }));
                        await Task.Run(() => GetSeasonInfo());
                        lblDbOptions.Invoke((MethodInvoker)(() =>
                        {
                            lblDbOptions.Text = "Options";
                            CenterElement(pnlDbUtil, lblDbOptions);
                        }));
                        dbOverviewFirstOpen = false;
                    }
                    parent.Focus();
                    growShrink.Text = "-";
                    dbOverviewOpened = true;
                    DbOverviewVisibility(true, "Click Open");
                }

            };
        }
        public void DbOverviewVisibility(bool vis, string sender)
        {
            pnlDbOverview.MaximumSize = new Size(pnlWelcome.Left, (int)(pnlDbUtil.Height / 2));
            ClearOverview();
            pnlDbOverview.Refresh();
            if (!vis)
            {
                dbOverviewOpened = false;
                //hide everything and clean up
                pnlDbOverview.BackColor = Color.Transparent;
                lblDbOverview.BackColor = Color.Transparent;
                lblDbOvExpand.BackColor = Color.Transparent;
                this.ActiveControl = null;
                pnlDbOverview.Height = (int)(lblDbOverview.Height * 1.3);
                lblDbOptions.Top = pnlDbOverview.Bottom;
                lblPopulate.Top = lblDbOptions.Bottom;
                lblDbSelectSeason.Top = lblPopulate.Bottom;
                listSeasons.Top = lblDbSelectSeason.Bottom;
                btnPopulate.Top = listSeasons.Bottom;
                lblRefresh.Top = btnPopulate.Bottom + (int)(btnPopulate.Height / 2);
                btnRefresh.Top = lblRefresh.Bottom;

                lblDownloadSeasonData.Top = lblDbOptions.Bottom;
                lblDlSelectSeason.Top = lblPopulate.Bottom;
                listDownloadSeasonData.Top = lblDbSelectSeason.Bottom;
                btnDownloadSeasonData.Top = listDownloadSeasonData.Bottom;
                lblDownloadSeasonData.Left = lblDbOptions.Left + (int)(lblDbOptions.Width / 2);
                listDownloadSeasonData.Left = lblDownloadSeasonData.Left + listSeasons.Left;
                lblDlSelectSeason.Left = lblDownloadSeasonData.Left + lblDbSelectSeason.Left;
                btnDownloadSeasonData.Left = listDownloadSeasonData.Left;
                if (dbConnection)
                {
                    lblDbOvName.ForeColor = SuccessColor;
                    lblDbOvName.BackColor = Color.Transparent;
                }
                else
                {
                    lblDbOvName.ForeColor = WarningColor;
                    if (!vis)
                    {
                        lblDbOvName.BackColor = Color.FromArgb(100, 0, 0, 0);
                    }
                    else
                    {
                        lblDbOvName.BackColor = SubThemeColor;
                    }
                }
                return;
            }
            else
            {
                dbOverviewOpened = true;
                //pnlDbOverview.Height = (int)(pnlDbUtil.Height / 2);
                lblDbOptions.Top = pnlDbOverview.Bottom;
                lblPopulate.Top = lblDbOptions.Bottom;
                lblDbSelectSeason.Top = lblPopulate.Bottom;
                listSeasons.Top = lblDbSelectSeason.Bottom;
                btnPopulate.Top = listSeasons.Bottom;
                lblRefresh.Top = btnPopulate.Bottom + (int)(btnPopulate.Height / 2);
                btnRefresh.Top = lblRefresh.Bottom;

                lblDownloadSeasonData.Top = lblDbOptions.Bottom;
                lblDlSelectSeason.Top = lblPopulate.Bottom;
                listDownloadSeasonData.Top = lblDbSelectSeason.Bottom;
                btnDownloadSeasonData.Top = listDownloadSeasonData.Bottom;
                lblDownloadSeasonData.Left = lblDbOptions.Left + (int)(lblDbOptions.Width / 2);
                listDownloadSeasonData.Left = lblDownloadSeasonData.Left + listSeasons.Left;
                lblDlSelectSeason.Left = lblDownloadSeasonData.Left + lblDbSelectSeason.Left;
                btnDownloadSeasonData.Left = listDownloadSeasonData.Left;
            }

            //show and build overview
            pnlDbOverview.BackColor = SubThemeColor;
            lblDbOverview.BackColor = SubThemeColor;
            lblDbOvExpand.BackColor = SubThemeColor;
            lblDbOvName.BackColor = SubThemeColor;
            if (dbConnection)
            {
                lblDbOvName.ForeColor = SuccessColor;
            }
            else
            {
                lblDbOvName.ForeColor = WarningColor;
            }
            lblDbOverview.ForeColor = ThemeColor;
            lblDbOvExpand.ForeColor = ThemeColor;

            if (sender == "Change")
            {
                ClearGridLines();
            }

            BuildOverview();
            int test = lblEmpty.Bottom;
            if (vis && lblUnpopulated.Bottom + pnlDbOverview.Top <= pnlDbUtil.Height / 2)
            {
                pnlDbOverview.Height = lblUnpopulated.Bottom + pnlDbOverview.Top;
                lblDbOptions.Top = pnlDbOverview.Bottom;
                lblPopulate.Top = lblDbOptions.Bottom;
                lblDbSelectSeason.Top = lblPopulate.Bottom;
                listSeasons.Top = lblDbSelectSeason.Bottom;
                btnPopulate.Top = listSeasons.Bottom;
                lblRefresh.Top = btnPopulate.Bottom + (int)(btnPopulate.Height / 2);
                btnRefresh.Top = lblRefresh.Bottom;

                lblDownloadSeasonData.Top = lblDbOptions.Bottom;
                lblDlSelectSeason.Top = lblPopulate.Bottom;
                listDownloadSeasonData.Top = lblDbSelectSeason.Bottom;
                btnDownloadSeasonData.Top = listDownloadSeasonData.Bottom;
                lblDownloadSeasonData.Left = lblDbOptions.Left + (int)(lblDbOptions.Width / 2);
                listDownloadSeasonData.Left = lblDownloadSeasonData.Left + listSeasons.Left;
                lblDlSelectSeason.Left = lblDownloadSeasonData.Left + lblDbSelectSeason.Left;
                btnDownloadSeasonData.Left = listDownloadSeasonData.Left;
            }
            else if (vis)
            {
                pnlDbOverview.Height = (int)(pnlDbUtil.Height / 2);
                lblDbOptions.Top = pnlDbOverview.Bottom;
                lblPopulate.Top = lblDbOptions.Bottom;
                lblDbSelectSeason.Top = lblPopulate.Bottom;
                listSeasons.Top = lblDbSelectSeason.Bottom;
                btnPopulate.Top = listSeasons.Bottom;
                lblRefresh.Top = btnPopulate.Bottom + (int)(btnPopulate.Height / 2);
                btnRefresh.Top = lblRefresh.Bottom;

                lblDownloadSeasonData.Top = lblDbOptions.Bottom;
                lblDlSelectSeason.Top = lblPopulate.Bottom;
                listDownloadSeasonData.Top = lblDbSelectSeason.Bottom;
                btnDownloadSeasonData.Top = listDownloadSeasonData.Bottom;
                lblDownloadSeasonData.Left = lblDbOptions.Left + (int)(lblDbOptions.Width / 2);
                listDownloadSeasonData.Left = lblDownloadSeasonData.Left + listSeasons.Left;
                lblDlSelectSeason.Left = lblDownloadSeasonData.Left + lblDbSelectSeason.Left;
                btnDownloadSeasonData.Left = listDownloadSeasonData.Left;
            }
        }
        private void BuildOverview()
        {
            popCount = 0;
            float fontSize = ((float)(lblDbOverview.Height * .6) / (96 / 12)) * (72 / 12);
            int leftTable = pnlDbUtil.Width / 7;
            int topTable = lblDbOverview.Height;
            List<int> columnPositions = new List<int>();
            List<int> rowPositions = new List<int>();

            //create table headers if needed
            if (tableHeaders.Count == 0)
            {
                string[] tables = { "Game", "Team", "Arena", "Player", "Official", "TeamBox", "PlayerBox", "PlayByPlay", "StartingLineups", "TeamBoxLineups" };
                foreach (string table in tables)
                {
                    Label header = new Label
                    {
                        Text = table,
                        Tag = "Table",
                        ForeColor = ThemeColor,
                        BackColor = SubThemeColor,
                        Left = leftTable,
                        Top = topTable,
                        Visible = true
                    };
                    header.Font = SetFontSize("Segoe UI", (float)(fontSize * 1.2), FontStyle.Bold, (int)(pnlDbUtil.Width * .7), header);
                    header.AutoSize = true;
                    tableHeaders.Add(header);
                    pnlDbOverview.Controls.Add(header);
                    columnPositions.Add(leftTable + header.Width);
                    leftTable += header.Width;
                    lineHeight += header.Height;
                    lineWidth = leftTable;
                }
            }
            else
            {
                //reposition existing headers
                foreach (Label header in tableHeaders)
                {
                    header.Font = SetFontSize("Segoe UI", fontSize * 1.2f, FontStyle.Bold, (int)(pnlDbUtil.Width * .7), header);
                    header.Left = leftTable;
                    header.Top = topTable;
                    header.Visible = true;
                    columnPositions.Add(leftTable + header.Width);
                    leftTable += header.Width;
                }
                lineWidth = leftTable;
            }

            //build year rows
            int topYear = tableHeaders[0].Bottom + (int)(tableHeaders[0].Height * .3);
            List<int> populatedYears = new List<int>();
            List<int> unpopulatedYears = new List<int>();
            int heightMod = 0;

            //check which years have data
            for (int year = 2024; year >= 1996; year--)
            {
                if (seasonInfo.Any(s => s.SeasonID == year && (s.Item2.TeamBox > 0 || s.Item2.PlayByPlay > 0 || s.Item2.PlayerBox > 0)))
                {
                    populatedYears.Add(year);
                    popCount++;
                }
                else
                {
                    lineHeight = 0;
                    unpopulatedYears.Add(year);
                }
            }
            //add first column position for years
            if (populatedYears.Count > 0)
            {
                Label firstYear = GetOrCreateYearLabel(populatedYears[0], fontSize);
                firstYear.Top = topYear;
                columnPositions.Insert(0, ((pnlDbUtil.Width / 7) + firstYear.Width) / 2);
            }

            //create/show populated year labels
            int singleLineHeight = 0;
            foreach (int year in populatedYears)
            {
                Label yearLabel = GetOrCreateYearLabel(year, fontSize);
                yearLabel.AutoSize = true;
                yearLabel.Top = topYear;
                yearLabel.Visible = true;
                if (rowPositions.Count == 0)
                {
                    AddDataLabelsForYear(year, topYear, columnPositions, tableHeaders[0].Bottom, fontSize);
                }
                else
                {
                    AddDataLabelsForYear(year, topYear, columnPositions, rowPositions[rowPositions.Count - 1], fontSize);
                }
                if (seasonDataWarning)
                {
                    yearLabel.ForeColor = WarningColor;
                }
                if (seasonDataError)
                {
                    yearLabel.ForeColor = ErrorColor;
                }
                else if (seasonDataSuccess)
                {
                    yearLabel.ForeColor = SuccessColor;
                }
                heightMod = (int)(yearLabel.Height * 2);
                int originalHeight = yearLabel.Height;
                int originalWidth = yearLabel.Width;
                yearLabel.AutoSize = false;
                yearLabel.Width = originalWidth;
                yearLabel.Height = heightMod;
                yearLabel.TextAlign = ContentAlignment.BottomCenter;
                rowPositions.Add(topYear + heightMod);
                topYear += heightMod + (int)(heightMod * .1);
                singleLineHeight = heightMod + (int)(heightMod * .1);
            }
            lineHeight = topYear;

            //handle unpopulated years
            if (unpopulatedYears.Count > 0)
            {
                string unpopText = "Unpopulated:\n";
                for (int i = 0; i < unpopulatedYears.Count; i++)
                {
                    unpopText += unpopulatedYears[i];
                    if (i < unpopulatedYears.Count - 1) unpopText += ", ";
                    if ((i + 1) % 11 == 0) unpopText += "\n";
                }

                if (lblUnpopulated == null)
                {
                    lblUnpopulated = new Label
                    {
                        Tag = "Year",
                        Name = "Unpopulated",
                        ForeColor = ThemeColor,
                        BackColor = SubThemeColor,
                        Left = 0
                    };
                    lblUnpopulated.Font = SetFontSize("Segoe UI", fontSize * 1.2f, FontStyle.Bold, (int)(pnlDbUtil.Width * .7), lblUnpopulated);
                    lblUnpopulated.AutoSize = true;
                    pnlDbOverview.Controls.Add(lblUnpopulated);
                }
                lblUnpopulated.Text = unpopText;
                lblUnpopulated.Top = topYear;
                lblUnpopulated.Visible = true;
                lineHeight = lblUnpopulated.Top - singleLineHeight;
            }
            else
            {
                lblUnpopulated = null;
                lblUnpopulated = new Label
                {
                    Tag = "Year",
                    Name = "Unpopulated",
                    ForeColor = ThemeColor,
                    BackColor = SubThemeColor,
                    Left = 0
                };
                lblUnpopulated.Font = SetFontSize("Segoe UI", fontSize * 1.2f, FontStyle.Bold, (int)(pnlDbUtil.Width * .7), lblUnpopulated);
                lblUnpopulated.AutoSize = true;
                pnlDbOverview.Controls.Add(lblUnpopulated);
                lblUnpopulated.Text = "All seasons populated!";
                lblUnpopulated.Top = topYear;
                lblUnpopulated.Visible = true;
                lineHeight = lblUnpopulated.Top - singleLineHeight;

            }

            //add final row position
            rowPositions.Add(tableHeaders[0].Bottom);

            //create grid lines
            CreateGridLines(columnPositions, rowPositions, topTable, heightMod);
        }
        private Label GetOrCreateYearLabel(int year, float fontSize)
        {
            if (!yearLabels.ContainsKey(year))
            {
                Label yearLabel = new Label
                {
                    Text = year.ToString(),
                    Tag = "Year",
                    ForeColor = ThemeColor,
                    BackColor = SubThemeColor,
                    Left = 0
                };
                yearLabel.Font = SetFontSize("Segoe UI", fontSize * 1.2f, FontStyle.Bold, (int)(pnlDbUtil.Width * .7), yearLabel);
                yearLabel.AutoSize = true;
                yearLabels[year] = yearLabel;
                pnlDbOverview.Controls.Add(yearLabel);
            }
            return yearLabels[year];
        }
        private void AddDataLabelsForYear(int year, int topYear, List<int> columnPositions, int rowPosition, float fontSize)
        {
            seasonDataWarning = false;
            seasonDataError = false;
            seasonDataSuccess = false;
            //find the season data for this year
            var seasonData = seasonInfo.FirstOrDefault(s => s.SeasonID == year);
            var seasonDataControl = seasonControl.FirstOrDefault(s => s.SeasonID == year);
            var seasonCurrentDataControl = seasonCurrentControl.FirstOrDefault(s => s.SeasonID == year);


            //data values in order: Game, Team, Arena, Player, Official, TeamBox, PlayerBox, PlayByPlay
            int[] dataValues = {
                seasonData.Item2.Game,
                seasonData.Item2.Team,
                seasonData.Item2.Arena,
                seasonData.Item2.Player,
                seasonData.Item2.Official,
                seasonData.Item2.PlayerBox,
                seasonData.Item2.TeamBox,
                seasonData.Item2.PlayByPlay,
                seasonData.Item2.StartingLineups,
                seasonData.Item2.TeamBoxLineups,
                seasonData.Item2.Games,
                seasonData.Item2.HistoricLoaded,
                seasonData.Item2.CurrentLoaded
            };
            int[] controlValues = {
                seasonDataControl.Item2.Game,
                seasonDataControl.Item2.Team,
                seasonDataControl.Item2.Arena,
                seasonDataControl.Item2.Player,
                seasonDataControl.Item2.Official,
                seasonDataControl.Item2.PlayerBox,
                seasonDataControl.Item2.TeamBox,
                seasonDataControl.Item2.PlayByPlay,
                seasonDataControl.Item2.StartingLineups,
                seasonDataControl.Item2.TeamBoxLineups,
                seasonDataControl.Item2.Games
            };
            int[] controlCurrentValues = {
                seasonCurrentDataControl.Item2.Game,
                seasonCurrentDataControl.Item2.Team,
                seasonCurrentDataControl.Item2.Arena,
                seasonCurrentDataControl.Item2.Player,
                seasonCurrentDataControl.Item2.Official,
                seasonCurrentDataControl.Item2.PlayerBox,
                seasonCurrentDataControl.Item2.TeamBox,
                seasonCurrentDataControl.Item2.PlayByPlay,
                seasonCurrentDataControl.Item2.StartingLineups,
                seasonCurrentDataControl.Item2.TeamBoxLineups,
                seasonCurrentDataControl.Item2.Games
            };
            string[] textValues =
            {
                "Games", "Teams", "Arenas", "Players", "Officials", "Games", "Games", "Games", "Games", "Games"
            };

            //create labels for each data point
            for (int i = 0; i < Math.Min(dataValues.Length, columnPositions.Count - 1); i++)
            {
                string labelKey = $"data_{year}_{i}";
                Label dataLabel = GetOrCreateDataLabel(labelKey, fontSize * 0.6f);
                dataLabel.Text = dataValues[i].ToString() + " " + textValues[i];
                dataLabel.AutoSize = true;
                dataLabel.Left = columnPositions[i]; //center under column
                dataLabel.Top = rowPosition + (int)(dataLabel.Height * .4);
                dataLabel.Visible = true;
                if (dataValues[11] == 1 && dataValues[12] == 0)
                {
                    if (dataValues[i] != controlValues[i])
                    {
                        dataLabel.ForeColor = ErrorColor;
                        seasonDataWarning = true;
                    }
                    else
                    {
                        dataLabel.ForeColor = ThemeColor;
                    }
                }
                else if(dataValues[12] == 1 && dataValues[11] == 0)
                {
                    if (dataValues[i] != controlCurrentValues[i])
                    {
                        dataLabel.ForeColor = ErrorColor;
                        seasonDataWarning = true;
                    }
                    else
                    {
                        dataLabel.ForeColor = ThemeColor;
                    }
                }
                else
                {
                    if (dataValues[i] != controlValues[i] || dataValues[i] != controlCurrentValues[i])
                    {
                        dataLabel.ForeColor = ErrorColor;
                        seasonDataError = true;
                    }
                    else
                    {
                        dataLabel.ForeColor = ThemeColor;
                    }
                }
            }
            if(!seasonDataWarning && !seasonDataError)
            {
                seasonDataSuccess = true;
            }
        }
        private Label GetOrCreateDataLabel(string key, float fontSize)
        {
            if (!dataLabels.ContainsKey(key))
            {
                Label dataLabel = new Label
                {
                    Tag = "Data",
                    ForeColor = ThemeColor,
                    BackColor = Color.Transparent
                };
                dataLabel.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, (int)(pnlDbUtil.Width * .7), dataLabel);
                dataLabels[key] = dataLabel;
                pnlDbOverview.Controls.Add(dataLabel);
            }
            return dataLabels[key];
        }
        private void CreateGridLines(List<int> columnPositions, List<int> rowPositions, int topTable, int rowHeight)
        {
            ClearGridLines();

            //calculate height for vertical lines

            int height = rowHeight * popCount + lblEmpty.Height + topTable;

            //create vertical lines
            foreach (int x in columnPositions)
            {
                Panel line = new Panel
                {
                    Width = 1,
                    Height = height,
                    Left = x,
                    Top = topTable,
                    BackColor = ThemeColor,
                    Visible = true,
                    TabIndex = 0
                };
                pnlDbOverview.Controls.Add(line);
                pnlDbOverview.Controls.SetChildIndex(line, 0);
                gridLines.Add(line);
            }

            //create horizontal lines
            foreach (int y in rowPositions)
            {
                Panel line = new Panel
                {
                    Width = lineWidth,
                    Height = 1,
                    Left = 0,
                    Top = y,
                    BackColor = ThemeColor,
                    Visible = true
                };
                pnlDbOverview.Controls.Add(line);
                gridLines.Add(line);
            }
            lineHeight = 0;
            lineWidth = 0;
        }
        private void ClearGridLines()
        {
            foreach (Panel line in gridLines)
            {
                pnlDbOverview.Controls.Remove(line);
                line.Dispose();
            }
            gridLines.Clear();
        }
        private void ClearOverview()
        {
            //hide all controls
            foreach (Label header in tableHeaders)
            {
                header.Visible = false;
            }

            foreach (Label yearLabel in yearLabels.Values)
            {
                yearLabel.Visible = false;
            }

            //hide data labels
            foreach (Label dataLabel in dataLabels.Values)
            {
                dataLabel.Visible = false;
            }

            if (lblUnpopulated != null)
            {
                lblUnpopulated.Visible = false;
            }

            ClearGridLines();
        }
        #endregion
        public void SettingsClick(Control control, PictureBox picture, float fontSize)
        {
            fontSize = ((float)(screenFontSize * lblServer.Height) / (96 / 12)) * (72 / 12) / 2;
            control.Click += (s, e) =>
            {
                if (lblSettings.Focused)
                {
                    this.ActiveControl = null;
                    if (picture.Image != null)
                    {
                        picture.Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        picture.Refresh();
                    }
                    lblSettings.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, (int)(pnlWelcome.Width * .7), lblSettings);
                    lblSettings.Left = (pnlWelcome.ClientSize.Width - lblSettings.Width) / 2;
                    SettingsVisibility(false);
                }
                else
                {
                    lblSettings.Focus();
                    if (picture.Image != null)
                    {
                        picture.Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        picture.Refresh();
                    }
                    lblSettings.Font = SetFontSize("Segoe UI", (float)(fontSize * 1.1), FontStyle.Bold | FontStyle.Underline, (int)(pnlWelcome.Width * .73), lblSettings);
                    lblSettings.Left = (pnlWelcome.ClientSize.Width - lblSettings.Width) / 2;
                    SettingsVisibility(true);
                }
            };
        }
        public void SettingsVisibility(bool vis)
        {
            pnlSettings.Visible = vis;
        }


        #endregion




        #region Need to organize/move or delete


        public HashSet<string> downloadedSeasons = new HashSet<string>();
        public HashSet<string> missingSeasons = new HashSet<string>();
        public bool allFilesDownloaded = false;
        public void CheckDataFiles()
        {
            if (!isPopulating)
            {
                listSeasons.Items.Clear();
                listDownloadSeasonData.Items.Clear();
                downloadedSeasons.Clear();
                missingSeasons.Clear();
                string historicDataPath = Path.Combine(projectRoot, @"Content\Historic Data\");
                int k = 0;
                for (int i = 2024; i >= 1996; i--)
                {
                    string seasonID = i.ToString();
                    listSeasons.Items.Add(seasonID);

                    //Check if ALL files exist for this season
                    int iter = (int.Parse(seasonID) <= 2012 || int.Parse(seasonID) == 2019 || int.Parse(seasonID) == 2020) ? 3 : 4;
                    bool allFilesExist = true;

                    for (int j = 0; j < iter; j++)
                    {
                        string fileName = $"{seasonID}p{j}.json";
                        if (!File.Exists(Path.Combine(historicDataPath, fileName)))
                        {
                            allFilesExist = false;
                            break;
                        }
                    }
                    if (allFilesExist)
                    {
                        downloadedSeasons.Add(seasonID);
                        if (k != -1)
                        {
                            k = 0;
                        }
                        allFilesDownloaded = true;
                    }
                    else
                    {
                        k = -1;
                        missingSeasons.Add(seasonID);
                        listDownloadSeasonData.Items.Add(seasonID);
                        allFilesDownloaded = false;
                        ButtonChangeState(btnDownloadSeasonData, true);
                    }
                }
            }
            else
            {
                listDownloadSeasonData.Items.Clear();
                missingSeasons.Clear();
                string historicDataPath = Path.Combine(projectRoot, @"Content\Historic Data\");
                int k = 0;
                for (int i = 2024; i >= 1996; i--)
                {
                    string seasonID = i.ToString();

                    //Check if ALL files exist for this season
                    int iter = (int.Parse(seasonID) <= 2012 || int.Parse(seasonID) == 2019 || int.Parse(seasonID) == 2020) ? 3 : 4;
                    bool allFilesExist = true;

                    for (int j = 0; j < iter; j++)
                    {
                        string fileName = $"{seasonID}p{j}.json";
                        if (!File.Exists(Path.Combine(historicDataPath, fileName)))
                        {
                            allFilesExist = false;
                            break;
                        }
                    }
                    if (allFilesExist)
                    {
                        if (k != -1)
                        {
                            k = 0;
                        }
                        allFilesDownloaded = true;
                    }
                    else
                    {
                        k = -1;
                        missingSeasons.Add(seasonID);
                        listDownloadSeasonData.Items.Add(seasonID);
                        allFilesDownloaded = false;
                    }
                }
            }

        }
        public void GetSeasonInfo()
        {
            seasonInfo.Clear();
            if (dbConnection)
            {
                using (SqlConnection conn = new SqlConnection(bob.ToString()))
                using (SqlCommand SQLSeasons = new SqlCommand("Seasons", conn))
                {
                    SQLSeasons.CommandType = CommandType.StoredProcedure;
                    conn.Open();

                    using (SqlDataReader sdr = SQLSeasons.ExecuteReader())
                    {
                        while (sdr.Read())
                        {
                            seasonInfo.Add((sdr.GetInt32(0), (sdr.GetInt32(1), sdr.GetInt32(2), sdr.GetInt32(3), sdr.GetInt32(4), sdr.GetInt32(5), sdr.GetInt32(6), sdr.GetInt32(7)
                                , sdr.GetInt32(8), sdr.GetInt32(9), sdr.GetInt32(10), sdr.GetInt32(11), sdr.GetInt32(12), sdr.GetInt32(13), sdr.GetInt32(14))));
                        }
                    }
                }
            }
        }
        public Stopwatch stopwatchDelete = new Stopwatch();
        public void AlterDeleteExisting()
        {
            int rows = 0;
            try
            {
                using (SqlConnection connection = new SqlConnection(cString))
                {
                    connection.Open();
                    using (SqlCommand keysOff = new SqlCommand("TableKeysOff", connection))
                    {
                        keysOff.CommandType = CommandType.StoredProcedure;
                        keysOff.Parameters.AddWithValue("@SeasonID", SeasonID);
                        using (SqlDataReader reader = keysOff.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                rows = Convert.ToInt32(reader[1]);
                            }
                        }
                    }
                    if (rows != 0)
                    {
                        using (SqlCommand alterDeletePbp = new SqlCommand("AlterTablesDeletePBP", connection))
                        {
                            alterDeletePbp.CommandType = CommandType.StoredProcedure;
                            alterDeletePbp.Parameters.AddWithValue("@season", SeasonID);
                            alterDeletePbp.ExecuteNonQuery();
                        }
                        stopwatchAlterDelete.Stop();
                        // Second: Delete other tables' data
                        using (SqlCommand deleteOthers = new SqlCommand("DeleteSeasonData", connection))
                        {
                            deleteOthers.CommandType = CommandType.StoredProcedure;
                            deleteOthers.Parameters.AddWithValue("@season", SeasonID);
                            deleteOthers.ExecuteNonQuery();
                        }
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error in deletion process: {e.Message}");
            }
        }



        #endregion






        //Historic Data
        #region Historic Data
        //Declarations
        #region Declarations
        public bool teamsDone = false;
        public bool arenasDone = false;
        public bool playersDone = false;
        public HashSet<(int SeasonID, int TeamID)> teamList = new HashSet<(int, int)>();
        public HashSet<(int SeasonID, int PlayerID)> playerList = new HashSet<(int, int)>();
        public HashSet<(int SeasonID, int OfficialID)> officialList = new HashSet<(int, int)>();
        public HashSet<(int SeasonID, int ArenaID)> arenaList = new HashSet<(int, int)>();
        public string teamInsert = "";
        public string arenaInsert = "";
        public string officialInsert = "";
        public string playerInsert = "";
        public string gameInsert = "";
        public string teamBoxInsert = "";
        public string startingLineupsInsertString = "";
        public string playByPlayInsertString = "";
        public string playerBoxInsertString = "";
        public int lastSeason = 0; //Used with Players/PlayerBox
        #endregion

        [DllImport("psapi.dll")]
        static extern bool EmptyWorkingSet(IntPtr process);
        public async Task ReadSeasonFile(bool bHistoric, bool bCurrent)
        {
            root = null;
            string filePath = Path.Combine(projectRoot, "Content\\", "dbconfig.json");              //Line 2050 is TESTing data, 2049 normal
            filePath = filePath.Replace("dbconfig.json", "Historic Data\\");
            //filePath = filePath.Replace("dbconfig.json", "Historic Data\\test\\");
            int iter = (SeasonID <= 2012 || SeasonID == 2019 || SeasonID == 2020) ? 3 : 4;      //No Selection
            root = await historic.ReadFile(SeasonID, iter, filePath);
        }
        public async Task InsertGameWithLoading(NBAdbToolboxHistoric.Game game, int season, int imageIteration, string sender)
        {
            await Task.Run(() =>
            {
                GetGameDetails(game, sender);
            });

        }

        //For each game, this method checks each of our tables and determines what to do, Insert, update, or nothing.
        public void GetGameDetails(NBAdbToolboxHistoric.Game game, string sender)
        {
            // Clear the StringBuilder before starting

            //Check Db and build strings for Inserts & Updates
            #region Check Db and build strings for Inserts & Updates
            //Teams
            TeamStaging(game);//5.7 Populate DB Update

            //Arenas
            if (!arenaList.Contains((SeasonID, game.box.arena.arenaId)))
            {
                arenaList.Add((SeasonID, game.box.arena.arenaId));
                HistoricArenaInsert(game.box.arena, game.box.homeTeamId, "Historic"); //5.7 Populate DB Update
            }

            //Officials
            List<int> officials = new List<int>();
            foreach (NBAdbToolboxHistoric.Official official in game.box.officials)
            {
                if (!officialList.Contains((SeasonID, official.personId)))
                {
                    HistoricOfficialInsert(official, "Historic"); //5.7 Populate DB Update
                }
                officials.Add(official.personId);
            }

            //Games
            HistoricGameInsert(game, sender, officials); //5.7 Populate DB Update

            //TeamBox
            HistoricTeamBoxStaging(game); //5.7 Populate DB Update

            //Players
            HistoricPlayerStaging(game); //5.7 Populate DB Update

            //PlayByPlay
            HistoricPlayByPlayStaging(game.playByPlay); //5.7 Populate DB Update
            #endregion

            //Insert data into Main tables and wait for execution
            #region Insert data into Main tables and wait for execution
            // Get the SQL string from the StringBuilder
            string hitDb = "set nocount on;\n" + sqlBuilder.ToString();
            sqlBuilder.Clear();
            try
            {
                using (SqlConnection bigInserts = new SqlConnection(cString))
                {
                    using (SqlCommand AllInOneInsert = new SqlCommand(hitDb))
                    {
                        AllInOneInsert.Connection = bigInserts;
                        AllInOneInsert.CommandType = CommandType.Text;
                        bigInserts.Open();
                        AllInOneInsert.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception e)
            {

            }
            hitDb = null;
            #endregion
        }

        //Team methods
        #region Team Methods
        public void TeamStaging(NBAdbToolboxHistoric.Game game)
        {
            if (!teamList.Contains((SeasonID, game.box.homeTeam.teamId)))
            {
                //TeamCheck(game.box.homeTeam, game.box.homeTeam.teamId, SeasonID);
                HistoricTeamInsert(game.box.homeTeam, game.box.homeTeam.teamId); //5.7 Populate DB Update
            }
            if (!teamList.Contains((SeasonID, game.box.awayTeam.teamId)))
            {
                //TeamCheck(game.box.awayTeam, game.box.awayTeam.teamId, SeasonID);
                HistoricTeamInsert(game.box.awayTeam, game.box.awayTeam.teamId); //5.7 Populate DB Update
            }
            if (game.box.homeTeam.teamWins + game.box.homeTeam.teamLosses == 82 ||  //If we've reached the last game of the SeasonID (or over 40 for covid shortened 2019) update Team records 
            (SeasonID == 2019 && (game.box.homeTeam.teamWins + game.box.homeTeam.teamLosses >= 40)) ||
            (SeasonID == 2020 && (game.box.homeTeam.teamWins + game.box.homeTeam.teamLosses == 72)))
            {
                //TeamUpdate(game.box.homeTeam, SeasonID);
                HistoricTeamUpdate(game.box.homeTeam, game.box.homeTeamId); //5.7 Populate DB Update
            }
            if (game.box.awayTeam.teamWins + game.box.awayTeam.teamLosses == 82 ||  //Same for away team
            (SeasonID == 2019 && (game.box.awayTeam.teamWins + game.box.awayTeam.teamLosses >= 40)) ||
            (SeasonID == 2020 && (game.box.awayTeam.teamWins + game.box.awayTeam.teamLosses == 72)))
            {
                //TeamUpdate(game.box.awayTeam, SeasonID);
                HistoricTeamUpdate(game.box.awayTeam, game.box.awayTeamId); //5.7 Populate DB Update
            }
        }
        public void HistoricTeamInsert(NBAdbToolboxHistoric.Team team, int teamID)
        {
            teamList.Add((SeasonID, team.teamId));

            sqlBuilder.Append("insert into Team values(")
                      .Append(SeasonID).Append(", ")
                      .Append(team.teamId).Append(", '")
                      .Append(team.teamCity).Append("', '")
                      .Append(team.teamName).Append("', '")
                      .Append(team.teamTricode).Append("', ")
                      .Append(team.teamWins).Append(", ")
                      .Append(team.teamLosses).Append(", '(")
                      .Append(team.teamTricode).Append(") ")
                      .Append(team.teamCity).Append(" ")
                      .Append(team.teamName).Append("')\n");
        }
        public void HistoricTeamUpdate(NBAdbToolboxHistoric.Team team, int teamID)
        {
            sqlBuilder.Append("update Team set WIns = ")
                      .Append(team.teamWins).Append(", Losses = ")
                      .Append(team.teamLosses).Append(" where TeamID = ")
                      .Append(teamID).Append(" and SeasonID = ")
                      .Append(SeasonID).Append("\n");
        }

        #endregion

        //Arena methods
        #region Arena Methods
        public void HistoricArenaInsert(NBAdbToolboxHistoric.Arena arena, int teamID, string sender)
        {

            sqlBuilder.Append("insert into Arena values(")
                      .Append(SeasonID).Append(", ")
                      .Append(arena.arenaId).Append(", ")
                      .Append(teamID).Append(", '")
                      .Append(arena.arenaCity).Append("', '")
                      .Append(arena.arenaCountry).Append("', '")
                      .Append(arena.arenaName).Append("', '")
                      .Append(arena.arenaPostalCode).Append("', '")
                      .Append(arena.arenaState).Append("', '")
                      .Append(arena.arenaStreetAddress).Append("', '")
                      .Append(arena.arenaTimezone).Append("')\n");
        }

        #endregion

        //Official methods
        #region Official Methods
        public void OfficialCheck(NBAdbToolboxHistoric.Official official, int season, string sender)
        {
            using (SqlCommand TeamSearch = new SqlCommand("OfficialCheck"))
            {
                TeamSearch.CommandType = CommandType.StoredProcedure;
                TeamSearch.Parameters.AddWithValue("@OfficialID", official.personId);
                TeamSearch.Parameters.AddWithValue("@SeasonID", season);
                using (SqlDataAdapter sTeamSearch = new SqlDataAdapter())
                {
                    TeamSearch.Connection = SQLdb;
                    sTeamSearch.SelectCommand = TeamSearch;
                    SQLdb.Open();
                    SqlDataReader reader = TeamSearch.ExecuteReader();
                    if (reader.HasRows)
                    {
                        SQLdb.Close();
                    }
                    else
                    {
                        SQLdb.Close();
                        if (sender == "Historic")
                        {
                            officialList.Add((season, official.personId));
                        }
                        else if (sender == "Missing")
                        {
                            officialList.Add((season, official.personId));
                        }
                        officialInsert += "insert into Official values(" + season + ", " + official.personId + ", '" + official.name.Replace("'", "''") + "', '" + official.jerseyNum + "')\n";
                    }
                }
            }
        }

        //5.7 Populate DB Update
        public void HistoricOfficialInsert(NBAdbToolboxHistoric.Official official, string sender)
        {
            if (sender == "Historic")
            {
                officialList.Add((SeasonID, official.personId));
            }
            else if (sender == "Missing")
            {
                officialList.Add((SeasonID, official.personId));
            }

            // Properly escape single quotes in the official's name using StringBuilder
            string escapedName = official.name.Replace("'", "''");

            sqlBuilder.Append("insert into Official values(")
                      .Append(SeasonID).Append(", ")
                      .Append(official.personId).Append(", '")
                      .Append(escapedName).Append("', '")
                      .Append(official.jerseyNum).Append("')\n");
        }
        #endregion

        //Game methods
        #region Game Methods        
        public void GameCheck(NBAdbToolboxHistoric.Game game, int season, string sender, List<int> officials)
        {
            string iExt = "Insert into GameExt(SeasonID, GameID, Status, Attendance, Sellout, ";
            string vExt = "";
            using (SqlCommand GameCheck = new SqlCommand("GameCheck"))
            {
                GameCheck.CommandType = CommandType.StoredProcedure;
                GameCheck.Parameters.AddWithValue("@GameID", GameID);
                GameCheck.Parameters.AddWithValue("@SeasonID", season);
                using (SqlDataAdapter sGameSearch = new SqlDataAdapter())
                {
                    GameCheck.Connection = SQLdb;
                    sGameSearch.SelectCommand = GameCheck;
                    SQLdb.Open();
                    SqlDataReader reader = GameCheck.ExecuteReader();
                    reader.Read();
                    if (!reader.HasRows)
                    {
                        SQLdb.Close();
                        HistoricGameInsert(game, sender, officials);
                    }
                    else
                    {
                        if (reader.GetInt32(5) != game.box.homeTeam.score || reader.GetInt32(7) != game.box.awayTeam.score)
                        {
                            SQLdb.Close();
                            GameUpdate(game, sender);
                        }
                        else
                        {
                            SQLdb.Close();
                        }
                    }
                }
            }
        }

        //5.7 Populate DB Update
        public void HistoricGameInsert(NBAdbToolboxHistoric.Game game, string sender, List<int> officials)
        {
            SqlDateTime datetime = SqlDateTime.Parse(game.box.gameTimeUTC);
            SqlDateTime gameDate = SqlDateTime.Parse(game.box.gameEt.Remove(game.box.gameEt.IndexOf('T')));

            //Build the Game table insert
            sqlBuilder.Append("Insert into Game(SeasonID, GameID, Date, HomeID, HScore, AwayID, AScore, Datetime, ")
                      .Append("WinnerID, WScore, LoserID, Lscore, GameType, SeriesID) values(")
                      .Append(SeasonID).Append(", ")
                      .Append(GameID).Append(", '")
                      .Append(gameDate).Append("', ")
                      .Append(game.box.homeTeamId).Append(", ")
                      .Append(game.box.homeTeam.score).Append(", ")
                      .Append(game.box.awayTeamId).Append(", ")
                      .Append(game.box.awayTeam.score).Append(", '")
                      .Append(datetime).Append("', ");

            //Winner/loser logic
            if (game.box.homeTeam.score > game.box.awayTeam.score)
            {
                sqlBuilder.Append(game.box.homeTeamId).Append(", ")
                          .Append(game.box.homeTeam.score).Append(", ")
                          .Append(game.box.awayTeamId).Append(", ")
                          .Append(game.box.awayTeam.score).Append(", ");
            }
            else
            {
                sqlBuilder.Append(game.box.awayTeamId).Append(", ")
                          .Append(game.box.awayTeam.score).Append(", ")
                          .Append(game.box.homeTeamId).Append(", ")
                          .Append(game.box.homeTeam.score).Append(", ");
            }

            //GameType
            if (sender == "Regular Season")
            {
                sqlBuilder.Append("'RS', null)\n");
            }
            else if (sender == "Postseason")
            {
                sqlBuilder.Append("'PS', '" + GameID.ToString().Remove(7) + "')\n");
            }
            else
            {
                sqlBuilder.Append("null, null)\n");
            }

            //GameExt
            sqlBuilder.Append("Insert into GameExt(SeasonID, GameID, ArenaID, Status, Attendance, Sellout, Label, LabelDetail");

            //Officials
            for (int o = 0; o < officials.Count && o < 4; o++)
            {
                if (o == 0)
                {
                    sqlBuilder.Append(", OfficialID");
                }
                else if (o < 3)
                {
                    sqlBuilder.Append(", Official").Append(o + 1).Append("ID");
                }
                else if (o == 3)
                {
                    sqlBuilder.Append(", OfficialAlternateID");
                }
            }

            //Start the values part
            sqlBuilder.Append(") values(")
                      .Append(SeasonID).Append(", ")
                      .Append(GameID).Append(", ")
                      .Append(game.box.arena.arenaId).Append(", '")
                      .Append(game.box.gameStatusText).Append("', ")
                      .Append(game.box.attendance).Append(", ")
                      .Append(game.box.sellout).Append(", '")
                      .Append(game.box.gameLabel).Append("', '")
                      .Append(game.box.gameSubLabel).Append("'");
            //Add official values
            for (int o = 0; o < officials.Count && o < 4; o++)
            {
                sqlBuilder.Append(", ").Append(officials[o]);
            }
            sqlBuilder.Append(")\n");
        }
        public void GameUpdate(NBAdbToolboxHistoric.Game game, string sender)
        {
            using (SqlCommand GameUpdate = new SqlCommand("GameUpdate"))
            {
                GameUpdate.Connection = SQLdb;
                GameUpdate.CommandType = CommandType.StoredProcedure;
                GameUpdate.Parameters.AddWithValue("@SeasonID", SeasonID);
                GameUpdate.Parameters.AddWithValue("@GameID", GameID);
                GameUpdate.Parameters.AddWithValue("@HomeID", game.box.homeTeamId);
                GameUpdate.Parameters.AddWithValue("@AwayID", game.box.awayTeamId);
                GameUpdate.Parameters.AddWithValue("@HScore", game.box.homeTeam.score);
                GameUpdate.Parameters.AddWithValue("@AScore", game.box.awayTeam.score);
                if (game.box.homeTeam.score > game.box.awayTeam.score)
                {
                    GameUpdate.Parameters.AddWithValue("@WinnerID", game.box.homeTeamId);
                    GameUpdate.Parameters.AddWithValue("@LoserID", game.box.awayTeamId);
                    GameUpdate.Parameters.AddWithValue("@WScore", game.box.homeTeam.score);
                    GameUpdate.Parameters.AddWithValue("@LScore", game.box.awayTeam.score);
                }
                else
                {
                    GameUpdate.Parameters.AddWithValue("@WinnerID", game.box.awayTeamId);
                    GameUpdate.Parameters.AddWithValue("@LoserID", game.box.homeTeamId);
                    GameUpdate.Parameters.AddWithValue("@WScore", game.box.awayTeam.score);
                    GameUpdate.Parameters.AddWithValue("@LScore", game.box.homeTeam.score);
                }
                SQLdb.Open();
                GameUpdate.ExecuteScalar();
            }
        }
        #endregion

        //Player & PlayerBox Methods - Includes Inactive Players.Populates Player, PlayerBox and StartingLineups tables
        #region Player & PlayerBox Methods - Includes Inactive Players. Populates Player, PlayerBox and StartingLineups tables
        public void HistoricPlayerStaging(NBAdbToolboxHistoric.Game game)
        {
            //Process home team players
            foreach (NBAdbToolboxHistoric.Player player in game.box.homeTeam.players)
            {//Home Team
                if (!playerList.Contains((SeasonID, player.personId)))
                {
                    int index = game.box.homeTeamPlayers.FindIndex(p => p.personId == player.personId);
                    if (index == -1)
                    {
                        HistoricPlayerInsert(player, player.jerseyNum, "Historic");
                    }
                    else
                    {
                        HistoricPlayerInsert(player, game.box.homeTeamPlayers[index].jerseyNum, "Historic");
                    }
                }
                HistoricPlayerBoxInsert(game, player, game.box.homeTeamId, game.box.awayTeamId);
            }

            //Process away team players
            foreach (NBAdbToolboxHistoric.Player player in game.box.awayTeam.players)
            {//Away Team
                if (!playerList.Contains((SeasonID, player.personId)))
                {
                    int index = game.box.awayTeamPlayers.FindIndex(p => p.personId == player.personId);
                    if (index == -1)
                    {
                        HistoricPlayerInsert(player, player.jerseyNum, "Historic");
                    }
                    else
                    {
                        HistoricPlayerInsert(player, game.box.awayTeamPlayers[index].jerseyNum, "Historic");
                    }
                }
                HistoricPlayerBoxInsert(game, player, game.box.awayTeamId, game.box.homeTeamId);
            }

            //Process home team inactive players
            foreach (NBAdbToolboxHistoric.Inactive inactive in game.box.homeTeam.inactives)
            {
                if (!playerList.Contains((SeasonID, inactive.personId)))
                {
                    HistoricInactiveInsert(inactive, "Historic");
                }
                HistoricInactiveBoxInsert(game.box.homeTeamId, game.box.awayTeamId, inactive.personId);
            }

            //Process away team inactive players
            foreach (NBAdbToolboxHistoric.Inactive inactive in game.box.awayTeam.inactives)
            {
                if (!playerList.Contains((SeasonID, inactive.personId)))
                {
                    HistoricInactiveInsert(inactive, "Historic");
                }
                HistoricInactiveBoxInsert(game.box.awayTeamId, game.box.homeTeamId, inactive.personId);
            }
        }
        public void HistoricPlayerInsert(NBAdbToolboxHistoric.Player player, string number, string sender)
        {
            //Add player to appropriate tracking list
            if (sender == "Historic")
            {
                playerList.Add((SeasonID, player.personId));
            }
            else if (sender == "Missing")
            {
                playerList.Add((SeasonID, player.personId));
            }

            //Basic player information
            sqlBuilder.Append("Insert into Player values(")
                      .Append(SeasonID).Append(", ")
                      .Append(player.personId).Append(", '")
                      .Append(player.firstName.Replace("'", "''")).Append(" ")
                      .Append(player.familyName.Replace("'", "''")).Append("', '")
                      .Append(number).Append("', ");

            //Position (nullable)
            if (player.position != null && player.position != "")
            {
                sqlBuilder.Append("'").Append(player.position).Append("')\n");
            }
            else
            {
                sqlBuilder.Append("null)\n");
            }
        }
        public void HistoricInactiveInsert(NBAdbToolboxHistoric.Inactive inactive, string sender)
        {
            //Add inactive player to appropriate tracking list
            if (sender == "Historic")
            {
                playerList.Add((SeasonID, inactive.personId));
            }
            else if (sender == "Missing")
            {
                playerList.Add((SeasonID, inactive.personId));
            }

            //Inactive player has fewer columns
            sqlBuilder.Append("Insert into Player(SeasonID, PlayerID, Name) values(")
                      .Append(SeasonID).Append(", ")
                      .Append(inactive.personId).Append(", '")
                      .Append(inactive.firstName.Replace("'", "''")).Append(" ")
                      .Append(inactive.familyName.Replace("'", "''")).Append("')\n");
        }
        public void HistoricInactiveBoxInsert(int TeamID, int MatchupID, int InactiveID)
        {
            //Simple insert for inactive player stats
            sqlBuilder.Append("insert into PlayerBox(SeasonID, GameID, TeamID, MatchupID, PlayerID, Status) values(")
                      .Append(SeasonID).Append(", ")
                      .Append(GameID).Append(", ")
                      .Append(TeamID).Append(", ")
                      .Append(MatchupID).Append(", ")
                      .Append(InactiveID).Append(", 'INACTIVE')\n");
        }
        public void HistoricPlayerBoxInsert(NBAdbToolboxHistoric.Game game, NBAdbToolboxHistoric.Player player, int TeamID, int MatchupID)
        {
            //Column definitions
            sqlBuilder.Append("insert into PlayerBox(SeasonID, GameID, TeamID, MatchupID, PlayerID, Status, FGM, FGA, [FG%], FG2M, FG2A, FG3M, FG3A, [FG3%], FTM, FTA, [FT%], ")
                      .Append("ReboundsDefensive, ReboundsOffensive, ReboundsTotal, Assists, Turnovers, Steals, Blocks, Points, FoulsPersonal");

            //Minutes and PlusMinusPoints
            if (player.statistics.minutes != "")
            {
                sqlBuilder.Append(", Minutes");

                if (player.statistics.plusMinusPoints != 0)
                {
                    sqlBuilder.Append(", PlusMinusPoints");
                }
            }
            else if (player.statistics.minutes == "")
            {
                sqlBuilder.Append(", Minutes");
            }

            //FG2% and AssistsTurnoverRatio
            sqlBuilder.Append(", [FG2%], AssistsTurnoverRatio");

            //Value section
            sqlBuilder.Append(") values(")
                      .Append(SeasonID).Append(", ")
                      .Append(GameID).Append(", ")
                      .Append(TeamID).Append(", ")
                      .Append(MatchupID).Append(", ")
                      .Append(player.personId).Append(", 'ACTIVE', ")
                      .Append(player.statistics.fieldGoalsMade).Append(", ")
                      .Append(player.statistics.fieldGoalsAttempted).Append(", ")
                      .Append(player.statistics.fieldGoalsPercentage).Append(", ")
                      .Append(player.statistics.fieldGoalsMade - player.statistics.threePointersMade).Append(", ")
                      .Append(player.statistics.fieldGoalsAttempted - player.statistics.threePointersAttempted).Append(", ")
                      .Append(player.statistics.threePointersMade).Append(", ")
                      .Append(player.statistics.threePointersAttempted).Append(", ")
                      .Append(player.statistics.threePointersPercentage).Append(", ")
                      .Append(player.statistics.freeThrowsMade).Append(", ")
                      .Append(player.statistics.freeThrowsAttempted).Append(", ")
                      .Append(player.statistics.freeThrowsPercentage).Append(", ")
                      .Append(player.statistics.reboundsDefensive).Append(", ")
                      .Append(player.statistics.reboundsOffensive).Append(", ")
                      .Append(player.statistics.reboundsTotal).Append(", ")
                      .Append(player.statistics.assists).Append(", ")
                      .Append(player.statistics.turnovers).Append(", ")
                      .Append(player.statistics.steals).Append(", ")
                      .Append(player.statistics.blocks).Append(", ")
                      .Append(player.statistics.points).Append(", ")
                      .Append(player.statistics.foulsPersonal);

            //Minutes and PlusMinusPoints values
            if (player.statistics.minutes != "")
            {
                sqlBuilder.Append(", '")
                          .Append(player.statistics.minutes.Replace("PT", "").Replace("M", ":").Replace("S", ""))
                          .Append("'");

                if (player.statistics.plusMinusPoints != 0)
                {
                    sqlBuilder.Append(", ")
                              .Append(player.statistics.plusMinusPoints);
                }
            }
            else if (player.statistics.minutes == "")
            {
                sqlBuilder.Append(", '0'");
            }

            //FG2% calculation
            if ((double)(player.statistics.fieldGoalsAttempted - player.statistics.threePointersAttempted) != 0)
            {
                sqlBuilder.Append(", ")
                          .Append(Math.Round((double)(player.statistics.fieldGoalsMade - player.statistics.threePointersMade) /
                                   (double)(player.statistics.fieldGoalsAttempted - player.statistics.threePointersAttempted), 4));
            }
            else
            {
                sqlBuilder.Append(", 0");
            }

            //AssistsTurnoverRatio calculation
            if (player.statistics.turnovers > 0)
            {
                sqlBuilder.Append(", ")
                          .Append(Math.Round((double)(player.statistics.assists) / (double)(player.statistics.turnovers), 3))
                          .Append(")\n");
            }
            else
            {
                sqlBuilder.Append(", 0)\n");
            }

            //StartingLineups insert
            sqlBuilder.Append("insert into StartingLineups values(")
                      .Append(SeasonID).Append(", ")
                      .Append(GameID).Append(", ")
                      .Append(TeamID).Append(", ")
                      .Append(MatchupID).Append(", ")
                      .Append(player.personId).Append(", '");

            if (player.position == "")
            {
                sqlBuilder.Append("Bench', null)\n");
            }
            else
            {
                sqlBuilder.Append("Starters', '")
                          .Append(player.position)
                          .Append("')\n");
            }
        }


        #endregion

        //TeamBox methods
        #region TeamBox Methods
        public void HistoricTeamBoxStaging(NBAdbToolboxHistoric.Game game)
        {
            //5.7 Populate DB Update
            HistoricTeamBoxInsert(game.box.homeTeam, game.box.awayTeamId, game.box.awayTeam.statistics.points);
            HistoricTeamBoxInsert(game.box.awayTeam, game.box.homeTeamId, game.box.homeTeam.statistics.points);

            if (GameID != 20300778 && GameID != 29800661 && GameID != 29600332 && GameID != 29600370)
            {
                foreach (NBAdbToolboxHistoric.Lineups lineup in game.box.homeTeam.lineups)
                {
                    HistoricTeamBoxLineupsInsert(game.box.homeTeam, game.box.awayTeamId, game.box.awayTeam.statistics.points, lineup);
                }
                foreach (NBAdbToolboxHistoric.Lineups lineup in game.box.awayTeam.lineups)
                {
                    HistoricTeamBoxLineupsInsert(game.box.awayTeam, game.box.homeTeamId, game.box.homeTeam.statistics.points, lineup);
                }
            }
        }
        public void HistoricTeamBoxInsert(NBAdbToolboxHistoric.Team team, int MatchupID, int PointsAgainst)
        {
            //Column names
            sqlBuilder.Append("insert into TeamBox(SeasonID, GameID, TeamID, MatchupID, FGM, FGA, [FG%], FG2M, FG2A, FG3M, FG3A, [FG3%], FTM, FTA, [FT%], ")
                      .Append("ReboundsDefensive, ReboundsOffensive, ReboundsTotal, Assists, Turnovers, Steals, ")
                      .Append("Blocks, Points, FoulsPersonal, PointsAgainst, [FG2%], AssistsTurnoverRatio");

            //Value section
            sqlBuilder.Append(") values(")
                      .Append(SeasonID).Append(", ")
                      .Append(GameID).Append(", ")
                      .Append(team.teamId).Append(", ")
                      .Append(MatchupID).Append(", ")
                      .Append(team.statistics.fieldGoalsMade).Append(", ")
                      .Append(team.statistics.fieldGoalsAttempted).Append(", ")
                      .Append(team.statistics.fieldGoalsPercentage).Append(", ")
                      .Append(team.statistics.fieldGoalsMade - team.statistics.threePointersMade).Append(", ")
                      .Append(team.statistics.fieldGoalsAttempted - team.statistics.threePointersAttempted).Append(", ")
                      .Append(team.statistics.threePointersMade).Append(", ")
                      .Append(team.statistics.threePointersAttempted).Append(", ")
                      .Append(team.statistics.threePointersPercentage).Append(", ")
                      .Append(team.statistics.freeThrowsMade).Append(", ")
                      .Append(team.statistics.freeThrowsAttempted).Append(", ")
                      .Append(team.statistics.freeThrowsPercentage).Append(", ")
                      .Append(team.statistics.reboundsDefensive).Append(", ")
                      .Append(team.statistics.reboundsOffensive).Append(", ")
                      .Append(team.statistics.reboundsTotal).Append(", ")
                      .Append(team.statistics.assists).Append(", ")
                      .Append(team.statistics.turnovers).Append(", ")
                      .Append(team.statistics.steals).Append(", ")
                      .Append(team.statistics.blocks).Append(", ")
                      .Append(team.statistics.points).Append(", ")
                      .Append(team.statistics.foulsPersonal).Append(", ")
                      .Append(PointsAgainst).Append(", ");

            //FG2 percentage calculation
            if ((double)(team.statistics.fieldGoalsAttempted - team.statistics.threePointersAttempted) != 0)
            {
                sqlBuilder.Append(Math.Round((double)(team.statistics.fieldGoalsMade - team.statistics.threePointersMade) /
                    (double)(team.statistics.fieldGoalsAttempted - team.statistics.threePointersAttempted), 4)).Append(", ");
            }
            else
            {
                sqlBuilder.Append("0, ");
            }

            //Assists/turnover ratio calculation
            if (team.statistics.turnovers > 0)
            {
                sqlBuilder.Append(Math.Round((double)(team.statistics.assists) / (double)(team.statistics.turnovers), 3)).Append(")\n");
            }
            else
            {
                sqlBuilder.Append("0)\n");
            }
        }
        public void HistoricTeamBoxLineupsInsert(NBAdbToolboxHistoric.Team team, int MatchupID, int PointsAgainst, NBAdbToolboxHistoric.Lineups lineup)
        {
            int minutes = 0;
            int seconds = 0;

            //Column names
            sqlBuilder.Append("insert into TeamBoxLineups(SeasonID, GameID, TeamID, MatchupID, Unit, FGM, FGA, [FG%], FG2M, FG2A, FG3M, FG3A, [FG3%], FTM, FTA, [FT%], ")
                      .Append("ReboundsDefensive, ReboundsOffensive, ReboundsTotal, Assists, Turnovers, Steals, ")
                      .Append("Blocks, Points, FoulsPersonal, Minutes, [FG2%], AssistsTurnoverRatio");

            //Value section
            sqlBuilder.Append(") values(")
                      .Append(SeasonID).Append(", ")
                      .Append(GameID).Append(", ")
                      .Append(team.teamId).Append(", ")
                      .Append(MatchupID).Append(", '")
                      .Append(lineup.unit.Substring(0, 1).ToUpper()).Append(lineup.unit.Substring(1)).Append("', ")
                      .Append(lineup.fieldGoalsMade).Append(", ")
                      .Append(lineup.fieldGoalsAttempted).Append(", ")
                      .Append(lineup.fieldGoalsPercentage).Append(", ")
                      .Append(lineup.fieldGoalsMade - lineup.threePointersMade).Append(", ")
                      .Append(lineup.fieldGoalsAttempted - lineup.threePointersAttempted).Append(", ")
                      .Append(lineup.threePointersMade).Append(", ")
                      .Append(lineup.threePointersAttempted).Append(", ")
                      .Append(lineup.threePointersPercentage).Append(", ")
                      .Append(lineup.freeThrowsMade).Append(", ")
                      .Append(lineup.freeThrowsAttempted).Append(", ")
                      .Append(lineup.freeThrowsPercentage).Append(", ")
                      .Append(lineup.reboundsDefensive).Append(", ")
                      .Append(lineup.reboundsOffensive).Append(", ")
                      .Append(lineup.reboundsTotal).Append(", ")
                      .Append(lineup.assists).Append(", ")
                      .Append(lineup.turnovers).Append(", ")
                      .Append(lineup.steals).Append(", ")
                      .Append(lineup.blocks).Append(", ")
                      .Append(lineup.points).Append(", ")
                      .Append(lineup.foulsPersonal).Append(", '");

            //Calculate minutes for bench players
            if (lineup.unit == "bench")
            {
                for (int i = 0; i < team.players.Count; i++)
                {
                    if (team.players[i].position == "" && team.players[i].statistics.minutes != "")
                    {
                        try
                        {
                            minutes += Int32.Parse(team.players[i].statistics.minutes.Remove(team.players[i].statistics.minutes.IndexOf(":")));
                            seconds += Int32.Parse(team.players[i].statistics.minutes.Substring(team.players[i].statistics.minutes.IndexOf(":") + 1));
                        }
                        catch (ArgumentOutOfRangeException e)
                        {
                            minutes += Int32.Parse(team.players[i].statistics.minutes);
                            seconds += 0;

                        }
                    }
                }
                double secondsDiv = (double)seconds % 60;
                double minutesWhole = Math.Floor((double)(seconds / 60));
                minutes += (int)(minutesWhole);
                seconds = (int)secondsDiv;
                sqlBuilder.Append(minutes).Append(":").Append(seconds).Append(".00', ");
            }
            else
            {
                sqlBuilder.Append(lineup.minutes).Append(".00', ");
            }

            //FG2 percentage calculation
            if ((double)(lineup.fieldGoalsAttempted - lineup.threePointersAttempted) != 0)
            {
                sqlBuilder.Append(Math.Round((double)(lineup.fieldGoalsMade - lineup.threePointersMade) /
                    (double)(lineup.fieldGoalsAttempted - lineup.threePointersAttempted), 4)).Append(", ");
            }
            else
            {
                sqlBuilder.Append("0, ");
            }

            //Assists/turnover ratio calculation
            if (lineup.turnovers > 0)
            {
                sqlBuilder.Append(Math.Round((double)(lineup.assists) / (double)(lineup.turnovers), 3)).Append(")\n");
            }
            else
            {
                sqlBuilder.Append("0)\n");
            }
        }
        #endregion

        //PlayByPlay methods StringBuilder
        #region PlayByPlay Methods
        public StringBuilder sqlBuilder = new StringBuilder(220 * 1024); //Start with roughly .225 MB initial capacity
        public StringBuilder playByPlayBuilder = new StringBuilder(245 * 1024); //Start with about .25MB capacity
        public void PlayByPlayInsertString(NBAdbToolboxHistoric.Action action, int GameID)
        {
            StringBuilder insert = new StringBuilder("insert into PlayByPlay (SeasonID, GameID, ActionID, ActionNumber, Qtr, Clock, ");
            StringBuilder values = new StringBuilder(") values(")
                .Append(SeasonID).Append(", ")
                .Append(GameID).Append(", ")
                .Append(action.actionId).Append(", ")
                .Append(action.actionNumber).Append(", ")
                .Append(action.period).Append(", replace(replace(replace('")
                .Append(action.clock).Append("', 'PT', ''), 'M', ':'), 'S', ''), ");

            if (action.description == "" && action.actionType != "" && action.actionType != " ")
            {
                insert.Append("Description, ");
                values.Append("'").Append(action.actionType).Append("', ");
            }
            else if (action.description != "" && action.description != " ")
            {
                insert.Append("Description, ");
                values.Append("'").Append(action.description.Replace("'", "''")).Append("', ");
            }

            if (action.subType != "Unknown" && action.subType != "")
            {
                insert.Append("SubType, ");
                values.Append("'").Append(action.subType).Append("', ");
            }

            if (action.description.Length > 3)
            {
                if (action.description.Substring(action.description.Length - 4) == "STL)")
                {
                    insert.Append("ActionType, ");
                    values.Append("'Steal', ");
                }
                else if (action.description.Substring(action.description.Length - 4) == "BLK)")
                {
                    insert.Append("ActionType, ");
                    values.Append("'Block', ");
                }
                else
                {
                    insert.Append("ActionType, ");
                    values.Append("'").Append(action.actionType).Append("', ");
                }
            }
            else if (action.actionType != "" && action.actionType != " ")
            {
                insert.Append("ActionType, ");
                values.Append("'").Append(action.actionType).Append("', ");
            }

            if (action.location != "")
            {
                insert.Append("Location, ");
                values.Append("'").Append(action.location).Append("', ");
            }

            //Field Goals
            if (action.isFieldGoal == 1)
            {
                insert.Append("IsFieldGoal, ");
                values.Append(action.isFieldGoal).Append(", ");

                if (action.shotResult == "Made")
                {
                    insert.Append("ShotType, ");
                    values.Append("'FG").Append(action.shotValue).Append("M', ");
                    insert.Append("PtsGenerated, ");
                    values.Append(action.shotValue).Append(", ");
                }
                else if (action.shotResult == "Missed")
                {
                    insert.Append("ShotType, ");
                    values.Append("'FG").Append(action.shotValue).Append("A', ");
                    insert.Append("PtsGenerated, ");
                    values.Append("0, ");
                }

                insert.Append("ShotResult, ShotValue, ShotDistance, ");
                values.Append("'").Append(action.shotResult).Append("', ")
                     .Append(action.shotValue).Append(", ")
                     .Append(action.shotDistance).Append(", ");
            }
            else if (action.actionType == "Free Throw")
            {
                if (action.description.Substring(action.description.Length - 4) == "PTS)")
                {
                    insert.Append("ShotType, PtsGenerated, ShotResult, ShotValue, ");
                    values.Append("'FTM', 1, 'Made', 1, ");
                }
                else if (action.description.Substring(0, 4) == "MISS")
                {
                    insert.Append("ShotType, PtsGenerated, ShotResult, ShotValue, ");
                    values.Append("'FTA', 0, 'Missed', 1, ");
                }
            }

            if (action.scoreAway != "")
            {
                insert.Append("ScoreAway, ");
                values.Append(action.scoreAway).Append(", ");
            }

            if (action.scoreHome != "")
            {
                insert.Append("ScoreHome, ");
                values.Append(action.scoreHome).Append(", ");
            }

            if (action.teamId != 0)
            {
                insert.Append("TeamID, Tricode, ");
                values.Append(action.teamId).Append(", '")
                     .Append(action.teamTricode).Append("', ");
            }

            if (action.personId != 0)
            {
                insert.Append("PlayerID, ");
                values.Append(action.personId).Append(", ");
            }

            if (action.isFieldGoal == 1)
            {
                insert.Append("Xlegacy, Ylegacy, ");
                values.Append(action.xLegacy).Append(", ")
                     .Append(action.yLegacy).Append(", ");
            }

            // Remove trailing comma and space
            insert.Length = insert.Length - 2;
            values.Length = values.Length - 2;

            // Finalize the values part
            values.Append(") ");

            // Append to the main builder 
            playByPlayBuilder.Append(insert)
                            .Append(values)
                            .Append("\n");

        }
        public void HistoricPlayByPlayStaging(NBAdbToolboxHistoric.PlayByPlay pbp)
        {
            int actions = pbp.actions.Count;
            for (int i = 0; i < actions; i++)
            {
                PlayByPlayInsertString(pbp.actions[i], Int32.Parse(pbp.gameId));
            }

        }
        #endregion

        #endregion

        //Current Data
        #region Current Data StringBuilder Update 5.17
        //Declarations
        #region Declarations
        public string missingData = "";
        //Used in Game region
        public string insertExt = "";
        public string valuesExt = "";
        public string lastGame = "";
        public string seriesIDfirst7 = "";
        public StringBuilder sqlBuilderParallel = new StringBuilder(220 * 1024); //Start with roughly .225 MB initial capacity
        #endregion
        public bool boxMissing = false;
        public async Task CurrentGameGPS(int GameID, string sender)
        {
            bool doBox = true;
            bool doPBP = true;
            boxMissing = false;
            bool useHistoricBox = false;
            bool useHistoricPBP = false;
            string missingNote = "";
            missingData = "";
            //Try to get Current Data
            #region Try to get Current Data
            rootCPBP = await currentDataPBP.GetJSON(GameID, SeasonID);
            if (rootCPBP.game == null)
            {
                doPBP = false;
                useHistoricPBP = true;
                missingNote = "'No file available from NBA')\n";
            }
            rootC = await currentData.GetJSON(GameID, SeasonID);
            if (rootC.game == null)
            {
                boxMissing = true;
                doBox = false;
                useHistoricBox = true;
                missingNote = "'No file available from NBA')\n";
            }
            #endregion

            //If it's available, get Boxscore endpoint data ready
            #region Box endpoint data collection
            if (doBox)
            {
                try
                {
                    //CurrentInsertStaging(rootC.game, season);
                    CurrentDataDriver(rootC.game);
                }
                catch (Exception e)
                {
                    useHistoricBox = true;
                    missingNote = "'JSON file formatting - NBA pls fix')\n";
                    ErrorOutput(e);
                }
            }
            else
            {
                missingData += "insert into util.MissingData values(" + SeasonID + ", " + GameID + ", 'Current', 'Box', " + missingNote + "\n";
            }
            #endregion

            //If it's available, get PlayByPlay endpoint data ready
            #region PlayByPlay endpoint data collection
            if (doPBP)
            {
                try
                {
                    InitiateCurrentPlayByPlay(rootCPBP.game);
                }
                catch (Exception e)
                {
                    useHistoricPBP = true;
                    missingNote = "'JSON file formatting - NBA pls fix')\n";
                    ErrorOutput(e);
                }
            }
            else
            {
                missingData += "insert into util.MissingData values(" + SeasonID + ", " + GameID + ", 'Current', 'PlayByPlay', " + missingNote + "\n";
            }
            #endregion

            #region Getting historic Data for missing data

            if (useHistoricBox || useHistoricPBP)
            {
                MissingDataGPS(useHistoricBox, useHistoricPBP);
            }
            #endregion
        }
        public void CurrentDataDriver(NBAdbToolboxCurrent.Game game) //Replacing CurrentInsertStaging
        {
            foreach (NBAdbToolboxCurrent.Team team in new[] { game.homeTeam, game.awayTeam })
            {
                int MatchupID = (team == game.homeTeam) ? game.awayTeam.teamId : game.homeTeam.teamId;
                CurrentTeamBox(team, MatchupID);
                InitiateCurrentPlayerBox(game, team, MatchupID);
            }
            if (!teamsDone)
            {
                InitiateCurrentTeam(game);
            }
            if (!arenaList.Contains((SeasonID, game.arena.arenaId)))
            {
                arenaList.Add((SeasonID, game.arena.arenaId));
                CurrentArena(game.arena, game.homeTeam.teamId);
            }
            Dictionary<int, string> officials = new Dictionary<int, string>();
            foreach (NBAdbToolboxCurrent.Official official in game.officials)
            {
                if (!officialList.Contains((SeasonID, official.personId)))
                {
                    officialList.Add((SeasonID, official.personId));
                    officials.Add(official.personId, official.assignment);
                    CurrentOfficial(official);
                }
            }
            CurrentGame(game, officials);
            CurrentPlayer(game);

            // Append the parallel builder content to the main SQL builder
            //CheckStringBuildSize(GameID, "sqlBuilderParallel", sqlBuilderParallel);
            //CheckStringBuildSize(GameID, "sqlBuilder", sqlBuilder);

            sqlBuilder.Append("\n").Append(sqlBuilderParallel.ToString());
            sqlBuilderParallel.Clear();
            string execute = sqlBuilder.ToString();
            Task InsertCurrent = CurrentDataInsert(execute);
            sqlBuilder.Clear();
        }
        public async Task CurrentDataInsert(string execute)
        {
            if (execute == "")
            {
                return;
            }
            try
            {
                using (SqlConnection connection = new SqlConnection(cString))
                using (SqlCommand insert = new SqlCommand("set nocount on;\n" + execute, connection))
                {
                    insert.CommandType = CommandType.Text;
                    connection.Open();
                    insert.ExecuteNonQuery();
                }
            }
            catch (Exception e)
            {
                ErrorOutput(e);
            }
        }

        #region Team
        public void InitiateCurrentTeam(NBAdbToolboxCurrent.Game game) //Replacing CurrentTeamStaging
        {
            if (!teamList.Contains((SeasonID, game.homeTeam.teamId)))
            {
                teamList.Add((SeasonID, game.homeTeam.teamId));
                CurrentTeam(game.homeTeam);

                if (teamList.Count == 30)
                {
                    teamsDone = true;
                }
            }

            if (!teamList.Contains((SeasonID, game.awayTeam.teamId)))
            {
                teamList.Add((SeasonID, game.awayTeam.teamId));
                CurrentTeam(game.awayTeam);

                if (teamList.Count == 30)
                {
                    teamsDone = true;
                }
            }
        }

        public void CurrentTeam(NBAdbToolboxCurrent.Team team)
        {
            sqlBuilder.Append("insert into Team(SeasonID, TeamID, City, Name, Tricode, FullName) values(")
                      .Append(SeasonID).Append(", ")
                      .Append(team.teamId).Append(", '")
                      .Append(team.teamCity).Append("', '")
                      .Append(team.teamName).Append("', '")
                      .Append(team.teamTricode).Append("', '(")
                      .Append(team.teamTricode).Append(") ")
                      .Append(team.teamCity).Append(" ")
                      .Append(team.teamName).Append("')\n");
        }
        #endregion

        #region TeamBox
        public void CurrentTeamBox(NBAdbToolboxCurrent.Team team, int MatchupID)
        {
            sqlBuilderParallel.Append("insert into TeamBox(SeasonID, GameID, TeamID, MatchupID, Assists, AssistsTurnoverRatio, BenchPoints, BiggestLead, BiggestLeadScore, BiggestScoringRun, BiggestScoringRunScore, Blocks, BlocksReceived, FastBreakPointsAttempted, FastBreakPointsMade, FastBreakPointsPercentage, FGA, FieldGoalsEffectiveAdjusted, FGM, [FG%], FoulsOffensive, FoulsDrawn, FoulsPersonal, FoulsTeam, FoulsTechnical, FoulsTeamTechnical, FTA, FTM, [FT%], LeadChanges, Points, PointsAgainst, PointsFastBreak, PointsFromTurnovers, PointsInThePaint, PointsInThePaintAttempted, PointsInThePaintMade, PointsInThePaintPercentage, PointsSecondChance, ReboundsDefensive, ReboundsOffensive, ReboundsPersonal, ReboundsTeam, ReboundsTeamDefensive, ReboundsTeamOffensive, ReboundsTotal, SecondChancePointsAttempted, SecondChancePointsMade, SecondChancePointsPercentage, Steals, FG3A, FG3M, [FG3%], TimeLeading, TimesTied, TrueShootingAttempts, TrueShootingPercentage, Turnovers, TurnoversTeam, TurnoversTotal, FG2A, FG2M, [FG2%]) values(")
            // Append values
            .Append(SeasonID).Append(", ")
            .Append(GameID).Append(", ")
            .Append(team.teamId).Append(", ")
            .Append(MatchupID).Append(", ")
            .Append(team.statistics.assists).Append(", ")
            .Append(team.statistics.assistsTurnoverRatio).Append(", ")
            .Append(team.statistics.benchPoints).Append(", ")
            .Append(team.statistics.biggestLead).Append(", '")
            .Append(team.statistics.biggestLeadScore).Append("', ")
            .Append(team.statistics.biggestScoringRun).Append(", '")
            .Append(team.statistics.biggestScoringRunScore).Append("', ")
            .Append(team.statistics.blocks).Append(", ")
            .Append(team.statistics.blocksReceived).Append(", ")
            .Append(team.statistics.fastBreakPointsAttempted).Append(", ")
            .Append(team.statistics.fastBreakPointsMade).Append(", ")
            .Append(team.statistics.fastBreakPointsPercentage).Append(", ")
            .Append(team.statistics.fieldGoalsAttempted).Append(", ")
            .Append(team.statistics.fieldGoalsEffectiveAdjusted).Append(", ")
            .Append(team.statistics.fieldGoalsMade).Append(", ")
            .Append(team.statistics.fieldGoalsPercentage).Append(", ")
            .Append(team.statistics.foulsOffensive).Append(", ")
            .Append(team.statistics.foulsDrawn).Append(", ")
            .Append(team.statistics.foulsPersonal).Append(", ")
            .Append(team.statistics.foulsTeam).Append(", ")
            .Append(team.statistics.foulsTechnical).Append(", ")
            .Append(team.statistics.foulsTeamTechnical).Append(", ")
            .Append(team.statistics.freeThrowsAttempted).Append(", ")
            .Append(team.statistics.freeThrowsMade).Append(", ")
            .Append(team.statistics.freeThrowsPercentage).Append(", ")
            .Append(team.statistics.leadChanges).Append(", ")
            .Append(team.statistics.points).Append(", ")
            .Append(team.statistics.pointsAgainst).Append(", ")
            .Append(team.statistics.pointsFastBreak).Append(", ")
            .Append(team.statistics.pointsFromTurnovers).Append(", ")
            .Append(team.statistics.pointsInThePaint).Append(", ")
            .Append(team.statistics.pointsInThePaintAttempted).Append(", ")
            .Append(team.statistics.pointsInThePaintMade).Append(", ")
            .Append(team.statistics.pointsInThePaintPercentage).Append(", ")
            .Append(team.statistics.pointsSecondChance).Append(", ")
            .Append(team.statistics.reboundsDefensive).Append(", ")
            .Append(team.statistics.reboundsOffensive).Append(", ")
            .Append(team.statistics.reboundsPersonal).Append(", ")
            .Append(team.statistics.reboundsTeam).Append(", ")
            .Append(team.statistics.reboundsTeamDefensive).Append(", ")
            .Append(team.statistics.reboundsTeamOffensive).Append(", ")
            .Append(team.statistics.reboundsTotal).Append(", ")
            .Append(team.statistics.secondChancePointsAttempted).Append(", ")
            .Append(team.statistics.secondChancePointsMade).Append(", ")
            .Append(team.statistics.secondChancePointsPercentage).Append(", ")
            .Append(team.statistics.steals).Append(", ")
            .Append(team.statistics.threePointersAttempted).Append(", ")
            .Append(team.statistics.threePointersMade).Append(", ")
            .Append(team.statistics.threePointersPercentage).Append(", '")
            .Append(team.statistics.timeLeading).Append("', ")
            .Append(team.statistics.timesTied).Append(", ")
            .Append(team.statistics.trueShootingAttempts).Append(", ")
            .Append(team.statistics.trueShootingPercentage).Append(", ")
            .Append(team.statistics.turnovers).Append(", ")
            .Append(team.statistics.turnoversTeam).Append(", ")
            .Append(team.statistics.turnoversTotal).Append(", ")
            .Append(team.statistics.twoPointersAttempted).Append(", ")
            .Append(team.statistics.twoPointersMade).Append(", ")
            .Append(team.statistics.twoPointersPercentage).Append(")\n");
        }

        #endregion

        #region PlayerBox
        public void InitiateCurrentPlayerBox(NBAdbToolboxCurrent.Game game, NBAdbToolboxCurrent.Team team, int MatchupID)
        {
            foreach (NBAdbToolboxCurrent.Player player in team.players)
            {
                CurrentPlayerBox(player, Int32.Parse(game.gameId), team.teamId, MatchupID);
            }
        }
        public void CurrentPlayerBox(NBAdbToolboxCurrent.Player player, int GameID, int TeamID, int MatchupID)
        {
            // Main SQL builder
            sqlBuilderParallel.Append("insert into PlayerBox(SeasonID, GameID, TeamID, MatchupID, PlayerID, FGM, FGA, [FG%], FG2M, FG2A, [FG2%], FG3M, FG3A, [FG3%], FTM, FTA, [FT%], ReboundsDefensive, ReboundsOffensive, ReboundsTotal, Assists, Turnovers, Steals, Blocks, Points, FoulsPersonal");

            // Values builder
            StringBuilder valuesSB = new StringBuilder();
            valuesSB.Append(SeasonID).Append(", ")
                    .Append(GameID).Append(", ")
                    .Append(TeamID).Append(", ")
                    .Append(MatchupID).Append(", ")
                    .Append(player.personId).Append(", ")
                    .Append(player.statistics.fieldGoalsMade).Append(", ")
                    .Append(player.statistics.fieldGoalsAttempted).Append(", ")
                    .Append(player.statistics.fieldGoalsPercentage).Append(", ")
                    .Append(player.statistics.twoPointersMade).Append(", ")
                    .Append(player.statistics.twoPointersAttempted).Append(", ")
                    .Append(player.statistics.twoPointersPercentage).Append(", ")
                    .Append(player.statistics.threePointersMade).Append(", ")
                    .Append(player.statistics.threePointersAttempted).Append(", ")
                    .Append(player.statistics.threePointersPercentage).Append(", ")
                    .Append(player.statistics.freeThrowsMade).Append(", ")
                    .Append(player.statistics.freeThrowsAttempted).Append(", ")
                    .Append(player.statistics.freeThrowsPercentage).Append(", ")
                    .Append(player.statistics.reboundsDefensive).Append(", ")
                    .Append(player.statistics.reboundsOffensive).Append(", ")
                    .Append(player.statistics.reboundsTotal).Append(", ")
                    .Append(player.statistics.assists).Append(", ")
                    .Append(player.statistics.turnovers).Append(", ")
                    .Append(player.statistics.steals).Append(", ")
                    .Append(player.statistics.blocks).Append(", ")
                    .Append(player.statistics.points).Append(", ")
                    .Append(player.statistics.foulsPersonal);

            // Calculate minutes values - avoid exceptions with null/empty checks
            double minCalc = 0;
            string minutes = player.statistics.minutes;
            bool hasMinutes = !string.IsNullOrEmpty(minutes);

            if (hasMinutes)
            {
                // Only try to parse if we have minutes
                int mIndex = minutes.IndexOf("M");
                if (mIndex > 0)
                {
                    int colonIndex;
                    string minString = minutes.Replace("PT", "").Replace("M", ":").Replace("S", "");

                    // Check if we can safely parse
                    if (mIndex + 1 < minutes.Length && mIndex + 6 <= minutes.Length &&
                        double.TryParse(minutes.Substring(2, mIndex - 2), out double mins) &&
                        double.TryParse(minutes.Substring(mIndex + 1, 5), out double secs))
                    {
                        minCalc = Math.Round(mins + (secs / 60), 2);
                    }

                    // Minutes column
                    sqlBuilderParallel.Append(", Minutes");
                    valuesSB.Append(", '").Append(minString).Append("'");

                    // Plus/minus points if they exist
                    if (player.statistics.plusMinusPoints != 0)
                    {
                        sqlBuilderParallel.Append(", PlusMinusPoints, Plus, Minus");
                        valuesSB.Append(", ").Append(player.statistics.plusMinusPoints)
                                .Append(", ").Append(player.statistics.plus)
                                .Append(", ").Append(player.statistics.minus);
                    }
                }
            }
            else
            {
                // No minutes data available
                sqlBuilderParallel.Append(", Minutes");
                valuesSB.Append(", '0'");
            }

            // Handle assists/turnover ratio
            sqlBuilderParallel.Append(", AssistsTurnoverRatio");
            if (player.statistics.turnovers > 0)
            {
                valuesSB.Append(", ").Append(Math.Round((double)player.statistics.assists / player.statistics.turnovers, 3));
            }
            else
            {
                valuesSB.Append(", 0");
            }

            // Handle position (if exists)
            if (!string.IsNullOrEmpty(player.position))
            {
                sqlBuilderParallel.Append(", Starter");
                valuesSB.Append(", 1");
                sqlBuilderParallel.Append(", Position");
                valuesSB.Append(", '").Append(player.position).Append("'");
            }

            // Handle status
            sqlBuilderParallel.Append(", Status");
            valuesSB.Append(", '").Append(player.status).Append("'");

            // Handle status reason/description (if they exist)
            if (!string.IsNullOrEmpty(player.notPlayingReason))
            {
                sqlBuilderParallel.Append(", StatusReason");
                valuesSB.Append(", '").Append(player.notPlayingReason.Replace("'", "''")).Append("'");

                if (!string.IsNullOrEmpty(player.notPlayingDescription))
                {
                    sqlBuilderParallel.Append(", StatusDescription");
                    valuesSB.Append(", '").Append(player.notPlayingDescription.Replace("'", "''")).Append("'");
                }
            }

            // Add remaining columns
            sqlBuilderParallel.Append(", MinutesCalculated, BlocksReceived, PointsFastBreak, PointsInThePaint, PointsSecondChance, FoulsOffensive, FoulsDrawn, FoulsTechnical")
                             .Append(") values(")
                             .Append(valuesSB.ToString())
                             .Append(", ").Append(minCalc)
                             .Append(", ").Append(player.statistics.blocksReceived)
                             .Append(", ").Append(player.statistics.pointsFastBreak)
                             .Append(", ").Append(player.statistics.pointsInThePaint)
                             .Append(", ").Append(player.statistics.pointsSecondChance)
                             .Append(", ").Append(player.statistics.foulsOffensive)
                             .Append(", ").Append(player.statistics.foulsDrawn)
                             .Append(", ").Append(player.statistics.foulsTechnical)
                             .Append(")\n");

            // Add the starting lineups insert
            sqlBuilderParallel.Append("insert into StartingLineups values(")
                .Append(SeasonID).Append(", ")
                .Append(GameID).Append(", ")
                .Append(TeamID).Append(", ")
                .Append(MatchupID).Append(", ")
                .Append(player.personId).Append(", '")
                .Append(player.starter == "1" ? "Starters', '" + player.position + "'" : "Bench', null")
                .Append(")\n");
        }

        #endregion

        #region Arena
        public void CurrentArena(NBAdbToolboxCurrent.Arena arena, int teamID)
        {
            sqlBuilder.Append("insert into Arena(SeasonID, ArenaID, TeamID, City, Country, Name, State, Timezone) values(")
                      .Append(SeasonID).Append(", ")
                      .Append(arena.arenaId).Append(", ")
                      .Append(teamID).Append(", '")
                      .Append(arena.arenaCity).Append("', '")
                      .Append(arena.arenaCountry).Append("', '")
                      .Append(arena.arenaName).Append("', '")
                      .Append(arena.arenaState).Append("', '")
                      .Append(arena.arenaTimezone).Append("')\n");
        }
        #endregion

        #region Official
        public void CurrentOfficial(NBAdbToolboxCurrent.Official official)
        {
            sqlBuilder.Append("insert into Official values(")
                      .Append(SeasonID).Append(", ")
                      .Append(official.personId).Append(", '")
                      .Append(official.name.Replace("'", "''")).Append("', '")
                      .Append(official.jerseyNum).Append("')\n");
        }
        #endregion

        #region Game
        public void CurrentGame(NBAdbToolboxCurrent.Game game, Dictionary<int, string> officials)
        {
            // Create StringBuilder for Game table insert
            StringBuilder gameInsertSB = new StringBuilder();
            StringBuilder gameExtSB = new StringBuilder();

            // Parse the datetime values
            SqlDateTime datetime = SqlDateTime.Parse(game.gameTimeUTC);
            SqlDateTime gameDate = SqlDateTime.Parse(game.gameEt.Remove(game.gameEt.IndexOf('T')));

            // Build the Game insert statement
            gameInsertSB.Append("Insert into Game(SeasonID, GameID, Date, HomeID, HScore, AwayID, AScore, Datetime, WinnerID, WScore, LoserID, Lscore, GameType, SeriesID) values(")
                        .Append(SeasonID).Append(", ")
                        .Append(GameID).Append(", '")
                        .Append(gameDate).Append("', ")
                        .Append(game.homeTeam.teamId).Append(", ")
                        .Append(game.homeTeam.score).Append(", ")
                        .Append(game.awayTeam.teamId).Append(", ")
                        .Append(game.awayTeam.score).Append(", '")
                        .Append(datetime).Append("', ");

            // Determine winner/loser
            if (game.homeTeam.score > game.awayTeam.score)
            {
                gameInsertSB.Append(game.homeTeam.teamId).Append(", ")
                            .Append(game.homeTeam.score).Append(", ")
                            .Append(game.awayTeam.teamId).Append(", ")
                            .Append(game.awayTeam.score).Append(", ");
            }
            else
            {
                gameInsertSB.Append(game.awayTeam.teamId).Append(", ")
                            .Append(game.awayTeam.score).Append(", ")
                            .Append(game.homeTeam.teamId).Append(", ")
                            .Append(game.homeTeam.score).Append(", ");
            }

            // Handle game type and series ID
            string gameType = game.gameId.Substring(2, 1);
            if (gameType == "2")
            {
                gameInsertSB.Append("'RS', null)");
            }
            else if (gameType == "4")
            {
                string SeriesID = GameID.ToString().Remove(7);
                gameInsertSB.Append("'PS', '").Append(SeriesID).Append("')");
            }
            else if (gameType == "5")
            {
                gameInsertSB.Append("'PS', null)");
            }
            else if (gameType == "6")
            {
                gameInsertSB.Append("'RS', null)");
            }

            // Add newline
            gameInsertSB.Append("\n");

            // Build the GameExt insert
            gameExtSB.Append("Insert into GameExt(SeasonID, GameID, ArenaID, Status, Attendance");

            // Start the values part for GameExt
            StringBuilder gameExtValuesSB = new StringBuilder();
            gameExtValuesSB.Append(") values(")
                           .Append(SeasonID).Append(", ")
                           .Append(game.gameId).Append(", ")
                           .Append(game.arena.arenaId).Append(", '")
                           .Append(game.gameStatusText).Append("', ")
                           .Append(game.attendance);

            // Add sellout if applicable
            if (game.sellout != "")
            {
                gameExtSB.Append(", Sellout");
                gameExtValuesSB.Append(", ").Append(game.sellout);
            }

            // Add officials
            foreach (KeyValuePair<int, string> kvp in officials)
            {
                if (kvp.Value == "OFFICIAL1")
                {
                    gameExtSB.Append(", OfficialID");
                    gameExtValuesSB.Append(", ").Append(kvp.Key);
                }
                if (kvp.Value == "OFFICIAL2")
                {
                    gameExtSB.Append(", Official2ID");
                    gameExtValuesSB.Append(", ").Append(kvp.Key);
                }
                if (kvp.Value == "OFFICIAL3")
                {
                    gameExtSB.Append(", Official3ID");
                    gameExtValuesSB.Append(", ").Append(kvp.Key);
                }
                if (kvp.Value == "ALTERNATE")
                {
                    gameExtSB.Append(", OfficialAlternateID");
                    gameExtValuesSB.Append(", ").Append(kvp.Key);
                }
            }

            // Complete the GameExt statement
            gameExtValuesSB.Append(")\n");

            // Append both statements to the main sqlBuilder
            sqlBuilder.Append(gameInsertSB)
                      .Append(gameExtSB)
                      .Append(gameExtValuesSB);
        }
        #endregion

        #region Player
        public void CurrentPlayer(NBAdbToolboxCurrent.Game game)
        {
            // Process players from both home and away teams
            foreach (NBAdbToolboxCurrent.Team team in new[] { game.homeTeam, game.awayTeam })
            {
                // Calculate the matchup ID (opposing team)
                int MatchupID = (team == game.homeTeam) ? game.awayTeam.teamId : game.homeTeam.teamId;

                // Process each player on the team
                foreach (NBAdbToolboxCurrent.Player player in team.players)
                {
                    // Only insert player if they're not in our tracking list
                    if (!playerList.Contains((SeasonID, player.personId)))
                    {
                        // Add player to our tracking list
                        playerList.Add((SeasonID, player.personId));

                        // Build the player insert statement
                        sqlBuilder.Append("Insert into Player values(")
                                  .Append(SeasonID).Append(", ")
                                  .Append(player.personId).Append(", '")
                                  .Append(player.firstName.Replace("'", "''")).Append(" ")
                                  .Append(player.familyName.Replace("'", "''")).Append("', '")
                                  .Append(player.jerseyNum).Append("', ");

                        // Handle the position (which might be null)
                        if (player.position != null && player.position != "")
                        {
                            sqlBuilder.Append("'").Append(player.position).Append("')\n");
                        }
                        else
                        {
                            sqlBuilder.Append("null)\n");
                        }
                    }
                }
            }
        }
        #endregion

        #region PlayByPlay
        public async Task InitiateCurrentPlayByPlay(NBAdbToolboxCurrentPBP.Game game)
        {
            int i = 1;

            // Process each play-by-play action
            foreach (NBAdbToolboxCurrentPBP.Action action in game.actions)
            {
                CurrentPlayByPlay(action, Int32.Parse(game.gameId), i);
                i++;
            }
            Task DoPbp = Task.Run(async () =>
            {
                int retryAttempts = 3;
                int currentAttempt = 0;
                bool success = false;
                string inserts = playByPlayBuilder.ToString();
                playByPlayBuilder.Clear();

                while (!success && currentAttempt < retryAttempts)
                {
                    try
                    {
                        currentAttempt++;
                        using (SqlConnection connection = new SqlConnection(cString))
                        using (SqlCommand insert = new SqlCommand("set nocount on;\n" + inserts, connection))
                        {
                            insert.CommandType = CommandType.Text;
                            insert.CommandTimeout = 120; // 2 minutes
                            connection.Open();
                            insert.ExecuteNonQuery();
                            success = true;
                        }
                    }
                    catch (SqlException ex) when (ex.Number == 1205) // Deadlock victim error code
                    {
                        Console.WriteLine($"Deadlock detected (attempt {currentAttempt}/{retryAttempts}): {ex.Message}");
                        // Wait a bit before retrying to allow other transactions to complete
                        await Task.Delay(500 * currentAttempt);
                    }
                    catch (Exception e)
                    {
                        ErrorOutput(e);
                        if (currentAttempt < retryAttempts)
                        {
                            // For other exceptions, also retry but log the error
                            Console.WriteLine($"Error in DoPbp (attempt {currentAttempt}/{retryAttempts}): {e.Message}");
                            await Task.Delay(500 * currentAttempt);
                        }
                        else
                        {
                            // Final error handling for when all retries have failed
                            i = 0;
                        }
                    }
                }
            });

            if (iterator == TotalGamesCD)
            {
                await DoPbp;
            }
        }
        public void CurrentPlayByPlay(NBAdbToolboxCurrentPBP.Action action, int GameID, int iteration)
        {
            // Start building column names
            playByPlayBuilder.Append("insert into PlayByPlay(SeasonID, GameID, ActionID, ActionNumber, Qtr, QtrType, Clock, TimeActual, ScoreHome, ScoreAway, ActionType");

            // Start building values part
            StringBuilder valuesSB = new StringBuilder();
            valuesSB.Append(") values(")
                    .Append(SeasonID).Append(", ")
                    .Append(GameID).Append(", ")
                    .Append(iteration).Append(", ")
                    .Append(action.actionNumber).Append(", ")
                    .Append(action.period).Append(", '")
                    .Append(action.periodType).Append("', replace(replace(replace('")
                    .Append(action.clock).Append("', 'PT', ''), 'M', ':'), 'S', ''), '")
                    .Append(SqlDateTime.Parse(action.timeActual)).Append("', ")
                    .Append(action.scoreHome).Append(", ")
                    .Append(action.scoreAway).Append(", '")
                    .Append(action.actionType).Append("'");

            // Optional fields - Description
            if (action.description != null && action.description != "")
            {
                playByPlayBuilder.Append(", Description");
                valuesSB.Append(", '").Append(action.description.Replace("'", "''")).Append("'");
            }

            // Side
            if (action.side != null && action.side != "")
            {
                playByPlayBuilder.Append(", Side");
                valuesSB.Append(", '").Append(action.side).Append("'");
            }

            // SubType
            if (action.subType != "")
            {
                playByPlayBuilder.Append(", SubType");
                valuesSB.Append(", '").Append(action.subType).Append("'");
            }

            // Area and AreaDetail
            if (action.area != null && action.area != null)
            {
                playByPlayBuilder.Append(", Area, AreaDetail");
                valuesSB.Append(", '").Append(action.area).Append("', '")
                        .Append(action.areaDetail).Append("'");
            }

            // Coordinates
            if (action.x != null)
            {
                playByPlayBuilder.Append(", X");
                valuesSB.Append(", ").Append(action.x);
            }

            if (action.y != null)
            {
                playByPlayBuilder.Append(", Y");
                valuesSB.Append(", ").Append(action.y);
            }

            if (action.xLegacy != null)
            {
                playByPlayBuilder.Append(", XLegacy");
                valuesSB.Append(", ").Append(action.xLegacy);
            }

            if (action.yLegacy != null)
            {
                playByPlayBuilder.Append(", YLegacy");
                valuesSB.Append(", ").Append(action.yLegacy);
            }

            // Field goal information
            if (action.isFieldGoal == 1)
            {
                playByPlayBuilder.Append(", IsFieldGoal");
                valuesSB.Append(", ").Append(action.isFieldGoal);

                if (action.shotResult == "Made")
                {
                    playByPlayBuilder.Append(", ShotType, PtsGenerated");
                    valuesSB.Append(", 'FG").Append(action.actionType.Substring(0, 1)).Append("M', ")
                            .Append(action.actionType.Substring(0, 1));
                }
                else if (action.shotResult == "Missed")
                {
                    playByPlayBuilder.Append(", ShotType, PtsGenerated");
                    valuesSB.Append(", 'FG").Append(action.actionType.Substring(0, 1)).Append("A', 0");
                }

                playByPlayBuilder.Append(", ShotResult, ShotValue, ShotDistance");
                valuesSB.Append(", '").Append(action.shotResult).Append("', ")
                        .Append(action.actionType.Substring(0, 1)).Append(", ")
                        .Append(action.shotDistance);
            }
            else if (action.actionType == "Free Throw")
            {
                if (action.description.Length >= 4)
                {
                    if (action.description.Substring(action.description.Length - 4) == "PTS)")
                    {
                        playByPlayBuilder.Append(", ShotType, PtsGenerated, ShotResult, ShotValue");
                        valuesSB.Append(", 'FTM', 1, 'Made', 1");
                    }
                    else if (action.description.Length >= 4 && action.description.Substring(0, 4) == "MISS")
                    {
                        playByPlayBuilder.Append(", ShotType, PtsGenerated, ShotResult, ShotValue");
                        valuesSB.Append(", 'FTA', 0, 'Missed', 1");
                    }
                }
            }

            // Entity references
            if (action.teamId != 0 && action.teamId != null)
            {
                playByPlayBuilder.Append(", TeamID, Tricode");
                valuesSB.Append(", ").Append(action.teamId).Append(", '")
                        .Append(action.teamTricode).Append("'");
            }

            if (action.possession != 0 && action.possession != null)
            {
                playByPlayBuilder.Append(", Possession");
                valuesSB.Append(", ").Append(action.possession);
            }

            if (action.officialId != null)
            {
                playByPlayBuilder.Append(", OfficialID");
                valuesSB.Append(", ").Append(action.officialId);
            }

            if (action.personId != 0)
            {
                playByPlayBuilder.Append(", PlayerID");
                valuesSB.Append(", ").Append(action.personId);
            }

            if (action.assistPersonId != null)
            {
                playByPlayBuilder.Append(", PlayerIDAst");
                valuesSB.Append(", ").Append(action.assistPersonId);
            }

            if (action.blockPersonId != null)
            {
                playByPlayBuilder.Append(", PlayerIDBlk");
                valuesSB.Append(", ").Append(action.blockPersonId);
            }

            if (action.stealPersonId != null)
            {
                playByPlayBuilder.Append(", PlayerIDStl");
                valuesSB.Append(", ").Append(action.stealPersonId);
            }

            if (action.jumpBallLostPersonId != null)
            {
                playByPlayBuilder.Append(", PlayerIDJumpL");
                valuesSB.Append(", ").Append(action.jumpBallLostPersonId);
            }

            if (action.jumpBallWonPersonId != null)
            {
                playByPlayBuilder.Append(", PlayerIDJumpW");
                valuesSB.Append(", ").Append(action.jumpBallWonPersonId);
            }

            if (action.foulDrawnPersonId != null)
            {
                playByPlayBuilder.Append(", PlayerIDFoulDrawn");
                valuesSB.Append(", ").Append(action.foulDrawnPersonId);
            }

            // Qualifiers
            int q = 1;
            foreach (object qual in action.qualifiers)
            {
                if (qual != null && q < 4)
                {
                    playByPlayBuilder.Append(", Qual").Append(q);
                    valuesSB.Append(", '").Append(qual).Append("'");
                }
                q++;
            }

            // Descriptor
            if (action.descriptor != null)
            {
                playByPlayBuilder.Append(", Descriptor");
                valuesSB.Append(", '").Append(action.descriptor).Append("'");
            }

            // Shot action number
            if (action.shotActionNumber != null)
            {
                playByPlayBuilder.Append(", ShotActionNbr");
                valuesSB.Append(", ").Append(action.shotActionNumber);
            }

            // Complete the SQL statement
            valuesSB.Append(")\n");

            // Append values to the main builder
            playByPlayBuilder.Append(valuesSB);
        }
        #endregion

        #endregion

        //If Current Data is Missing, use Historic Data
        #region Missing Data Inserts

        public void MissingDataGPS(bool box, bool pbp)
        {
            string missingInstructions = "";
            if (box)
            {
                missingInstructions += "Box";
            }
            if (pbp)
            {
                missingInstructions += " PlayByPlay";
            }
            if (box || pbp)
            {
                NBAdbToolboxHistoric.Game game = null;
                if (GameID.ToString().Substring(0, 1) == "2")
                {
                    game = root.season.games.regularSeason.FirstOrDefault(g => Int32.Parse(g.game_id) == GameID);
                }
                else
                {
                    game = root.season.games.playoffs.FirstOrDefault(g => Int32.Parse(g.game_id) == GameID);
                }
                GetMissingDataDetails(game, missingInstructions);
            }
        }
        public void GetMissingDataDetails(NBAdbToolboxHistoric.Game game, string sender)
        {

            string allInOne = "";
            SQLdb = new SqlConnection(cString);

            //Check Db and build strings for Inserts & Updates
            #region Check Db and build strings for Inserts & Updates
            if (sender.Contains("Box"))
            {
                //Teams
                TeamStaging(game);
                //Arenas
                if (!arenaList.Contains((SeasonID, game.box.arena.arenaId)))
                {
                    HistoricArenaInsert(game.box.arena, game.box.homeTeamId, "Missing");
                }
                //Officials
                List<int> officials = new List<int>();
                foreach (NBAdbToolboxHistoric.Official official in game.box.officials)
                {
                    if (!officialList.Contains((SeasonID, official.personId)))
                    {
                        HistoricOfficialInsert(official, "Missing");
                    }
                    officials.Add(official.personId);
                }
                //Games
                string gameType = "";
                if (GameID.ToString().Substring(0) == "2" || GameID.ToString().Substring(0) == "6")
                {
                    gameType = "Regular Season";
                }
                else if (GameID.ToString().Substring(0) == "4" || GameID.ToString().Substring(0) == "5")
                {
                    gameType = "Postseason";
                }
                HistoricGameInsert(game, gameType, officials); //5.7 Populate DB Update
                //TeamBox
                HistoricTeamBoxStaging(game);
                //Players
                HistoricPlayerStaging(game);
                string sqlBuilderStr = sqlBuilder.ToString() + "\n";
                sqlBuilder.Clear();

                allInOne = sqlBuilderStr;
            }
            if (sender.Contains("PlayByPlay"))
            {
                HistoricPlayByPlayStaging(game.playByPlay);
                allInOne += playByPlayInsertString;
            }
            #endregion
            string hitDb = missingData + allInOne;
            missingData = "";
            allInOne = "";
            SqlConnection bigInserts = new SqlConnection(cString);
            try
            {
                using (bigInserts)
                {
                    using (SqlCommand AllInOneInsert = new SqlCommand(hitDb))
                    {
                        AllInOneInsert.Connection = bigInserts;
                        AllInOneInsert.CommandType = CommandType.Text;
                        bigInserts.Open();
                        AllInOneInsert.ExecuteNonQuery();
                        bigInserts.Close();
                    }
                }
            }
            catch (Exception e)
            {
                ErrorOutput(e);
            }
        }
        #endregion










    }
}
