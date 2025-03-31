using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Data.SqlClient;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using Newtonsoft.Json;

using Newtonsoft.Json;

namespace NBAdbToolbox
{
    public partial class Main: Form
    {
        //public static Utlilities Utilities = new Utlilities();
        static string projectRoot = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"..\.."));
        string configPath = Path.Combine(projectRoot, "Content", "dbconfig.json");
        public Panel pnlWelcome = new Panel();
        private Button btnEdit = new Button();
        private DbConfig config;
        private Label lblStatus = new Label();
        private Label lblServer = new Label();
        private Label lblServerName = new Label();

        private Label lblDB = new Label();
        private Label lblDBName = new Label();
        private Label lblCStatus = new Label();
        private Label lblDbStat = new Label();
        private PictureBox picStatus = new PictureBox();
        private string iconFile = "";
        private string imagePath = "";
        private Button btnBuild = new Button();
        public string cString = "";
        private SqlConnectionStringBuilder builder;


        private Panel navBar;
        private Panel mainContentPanel;
        private Label lblUtilities = new Label();
        private Panel UtilPanel;
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
            PictureBox courtPreview = new PictureBox
            {
                Image = Image.FromFile(Path.Combine(projectRoot, "Content", "Court.png")),
                SizeMode = PictureBoxSizeMode.StretchImage,
                Width= this.Width, 
                Height= this.Height
            };



            pnlWelcome.BorderStyle = BorderStyle.FixedSingle;
            pnlWelcome.Width = (int)(this.ClientSize.Width / 2.5);
            pnlWelcome.Height = (int)(this.ClientSize.Height / 2.5);
            pnlWelcome.Left = (this.ClientSize.Width - pnlWelcome.Width) / 2;
            pnlWelcome.Top = (this.ClientSize.Height - pnlWelcome.Height) / 2;

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
                SqlConnectionStringBuilder bob = new SqlConnectionStringBuilder();  //This builder connection string
                lblStatus.Text = "Welcome Back!";
                btnEdit.Text = "Edit Server connection";
                btnEdit.Width = (int)(lblStatus.Width / 1.5);
                string json = File.ReadAllText(configPath);
                config = JsonConvert.DeserializeObject<DbConfig>(json);
                //Set label text
                //lblServer.Text += config.Server;
                lblServerName.Text = config.Server;
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
                if (isConnected == true)
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
                CheckServer(cString);
                btnBuild.Enabled = true;
            }





            //This should be second to last i believe.
            //Children elements should go above the parents, background image should be last added.
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
            AddMainElement(this, courtPreview); //Ading background image





            //After the Element is added, set its properties


            pnlWelcome.Parent = courtPreview; //Set Panel parent as the image
            pnlWelcome.BackColor = Color.Transparent; //Set Panel to transparent

            //To set font, i'll need the name, ideal size or pt, and its Style.
            //In addition, i also need the parent element and the child or the element we're working with
            lblStatus.Height = (int)(pnlWelcome.Height * .1);
            float fontSize = ((float)(lblStatus.Height) / (96 / 12)) * (72 / 12); //Formula is picking the correct Pt, as determined by the height of the label
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
            lblServerName.Font = new Font("Segoe UI", fontSize, FontStyle.Bold);

            //Database label Properties
            lblDB.Left = 5;
            lblDB.Top = lblServer.Bottom;
            lblDB.Height = (int)(pnlWelcome.Height * .067);
            lblDB.AutoSize = true;
            fontSize = ((float)(lblDB.Height) / (96 / 12)) * (72 / 12);
            lblDB.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, pnlWelcome, lblDB);
            lblDBName.Left = lblDB.Right - 10;
            lblDBName.Top = lblServer.Bottom;
            lblDBName.AutoSize = true;
            lblDBName.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, pnlWelcome, lblDB);
            

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





            ////SQL Connection status Image and label
            //lblCStatus.Font = SetFontSize("Segoe UI", 16F, FontStyle.Bold, pnlWelcome, lblCStatus);
            //picStatus.Width = 30;
            //picStatus.Height = 30;
            //picStatus.SizeMode = PictureBoxSizeMode.Zoom;            
            //int totalWidth = picStatus.Width + lblCStatus.Width; //Measure combined width            
            //int startX = (pnlWelcome.ClientSize.Width - totalWidth) / 2; //Starting X position to center them together            
            //int topY = lblDB.Bottom + 20; //Vertical position            
            //picStatus.Left = startX - 7; //Position image on the left
            //picStatus.Top = (int)(pnlWelcome.Height - (pnlWelcome.Height * .2));            
            //lblCStatus.Left = picStatus.Right - 2; //Position label on the right
            //lblCStatus.Top = (int)(pnlWelcome.Height - (pnlWelcome.Height * .2));
            //lblCStatus.AutoSize = true;
            //SQL Connection status Image and label

            fontSize = ((float)(lblServer.Height) / (96 / 12)) * (72 / 12)/2;
            lblCStatus.Font = SetFontSize("Segoe UI", fontSize, FontStyle.Bold, pnlWelcome, lblCStatus);
            picStatus.Width = lblServer.Height;
            picStatus.Height = lblServer.Height / 2;
            picStatus.SizeMode = PictureBoxSizeMode.Zoom;    
            //int startX = (pnlWelcome.ClientSize.Width - totalWidth) / 2; //Starting X position to center them together            
            int topY = lblDB.Bottom + 20; //Vertical position            
            picStatus.Top = lblStatus.Top+3;
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
                }
            };


        }


        public void CheckServer(string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand dbCheck = new SqlCommand("select Name from sys.databases where Name = '" + config.Database + "'"))
                {
                    dbCheck.Connection = conn;
                    dbCheck.CommandType = CommandType.Text;
                    conn.Open();
                    SqlDataReader reader = dbCheck.ExecuteReader();
                    if(reader.Read())
                    {
                        lblDbStat.Text = "Database created";
                    }
                    else
                    {
                        lblDbStat.Text = "Need to create Database";
                    }
                }
            }

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
        }

        public void CenterElement(Panel panel, Control control)
        {
            lblStatus.AutoSize = true;
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

            SqlConnectionStringBuilder bob = new SqlConnectionStringBuilder();  //This builder connection string
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

        public void CreateDB(string connectionString)
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand InsertData = new SqlCommand("use master create database " + config.Database))
                {
                    InsertData.Connection = conn;
                    InsertData.CommandType = CommandType.Text;
                    conn.Open();
                    try
                    {
                        InsertData.ExecuteScalar();
                        config.Create = false;
                    }
                    catch (SqlException ex)
                    {

                    }
                    conn.Close();
                }
                imagePath = Path.Combine(projectRoot, "Content", "X.png");
                string build = File.ReadAllText(Path.Combine(projectRoot, "Content", "build.sql"));
                using (SqlCommand InsertData = new SqlCommand("use " + config.Database + build))
                {
                    InsertData.Connection = conn;
                    InsertData.CommandType = CommandType.Text;
                    conn.Open();
                    try
                    {
                        InsertData.ExecuteScalar();
                    }
                    catch (SqlException ex)
                    {

                    }
                    conn.Close();
                }
            }
        }            
    }
}
