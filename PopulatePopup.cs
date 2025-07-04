using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.Remoting.Lifetime;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NBAdbToolbox
{
    public partial class PopulatePopup : Form
    {
        public bool historic { get; private set; }   //Historic Data?
        public bool current { get; private set; }   //Current Data?
        public PopulatePopup(string seasons, List<int> seasonList)
        {
            this.Text = "Populate Database";
            this.Width = 300;
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.StartPosition = FormStartPosition.CenterScreen;
            int top = 20;
            int spacing = 10;

            Label lbl1 = new Label() { Text = "Seasons selected:", Left = 20, Top = top, AutoSize = true };
            Label lblSeasons = new Label() {Text = seasons.Replace("Seasons selected: ", "") };
            int height = 20;
            if (seasonList.Count > 4)
            {
                top += top;
                lblSeasons.Left = 20;
                height += 10;
                if(seasonList.Count() > 8)
                {
                    height += 10;
                    lblSeasons.Text = lblSeasons.Text.Insert(48, "\n");
                    if (seasonList.Count() > 16)
                    {
                        height += 10;
                        lblSeasons.Text = lblSeasons.Text.Insert(97, "\n");
                    }
                    if (seasonList.Count() > 24)
                    {
                        height += 10;
                        lblSeasons.Text = lblSeasons.Text.Insert(146, "\n");
                    }
                }
            }
            else
            {
                lblSeasons.Left = lbl1.Right;
            }
            lblSeasons.Top = top;
            lblSeasons.Height = height;
            lblSeasons.AutoSize = true;
            top += spacing + 20;

            Label lbl2 = new Label() { Text = "Data source:", Left = 20, Top = lblSeasons.Bottom, AutoSize = true };
            Label lbl3 = new Label() { Text = "Leave unchecked if unsure", Left = lbl2.Right - (lbl2.Width /3), Top = lblSeasons.Bottom, Font = new Font("Segoe UI", lbl2.Font.Size, FontStyle.Bold), AutoSize = true };
            top += spacing + 20;
            CheckBox chkHistoric = new CheckBox() { Text = "Data Files", Left = 20, Top = lbl2.Bottom, AutoSize = true };
            CheckBox chkCurrent = new CheckBox() { Text = "NBA Endpoints", Left = chkHistoric.Right, Top = lbl2.Bottom, AutoSize = true };
            top += spacing + 20; 
            ToolTip tip = new ToolTip();
            tip.BackColor = Color.Black;
            tip.ForeColor = Color.Wheat;
            tip.SetToolTip(lbl2, "2012-2018 is sourced from Data Files only, 2019-2024 can be sourced by either.");
            tip.SetToolTip(chkHistoric, "Data gathered from NBA game pages. " +
                "\nMUCH faster than Endpoints. " +
                "\nValid source for all seasons. " +
                "\nHas more data than current source, but is of lesser quality/detail." +
                "\n\u2003Contains less non-critical PlayByPlay information for each row.");
            tip.SetToolTip(chkCurrent, "Data gathered from BoxScore and PlayByPlay endpoints" +
                "\nOnly valid for 2019-2024. (2019 seems to be missing a few arbitrary games.)" +
                "\nData has more detail per row for PlayByPlay and Boxscore");
            tip.IsBalloon = true; // Rounded bubble style
            


            lbl2.Paint += (s, e) => {
                ToolTipUnderline(s, e);
            };
            chkHistoric.Paint += (s, e) => {
                ToolTipUnderline(s, e);
            };
            chkCurrent.Paint += (s, e) => {
                ToolTipUnderline(s, e);
            };


            Button btnOK = new Button() { Text = "OK", Left = 80, Top = chkCurrent.Bottom, Width = 60, DialogResult = DialogResult.OK };
            Button btnCancel = new Button() { Text = "Cancel", Left = 150, Top = chkCurrent.Bottom, Width = 70, DialogResult = DialogResult.Cancel };

            if(seasons == "Seasons selected: ")
            {
                btnOK.Enabled = false;
            }

            btnOK.Click += (s, e) =>
            {
                historic = chkHistoric.Checked;
                current = chkCurrent.Checked;
                this.Close();
            };
            this.Controls.AddRange(new Control[] { lbl1, lblSeasons, lbl2, lbl3, chkHistoric, chkCurrent, btnOK, btnCancel });
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
