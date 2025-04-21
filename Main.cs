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

namespace NBAdbToolbox
{
    public partial class Main : Form
    {
        NBAdbToolboxHistoric.Root root = new NBAdbToolboxHistoric.Root();
        NBAdbToolboxCurrent.Root rootC = new NBAdbToolboxCurrent.Root();
        NBAdbToolboxCurrentPBP.Root rootCPBP = new NBAdbToolboxCurrentPBP.Root();
        //Determine whether or not we have a connection to the Database in dbconfig file
        public bool dbConnection = false;
        //File path for project
        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\.."));
        //dbconfig file
        string configPath = Path.Combine(projectRoot, "Content", "dbconfig.json");
        private DbConfig config;
        //Screen size/Display variables
        public int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
        public int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;

        //Connection String items
        public static string cString = "";
        public SqlConnectionStringBuilder bob = new SqlConnectionStringBuilder();  //This builds connection string
        public SqlConnection SQLdb = new SqlConnection(cString);

        //SQL building items
        public static string buildFile = File.ReadAllText(Path.Combine(projectRoot, "Content", "build.sql"));   //Creates tables
        //Splits procedures off from table creation. For some reason, it won't let me do them at once, even if the formatting I have works straight in SQL. it really doesnt like 'go'
        public static string procedures = buildFile.Substring(buildFile.IndexOf("~") + 1).Replace("*/", "");
        //Each procedure is a list item
        public static List<string> procs = new List<string>();

        //pnlWelcome items
        public Panel pnlWelcome = new Panel();
        private Label lblStatus = new Label();      //Header label
        private Label lblServer = new Label();      //Server
        private Label lblServerName = new Label();  //Server name
        private Label lblCStatus = new Label();     //Connection string status
        private PictureBox picStatus =              //Connection string icon
            new PictureBox();
        private string iconFile = "";               //Icon file name
        private string imagePath = "";              //Icon file path
        private Label lblDB = new Label();          //Database
        private Label lblDbName = new Label();      //Database name
        private PictureBox picDbStatus =            //Db Status icon
            new PictureBox();
        private string imagePathDb = "";            //Db icon file path
        private Label lblDbStat = new Label();      //Need to create database/Database created label
        private Button btnEdit = new Button();      //Edit config file
        private Button btnBuild = new Button();     //Build Database


        //pnlDbUtil items
        public Panel pnlDbUtil = new Panel();
        public Label lblDbUtil = new Label {
            Text = "Database Utilities",
        };
        public Panel pnlSeason = new Panel(); public Label lblSeason = new Label(); public PictureBox picSeason = new PictureBox(); public Label lblSeasonSub = new Label();
        public Panel pnlTeam = new Panel(); public Label lblTeam = new Label(); public PictureBox picTeam = new PictureBox(); public Label lblTeamSub = new Label();
        public Panel pnlGame = new Panel(); public Label lblGame = new Label(); public PictureBox picGame = new PictureBox(); public Label lblGameSub = new Label();
        public Panel pnlPlayerBox = new Panel(); public Label lblPlayerBox = new Label(); public PictureBox picPlayerBox = new PictureBox(); public Label lblPlayerBoxSub = new Label();
        public Panel pnlTeamBox = new Panel(); public Label lblTeamBox = new Label(); public PictureBox picTeamBox = new PictureBox(); public Label lblTeamBoxSub = new Label();
        public Panel pnlPlayer = new Panel(); public Label lblPlayer = new Label(); public PictureBox picPlayer = new PictureBox(); public Label lblPlayerSub = new Label();
        public Panel pnlPbp = new Panel(); public Label lblPbp = new Label(); public PictureBox picPbp = new PictureBox(); public Label lblPbpSub = new Label();
        public Panel pnlTeamBoxLineups = new Panel(); public Label lblTeamBoxLineups = new Label(); public PictureBox picTeamBoxLineups = new PictureBox(); public Label lblTeamBoxLineupsSub = new Label();
        //pnlDbUtil sub panel Positions and sizes
        public int leftPanelPos = 0;
        public int midPanelPos = 0;
        public int rightPanelPos = 0;
        public int fullHeight = 0;
        public int dimW = 0;
        public int dimH = 0;
        public int dimH2 = 0;
        //pnlDbUtil Options section
        public Label lblDbOptions = new Label
        {
            Text = "Options",
        };
        public Label lblDbSelectSeason = new Label
        {
            Text = "Season Select"
        };
        public ListBox listSeasons = new ListBox();
        public Button btnPopulate = new Button();
        public static DataHistoric historic = new DataHistoric();
        public static DataCurrent currentData = new DataCurrent();
        public static DataCurrentPBP currentDataPBP = new DataCurrentPBP();
        public Panel pnlLoad = new Panel();
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
        public float loadFontSize = 0;
        public string completionMessage = "";

        PictureBox picLoad = new PictureBox
        {
            Image = Image.FromFile(Path.Combine(projectRoot, "Content", "Loading", "kawhi1.png")),
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
        public HashSet<(int, (int, int, int, int))> seasonInfo = new HashSet<(int, (int, int, int, int))>();


        public Stopwatch stopwatchInsert    = new Stopwatch();
        public Stopwatch stopwatchRead      = new Stopwatch();
        public TimeSpan  timeElapsedRead    = new TimeSpan(0);
        public string    elapsedStringRead  = "";
        public Main()
        {
            InitializeComponent();
            //Set screen size
            screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
            this.Width = (int)(screenWidth / 1.5);
            this.Height = (int)(screenHeight / 1.2);
            this.StartPosition = FormStartPosition.Manual;
            this.Left = (screenWidth - this.Width) / 2;
            this.Top = (screenHeight - this.Height) / 2;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false; // optional

            //Server Connection status variable
            bool isConnected = false;
            // Create Background image
            PictureBox bgCourt = new PictureBox
            {
                Image = Image.FromFile(Path.Combine(projectRoot, "Content", "Court.png")),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width= this.Width, 
                Height= this.Height,
                Dock = DockStyle.Fill
            };


            pnlWelcome.BorderStyle = BorderStyle.FixedSingle;
            pnlWelcome.Width = (int)(this.ClientSize.Width / 2.5);
            pnlWelcome.Height = (int)(this.ClientSize.Height / 2.5);
            pnlWelcome.Left = (this.ClientSize.Width - pnlWelcome.Width) / 2;
            pnlWelcome.Top = (this.ClientSize.Height - pnlWelcome.Height) / 2;
            pnlWelcome.BackColor = Color.Transparent;


            pnlDbUtil.Height = this.Height;
            pnlDbUtil.Dock = DockStyle.Left;
            pnlDbUtil.Width = pnlWelcome.Left;

            lblServer.Text = "Server: ";
            lblDB.Text = "Database: ";
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
                imagePath = Path.Combine(projectRoot, "Content", "Error.png");
                picStatus.Image = Image.FromFile(imagePath);
                btnBuild.Enabled = false;
                btnPopulate.Enabled = false;
            }
            else if (File.Exists(configPath)) //If our file does exist
            {
                lblStatus.Text = "Welcome Back!";
                btnEdit.Text = "Edit Server connection";
                btnEdit.Width = (int)(lblStatus.Width / 1.5);
                string json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<DbConfig>(json);
                //Set label text
                //lblServer.Text += config.Server;
                lblServerName.Text = "placeholder";//config.Server;
                lblDbName.Text = config.Database;

                //Build connection string
                bob.DataSource = config.Server;
                if (config.Create == false)
                {
                    bob.InitialCatalog = config.Database;
                }
                bob.UserID = config.Username;
                bob.Password = config.Password;
                bob.IntegratedSecurity = false;
                cString = bob.ToString();
                if (config.Server != "" && config.Username != "" && config.Password != "")
                {
                    isConnected = TestDbConnection(cString);
                }
                if (isConnected)
                {
                    lblCStatus.Text = "Connected";
                    lblCStatus.ForeColor = Color.Green;
                    // Load image
                    imagePath = Path.Combine(projectRoot, "Content", "Success.png");
                    picStatus.Image = Image.FromFile(imagePath);
                    btnBuild.Enabled = true;
                }
                else
                {
                    lblCStatus.Text = "Disconnected";
                    lblCStatus.ForeColor = Color.Red;
                    // Load image
                    imagePath = Path.Combine(projectRoot, "Content", "Error.png");
                    picStatus.Image = Image.FromFile(imagePath);
                    btnBuild.Enabled = false;
                }
                CheckServer(cString, "main");
            }








            #region Add Elements
            //This should be second to last i believe.
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
            AddPanelElement(pnlDbUtil, lblDbUtil);
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
            #endregion

            //Set Panel Properties ***************************************************************************
            //DbUtil
            //Draws border without messing up the background image alignment
            pnlDbUtil.BorderStyle = BorderStyle.None;
            pnlDbUtil.Paint += (s, e) =>
            {
                Control p = (Control)s;
                using (Pen pen = new Pen(Color.White, 1))
                {
                    e.Graphics.DrawLine(pen, p.Width - 1, 0, p.Width - 1, p.Height);
                }
            };
            //End Border drawing section
            pnlDbUtil.Parent = bgCourt;
            lblDbUtil.Height = (int)(pnlWelcome.Height * .1);
            float fontSize = ((float)(pnlWelcome.Height * .08) / (96 / 12)) * (72 / 12);
            lblDbUtil.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, pnlDbUtil, lblDbUtil);
            //Auto-size and center
            lblDbUtil.AutoSize = true;
            CenterElement(pnlDbUtil, lblDbUtil);

            //Will be used primarily for table panel expansion later, but need for DbOptions
            fullHeight = (int)(pnlDbUtil.Height * .5);
            dimW = pnlDbUtil.Width / 3;
            dimH = (int)(fullHeight * .25);
            dimH2 = (int)(fullHeight * .5);

            lblDbOptions.Height = (int)(lblDbUtil.Height * .9);
            lblDbOptions.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, pnlDbUtil, lblDbOptions);
            //Auto-size and center
            lblDbOptions.Top = fullHeight + (int)(lblDbOptions.Height * .85);
            lblDbOptions.AutoSize = true;
            CenterElement(pnlDbUtil, lblDbOptions);

