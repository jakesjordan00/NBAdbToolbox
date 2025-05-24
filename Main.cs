using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;
using System.Diagnostics;
using static NBAdbToolbox.Main;
using System.Data.SqlTypes;
using System.Drawing.Drawing2D;
using System.Globalization;
using System.Runtime.ConstrainedExecution;
using NBAdbToolboxHistoric;
using NBAdbToolboxCurrent;
using NBAdbToolboxCurrentPBP;
using NBAdbToolboxSchedule;
using System.Media;
using System.Runtime.InteropServices;
using System.Collections;
using System.Runtime;

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

        string settingsPath = Path.Combine(projectRoot, @"Content", "settings.json");
        private Settings settings;

        string configPath = Path.Combine(projectRoot, @"Content\Configuration", "dbconfig.json"); //dbconfig file
        private DbConfig config;        


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
            public Label lblCStatus = new Label();     //Connection string status
            public Label lblDB = new Label();          //Database
            public Label lblDbName = new Label();      //Database name
            public Label lblDbStat = new Label();      //Need to create database/Database created label
            public Button btnEdit = new Button();      //Edit config file
            public Button btnBuild = new Button();     //Build Database
            public PictureBox picStatus = new PictureBox();//Connection string icon
            public PictureBox picDbStatus = new PictureBox();//Db Status icon


        public Label lblSettings = new Label();
        public PictureBox picSettings = new PictureBox();//Db Status icon
        public FolderBrowserDialog dlgDefualtPath = new FolderBrowserDialog();
        #endregion
        #region Variables
        #endregion


        #endregion




        #region pnlDbUtil - Database Utilities
        public Panel pnlDbUtil = new Panel();
        public Panel pnlDbOverview = new Panel();
            #region Labels and Controls
            public Label lblDbUtil = new Label {Text = "Database Utilities"};
            public Label lblDbOverview = new Label();
            public Label lblDbOvExpand = new Label();
            public Label lblDbOptions = new Label();
            public Label lblDbSelectSeason = new Label();
            public ListBox listSeasons = new ListBox();
            public Button btnPopulate = new Button();
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

        public HashSet<(int SeasonID, int Games)> seasonGames = new HashSet<(int, int)>
        {
            (2012, 1314),
            (2013, 1319),
            (2014, 1311),
            (2015, 1316),
            (2016, 1309),
            (2017, 1311),
            (2018, 1312),
            (2019, 1142),
            (2020, 1171),
            (2021, 1323),
            (2022, 1320),
            (2023, 1318),
            (2024, 1230)
        };
        public HashSet<(int, (int, int, int, int, int))> seasonInfo = new HashSet<(int, (int, int, int, int, int))>();




        public int currentImageIterator = 0;
        public bool currentReverse = false;
        public string currentBoxUpdate = "";




        public string json = "";
        PictureBox bgCourt = new PictureBox //Create Background image
        {
            SizeMode = PictureBoxSizeMode.StretchImage,
            Dock = DockStyle.Fill
        };

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

        public string settingsJSON = "";


        //Settings
        public void GetConfig()
        {

        }
        public void WriteConfig()
        {
            string name = "";
            string db = "";
            if(config.Database != null)
            {
                db = " - " + config.Database;
            }
            if (config.Alias != null)
            {
                name = config.Alias + db + ".json";
            }
            else
            {
                name = config.Server + db + ".json";
            }
            if (config.Default == true)
            {
                settings.DefaultConfig = name;
            }
            File.WriteAllText(Path.Combine(settings.ConfigPath, name), JsonConvert.SerializeObject(config, Formatting.Indented));
            configPath = Path.Combine(settings.ConfigPath, name);
            File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));

        }
        public void GetSettings()
        {
            if (File.Exists(settingsPath)) //If our file does exist
            {
                settingsJSON = "";
                settingsJSON = File.ReadAllText(settingsPath);
                settings = JsonConvert.DeserializeObject<Settings>(settingsJSON);
                DefaultSettings();
                //Set Background Image
                bgCourt.Image = Image.FromFile(Path.Combine(projectRoot, @"Content\Images", settings.BackgroundImage + ".png"));
                //Set Default dbconfig.json filepath
                if(File.Exists(Path.Combine(settings.ConfigPath, settings.DefaultConfig)))
                {
                    configPath = Path.Combine(settings.ConfigPath, settings.DefaultConfig);
                }
            }
            else if (!File.Exists(settingsPath)) //If our file doesnt exist, just set to defaults
            {
                DefaultSettings();
            }
            File.WriteAllText(settingsPath, JsonConvert.SerializeObject(settings, Formatting.Indented));
        }
        public void DefaultSettings()
        {
            if (settings.ConfigPath == "")
            {
                settings.ConfigPath = Path.Combine(projectRoot, @"Content\Configuration");
            }
            if (settings.DefaultConfig == "")
            {
                settings.DefaultConfig = "dbconfig.json";
            }
            if (settings.BackgroundImage == "")
            {
                settings.BackgroundImage = "Court Default";
            }
            if (settings.WindowSize == "")
            {
                settings.WindowSize = "Default";
            }
            if (settings.Sound == "")
            {
                settings.Sound = "Default";
            }

        }



        public Main()
        {
            InitializeComponent();
            GetSettings();
            //Set screen size
            #region Set screen size
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
            this.ResizeEnd += FormResize;
            #endregion

            #region Set and declare variables
            //Declarations       
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


            //Check for dbconfig - Verify Server/Database Connectivity
            #region Verify Server/Database Connectivity
            InitializeDbConfig();
            #endregion

            #region Add Elements - Adding Labels/Panels etc to Window
            //This should be second to last i believe.
            InitializeElements();
            #endregion




            #region dbUtilProperties
            //DbUtil
            pnlDbUtil.BorderStyle = BorderStyle.None;
            pnlDbUtil.Paint += (s, e) =>
            {
                Control p = (Control)s;
                using (Pen pen = new Pen(Color.White, 1))
                {
                    e.Graphics.DrawLine(pen, p.Width - 1, 0, p.Width - 1, p.Height);
                }
            };
            pnlDbUtil.Parent = bgCourt;
            lblDbUtil.Height = (int)(pnlWelcome.Height * .1);
            float fontSize = ((float)(pnlWelcome.Height * .08) / (96 / 12)) * (72 / 12);
            lblDbUtil.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, pnlDbUtil, lblDbUtil);
            lblDbUtil.AutoSize = true;
            CenterElement(pnlDbUtil, lblDbUtil);

            //Will be used primarily for table panel expansion later, but need for DbOptions
            fullHeight = (int)(pnlDbUtil.Height * .5);
            dimW = pnlDbUtil.Width / 3;
            dimH = (int)(fullHeight * .25);
            dimH2 = (int)(fullHeight * .5);


            ChangeLabel(lblDbOverview, pnlDbUtil, new List<string> { 
                "Database Overview", 
                "Bold", 
                (((float)(pnlWelcome.Height * .05) / (96 / 12)) * (72 / 12)).ToString(), 
                ".", 
                "true", 
                pnlDbUtil.Left.ToString(), 
                (lblDbUtil.Bottom + (int)(lblDbUtil.Height * .3)).ToString(), 
                ".", 
                "true", 
                ((int)(lblDbUtil.Height * .8)).ToString() 
            });           
            ChangeLabel(lblDbOvExpand, pnlDbUtil, new List<string> { 
                "+", 
                "Bold", 
                (((float)(pnlWelcome.Height * .05) / (96 / 12)) * (72 / 12)).ToString(), 
                ".", 
                "true", 
                ".", 
                lblDbOverview.Top.ToString(), 
                ".", 
                "true", 
                ((int)(lblDbUtil.Height * .8)).ToString() 
            });
            lblDbOvExpand.Left = (pnlDbUtil.Left + pnlDbUtil.Width - lblDbOvExpand.Width);
            lblDbOvExpand.Height = lblDbOverview.Height;
            overviewHeight = lblDbOverview.Height;

            ChangeLabel(lblDbOptions, pnlDbUtil, new List<string> { 
                "Options", 
                "Bold", 
                fontSize.ToString(), 
                ".", 
                "true", 
                ".", 
                (lblDbOverview.Bottom + lblDbUtil.Height).ToString(), 
                ".", 
                "true", 
                ((int)(lblDbUtil.Height * .9)).ToString() 
            });
            CenterElement(pnlDbUtil, lblDbOptions);


            ChangeLabel(lblDbSelectSeason, lblDbOptions, new List<string> { 
                "Season Select", 
                "Bold", 
                fontSize.ToString(), 
                ".", 
                "true", 
                pnlDbUtil.Left.ToString(), 
                lblDbOptions.Bottom.ToString(), 
                ".", 
                "true", 
                ((int)(lblDbUtil.Height * .8)).ToString() 
            });
            AlignLeft(pnlDbUtil, lblDbSelectSeason, lblDbOptions);


            listSeasons.SelectionMode = SelectionMode.MultiExtended;
            listSeasons.Top = lblDbSelectSeason.Bottom;
            listSeasons.Left = pnlDbUtil.Left; 
            listSeasons.KeyDown += ListSeasons_SelectAll;

            btnPopulate.Text = "Populate Db";
            btnPopulate.Font = SetFontSize("Segoe UI", (float)(fontSize / 3.3), FontStyle.Bold, pnlWelcome, btnPopulate); //6.5
            btnPopulate.Width = (int)(listSeasons.Width * .8);
            btnPopulate.Top = listSeasons.Bottom; //subject to change
            #endregion
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
                var popup = new PopulatePopup(dialog);
                if (popup.ShowDialog() == DialogResult.OK)
                {
                    //Task.Run(() => LoadImages());

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
                    int historic = 0;
                    int current = 0;
                    string source = "";
                    gpm.Visible = true;
                    gpmValue.Visible = true;
                    lblCurrentGameCount.Visible = true;
                    lblSeasonStatusLoadInfo.Visible = true;
                    picLoad.Visible = true;
                    #region ChangeLabel
                    ChangeLabel(lblCurrentGame, pnlLoad, new List<string> {
                        "Current game: ", //Text
                        "Regular", //FontStyle
                        ((float)(pnlLoad.Height * .05) / (96 / 12) * (72 / 12)).ToString(), //FontSize
                        ".", //Width
                        "true", //AutoSize
                        "4", //Left
                        ".", //Top
                        Color.Black.ToString(), //Color
                        "true", //Visible
                        "." //Height
                    }); //Current game: 
                    gpm.Top = lblCurrentGame.Bottom;
                    gpmValue.Top = gpm.Bottom;
                    ChangeLabel(lblSeasonStatusLoad, pnlLoad, new List<string> {
                        "Checking util.BuildLog", //Text
                        "Bold", //FontStyle
                        ((float)(pnlLoad.Height * .08) / (96 / 12) * (72 / 12)).ToString(), //FontSize
                        ".", //Width
                        "true", //AutoSize
                        "0", //Left
                        ".", //Top
                        Color.Black.ToString(), //Color
                        "true", //Visible
                        "." //Height
                    }); //Currently Loading: 
                    ChangeLabel(lblWorkingOn, pnlLoad, new List<string> {
                        ".", //Text
                        "Regular", //FontStyle
                        ((float)(pnlLoad.Height * .03) / (96 / 12) * (72 / 12)).ToString(), //FontSize
                        ".", //Width
                        "true", //AutoSize
                        (pnlLoad.Width - lblWorkingOn.Width).ToString(), //Left
                        lblSeasonStatusLoadInfo.Top.ToString(), //Top
                        Color.Black.ToString(), //Color
                        "true", //Visible
                        "." //Height
                    }); //No text yet
                    #endregion

                    #endregion
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
                            //if (root == null)
                            //{
                            //    await Task.Run(async () =>      //This sets the root variable to our big file
                            //    {
                            //        await ReadSeasonFile(popup.historic, popup.current);
                            //    });
                            //}
                            PopulateDb_2_AfterHistoricRead();
                            await DeleteSeasonData;
                            PopulateDb_3_AfterDelete_BeforeGames();
                            foreach (NBAdbToolboxHistoric.Game game in root.season.games.regularSeason)
                            {
                                sqlBuilder.Clear();
                                playByPlayBuilder.Clear();
                                GameID = Int32.Parse(game.game_id);
                                lblCurrentGameCount.Text = game.game_id;
                                await Task.Run(async () =>      //This inserts the games from season file into db
                                {
                                    await InsertGameWithLoading(game, season, imageIteration, "Regular Season");
                                });
                                //After Insert is complete, use multithreading to insert the bulk of our data - PlayByPlay, PlayerBox (+StartingLineups) and TeamBox (+TeamBoxLineups)
                                #region Second Insert

                                Task SecondInsert = Task.Run(async () =>
                                {
                                    int retryAttempts = 3;
                                    int currentAttempt = 0;
                                    bool success = false;

                                    while (!success && currentAttempt < retryAttempts)
                                    {
                                        try
                                        {
                                            currentAttempt++;
                                            // Store the query before clearing the StringBuilder
                                            string pbpInsert = playByPlayBuilder.ToString();
                                            playByPlayBuilder.Clear();

                                            using SqlConnection bigInsertsPBP = new SqlConnection(cString);
                                            using SqlCommand PBPInsert = new SqlCommand(pbpInsert, bigInsertsPBP);
                                            PBPInsert.CommandType = CommandType.Text;

                                            // Set a longer command timeout to give the operation more time
                                            PBPInsert.CommandTimeout = 120; // 2 minutes

                                            await bigInsertsPBP.OpenAsync();
                                            await PBPInsert.ExecuteNonQueryAsync();
                                            success = true;
                                        }
                                        catch (SqlException ex) when (ex.Number == 1205) // Deadlock victim error code
                                        {
                                            Console.WriteLine($"Deadlock detected (attempt {currentAttempt}/{retryAttempts}): {ex.Message}");

                                            // Wait a bit before retrying to allow other transactions to complete
                                            await Task.Delay(500 * currentAttempt);
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine($"Error in SecondInsert: {e.Message}");
                                            break; // Don't retry on other errors
                                        }
                                    }
                                });
                                lastSeason = season;

                                UpdateLoadingImage(imageIteration);
                                #endregion
                                PopulateDb_4_AfterHistoricGame();
                                game.box = null;
                                game.playByPlay = null;
                            }

                            root.season.games.regularSeason = null;
                            lblSeasonStatusLoad.Text = "Inserting " + SeasonID + " Postseason";

                            foreach (NBAdbToolboxHistoric.Game game in root.season.games.playoffs)
                            {
                                sqlBuilder.Clear();
                                playByPlayBuilder.Clear();
                                GameID = Int32.Parse(game.game_id);
                                lblCurrentGameCount.Text = game.game_id;
                                await Task.Run(async () =>      //This inserts the games from season file into db
                                {
                                    await InsertGameWithLoading(game, season, imageIteration, "Postseason");
                                });
                                #region Second Insert
                                Task SecondInsert = Task.Run(async () =>
                                {
                                    int retryAttempts = 3;
                                    int currentAttempt = 0;
                                    bool success = false;

                                    while (!success && currentAttempt < retryAttempts)
                                    {
                                        try
                                        {
                                            currentAttempt++;
                                            // Store the query before clearing the StringBuilder
                                            string pbpInsert = playByPlayBuilder.ToString();
                                            playByPlayBuilder.Clear();

                                            using SqlConnection bigInsertsPBP = new SqlConnection(cString);
                                            using SqlCommand PBPInsert = new SqlCommand(pbpInsert, bigInsertsPBP);
                                            PBPInsert.CommandType = CommandType.Text;

                                            // Set a longer command timeout to give the operation more time
                                            PBPInsert.CommandTimeout = 120; // 2 minutes

                                            await bigInsertsPBP.OpenAsync();
                                            await PBPInsert.ExecuteNonQueryAsync();
                                            success = true;
                                        }
                                        catch (SqlException ex) when (ex.Number == 1205) // Deadlock victim error code
                                        {
                                            Console.WriteLine($"Deadlock detected (attempt {currentAttempt}/{retryAttempts}): {ex.Message}");

                                            // Wait a bit before retrying to allow other transactions to complete
                                            await Task.Delay(500 * currentAttempt);
                                        }
                                        catch (Exception e)
                                        {
                                            Console.WriteLine($"Error in SecondInsert: {e.Message}");
                                            break; // Don't retry on other errors
                                        }
                                    }
                                });

                                UpdateLoadingImage(imageIteration);

                                #endregion
                                PopulateDb_4_AfterHistoricGame();
                                game.box = null;
                                game.playByPlay = null;
                                if(iterator == TotalGames)
                                {
                                    await SecondInsert;
                                }
                            }
                            root.season.games.playoffs = null;
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
                                root.season.games.regularSeason[i].box = null;
                                root.season.games.regularSeason[i].playByPlay = null;
                                PopulateDb_8_AfterCurrentGame(gamesRS[i].ToString());
                            }
                            for (int i = 0; i < PostseasonGames; i++)
                            {
                                GameID = gamesPS[i];
                                lblCurrentGameCount.Text = gamesPS[i].ToString();
                                await CurrentGameGPS(gamesPS[i], "");
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

                }
            };
            #endregion
            //Panel Formatting
            #region Panel Formatting
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



            //lblSeasonStatusLoadInfo
            lblSeasonStatusLoad.Left = 0;
            fontSize = ((float)(pnlLoad.Height * .08) / (96 / 12)) * (72 / 12);
            lblSeasonStatusLoad.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, pnlLoad, lblSeasonStatusLoad);
            lblSeasonStatusLoad.AutoSize = true;
            lblSeasonStatusLoadInfo.Left = lblSeasonStatusLoad.Right - (int)(pnlLoad.Width * .01);
            lblSeasonStatusLoadInfo.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, pnlLoad, lblSeasonStatusLoadInfo);
            lblSeasonStatusLoadInfo.AutoSize = true;



            lblCurrentGame.Left = 4;
            fontSize = ((float)(pnlLoad.Height * .05) / (96 / 12)) * (72 / 12);
            lblCurrentGame.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, pnlLoad, lblCurrentGame);
            lblCurrentGame.AutoSize = true;
            lblCurrentGameCount.Left = lblCurrentGame.Right - (int)(pnlLoad.Width * .02);
            lblCurrentGameCount.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, pnlLoad, lblCurrentGameCount);
            lblCurrentGameCount.AutoSize = true;
            lblCurrentGame.Top = lblSeasonStatusLoad.Bottom;
            lblCurrentGameCount.Top = lblSeasonStatusLoad.Bottom;

            fontSize = ((float)(pnlLoad.Height * .045) / (96 / 12)) * (72 / 12);
            gpm.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, pnlLoad, gpm);
            gpm.AutoSize = true;
            gpm.Top = lblCurrentGame.Bottom;
            gpm.Left = 4;
            fontSize = ((float)(pnlLoad.Height * .05) / (96 / 12)) * (72 / 12);
            gpmValue.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, pnlLoad, gpmValue);
            gpmValue.AutoSize = true;
            gpmValue.Top = gpm.Bottom;
            gpmValue.Left = 4;
            gpmValue.ForeColor = Color.White;



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


            //Scoreboard
            pnlScoreboard.Height = this.Height / 18;
            pnlScoreboard.Width = this.Width - pnlDbUtil.Width;
            pnlScoreboard.Parent = bgCourt;
            pnlScoreboard.Top = pnlNav.Bottom;
            pnlScoreboard.Left = pnlDbUtil.Right;



            //Set Label Properties ***************************************************************************

            //To set font, i'll need the name, ideal size or pt, and its Style.
            //In addition, i also need the parent element and the child or the element we're working with
            lblStatus.Height = (int)(pnlWelcome.Height * .1);
            fontSize = ((float)(lblStatus.Height) / (96 / 12)) * (72 / 12); //Formula is picking the correct Pt, as determined by the height of the label
            lblStatus.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, pnlWelcome, lblStatus);
            //Auto-size and center
            CenterElement(pnlWelcome, lblStatus);





            //Server label properties
            lblServer.Left = 5;
            lblServer.Top = lblStatus.Bottom + 10;
            lblServer.Height = (int)(pnlWelcome.Height * .067);
            fontSize = ((float)((pnlWelcome.Height * .05)) / (96 / 12)) * (72 / 12);
            lblServer.AutoSize = true;
            //lblServer.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, pnlWelcome, lblServer);

            lblServer.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
            lblServerName.Left = lblServer.Right - 10;
            lblServerName.Height = (int)(pnlWelcome.Height * .067);
            lblServerName.Top = lblStatus.Bottom + 10;
            lblServerName.Height = lblServer.Height;
            lblServerName.AutoSize = true;
            lblServerName.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);

            //Database label Properties
            lblDB.Left = 5;
            lblDB.Top = lblServer.Bottom;
            lblDB.Height = (int)(pnlWelcome.Height * .06);
            lblDB.AutoSize = true;
            fontSize = ((float)((pnlWelcome.Height * .04)) / (96 / 12)) * (72 / 12);
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
            lblDbStat.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, pnlWelcome, lblDbStat);





            //Edit Button properties
            btnEdit.Width = lblStatus.Width / 2;
            if (File.Exists(configPath)) //If our file exists, set width proper
            {
                btnEdit.Width = (int)(lblStatus.Width / 1.5);
            }
            btnEdit.Height = 30;
            fontSize = ((float)(lblDB.Height) / (96 / 12)) * (72 / 12);
            btnEdit.Font = SetFontSize("Segoe UI", (float)(fontSize * .67), FontStyle.Bold, pnlWelcome, btnEdit); //12F
            CenterElement(pnlWelcome, btnEdit);
            btnEdit.Top = lblDbStat.Bottom + 10; //subject to change
            btnEdit.TextAlign = ContentAlignment.BottomCenter;





            fontSize = ((float)(lblServer.Height) / (96 / 12)) * (72 / 12) / 2;
            lblCStatus.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, pnlWelcome, lblCStatus);
            lblCStatus.Height = lblServer.Height / 2;
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
            btnBuild.Font = SetFontSize("Segoe UI", (float)(fontSize * 1.3), FontStyle.Bold, pnlWelcome, btnBuild);
            btnBuild.AutoSize = true;
            CenterElement(pnlWelcome, btnBuild);
            btnBuild.Top = btnEdit.Bottom + 10; //subject to change
            #endregion
            #region Edit Connection & Build DB
            if (dbConnection)
            {
                GetSeasons(bob.ToString());
            }

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
                    WriteConfig();
                    RefreshConnectionUI();
                }
            };

            btnBuild.Click += (s, e) =>
            {
                if (config.Create == true)
                {
                    CreateDB(cString);
                    bob.InitialCatalog = config.Database;
                    cString = bob.ToString();
                }
            };
            #endregion


            #region Database Overview
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


            DbOverviewClick(pnlDbOverview, lblDbOvExpand, pnlDbOverview);
            DbOverviewClick(lblDbOverview, lblDbOvExpand, pnlDbOverview);
            DbOverviewClick(lblDbOvExpand, lblDbOvExpand, pnlDbOverview);
            #endregion

            #region Settings area
            lblSettings.Text = "Settings";
            lblSettings.Top = btnBuild.Bottom + 10; //subject to change
            lblSettings.Font = SetFontSize("Segoe UI", (float)(fontSize), FontStyle.Bold, pnlWelcome, btnBuild);
            lblSettings.AutoSize = true;
            lblSettings.Left = (pnlWelcome.ClientSize.Width - lblSettings.Width) / 2;
            picSettings.Width = lblSettings.Height;
            picSettings.Height = lblSettings.Height;
            picSettings.SizeMode = PictureBoxSizeMode.Zoom;
            picSettings.Image = Image.FromFile(Path.Combine(projectRoot, @"Content\Images", "Settings.png"));
            picSettings.Left = lblSettings.Right;
            picSettings.Top = lblSettings.Top + lblSettings.Height/6;
            picSettings.BackColor = Color.FromArgb(0, 0, 0, 0);

            SettingsClick(lblSettings, picSettings, fontSize);
            SettingsClick(picSettings, picSettings, fontSize);
            #endregion
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
            btnPopulate.Enabled = false;
            btnEdit.Enabled = false;
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
            ChangeLabel(lblSeasonStatusLoad, pnlLoad, new List<string> {
            "Deleting any " + SeasonID + " data, one sec...", //Text
            "Bold", //FontStyle
            ((float)(pnlLoad.Height * .075) / (96 / 12) * (72 / 12)).ToString(), //FontSize
            ".", //Width
            "true", //AutoSize
            "0", //Left
            ".", //Top
            Color.Black.ToString(), //Color
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
        public void PopulateDb_4_AfterHistoricGame()
        {
            ImageDriver();
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
            ChangeLabel(lblSeasonStatusLoad, pnlLoad, new List<string> {
                            "Deleting any " + SeasonID + " data, one sec...", //Text
                            "Bold", //FontStyle
                            ((float)(pnlLoad.Height * .075) / (96 / 12) * (72 / 12)).ToString(), //FontSize
                            ".", //Width
                            "true", //AutoSize
                            "0", //Left
                            ".", //Top
                            Color.Black.ToString(), //Color
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
                if(game.GameId.Substring(2, 1) == "2")
                {
                    RegularSeasonGames++;
                    GameID = Int32.Parse(game.GameId);
                    gamesRS.Add(GameID);
                }
                else if(game.GameId.Substring(2, 1) != "1" && game.GameId.Substring(2, 1) != "3" && game.GameId.Substring(2, 1) != "6")
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
            ChangeLabel(lblSeasonStatusLoad, pnlLoad, new List<string> {
                            "Deleting any " + SeasonID + " data, one sec...", //Text
                            "Bold", //FontStyle
                            ((float)(pnlLoad.Height * .075) / (96 / 12) * (72 / 12)).ToString(), //FontSize
                            ".", //Width
                            "true", //AutoSize
                            "0", //Left
                            ".", //Top
                            Color.Black.ToString(), //Color
                            "true", //Visible
                            "." //Height
                            }); //Hitting endpoints and inserting 
        }
        public void PopulateDb_7_AfterCurrentDelete()
        {
            stopwatchInsert.Restart();
            lblStatus.Text = "Loading " + SeasonID + "...";
            CenterElement(pnlWelcome, lblStatus);
            ChangeLabel(lblSeasonStatusLoad, pnlLoad, new List<string> {
            "Hitting endpoints and inserting " + SeasonID + " data", //Text
            "Bold", //FontStyle
            ((float)(pnlLoad.Height * .075) / (96 / 12) * (72 / 12)).ToString(), //FontSize
            ".", //Width
            "true", //AutoSize
            "0", //Left
            ".", //Top
            Color.Black.ToString(), //Color
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
            using(SqlConnection Main = new SqlConnection(cString))
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

            #endregion
            #region Completion Message
            completionMessage += elapsedStringSeason + ". ";
            completionMessage += iterator + " games, " + RegularSeasonGames + "/" + PostseasonGames + "\n";
            int lblLeft = lblWorkingOn.Width;
            ChangeLabel(lblWorkingOn, pnlLoad, new List<string>
                        {
             /*Text*/       completionMessage,
             /*FontStyle*/  "Regular",
             /*FontSize*/   ((float)(pnlLoad.Height * .03) / (96 / 12) * (72 / 12)).ToString(),
             /*Width*/      ".",
             /*Autosize*/   "true",
             /*Left*/       (pnlLoad.Width - lblWorkingOn.Width).ToString(),
             /*Top*/        lblSeasonStatusLoadInfo.Bottom.ToString(),
             /*Color*/      ".",
             /*Visible*/    "true",
             /*Height*/     "."
                        });
            lblWorkingOn.Left = pnlLoad.Width - lblWorkingOn.Width;
            #endregion

            if (seasonIterator != selectedSeasons)
            {
                PlayCompletionSound("Season");
            }
            currentTeamsDone = false;
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
            PlayCompletionSound("Run");
            #region Enable buttons and clear label text
            btnPopulate.Enabled = true;
            btnEdit.Enabled = true;
            listSeasons.Enabled = true;

            lblStatus.Text = "Welcome Back!";
            CenterElement(pnlWelcome, lblStatus);

            //lblCurrentGame

            ChangeLabel(lblSeasonStatusLoadInfo, pnlLoad, new List<string> { "", ".", ".", ".", ".", ".", ".", ".", "false", "." });
            //...............................................................Text,  FontStyle, FontSize, Width, AutoSize, Left, Top, Color, Visible, Height
            ChangeLabel(lblCurrentGameCount, pnlLoad, new List<string> { "", ".", ".", ".", ".", ".", ".", ".", "false", "." });
            //...........................................................Text,  FontStyle, FontSize, Width, AutoSize, Left, Top, Color, Visible, Height
            ChangeLabel(lblCurrentGame, pnlLoad, new List<string> {
                        "Full Load: " + elapsedStringFull,
                        "Bold",
                        ((float)(pnlLoad.Height * .06) / (96 / 12) * (72 / 12)).ToString(),
                        ".",
                        "true",
                        ".",
                        ".",
                        Color.Green.ToString(),
                        "true",
                        "." }
            );//Done! Check your SQL db
            gpm.Top = lblCurrentGame.Bottom;
            gpmValue.Top = gpm.Bottom;
            ChangeLabel(lblSeasonStatusLoad, pnlLoad, new List<string> {
                        "Done! Check your SQL db",
                        "Regular",
                        ((float)(pnlLoad.Height * .08) / (96 / 12) * (72 / 12)).ToString(),
                        ".",
                        "true",
                        ".",
                        ".",
                        Color.Green.ToString(),
                        "true",
                        "." }
            );//Done! Check your SQL db


            ChangeLabel(lblWorkingOn, pnlLoad, new List<string> {
                        ".", //Text
                        "Bold", //FontStyle
                        ((float)(pnlLoad.Height * .04) / (96 / 12) * (72 / 12)).ToString(), //FontSize
                        ".", //Width
                        "true", //AutoSize
                        ".", //Left
                        "0", //Top
                        ".", //Color
                        "true",//Visible
                        "." } //Height
            );//No text
            lblWorkingOn.Left = pnlLoad.Width - lblWorkingOn.Width;
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
        public void InitializeElements()
        {
            //Children elements should go above the parents, background image should be last added. 
            AddPanelElement(pnlLoad, gpmValue);
            AddPanelElement(pnlLoad, gpm);
            AddPanelElement(pnlLoad, lblWorkingOn);
            AddPanelElement(pnlLoad, lblCurrentGameCount);
            AddPanelElement(pnlLoad, lblCurrentGame);
            AddPanelElement(pnlLoad, lblSeasonStatusLoadInfo);
            AddPanelElement(pnlLoad, lblSeasonStatusLoad);
            AddPanelElement(pnlLoad, picLoad);
            AddPanelElement(pnlDbUtil, btnPopulate);
            AddPanelElement(pnlDbUtil, listSeasons);
            AddPanelElement(pnlDbUtil, lblDbSelectSeason);
            AddPanelElement(pnlDbUtil, lblDbOptions);
            AddPanelElement(pnlDbUtil, lblDbOvExpand);
            AddPanelElement(pnlDbUtil, lblDbOverview);
            AddPanelElement(pnlDbUtil, pnlDbOverview);
            AddPanelElement(pnlDbUtil, lblDbUtil);
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
        public void InitializeDbConfig()
        {
            if (!File.Exists(configPath)) //If our file doesnt exist
            {
                lblStatus.Text = "Set Database Configuration";
                btnEdit.Text = "Create Server connection";
                //lblServer.Text += "Not Connected.";
                lblServerName.Text = "Not Connected.";
                lblServerName.ForeColor = Color.Red;
                lblCStatus.Text = "Disconnected";
                lblCStatus.ForeColor = Color.Red;
                // Load image
                imagePath = Path.Combine(projectRoot, @"Content\Images", "Error.png");
                picStatus.Image = Image.FromFile(imagePath);
                btnBuild.Enabled = false;
                btnPopulate.Enabled = false;
            }
            else if (File.Exists(configPath)) //If our file does exist
            {
                json = "";
                lblStatus.Text = "Welcome Back!";
                btnEdit.Text = "Edit Server connection";
                btnEdit.Width = (int)(lblStatus.Width / 1.5);
                json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<DbConfig>(json);
                //Set label text
                if(config.Alias != null)
                {
                    lblServerName.Text = config.Alias;
                }
                else
                {
                    lblServerName.Text = config.Server;
                }
                lblDbName.Text = config.Database;

                //Build connection string
                bob.DataSource = config.Server;
                if (config.Create == false)
                {
                    bob.InitialCatalog = config.Database;
                }
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
                    isConnected = TestDbConnection(cString);
                }
                if (picStatus.Image != null)
                {
                    picStatus.Image.Dispose();
                    picStatus.Image = null;
                }
                if (isConnected)
                {
                    lblCStatus.Text = "Connected";
                    lblCStatus.ForeColor = Color.Green;
                    // Load image
                    imagePath = Path.Combine(projectRoot, @"Content\Images", "Success.png");
                    picStatus.Image = Image.FromFile(imagePath);
                    btnBuild.Enabled = true;
                }
                else
                {
                    lblCStatus.Text = "Disconnected";
                    lblCStatus.ForeColor = Color.Red;
                    // Load image
                    imagePath = Path.Combine(projectRoot, @"Content\Images", "Error.png");
                    picStatus.Image = Image.FromFile(imagePath);
                    btnBuild.Enabled = false;
                }
                CheckServer(cString, "main");
            }

        }

        #endregion

        #region Database and Server Methods
        //Refresh connection after connection update
        private void RefreshConnectionUI()
        {
            // Reload the config from file
            config = JsonConvert.DeserializeObject<DbConfig>(File.ReadAllText(configPath));

            if (picStatus.Image != null)
            {
                picStatus.Image.Dispose();
                picStatus.Image = null;
            }
            if (picDbStatus.Image != null)
            {
                picDbStatus.Image.Dispose();
                picDbStatus.Image = null;
            }
            // Update server/database labels
            lblServer.Text = "Server: ";
            if (config.Alias != null)
            {
                lblServerName.Text = config.Alias;
            }
            else
            {
                lblServerName.Text = config.Server;
            }
            lblDB.Text = "Database: ";
            lblDbName.Text = config.Database;

            //Build connection string
            bob.DataSource = config.Server;
            if (config.Create == false)
            {
                bob.InitialCatalog = config.Database;
            }
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

            bool isConnected = TestDbConnection(bob.ToString());

            lblCStatus.Text = isConnected ? "Connected" : "Disconnected";
            lblCStatus.ForeColor = isConnected ? Color.Green : Color.Red;

            iconFile = isConnected ? "Success.png" : "Error.png";
            imagePath = Path.Combine(projectRoot, @"Content\Images", iconFile);

            if (File.Exists(imagePath))
                picStatus.Image = Image.FromFile(imagePath);
            else
                picStatus.Image = null;

            if (config.Create == true && isConnected)
            {
                lblDbStat.Text = "Need to create Database";
                lblDbStat.ForeColor = Color.FromArgb(255, 204, 0);
                lblDbName.ForeColor = Color.FromArgb(255, 204, 0);
                lblDbName.BackColor = Color.FromArgb(100, 0, 0, 0);
                lblDbStat.BackColor = Color.FromArgb(100, 0, 0, 0);
                // Load image
                imagePathDb = Path.Combine(projectRoot, @"Content\Images", "Warning.png");
                picDbStatus.Image = Image.FromFile(imagePathDb);
                picDbStatus.Left = lblDbName.Right;
                picDbStatus.BackColor = Color.FromArgb(100, 0, 0, 0);
            }


        }
        //Test Server connection
        public bool TestDbConnection(string connectionString)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    btnBuild.Enabled = true;
                    lblServerName.ForeColor = Color.Green;
                    btnEdit.Text = "Edit Server connection";
                    return true; // Connected successfully
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //Checks server for database
        public void CheckServer(string connectionString, string sender)
        {
            if (picDbStatus.Image != null)
            {
                picDbStatus.Image.Dispose();
                picDbStatus.Image = null;
            }
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                string name = "";
                using (SqlCommand dbCheck = new SqlCommand("use master select Name from sys.databases where Name = '" + config.Database + "'"))
                {
                    dbCheck.Connection = conn;
                    dbCheck.CommandType = CommandType.Text;
                    conn.Open();
                    SqlDataReader reader = dbCheck.ExecuteReader();
                    if (reader.Read())
                    {
                        lblDbStat.Text = "Database created";
                        lblDbStat.ForeColor = Color.Green;
                        lblDbStat.BackColor = Color.Transparent;
                        conn.Close();
                        dbConnection = true;
                        bob.InitialCatalog = config.Database;
                        btnBuild.Enabled = false;
                        config.Create = false;
                        WriteConfig();
                        lblDbName.ForeColor = Color.Green;
                        lblDbName.BackColor = Color.Transparent;
                        imagePathDb = Path.Combine(projectRoot, @"Content\Images", "Success.png");
                        picDbStatus.Image = Image.FromFile(imagePathDb);
                        picDbStatus.BackColor = Color.Transparent;
                        btnPopulate.Enabled = true;
                    }
                    else
                    {
                        btnPopulate.Enabled = false;
                        dbConnection = false;
                        btnBuild.Enabled = true;
                        config.Create = true;
                        WriteConfig();
                        lblDbStat.Text = "Need to create Database";
                        lblDbStat.ForeColor = Color.FromArgb(255, 204, 0);
                        lblDbStat.BackColor = Color.FromArgb(100, 0, 0, 0);
                        // Load image
                        lblDbName.ForeColor = Color.FromArgb(255, 204, 0);
                        lblDbName.BackColor = Color.FromArgb(100, 0, 0, 0);
                        imagePathDb = Path.Combine(projectRoot, @"Content\Images", "Warning.png");
                        picDbStatus.BackColor = Color.FromArgb(100, 0, 0, 0);
                        picDbStatus.Image = Image.FromFile(imagePathDb);
                    }
                }
            }

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
            CheckServer(connectionString, "create");
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
                        WriteConfig();
                        btnBuild.Enabled = false;
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
                GetSeasons(connectionString);
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



        //Add Status picture to Table panel
        public void AddTablePic(Panel panel, PictureBox pic, string path, string sender, Label header)
        {
            AddPanelElement(panel, pic);
            pic.Image = Image.FromFile(path);
            pic.Height = header.Height - (int)(header.Height * .1);
            pic.Width = header.Height - (int)(header.Height * .1);
            pic.Left = header.Right - (int)(header.Height * .1);
            pic.Top = header.Top + (int)(header.Height * .15);
            pic.SizeMode = PictureBoxSizeMode.Zoom;
            if (sender == "expand")
            {
                pic.Height = header.Height - (int)(header.Height * .15);
                pic.Width = header.Height - (int)(header.Height * .15);
                pic.Left = header.Right - (int)(header.Height * .2);

            }
            panel.Controls.SetChildIndex(pic, 0);

        }
        //Add Label to Table panels
        public void TableLabels(Panel pnl, Label lbl, float fontSize, string labelType, Label parent)
        {
            lbl.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
            AddPanelElement(pnl, lbl);
            if (labelType == "Header")
            {
                CenterElement(pnl, lbl);
            }
            else if (labelType == "Subhead")
            {
                AlignLeft(pnl, lbl, parent);
            }

        }
        //Align label to left side of panel
        public void AlignLeft(Panel pnl, Label lbl, Label parent)
        {
            lbl.Left = pnl.Left;
            lbl.Top = parent.Bottom;
            lbl.AutoSize = true;
        }
        //Set Dynamic Font size
        public Font SetFontSize(string font, Single size, FontStyle style, Control parent, Control child)
        {
            Font newFont = new Font(font, size, style);
            int targetWidth = 0;
            if (parent.Text == "Options")
            {
                targetWidth = (int)(parent.Width * 0.8);
            }
            else
            {
                targetWidth = (int)(parent.Width * 0.7);
            }
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
        //Enables Esc key to close program
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close(); // or Application.Exit();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }
        private void ListSeasons_SelectAll(object sender, KeyEventArgs e)
        {
            // Check if Ctrl+A was pressed
            if (e.Control && e.KeyCode == Keys.A)
            {
                // Select all items
                for (int i = 0; i < listSeasons.Items.Count; i++)
                {
                    listSeasons.SetSelected(i, true);
                }

                // Mark the event as handled to prevent further processing
                e.Handled = true;
            }
        }

        public HashSet<(int GameID, string builder, double mb, long len)> gameBytes = new HashSet<(int, string builder, double, long)>();
        public void CheckStringBuildSize(int GameID, string builder, StringBuilder str)
        {
            long len = str.Length;
            double mb = len / (1024.0 * 1024.0);
            gameBytes.Add((GameID, builder, mb, len));

        }
        private void FormResize(object sender, EventArgs e)
        {
            //Maintain aspect ratio when user finishes resizing
            MaintainAspectRatio();
        }
        private void MaintainAspectRatio()
        {
            // Prevent recursive calls
            if (isResizing)
                return;

            isResizing = true;

            // Determine which dimension changed last and adjust the other
            float currentRatio = (float)this.Width / this.Height;

            if (currentRatio > aspectRatio)
            {
                // Width is too large for the ratio, adjust it
                this.Width = (int)(this.Height * aspectRatio);
            }
            else if (currentRatio < aspectRatio)
            {
                // Height is too large for the ratio, adjust it
                this.Height = (int)(this.Width / aspectRatio);
            }

            isResizing = false;
        }
        public void ChangeLabel(Label label, Control parent, List<string> structions)
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
                label.Font = SetFontSize("Segoe UI", fontSize, style, parent, label);
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
                label.ForeColor = Color.FromName(structions[7]);
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


        public void DbOverviewClick(Control control, Label growShrink, Control parent)
        {
            control.Click += (s, e) =>
            {
                if (control.Focused || parent.Focused)
                {
                    this.ActiveControl = null;
                    growShrink.Text = "+";
                    parent.Height = parent.Height / 4;
                    pnlDbOverview.Refresh();
                    lblDbOptions.Top = parent.Bottom;
                    lblDbSelectSeason.Top = lblDbOptions.Bottom;
                    listSeasons.Top = lblDbSelectSeason.Bottom;
                    btnPopulate.Top = listSeasons.Bottom; //subject to change
                }
                else
                {
                    parent.Focus();
                    growShrink.Text = "-";
                    parent.Height = parent.Height * 4;
                    pnlDbOverview.Refresh();
                    lblDbOptions.Top = parent.Bottom;
                    lblDbSelectSeason.Top = lblDbOptions.Bottom;
                    listSeasons.Top = lblDbSelectSeason.Bottom;
                    btnPopulate.Top = listSeasons.Bottom; //subject to change
                }
            };
        }

        public void SettingsClick(Control control, PictureBox picture, float fontSize)
        {
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
                    lblSettings.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, pnlWelcome, btnBuild);
                    lblSettings.Left = (pnlWelcome.ClientSize.Width - lblSettings.Width) / 2;
                }
                else
                {
                    lblSettings.Focus();
                    if (picture.Image != null)
                    {
                        picture.Image.RotateFlip(RotateFlipType.RotateNoneFlipY);
                        picture.Refresh();
                    }
                    lblSettings.Font = SetFontSize("Segoe UI", (float)(fontSize * 1.05), FontStyle.Bold | FontStyle.Underline, pnlWelcome, btnBuild);
                    lblSettings.Left = (pnlWelcome.ClientSize.Width - lblSettings.Width) / 2;
                }
            };
        }
        public void ClearImages()
        {
            // Release previous image
            if (picLoad.Image != null)
            {
                picLoad.Image.Dispose();
                picLoad.Image = null;
            }
        }


        #endregion




        #region Need to organize/move or delete
        public void GetSeasons(string connection)
        {
            listSeasons.Items.Clear();
            seasonInfo.Clear();

            using (SqlConnection conn = new SqlConnection(bob.ToString()))
            using (SqlCommand SQLSeasons = new SqlCommand("Seasons", conn))
            {
                SQLSeasons.CommandType = CommandType.StoredProcedure;
                conn.Open();

                using (SqlDataReader sdr = SQLSeasons.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        listSeasons.Items.Add(sdr["SeasonID"].ToString());
                        seasonInfo.Add((sdr.GetInt32(0), (sdr.GetInt32(1), sdr.GetInt32(2), sdr.GetInt32(3), sdr.GetInt32(4), sdr.GetInt32(5))));
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




        public async Task GetSchedule()
        {
            schedule = null;
            List<Games> games = new List<Games>();
            scheduleGames = await leagueSchedule.GetJSON();
        }


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
            int iter = (SeasonID == 2012 || SeasonID == 2019 || SeasonID == 2020) ? 3 : 4;      //No Selection
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
            TeamBoxStaging(game); //5.7 Populate DB Update

            //Players
            HistoricPlayerStaging(game); //5.7 Populate DB Update

            //PlayByPlay
            HistoricPlayByPlayStaging(game.playByPlay); //5.7 Populate DB Update
            #endregion

            //Insert data into Main tables and wait for execution
            #region Insert data into Main tables and wait for execution
            // Get the SQL string from the StringBuilder
            string hitDb = sqlBuilder.ToString();
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
            if (sender == "Historic")
            {
                arenaList.Add((SeasonID, arena.arenaId));
            }
            else if (sender == "Missing")
            {
                arenaList.Add((SeasonID, arena.arenaId));
            }

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
            sqlBuilder.Append("Insert into Game(SeasonID, GameID, Date, HomeID, HScore, AwayID, AScore, Label, LabelDetail, Datetime, ")
                      .Append("WinnerID, WScore, LoserID, Lscore, GameType, SeriesID) values(")
                      .Append(SeasonID).Append(", ")
                      .Append(GameID).Append(", '")
                      .Append(gameDate).Append("', ")
                      .Append(game.box.homeTeamId).Append(", ")
                      .Append(game.box.homeTeam.score).Append(", ")
                      .Append(game.box.awayTeamId).Append(", ")
                      .Append(game.box.awayTeam.score).Append(", '")
                      .Append(game.box.gameLabel).Append("', '")
                      .Append(game.box.gameSubLabel).Append("', '")
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
            if (Int32.Parse(game.box.gameId).ToString().Substring(0, 1) == "2")
            {
                sqlBuilder.Append("'RS', null)\n");
            }
            else
            {
                sqlBuilder.Append("'PS', 'placeholder')\n");
            }

            //GameExt
            sqlBuilder.Append("Insert into GameExt(SeasonID, GameID, Status, Attendance, Sellout");

            //Officials
            for (int o = 0; o < officials.Count; o++)
            {
                if (o == 0)
                {
                    sqlBuilder.Append(", OfficialID");
                }
                else if (o != 3)
                {
                    sqlBuilder.Append(", Official").Append(o + 1).Append("ID");
                }
                else
                {
                    sqlBuilder.Append(", OfficialAlternateID");
                }
            }

            //Start the values part
            sqlBuilder.Append(") values(")
                      .Append(SeasonID).Append(", ")
                      .Append(GameID).Append(", '")
                      .Append(game.box.gameStatusText).Append("', ")
                      .Append(game.box.attendance).Append(", ")
                      .Append(game.box.sellout);
            //Add official values
            for (int o = 0; o < officials.Count; o++)
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

        #region Historic Players - 5.7 Populate DB Update
        //5.7 Populate DB Update
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
                PlayerBoxInsertString(game, player, game.box.homeTeamId, game.box.awayTeamId);
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
                PlayerBoxInsertString(game, player, game.box.awayTeamId, game.box.homeTeamId);
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
                HistoricInactiveBoxInsert(game.box.awayTeamId, game.box.homeTeamId,inactive.personId);
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

        public void PlayerBoxInsertString(NBAdbToolboxHistoric.Game game, NBAdbToolboxHistoric.Player player, int TeamID, int MatchupID)
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

        public void PlayerCheck(NBAdbToolboxHistoric.Player player, int season, string number, string sender)
        {
            using (SqlCommand PlayerCheck = new SqlCommand("PlayerCheck"))
            {
                PlayerCheck.CommandType = CommandType.StoredProcedure;
                PlayerCheck.Parameters.AddWithValue("@PlayerID", player.personId);
                PlayerCheck.Parameters.AddWithValue("@SeasonID", season);
                using (SqlDataAdapter sPlayerSearch = new SqlDataAdapter())
                {
                    PlayerCheck.Connection = SQLdb;
                    sPlayerSearch.SelectCommand = PlayerCheck;
                    SQLdb.Open();
                    SqlDataReader reader = PlayerCheck.ExecuteReader();
                    reader.Read();
                    if (!reader.HasRows)
                    {
                        SQLdb.Close();
                        if(sender == "Historic")
                        {
                            playerList.Add((season, player.personId));
                        }
                        else if(sender == "Missing")
                        {
                            playerList.Add((season, player.personId));
                        }
                        string pInsert = "Insert into Player values(" + season + ", " + player.personId + ", '" + player.firstName.Replace("'", "''") + " " + player.familyName.Replace("'", "''") + "', '" + number + "', ";
                        if (player.position != null && player.position != "")
                        {
                            pInsert += "'" + player.position + "')\n";
                        }
                        else
                        {
                            pInsert += "null)\n";
                        }
                        playerInsert += pInsert;
                    }
                    else
                    {
                        SQLdb.Close();
                    }
                }
            }
        }
        public void InactiveCheck(NBAdbToolboxHistoric.Inactive inactive, int season, string sender)
        {
            using (SqlCommand InactiveCheck = new SqlCommand("PlayerCheck"))
            {
                InactiveCheck.CommandType = CommandType.StoredProcedure;
                InactiveCheck.Parameters.AddWithValue("@PlayerID", inactive.personId);
                InactiveCheck.Parameters.AddWithValue("@SeasonID", season);
                using (SqlDataAdapter sInactiveCheck = new SqlDataAdapter())
                {
                    InactiveCheck.Connection = SQLdb;
                    sInactiveCheck.SelectCommand = InactiveCheck;
                    SQLdb.Open();
                    SqlDataReader reader = InactiveCheck.ExecuteReader();
                    reader.Read();
                    if (!reader.HasRows)
                    {
                        SQLdb.Close();
                        if (sender == "Historic")
                        {
                            playerList.Add((season, inactive.personId));
                        }
                        else if (sender == "Missing")
                        {
                            playerList.Add((season, inactive.personId));
                        }
                        playerInsert += "Insert into Player(SeasonID, PlayerID, Name) values(" + season + ", " + inactive.personId + ", '" + inactive.firstName.Replace("'", "''") + " " + inactive.familyName.Replace("'", "''") + "')\n";
                    }
                    else
                    {
                        SQLdb.Close();
                    }
                }
            }
        }

        //PlayerBox methods with Inactive players
        #region PlayerBox Methods with Inactive players
        public string playerBoxUpdateString = "";
        public void PlayerBoxCheck(NBAdbToolboxHistoric.Game game, NBAdbToolboxHistoric.Player player, int TeamID, int MatchupID, string procedure)
        {
            SqlConnection SQLdb = new SqlConnection(cString);
            using (SqlCommand PlayerBoxCheck = new SqlCommand(procedure))
            {
                PlayerBoxCheck.Connection = SQLdb;
                PlayerBoxCheck.CommandType = CommandType.StoredProcedure;
                PlayerBoxCheck.Parameters.AddWithValue("@SeasonID", SeasonID);
                PlayerBoxCheck.Parameters.AddWithValue("@GameID", GameID);
                PlayerBoxCheck.Parameters.AddWithValue("@TeamID", TeamID);
                PlayerBoxCheck.Parameters.AddWithValue("@MatchupID", MatchupID);
                PlayerBoxCheck.Parameters.AddWithValue("@PlayerID", player.personId);
                using (SqlDataAdapter sTeamSearch = new SqlDataAdapter())
                {
                    PlayerBoxCheck.Connection = SQLdb;
                    sTeamSearch.SelectCommand = PlayerBoxCheck;
                    SQLdb.Open();
                    SqlDataReader reader = PlayerBoxCheck.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        SQLdb.Close();
                        //PlayerBoxInsert(game, player, TeamID, season, "PlayerBoxInsertHistoric");
                        PlayerBoxInsertString(game, player, TeamID, MatchupID);
                    }
                    else
                    {
                        reader.Read();
                        if (reader.GetString(4) != player.statistics.minutes && reader.GetString(4) != "0" && reader.GetString(4) != "")
                        {
                            SQLdb.Close();
                            //PlayerBoxUpdate(game, player, TeamID, season, "PlayerBoxUpdateHistoric");
                            PlayerBoxUpdateString(game, player, TeamID, MatchupID);
                        }
                        else
                        {
                            SQLdb.Close();
                        }
                    }
                }
            }
        }
        public void PlayerBoxUpdateString(NBAdbToolboxHistoric.Game game, NBAdbToolboxHistoric.Player player, int TeamID, int MatchupID)
        {
            string update = "update PlayerBox set" +
                " Points = " + player.statistics.points
                + ", FGM = " + player.statistics.fieldGoalsMade
                + ", FGA = " + player.statistics.fieldGoalsAttempted
                + ", [FG%] = " + player.statistics.fieldGoalsPercentage
                + ", FG3M = " + player.statistics.threePointersMade
                + ", FG3A = " + player.statistics.threePointersAttempted
                + ", [FG3%] = " + player.statistics.threePointersPercentage
                + ", FTM = " + player.statistics.freeThrowsMade
                + ", FTA = " + player.statistics.freeThrowsAttempted
                + ", [FT%] = " + player.statistics.freeThrowsPercentage
                + ", ReboundsDefensive = " + player.statistics.reboundsDefensive
                + ", ReboundsOffensive = " + player.statistics.reboundsOffensive
                + ", ReboundsTotal = " + player.statistics.reboundsTotal
                + ", Assists = " + player.statistics.assists
                + ", Turnovers = " + player.statistics.turnovers
                + ", Steals = " + player.statistics.steals
                + ", Blocks = " + player.statistics.blocks
                + ", FoulsPersonal = " + player.statistics.foulsPersonal
                + ", PlusMinusPoints = " + player.statistics.plusMinusPoints
                + ", FG2M = " + (player.statistics.fieldGoalsMade - player.statistics.threePointersMade)
                + ", FG2A = " + (player.statistics.fieldGoalsAttempted - player.statistics.threePointersAttempted);
            double sec = double.Parse(player.statistics.minutes.Substring(player.statistics.minutes.IndexOf("M") + 1, 5));
            sec = sec / 60;
            double minCalc = double.Parse(player.statistics.minutes.Substring(2, 2));
            minCalc = Math.Round(minCalc + sec, 2);
            update += "MinutesCalculated = " + minCalc + ", ";


            if ((double)(player.statistics.fieldGoalsAttempted - player.statistics.threePointersAttempted) != 0)
            {
                update += ", [FG2%] = " + Math.Round((double)(player.statistics.fieldGoalsMade - player.statistics.threePointersMade) /
                    (double)(player.statistics.fieldGoalsAttempted - player.statistics.threePointersAttempted), 4);
            }

            if (player.statistics.turnovers > 0)
            {
                update += ", AssistsTurnoverRatio = " + Math.Round((double)(player.statistics.assists) / (double)(player.statistics.turnovers), 3);
            }
            if (player.statistics.minutes != "")
            {
                update += ", Minutes = '" + player.statistics.minutes + "'";
            }
            string where = " where SeasonID = " + SeasonID + " and GameID = " + GameID + " and TeamID = " + TeamID + " and MatchupID = " + MatchupID + " and PlayerID = " + player.personId;

            playerBoxUpdateString += update + where + "\n";
        }
        //PlayerBox and StartingLineups
        public void InactiveBoxCheck(NBAdbToolboxHistoric.Game game, NBAdbToolboxHistoric.Inactive inactive, int TeamID, int MatchupID)
        {
            using (SqlCommand PlayerBoxCheck = new SqlCommand("PlayerBoxCheck"))
            {
                PlayerBoxCheck.Connection = new SqlConnection(bob.ToString());
                PlayerBoxCheck.CommandType = CommandType.StoredProcedure;
                PlayerBoxCheck.Parameters.AddWithValue("@SeasonID", SeasonID);
                PlayerBoxCheck.Parameters.AddWithValue("@GameID", GameID);
                PlayerBoxCheck.Parameters.AddWithValue("@TeamID", TeamID);
                PlayerBoxCheck.Parameters.AddWithValue("@MatchupID", MatchupID);
                PlayerBoxCheck.Parameters.AddWithValue("@PlayerID", inactive.personId);
                using (SqlDataAdapter sTeamSearch = new SqlDataAdapter())
                {
                    sTeamSearch.SelectCommand = PlayerBoxCheck;
                    PlayerBoxCheck.Connection.Open();
                    SqlDataReader reader = PlayerBoxCheck.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        PlayerBoxCheck.Connection.Close();
                        InactiveBoxInsertString(TeamID, MatchupID, inactive.personId);
                    }
                    else
                    {
                        PlayerBoxCheck.Connection.Close();
                    }
                }
            }
        }
        public void InactiveBoxInsertString(int TeamID, int MatchupID, int InactiveID)
        {
            playerBoxInsertString += "insert into PlayerBox(SeasonID, GameID, TeamID, MatchupID, PlayerID, Status) values(" + SeasonID + ", " + GameID + ", " + TeamID + ", " + MatchupID + ", " + InactiveID + ", 'INACTIVE')\n";
            
        }
        #endregion

        #endregion

        //TeamBox methods
        #region TeamBox Methods
        public void TeamBoxStaging(NBAdbToolboxHistoric.Game game)
        {
            //TeamBoxCheck(game.box.homeTeam, season, GameID, game.box.awayTeamId, game.box.awayTeam.statistics.points, "TeamBoxCheck", game.box.homeTeam.lineups[0]);
            //TeamBoxCheck(game.box.awayTeam, season, GameID, game.box.homeTeamId, game.box.homeTeam.statistics.points, "TeamBoxCheck", game.box.awayTeam.lineups[0]);
            //foreach (NBAdbToolboxHistoric.Lineups lineup in game.box.homeTeam.lineups)
            //{
            //    TeamBoxCheck(game.box.homeTeam, season, GameID, game.box.awayTeamId, game.box.awayTeam.statistics.points, "TeamBoxLineupCheck", lineup);
            //}
            //foreach (NBAdbToolboxHistoric.Lineups lineup in game.box.awayTeam.lineups)
            //{
            //    TeamBoxCheck(game.box.awayTeam, season, GameID, game.box.homeTeamId, game.box.homeTeam.statistics.points, "TeamBoxLineupCheck", lineup);
            //}

            //5.7 Populate DB Update
            WriteTeamBoxInsert(game.box.homeTeam, game.box.awayTeamId, game.box.awayTeam.statistics.points);
            WriteTeamBoxInsert(game.box.awayTeam, game.box.homeTeamId, game.box.homeTeam.statistics.points);
            foreach (NBAdbToolboxHistoric.Lineups lineup in game.box.homeTeam.lineups)
            {
                WriteTeamBoxLineupsInsert(game.box.homeTeam, game.box.awayTeamId, game.box.awayTeam.statistics.points, lineup);
            }
            foreach (NBAdbToolboxHistoric.Lineups lineup in game.box.awayTeam.lineups)
            {
                WriteTeamBoxLineupsInsert(game.box.awayTeam, game.box.homeTeamId, game.box.homeTeam.statistics.points, lineup);
            }
        }

        public void WriteTeamBoxInsert(NBAdbToolboxHistoric.Team team, int MatchupID, int PointsAgainst)
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

        public void WriteTeamBoxLineupsInsert(NBAdbToolboxHistoric.Team team, int MatchupID, int PointsAgainst, NBAdbToolboxHistoric.Lineups lineup)
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
                        minutes += Int32.Parse(team.players[i].statistics.minutes.Remove(team.players[i].statistics.minutes.IndexOf(":")));
                        seconds += Int32.Parse(team.players[i].statistics.minutes.Substring(team.players[i].statistics.minutes.IndexOf(":") + 1));
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
        #region Current Data Up to Date
        //Declarations
        #region Declarations
        public bool currentTeamsDone = false;
        public string currentBigInsert = "";
        public string currentFirstInsert = "";

        public string currentPlayer = "";
        public string currentPlayerBox = "";
        public string currentTeamBox = "";
        public string currentPlayByPlay = "";
        public string missingData = "";
        //Used in Game region
        public string insertExt = "";
        public string valuesExt = "";
        public string lastGame = "";
        public string seriesIDfirst7 = "";
        #endregion
        public int currentGame = 0;
        public async Task CurrentGameData(int GameID, string sender)
        {
            bool doBox = true;
            bool doPBP = true;
            bool useHistoricBox = false;
            bool useHistoricPBP = false;
            string missingInstructions = "";
            string missingNote = "";
            missingData = "";


            //Try to get Current Data
            #region Try to get Current Data
            try
            {
                rootCPBP = await currentDataPBP.GetJSON(GameID, SeasonID);
                if (rootCPBP.game == null)
                {
                    doPBP = false;
                    useHistoricPBP = true;
                    missingNote = "'No file available from NBA')\n";
                }
            }
            catch (WebException ex)
            {
                doPBP = false;
                useHistoricPBP = true;
                missingNote = "'No file available from NBA')\n";
            }

            try
            {
                rootC = await currentData.GetJSON(GameID, SeasonID);
                if (rootC.game == null)
                {
                    doBox = false;
                    useHistoricBox = true;
                    missingNote = "'No file available from NBA')\n";
                }
            }
            catch (WebException ex)
            {
                doBox = false;
                useHistoricBox = true;
                missingNote = "'No file available from NBA')\n";
            }
            #endregion
            //If it's available, get Boxscore endpoint data ready
            #region Box endpoint data collection
            Task DoBox = Task.Run(() =>
            {
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
            });
            if (iterator == TotalGames)
            {
                await DoBox;
            }
            #endregion

            //If it's available, get PlayByPlay endpoint data ready
            #region PlayByPlay endpoint data collection
            Task DoPbp = Task.Run(() =>
            {
                if (doPBP)
                {
                    try
                    {
                        InitiateCurrentPlayByPlay(rootCPBP.game);
                    }
                    catch
                    {
                        useHistoricPBP = true;
                        missingNote = "'JSON file formatting - NBA pls fix')\n";
                    }
                }
                else
                {
                    missingData += "insert into util.MissingData values(" + SeasonID + ", " + GameID + ", 'Current', 'PlayByPlay', " + missingNote + "\n";
                }
            });
            if (iterator == TotalGames)
            {
                await DoPbp;
            }
            #endregion

            #region Getting historic Data for missing data
            if (useHistoricBox)
            {
                missingInstructions += "Box";
            }
            if (useHistoricPBP)
            {
                missingInstructions += " PlayByPlay";
            }
            if(useHistoricBox || useHistoricPBP)
            {
                NBAdbToolboxHistoric.Game game = null;
                if(GameID.ToString().Substring(0, 1) == "2")
                {
                    game = root.season.games.regularSeason.FirstOrDefault(g => Int32.Parse(g.game_id) == GameID);
                }
                else
                {
                    game = root.season.games.playoffs.FirstOrDefault(g => Int32.Parse(g.game_id) == GameID);
                }
                GetMissingDataDetails(game, missingInstructions);
            }
            #endregion
        }

        public StringBuilder sqlBuilderParallel = new StringBuilder(220 * 1024); //Start with roughly .225 MB initial capacity
        #endregion


        #region Current Data StringBuilder Update 5.17
        public async Task CurrentGameGPS(int GameID, string sender)
        {
            bool doBox = true;
            bool doPBP = true;
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
            if (!currentTeamsDone)
            {
                InitiateCurrentTeam(game);
            }
            if (!arenaList.Contains((SeasonID, game.arena.arenaId))){
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
            CurrentDataInsert(sqlBuilder);
            
            sqlBuilder.Clear();
        }
        public void CurrentDataInsert(StringBuilder sqlStringBuilder)
        {
            if (sqlStringBuilder.Length == 0)
                return;
            try
            {
                string inserts = sqlStringBuilder.ToString();
                using (SqlConnection connection = new SqlConnection(cString))
                using (SqlCommand insert = new SqlCommand("set nocount on;\n" + inserts, connection))
                {
                    insert.CommandType = CommandType.Text;
                    connection.Open();
                    insert.ExecuteNonQuery();
                }
            }
            catch(Exception e) 
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
                    currentTeamsDone = true;
                }
            }

            if (!teamList.Contains((SeasonID, game.awayTeam.teamId)))
            {
                teamList.Add((SeasonID, game.awayTeam.teamId));
                CurrentTeam(game.awayTeam);

                if (teamList.Count == 30)
                {
                    currentTeamsDone = true;
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
                        .Append(game.gameId).Append(", '")
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
                string SeriesID = game.gameId.Substring(2);
                SeriesID = SeriesID.Remove(SeriesID.Length - 1) + "-";
                gameInsertSB.Append("'PS', '").Append(SeriesID).Append("')");

                // Update tracking variables
                lastGame = Int32.Parse(game.gameId).ToString();
                seriesIDfirst7 = lastGame.Remove(lastGame.Length - 1);
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
            gameExtSB.Append("Insert into GameExt(SeasonID, GameID, Status, Attendance");

            // Start the values part for GameExt
            StringBuilder gameExtValuesSB = new StringBuilder();
            gameExtValuesSB.Append(") values(")
                           .Append(SeasonID).Append(", ")
                           .Append(game.gameId).Append(", '")
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
                HistoricGameInsert(game, sender, officials); //5.7 Populate DB Update
                //TeamBox
                TeamBoxStaging(game);
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
