using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NBAdbToolbox
{
    public partial class EditPopup : Form
    {
        public string Server { get; private set; }
        public string Alias { get; private set; }
        public bool? CreateDatabase { get; private set; }
        public bool? DefaultDb { get; private set; }
        public string Database { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public bool? UseWindowsAuth { get; private set; }

        public EditPopup(string mode, bool fileExists, string initialServer = "", string initialAlias = "", bool? initialCreateDb = true, bool? initialDefaultDb = true, string initialDb = "", bool? initialWindowsAuth = true, string initialUser = "", string initialPass = "")
        {
            this.Text = "Create or Edit Connection";
            this.Width = 300;
            this.Name = "EditPopup";
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            int top = (int)(this.Height * .1);
            int spacing = (int)(this.Height * .1);
            ToolTip tip = new ToolTip();

            //Server
            Label lblReq = new Label()
            {
                Text = "*",
                Left = 10,
                Top = top,
                AutoSize = true,
                ForeColor = Color.Red,
                Font = new Font(this.Font.FontFamily, this.Font.Size + 2, FontStyle.Bold)
            };
            Label lblServer = new Label() { Text = "Server:", Left = 20, Top = top, AutoSize = true};
            TextBox txtServer = new TextBox() { Text = initialServer, Left = 20, Top = top + 20, Width = 240 };
            top += spacing + 20;
            tip.SetToolTip(lblServer, "If hosting locally, use localhost or your computer's name");

            Label lblAlias = new Label() { Text = "Server Alias:", Left = 20, Top = top, AutoSize = true };
            TextBox txtAlias = new TextBox() { Text = initialAlias, Left = 20, Top = top + 20, Width = 240 };
            top += spacing + 20;
            tip.SetToolTip(lblAlias, "Server nickname to show on UI");

            //Database
            Label lblDatabase = new Label() { Text = "Database:", Left = 20, Top = top, AutoSize = true };
            TextBox txtDatabase = new TextBox() { Text = initialDb, Left = 20, Top = top + 20, Width = 240 };
            top += spacing + 20;
            tip.SetToolTip(lblDatabase, "Name of Database to create or connect to");

            //Create DB
            CheckBox chkCreateDb = new CheckBox() { Text = "Create Database?", Left = 20, Top = top, AutoSize = true };
            chkCreateDb.Checked = !fileExists || initialCreateDb == true;
            top += (int)(this.Height * .1);
            tip.SetToolTip(chkCreateDb, "Check box if Database doesn't exist");

            //Default DB
            CheckBox chkDefaultDb = new CheckBox() { Text = "Set as Default Db", Left = 20, Top = top, AutoSize = true };
            chkDefaultDb.Checked = !fileExists || initialDefaultDb == true;
            top += (int)(this.Height * .1);
            tip.SetToolTip(chkDefaultDb, "Check box if Toolbox should connect to this Db on launch");

            //Windows Auth
            CheckBox chkWindowsAuth = new CheckBox() { Text = "Use Windows Authentication", Left = 20, Top = top, AutoSize = true };
            chkWindowsAuth.Checked = initialWindowsAuth == true;
            top += (int)(this.Height * .1);
            tip.SetToolTip(chkDefaultDb, "Check box to connect with Windows Auth. If unchecked, Username and Password must be filled.");

            //Username
            Label lblUsername = new Label() { Text = "Username:", Left = 20, Top = top, AutoSize = true };
            TextBox txtUsername = new TextBox() { Text = initialUser, Left = 20, Top = top + 20, Width = 240 };
            top += spacing + 20;
            tip.SetToolTip(lblUsername, "Username for your SQL Server connection");

            //Password
            Label lblPassword = new Label() { Text = "Password:", Left = 20, Top = top, AutoSize = true };
            TextBox txtPassword = new TextBox() { Text = initialPass, Left = 20, Top = top + 20, Width = 240, UseSystemPasswordChar = true };
            top += spacing + 20;
            tip.SetToolTip(lblPassword, "Password for your SQL Server connection");

            tip.BackColor = Color.Black;
            tip.ForeColor = Color.Wheat;
            tip.IsBalloon = true; // Rounded bubble style

            lblServer.Paint += (s, e) => {
                ToolTipUnderline(s, e);
            };
            lblAlias.Paint += (s, e) => {
                ToolTipUnderline(s, e);
            };
            lblDatabase.Paint += (s, e) => {
                ToolTipUnderline(s, e);
            };
            chkCreateDb.Paint += (s, e) => {
                ToolTipUnderline(s, e);
            };
            chkDefaultDb.Paint += (s, e) => {
                ToolTipUnderline(s, e);
            };
            chkWindowsAuth.Paint += (s, e) => {
                ToolTipUnderline(s, e);
            };
            lblUsername.Paint += (s, e) => {
                ToolTipUnderline(s, e);
            };
            lblPassword.Paint += (s, e) => {
                ToolTipUnderline(s, e);
            };

            if (initialWindowsAuth == true)
            {
                txtUsername.Enabled = false;
                txtPassword.Enabled = false;
            }
            //Toggle username/password fields based on checkbox
            chkWindowsAuth.CheckedChanged += (s, e) =>
            {
                bool useAuth = chkWindowsAuth.Checked;
                txtUsername.Enabled = !useAuth;
                txtPassword.Enabled = !useAuth;
            };

            //Buttons
            Button btnOK = new Button() { Text = "OK", Left = 80, Top = top, Width = 60, DialogResult = DialogResult.OK };
            Button btnCancel = new Button() { Text = "Cancel", Left = 150, Top = top, Width = 70, DialogResult = DialogResult.Cancel };

            btnOK.Click += (s, e) =>
            {
                Server = txtServer.Text;
                Alias = txtAlias.Text;
                CreateDatabase = chkCreateDb.Checked;
                DefaultDb = chkDefaultDb.Checked;
                Database = txtDatabase.Text;
                UseWindowsAuth = chkWindowsAuth.Checked;
                if (chkWindowsAuth.Checked)
                {
                    Username = null;
                    Password = null;
                }
                else
                {
                    Username = txtUsername.Text;
                    Password = txtPassword.Text;
                }
                this.Close();
            };

            this.Controls.AddRange(new Control[]
            {
                lblReq,lblServer, txtServer,
                lblAlias, txtAlias,
                lblDatabase, txtDatabase,
                chkCreateDb,
                chkDefaultDb,
                chkWindowsAuth,
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                btnOK, btnCancel
            });

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
            this.Height = btnOK.Bottom + 60;


        }

        public void ToolTipUnderline(object s, PaintEventArgs e)
        {
            if (s is Label label)
            {
                using (var pen = new Pen(Color.Gray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawLine(pen, 0, label.Height - 2, label.Width, label.Height - 2);
                }
            }
            else if (s is CheckBox checkBox)
            {
                //Get the text bounds for the checkbox
                var textSize = TextRenderer.MeasureText(checkBox.Text, checkBox.Font);
                var checkBoxSize = 16; //Standard checkbox size
                var textStart = checkBoxSize + 3; //Space after checkbox

                using (var pen = new Pen(Color.Gray, 1))
                {
                    pen.DashStyle = System.Drawing.Drawing2D.DashStyle.Dot;
                    e.Graphics.DrawLine(pen, textStart, checkBox.Height - 2, textStart + textSize.Width, checkBox.Height - 2);
                }
            }
        }

    }
}
