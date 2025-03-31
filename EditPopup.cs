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
    public partial class EditPopup: Form
    {
        public string Value1 { get; private set; } //Server
        public bool ?Value2 { get; private set; }   //Create Database?
        public string Value3 { get; private set; } //Database
        public string Value4 { get; private set; } //Username
        public string Value5 { get; private set; } //Password

        public EditPopup(string mode, bool fileExist, string initial1 = "", bool ?initial2 = true, string initial3 = "", string initial4 = "", string initial5 = "")
        {
            this.Text = "Create or Edit Connection";
            this.Width = 300;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterParent;

            int top = 20;
            int spacing = 30;

            Label lbl1 = new Label() { Text = "Server:", Left = 20, Top = top, AutoSize = true };
            TextBox txt1 = new TextBox() { Text = initial1, Left = 20, Top = top + 20, Width = 240 };
            top += spacing + 20;

            CheckBox chk2 = new CheckBox() { Text = "Create Database?", Left = 20, Top = top, AutoSize = true };
            top += spacing + 20;
            if(fileExist == false)
            {
                chk2.Checked = true;
            }

            Label lbl3 = new Label() { Text = "Database:", Left = 20, Top = top, AutoSize = true };
            TextBox txt3= new TextBox() { Text = initial3, Left = 20, Top = top + 20, Width = 240 };
            top += spacing + 20;

            Label lbl4 = new Label() { Text = "Username:", Left = 20, Top = top, AutoSize = true };
            TextBox txt4 = new TextBox() { Text = initial4, Left = 20, Top = top + 20, Width = 240 };
            top += spacing + 20;

            Label lbl5 = new Label() { Text = "Password:", Left = 20, Top = top, AutoSize = true };
            TextBox txt5 = new TextBox() { Text = initial5, Left = 20, Top = top + 20, Width = 240, UseSystemPasswordChar = true };
            top += spacing + 20;

            Button btnOK = new Button() { Text = "OK", Left = 80, Top = top, Width = 60, DialogResult = DialogResult.OK };
            Button btnCancel = new Button() { Text = "Cancel", Left = 150, Top = top, Width = 70, DialogResult = DialogResult.Cancel };

            btnOK.Click += (s, e) =>
            {
                Value1 = txt1.Text;
                Value2 = chk2.Checked;
                Value3 = txt3.Text;
                Value4 = txt4.Text;
                Value5 = txt5.Text;
                this.Close();
            };

            this.Controls.AddRange(new Control[] { lbl1, txt1, chk2, lbl3, txt3, lbl4, txt4, lbl5, txt5, btnOK, btnCancel });
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
            this.Height = btnOK.Bottom + 60;
        }
    }
}
