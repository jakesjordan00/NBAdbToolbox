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
        public string Database { get; private set; }
        public string Username { get; private set; }
        public string Password { get; private set; }
        public bool? UseWindowsAuth { get; private set; }

        public EditPopup(string mode, bool fileExists, string initialServer = "", string initialAlias = "", bool? initialCreateDb = true, string initialDb = "", bool? initialWindowsAuth = true, string initialUser = "", string initialPass = "")
        {
            this.Text = "Create or Edit Connection";
            this.Width = 300;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            int top = (int)(this.Height * .1);
            int spacing = (int)(this.Height * .1);

            // Server
            Label lblReq = new Label()
            {
                Text = "*",
                Left = 10,
                Top = top,
                AutoSize = true,
                ForeColor = Color.Red,
                Font = new Font(this.Font.FontFamily, this.Font.Size + 2, FontStyle.Bold)
            };
            Label lblServer = new Label() { Text = "Server:", Left = 20, Top = top, AutoSize = true };
            TextBox txtServer = new TextBox() { Text = initialServer, Left = 20, Top = top + 20, Width = 240 };
            top += spacing + 20;

            Label lblAlias = new Label() { Text = "Server Alias:", Left = 20, Top = top, AutoSize = true };
            TextBox txtAlias = new TextBox() { Text = initialAlias, Left = 20, Top = top + 20, Width = 240 };
            top += spacing + 20;

            // Database
            Label lblDatabase = new Label() { Text = "Database:", Left = 20, Top = top, AutoSize = true };
            TextBox txtDatabase = new TextBox() { Text = initialDb, Left = 20, Top = top + 20, Width = 240 };
            top += spacing + 20;

            // Create DB
            CheckBox chkCreateDb = new CheckBox() { Text = "Create Database?", Left = 20, Top = top, AutoSize = true };
            chkCreateDb.Checked = !fileExists || initialCreateDb == true;
            top += (int)(this.Height * .1);

            // Windows Auth
            CheckBox chkWindowsAuth = new CheckBox() { Text = "Use Windows Authentication", Left = 20, Top = top, AutoSize = true };
            chkWindowsAuth.Checked = initialWindowsAuth == true;
            top += (int)(this.Height * .1);

            // Username
            Label lblUsername = new Label() { Text = "Username:", Left = 20, Top = top, AutoSize = true };
            TextBox txtUsername = new TextBox() { Text = initialUser, Left = 20, Top = top + 20, Width = 240 };
            top += spacing + 20;

            // Password
            Label lblPassword = new Label() { Text = "Password:", Left = 20, Top = top, AutoSize = true };
            TextBox txtPassword = new TextBox() { Text = initialPass, Left = 20, Top = top + 20, Width = 240, UseSystemPasswordChar = true };
            top += spacing + 20;

            if (initialWindowsAuth == true)
            {
                txtUsername.Enabled = false;
                txtPassword.Enabled = false;
            }
            // Toggle username/password fields based on checkbox
            chkWindowsAuth.CheckedChanged += (s, e) =>
            {
                bool useAuth = chkWindowsAuth.Checked;
                txtUsername.Enabled = !useAuth;
                txtPassword.Enabled = !useAuth;
            };

            // Buttons
            Button btnOK = new Button() { Text = "OK", Left = 80, Top = top, Width = 60, DialogResult = DialogResult.OK };
            Button btnCancel = new Button() { Text = "Cancel", Left = 150, Top = top, Width = 70, DialogResult = DialogResult.Cancel };

            btnOK.Click += (s, e) =>
            {
                Server = txtServer.Text;
                Alias = txtAlias.Text;
                CreateDatabase = chkCreateDb.Checked;
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
                chkWindowsAuth,
                lblUsername, txtUsername,
                lblPassword, txtPassword,
                btnOK, btnCancel
            });

            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
            this.Height = btnOK.Bottom + 60;
        }
    }
}
