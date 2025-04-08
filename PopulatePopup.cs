using Newtonsoft.Json.Linq;
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
    public partial class PopulatePopup : Form
    {
        public PopulatePopup(string seasons)
        {
            this.Text = "Populate Database";
            this.Width = 300;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            int top = 20;
            int spacing = 10;

            Label lbl1 = new Label() { Text = seasons, Left = 20, Top = top, AutoSize = true };
            top += spacing + 20;


            Label lbl2 = new Label() { Text = "Data source:", Left = 20, Top = top, AutoSize = true };
            top += spacing + 20;
            CheckBox chk2 = new CheckBox() { Text = "Historic", Left = 20, Top = top, AutoSize = true };
            CheckBox chk3 = new CheckBox() { Text = "Current", Left = chk2.Right, Top = top, AutoSize = true };
            top += spacing + 20;

            Button btnOK = new Button() { Text = "OK", Left = 80, Top = top, Width = 60, DialogResult = DialogResult.OK };
            Button btnCancel = new Button() { Text = "Cancel", Left = 150, Top = top, Width = 70, DialogResult = DialogResult.Cancel };

            if(seasons == "Seasons selected: ")
            {
                btnOK.Enabled = false;
            }

            btnOK.Click += (s, e) =>
            {
                this.Close();
            };
            this.Controls.AddRange(new Control[] { lbl1, lbl2, chk2, chk3, btnOK, btnCancel });
            this.AcceptButton = btnOK;
            this.CancelButton = btnCancel;
            this.Height = btnOK.Bottom + 60;
        }
    }
}
