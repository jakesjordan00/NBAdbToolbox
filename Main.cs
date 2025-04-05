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

using Newtonsoft.Json;

namespace NBAdbToolbox
{
    public partial class Main: Form
    {
        //Determine whether or not we have a connection to the Database in dbconfig file
        public bool dbConnection = false;
        //File path for project
        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\.."));
        //dbconfig file
        string configPath = Path.Combine(projectRoot, "Content", "dbconfig.json");
        private DbConfig config;

        //Connection String items
        public string cString = "";
        public SqlConnectionStringBuilder bob = new SqlConnectionStringBuilder();  //This builds connection string

        //SQL building items
        public static string buildFile = File.ReadAllText(Path.Combine(projectRoot, "Content", "build.sql"));   //Creates tables
        //Splits procedures off from table creation. For some reason, it won't let me do them at once, even if the formatting I have works straight in SQL.
        public static string procedures = buildFile.Substring(buildFile.IndexOf("~") + 1).Replace("*/", "");
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
        private Label lblDBName = new Label();      //Database name
        private Label lblDbStat = new Label();      //Need to create database/Database created label
        private Button btnEdit = new Button();      //Edit config file
        private Button btnBuild = new Button();     //Build Database


        //pnlDbUtil items
        public Panel pnlDbUtil = new Panel();
        public Label lblDbUtil = new Label { 
        Text = "Database Utilities",
        };
        public Panel pnlSeason = new Panel();           public Label lblSeason = new Label();
        public Panel pnlTeam = new Panel();             public Label lblTeam = new Label();
        public Panel pnlGame = new Panel();             public Label lblGame = new Label();
        public Panel pnlPlayerBox = new Panel();        public Label lblPlayerBox = new Label();
        public Panel pnlTeamBox = new Panel();          public Label lblTeamBox = new Label();
        public Panel pnlPlayer = new Panel();           public Label lblPlayer = new Label();
        public Panel pnlPbp = new Panel();              public Label lblPbp = new Label();
        public Panel pnlTeamBoxLineups = new Panel();   public Label lblTeamBoxLineups = new Label();


        //Header Panels
        public Panel pnlNav = new Panel();
        public Panel pnlScoreboard = new Panel();


        public Main()
        {
            InitializeComponent();
            //Set screen size
            int screenWidth = Screen.PrimaryScreen.WorkingArea.Width;
            int screenHeight = Screen.PrimaryScreen.WorkingArea.Height;
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
                imagePath = Path.Combine(projectRoot, "Content", "X.png");
                picStatus.Image = Image.FromFile(imagePath);
                btnBuild.Enabled = false;
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
                lblDBName.Text = config.Database;

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
                    imagePath = Path.Combine(projectRoot, "Content", "Check.png");
                    picStatus.Image = Image.FromFile(imagePath);
                    btnBuild.Enabled = true;
                }
                else
                {
                    lblCStatus.Text = "Disconnected";
                    lblCStatus.ForeColor = Color.Red;
                    // Load image
                    imagePath = Path.Combine(projectRoot, "Content", "X.png");
                    picStatus.Image = Image.FromFile(imagePath);
                    btnBuild.Enabled = false;
                }
                CheckServer(cString, "main");
            }