            lblDbSelectSeason.Height = (int)(lblDbUtil.Height * .8);
            lblDbSelectSeason.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, lblDbOptions, lblDbSelectSeason);
            lblDbSelectSeason.Top = lblDbOptions.Bottom;
            lblDbSelectSeason.Left = pnlDbUtil.Left;
            lblDbSelectSeason.AutoSize = true;
            AlignLeft(pnlDbUtil, lblDbSelectSeason, lblDbOptions); 


            listSeasons.SelectionMode = SelectionMode.MultiExtended;
            listSeasons.Top = lblDbSelectSeason.Bottom;
            listSeasons.Left = pnlDbUtil.Left;

            btnPopulate.Text = "Populate Db";
            btnPopulate.Font = SetFontSize("Segoe UI", 6.5F, FontStyle.Bold, pnlWelcome, btnPopulate);
            btnPopulate.Width = (int)(listSeasons.Width * .8);
            btnPopulate.Top = listSeasons.Bottom; //subject to change



            btnPopulate.Click += async (s, e) =>
            {
                int selectedSeasons = listSeasons.SelectedItems.Count;
                string dialog = "Seasons selected: ";
                List<int> seasons = new List<int>();

                for (int i = 0; i < listSeasons.SelectedItems.Count; i++)
                {
                    seasons.Add(Int32.Parse(listSeasons.SelectedItems[i].ToString()));
                    dialog += seasons[i] + ", ";
                }
                if(selectedSeasons > 0)
                {
                    dialog = dialog.Remove(dialog.Length - 2);
                }
                var popup = new PopulatePopup(dialog);
                if (popup.ShowDialog() == DialogResult.OK)
                {
                    using (var img = Image.FromFile(Path.Combine(projectRoot, "Content", "Loading", "kawhi1" + ".png")))
                    {
                        picLoad.Image = new Bitmap(img); // clone it so file lock is released
                        picLoad.SizeMode = PictureBoxSizeMode.Zoom;
                        picLoad.Width = pnlLoad.Height;
                        picLoad.Height = pnlLoad.Height;
                        picLoad.Left = (pnlLoad.ClientSize.Width - picLoad.Width) / 2;
                        picLoad.BackColor = Color.Transparent;
                        picLoad.Top = 0;
                    }
                    int historic = 0;
                    int current = 0;
                    string source = "";
                    gpm.Visible = true;
                    gpmValue.Visible = true;
                    lblCurrentGame.Visible = true;
                    lblCurrentGameCount.Visible = true;
                    lblCurrentGame.Text = "Current game: ";
                    lblCurrentGame.Left = 4;
                    fontSize = ((float)(pnlLoad.Height * .05) / (96 / 12)) * (72 / 12);
                    lblCurrentGame.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, pnlLoad, lblCurrentGame);
                    lblCurrentGame.AutoSize = true;
                    lblCurrentGame.ForeColor = Color.Black;
                    lblSeasonStatusLoad.Visible = true;
                    lblSeasonStatusLoad.Text = "Currently loading: ";
                    lblSeasonStatusLoad.ForeColor = Color.Black;
                    fontSize = ((float)(pnlLoad.Height * .08) / (96 / 12)) * (72 / 12);
                    lblSeasonStatusLoad.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, pnlLoad, lblSeasonStatusLoad);
                    lblSeasonStatusLoad.Left = 0;
                    lblSeasonStatusLoad.AutoSize = true; 

                    lblSeasonStatusLoadInfo.Visible = true;
                    picLoad.Visible = true;
                    lblWorkingOn.Visible = true;
                    fontSize = ((float)(pnlLoad.Height * .04) / (96 / 12)) * (72 / 12);
                    lblWorkingOn.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, pnlLoad, lblWorkingOn);
                    lblWorkingOn.Left = picLoad.Right - (int)(picLoad.Width / 6);
                    lblWorkingOn.AutoSize = true;
                    lblWorkingOn.Top = lblSeasonStatusLoadInfo.Bottom;
                    int buildID = 0;
                    using (SqlCommand BuildLogCheck = new SqlCommand("BuildLogCheck"))
                    {
                        BuildLogCheck.CommandType = CommandType.StoredProcedure;
                        SqlConnection SQLdb = new SqlConnection(cString);
                            using (SqlDataAdapter sBuildLogCheck = new SqlDataAdapter())
                            {

                                BuildLogCheck.Connection = SQLdb;
                                sBuildLogCheck.SelectCommand = BuildLogCheck;
                                SQLdb.Open();
                                SqlDataReader reader = BuildLogCheck.ExecuteReader();
                                while (reader.Read())
                                {
                                    buildID = reader.GetInt32(0);
                                }
                                SQLdb.Close();
                            }
                        
                    }
                    Stopwatch stopwatchFull = Stopwatch.StartNew();
                    DateTime startFull = DateTime.Now;
                    foreach (int season in seasons)
                    {
                        Stopwatch stopwatch = Stopwatch.StartNew();
                        DateTime start = DateTime.Now;
                        completionMessage += season + ": ";
                        lblStatus.Text = "Loading " + season + " season...";
                        CenterElement(pnlWelcome, lblStatus);
                        btnPopulate.Enabled = false;
                        btnEdit.Enabled = false;
                        listSeasons.Enabled = false;

                        int iterator = 0;
                        int imageIteration = 1;
                        bool reverse = false;
                        int remainder = 5;
                        int regGames = 0;
                        //Historic Data
                        #region Historic Data
                        if ((popup.historic || (!popup.historic && !popup.current)) && season < 2019)
                        {
                            source = "Historic";
                            historic = 1;
                            lblSeasonStatusLoadInfo.Text = season + " historic data file";
                            stopwatchRead.Start();
                            await Task.Run(async () =>      //This sets the root variable to our big file
                            {
                                await ReadSeasonFile(season, popup.historic, popup.current);
                            });
                            stopwatchRead.Stop();
                            TimeSpan timeElapsedRead = stopwatchRead.Elapsed;
                            Dictionary<string, (int, string)> timeUnitsRead = new Dictionary<string, (int value, string sep)>
                            {
                                { "h", (timeElapsedRead.Hours, ":") },
                                { "m", (timeElapsedRead.Minutes, ":") },
                                { "s", (timeElapsedRead.Seconds, ".") },
                                { "ms", (timeElapsedRead.Milliseconds, "") }
                            };
                            string elapsedStringRead = CheckTime(timeUnitsRead);
                            //End season read
                            lblStatus.Text = season + " parsed. Inserting data...";
                            CenterElement(pnlWelcome, lblStatus);
                            lblSeasonStatusLoadInfo.Text = season + " Regular Season";
                            stopwatchInsert.Restart();
                            //int totalGames = seasonGames.First(g => g.SeasonID == season).Games;
                            int totalGamesCount = root.season.games.regularSeason.Count + root.season.games.playoffs.Count;
                            foreach (NBAdbToolboxHistoric.Game game in root.season.games.regularSeason)
                            {
                                await Task.Run(async () =>      //This inserts the games from season file into db
                                {
                                    await InsertGameWithLoading(game, season, imageIteration, "Regular Season");
                                });
                                if (reverse)
                                {
                                    imageIteration--;
                                }
                                else
                                {
                                    imageIteration++;
                                }
                                if (imageIteration == 25)
                                {
                                    reverse = true;
                                }
                                if (imageIteration == 1)
                                {
                                    reverse = false;
                                }
                                iterator++;
                                int gamesLeft = totalGamesCount - iterator;
                                double gamesPerSec = iterator / stopwatchInsert.Elapsed.TotalSeconds;
                                double gamesPerMin = Math.Round(gamesPerSec * 60, 2);
                                double estimatedSeconds = gamesLeft / gamesPerSec;
                                TimeSpan timeRemaining = TimeSpan.FromSeconds(estimatedSeconds);
                                string time = timeRemaining.ToString();
                                time = time.Remove(time.Length - 3);
                                gpmValue.Invoke((MethodInvoker)(() =>
                                {
                                    gpmValue.Text = gamesPerMin + "\n" + time;
                                }));
                            }
                            regGames = iterator;
                            lblSeasonStatusLoadInfo.Text = season + " Postseason";
                            foreach (NBAdbToolboxHistoric.Game game in root.season.games.playoffs)
                            {
                                await Task.Run(async () =>      //This inserts the games from season file into db
                                {
                                    await InsertGameWithLoading(game, season, imageIteration, "Postseason");
                                });
                                if (reverse)
                                {
                                    imageIteration--;
                                }
                                else
                                {
                                    imageIteration++;
                                }
                                if (imageIteration == 25)
                                {
                                    reverse = true;
                                }
                                if (imageIteration == 1)
                                {
                                    reverse = false;
                                }
                                iterator++;
                                int gamesLeft = totalGamesCount - iterator;
                                double gamesPerSec = iterator / stopwatchInsert.Elapsed.TotalSeconds;
                                double gamesPerMin = Math.Round(gamesPerSec * 60, 2);
                                double estimatedSeconds = gamesLeft / gamesPerSec;
                                TimeSpan timeRemaining = TimeSpan.FromSeconds(estimatedSeconds);
                                string time = timeRemaining.ToString();
                                time = time.Remove(time.Length - 3);
                                gpmValue.Invoke((MethodInvoker)(() =>
                                {
                                    gpmValue.Text = gamesPerMin + "\n" + time;
                                }));
                            }
                        
                        }
                        #endregion
                        //Current Data
                        #region Current Data
                        else if ((popup.current || (!popup.historic && !popup.current)) && season > 2018)
                        {
                            source = "Current";
                            List<int> gamesRS = new List<int>();
                            List<int> gamesPS = new List<int>();
                            current = 1;
                            lblSeasonStatusLoadInfo.Text = "Getting " + season + " GameIDs";
                            await Task.Run(async () =>      //We need to read the big file to get our game list
                            {
                                await ReadSeasonFile(season, popup.historic, popup.current);
                            });
                            foreach(NBAdbToolboxHistoric.Game g in root.season.games.regularSeason)
                            {
                                gamesRS.Add(Int32.Parse(g.game_id));
                            }
                            foreach (NBAdbToolboxHistoric.Game g in root.season.games.playoffs)
                            {
                                gamesPS.Add(Int32.Parse(g.game_id));
                            }
                            //int totalGames = seasonGames.First(g => g.SeasonID == season).Games;
                            int regular = gamesRS.Count;
                            int post = gamesPS.Count;
                            int totalGamesCount = gamesRS.Count + gamesPS.Count;
                            stopwatchInsert.Restart();
                            currentIterator = 0;
                            currentImageIterator = 0;
                            currentReverse = false;
                            stopwatchInsert.Restart();
                            lblSeasonStatusLoad.Text = "Currently ";
                            lblSeasonStatusLoad.AutoSize = true;
                            lblSeasonStatusLoadInfo.Text = "Hitting endpoints and inserting" + season + " data";
                            for (int i = 0; i < regular; i++)
                            {
                                await CurrentGameData(gamesRS[i], season, "");
                                iterator++;
                                int gamesLeft = totalGamesCount - iterator;
                                double gamesPerSec = iterator / stopwatchInsert.Elapsed.TotalSeconds;
                                double gamesPerMin = Math.Round(gamesPerSec * 60, 2);
                                double estimatedSeconds = gamesLeft / gamesPerSec;
                                TimeSpan timeRemaining = TimeSpan.FromSeconds(estimatedSeconds);
                                string time = timeRemaining.ToString();
                                time = time.Remove(time.Length - 3);
                                gpmValue.Invoke((MethodInvoker)(() =>
                                {
                                    gpmValue.Text = gamesPerMin + "\n" + time;
                                }));
                            }
                            for (int i = 0; i < post; i++)
                            {
                                await CurrentGameData(gamesPS[i], season, "");
                                iterator++;
                                int gamesLeft = totalGamesCount - iterator;
                                double gamesPerSec = iterator / stopwatchInsert.Elapsed.TotalSeconds;
                                double gamesPerMin = Math.Round(gamesPerSec * 60, 2);
                                double estimatedSeconds = gamesLeft / gamesPerSec;
                                TimeSpan timeRemaining = TimeSpan.FromSeconds(estimatedSeconds);
                                string time = timeRemaining.ToString();
                                time = time.Remove(time.Length - 3);
                                gpmValue.Invoke((MethodInvoker)(() =>
                                {
                                    gpmValue.Text = gamesPerMin + "\n" + time;
                                }));
                            }
                        }
                        #endregion


                        //Updates
                        #region Need to fix
                        //List<int> currentGames = new List<int>();
                        //var match = seasonInfo.FirstOrDefault(x => x.Item1 == season);
                        //int historicCheck = 0;
                        //if (!match.Equals(default))
                        //{
                        //    historicCheck = match.Item2.Item3;
                        //    // use thirdValue here
                        //}
                        ////If historic exists
                        //if (!popup.historic && popup.current && season > 2018 && historicCheck == 1)
                        //{
                        //    source = "Current";
                        //    current = 1;                                                                                                 //Line below is for testing
                        //    using (SqlCommand SelectGamesDeletePBP = new SqlCommand("SelectGamesDeletePBP")) ///GameID >= 21900194 and 
                        //    {
                        //        SqlConnection conn = new SqlConnection(bob.ToString());
                        //        SelectGamesDeletePBP.Connection = conn;
                        //        SelectGamesDeletePBP.CommandType = CommandType.StoredProcedure;
                        //        SelectGamesDeletePBP.Parameters.AddWithValue("@Season", season);
                        //        conn.Open();
                        //        using (SqlDataReader sdr = SelectGamesDeletePBP.ExecuteReader())
                        //        {
                        //            while (sdr.Read())
                        //            {
                        //                currentGames.Add(sdr.GetInt32(0));
                        //            }
                        //        }
                        //    }

                        //    currentIterator = 0;
                        //    currentImageIterator = 0;
                        //    currentReverse = false;
                        //    stopwatchInsert.Restart();
                        //    foreach (int game in currentGames)
                        //    {
                        //        await Task.Run(async () =>      //This sets the root variable to our big file
                        //        {
                        //            await ReadCurrentGameData(game, season, "Existing");
                        //        });
                        //        iterator++;
                        //        int gamesLeft = currentGames.Count - iterator;
                        //        double gamesPerSec = iterator / stopwatchInsert.Elapsed.TotalSeconds;
                        //        double gamesPerMin = Math.Round(gamesPerSec * 60, 2);
                        //        double estimatedSeconds = gamesLeft / gamesPerSec;
                        //        TimeSpan timeRemaining = TimeSpan.FromSeconds(estimatedSeconds);
                        //        string time = timeRemaining.ToString();
                        //        time = time.Remove(time.Length - 3);
                        //        gpmValue.Invoke((MethodInvoker)(() =>
                        //        {
                        //            gpmValue.Text = gamesPerMin + "\n" + time;
                        //        }));
                        //    }
                        //}
                        #endregion

                        stopwatch.Stop();
                        stopwatchInsert.Stop();
                        DateTime end = DateTime.Now;
                        TimeSpan timeElapsedSeason = stopwatch.Elapsed;
                        Dictionary<string, (int, string)> timeUnitsSeason = new Dictionary<string, (int value, string sep)>
                        {
                        { "h", (timeElapsedSeason.Hours, ":") },
                        { "m", (timeElapsedSeason.Minutes, ":") },
                        { "s", (timeElapsedSeason.Seconds, ".") },
                        { "ms", (timeElapsedSeason.Milliseconds, "") }
                        };
                        string elapsedStringSeason = CheckTime(timeUnitsSeason);

                        TimeSpan timeElapsedInsert = stopwatchInsert.Elapsed;
                        Dictionary<string, (int, string)> timeUnitsInsert = new Dictionary<string, (int value, string sep)>
                        {
                        { "h", (timeElapsedInsert.Hours, ":") },
                        { "m", (timeElapsedInsert.Minutes, ":") },
                        { "s", (timeElapsedInsert.Seconds, ".") },
                        { "ms", (timeElapsedInsert.Milliseconds, "") }
                        };
                        string elapsedStringInsert = CheckTime(timeUnitsInsert);


                        using(SQLdb = new SqlConnection(cString))
                        {
                            using (SqlCommand BuildLogInsert = new SqlCommand("BuildLogInsert"))
                            {
                                BuildLogInsert.Connection = SQLdb;
                                BuildLogInsert.CommandType = CommandType.StoredProcedure;
                                BuildLogInsert.Parameters.AddWithValue("@BuildID", buildID);
                                BuildLogInsert.Parameters.AddWithValue("@Season", season);
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
                                SQLdb.Open();
                                BuildLogInsert.ExecuteScalar();
                                SQLdb.Close();
                            }
                        }

                        //completionMessage += (iterator - regGames) + " postseason games loaded successfully.\n";

                        completionMessage += elapsedStringSeason + ". ";
                        completionMessage += iterator + " games, " + regGames + "/" + (iterator - regGames) + "\n";
                        lblWorkingOn.Text = completionMessage;
                        fontSize = ((float)(pnlLoad.Height * .03) / (96 / 12)) * (72 / 12);
                        lblWorkingOn.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, pnlLoad, lblWorkingOn);
                        lblWorkingOn.AutoSize = true;
                        //lblWorkingOn.Left = picLoad.Right - (int)(picLoad.Width / 5);
                        lblWorkingOn.Left = pnlLoad.Width - lblWorkingOn.Width;
                        lblWorkingOn.Top = lblSeasonStatusLoadInfo.Bottom;


                        teamsDone = false;
                        arenasDone = false;
                        playersDone = false;
                        timeElapsedRead = TimeSpan.Zero;
                        elapsedStringRead = "";
                        iterator = 0;
                    }
                    stopwatchFull.Stop();
                    DateTime endFull = DateTime.Now;
                    TimeSpan timeElapsed = stopwatchFull.Elapsed;
                    Dictionary<string, (int, string)> timeUnits = new Dictionary<string, (int value, string sep)>
                    {
                        { "h", (timeElapsed.Hours, ":") },
                        { "m", (timeElapsed.Minutes, ":") },
                        { "s", (timeElapsed.Seconds, ".") },
                        { "ms", (timeElapsed.Milliseconds, "") }
                    };
                    //string elapsedString = timeElapsed.Hours + ":" + timeElapsed.Minutes + ":" + timeElapsed.Seconds + "." + timeElapsed.Milliseconds;
                    string elapsedString = CheckTime(timeUnits);
                    btnPopulate.Enabled = true;
                    btnEdit.Enabled = true;
                    listSeasons.Enabled = true;
                    lblStatus.Text = "Welcome Back!";
                    CenterElement(pnlWelcome, lblStatus);
                    lblCurrentGame.Text = "Total time elapsed: " + elapsedString;
                    lblCurrentGame.ForeColor = Color.Green;
                    fontSize = ((float)(pnlLoad.Height * .05) / (96 / 12)) * (72 / 12);
                    lblCurrentGame.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, pnlLoad, lblCurrentGame);
                    lblCurrentGame.AutoSize = true;

                    lblSeasonStatusLoadInfo.Text = "";
                    lblSeasonStatusLoadInfo.Visible = false;


                    lblCurrentGameCount.Text = "";
                    lblCurrentGameCount.Visible = false;
                    lblSeasonStatusLoad.Text = "Done! Check your SQL db";
                    lblSeasonStatusLoad.ForeColor = Color.Green;
                    fontSize = ((float)(pnlLoad.Height * .08) / (96 / 12)) * (72 / 12);
                    lblSeasonStatusLoad.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, pnlLoad, lblSeasonStatusLoad);
                    lblSeasonStatusLoad.AutoSize = true;
                    if (picLoad.Image != null)
                    {
                        picLoad.Image.Dispose(); // release the previous image
                        picLoad.Image = null;
                    }

                    fontSize = ((float)(pnlLoad.Height * .035) / (96 / 12)) * (72 / 12);
                    lblWorkingOn.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, pnlLoad, lblWorkingOn);
                    lblWorkingOn.AutoSize = true;
                    lblWorkingOn.Left = pnlLoad.Width - lblWorkingOn.Width;
                    lblWorkingOn.Top = 0;
                    using (var img = Image.FromFile(Path.Combine(projectRoot, "Content", "Success.png")))
                    {
                        picLoad.Image = new Bitmap(img); // clone it so file lock is released
                        picLoad.SizeMode = PictureBoxSizeMode.Zoom;
                        picLoad.Width = (int)(pnlLoad.Height * .5);
                        picLoad.Height = (int)(pnlLoad.Height * .5);
                        picLoad.Top = gpm.Bottom + (int)(picLoad.Height * .25);
                        picLoad.Left = ((pnlLoad.ClientSize.Width - picLoad.Width) / 2) - picLoad.Width;
                    }

                }
            };
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
            pnlNav.Height = this.Height/20;
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
            //GetScoreboardGames();
            //pnlScoreboard.Paint += (s, e) =>
            //{
            //    Control p = (Control)s;
            //    using (Pen pen = new Pen(Color.Black, 3))
            //    {
            //        e.Graphics.DrawLine(pen, 0, p.Height - 1, p.Width, p.Height - 1);
            //    }
            //};




            //Set Label Properties ***************************************************************************



            //To set font, i'll need the name, ideal size or pt, and its Style.
            //In addition, i also need the parent element and the child or the element we're working with
            lblStatus.Height = (int)(pnlWelcome.Height * .1);
            fontSize = ((float)(lblStatus.Height) / (96 / 12)) * (72 / 12); //Formula is picking the correct Pt, as determined by the height of the label
            lblStatus.Font = SetFontSize("Segoe UI",fontSize, FontStyle.Bold, pnlWelcome, lblStatus);
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
            btnEdit.Font = SetFontSize("Segoe UI", 12F, FontStyle.Bold, pnlWelcome, btnEdit);
            CenterElement(pnlWelcome, btnEdit);
            btnEdit.Top = lblDbStat.Bottom + 10; //subject to change
            btnEdit.TextAlign = ContentAlignment.BottomCenter;





            fontSize = ((float)(lblServer.Height) / (96 / 12)) * (72 / 12)/2;
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
            btnBuild.Font = SetFontSize("Segoe UI", 14F, FontStyle.Bold, pnlWelcome, btnBuild);
            btnBuild.AutoSize = true;
            CenterElement(pnlWelcome, btnBuild);
            btnBuild.Top = btnEdit.Bottom + 10; //subject to change
            #endregion





            #region Edit Connection & Build DB
            //Edit Button Actions
            btnEdit.Click += (s, e) =>
            {
                string server = config?.Server ?? "";
                bool? create = config?.Create ;
                string database = config?.Database ?? "";
                string username = config?.Username ?? "";
                string password = config?.Password ?? "";
                bool fileExist = true;

                if (!File.Exists(configPath))
                {
                    fileExist = false;
                }
                var popup = new EditPopup("create", fileExist, server, create, database, username, password);
                if (popup.ShowDialog() == DialogResult.OK)
                {
                    config = new DbConfig
                    {
                        Server = popup.Value1,
                        Create = popup.Value2,
                        Database = popup.Value3,
                        Username = popup.Value4,
                        Password = popup.Value5
                    };

                    File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
                    RefreshConnectionUI();
                }
            };

            btnBuild.Click += (s, e) =>
            {
                if(config.Create == true)
                {
                    CreateDB(cString);
                    bob.InitialCatalog = config.Database;
                    cString = bob.ToString();
                }
            };
            #endregion
            if (dbConnection)
            {
                GetTablePanelInfo(bob.ToString());
            }
            List<Panel> panels = new List<Panel> { pnlSeason, pnlTeam, pnlPlayer, pnlGame, pnlPlayerBox, pnlTeamBox, pnlPbp, pnlTeamBoxLineups };
            
            //Mid Start
            pnlTeam.Click += (s, e) =>
            {
                if (pnlTeam.Focused)
                {
                    this.ActiveControl = null;
                    pnlTeam.Left = pnlSeason.Right;
                    pnlTeam.Width = dimW;
                    pnlTeam.Height = dimH;
                    fontSize = ((float)((pnlTeam.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlTeam, lblTeam, fontSize, "Header", lblTeam);
                    pnlTeam.Left = midPanelPos;
                }
                else
                {
                    midPanelPos = pnlTeam.Left;
                    pnlTeam.Focus();
                    pnlDbUtil.Controls.SetChildIndex(pnlTeam, 1);
                    pnlTeam.Left = pnlDbUtil.Left;
                    pnlTeam.Width = pnlDbUtil.Width;
                    pnlTeam.Height = fullHeight;
                    fontSize = ((float)((pnlTeam.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlTeam, lblTeam, fontSize, "Header", lblTeam);

                }
            };
            lblTeam.Click += (s, e) =>
            {
                if (pnlTeam.Focused)
                {
                    this.ActiveControl = null;
                    pnlTeam.Left = pnlSeason.Right;
                    pnlTeam.Width = dimW;
                    pnlTeam.Height = dimH;
                    fontSize = ((float)((pnlTeam.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlTeam, lblTeam, fontSize, "Header", lblTeam);
                    pnlTeam.Left = midPanelPos;
                }
                else
                {
                    midPanelPos = pnlTeam.Left;
                    pnlTeam.Focus();
                    pnlDbUtil.Controls.SetChildIndex(pnlTeam, 1);
                    pnlTeam.Left = pnlDbUtil.Left;
                    pnlTeam.Width = pnlDbUtil.Width;
                    pnlTeam.Height = fullHeight;
                    fontSize = ((float)((pnlTeam.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlTeam, lblTeam, fontSize, "Header", lblTeam);

                }
            }; 
            //Left start
            pnlSeason.Click += (s, e) =>
            {
                if (pnlSeason.Focused)
                {
                    this.ActiveControl = null;
                    pnlSeason.Width = dimW;
                    pnlSeason.Height = dimH;
                    fontSize = ((float)((pnlSeason.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlSeason, lblSeason, fontSize, "Header", lblSeason);
                    fontSize = ((float)((pnlSeason.Height * .08)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlSeason, lblSeasonSub, fontSize, "Subhead", lblSeason);
                    AddTablePic(pnlSeason, picSeason, imagePath, "contract", lblSeason);
                    pnlSeason.Left = leftPanelPos;
                }
                else
                {
                    leftPanelPos = pnlSeason.Left;
                    pnlSeason.Focus();
                    pnlDbUtil.Controls.SetChildIndex(pnlSeason, 1);
                    pnlSeason.Width = pnlDbUtil.Width;
                    pnlSeason.Height = fullHeight;
                    fontSize = ((float)((pnlSeason.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlSeason, lblSeason, fontSize, "Header", lblSeason);
                    fontSize = ((float)((pnlSeason.Height * .08)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlSeason, lblSeasonSub,  fontSize, "Subhead", lblSeason);
                    AddTablePic(pnlSeason, picSeason, imagePath, "expand", lblSeason);

                }
            };
            lblSeason.Click += (s, e) =>
            {
                if (pnlSeason.Focused)
                {
                    this.ActiveControl = null;
                    pnlSeason.Width = dimW;
                    pnlSeason.Height = dimH;
                    fontSize = ((float)((pnlSeason.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlSeason, lblSeason, fontSize, "Header", lblSeason);
                    fontSize = ((float)((pnlSeason.Height * .08)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlSeason, lblSeasonSub, fontSize, "Subhead", lblSeason);
                    AddTablePic(pnlSeason, picSeason, imagePath, "contract", lblSeason);
                    pnlSeason.Left = leftPanelPos;
                }
                else
                {
                    leftPanelPos = pnlSeason.Left;
                    pnlSeason.Focus();
                    pnlDbUtil.Controls.SetChildIndex(pnlSeason, 1);
                    pnlSeason.Width = pnlDbUtil.Width;
                    pnlSeason.Height = fullHeight;
                    fontSize = ((float)((pnlSeason.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlSeason, lblSeason, fontSize, "Header", lblSeason);
                    fontSize = ((float)((pnlSeason.Height * .08)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlSeason, lblSeasonSub, fontSize, "Subhead", lblSeason);
                    AddTablePic(pnlSeason, picSeason, imagePath, "expand", lblSeason);

                }
            };
        }

        public void LoadStatusLabels(Label label, string text, float fontSize, int left, int top, int height, int width, Color color)
        {

        }

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
        //Get Table Information for DBUtil
        public void GetTablePanelInfo(string connectionString)
        {
            List<Panel> panels = new List<Panel> { pnlSeason, pnlTeam, pnlPlayer, pnlGame, pnlTeamBox, pnlPlayerBox, pnlPbp, pnlTeamBoxLineups };
            fullHeight = (int)(pnlDbUtil.Height * .5);
            dimW = pnlDbUtil.Width / 3;
            dimH = (int)(fullHeight * .25);
            dimH2 = (int)(fullHeight * .5);
            for (int i = 0; i < panels.Count; i++)
            {
                if (i <= 5)
                {
                    panels[i].Height = dimH;
                    panels[i].Width = dimW;
                }
                else
                {
                    panels[i].Height = dimH2;
                    panels[i].Width = pnlDbUtil.Width / 2;
                }
                if (i == 0)
                {
                    pnlScoreboard.Height = this.Height / 18;
                    panels[i].Top = (int)(pnlScoreboard.Top + (pnlScoreboard.Height * .1));
                    panels[i].Left = pnlDbUtil.Left;
                }
                else if (i == 1 || i == 2)
                {
                    panels[i].Top = panels[0].Top;
                    panels[i].Left = panels[i - 1].Right;
                }
                else if (i == 3 || i == 6)
                {
                    panels[i].Top = panels[i - 3].Bottom;
                    panels[i].Left = panels[i - 3].Left;
                }
                else if (i == 4 || i == 5)
                {
                    panels[i].Top = panels[0].Bottom;
                    panels[i].Left = panels[i - 1].Right;
                }
                else if (i == 7)
                {
                    panels[i].Top = panels[i - 1].Top;
                    panels[i].Left = panels[i - 1].Right;
                }
                AddPanelElement(pnlDbUtil, panels[i]);
                panels[i].BorderStyle = BorderStyle.FixedSingle;
            }
            using (SqlCommand GetTables = new SqlCommand("Tables"))
            {
                SqlConnection conn = new SqlConnection(bob.ToString());
                GetTables.Connection = conn;
                GetTables.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (SqlDataReader sdr = GetTables.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        if (sdr.GetString(0) == "Season")   //Season Panel
                        {
                            lblSeason.Text = sdr["Name"].ToString();
                            lblSeasonSub.Text = sdr["Rows"].ToString() + " seasons available";
                            float fontSize = ((float)((pnlSeason.Height * .15)) / (96 / 12)) * (72 / 12);

                            TableLabels(pnlSeason, lblSeason, fontSize, "Header", lblSeason);

                            fontSize = ((float)((pnlSeason.Height * .08)) / (96 / 12)) * (72 / 12);
                            TableLabels(pnlSeason, lblSeasonSub, fontSize, "Subhead", lblSeason);



                            if (sdr["Rows"].ToString() == "13")
                            {
                                imagePath = Path.Combine(projectRoot, "Content", "Success.png");
                            }
                            else if (sdr["Rows"].ToString() == "0")
                            {
                                imagePath = Path.Combine(projectRoot, "Content", "Error.png");
                            }
                            else
                            {
                                imagePath = Path.Combine(projectRoot, "Content", "Warning.png");
                            }
                            AddTablePic(pnlSeason, picSeason, imagePath, "load", lblSeason);

                        }
                        if (sdr.GetString(0) == "Team")   //Team Panel
                        {
                            lblTeam.Text = sdr.GetString(0);
                            float fontSize = ((float)((pnlTeam.Height * .15)) / (96 / 12)) * (72 / 12);
                            TableLabels(pnlTeam, lblTeam, fontSize, "Header", lblTeam);
                        }
                        if (sdr.GetString(0) == "Player")   //PlayerBox Panel
                        {
                            lblPlayer.Text = sdr.GetString(0);
                            float fontSize = ((float)((pnlPlayer.Height * .15)) / (96 / 12)) * (72 / 12);
                            TableLabels(pnlPlayer, lblPlayer, fontSize, "Header", lblPlayer);
                        }
                        if (sdr.GetString(0) == "Game")   //Game Panel
                        {
                            lblGame.Text = sdr.GetString(0);
                            float fontSize = ((float)((pnlGame.Height * .15)) / (96 / 12)) * (72 / 12);
                            TableLabels(pnlGame, lblGame, fontSize, "Header", lblGame);
                        }
                        if (sdr.GetString(0) == "TeamBox")   //TeamBox Panel
                        {
                            lblTeamBox.Text = sdr.GetString(0);
                            float fontSize = ((float)((pnlTeamBox.Height * .15)) / (96 / 12)) * (72 / 12);
                            TableLabels(pnlTeamBox, lblTeamBox, fontSize, "Header", lblTeamBox);
                        }
                        if (sdr.GetString(0) == "PlayerBox")   //PlayerBox Panel
                        {
                            lblPlayerBox.Text = sdr.GetString(0);
                            float fontSize = ((float)((pnlPlayerBox.Height * .15)) / (96 / 12)) * (72 / 12);
                            TableLabels(pnlPlayerBox, lblPlayerBox, fontSize, "Header", lblPlayerBox);
                        }
                        if (sdr.GetString(0) == "PlayByPlay")   //PlayByPlay Panel
                        {
                            lblPbp.Text = sdr.GetString(0);
                            float fontSize = ((float)((pnlPbp.Height * .1)) / (96 / 12)) * (72 / 12);
                            TableLabels(pnlPbp, lblPbp, fontSize, "Header", lblPbp);
                        }
                        if (sdr.GetString(0) == "TeamBoxLineups")   //PlayByPlay Panel
                        {
                            float fontSize = ((float)((pnlTeamBoxLineups.Height * .1)) / (96 / 12)) * (72 / 12);
                            lblTeamBoxLineups.Text = sdr.GetString(0);
                            TableLabels(pnlTeamBoxLineups, lblTeamBoxLineups, fontSize, "Header", lblTeamBoxLineups);
                        }
                    }
                }
            }
            using (SqlCommand GetTables = new SqlCommand("Seasons"))
            {
                SqlConnection conn = new SqlConnection(bob.ToString());
                GetTables.Connection = conn;
                GetTables.CommandType = CommandType.StoredProcedure;
                conn.Open();
                using (SqlDataReader sdr = GetTables.ExecuteReader())
                {
                    while (sdr.Read())
                    {
                        //select SeasonID, Games + PlayoffGames Games, HistoricLoaded, CurrentLoaded, Games, PlayoffGames
                        listSeasons.Items.Add(sdr["SeasonID"].ToString());
                        seasonInfo.Add((sdr.GetInt32(0), (sdr.GetInt32(1), sdr.GetInt32(2), sdr.GetInt32(3), sdr.GetInt32(4))));
                    }
                }
            }
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


        #region Database and Server Methods
        //Refresh connection after connection update
        private void RefreshConnectionUI()
        {
            // Reload the config from file
            config = JsonConvert.DeserializeObject<DbConfig>(File.ReadAllText(configPath));

            // Update server/database labels
            lblServer.Text = "Server: ";
            lblServerName.Text = config.Server;
            lblDB.Text = "Database: ";
            lblDbName.Text = config.Database;

                                                                                //Build connection string
            bob.DataSource = config.Server;
            if (config.Create == false)
            {
                bob.InitialCatalog = config.Database;
            }
            bob.UserID = config.Username;
            bob.Password = config.Password;
            bob.IntegratedSecurity = false;
            cString = bob.ToString();

            bool isConnected = TestDbConnection(bob.ToString());

            lblCStatus.Text = isConnected ? "Connected" : "Disconnected";
            lblCStatus.ForeColor = isConnected ? Color.Green : Color.Red;

            iconFile = isConnected ? "Success.png" : "Error.png";
            imagePath = Path.Combine(projectRoot, "Content", iconFile);

            if (File.Exists(imagePath))
                picStatus.Image = Image.FromFile(imagePath);
            else
                picStatus.Image = null;

            if(config.Create == true && isConnected)
            {
                lblDbStat.Text = "Need to create Database";
                lblDbStat.ForeColor = Color.FromArgb(255, 204, 0);
                lblDbName.ForeColor = Color.FromArgb(255, 204, 0);
                lblDbName.BackColor = Color.FromArgb(100, 0, 0, 0);
                lblDbStat.BackColor = Color.FromArgb(100, 0, 0, 0);
                // Load image
                imagePathDb = Path.Combine(projectRoot, "Content", "Warning.png");
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
                    return true; // Connected successfully
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
        //Checks server for database
        public void CheckServer( string connectionString, string sender)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
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
                        File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
                        lblDbName.ForeColor = Color.Green;
                        lblDbName.BackColor = Color.Transparent;
                        imagePathDb = Path.Combine(projectRoot, "Content", "Success.png");
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
                        File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));

                        lblDbStat.Text = "Need to create Database";
                        lblDbStat.ForeColor = Color.FromArgb(255, 204, 0);
                        lblDbStat.BackColor = Color.FromArgb(100, 0, 0, 0);
                        // Load image
                        lblDbName.ForeColor = Color.FromArgb(255, 204, 0);
                        lblDbName.BackColor = Color.FromArgb(100, 0, 0, 0);
                        imagePathDb = Path.Combine(projectRoot, "Content", "Warning.png");
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
                using (SqlCommand InsertData = new SqlCommand("use master create database " + config.Database))     //Create database
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
                        File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
                        btnBuild.Enabled = false;
                        FormatProcedures();
                    }
                    catch (SqlException ex)
                    {

                    }
                    conn.Close();
                }
                for(int i = 0; i < procs.Count; i++)
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
                GetTablePanelInfo(connectionString);
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

        #region Misc Utilities
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
        #endregion

        //Scoreboard
        #region Scoreboard
        public class Meta
        {
            public int Version { get; set; }
            public string Request { get; set; }
            public DateTime Time { get; set; }
            public int Code { get; set; }
        }

        public class Period
        {
            public int PeriodNumber { get; set; }
            public string PeriodType { get; set; }
            public int Score { get; set; }
        }

        public class Team
        {
            public int TeamId { get; set; }
            public string TeamName { get; set; }
            public string TeamCity { get; set; }
            public string TeamTricode { get; set; }
            public int Wins { get; set; }
            public int Losses { get; set; }
            public int Score { get; set; }
            public int? Seed { get; set; }
            public int TimeoutsRemaining { get; set; }
            public List<Period> Periods { get; set; }
        }

        public class Leaders
        {
            public int PersonId { get; set; }
            public string Name { get; set; }
            public string JerseyNum { get; set; }
            public string Position { get; set; }
            public string TeamTricode { get; set; }
            public string PlayerSlug { get; set; }
            public int Points { get; set; }
            public int Rebounds { get; set; }
            public int Assists { get; set; }
        }

        public class GameLeaders
        {
            public Leaders HomeLeaders { get; set; }
            public Leaders AwayLeaders { get; set; }
        }

        public class PbOdds
        {
            public object Team { get; set; }
            public double Odds { get; set; }
            public int Suspended { get; set; }
        }

        public class Game
        {
            public string GameId { get; set; }
            public string GameCode { get; set; }
            public int GameStatus { get; set; }
            public string GameStatusText { get; set; }
            public int Period { get; set; }
            public string GameClock { get; set; }
            public DateTime GameTimeUTC { get; set; }
            public DateTime GameEt { get; set; }
            public int RegulationPeriods { get; set; }
            public bool IfNecessary { get; set; }
            public string SeriesGameNumber { get; set; }
            public string GameLabel { get; set; }
            public string GameSubLabel { get; set; }
            public string SeriesText { get; set; }
            public string SeriesConference { get; set; }
            public string PoRoundDesc { get; set; }
            public string GameSubtype { get; set; }
            public Team HomeTeam { get; set; }
            public Team AwayTeam { get; set; }
            public GameLeaders GameLeaders { get; set; }
            public PbOdds PbOdds { get; set; }
        }

        public class Scoreboard
        {
            public string GameDate { get; set; }
            public string LeagueId { get; set; }
            public string LeagueName { get; set; }
            public List<Game> Games { get; set; }
        }

        public class Root
        {
            public Meta Meta { get; set; }
            public Scoreboard Scoreboard { get; set; }
        }

        public static int postback = 0;
        public static int games = 0;
        public int tomorrowGames = 0;
        public static List<string> gameID = new List<string>();
        public static List<int> gameStatus = new List<int>();
        public static List<string> gameStatusText = new List<string>();
        public static List<string> gameStart = new List<string>();
        public static List<int> homeID = new List<int>();
        public static List<string> homeTri = new List<string>();
        public static List<string> homeCity = new List<string>();
        public static List<string> homeName = new List<string>();
        public static List<int> homeScore = new List<int>();
        public static List<int> awayID = new List<int>();
        public static List<string> awayTri = new List<string>();
        public static List<string> awayCity = new List<string>();
        public static List<string> awayName = new List<string>();
        public static List<int> awayScore = new List<int>();
        public static List<DateTime> gameDate = new List<DateTime>();
        public static List<string> broadcasts = new List<string>();

        public void PopulateScoreboard(string gameId, int i)
        {
            Label lbl = new Label();

            lbl.Text = gameID[i];
            
            pnlScoreboard.Controls.Add(lbl);
            lbl.Left = i * 10;

        }
        public void GetScoreboardGames()
        {
            var sbClient = new WebClient { Encoding = System.Text.Encoding.UTF8 };
            string sbEndpoint = "https://cdn.nba.com/static/json/liveData/scoreboard/todaysScoreboard_00.json";
            try
            {
                WebRequest sbReq = WebRequest.Create(sbEndpoint);
                WebResponse sbResp = sbReq.GetResponse();
                string sbJson = sbClient.DownloadString(sbEndpoint);
                Root JSON = JsonConvert.DeserializeObject<Root>(sbJson);
                games = JSON.Scoreboard.Games.Count;
                for (int i = 0; i < JSON.Scoreboard.Games.Count; i++)
                {
                    gameID.Add(JSON.Scoreboard.Games[i].GameId);
                    gameStatus.Add(JSON.Scoreboard.Games[i].GameStatus);
                    gameStatusText.Add(JSON.Scoreboard.Games[i].GameStatusText);
                    gameStart.Add(JSON.Scoreboard.Games[i].GameEt.ToShortTimeString());
                    homeID.Add(JSON.Scoreboard.Games[i].HomeTeam.TeamId);
                    homeTri.Add(JSON.Scoreboard.Games[i].HomeTeam.TeamTricode);
                    homeCity.Add(JSON.Scoreboard.Games[i].HomeTeam.TeamCity);
                    homeName.Add(JSON.Scoreboard.Games[i].HomeTeam.TeamName);
                    homeScore.Add(JSON.Scoreboard.Games[i].HomeTeam.Score);
                    awayID.Add(JSON.Scoreboard.Games[i].AwayTeam.TeamId);
                    awayTri.Add(JSON.Scoreboard.Games[i].AwayTeam.TeamTricode);
                    awayCity.Add(JSON.Scoreboard.Games[i].AwayTeam.TeamCity);
                    awayName.Add(JSON.Scoreboard.Games[i].AwayTeam.TeamName);
                    awayScore.Add(JSON.Scoreboard.Games[i].AwayTeam.Score);
                    broadcasts.Add(string.Empty);
                    gameDate.Add(DateTime.MinValue);
                    PopulateScoreboard(gameID[i], i);

                }
            }
            catch
            {

            }
        }

        #endregion

        //Historic Data
        #region Historic Data
        public async Task ReadSeasonFile(int season, bool bHistoric, bool bCurrent)
        {
            string filePath = Path.Combine(projectRoot, "Content\\", "dbconfig.json");              //Line 1568 is TESTing data, 1567 normal
            filePath = filePath.Replace("dbconfig.json", "Historic Data\\");
            //filePath = filePath.Replace("dbconfig.json", "Historic Data\\test\\");
            int iter = (season == 2012 || season == 2019 || season == 2020 || season == 2024) ? 3 : 4;      //No Selection
            root = await historic.ReadFile(season, iter, filePath);                                         //Season before 2019

        }

        private async Task InsertGameWithLoading(NBAdbToolboxHistoric.Game game, int season, int imageIteration, string sender)
        {
            lblCurrentGameCount.Invoke((MethodInvoker)(() =>
            {
                lblCurrentGameCount.Text = game.game_id;
            }));
            await Task.Run(() =>
            {
                GetGameDetails(game, season, sender);
            });
            //pnlLoad.Invoke((MethodInvoker)(() => pnlLoad.Visible = true));
            picLoad.Invoke((MethodInvoker)(() =>
            {
                if (picLoad.Image != null)
                {
                    picLoad.Image.Dispose(); // release the previous image
                    picLoad.Image = null;
                }

                using (var img = Image.FromFile(Path.Combine(projectRoot, "Content", "Loading", "kawhi" + imageIteration + ".png")))
                {
                    picLoad.Image = new Bitmap(img); // clone it so file lock is released
                }
            })); 
        }

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
        //For each game, this method checks each of our tables and determines what to do, Insert, update, or nothing.
        public void GetGameDetails(NBAdbToolboxHistoric.Game game, int season, string sender)
        {
            teamInsert = "";
            arenaInsert = "";
            officialInsert = "";
            playerInsert = "";
            gameInsert = "";
            teamBoxInsert = "";
            playerBoxInsertString = "";
            playerBoxUpdateString = "";
            if (season != lastSeason)
            {
                playerList.Clear();
                officialList.Clear();
                arenaList.Clear();
            }
            SQLdb = new SqlConnection(cString);

            //Check Db and build strings for Inserts & Updates
            #region Check Db and build strings for Inserts & Updates
            //Teams
            TeamStaging(game, season);
            //Arenas
            if (!arenaList.Contains((season, game.box.arena.arenaId)))
            {
                ArenaCheck(game.box.arena, game.box.homeTeamId, season);
            }
            //Officials
            foreach (NBAdbToolboxHistoric.Official official in game.box.officials)
            {
                if (!officialList.Contains((season, official.personId)))
                {
                    OfficialCheck(official, season);
                }
            }
            //Games
            GameCheck(game, season, sender);
            //TeamBox
            TeamBoxStaging(game, season);
            //Players
            PlayerStaging(game, season);
            //PlayByPlay
            PlayByPlayStaging(game.playByPlay, season);
            #endregion

            //Insert data into Main tables and wait for execution
            #region Insert data into Main tables and wait for execution
            string allInOne = teamInsert + arenaInsert + officialInsert + gameInsert + playerInsert;
            string hitDb = allInOne;
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

            }
            #endregion

            //After Insert is complete, use multithreading to insert the bulk of our data - PlayByPlay, PlayerBox (+StartingLineups) and TeamBox (+TeamBoxLineups)
            #region
            _ = Task.Run(() =>
            {
                SqlConnection bigInsertsPBP = new SqlConnection(cString);
                try
                {
                    using (bigInsertsPBP)
                    {
                        using (SqlCommand PBPInsert = new SqlCommand(teamBoxInsert + playerBoxInsertString + insertPBPString))
                        {
                            PBPInsert.Connection = bigInsertsPBP;
                            PBPInsert.CommandType = CommandType.Text;
                            bigInsertsPBP.Open();
                            PBPInsert.ExecuteNonQuery();
                            bigInsertsPBP.Close();
                        }
                    }
                }
                catch (Exception e)
                {

                }
            });
            #endregion

            lastSeason = season;
        }

        //Team methods
        #region Team Methods
        //Reduces GetGameDetails
        public void TeamStaging(NBAdbToolboxHistoric.Game game, int season)
        {
            if (!teamsDone) //If we don't have 30 teams, check to see if team exists
            {
                TeamCheck(game.box.homeTeam, game.box.homeTeam.teamId, season);
                TeamCheck(game.box.awayTeam, game.box.awayTeam.teamId, season);
            }
            if (game.box.homeTeam.teamWins + game.box.homeTeam.teamLosses == 82 || (season == 2024) ||  //If we've reached the last game of the season (or over 40 for covid shortened 2019) update Team records 
            (season == 2019 && (game.box.homeTeam.teamWins + game.box.homeTeam.teamLosses >= 40)) ||
            (season == 2020 && (game.box.homeTeam.teamWins + game.box.homeTeam.teamLosses == 72)))      
            {
                TeamUpdate(game.box.homeTeam, season);
            }
            if (game.box.awayTeam.teamWins + game.box.awayTeam.teamLosses == 82 || (season == 2024) ||  //Same for away team
            (season == 2019 && (game.box.awayTeam.teamWins + game.box.awayTeam.teamLosses >= 40)) ||
            (season == 2020 && (game.box.awayTeam.teamWins + game.box.awayTeam.teamLosses == 72)))     
            {
                TeamUpdate(game.box.awayTeam, season);
            }
        }
        //Checks to see if Team exists and if we have 30 Teams posted

        public void TeamCheck(NBAdbToolboxHistoric.Team team, int teamID, int season)
        {
            using (SqlCommand TeamSearch = new SqlCommand("TeamCheck"))
            {
                TeamSearch.CommandType = CommandType.StoredProcedure;
                TeamSearch.Parameters.AddWithValue("@TeamID", teamID);
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
                    { //If no result, send to TeamInsert to insert into DB
                        SQLdb.Close();
                        teamList.Add((season, team.teamId));
                        teamInsert += "insert into Team values(" + season + ", " + team.teamId + ", '" + team.teamCity + "', '" + team.teamName + "', '" + team.teamTricode + "', " + team.teamWins + ", " + team.teamLosses + ", '(" + team.teamTricode + ") " + 
                            team.teamCity + " " + team.teamName + "')\n";
                        if(teamList.Count == 30)
                        {
                            teamsDone = true;
                        }
                    }
                }
            }
        }
        public void TeamUpdate(NBAdbToolboxHistoric.Team team, int season)
        {
            using (SqlCommand TeamUpdate = new SqlCommand("TeamUpdate"))
            {
                TeamUpdate.Connection = SQLdb;
                TeamUpdate.CommandType = CommandType.StoredProcedure;
                TeamUpdate.Parameters.AddWithValue("@SeasonID", season);
                TeamUpdate.Parameters.AddWithValue("@TeamID", team.teamId);
                TeamUpdate.Parameters.AddWithValue("@W", team.teamWins);
                TeamUpdate.Parameters.AddWithValue("@L", team.teamLosses);
                SQLdb.Open();
                TeamUpdate.ExecuteScalar();
                SQLdb.Close();
            }
        }
        #endregion

        //Arena methods
        #region Arena Methods
        public void ArenaCheck(NBAdbToolboxHistoric.Arena arena, int teamID, int season)
        {
            using (SqlCommand TeamSearch = new SqlCommand("ArenaCheck"))
            {
                TeamSearch.CommandType = CommandType.StoredProcedure;
                TeamSearch.Parameters.AddWithValue("@ArenaID", arena.arenaId);
                TeamSearch.Parameters.AddWithValue("@SeasonID", season);
                using (SqlDataAdapter sTeamSearch = new SqlDataAdapter())
                {
                    TeamSearch.Connection = SQLdb;
                    sTeamSearch.SelectCommand = TeamSearch;
                    SQLdb.Open();
                    SqlDataReader reader = TeamSearch.ExecuteReader();
                    if (reader.HasRows)
                    {
                        while (reader.Read())
                        {
                            if (Int32.Parse(reader["Arenas"].ToString()) >= 40)
                            {
                                arenasDone = true;
                            }
                        }
                        SQLdb.Close();
                    }
                    else
                    {
                        SQLdb.Close();
                        //ArenaInsert(arena, teamID, season);
                        arenaList.Add((season, arena.arenaId));
                        arenaInsert += "insert into Arena values(" + season + ", " + arena.arenaId + ", " + teamID + ", '" + arena.arenaCity + "', '" + arena.arenaCountry + "', '" + arena.arenaName + "', '" +
                        arena.arenaPostalCode + "', '" + arena.arenaState + "', '" + arena.arenaStreetAddress + "', '" + arena.arenaTimezone + "')\n";
                    }
                }
            }
        }
        #endregion

        //Official methods
        #region Official Methods
        public void OfficialCheck(NBAdbToolboxHistoric.Official official, int season)
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
                        officialList.Add((season, official.personId));
                        officialInsert += "insert into Official values(" + season + ", " + official.personId + ", '" + official.name + "', '" + official.jerseyNum + "')\n";
                    }
                }
            }
        }
        #endregion

        //Game methods
        #region Game Methods
        public void GameCheck(NBAdbToolboxHistoric.Game game, int season, string sender)
        {
            using (SqlCommand GameCheck = new SqlCommand("GameCheck"))
            {
                GameCheck.CommandType = CommandType.StoredProcedure;
                GameCheck.Parameters.AddWithValue("@GameID", game.game_id);
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
                        SqlDateTime datetime = SqlDateTime.Parse(game.box.gameTimeUTC);
                        string insert = "Insert into Game(SeasonID, GameID, Date, HomeID, HScore, AwayID, AScore, Label, LabelDetail, Datetime, ";
                        string values = ") values(" + season + ", " + game.game_id + ", '" + SqlDateTime.Parse(game.box.gameEt.Remove(game.box.gameEt.IndexOf('T'))) + "', " + game.box.homeTeamId + ", " + game.box.homeTeam.score
                            + ", " + game.box.awayTeamId + ", " + game.box.awayTeam.score + ", '" + game.box.gameLabel + "', '" + game.box.gameSubLabel + "', '" + datetime + "', ";
                        insert += "WinnerID, WScore, LoserID, Lscore, GameType, SeriesID";
                        if (game.box.homeTeam.score > game.box.awayTeam.score)
                        {
                            values += game.box.homeTeamId + ", " + game.box.homeTeam.score + ", " + game.box.awayTeamId + ", " + game.box.awayTeam.score + ", ";
                        }
                        else
                        {
                            values += game.box.awayTeamId + ", " + game.box.awayTeam.score + ", " + game.box.homeTeamId + ", " + game.box.homeTeam.score + ", ";
                        }
                        if (sender == "Regular Season")
                        {
                            values += "'RS', null)";
                        }
                        else
                        {
                            values += "'PS', 'placeholder')";
                        }
                        gameInsert = insert + values + "\n";
                    }
                    else
                    {
                        if (reader.GetInt32(5) != game.box.homeTeam.score || reader.GetInt32(7) != game.box.awayTeam.score)
                        {
                            SQLdb.Close();
                            GameUpdate(game, season, sender);
                        }
                        else
                        {
                            SQLdb.Close();
                        }
                    }
                }
            }
        }
        public void GameUpdate(NBAdbToolboxHistoric.Game game, int season, string sender)
        {
            using (SqlCommand GameUpdate = new SqlCommand("GameUpdate"))
            {
                GameUpdate.Connection = SQLdb;
                GameUpdate.CommandType = CommandType.StoredProcedure;
                GameUpdate.Parameters.AddWithValue("@SeasonID", season);
                GameUpdate.Parameters.AddWithValue("@GameID", game.game_id);
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
                SQLdb.Close();
            }
        }
        #endregion
        
        //Player & PlayerBox Methods - Includes Inactive Players.Populates Player, PlayerBox and StartingLineups tables
        #region Player & PlayerBox Methods - Includes Inactive Players. Populates Player, PlayerBox and StartingLineups tables

        public string playerBoxInsertString = "";
        public int lastSeason = 0;
        public void PlayerStaging(NBAdbToolboxHistoric.Game game, int season)
        {
            foreach (NBAdbToolboxHistoric.Player player in game.box.homeTeam.players)
            {//Home Team
                if (!playerList.Contains((season, player.personId)))
                {
                    int index = game.box.homeTeamPlayers.FindIndex(p => p.personId == player.personId);
                    if (index == -1)
                    {
                        PlayerCheck(player, season, player.jerseyNum);
                    }
                    else
                    {
                        PlayerCheck(player, season, game.box.homeTeamPlayers[index].jerseyNum);
                    }
                }
                PlayerBoxCheck(game, player, game.box.homeTeamId, game.box.awayTeamId, season, "PlayerBoxCheck");
            }
            foreach (NBAdbToolboxHistoric.Player player in game.box.awayTeam.players)
            {//Away Team
                if (!playerList.Contains((season, player.personId)))
                {
                    int index = game.box.awayTeamPlayers.FindIndex(p => p.personId == player.personId);
                    if (index == -1)
                    {
                        PlayerCheck(player, season, player.jerseyNum);
                    }
                    else
                    {
                        PlayerCheck(player, season, game.box.awayTeamPlayers[index].jerseyNum);
                    }
                }
                PlayerBoxCheck(game, player, game.box.awayTeamId, game.box.homeTeamId, season, "PlayerBoxCheck");
            }

            foreach (NBAdbToolboxHistoric.Inactive inactive in game.box.homeTeam.inactives)
            {
                if (!playerList.Contains((season, inactive.personId)))
                {
                    InactiveCheck(inactive, season);
                }
                InactiveBoxCheck(game, inactive, game.box.homeTeamId, game.box.awayTeamId, season);
            }
            foreach (NBAdbToolboxHistoric.Inactive inactive in game.box.awayTeam.inactives)
            {
                if (!playerList.Contains((season, inactive.personId)))
                {
                    InactiveCheck(inactive, season);
                }
                InactiveBoxCheck(game, inactive, game.box.awayTeamId, game.box.homeTeamId, season);
            }
            playerBoxInsertString = playerBoxInsertString.Replace("'',", "") + "\n" + playerBoxUpdateString;
        }
        public void PlayerCheck(NBAdbToolboxHistoric.Player player, int season, string number)
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
                        playerList.Add((season, player.personId));
                        string pInsert = "Insert into Player values(" + season + ", " + player.personId + ", '" + player.firstName.Replace("'", "''") + " " + player.familyName.Replace("'", "''") + "', '" + number + "', ";
                        if(player.position != null && player.position != "")
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
        public void PlayerUpdate(NBAdbToolboxHistoric.Player player, int season, string oldPosition, string number)
        {
            using (SqlCommand PlayerUpdate = new SqlCommand("PlayerUpdate"))
            {
                PlayerUpdate.Connection = SQLdb;
                PlayerUpdate.CommandType = CommandType.StoredProcedure;
                PlayerUpdate.Parameters.AddWithValue("@SeasonID", season);
                PlayerUpdate.Parameters.AddWithValue("@PlayerID", player.personId);
                PlayerUpdate.Parameters.AddWithValue("@Name", player.firstName + " " + player.familyName);
                PlayerUpdate.Parameters.AddWithValue("@Number", number);
                if (oldPosition != "" && !oldPosition.Contains(player.position))
                {
                    PlayerUpdate.Parameters.AddWithValue("@position", oldPosition + "/" + player.position);
                }
                else
                {
                    PlayerUpdate.Parameters.AddWithValue("@position", player.position);
                }

                SQLdb.Open();
                PlayerUpdate.ExecuteScalar();
                SQLdb.Close();
            }
        }
        public void InactiveCheck(NBAdbToolboxHistoric.Inactive inactive, int season)
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
                        playerList.Add((season, inactive.personId));
                        playerInsert += "Insert into Player(SeasonID, PlayerID, Name) values(" + season + ", " + inactive.personId + ", '" + inactive.firstName + " " + inactive.familyName + "')\n";
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
        public void PlayerBoxCheck(NBAdbToolboxHistoric.Game game, NBAdbToolboxHistoric.Player player, int TeamID, int MatchupID, int season, string procedure)
        {
            using (SqlCommand PlayerBoxCheck = new SqlCommand(procedure))
            {
                PlayerBoxCheck.Connection = SQLdb;
                PlayerBoxCheck.CommandType = CommandType.StoredProcedure;
                PlayerBoxCheck.Parameters.AddWithValue("@SeasonID", season);
                PlayerBoxCheck.Parameters.AddWithValue("@GameID", Int32.Parse(game.game_id));
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
                        PlayerBoxInsertString(game, player, TeamID, MatchupID, season);
                    }
                    else
                    {
                        reader.Read();
                        if (reader.GetString(4) != player.statistics.minutes && reader.GetString(4) != "0" && reader.GetString(4) != "")
                        {
                            SQLdb.Close();
                            //PlayerBoxUpdate(game, player, TeamID, season, "PlayerBoxUpdateHistoric");
                            PlayerBoxUpdateString(game, player, TeamID, MatchupID, season);
                        }
                        else
                        {
                            SQLdb.Close();
                        }
                    }
                }
            }
        }
        public void PlayerBoxUpdateString(NBAdbToolboxHistoric.Game game, NBAdbToolboxHistoric.Player player, int TeamID, int MatchupID, int season)
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
            string where = " where SeasonID = " + season + " and GameID = " + game.game_id + " and TeamID = " + TeamID + " and MatchupID = " + MatchupID + " and PlayerID = " + player.personId;

            playerBoxUpdateString += update + where + "\n";
        }
        //PlayerBox and StartingLineups
        public void PlayerBoxInsertString(NBAdbToolboxHistoric.Game game, NBAdbToolboxHistoric.Player player, int TeamID, int MatchupID, int season)
        {
            string insert = "insert into PlayerBox(SeasonID, GameID, TeamID, MatchupID, PlayerID, Status, FGM, FGA, [FG%], FG2M, FG2A, FG3M, FG3A, [FG3%], FTM, FTA, [FT%], " +
                "ReboundsDefensive, ReboundsOffensive, ReboundsTotal, Assists, Turnovers, Steals, Blocks, Points, FoulsPersonal, ";
            string values = ") values(" + season + ", " + game.game_id + ", " + TeamID + ", " + MatchupID + ", " + player.personId + ", 'ACTIVE', " + player.statistics.fieldGoalsMade + ", " + player.statistics.fieldGoalsAttempted
                    + ", " + player.statistics.fieldGoalsPercentage
                    + ", " + (player.statistics.fieldGoalsMade - player.statistics.threePointersMade)
                    + ", " + (player.statistics.fieldGoalsAttempted - player.statistics.threePointersAttempted)
                    + ", " + player.statistics.threePointersMade + ", " + player.statistics.threePointersAttempted + ", " + player.statistics.threePointersPercentage
                    + ", " + player.statistics.freeThrowsMade + ", " + player.statistics.freeThrowsAttempted + ", " + player.statistics.freeThrowsPercentage + ", " + player.statistics.reboundsDefensive
                    + ", " + player.statistics.reboundsOffensive + ", " + player.statistics.reboundsTotal + ", " + player.statistics.assists + ", " + player.statistics.turnovers
                    + ", " + player.statistics.steals + ", " + player.statistics.blocks + ", " + player.statistics.points + ", " + player.statistics.foulsPersonal + ", ";
            if (player.statistics.minutes != "")
            {
                insert += "Minutes, ";
                values += "'" + player.statistics.minutes.Replace("PT", "").Replace("M", ":").Replace("S", "") + "', ";
                if (player.statistics.plusMinusPoints != 0)
                {
                    insert += "PlusMinusPoints, ";
                    values += player.statistics.plusMinusPoints + ", ";
                }
            }
            else if (player.statistics.minutes == "")
            {
                insert += "Minutes, ";
                values += "'0', ";
            }

            if ((double)(player.statistics.fieldGoalsAttempted - player.statistics.threePointersAttempted) != 0)
            {
                insert += "[FG2%], ";
                values += Math.Round((double)(player.statistics.fieldGoalsMade - player.statistics.threePointersMade) /
                    (double)(player.statistics.fieldGoalsAttempted - player.statistics.threePointersAttempted), 4) + ", ";
            }
            else
            {
                insert += "[FG2%], ";
                values += "0, ";
            }


            if (player.statistics.turnovers > 0)
            {
                insert += "AssistsTurnoverRatio, ";
                values += Math.Round((double)(player.statistics.assists) / (double)(player.statistics.turnovers), 3) + ", ";
            }
            else
            {
                insert += "AssistsTurnoverRatio, ";
                values += "0, ";
            }
            insert = insert.Remove(insert.Length - ", ".Length);
            values = values.Remove(values.Length - ", ".Length) + ") ";

            playerBoxInsertString += insert + values + "\n";


            string insertStarters = "insert into StartingLineups values(" + season + ", " + game.game_id + ", " + TeamID + ", " + MatchupID + ", " + player.personId + ", '";
            if (player.position == "")
            {
                insertStarters += "Bench', null)\n";
            }
            else
            {
                insertStarters += "Starters', '" + player.position + "')\n";
            }

            playerBoxInsertString += insertStarters;
        }
        public void InactiveBoxCheck(NBAdbToolboxHistoric.Game game, NBAdbToolboxHistoric.Inactive inactive, int TeamID, int MatchupID, int season)
        {
            using (SqlCommand PlayerBoxCheck = new SqlCommand("PlayerBoxCheck"))
            {
                PlayerBoxCheck.Connection = SQLdb;
                PlayerBoxCheck.CommandType = CommandType.StoredProcedure;
                PlayerBoxCheck.Parameters.AddWithValue("@SeasonID", season);
                PlayerBoxCheck.Parameters.AddWithValue("@GameID", Int32.Parse(game.game_id));
                PlayerBoxCheck.Parameters.AddWithValue("@TeamID", TeamID);
                PlayerBoxCheck.Parameters.AddWithValue("@MatchupID", MatchupID);
                PlayerBoxCheck.Parameters.AddWithValue("@PlayerID", inactive.personId);
                using (SqlDataAdapter sTeamSearch = new SqlDataAdapter())
                {
                    PlayerBoxCheck.Connection = SQLdb;
                    sTeamSearch.SelectCommand = PlayerBoxCheck;
                    SQLdb.Open();
                    SqlDataReader reader = PlayerBoxCheck.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        SQLdb.Close();
                        InactiveBoxInsertString(Int32.Parse(game.game_id), TeamID, MatchupID, season, inactive.personId);
                    }
                    else
                    {
                        SQLdb.Close();
                    }
                }
            }
        }
        public void InactiveBoxInsertString(int GameID, int TeamID, int MatchupID, int season, int InactiveID)
        {
            playerBoxInsertString += "insert into PlayerBox(SeasonID, GameID, TeamID, MatchupID, PlayerID, Status) values(" + season + ", " + GameID + ", " + TeamID + ", " + MatchupID + ", " + InactiveID + ", 'INACTIVE')\n";
        }
        #endregion

        #endregion

        //TeamBox methods
        #region TeamBox Methods
        public void TeamBoxStaging(NBAdbToolboxHistoric.Game game, int season)
        {
            TeamBoxCheck(game.box.homeTeam, season, game.game_id, game.box.awayTeamId, game.box.awayTeam.statistics.points, "TeamBoxCheck", game.box.homeTeam.lineups[0]);
            TeamBoxCheck(game.box.awayTeam, season, game.game_id, game.box.homeTeamId, game.box.homeTeam.statistics.points, "TeamBoxCheck", game.box.awayTeam.lineups[0]);


            foreach(NBAdbToolboxHistoric.Lineups lineup in game.box.homeTeam.lineups)
            {
                TeamBoxCheck(game.box.homeTeam, season, game.game_id, game.box.awayTeamId, game.box.awayTeam.statistics.points, "TeamBoxLineupCheck", lineup);
            }
            foreach (NBAdbToolboxHistoric.Lineups lineup in game.box.awayTeam.lineups)
            {
                TeamBoxCheck(game.box.awayTeam, season, game.game_id, game.box.homeTeamId, game.box.homeTeam.statistics.points, "TeamBoxLineupCheck", lineup);
            }

        }

        public void TeamBoxCheck(NBAdbToolboxHistoric.Team team, int season, string GameID, int MatchupID, int PointsAgainst, string procedure, NBAdbToolboxHistoric.Lineups lineup)
        {
            using (SqlCommand TeamBoxCheck = new SqlCommand(procedure))
            {
                TeamBoxCheck.Connection = SQLdb;
                TeamBoxCheck.CommandType = CommandType.StoredProcedure;
                TeamBoxCheck.Parameters.AddWithValue("@SeasonID", season);
                TeamBoxCheck.Parameters.AddWithValue("@GameID", Int32.Parse(GameID));
                TeamBoxCheck.Parameters.AddWithValue("@TeamID", team.teamId);
                TeamBoxCheck.Parameters.AddWithValue("@MatchupID", MatchupID);
                if(procedure == "TeamBoxLineupCheck")
                {
                    TeamBoxCheck.Parameters.AddWithValue("@Unit", lineup.unit.Substring(0, 1).ToUpper() + lineup.unit.Substring(1));
                }
                using (SqlDataAdapter sGameSearch = new SqlDataAdapter())
                {
                    TeamBoxCheck.Connection = SQLdb;
                    sGameSearch.SelectCommand = TeamBoxCheck;
                    SQLdb.Open();
                    SqlDataReader reader = TeamBoxCheck.ExecuteReader();
                    reader.Read();
                    if (!reader.HasRows)
                    {
                        SQLdb.Close();
                        if (procedure == "TeamBoxCheck")
                        {
                            //TeamBoxInsert(team, season, GameID, MatchupID, PointsAgainst, "TeamBoxInsertHistoric", lineup);
                            teamBoxInsert += WriteTeamBoxInsert(team, season, GameID, MatchupID, PointsAgainst, lineup);
                        }
                        else if(procedure == "TeamBoxLineupCheck")
                        {
                            //TeamBoxInsert(team, season, GameID, MatchupID, PointsAgainst, "TeamBoxLineupInsertHistoric", lineup);
                            teamBoxInsert += WriteTeamBoxLineupsInsert(team, season, GameID, MatchupID, PointsAgainst, lineup);
                        }
                    }
                    else
                    {
                        if (procedure == "TeamBoxCheck")
                        {
                            if (reader.GetInt32(3) != team.statistics.points || reader.GetInt32(4) != PointsAgainst)
                            {
                                SQLdb.Close();
                                //TeamBoxUpdate(team, season, GameID, MatchupID, PointsAgainst, "TeamBoxUpdateHistoric", lineup);
                            }
                            else
                            {
                                SQLdb.Close();
                            }
                        }
                        else if (procedure == "TeamBoxLineupCheck")
                        {
                            if (reader.GetInt32(4) != lineup.points)
                            {
                                SQLdb.Close();
                                //TeamBoxUpdate(team, season, GameID, MatchupID, PointsAgainst, "TeamBoxLineupUpdateHistoric", lineup);
                            }
                            else
                            {
                                SQLdb.Close();
                            }
                        }
                    }
                }
            }
        }
        public string WriteTeamBoxInsert(NBAdbToolboxHistoric.Team team, int season, string GameID, int MatchupID, int PointsAgainst, NBAdbToolboxHistoric.Lineups lineup)
        {
            string insert = "insert into TeamBox(SeasonID, GameID, TeamID, MatchupID, FGM, FGA, [FG%], FG2M, FG2A, FG3M, FG3A, [FG3%], FTM, FTA, [FT%], ReboundsDefensive, ReboundsOffensive, ReboundsTotal, Assists, Turnovers, Steals, "
                + "Blocks, Points, FoulsPersonal, PointsAgainst, [FG2%], AssistsTurnoverRatio";
            string values = ") values(" + season + ", " + Int32.Parse(GameID) + ", " + team.teamId + ", " + MatchupID + ", "
                + team.statistics.fieldGoalsMade + ", " + team.statistics.fieldGoalsAttempted + ", " + team.statistics.fieldGoalsPercentage + ", " + (team.statistics.fieldGoalsMade - team.statistics.threePointersMade) + ", " + (team.statistics.fieldGoalsAttempted - team.statistics.threePointersAttempted)
                + ", " + team.statistics.threePointersMade + ", " + team.statistics.threePointersAttempted + ", " + team.statistics.threePointersPercentage + ", " + team.statistics.freeThrowsMade + ", " + team.statistics.freeThrowsAttempted + ", " + team.statistics.freeThrowsPercentage
                + ", " + team.statistics.reboundsDefensive + ", " + team.statistics.reboundsOffensive + ", " + team.statistics.reboundsTotal + ", " + team.statistics.assists + ", " + team.statistics.turnovers + ", " + team.statistics.steals + ", " + team.statistics.blocks
                 + ", " + team.statistics.points + ", " + team.statistics.foulsPersonal + ", " + PointsAgainst + ", ";
            if ((double)(team.statistics.fieldGoalsAttempted - team.statistics.threePointersAttempted) != 0)
            {
                values += Math.Round((double)(team.statistics.fieldGoalsMade - team.statistics.threePointersMade) /
                (double)(team.statistics.fieldGoalsAttempted - team.statistics.threePointersAttempted), 4) + ", ";
            }
            else
            {
                values += "0, ";
            }
            if (team.statistics.turnovers > 0)
            {
                values += Math.Round((double)(team.statistics.assists) / (double)(team.statistics.turnovers), 3) + ")\n";
            }
            else
            {
                values += "0)\n";
            }
            return insert + values;
        }
        public string WriteTeamBoxLineupsInsert(NBAdbToolboxHistoric.Team team, int season, string GameID, int MatchupID, int PointsAgainst, NBAdbToolboxHistoric.Lineups lineup)
        {
            int minutes = 0;
            int seconds = 0;
            string insertLi = "insert into TeamBoxLineups(SeasonID, GameID, TeamID, MatchupID, Unit, FGM, FGA, [FG%], FG2M, FG2A, FG3M, FG3A, [FG3%], FTM, FTA, [FT%], ReboundsDefensive, ReboundsOffensive, ReboundsTotal, Assists, Turnovers, Steals, "
                + "Blocks, Points, FoulsPersonal, Minutes, [FG2%], AssistsTurnoverRatio";
            string valuesLi = ") values(" + season + ", " + Int32.Parse(GameID) + ", " + team.teamId + ", " + MatchupID + ", '" + lineup.unit.Substring(0, 1).ToUpper() + lineup.unit.Substring(1) + "', "
                + lineup.fieldGoalsMade + ", " + lineup.fieldGoalsAttempted + ", " + lineup.fieldGoalsPercentage + ", " + (lineup.fieldGoalsMade - lineup.threePointersMade) + ", " + (lineup.fieldGoalsAttempted - lineup.threePointersAttempted)
                + ", " + lineup.threePointersMade + ", " + lineup.threePointersAttempted + ", " + lineup.threePointersPercentage + ", " + lineup.freeThrowsMade + ", " + lineup.freeThrowsAttempted + ", " + lineup.freeThrowsPercentage
                + ", " + lineup.reboundsDefensive + ", " + lineup.reboundsOffensive + ", " + lineup.reboundsTotal + ", " + lineup.assists + ", " + lineup.turnovers + ", " + lineup.steals + ", " + lineup.blocks
                 + ", " + lineup.points + ", " + lineup.foulsPersonal + ", '";

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
                valuesLi += minutes + ":" + seconds + ".00', ";
            }
            else
            {
                valuesLi += lineup.minutes + ".00', ";
            }
            if ((double)(lineup.fieldGoalsAttempted - lineup.threePointersAttempted) != 0)
            {
                valuesLi += Math.Round((double)(lineup.fieldGoalsMade - lineup.threePointersMade) /
                (double)(lineup.fieldGoalsAttempted - lineup.threePointersAttempted), 4) + ", ";
            }
            else
            {
                valuesLi += "0, ";
            }
            if (lineup.turnovers > 0)
            {
                valuesLi += Math.Round((double)(lineup.assists) / (double)(lineup.turnovers), 3) + ")\n";
            }
            else
            {
                valuesLi += "0)\n";
            }
            return insertLi + valuesLi;
        }
        public void TeamBoxUpdate(NBAdbToolboxHistoric.Team team, int season, string GameID, int MatchupID, int PointsAgainst, string procedure, NBAdbToolboxHistoric.Lineups lineup)
        {
            using (SqlCommand TeamBoxUpdate = new SqlCommand(procedure))
            {
                TeamBoxUpdate.Connection = SQLdb;
                TeamBoxUpdate.CommandType = CommandType.StoredProcedure;
                TeamBoxUpdate.Parameters.AddWithValue("@SeasonID", season);
                TeamBoxUpdate.Parameters.AddWithValue("@GameID", Int32.Parse(GameID));
                TeamBoxUpdate.Parameters.AddWithValue("@TeamID", team.teamId);
                TeamBoxUpdate.Parameters.AddWithValue("@MatchupID", MatchupID);
                if (procedure == "TeamBoxLineupUpdateHistoric")
                {
                    int minutes = 0;
                    int seconds = 0;
                    TeamBoxUpdate.Parameters.AddWithValue("@Unit", lineup.unit.Substring(0, 1).ToUpper() + lineup.unit.Substring(1));
                    if(lineup.unit == "bench")
                    {
                        for(int i = 0; i < team.players.Count; i++)
                        {
                            if(team.players[i].position == "" && team.players[i].statistics.minutes != "")
                            {
                                minutes += Int32.Parse(team.players[i].statistics.minutes.Remove(team.players[i].statistics.minutes.IndexOf(":")));
                                seconds += Int32.Parse(team.players[i].statistics.minutes.Substring(team.players[i].statistics.minutes.IndexOf(":") + 1));
                            }
                        }
                        double secondsDiv = (double) seconds % 60;
                        double minutesWhole = Math.Floor((double)(seconds / 60));
                        minutes += (int)(minutesWhole);
                        seconds = (int)secondsDiv;
                        TeamBoxUpdate.Parameters.AddWithValue("@Minutes", minutes + ":" + seconds + ".00");
                    }
                    else
                    {
                        TeamBoxUpdate.Parameters.AddWithValue("@Minutes", lineup.minutes + ".00");
                    } 
                    TeamBoxUpdate.Parameters.AddWithValue("@FGM", lineup.fieldGoalsMade);
                    TeamBoxUpdate.Parameters.AddWithValue("@FGA", lineup.fieldGoalsAttempted);
                    TeamBoxUpdate.Parameters.AddWithValue("@FGpct", lineup.fieldGoalsPercentage);
                    TeamBoxUpdate.Parameters.AddWithValue("@FG2M", lineup.fieldGoalsMade - lineup.threePointersMade);
                    TeamBoxUpdate.Parameters.AddWithValue("@FG2A", lineup.fieldGoalsAttempted - lineup.threePointersAttempted);
                    if ((double)(lineup.fieldGoalsAttempted - lineup.threePointersAttempted) != 0)
                    {
                        TeamBoxUpdate.Parameters.AddWithValue("@FG2pct", 
                        Math.Round((double)(lineup.fieldGoalsMade - lineup.threePointersMade) /
                        (double)(lineup.fieldGoalsAttempted - lineup.threePointersAttempted), 4));
                    }
                    else
                    {
                        TeamBoxUpdate.Parameters.AddWithValue("@FG2pct", 0);
                    }
                    TeamBoxUpdate.Parameters.AddWithValue("@FG3M", lineup.threePointersMade);
                    TeamBoxUpdate.Parameters.AddWithValue("@FG3A", lineup.threePointersAttempted);
                    TeamBoxUpdate.Parameters.AddWithValue("@FG3pct", lineup.threePointersPercentage);
                    TeamBoxUpdate.Parameters.AddWithValue("@FTM", lineup.freeThrowsMade);
                    TeamBoxUpdate.Parameters.AddWithValue("@FTA", lineup.freeThrowsAttempted);
                    TeamBoxUpdate.Parameters.AddWithValue("@FTpct", lineup.freeThrowsPercentage);
                    TeamBoxUpdate.Parameters.AddWithValue("@RebD", lineup.reboundsDefensive);
                    TeamBoxUpdate.Parameters.AddWithValue("@RebO", lineup.reboundsOffensive);
                    TeamBoxUpdate.Parameters.AddWithValue("@RebT", lineup.reboundsTotal);
                    TeamBoxUpdate.Parameters.AddWithValue("@Assists", lineup.assists);
                    TeamBoxUpdate.Parameters.AddWithValue("@Turnovers", lineup.turnovers);
                    if (lineup.turnovers > 0)
                    {
                        TeamBoxUpdate.Parameters.AddWithValue("@AtoR", Math.Round((double)(lineup.assists) / (double)(lineup.turnovers), 3));
                    }
                    else
                    {
                        TeamBoxUpdate.Parameters.AddWithValue("@AtoR", 99);
                    }
                    TeamBoxUpdate.Parameters.AddWithValue("@Steals", lineup.steals);
                    TeamBoxUpdate.Parameters.AddWithValue("@Blocks", lineup.blocks);
                    TeamBoxUpdate.Parameters.AddWithValue("@Points", lineup.points);
                    TeamBoxUpdate.Parameters.AddWithValue("@FoulsPersonal", lineup.foulsPersonal);
                }
                else if (procedure == "TeamBoxUpdateHistoric")
                {
                    TeamBoxUpdate.Parameters.AddWithValue("@FGM", team.statistics.fieldGoalsMade);
                    TeamBoxUpdate.Parameters.AddWithValue("@FGA", team.statistics.fieldGoalsAttempted);
                    TeamBoxUpdate.Parameters.AddWithValue("@FGpct", team.statistics.fieldGoalsPercentage);
                    TeamBoxUpdate.Parameters.AddWithValue("@FG2M", team.statistics.fieldGoalsMade - team.statistics.threePointersMade);
                    TeamBoxUpdate.Parameters.AddWithValue("@FG2A", team.statistics.fieldGoalsAttempted - team.statistics.threePointersAttempted);
                    if ((double)(team.statistics.fieldGoalsAttempted - team.statistics.threePointersAttempted) != 0)
                    {
                        TeamBoxUpdate.Parameters.AddWithValue("@FG2pct", 
                        Math.Round((double)(team.statistics.fieldGoalsMade - team.statistics.threePointersMade) /
                        (double)(team.statistics.fieldGoalsAttempted - team.statistics.threePointersAttempted), 4));
                    }
                    else
                    {
                        TeamBoxUpdate.Parameters.AddWithValue("@FG2pct", 0);
                    }
                    TeamBoxUpdate.Parameters.AddWithValue("@FG3M", team.statistics.threePointersMade);
                    TeamBoxUpdate.Parameters.AddWithValue("@FG3A", team.statistics.threePointersAttempted);
                    TeamBoxUpdate.Parameters.AddWithValue("@FG3pct", team.statistics.threePointersPercentage);
                    TeamBoxUpdate.Parameters.AddWithValue("@FTM", team.statistics.freeThrowsMade);
                    TeamBoxUpdate.Parameters.AddWithValue("@FTA", team.statistics.freeThrowsAttempted);
                    TeamBoxUpdate.Parameters.AddWithValue("@FTpct", team.statistics.freeThrowsPercentage);
                    TeamBoxUpdate.Parameters.AddWithValue("@RebD", team.statistics.reboundsDefensive);
                    TeamBoxUpdate.Parameters.AddWithValue("@RebO", team.statistics.reboundsOffensive);
                    TeamBoxUpdate.Parameters.AddWithValue("@RebT", team.statistics.reboundsTotal);
                    TeamBoxUpdate.Parameters.AddWithValue("@Assists", team.statistics.assists);
                    TeamBoxUpdate.Parameters.AddWithValue("@Turnovers", team.statistics.turnovers);
                    if (team.statistics.turnovers > 0)
                    {
                        TeamBoxUpdate.Parameters.AddWithValue("@AtoR", Math.Round((double)(team.statistics.assists) / (double)(team.statistics.turnovers), 3));
                    }
                    else
                    {
                        TeamBoxUpdate.Parameters.AddWithValue("@AtoR", 99);
                    }
                    TeamBoxUpdate.Parameters.AddWithValue("@Steals", team.statistics.steals);
                    TeamBoxUpdate.Parameters.AddWithValue("@Blocks", team.statistics.blocks);
                    TeamBoxUpdate.Parameters.AddWithValue("@Points", team.statistics.points);
                    TeamBoxUpdate.Parameters.AddWithValue("@FoulsPersonal", team.statistics.foulsPersonal);
                    TeamBoxUpdate.Parameters.AddWithValue("@PointsAgainst", PointsAgainst);
                }
                SQLdb.Open();
                TeamBoxUpdate.ExecuteScalar();
                SQLdb.Close();
            }

        }
        #endregion

        //PlayByPlay methods
        #region PlayByPlay Methods
        public string insertPBPString = "";
        public void PlayByPlayStaging(NBAdbToolboxHistoric.PlayByPlay pbp, int season)
        {
            insertPBPString = "";
            string instructions = PlayByPlayCheck(pbp, season, "PlayByPlayCheckHistorical");
            int actions = pbp.actions.Count;
            int start = 0;
            if (instructions.Contains("Update"))
            {
                start = Int32.Parse(instructions.Substring(instructions.IndexOf("-") + 1));
                instructions = "Insert";
            }
            for (int i = start; i < actions; i++)
            {
                if (instructions == "Insert")
                {
                    //PlayByPlayInsert(pbp.actions[i], season, Int32.Parse(pbp.gameId), "PlayByPlayInsertHistorical");
                    PlayByPlayInsertString(pbp.actions[i], season, Int32.Parse(pbp.gameId));
                }
            }
            insertPBPString = insertPBPString.Replace("'',", "");
            //if(insertString != "") 
            //{
            //    _ = Task.Run(() =>
            //    {
            //        try
            //        {
            //            using (SqlCommand PlayByPlayInsert = new SqlCommand(insertString))
            //            {
            //                PlayByPlayInsert.Connection = SQLdb;
            //                PlayByPlayInsert.CommandType = CommandType.Text;
            //                SQLdb.Open();
            //                PlayByPlayInsert.ExecuteNonQuery();
            //                SQLdb.Close();
            //            }
                        
            //        }
            //        catch (Exception e)
            //        {
            //            // Optional: log the error or queue it for retry
            //        }
            //    });
            //}

        }
        public string PlayByPlayCheck(NBAdbToolboxHistoric.PlayByPlay pbp, int season, string procedure)
        {
            using (SqlCommand PlayByPlayCheck = new SqlCommand(procedure))
            {
                PlayByPlayCheck.Connection = SQLdb;
                PlayByPlayCheck.CommandType = CommandType.StoredProcedure;
                PlayByPlayCheck.Parameters.AddWithValue("@SeasonID", season);
                PlayByPlayCheck.Parameters.AddWithValue("@GameID", Int32.Parse(pbp.gameId));
                using (SqlDataAdapter sTeamSearch = new SqlDataAdapter())
                {
                    PlayByPlayCheck.Connection = SQLdb;
                    sTeamSearch.SelectCommand = PlayByPlayCheck;
                    SQLdb.Open();
                    SqlDataReader reader = PlayByPlayCheck.ExecuteReader();
                    if (!reader.HasRows)
                    {
                        SQLdb.Close();
                        return "Insert"; //True means we insert
                    }
                    else
                    {
                        reader.Read();
                        if (reader.GetInt32(2) != pbp.actions.Count)
                        {
                            SQLdb.Close();
                            return "Update  -" + (reader.GetInt32(2));
                        }
                        else
                        {
                            SQLdb.Close();
                            return "Skip";
                        }
                    }
                }
            }
        }
        public void PlayByPlayInsert(NBAdbToolboxHistoric.Action action, int season, int GameID, string procedure)
        {
            using (SqlCommand PlayByPlayInsert = new SqlCommand(procedure))
            {
                PlayByPlayInsert.Connection = SQLdb;
                PlayByPlayInsert.CommandType = CommandType.StoredProcedure;
                PlayByPlayInsert.Parameters.AddWithValue("@SeasonID", season);
                PlayByPlayInsert.Parameters.AddWithValue("@GameID", GameID);
                PlayByPlayInsert.Parameters.AddWithValue("@ActionID", action.actionId);
                PlayByPlayInsert.Parameters.AddWithValue("@ActionNumber", action.actionNumber);
                PlayByPlayInsert.Parameters.AddWithValue("@Qtr", action.period);
                PlayByPlayInsert.Parameters.AddWithValue("@Clock", action.clock);
                if(action.scoreAway == "")                                                          //Away Score
                {                                                                                   //Away Score
                    PlayByPlayInsert.Parameters.AddWithValue("@ScoreAway", SqlInt32.Null);          //Away Score
                }                                                                                   //Away Score
                else                                                                                //Away Score
                {                                                                                   //Away Score
                    PlayByPlayInsert.Parameters.AddWithValue("@ScoreAway", action.scoreAway);       //Away Score
                }
                if (action.scoreHome == "")                                                         //Home Score
                {                                                                                   //Home Score
                    PlayByPlayInsert.Parameters.AddWithValue("@ScoreHome", SqlInt32.Null);          //Home Score
                }                                                                                   //Home Score
                else                                                                                //Home Score
                {                                                                                   //Home Score
                    PlayByPlayInsert.Parameters.AddWithValue("@ScoreHome", action.scoreHome);       //Home Score
                }
                if (action.teamId != 0)                                                             //TeamID and Tricode
                {                                                                                   //TeamID and Tricode
                    PlayByPlayInsert.Parameters.AddWithValue("@TeamID", action.teamId);             //TeamID and Tricode
                    PlayByPlayInsert.Parameters.AddWithValue("@Tricode", action.teamTricode);       //TeamID and Tricode
                }                                                                                   //TeamID and Tricode
                else                                                                                //TeamID and Tricode
                {                                                                                   //TeamID and Tricode
                    PlayByPlayInsert.Parameters.AddWithValue("@TeamID", SqlInt32.Null);             //TeamID and Tricode
                    PlayByPlayInsert.Parameters.AddWithValue("@Tricode", SqlString.Null);           //TeamID and Tricode
                }

                if (action.personId == 0)                                                           //PlayerID
                {                                                                                   //PlayerID
                    PlayByPlayInsert.Parameters.AddWithValue("@PlayerID", SqlInt32.Null);           //PlayerID
                }                                                                                   //PlayerID
                else                                                                                //PlayerID
                {                                                                                   //PlayerID
                    PlayByPlayInsert.Parameters.AddWithValue("@PlayerID", action.personId);         //PlayerID
                }
                PlayByPlayInsert.Parameters.AddWithValue("@Description", action.description);
                PlayByPlayInsert.Parameters.AddWithValue("@SubType", action.subType);
                PlayByPlayInsert.Parameters.AddWithValue("@IsFieldGoal", action.isFieldGoal);
                PlayByPlayInsert.Parameters.AddWithValue("@ShotResult", action.shotResult);
                PlayByPlayInsert.Parameters.AddWithValue("@ShotValue", action.shotValue);
                PlayByPlayInsert.Parameters.AddWithValue("@ActionType", action.actionType);
                PlayByPlayInsert.Parameters.AddWithValue("@ShotDistance", action.shotDistance);
                if (action.isFieldGoal == 1)
                {
                    PlayByPlayInsert.Parameters.AddWithValue("@XLegacy", action.xLegacy);
                    PlayByPlayInsert.Parameters.AddWithValue("@YLegacy", action.yLegacy);
                }
                else
                {
                    PlayByPlayInsert.Parameters.AddWithValue("@XLegacy", SqlDouble.Null);
                    PlayByPlayInsert.Parameters.AddWithValue("@YLegacy", SqlDouble.Null);
                }
                PlayByPlayInsert.Parameters.AddWithValue("@Location", action.location);

                SQLdb.Open();
                PlayByPlayInsert.ExecuteScalar();
                SQLdb.Close();
            }
        }
        public void PlayByPlayInsertString(NBAdbToolboxHistoric.Action action, int season, int GameID)
        {
            string insert = "insert into PlayByPlay (SeasonID, GameID, ActionID, ActionNumber, Qtr, Clock, ";
            string values = ") values(" + season + ", " + GameID + ", " + action.actionId + ", " + action.actionNumber + ", " + action.period + ", replace(replace(replace('" + action.clock + "', 'PT', ''), 'M', ':'), 'S', ''), ";
            //+ action.isFieldGoal + ", '" 
            //+ action.shotResult + "', " 
            //+ action.shotValue + ", '" 
            //+ action.actionType + "', " 
            //+ action.shotDistance + ", '" 
            //+ action.location + "', ";

            if (action.description == "" && action.actionType != "" && action.actionType != " ")
            {
                insert += "Description, ";
                values += "'" + action.actionType + "', ";
            }
            else if(action.description != "" && action.description != " ")
            {
                insert += "Description, ";
                values += "'" + action.description.Replace("'", "''") + "', " ;
            }
            if (action.subType != "Unknown" && action.subType != "")
            {
                insert += "SubType, ";
                values += "'" + action.subType + "', ";
            }
            if(action.description.Length > 3)
            {
                if (action.description.Substring(action.description.Length - 4) == "STL)")
                {
                    insert += "ActionType, ";
                    values += "'Steal', ";
                }
                else if (action.description.Substring(action.description.Length - 4) == "BLK)")
                {
                    insert += "ActionType, ";
                    values += "'Block', ";
                }
                else
                {
                    insert += "ActionType, ";
                    values += "'" + action.actionType + "', ";
                }
            }
            else if(action.actionType != "" && action.actionType != " ")
            {
                insert += "ActionType, ";
                values += "'" + action.actionType + "', ";
            }


            if (action.location != "")
            {
                insert += "Location, ";
                values += "'" + action.location + "', ";
            }

            //Field Goals
            if(action.isFieldGoal == 1)
            {
                insert += "IsFieldGoal, ";
                values += action.isFieldGoal + ", ";
                if(action.shotResult == "Made")                 //If Make
                {
                    insert += "ShotType, ";                         //Shot Type
                    values += "'FG" + action.shotValue + "M', ";    //Shot Type
                    insert += "PtsGenerated, ";                     //PtsGenerated
                    values += action.shotValue + ", ";              //PtsGenerated
                }
                else if (action.shotResult == "Missed")         //If Miss
                {
                    insert += "ShotType, ";                         //Shot Type
                    values += "'FG" + action.shotValue + "A', ";    //Shot Type
                    insert += "PtsGenerated, ";                     //PtsGenerated
                    values += "0, ";                                //PtsGenerated
                }
                insert += "ShotResult, ShotValue, ShotDistance, ";
                values += "'" + action.shotResult + "', " + action.shotValue + ", " + action.shotDistance + ", ";
            }
            else if(action.actionType == "Free Throw")
            {
                if (action.description.Substring(action.description.Length - 4) == "PTS)")
                {
                    insert += "ShotType, PtsGenerated, ShotResult, ShotValue, ";
                    values += "'FTM', 1, 'Made', 1, ";
                }
                else if(action.description.Substring(0, 4) == "MISS")
                {
                    insert += "ShotType, PtsGenerated, ShotResult, ShotValue, ";
                    values += "'FTA', 0, 'Missed', 1, ";
                }
            }


            if (action.scoreAway != "")
            {
                insert += "ScoreAway, ";
                values += action.scoreAway + ", ";
            }
            if (action.scoreHome != "")
            {
                insert += "ScoreHome, ";
                values += action.scoreHome + ", ";
            }
            if (action.teamId != 0)
            {
                insert += "TeamID, Tricode, ";
                values += action.teamId + ", '" + action.teamTricode + "', ";
            }
            if (action.personId != 0)
            {
                insert += "PlayerID, ";
                values += action.personId + ", ";
            }
            if (action.isFieldGoal == 1)
            {
                insert += "Xlegacy, Ylegacy, ";
                values += action.xLegacy + ", " + action.yLegacy + ", ";
            }
            insert = insert.Remove(insert.Length - ", ".Length);
            values = values.Remove(values.Length - ", ".Length) + ") ";

            insertPBPString += insert + values + "\n";
        }


        #endregion
        #endregion



        //Current Data
        #region Current Data Up to Date

        //Declarations
        #region Declarations
        public bool currentTeamsDone = false;
        public HashSet<(int SeasonID, int PlayerID)> currentTeamList = new HashSet<(int, int)>();
        public HashSet<(int SeasonID, int PlayerID)> currentPlayerList = new HashSet<(int, int)>();
        public HashSet<(int SeasonID, int PlayerID)> currentOfficialList = new HashSet<(int, int)>();
        public HashSet<(int SeasonID, int PlayerID)> currentArenaList = new HashSet<(int, int)>();
        public string currentBigInsert = "";
        public string currentFirstInsert = "";

        public string currentPlayer = "";
        public string currentPlayerBox = "";
        public string currentTeamBox = "";
        public string currentPlayByPlay = "";
        #endregion
        public async Task CurrentGameData(int GameID, int season, string sender)
        {
            bool doBox = true;
            bool doPBP = true;
            bool useHistoric = false;
            string missingData = "";

            //BoxScore
            ImageDriver(25);
            lblCurrentGameCount.Invoke((MethodInvoker)(() =>
            {
                lblCurrentGameCount.Text = GameID.ToString();
            }));

            try
            {
                rootCPBP = await currentDataPBP.GetJSON(GameID, season);
                if (rootCPBP.game == null)
                {
                    doPBP = false;
                    missingData += "insert into util.MissingData values(" + season + ", " + GameID + ", 'Current', 'PlayByPlay', 'No file available from NBA')\n";
                }
            }
            catch (WebException ex)
            {
                doPBP = false;
                missingData += "insert into util.MissingData values(" + season + ", " + GameID + ", 'Current', 'PlayByPlay', 'No file available from NBA')\n";
            }

            try
            {
                rootC = await currentData.GetJSON(GameID, season);
                if(rootC.game == null)
                {
                    doBox = false;
                    useHistoric = true;
                    missingData += "insert into util.MissingData values(" + season + ", " + GameID + ", 'Current', 'Box', 'No File available from NBA')\n";
                }
            }
            catch (WebException ex)
            {
                doBox = false;
                useHistoric = true;
                missingData += "insert into util.MissingData values(" + season + ", " + GameID + ", 'Current', 'Box', 'No File available from NBA')\n";
            }

            ImageDriver(25);
            if (doPBP)
            {
                _ = Task.Run(() =>
                {
                    try
                    {
                        CurrentPlayByPlayStaging(rootCPBP.game, season, "New");
                    }
                    catch
                    {
                        missingData += "insert into util.MissingData values(" + season + ", " + GameID + ", 'Current', 'PlayByPlay', 'JSON file formatting - NBA pls fix')\n";
                    }
                });
            }
            if (doBox)
            {
                //_ = Task.Run(() =>
                //{
                    try
                    {
                        CurrentInsertStaging(rootC.game, season);
                    }
                    catch
                    {
                        useHistoric = true;
                        missingData += "insert into util.MissingData values(" + season + ", " + GameID + ", 'Current', 'Box', 'JSON file formatting - NBA pls fix')\n";
                    }
                //});
            }
            string insertString = currentFirstInsert; //missingData + currentBoxUpdate + currentPBPInsert
            currentFirstInsert = "";              //clear for next batch

            // Kick off background DB update
            //_ = Task.Run(() =>
            //{
                try
                {
                    SqlConnection firstInsert = new SqlConnection(cString);
                    using (firstInsert)
                    {
                        using (SqlCommand insert = new SqlCommand(insertString))
                        {
                            insert.Connection = firstInsert;
                            insert.CommandType = CommandType.Text;
                            firstInsert.Open();
                            insert.ExecuteNonQuery();
                            firstInsert.Close();
                        }
                    }
                }
                catch (Exception e)
                {
                    // Optional: log the error or queue it for retry
                }
            //});

            string teamPlayerBoxPlayByPlay = currentTeamBox + currentPlayerBox + currentPlayByPlay;
            currentTeamBox = "";
            currentPlayerBox = "";
            currentPlayByPlay = "";
            _ = Task.Run(() =>
            {
                SqlConnection insertPBP = new SqlConnection(cString);
                try
                {
                    using (insertPBP)
                    {
                        using (SqlCommand PBPInsert = new SqlCommand(teamPlayerBoxPlayByPlay))
                        {
                            PBPInsert.Connection = insertPBP;
                            PBPInsert.CommandType = CommandType.Text;
                            insertPBP.Open();
                            PBPInsert.ExecuteNonQuery();
                            insertPBP.Close();
                        }
                    }
                }
                catch (Exception e)
                {

                }
            });

        }
        public void CurrentInsertStaging(NBAdbToolboxCurrent.Game game, int season)
        {
            string firstInsert = "";
            currentFirstInsert = "";
            currentPlayerBox = "";
            currentTeamBox = "";
            if (!currentTeamsDone)
            {
                firstInsert += CurrentTeamStaging(game, season);
            }
            if (!currentArenaList.Contains((season, game.arena.arenaId)))
            {
                firstInsert += CurrentArenaCheck(game.arena, game.homeTeam.teamId, season);
            }
            //Officials
            Dictionary<int, string> officials = new Dictionary<int, string>();
            foreach (NBAdbToolboxCurrent.Official official in game.officials)
            {
                if (!currentOfficialList.Contains((season, official.personId)))
                {
                    firstInsert += CurrentOfficialCheck(official, season);
                }
                officials.Add(official.personId, official.assignment);
            }

            firstInsert += CurrentGameCheck(game, season, officials);

            firstInsert += CurrentPlayerStaging(game, season);
            currentFirstInsert = firstInsert;
            _ = Task.Run(() =>
            {
                CurrentTeamBoxStaging(game, season);
            });
        }

        //Current Teams
        #region Current Teams
        public string CurrentTeamStaging(NBAdbToolboxCurrent.Game game, int season)
        {
            string firstInsert = "";
            if (!currentTeamList.Contains((season, game.homeTeam.teamId)))
            {
                firstInsert += CurrentTeamCheck(game.homeTeam, season);
            }
            if (!currentTeamList.Contains((season, game.awayTeam.teamId)))
            {
                firstInsert += CurrentTeamCheck(game.awayTeam, season);
            }
            return firstInsert;
        }
        public string CurrentTeamCheck(NBAdbToolboxCurrent.Team team, int season)
        {
            SQLdb = new SqlConnection(cString);
            using (SqlCommand TeamSearch = new SqlCommand("TeamCheck"))
            {
                TeamSearch.CommandType = CommandType.StoredProcedure;
                TeamSearch.Parameters.AddWithValue("@TeamID", team.teamId);
                TeamSearch.Parameters.AddWithValue("@SeasonID", season);
                using (SqlDataAdapter sTeamSearch = new SqlDataAdapter())
                {
                    TeamSearch.Connection = SQLdb;
                    sTeamSearch.SelectCommand = TeamSearch;
                    SQLdb.Open();
                    SqlDataReader reader = TeamSearch.ExecuteReader();
                    if (reader.HasRows)
                    {//If we have a result
                        SQLdb.Close();
                    }
                    else
                    { //If no result, send to TeamInsert to insert into DB
                        SQLdb.Close();
                        currentTeamList.Add((season, team.teamId));
                        currentBigInsert += "insert into Team(SeasonID, TeamID, City, Name, Tricode, FullName) values(" + season + ", " + team.teamId + ", '" + team.teamCity + "', '" + team.teamName + "', '" + team.teamTricode + "', '(" + team.teamTricode + ") " +
                            team.teamCity + " " + team.teamName + "')\n";
                        if(currentTeamList.Count == 30)
                        {
                            currentTeamsDone = true;
                        }
                    }
                }
            }
            return "insert into Team(SeasonID, TeamID, City, Name, Tricode, FullName) values(" + season + ", " + team.teamId + ", '" + team.teamCity + "', '" + team.teamName + "', '" + team.teamTricode + "', '(" + team.teamTricode + ") " +
                team.teamCity + " " + team.teamName + "')\n";
        }
        #endregion

        //Current Arenas
        #region Current Arenas
        public string CurrentArenaCheck(NBAdbToolboxCurrent.Arena arena, int teamID, int season)
        {
            string firstInsert = "";
            SQLdb = new SqlConnection(cString);
            using (SqlCommand TeamSearch = new SqlCommand("ArenaCheck"))
            {
                TeamSearch.CommandType = CommandType.StoredProcedure;
                TeamSearch.Parameters.AddWithValue("@ArenaID", arena.arenaId);
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
                        currentArenaList.Add((season, arena.arenaId));
                        currentBigInsert += "insert into Arena(SeasonID, ArenaID, TeamID, City, Country, Name, State, Timezone) values(" + season + ", " + arena.arenaId + ", " + teamID + ", '" + arena.arenaCity + "', '" + arena.arenaCountry + "', '" + arena.arenaName
                        + "', '" + arena.arenaState + "', '" + arena.arenaTimezone + "')\n";
                    }
                }
            }
            return "insert into Arena(SeasonID, ArenaID, TeamID, City, Country, Name, State, Timezone) values(" + season + ", " + arena.arenaId + ", " + teamID + ", '" + arena.arenaCity + "', '" + arena.arenaCountry + "', '" + arena.arenaName
                        + "', '" + arena.arenaState + "', '" + arena.arenaTimezone + "')\n";
        }
        #endregion

        //Current Officials
        #region Current Officials
        public string CurrentOfficialCheck(NBAdbToolboxCurrent.Official official, int season)
        {
            SQLdb = new SqlConnection(cString);
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
                        officialList.Add((season, official.personId));
                        currentBigInsert += "insert into Official values(" + season + ", " + official.personId + ", '" + official.name + "', '" + official.jerseyNum + "')\n";
                    }
                }
            }
            return "insert into Official values(" + season + ", " + official.personId + ", '" + official.name + "', '" + official.jerseyNum + "')\n";
        }
        #endregion

        //Current Games
        #region Current Games
        public string insertExt = "";
        public string valuesExt = "";
        public string CurrentGameCheck(NBAdbToolboxCurrent.Game game, int season, Dictionary<int, string> officials)
        {
            string insert = "Insert into Game(SeasonID, GameID, Date, HomeID, HScore, AwayID, AScore, Datetime, ";
            string values = "";
            insertExt = "Insert into GameExt(SeasonID, GameID, Status, Attendance, Sellout, ";
            valuesExt = "";
            SQLdb = new SqlConnection(cString);
            using (SqlCommand GameCheck = new SqlCommand("GameCheck"))
            {
                GameCheck.CommandType = CommandType.StoredProcedure;
                GameCheck.Parameters.AddWithValue("@GameID", game.gameId);
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
                        SqlDateTime datetime = SqlDateTime.Parse(game.gameTimeUTC);
                        values = ") values(" + season + ", " + game.gameId + ", '" + SqlDateTime.Parse(game.gameEt.Remove(game.gameEt.IndexOf('T'))) + "', " + game.homeTeam.teamId + ", " + game.homeTeam.score
                            + ", " + game.awayTeam.teamId + ", " + game.awayTeam.score + ", '" + datetime + "', ";
                        insert += "WinnerID, WScore, LoserID, Lscore, GameType, SeriesID";
                        if (game.homeTeam.score > game.awayTeam.score)
                        {
                            values += game.homeTeam.teamId + ", " + game.homeTeam.score + ", " + game.awayTeam.teamId + ", " + game.awayTeam.score + ", ";
                        }
                        else
                        {
                            values += game.awayTeam.teamId + ", " + game.awayTeam.score + ", " + game.homeTeam.teamId + ", " + game.homeTeam.score + ", ";
                        }
                        if (game.gameId.Substring(0, 1) == "2")
                        {
                            values += "'RS', null)";
                        }
                        else
                        {
                            values += "'PS', 'placeholder')";
                        }
                        gameInsert = insert + values + "\n";
                        valuesExt = ") values(" + season + ", " + game.gameId + ", '" + game.gameStatusText + "', " + game.attendance + ", " + game.sellout + ", ";
                        foreach (KeyValuePair<int, string> kvp in officials)
                        {
                            if(kvp.Value == "OFFICIAL1")
                            {
                                insertExt += "OfficialID, ";
                                valuesExt += kvp.Key + ", ";
                            }
                            if (kvp.Value == "OFFICIAL2")
                            {
                                insertExt += "Official2ID, ";
                                valuesExt += kvp.Key + ", ";
                            }
                            if (kvp.Value == "OFFICIAL3")
                            {
                                insertExt += "Official3ID, ";
                                valuesExt += kvp.Key + ", ";
                            }
                            if (kvp.Value == "ALTERNATE")
                            {
                                insertExt += "OfficialAlternateID, ";
                                valuesExt += kvp.Key + ", ";
                            }
                        }
                        
                    }
                    else
                    {
                        if (reader.GetInt32(5) != game.homeTeam.score || reader.GetInt32(7) != game.awayTeam.score)
                        {
                            SQLdb.Close();
                            //GameUpdate(game, season, sender);
                        }
                        else
                        {
                            SQLdb.Close();
                        }
                    }
                }
            }
            return insert + values + "\n" + insertExt.Remove(insertExt.Length - ", ".Length) + valuesExt.Remove(valuesExt.Length - ", ".Length) + ")" + "\n";
        }
        #endregion

        //Current Players and PlayerBox
        #region Current Players and PlayerBox
        public string CurrentPlayerStaging(NBAdbToolboxCurrent.Game game, int season)
        {
            string pInsert = "";
            //foreach(NBAdbToolboxCurrent.Team team in game.homeTeam, game.awayTeam)
            foreach (NBAdbToolboxCurrent.Team team in new[] { game.homeTeam, game.awayTeam })
            {
                int MatchupID = (team == game.homeTeam) ? game.awayTeam.teamId : game.homeTeam.teamId;
                pInsert += CurrentPlayerInitiator(game, team, season, MatchupID);
            }
            return pInsert;
        }
        public string CurrentPlayerInitiator(NBAdbToolboxCurrent.Game game, NBAdbToolboxCurrent.Team team, int season, int MatchupID)
        {
            string pInsert = "";
            foreach (NBAdbToolboxCurrent.Player player in team.players)
            {
                if (!playerList.Contains((season, player.personId)))
                {
                    pInsert += CurrentPlayerCheck(player, season);
                }
                _ = Task.Run(() =>
                {
                    CurrentPlayerBoxCheck(game, player, team.teamId, MatchupID, season);
                });
            }

            return pInsert;
        }
        public string CurrentPlayerCheck(NBAdbToolboxCurrent.Player player, int season)
        {
            string pInsert = "";
            SQLdb = new SqlConnection(cString);
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
                        pInsert = "Insert into Player values(" + season + ", " + player.personId + ", '" + player.firstName.Replace("'", "''") + " " + player.familyName.Replace("'", "''") + "', '" + player.jerseyNum + "', ";
                        SQLdb.Close();
                        playerList.Add((season, player.personId));
                        if (player.position != null && player.position != "")
                        {
                            pInsert += "'" + player.position + "')\n";
                        }
                        else
                        {
                            pInsert += "null)\n";
                        }
                        //playerInsert += pInsert;
                    }
                    else
                    {
                        SQLdb.Close();
                    }
                }
            }
            return pInsert;
        }

        public void CurrentPlayerBoxCheck(NBAdbToolboxCurrent.Game game, NBAdbToolboxCurrent.Player player, int TeamID, int MatchupID, int season)
        {
            SqlConnection pBox = new SqlConnection(cString);
            using (pBox)
            {
                using (SqlCommand PlayerBoxCheck = new SqlCommand("PlayerBoxCheck"))
                {
                    PlayerBoxCheck.Connection = pBox;
                    PlayerBoxCheck.CommandType = CommandType.StoredProcedure;
                    PlayerBoxCheck.Parameters.AddWithValue("@SeasonID", season);
                    PlayerBoxCheck.Parameters.AddWithValue("@GameID", Int32.Parse(game.gameId));
                    PlayerBoxCheck.Parameters.AddWithValue("@TeamID", TeamID);
                    PlayerBoxCheck.Parameters.AddWithValue("@MatchupID", MatchupID);
                    PlayerBoxCheck.Parameters.AddWithValue("@PlayerID", player.personId);
                    using (SqlDataAdapter sTeamSearch = new SqlDataAdapter())
                    {
                        PlayerBoxCheck.Connection = pBox;
                        sTeamSearch.SelectCommand = PlayerBoxCheck;
                        pBox.Open();
                        SqlDataReader reader = PlayerBoxCheck.ExecuteReader();
                        if (!reader.HasRows)
                        {
                            pBox.Close();
                            CurrentPlayerBoxInsert(player, season, Int32.Parse(game.gameId), TeamID, MatchupID);
                        }
                        else
                        {
                            reader.Read();
                            if (reader.GetString(4) != player.statistics.minutes && reader.GetString(4) != "0" && reader.GetString(4) != "")
                            {
                                pBox.Close();
                                //CurrentPlayerBoxUpdate(player, season, Int32.Parse(game.gameId), TeamID, MatchupID);
                            }
                            else
                            {
                                pBox.Close();
                            }
                        }
                    }
                }
            }
        }
        public void CurrentPlayerBoxUpdate(NBAdbToolboxCurrent.Player player, int season, int GameID, int TeamID, int MatchupID)
        {
            string updateSL = "update StartingLineups set ";
            string updatePB = "update PlayerBox set ";

            if (player.starter == "1")
            {
                updateSL += "Unit = 'Starters', ";
                updatePB += "Starter = 1, ";
            }
            else
            {
                updateSL += "Unit = 'Bench', ";
                updatePB += "Starter = 0, ";
            }
            if (player.position != "" && player.position != null)
            {
                updateSL += "Position = '" + player.position + "', ";
                updatePB += "Position = '" + player.position + "', ";
            }
            updatePB += "Status = '" + player.status + "', ";
            if (player.notPlayingReason != "" && player.notPlayingReason != null)
            {
                updatePB += "StatusReason = '" + player.notPlayingReason.Replace("'", "''") + "', ";
                if (player.notPlayingDescription != "" && player.notPlayingDescription != null)
                {
                    updatePB += "StatusDescription = '" + player.notPlayingDescription.Replace("'", "''") + "', ";
                }
            }
            //Minutes Calc
            double sec = double.Parse(player.statistics.minutes.Substring(player.statistics.minutes.IndexOf("M") + 1, 5));
            sec = sec / 60;
            double minCalc = double.Parse(player.statistics.minutes.Substring(2, 2));
            minCalc = Math.Round(minCalc + sec, 2);
            updatePB += "MinutesCalculated = " + minCalc + ", "
                + "BlocksReceived = " + player.statistics.blocksReceived + ", "
                + "Plus = " + player.statistics.plus + ", "
                + "Minus = " + player.statistics.minus + ", "
                + "PlusMinusPoints = " + player.statistics.plusMinusPoints + ", "
                + "PointsFastBreak = " + player.statistics.pointsFastBreak + ", "
                + "PointsInThePaint = " + player.statistics.pointsInThePaint + ", "
                + "PointsSecondChance = " + player.statistics.pointsSecondChance + ", "
                + "FoulsOffensive = " + player.statistics.foulsOffensive + ", "
                + "FoulsDrawn = " + player.statistics.foulsDrawn + ", "
                + "FoulsTechnical = " + player.statistics.foulsTechnical;


            string where = " where SeasonID = " + season + " and GameID = " + GameID + " and TeamID = " + TeamID + " and MatchupID = " + MatchupID + " and PlayerID = " + player.personId;

            currentBoxUpdate += (updateSL + where + "\n" + updatePB + where + "\n").Replace(",  where", " where").Replace(", where", " where");
        }
        public void CurrentPlayerBoxInsert(NBAdbToolboxCurrent.Player player, int season, int GameID, int TeamID, int MatchupID)
        {
            string insert = "insert into PlayerBox(SeasonID, GameID, TeamID, MatchupID, PlayerID, FGM, FGA, [FG%], FG2M, FG2A, [FG2%], FG3M, FG3A, [FG3%], FTM, FTA, [FT%], " +
                "ReboundsDefensive, ReboundsOffensive, ReboundsTotal, Assists, Turnovers, Steals, Blocks, Points, FoulsPersonal, ";
            string values = ") values(" + season + ", " + GameID + ", " + TeamID + ", " + MatchupID + ", " + player.personId + ", " + player.statistics.fieldGoalsMade + ", " + player.statistics.fieldGoalsAttempted
                    + ", " + player.statistics.fieldGoalsPercentage
                    + ", " + player.statistics.twoPointersMade + ", " + player.statistics.twoPointersAttempted + ", " + player.statistics.twoPointersPercentage
                    + ", " + player.statistics.threePointersMade + ", " + player.statistics.threePointersAttempted + ", " + player.statistics.threePointersPercentage
                    + ", " + player.statistics.freeThrowsMade + ", " + player.statistics.freeThrowsAttempted + ", " + player.statistics.freeThrowsPercentage + ", " + player.statistics.reboundsDefensive
                    + ", " + player.statistics.reboundsOffensive + ", " + player.statistics.reboundsTotal + ", " + player.statistics.assists + ", " + player.statistics.turnovers
                    + ", " + player.statistics.steals + ", " + player.statistics.blocks + ", " + player.statistics.points + ", " + player.statistics.foulsPersonal + ", ";

            double sec = 0;
            sec = double.Parse(player.statistics.minutes.Substring(player.statistics.minutes.IndexOf("M") + 1, 5));
            sec = sec / 60;
            double minCalc = 0;
            minCalc = double.Parse(player.statistics.minutes.Substring(2, 2));
            minCalc = Math.Round(minCalc + sec, 2);
            if (player.statistics.minutes != "")
            {
                insert += "Minutes, ";
                values += "'" + player.statistics.minutes.Replace("PT", "").Replace("M", ":").Replace("S", "") + "', ";
                if (player.statistics.plusMinusPoints != 0)
                {
                    insert += "PlusMinusPoints, Plus, Minus, ";
                    values += player.statistics.plusMinusPoints + ", " + player.statistics.plus + ", " + player.statistics.minus + ", ";
                }
                sec = double.Parse(player.statistics.minutes.Substring(player.statistics.minutes.IndexOf("M") + 1, 5));
                sec = sec / 60;
                minCalc = double.Parse(player.statistics.minutes.Substring(2, 2));
                minCalc = Math.Round(minCalc + sec, 2);
            }
            else if (player.statistics.minutes == "")
            {
                insert += "Minutes, ";
                values += "'0', ";
            }


            if (player.statistics.turnovers > 0)
            {
                insert += "AssistsTurnoverRatio, ";
                values += Math.Round((double)(player.statistics.assists) / (double)(player.statistics.turnovers), 3) + ", ";
            }
            else
            {
                insert += "AssistsTurnoverRatio, ";
                values += "0, ";
            }
            if (player.position != "" && player.position != null)
            {
                insert += "Position, ";
                values += "'" + player.position + "', ";
            }
            insert += "Status, ";
            values += "'" + player.status + "', ";
            if (player.notPlayingReason != "" && player.notPlayingReason != null)
            {
                insert += "StatusReason, ";
                values += "'" + player.notPlayingReason.Replace("'", "''") + "', ";
                if (player.notPlayingDescription != "" && player.notPlayingDescription != null)
                {
                    insert += "StatusDescription, ";
                    values += "'" + player.notPlayingDescription.Replace("'", "''") + "', ";
                }
            }

            insert += "MinutesCalculated, " + "BlocksReceived, " + "PointsFastBreak, " + "PointsInThePaint, " + "PointsSecondChance, " + "FoulsOffensive, " + "FoulsDrawn, " + "FoulsTechnical, ";
            values += minCalc + ", " + player.statistics.blocksReceived + ", " + player.statistics.pointsFastBreak + ", " + player.statistics.pointsInThePaint + ", " + player.statistics.pointsSecondChance
            + ", " + player.statistics.foulsOffensive + ", " + player.statistics.foulsDrawn + ", " + player.statistics.foulsTechnical + ", ";


            insert = insert.Remove(insert.Length - ", ".Length);
            values = values.Remove(values.Length - ", ".Length) + ") ";





            string insertStarters = "insert into StartingLineups values(" + season + ", " + GameID + ", " + TeamID + ", " + MatchupID + ", " + player.personId + ", '";

            if (player.starter != "1")
            {
                insertStarters += "Bench', null)\n";
            }
            else
            {
                insertStarters += "Starters', '" + player.position + "')\n";
            }
            currentPlayerBox += insert + values + "\n" + insertStarters;
        }
        #endregion

        //Current TeamBox
        #region Current TeamBox
        public void CurrentTeamBoxStaging(NBAdbToolboxCurrent.Game game, int season)
        {
            CurrentTeamBoxCheck(game.homeTeam, season, Int32.Parse(game.gameId), game.awayTeam.teamId);    //TeamBox Update - Home Team
            CurrentTeamBoxCheck(game.awayTeam, season, Int32.Parse(game.gameId), game.homeTeam.teamId);    //TeamBox Update - Away Team
        }
        public void CurrentTeamBoxCheck(NBAdbToolboxCurrent.Team team, int season, int GameID, int MatchupID)
        {
            SqlConnection tBox = new SqlConnection(cString);
            using (tBox)
            {
                using (SqlCommand TeamBoxCheck = new SqlCommand("TeamBoxCheck"))
                {
                    TeamBoxCheck.Connection = tBox;
                    TeamBoxCheck.CommandType = CommandType.StoredProcedure;
                    TeamBoxCheck.Parameters.AddWithValue("@SeasonID", season);
                    TeamBoxCheck.Parameters.AddWithValue("@GameID", GameID);
                    TeamBoxCheck.Parameters.AddWithValue("@TeamID", team.teamId);
                    TeamBoxCheck.Parameters.AddWithValue("@MatchupID", MatchupID);
                    using (SqlDataAdapter sGameSearch = new SqlDataAdapter())
                    {
                        TeamBoxCheck.Connection = tBox;
                        sGameSearch.SelectCommand = TeamBoxCheck;
                        tBox.Open();
                        SqlDataReader reader = TeamBoxCheck.ExecuteReader();
                        reader.Read();
                        if (!reader.HasRows)
                        {
                            tBox.Close();
                            CurrentTeamBoxInsert(team, season, GameID, MatchupID);

                        }
                        else
                        {
                            if (reader.GetInt32(3) != team.statistics.points || reader.GetInt32(4) != team.statistics.pointsAgainst)
                            {
                                tBox.Close();
                                //TeamBoxUpdate(team, season, GameID, MatchupID, PointsAgainst, "TeamBoxUpdateHistoric", lineup);
                            }
                            else
                            {
                                tBox.Close();
                            }
                        }
                    }
                }
            }
        }
        public void CurrentTeamBoxInsert(NBAdbToolboxCurrent.Team team, int season, int GameID, int MatchupID)
        {
            string insert = "insert into TeamBox(SeasonID, GameID, TeamID, MatchupID, Assists, AssistsTurnoverRatio, BenchPoints, BiggestLead, BiggestLeadScore," +
                "BiggestScoringRun, BiggestScoringRunScore, Blocks, BlocksReceived, FastBreakPointsAttempted, FastBreakPointsMade, " +
                "FastBreakPointsPercentage, FGA, FieldGoalsEffectiveAdjusted, FGM, [FG%], FoulsOffensive, FoulsDrawn, FoulsPersonal," +
                "FoulsTeam, FoulsTechnical, FoulsTeamTechnical, FTA, FTM, [FT%], LeadChanges, Points, PointsAgainst, " +
                "PointsFastBreak, PointsFromTurnovers, PointsInThePaint, PointsInThePaintAttempted, PointsInThePaintMade, " +
                "PointsInThePaintPercentage, PointsSecondChance, ReboundsDefensive, ReboundsOffensive, ReboundsPersonal, " +
                "ReboundsTeam, ReboundsTeamDefensive, ReboundsTeamOffensive, ReboundsTotal, SecondChancePointsAttempted, " +
                "SecondChancePointsMade, SecondChancePointsPercentage, Steals, FG3A, FG3M, [FG3%], TimeLeading, " +
                "TimesTied, TrueShootingAttempts, TrueShootingPercentage, Turnovers, TurnoversTeam, TurnoversTotal, " +
                "FG2A, FG2M, [FG2%])";
            string values = "values(" + season + ", " + GameID + ", " + team.teamId + ", " + MatchupID + ", "
                + team.statistics.assists + ", "
                + team.statistics.assistsTurnoverRatio + ", "
                + team.statistics.benchPoints + ", "
                + team.statistics.biggestLead + ", '"
                + team.statistics.biggestLeadScore + "', "
                + team.statistics.biggestScoringRun + ", '"
                + team.statistics.biggestScoringRunScore + "', "
                + team.statistics.blocks + ", "
                + team.statistics.blocksReceived + ", "
                + team.statistics.fastBreakPointsAttempted + ", "
                + team.statistics.fastBreakPointsMade + ", "
                + team.statistics.fastBreakPointsPercentage + ", "
                + team.statistics.fieldGoalsAttempted + ", "
                + team.statistics.fieldGoalsEffectiveAdjusted + ", "
                + team.statistics.fieldGoalsMade + ", "
                + team.statistics.fieldGoalsPercentage + ", "
                + team.statistics.foulsOffensive + ", "
                + team.statistics.foulsDrawn + ", "
                + team.statistics.foulsPersonal + ", "
                + team.statistics.foulsTeam + ", "
                + team.statistics.foulsTechnical + ", "
                + team.statistics.foulsTeamTechnical + ", "
                + team.statistics.freeThrowsAttempted + ", "
                + team.statistics.freeThrowsMade + ", "
                + team.statistics.freeThrowsPercentage + ", "
                + team.statistics.leadChanges + ", "
                + team.statistics.points + ", "
                + team.statistics.pointsAgainst + ", "
                + team.statistics.pointsFastBreak + ", "
                + team.statistics.pointsFromTurnovers + ", "
                + team.statistics.pointsInThePaint + ", "
                + team.statistics.pointsInThePaintAttempted + ", "
                + team.statistics.pointsInThePaintMade + ", "
                + team.statistics.pointsInThePaintPercentage + ", "
                + team.statistics.pointsSecondChance + ", "
                + team.statistics.reboundsDefensive + ", " + team.statistics.reboundsOffensive + ", " + team.statistics.reboundsPersonal + ", "
                + team.statistics.reboundsTeam + ", " + team.statistics.reboundsTeamDefensive + ", " + team.statistics.reboundsTeamOffensive + ", " + team.statistics.reboundsTotal + ", "
                + team.statistics.secondChancePointsAttempted + ", "
                + team.statistics.secondChancePointsMade + ", "
                + team.statistics.secondChancePointsPercentage + ", "
                + team.statistics.steals + ", "
                + team.statistics.threePointersAttempted + ", "
                + team.statistics.threePointersMade + ", "
                + team.statistics.threePointersPercentage + ", '"
                + team.statistics.timeLeading + "', "
                + team.statistics.timesTied + ", "
                + team.statistics.trueShootingAttempts + ", "
                + team.statistics.trueShootingPercentage + ", "
                + team.statistics.turnovers + ", "
                + team.statistics.turnoversTeam + ", "
                + team.statistics.turnoversTotal + ", "
                + team.statistics.twoPointersAttempted + ", "
                + team.statistics.twoPointersMade + ", "
                + team.statistics.twoPointersPercentage + ")";
            currentTeamBox += insert + values + "\n";
        }
        #endregion

        //Current PlayByPlay
        #region Current PlayByPlay
        public void CurrentPlayByPlayStaging(NBAdbToolboxCurrentPBP.Game game, int season, string sender)
        {
            currentPBPInsert = "";
            picLoad.Invoke((MethodInvoker)(() =>
            {
                if (picLoad.Image != null)
                {
                    picLoad.Image.Dispose(); // release the previous image
                    picLoad.Image = null;
                }

                using (var img = Image.FromFile(Path.Combine(projectRoot, "Content", "Loading", "kawhi" + currentImageIterator + ".png")))
                {
                    picLoad.Image = new Bitmap(img); // clone it so file lock is released
                }
            }));

            int i = 1;
            foreach (NBAdbToolboxCurrentPBP.Action action in game.actions)
            {
                CurrentPlayByPlayInsert(action, Int32.Parse(game.gameId), season, i);
                i++;
            }

            
        }
        public void CurrentPlayByPlayInsert(NBAdbToolboxCurrentPBP.Action action, int GameID, int season, int iteration)
        {
            string insert = "insert into PlayByPlay(SeasonID, GameID, ActionID, ActionNumber, Qtr, QtrType, Clock, TimeActual, ScoreHome, ScoreAway, ActionType, ";
            string values = ") values(" + season + ", " + GameID + ", " + iteration + ", " + action.actionNumber + ", " + action.period + ", '" + action.periodType + "', replace(replace(replace('" + action.clock + "', 'PT', ''), 'M', ':'), 'S', ''), '" + SqlDateTime.Parse(action.timeActual) +
                "', " + action.scoreHome + ", " + action.scoreAway + ", '" + action.actionType + "', ";
            if (action.description != null && action.description != "")
            {
                insert += "Description, ";
                values += "'" + action.description.Replace("'", "''") + "', ";
            }
            if (action.side != null && action.side != "")
            {
                insert += "Side, ";
                values += "'" + action.side + "', ";
            }
            if (action.subType != "")
            {
                insert += "SubType, ";
                values += "'" + action.subType + "', ";
            }
            if (action.area != null && action.area != null)
            {

                insert += "Area, AreaDetail, ";
                values += "'" + action.area + "', '" + action.areaDetail + "', ";
            }

            if (action.x != null)
            {
                insert += "X, Y, XLegacy, YLegacy, ";
                values += action.x + ", " + action.y + ", " + action.xLegacy + ", " + action.yLegacy + ", ";
            }

            if (action.isFieldGoal == 1)
            {
                insert += "IsFieldGoal, ";
                values += action.isFieldGoal + ", ";
                if (action.shotResult == "Made")                 //If Make
                {
                    insert += "ShotType, ";                         //Shot Type
                    values += "'FG" + action.actionType.Substring(0, 1) + "M', ";    //Shot Type
                    insert += "PtsGenerated, ";                     //PtsGenerated
                    values += action.actionType.Substring(0, 1) + ", ";              //PtsGenerated
                }
                else if (action.shotResult == "Missed")         //If Miss
                {
                    insert += "ShotType, ";                         //Shot Type
                    values += "'FG" + action.actionType.Substring(0, 1) + "A', ";    //Shot Type
                    insert += "PtsGenerated, ";                     //PtsGenerated
                    values += "0, ";                                //PtsGenerated
                }
                insert += "ShotResult, ShotValue, ShotDistance, ";
                values += "'" + action.shotResult + "', " + action.actionType.Substring(0, 1) + ", " + action.shotDistance + ", ";
            }
            else if (action.actionType == "Free Throw")
            {
                if (action.description.Substring(action.description.Length - 4) == "PTS)")
                {
                    insert += "ShotType, PtsGenerated, ShotResult, ShotValue, ";
                    values += "'FTM', 1, 'Made', 1, ";
                }
                else if (action.description.Substring(0, 4) == "MISS")
                {
                    insert += "ShotType, PtsGenerated, ShotResult, ShotValue, ";
                    values += "'FTA', 0, 'Missed', 1, ";
                }
            }
            #region Entities (Player, Teams, Officials)
            if (action.teamId != 0 && action.teamId != null)
            {
                insert += "TeamID, Tricode, ";
                values += action.teamId + ", '" + action.teamTricode + "', ";
            }
            if (action.possession != 0 && action.possession != null)
            {
                insert += "Possession, ";
                values += action.possession + ", ";
            }
            if (action.officialId != null)
            {
                insert += "OfficialID, ";
                values += action.officialId + ", ";
            }
            if (action.personId != 0)
            {
                insert += "PlayerID, ";
                values += action.personId + ", ";
            }
            if (action.assistPersonId != null)
            {
                insert += "PlayerIDAst, ";
                values += action.assistPersonId + ", ";
            }
            if (action.blockPersonId != null)
            {
                insert += "PlayerIDBlk, ";
                values += action.blockPersonId + ", ";
            }
            if (action.stealPersonId != null)
            {
                insert += "PlayerIDStl, ";
                values += action.stealPersonId + ", ";
            }
            if (action.jumpBallLostPersonId != null)
            {
                insert += "PlayerIDJumpL, ";
                values += action.jumpBallLostPersonId + ", ";
            }
            if (action.jumpBallWonPersonId != null)
            {
                insert += "PlayerIDJumpW, ";
                values += action.jumpBallWonPersonId + ", ";
            }
            if (action.foulDrawnPersonId != null)
            {
                insert += "PlayerIDFoulDrawn, ";
                values += action.foulDrawnPersonId + ", ";
            }
            #endregion

            int q = 1;
            foreach (object qual in action.qualifiers)
            {
                if (qual != null && q < 4)
                {
                    insert += "Qual" + q + ", ";
                    values += "'" + qual + "', ";
                }
                q++;
            }
            if (action.descriptor != null)
            {
                insert += "Descriptor, ";
                values += "'" + action.descriptor + "', ";
            }
            if (action.shotActionNumber != null)
            {
                insert += "ShotActionNbr, ";
                values += action.shotActionNumber + ", ";
            }



            insert = insert.Remove(insert.Length - ", ".Length);
            values = values.Remove(values.Length - ", ".Length) + ")";
            currentPlayByPlay += insert + values + "\n";

        }
        #endregion
        #endregion










        //Current Data Outdated
        #region Current Data Outdated

        public int currentIterator = 0;
        public int currentImageIterator = 0;
        public bool currentReverse = false;
        public void ImageDriver(int stop)
        {
            if (currentReverse)
            {
                currentImageIterator--;
            }
            else
            {
                currentImageIterator++;
            }
            if (currentImageIterator == stop)
            {
                currentReverse = true;
            }
            if (currentImageIterator == 1)
            {
                currentReverse = false;
            }
        }

        public string DBthrottleQuery = "";
        public async Task ReadCurrentGameData(int GameID, int season, string sender)
        {
            //BoxScore
            bool doBox = true;
            bool doPBP = true;
            bool useHistoric = false;
            string missingData = "";
            #region BoxScore
            ImageDriver(25);
            lblCurrentGameCount.Invoke((MethodInvoker)(() =>
            {
                lblCurrentGameCount.Text = GameID.ToString() + " Box";
            }));
            try
            {
                rootC = await currentData.GetJSON(GameID, season);
                if (rootC.game == null)
                {
                    doBox = false;
                    useHistoric = true;
                    missingData += "insert into util.MissingData values(" + season + ", " + GameID + ", 'Current', 'Box', 'No File available from NBA')\n";
                }
            }
            catch (WebException ex)
            {
                doBox = false;
                useHistoric = true;
                missingData += "insert into util.MissingData values(" + season + ", " + GameID + ", 'Current', 'Box', 'No File available from NBA')\n";
            }
            if (doBox)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if (sender == "New")
                        {
                            CurrentInsertStaging(rootC.game, season);
                        }
                        else if (sender == "Existing")
                        {
                            GetCurrentBoxDetails(rootC.game, season, sender);
                        }
                    }
                    catch
                    {
                        useHistoric = true;
                        missingData += "insert into util.MissingData values(" + season + ", " + GameID + ", 'Current', 'Box', 'JSON file formatting - NBA pls fix')\n";
                    }
                });
            }
            if (useHistoric)
            {

            }
            #endregion

            //PlayByPlay
            #region PlayByPlay
            ImageDriver(25);
            lblCurrentGameCount.Invoke((MethodInvoker)(() =>
            {
                lblCurrentGameCount.Text = GameID.ToString() + " PlayByPlay";
            }));
            try
            {
                rootCPBP = await currentDataPBP.GetJSON(GameID, season);
                if (rootCPBP.game == null)
                {
                    doPBP = false;
                    missingData += "insert into util.MissingData values(" + season + ", " + GameID + ", 'Current', 'PlayByPlay', 'No file available from NBA')\n";
                }
            }
            catch (WebException ex)
            {
                doPBP = false;
                missingData += "insert into util.MissingData values(" + season + ", " + GameID + ", 'Current', 'PlayByPlay', 'No file available from NBA')\n";
            }
            if (doPBP)
            {
                await Task.Run(() =>
                {
                    try
                    {
                        if (sender == "New")
                        {
                            GetCurrentPBPDetails(rootCPBP.game, season, sender);
                        }
                        else if (sender == "Existing")
                        {
                            GetCurrentPBPDetails(rootCPBP.game, season, sender);
                        }
                    }
                    catch
                    {
                        missingData += "insert into util.MissingData values(" + season + ", " + GameID + ", 'Current', 'PlayByPlay', 'JSON file formatting - NBA pls fix')\n";
                    }

                });
            }
            #endregion



            SQLdb = new SqlConnection(cString);
            //Insert data into Main tables and wait for execution
            #region Insert data into Main tables and wait for execution
            string allInOne = teamInsert + arenaInsert + officialInsert + gameInsert + playerInsert;
            string hitDb = allInOne;
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

            }
            #endregion
            DBthrottleQuery += missingData + currentBoxUpdate + "\n" + currentPBPInsert;
            currentBoxUpdate = "";
            currentPBPInsert = "";
            string mD_cBU_cPBP = DBthrottleQuery; //missingData + currentBoxUpdate + currentPBPInsert
            DBthrottleQuery = "";              //clear for next batch

            // Kick off background DB update
            _ = Task.Run(() =>
            {
                try
                {
                    using (SqlConnection conn = new SqlConnection(cString))
                    {
                        using (SqlCommand cmd = new SqlCommand(mD_cBU_cPBP, conn))
                        {
                            cmd.CommandType = CommandType.Text;
                            conn.Open();
                            cmd.ExecuteNonQuery();
                        }
                    }
                }
                catch (Exception e)
                {
                    // Optional: log the error or queue it for retry
                }
            });


        }
        public string currentBoxUpdate = "";
        public string currentBoxInsert = "";
        public void GetCurrentBoxDetails(NBAdbToolboxCurrent.Game game, int season, string sender)
        {
            picLoad.Invoke((MethodInvoker)(() =>
            {
                if (picLoad.Image != null)
                {
                    picLoad.Image.Dispose(); // release the previous image
                    picLoad.Image = null;
                }

                using (var img = Image.FromFile(Path.Combine(projectRoot, "Content", "Loading", "kawhi" + currentImageIterator + ".png")))
                {
                    picLoad.Image = new Bitmap(img); // clone it so file lock is released
                }
            }));

            if (sender == "New")
            {

            }
            else if (sender == "Existing")
            {
                CurrentTeamBoxStagingUpdate(game, season);
            }
        }
        public void GetCurrentPBPDetails(NBAdbToolboxCurrentPBP.Game game, int season, string sender)
        {
            currentPBPInsert = "";
            picLoad.Invoke((MethodInvoker)(() =>
            {
                if (picLoad.Image != null)
                {
                    picLoad.Image.Dispose(); // release the previous image
                    picLoad.Image = null;
                }

                using (var img = Image.FromFile(Path.Combine(projectRoot, "Content", "Loading", "kawhi" + currentImageIterator + ".png")))
                {
                    picLoad.Image = new Bitmap(img); // clone it so file lock is released
                }
            }));

            if (sender == "New")
            {

            }
            else if (sender == "Existing")
            {
                int i = 1;
                foreach (NBAdbToolboxCurrentPBP.Action action in game.actions)
                {
                    CurrentPBPInsertString(action, Int32.Parse(game.gameId), season, i);
                    i++;
                }

            }
        }
        public string currentPBPInsert = "";
        public void CurrentPBPInsertString(NBAdbToolboxCurrentPBP.Action action, int GameID, int season, int iteration)
        {
            string insert = "insert into PlayByPlay(SeasonID, GameID, ActionID, ActionNumber, Qtr, QtrType, Clock, TimeActual, ScoreHome, ScoreAway, ActionType, ";
            string values = ") values(" + season + ", " + GameID + ", " + iteration + ", " + action.actionNumber + ", " + action.period + ", '" + action.periodType + "', replace(replace(replace('" + action.clock + "', 'PT', ''), 'M', ':'), 'S', ''), '" + SqlDateTime.Parse(action.timeActual) +
                "', " + action.scoreHome + ", " + action.scoreAway + ", '" + action.actionType + "', ";
            if (action.description != null && action.description != "")
            {
                insert += "Description, ";
                values += "'" + action.description.Replace("'", "''") + "', ";
            }
            if (action.side != null && action.side != "")
            {
                insert += "Side, ";
                values += "'" + action.side + "', ";
            }
            if (action.subType != "")
            {
                insert += "SubType, ";
                values += "'" + action.subType + "', ";
            }
            if (action.area != null && action.area != null)
            {

                insert += "Area, AreaDetail, ";
                values += "'" + action.area + "', '" + action.areaDetail + "', ";
            }

            if (action.x != null)
            {
                insert += "X, Y, XLegacy, YLegacy, ";
                values += action.x + ", " + action.y + ", " + action.xLegacy + ", " + action.yLegacy + ", ";
            }

            if (action.isFieldGoal == 1)
            {
                insert += "IsFieldGoal, ";
                values += action.isFieldGoal + ", ";
                if (action.shotResult == "Made")                 //If Make
                {
                    insert += "ShotType, ";                         //Shot Type
                    values += "'FG" + action.actionType.Substring(0, 1) + "M', ";    //Shot Type
                    insert += "PtsGenerated, ";                     //PtsGenerated
                    values += action.actionType.Substring(0, 1) + ", ";              //PtsGenerated
                }
                else if (action.shotResult == "Missed")         //If Miss
                {
                    insert += "ShotType, ";                         //Shot Type
                    values += "'FG" + action.actionType.Substring(0, 1) + "A', ";    //Shot Type
                    insert += "PtsGenerated, ";                     //PtsGenerated
                    values += "0, ";                                //PtsGenerated
                }
                insert += "ShotResult, ShotValue, ShotDistance, ";
                values += "'" + action.shotResult + "', " + action.actionType.Substring(0, 1) + ", " + action.shotDistance + ", ";
            }
            else if (action.actionType == "Free Throw")
            {
                if (action.description.Substring(action.description.Length - 4) == "PTS)")
                {
                    insert += "ShotType, PtsGenerated, ShotResult, ShotValue, ";
                    values += "'FTM', 1, 'Made', 1, ";
                }
                else if (action.description.Substring(0, 4) == "MISS")
                {
                    insert += "ShotType, PtsGenerated, ShotResult, ShotValue, ";
                    values += "'FTA', 0, 'Missed', 1, ";
                }
            }
            #region Entities (Player, Teams, Officials)
            if (action.teamId != 0 && action.teamId != null)
            {
                insert += "TeamID, Tricode, ";
                values += action.teamId + ", '" + action.teamTricode + "', ";
            }
            if (action.possession != 0 && action.possession != null)
            {
                insert += "Possession, ";
                values += action.possession + ", ";
            }
            if (action.officialId != null)
            {
                insert += "OfficialID, ";
                values += action.officialId + ", ";
            }
            if (action.personId != 0)
            {
                insert += "PlayerID, ";
                values += action.personId + ", ";
            }
            if (action.assistPersonId != null)
            {
                insert += "PlayerIDAst, ";
                values += action.assistPersonId + ", ";
            }
            if (action.blockPersonId != null)
            {
                insert += "PlayerIDBlk, ";
                values += action.blockPersonId + ", ";
            }
            if (action.stealPersonId != null)
            {
                insert += "PlayerIDStl, ";
                values += action.stealPersonId + ", ";
            }
            if (action.jumpBallLostPersonId != null)
            {
                insert += "PlayerIDJumpL, ";
                values += action.jumpBallLostPersonId + ", ";
            }
            if (action.jumpBallWonPersonId != null)
            {
                insert += "PlayerIDJumpW, ";
                values += action.jumpBallWonPersonId + ", ";
            }
            if (action.foulDrawnPersonId != null)
            {
                insert += "PlayerIDFoulDrawn, ";
                values += action.foulDrawnPersonId + ", ";
            }
            #endregion

            int q = 1;
            foreach (object qual in action.qualifiers)
            {
                if (qual != null && q < 4)
                {
                    insert += "Qual" + q + ", ";
                    values += "'" + qual + "', ";
                }
                q++;
            }
            if (action.descriptor != null)
            {
                insert += "Descriptor, ";
                values += "'" + action.descriptor + "', ";
            }
            if (action.shotActionNumber != null)
            {
                insert += "ShotActionNbr, ";
                values += action.shotActionNumber + ", ";
            }



            insert = insert.Remove(insert.Length - ", ".Length);
            values = values.Remove(values.Length - ", ".Length) + ")";
            currentPBPInsert += insert + values + "\n";

        }
        public void CurrentTeamBoxUpdate(NBAdbToolboxCurrent.Team team, int season, int GameID, int MatchupID)
        {
            string update = "update TeamBox set"
                + " FieldGoalsEffectiveAdjusted = " + team.statistics.fieldGoalsEffectiveAdjusted
                + ", SecondChancePointsMade = " + team.statistics.secondChancePointsMade
                + ", SecondChancePointsAttempted = " + team.statistics.secondChancePointsAttempted
                + ", SecondChancePointsPercentage = " + team.statistics.secondChancePointsMade
                + ", TrueShootingAttempts = " + team.statistics.trueShootingAttempts
                + ", TrueShootingPercentage = " + team.statistics.trueShootingPercentage
                + ", PointsFromTurnovers = " + team.statistics.pointsFromTurnovers
                + ", PointsSecondChance = " + team.statistics.pointsSecondChance
                + ", PointsInThePaint = " + team.statistics.pointsInThePaint
                + ", PointsInThePaintMade = " + team.statistics.pointsInThePaintMade
                + ", PointsInThePaintAttempted = " + team.statistics.pointsInThePaintAttempted
                + ", PointsInThePaintPercentage = " + team.statistics.pointsInThePaintPercentage
                + ", PointsFastBreak = " + team.statistics.pointsFastBreak
                + ", FastBreakPointsMade = " + team.statistics.fastBreakPointsMade
                + ", FastBreakPointsAttempted = " + team.statistics.fastBreakPointsAttempted
                + ", FastBreakPointsPercentage = " + team.statistics.fastBreakPointsPercentage
                + ", BenchPoints = " + team.statistics.benchPoints
                + ", ReboundsPersonal = " + team.statistics.reboundsPersonal
                + ", ReboundsTeam = " + team.statistics.reboundsTeam
                + ", ReboundsTeamDefensive = " + team.statistics.reboundsTeamDefensive
                + ", ReboundsTeamOffensive = " + team.statistics.reboundsTeamOffensive
                + ", BiggestLead = " + team.statistics.biggestLead
                + ", BiggestLeadScore = '" + team.statistics.biggestLeadScore + "'"
                + ", BiggestScoringRun = " + team.statistics.biggestScoringRun
                + ", BiggestScoringRunScore = '" + team.statistics.biggestScoringRunScore + "'"
                + ", TimeLeading = '" + team.statistics.timeLeading + "'"
                + ", TimesTied = " + team.statistics.timesTied
                + ", LeadChanges = " + team.statistics.leadChanges
                + ", TurnoversTeam = " + team.statistics.turnoversTeam
                + ", TurnoversTotal = " + team.statistics.turnoversTotal
                + ", BlocksReceived = " + team.statistics.blocksReceived
                + ", FoulsDrawn = " + team.statistics.foulsDrawn
                + ", FoulsOffensive = " + team.statistics.foulsOffensive
                + ", FoulsTeam = " + team.statistics.foulsTeam
                + ", FoulsTeamTechnical = " + team.statistics.foulsTeamTechnical
                + ", FoulsTechnical = " + team.statistics.foulsTechnical;

            string where = " where SeasonID = " + season + " and GameID = " + GameID + " and TeamID = " + team.teamId + " and MatchupID = " + MatchupID;

            currentBoxUpdate += update + where + "\n";

            //Sends to PlayerUpdates
            foreach (NBAdbToolboxCurrent.Player player in team.players)
            {
                CurrentPlayerBoxUpdate(player, season, GameID, team.teamId, MatchupID);
            }
        }
        public void CurrentTeamBoxStagingUpdate(NBAdbToolboxCurrent.Game game, int season)
        {
            CurrentTeamBoxUpdate(game.homeTeam, season, Int32.Parse(game.gameId), game.awayTeam.teamId);    //TeamBox Update - Home Team
            CurrentTeamBoxUpdate(game.awayTeam, season, Int32.Parse(game.gameId), game.homeTeam.teamId);    //TeamBox Update - Away Team
        }
        #endregion
    }

}
