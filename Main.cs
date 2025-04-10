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
using NBAdbToolboxHistoric;
using System.Diagnostics;
using static NBAdbToolbox.Main;
using System.Data.SqlTypes;

namespace NBAdbToolbox
{
    public partial class Main: Form
    {
        NBAdbToolboxHistoric.Root root = new NBAdbToolboxHistoric.Root();
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
        public Panel pnlSeason = new Panel();           public Label lblSeason = new Label();           public PictureBox picSeason = new PictureBox();         public Label lblSeasonSub = new Label(); 
        public Panel pnlTeam = new Panel();             public Label lblTeam = new Label();             public PictureBox picTeam = new PictureBox();           public Label lblTeamSub = new Label(); 
        public Panel pnlGame = new Panel();             public Label lblGame = new Label();             public PictureBox picGame = new PictureBox();           public Label lblGameSub = new Label();
        public Panel pnlPlayerBox = new Panel();        public Label lblPlayerBox = new Label();        public PictureBox picPlayerBox = new PictureBox();      public Label lblPlayerBoxSub = new Label();
        public Panel pnlTeamBox = new Panel();          public Label lblTeamBox = new Label();          public PictureBox picTeamBox = new PictureBox();        public Label lblTeamBoxSub = new Label();
        public Panel pnlPlayer = new Panel();           public Label lblPlayer = new Label();           public PictureBox picPlayer = new PictureBox();         public Label lblPlayerSub = new Label();
        public Panel pnlPbp = new Panel();              public Label lblPbp = new Label();              public PictureBox picPbp = new PictureBox();            public Label lblPbpSub = new Label();
        public Panel pnlTeamBoxLineups = new Panel();   public Label lblTeamBoxLineups = new Label();   public PictureBox picTeamBoxLineups = new PictureBox(); public Label lblTeamBoxLineupsSub = new Label();
        //pnlDbUtil sub panel Positions and sizes
        public int leftPanelPos = 0;
        public int midPanelPos = 0;
        public int rightPanelPos = 0;
        public int fullHeight = 0;
        public int dimW = 0;
        public int dimH = 0;
        public int dimH2 =0;
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
        public Panel pnlLoad = new Panel();
        public Label lblSeasonStatusLoad = new Label
        {
            Text = "Currently loading: "
        };
        public Label lblSeasonStatusLoadInfo = new Label
        {
            Text = ""
        };
        public Label lblCurrentGame = new Label
        {
            Text = "Current game: "
        };
        public Label lblCurrentGameCount = new Label
        {
            Text = "0"
        };
        public Label lblWorkingOn = new Label
        {
            Text = "Currently working on: "
        };
        public Label lblWorkingOnInfo = new Label
        {
            Text = ""
        };
        public float loadFontSize = 0;
        public string completionMessage = "";

        PictureBox picLoad = new PictureBox
        {
            Image = Image.FromFile(Path.Combine(projectRoot, "Content", "Loading", "kawhi1.png"))
        };

        //Header Panels
        public Panel pnlNav = new Panel();
        public Panel pnlScoreboard = new Panel();



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









            //This should be second to last i believe.
            //Children elements should go above the parents, background image should be last added.        
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

                for(int i = 0; i < listSeasons.SelectedItems.Count; i++)
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
                    //Stopwatch stopwatch = Stopwatch.StartNew();
                    foreach (int season in seasons)
                    {
                        completionMessage += season + ": ";
                        lblStatus.Text = "Loading " + season + " season...";
                        CenterElement(pnlWelcome, lblStatus);
                        btnPopulate.Enabled = false;
                        btnEdit.Enabled = false;
                        listSeasons.Enabled = false;
                        await Task.Run(async () =>      //This sets the root variable to our big file
                        {
                            await ReadSeasonFile(season, popup.historic, popup.current);
                        });
                        //End season read
                        lblStatus.Text = season + " parsed. Inserting data...";
                        CenterElement(pnlWelcome, lblStatus);
                        int iterator = 0;
                        int imageIteration = 1;
                        bool reverse = false;
                        int remainder = 5;
                        lblSeasonStatusLoadInfo.Text = season + " Regular Season";
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
                        }
                        completionMessage += iterator + " regular season and ";
                        int regGames = iterator;
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
                        }
                        completionMessage += (iterator - regGames) + " postseason games loaded successfully.\n";