            //This should be second to last i believe.
            //Children elements should go above the parents, background image should be last added.
            AddPanelElement(pnlDbUtil, lblDbUtil);
            AddPanelElement(pnlWelcome, lblDbStat);
            AddPanelElement(pnlWelcome, btnBuild);
            AddPanelElement(pnlWelcome, lblCStatus);
            AddPanelElement(pnlWelcome, picStatus);
            AddPanelElement(pnlWelcome, lblDBName);
            AddPanelElement(pnlWelcome, lblDB);
            AddPanelElement(pnlWelcome, lblServerName);
            AddPanelElement(pnlWelcome, lblServer);
            AddPanelElement(pnlWelcome, btnEdit);
            AddPanelElement(pnlWelcome, lblStatus);
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
            //Tables.Width = pnlDbUtil.Width;
            //Tables.Height = pnlDbUtil.Height / 4;
            //Tables.Top = lblDbUtil.Bottom;






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
            lblDBName.Left = lblDB.Right - 10;
            lblDBName.Top = lblServer.Bottom;
            lblDBName.AutoSize = true;
            lblDBName.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);


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
            picStatus.Width = lblServer.Height / 2;
            picStatus.Height = lblServer.Height / 2;
            picStatus.SizeMode = PictureBoxSizeMode.Zoom;    
            //int startX = (pnlWelcome.ClientSize.Width - totalWidth) / 2; //Starting X position to center them together            
            int topY = lblDB.Bottom + 20; //Vertical position            
            picStatus.Top = lblStatus.Top+4;
            lblCStatus.Top = lblStatus.Top;
            lblCStatus.AutoSize = true;
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

            int dims = pnlDbUtil.Width / 3;
            int fullHeight = (dims * 2) + (pnlDbUtil.Width / 2);
            pnlSeason.Click += (s, e) =>
            {
                if (pnlSeason.Focused)
                {
                    this.ActiveControl = null;
                    pnlSeason.Width = dims;
                    pnlSeason.Height = dims;
                    fontSize = ((float)((pnlSeason.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlSeason, lblSeason, lblSeason.Text, fontSize);
                }
                else
                {
                    pnlSeason.Focus();
                    pnlDbUtil.Controls.SetChildIndex(pnlSeason, 1);
                    pnlSeason.Width = pnlDbUtil.Width;
                    pnlSeason.Height = fullHeight;
                    fontSize = ((float)((pnlSeason.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlSeason, lblSeason, lblSeason.Text, fontSize);

                }
            };
            lblSeason.Click += (s, e) =>
            {
                if (pnlSeason.Focused)
                {
                    this.ActiveControl = null;
                    pnlSeason.Width = dims;
                    pnlSeason.Height = dims;
                    fontSize = ((float)((pnlSeason.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlSeason, lblSeason, lblSeason.Text, fontSize);
                }
                else
                {
                    pnlSeason.Focus();
                    pnlDbUtil.Controls.SetChildIndex(pnlSeason, 1);
                    pnlSeason.Width = pnlDbUtil.Width;
                    pnlSeason.Height = fullHeight;
                    fontSize = ((float)((pnlSeason.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlSeason, lblSeason, lblSeason.Text, fontSize);

                }
            };

            pnlTeam.Click += (s, e) =>
            {
                if (pnlTeam.Focused)
                {
                    this.ActiveControl = null;
                    pnlTeam.Left = pnlSeason.Right;
                    pnlTeam.Width = dims;
                    pnlTeam.Height = dims;
                    fontSize = ((float)((pnlTeam.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlTeam, lblTeam, lblTeam.Text, fontSize);
                }
                else
                {
                    pnlTeam.Focus();
                    pnlDbUtil.Controls.SetChildIndex(pnlTeam, 1);
                    pnlTeam.Left = pnlDbUtil.Left;
                    pnlTeam.Width = pnlDbUtil.Width;
                    pnlTeam.Height = fullHeight;
                    fontSize = ((float)((pnlTeam.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlTeam, lblTeam, lblTeam.Text, fontSize);

                }
            };
            lblTeam.Click += (s, e) =>
            {
                if (pnlTeam.Focused)
                {
                    this.ActiveControl = null;
                    pnlTeam.Left = pnlSeason.Right;
                    pnlTeam.Width = dims;
                    pnlTeam.Height = dims;
                    fontSize = ((float)((pnlTeam.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlTeam, lblTeam, lblTeam.Text, fontSize);
                }
                else
                {
                    pnlTeam.Focus();
                    pnlDbUtil.Controls.SetChildIndex(pnlTeam, 1);
                    pnlTeam.Left = pnlDbUtil.Left;
                    pnlTeam.Width = pnlDbUtil.Width;
                    pnlTeam.Height = fullHeight;
                    fontSize = ((float)((pnlTeam.Height * .15)) / (96 / 12)) * (72 / 12);
                    TableLabels(pnlTeam, lblTeam, lblTeam.Text, fontSize);

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
            int targetWidth = (int)(parent.Width * 0.7);
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
            lblDBName.Text = config.Database;

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

            iconFile = isConnected ? "Check.png" : "X.png";
            imagePath = Path.Combine(projectRoot, "Content", iconFile);

            if (File.Exists(imagePath))
                picStatus.Image = Image.FromFile(imagePath);
            else
                picStatus.Image = null;

            if(config.Create == true && isConnected)
            {
                lblDbStat.Text = "Need to create Database";
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
                        conn.Close();
                        dbConnection = true;
                        bob.InitialCatalog = config.Database;
                        btnBuild.Enabled = false;
                        config.Create = false;
                        File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));
                    }
                    else
                    {
                        dbConnection = false;
                        btnBuild.Enabled = true;
                        config.Create = true;
                        File.WriteAllText(configPath, JsonConvert.SerializeObject(config, Formatting.Indented));

                        lblDbStat.Text = "Need to create Database";
                    }
                }
            }

        }

        public void GetTablePanelInfo(string connectionString)
        {
            List<Panel> panels = new List<Panel> { pnlSeason, pnlTeam, pnlPlayer, pnlGame, pnlPlayerBox, pnlTeamBox, pnlPbp, pnlTeamBoxLineups};
            int dims = pnlDbUtil.Width/3;
            int fullHeight = (dims * 6) + pnlDbUtil.Width;
            for (int i = 0; i < panels.Count; i++)
            {
                if(i <= 5)
                {
                    panels[i].Height = dims;
                    panels[i].Width = dims;
                }
                else
                {
                    panels[i].Height = pnlDbUtil.Width / 2;
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
            using (SqlCommand GetTables = new SqlCommand("select t.Name from sys.tables t where type_desc = 'USER_TABLE'"))
            {
                SqlConnection conn = new SqlConnection(bob.ToString());
                GetTables.Connection = conn;
                GetTables.CommandType = CommandType.Text;
                conn.Open();
                using (SqlDataReader sdr = GetTables.ExecuteReader())
                {
                    lblPlayerBox = new Label();
                    lblTeamBox = new Label();
                    lblPlayer = new Label();
                    lblPbp = new Label();
                    lblTeamBoxLineups = new Label();






                    while (sdr.Read())
                    {
                        if (sdr.GetString(0) == "Season")   //Season Panel
                        {
                            lblSeason.Text = sdr.GetString(0);
                            float fontSize = ((float)((panels[0].Height * .15)) / (96 / 12)) * (72 / 12);
                            lblSeason.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
                            AddPanelElement(pnlSeason, lblSeason);
                            CenterElement(pnlSeason, lblSeason);
                        }
                        if (sdr.GetString(0) == "Team")   //Team Panel
                        {
                            lblTeam.Text = sdr.GetString(0);
                            float fontSize = ((float)((panels[0].Height * .15)) / (96 / 12)) * (72 / 12);
                            lblTeam.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
                            AddPanelElement(pnlTeam, lblTeam);
                            CenterElement(pnlTeam, lblTeam);
                        }
                        if (sdr.GetString(0) == "Player")   //PlayerBox Panel
                        {
                            lblGame.Text = sdr.GetString(0);
                            float fontSize = ((float)((panels[0].Height * .15)) / (96 / 12)) * (72 / 12);
                            lblGame.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
                            AddPanelElement(pnlPlayer, lblGame);
                            CenterElement(pnlPlayer, lblGame);
                        }
                        if (sdr.GetString(0) == "Game")   //Game Panel
                        {
                            Label title = new Label();
                            title.Text = sdr.GetString(0);
                            float fontSize = ((float)((panels[0].Height * .15)) / (96 / 12)) * (72 / 12);
                            title.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
                            AddPanelElement(pnlGame, title);
                            CenterElement(pnlGame, title);
                        }
                        if (sdr.GetString(0) == "TeamBox")   //TeamBox Panel
                        {
                            Label title = new Label();
                            title.Text = sdr.GetString(0);
                            float fontSize = ((float)((panels[0].Height * .15)) / (96 / 12)) * (72 / 12);
                            title.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
                            AddPanelElement(pnlTeamBox, title);
                            CenterElement(pnlTeamBox, title);
                        }
                        if (sdr.GetString(0) == "PlayerBox")   //PlayerBox Panel
                        {
                            Label title = new Label();
                            title.Text = sdr.GetString(0);
                            float fontSize = ((float)((panels[0].Height * .15)) / (96 / 12)) * (72 / 12);
                            title.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
                            AddPanelElement(pnlPlayerBox, title);
                            CenterElement(pnlPlayerBox, title);
                        }
                        if (sdr.GetString(0) == "PlayByPlay")   //PlayByPlay Panel
                        {
                            Label title = new Label();
                            title.Text = sdr.GetString(0);
                            float fontSize = ((float)((panels[0].Height * .15)) / (96 / 12)) * (72 / 12);
                            title.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
                            AddPanelElement(pnlPbp, title);
                            CenterElement(pnlPbp, title);
                        }
                        if (sdr.GetString(0) == "TeamBoxLineups")   //PlayByPlay Panel
                        {
                            float fontSize = ((float)((panels[0].Height * .15)) / (96 / 12)) * (72 / 12);
                            TableLabels(pnlTeamBoxLineups, lblTeamBoxLineups, sdr.GetString(0), fontSize);
                        }
                    }
                }
            }
        }

        public void TableLabels(Panel pnl, Label lbl, String text, float fontSize)
        {
            lbl.Text = text;
            lbl.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);
            AddPanelElement(pnl, lbl);
            CenterElement(pnl, lbl);

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
                    }
                    catch (SqlException ex)
                    {

                    }
                    conn.Close();
                }
                for(int i = 0; i < procs.Count; i++)
                {
                    using (SqlCommand CreateProcedures = new SqlCommand(procedures))
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
                GetTablePanelInfo(connectionString);
            }            
        }

        public void FormatProcedures()
        {
            string search = "go";
            List<int> indices = new List<int>();
            int startIndex = 0;
            for (int i = 0; i <= procedures.Length - search.Length; i++)
            {
                if (procedures.Substring(i, search.Length) == search)
                {
                    indices.Add(i);
                    procs.Add(procedures.Substring(startIndex, i));
                    startIndex = i + search.Length;
                }
            }
            for(int i = 0; i < indices.Count; i++)
            {
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
    }
}