                        teamsDone = false;
                        arenasDone = false;
                    }
                    btnPopulate.Enabled = true;
                    btnEdit.Enabled = true;
                    listSeasons.Enabled = true;
                    lblStatus.Text = "Welcome Back!";
                    CenterElement(pnlWelcome, lblStatus);
                    lblCurrentGame.Text = completionMessage;
                    lblCurrentGame.ForeColor = Color.Green;
                    fontSize = ((float)(pnlLoad.Height * .05) / (96 / 12)) * (72 / 12);
                    lblCurrentGame.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, pnlLoad, lblCurrentGame);
                    lblCurrentGame.AutoSize = true;
                    lblCurrentGameCount.Text = "";
                    lblCurrentGameCount.Visible = false;
                    lblSeasonStatusLoad.Text = "Done! Check your SQL db";
                    lblSeasonStatusLoad.ForeColor = Color.Green;
                    fontSize = ((float)(pnlLoad.Height * .08) / (96 / 12)) * (72 / 12);
                    lblSeasonStatusLoad.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Regular, pnlLoad, lblSeasonStatusLoad);
                    lblSeasonStatusLoad.AutoSize = true;
                    lblSeasonStatusLoadInfo.Text = "";
                    lblSeasonStatusLoadInfo.Visible = false;
                }
            };

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




        //Add an elemnent to a panel
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

        public void CenterElement(Panel panel, Control control)
        {
            control.AutoSize = true;
            control.Left = (panel.ClientSize.Width - control.Width) / 2;
        }


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


        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == Keys.Escape)
            {
                this.Close(); // or Application.Exit();
                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private float GetBestFitFontSize(Control control, string text, Font baseFont, int targetWidth)
        {
            using (Graphics g = control.CreateGraphics())
            {
                float fontSize = baseFont.Size;

                while (fontSize > 1)
                {
                    Font testFont = new Font(baseFont.FontFamily, fontSize, baseFont.Style);
                    SizeF size = g.MeasureString(text, testFont);

                    if (size.Width <= targetWidth)
                        return fontSize;

                    fontSize -= 0.5f;
                }

                return baseFont.Size; // fallback
            }
        }


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

        public void GetTablePanelInfo(string connectionString)
        {
            List<Panel> panels = new List<Panel> { pnlSeason, pnlTeam, pnlPlayer, pnlGame, pnlTeamBox, pnlPlayerBox, pnlPbp, pnlTeamBoxLineups};
            fullHeight = (int)(pnlDbUtil.Height * .5);
            dimW = pnlDbUtil.Width / 3;
            dimH = (int)(fullHeight * .25);
            dimH2 = (int)(fullHeight * .5);
            for (int i = 0; i < panels.Count; i++)
            {
                if(i <= 5)
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
                            else if(sdr["Rows"].ToString() == "0")
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
                        listSeasons.Items.Add(sdr["SeasonID"].ToString());                    
                    }
                }
            }
        }

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

        public void TableLabels(Panel pnl, Label lbl, float fontSize, string labelType, Label parent)
        {
            lbl.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
            AddPanelElement(pnl, lbl);
            if(labelType == "Header")
            {
                CenterElement(pnl, lbl);
            }
            else if (labelType == "Subhead")
            {
                AlignLeft(pnl, lbl, parent);
            }

        }
        public void AlignLeft(Panel pnl, Label lbl, Label parent)
        {
            lbl.Left = pnl.Left;
            lbl.Top = parent.Bottom;
            lbl.AutoSize = true;
        }

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






        //Scoreboard

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




        public async Task ReadSeasonFile(int season, bool bHistoric, bool bCurrent)
        {
            string filePath = Path.Combine(projectRoot, "Content\\", "dbconfig.json");
            filePath = filePath.Replace("dbconfig.json", "Historic Data\\");
            if (bHistoric || (!bHistoric && !bCurrent))
            {
                int iter = (season == 2012 || season == 2019 || season == 2020 || season == 2024) ? 3 : 4;
                root = await historic.ReadFile(season, iter, filePath);
            }            
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
        public void GetGameDetails(NBAdbToolboxHistoric.Game game, int season, string sender)
        {
            SQLdb = new SqlConnection(cString);

            //Teams
            TeamStaging(game, season);            
            //Arenas
            if (!arenasDone)
            {
                ArenaCheck(game.box.arena, game.box.homeTeamId, season);
            }
            //Officials
            foreach (NBAdbToolboxHistoric.Official official in game.box.officials)
            {
                OfficialCheck(official, season);
            }
            //Players
            PlayerStaging(game, season);
            //Games
            GameCheck(game, season, sender);


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
                    {   //If we have a result
                        while (reader.Read())
                        {
                            if (reader["Teams"].ToString() == "30")
                            {
                                teamsDone = true;   //If we have 30 teams, we can skip the Team methods until we come up on the end of the load
                            }
                        }
                        SQLdb.Close();
                    }
                    else
                    { //If no result, send to TeamInsert to insert into DB
                        SQLdb.Close();
                        TeamInsert(team, season);
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
        public void TeamInsert(NBAdbToolboxHistoric.Team team, int season)
        {
            using (SqlCommand TeamInsert = new SqlCommand("TeamInsert"))
            {
                TeamInsert.Connection = SQLdb;
                TeamInsert.CommandType = CommandType.StoredProcedure;
                TeamInsert.Parameters.AddWithValue("@SeasonID", season);
                TeamInsert.Parameters.AddWithValue("@TeamID", team.teamId);
                TeamInsert.Parameters.AddWithValue("@City", team.teamCity);
                TeamInsert.Parameters.AddWithValue("@Name", team.teamName);
                TeamInsert.Parameters.AddWithValue("@Tricode", team.teamTricode);
                TeamInsert.Parameters.AddWithValue("@Wins", team.teamWins);
                TeamInsert.Parameters.AddWithValue("@Losses", team.teamLosses);
                SQLdb.Open();
                TeamInsert.ExecuteScalar();
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
                        ArenaInsert(arena, teamID, season);
                    }
                }
            }
        }
        public void ArenaInsert(NBAdbToolboxHistoric.Arena arena, int teamID, int season)
        {
            using (SqlCommand ArenaInsert = new SqlCommand("ArenaInsert"))
            {
                ArenaInsert.Connection = SQLdb;
                ArenaInsert.CommandType = CommandType.StoredProcedure;
                ArenaInsert.Parameters.AddWithValue("@SeasonID", season);
                ArenaInsert.Parameters.AddWithValue("@ArenaID", arena.arenaId);
                ArenaInsert.Parameters.AddWithValue("@TeamID", teamID);
                ArenaInsert.Parameters.AddWithValue("@City", arena.arenaCity);
                ArenaInsert.Parameters.AddWithValue("@Country", arena.arenaCountry);
                ArenaInsert.Parameters.AddWithValue("@Name", arena.arenaName);
                ArenaInsert.Parameters.AddWithValue("@PostalCode", arena.arenaPostalCode);
                ArenaInsert.Parameters.AddWithValue("@State", arena.arenaState);
                ArenaInsert.Parameters.AddWithValue("@StreetAddress", arena.arenaStreetAddress);
                ArenaInsert.Parameters.AddWithValue("@Timezone", arena.arenaTimezone);
                SQLdb.Open();
                ArenaInsert.ExecuteScalar();
                SQLdb.Close();
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
                        //while (reader.Read())
                        //{
                        //    if (reader["Officials"].ToString() == "30")
                        //    {
                        //        teamsDone = true;
                        //    }
                        //}
                        SQLdb.Close();
                    }
                    else
                    {
                        SQLdb.Close();
                        OfficialInsert(official, season);
                    }
                }
            }
        }
        public void OfficialInsert(NBAdbToolboxHistoric.Official official, int season)
        {
            using (SqlCommand ArenaInsert = new SqlCommand("OfficialInsert"))
            {
                ArenaInsert.Connection = SQLdb;
                ArenaInsert.CommandType = CommandType.StoredProcedure;
                ArenaInsert.Parameters.AddWithValue("@SeasonID", season);
                ArenaInsert.Parameters.AddWithValue("@OfficialID", official.personId);
                ArenaInsert.Parameters.AddWithValue("@Name", official.name);
                ArenaInsert.Parameters.AddWithValue("@Number", official.jerseyNum);
                SQLdb.Open();
                ArenaInsert.ExecuteScalar();
                SQLdb.Close();
            }
        }
        #endregion

        //Player methods
        #region Player Methods
        public void PlayerStaging(NBAdbToolboxHistoric.Game game, int season)
        {
            foreach (NBAdbToolboxHistoric.Player player in game.box.homeTeam.players)
            {//Home Team
                int index = game.box.homeTeamPlayers.FindIndex(p => p.personId == player.personId);
                if (index == -1)
                {
                    PlayerCheck(player, season, "");

                }
                else
                {
                    PlayerCheck(player, season, game.box.homeTeamPlayers[index].jerseyNum);
                }
            }
            foreach (NBAdbToolboxHistoric.Player player in game.box.awayTeam.players)
            {//Away Team
                int index = game.box.awayTeamPlayers.FindIndex(p => p.personId == player.personId);
                if (index == -1)
                {
                    PlayerCheck(player, season, "");

                }
                else
                {
                    PlayerCheck(player, season, game.box.awayTeamPlayers[index].jerseyNum);
                }
            }

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
                        PlayerInsert(player, season, number);
                    }
                    else if (reader.HasRows && player.position != null && player.position != "" && reader.GetString(4) != player.position)
                    {
                        string oldPosition = reader.GetString(4);
                        SQLdb.Close();
                        PlayerUpdate(player, season, oldPosition, number);
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
        public void PlayerInsert(NBAdbToolboxHistoric.Player player, int season, string number)
        {
            using (SqlCommand PlayerInsert = new SqlCommand("PlayerInsert"))
            {
                PlayerInsert.Connection = SQLdb;
                PlayerInsert.CommandType = CommandType.StoredProcedure;
                PlayerInsert.Parameters.AddWithValue("@SeasonID", season);
                PlayerInsert.Parameters.AddWithValue("@PlayerID", player.personId);
                PlayerInsert.Parameters.AddWithValue("@Name", player.firstName + " " + player.familyName);
                PlayerInsert.Parameters.AddWithValue("@Number", number);                
                PlayerInsert.Parameters.AddWithValue("@position", player.position);
                SQLdb.Open();
                PlayerInsert.ExecuteScalar();
                SQLdb.Close();
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
                        GameInsert(game, season, sender);
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
                if(game.box.homeTeam.score > game.box.awayTeam.score)
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
        public void GameInsert(NBAdbToolboxHistoric.Game game, int season, string sender)
        {
            using (SqlCommand GameInsert = new SqlCommand("GameInsert"))
            {
                GameInsert.Connection = SQLdb;
                GameInsert.CommandType = CommandType.StoredProcedure;
                GameInsert.Parameters.AddWithValue("@SeasonID", season);
                GameInsert.Parameters.AddWithValue("@GameID", game.game_id);
                GameInsert.Parameters.AddWithValue("@Date", SqlDateTime.Parse(game.box.gameEt.Remove(game.box.gameEt.IndexOf('T'))));
                GameInsert.Parameters.AddWithValue("@HomeID", game.box.homeTeamId);
                GameInsert.Parameters.AddWithValue("@HScore", game.box.homeTeam.score);
                GameInsert.Parameters.AddWithValue("@AwayID", game.box.awayTeamId);
                GameInsert.Parameters.AddWithValue("@AScore", game.box.awayTeam.score);
                if (game.box.homeTeam.score > game.box.awayTeam.score)
                {
                    GameInsert.Parameters.AddWithValue("@WinnerID", game.box.homeTeamId);
                    GameInsert.Parameters.AddWithValue("@WScore", game.box.homeTeam.score);
                    GameInsert.Parameters.AddWithValue("@LoserID", game.box.awayTeamId);
                    GameInsert.Parameters.AddWithValue("@LScore", game.box.awayTeam.score);
                }
                else
                {
                    GameInsert.Parameters.AddWithValue("@WinnerID", game.box.awayTeamId);
                    GameInsert.Parameters.AddWithValue("@WScore", game.box.awayTeam.score);
                    GameInsert.Parameters.AddWithValue("@LoserID", game.box.homeTeamId);
                    GameInsert.Parameters.AddWithValue("@LScore", game.box.homeTeam.score);
                }
                if (sender == "Regular Season")
                {
                    GameInsert.Parameters.AddWithValue("@GameType", "RS");
                    GameInsert.Parameters.AddWithValue("@SeriesID", SqlString.Null);
                }
                else
                {
                    GameInsert.Parameters.AddWithValue("@GameType", "PS");
                    GameInsert.Parameters.AddWithValue("@SeriesID", "placeholder");
                }
                SqlDateTime datetime = SqlDateTime.Parse(game.box.gameTimeUTC);
                GameInsert.Parameters.AddWithValue("@Label", game.box.gameLabel);
                GameInsert.Parameters.AddWithValue("@LabelDetail", game.box.gameSubLabel);
                GameInsert.Parameters.AddWithValue("@Datetime", datetime);
                SQLdb.Open();
                GameInsert.ExecuteScalar();
                SQLdb.Close();
            }
        }
        #endregion

        //TeamBox methods
        #region TeamBox Methods
        public void TeamBoxStaging(NBAdbToolboxHistoric.Game game, int season)
        {
            TeamBoxCheck(game.box.homeTeam, season, game.game_id, game.box.awayTeamId, game.box.awayTeam.statistics.points);
            TeamBoxCheck(game.box.awayTeam, season, game.game_id, game.box.homeTeamId, game.box.homeTeam.statistics.points);

        }

        public void TeamBoxCheck(NBAdbToolboxHistoric.Team team, int season, string GameID, int MatchupID, int PointsAgainst)
        {
            using (SqlCommand TeamBoxCheck = new SqlCommand("TeamBoxCheck"))
            {
                TeamBoxCheck.Connection = SQLdb;
                TeamBoxCheck.CommandType = CommandType.StoredProcedure;
                TeamBoxCheck.Parameters.AddWithValue("@SeasonID", season);
            }

        }
        public void TeamBoxUpdate(NBAdbToolboxHistoric.Team team, int season, string GameID, int MatchupID, int PointsAgainst)
        {
            using (SqlCommand TeamBoxUpdate = new SqlCommand("TeamBoxUpdate"))
            {
                TeamBoxUpdate.Connection = SQLdb;
                TeamBoxUpdate.CommandType = CommandType.StoredProcedure;
                TeamBoxUpdate.Parameters.AddWithValue("@SeasonID", season);
            }

        }
        public void TeamBoxInsert(NBAdbToolboxHistoric.Team team, int season, string GameID, int MatchupID, int PointsAgainst)
        {
            using (SqlCommand TeamBoxInsert = new SqlCommand("TeamBoxInsert"))
            {
                TeamBoxInsert.Connection = SQLdb;
                TeamBoxInsert.CommandType = CommandType.StoredProcedure;
                TeamBoxInsert.Parameters.AddWithValue("@SeasonID", season);
                TeamBoxInsert.Parameters.AddWithValue("@GameID", Int32.Parse(GameID));
                TeamBoxInsert.Parameters.AddWithValue("@TeamID", team.teamId);
                TeamBoxInsert.Parameters.AddWithValue("@MatchupID", MatchupID);
                TeamBoxInsert.Parameters.AddWithValue("@FGM", team.statistics.fieldGoalsMade);
                TeamBoxInsert.Parameters.AddWithValue("@FGA", team.statistics.fieldGoalsAttempted);
                TeamBoxInsert.Parameters.AddWithValue("@FGpct", team.statistics.fieldGoalsPercentage);
                TeamBoxInsert.Parameters.AddWithValue("@FG2M", team.statistics.fieldGoalsMade);
                TeamBoxInsert.Parameters.AddWithValue("@FG2A", team.statistics.fieldGoalsAttempted);
                TeamBoxInsert.Parameters.AddWithValue("@FG2pct", team.statistics.fieldGoalsPercentage);
                TeamBoxInsert.Parameters.AddWithValue("@FG3M", team.statistics.threePointersMade);
                TeamBoxInsert.Parameters.AddWithValue("@FG3A", team.statistics.threePointersAttempted);
                TeamBoxInsert.Parameters.AddWithValue("@FG3pct", team.statistics.threePointersPercentage);
                TeamBoxInsert.Parameters.AddWithValue("@FTM", team.statistics.freeThrowsMade);
                TeamBoxInsert.Parameters.AddWithValue("@FTA", team.statistics.freeThrowsAttempted);
                TeamBoxInsert.Parameters.AddWithValue("@FTpct", team.statistics.freeThrowsPercentage);
                TeamBoxInsert.Parameters.AddWithValue("@RebD", team.statistics.reboundsDefensive);
                TeamBoxInsert.Parameters.AddWithValue("@RebO", team.statistics.reboundsOffensive);
                TeamBoxInsert.Parameters.AddWithValue("@RebT", team.statistics.reboundsTotal);
                TeamBoxInsert.Parameters.AddWithValue("@Assists", team.statistics.assists);
                TeamBoxInsert.Parameters.AddWithValue("@Turnovers", team.statistics.turnovers);
                TeamBoxInsert.Parameters.AddWithValue("@AtoR", (double)(team.statistics.assists) / (double)(team.statistics.turnovers));
                TeamBoxInsert.Parameters.AddWithValue("@Steals", team.statistics.steals);
                TeamBoxInsert.Parameters.AddWithValue("@Blocks", team.statistics.blocks);
                TeamBoxInsert.Parameters.AddWithValue("@Points", team.statistics.points);
                TeamBoxInsert.Parameters.AddWithValue("@PointsAgainst", PointsAgainst);
            }
        }
        #endregion
    }
}
