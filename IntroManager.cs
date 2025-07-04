using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace NBAdbToolbox
{
    public struct DialogData
    {
        public string Name { get; set; }
        public string Title { get; set; }
        public string Text { get; set; }
        public string DialogType { get; set; }
        public int Frequency { get; set; }
        public int Occurrences { get; set; }
        public string Visibility { get; set; }
        public string[] TutorialSequence { get; set; }
        public int TutorialStep { get; set; }
    }

    public class IntroManager
    {
        private static Dictionary<string, DialogData> dialogs;
        private static string dialogsPath;
        private static bool loaded = false;

        public static void Initialize(string configPath)
        {
            dialogsPath = Path.Combine(configPath, "NewUserIntro.json");
        }

        private static void EnsureLoaded()
        {
            if (loaded) return;

            try
            {
                if (File.Exists(dialogsPath))
                {
                    string json = File.ReadAllText(dialogsPath);
                    dialogs = JsonConvert.DeserializeObject<Dictionary<string, DialogData>>(json)
                             ?? new Dictionary<string, DialogData>();
                }
                else
                {
                    dialogs = new Dictionary<string, DialogData>();
                }
            }
            catch
            {
                dialogs = new Dictionary<string, DialogData>();
            }
            loaded = true;
        }

        public static bool ShouldShow(string dialogKey)
        {
            EnsureLoaded();

            if (!dialogs.TryGetValue(dialogKey, out DialogData dialog))
            {
                return false;
            }

            return dialog.Visibility == "Visible" && dialog.Occurrences < dialog.Frequency;
        }

        public static void ShowInfoBubble(string dialogKey, Control parent, int maxWidth, int maxHeight)
        {
            if (!ShouldShow(dialogKey))
            {
                return;
            }
            var dialog = dialogs[dialogKey];

            //Create and show info bubble
            if(maxWidth == 0)
            {
                maxWidth = 450;
            }
            if(maxHeight == 0)
            {
                maxHeight = 300;
            }
            var bubble = new InfoBubble(dialog, dialogKey, parent, maxWidth, maxHeight);
            bubble.Show();

            //Increment occurrences
            dialog.Occurrences++;

            //If occurrences equals frequency, hide it
            if (dialog.Occurrences >= dialog.Frequency)
            {
                dialog.Visibility = "Hidden";
            }

            //Update the dictionary and save
            dialogs[dialogKey] = dialog;
            SaveAsync();
        }

        private static void SaveAsync()
        {
            try
            {
                string json = JsonConvert.SerializeObject(dialogs, Formatting.Indented);
                File.WriteAllText(dialogsPath, json);
            }
            catch { }
        }

        public static void SetVisibility(string dialogKey, string visibility, bool chkDontShow)
        {
            EnsureLoaded();
            if (dialogs.TryGetValue(dialogKey, out DialogData dialog))
            {
                if(visibility == "Visible" && dialog.Frequency == dialog.Occurrences)
                {
                    dialog.Visibility = "Hidden";
                }
                else
                {
                    dialog.Visibility = visibility;
                }
                if (chkDontShow)
                {
                    dialog.Occurrences = dialog.Frequency;
                }
                dialogs[dialogKey] = dialog;
                SaveAsync();
            }
        }
        public static void HideSpecificBubble(string dialogKey)
        {
            //Find the bgCourt control by searching through all forms
            Control bgCourt = null;
            foreach (Form form in Application.OpenForms)
            {
                foreach(Control c in form.Controls)
                {
                    bgCourt = InfoBubble.FindControlByName(c, "bgCourt");
                    if (bgCourt != null)
                    {
                        foreach (Control c2 in c.Controls)
                        {
                            if (c2 is InfoBubble bubble && bubble.dialogKey == dialogKey)
                            {
                                bubble.Hide();
                                bubble.Dispose();
                                bgCourt.Controls.Remove(c2);
                                break;
                            }
                        }
                        break;
                    }
                }
            }

            if (bgCourt == null)
            {
                return;
            }
        }

        public static void ShowTutorialSequence(string dialogKey, Control parent, int maxWidth, int maxHeight)
        {
            EnsureLoaded();
            if (!dialogs.TryGetValue(dialogKey, out DialogData dialog) || dialog.TutorialSequence == null)
            {
                ShowInfoBubble(dialogKey, parent, maxWidth, maxHeight);
                return;
            }

            ShowSequenceStep(dialog.TutorialSequence, 0, parent, maxWidth, maxHeight);
        }
        private static void ShowSequenceStep(string[] sequence, int stepIndex, Control parent, int maxWidth, int maxHeight)
        {
            if (stepIndex >= sequence.Length)
            {
                return; //End of sequence
            }

            string currentDialogKey = sequence[stepIndex];
            if (!ShouldShow(currentDialogKey))
            {
                //Skip this step and go to next
                ShowSequenceStep(sequence, stepIndex + 1, parent, maxWidth, maxHeight);
                return;
            }

            var dialog = dialogs[currentDialogKey];

            //Create bubble with step counter in title
            string stepTitle = $"{dialog.Title} ({stepIndex + 1}/{sequence.Length})";
            var modifiedDialog = dialog;
            modifiedDialog.Title = stepTitle;

            //Create the bubble but with a callback to show next step
            var bubble = new InfoBubble(modifiedDialog, currentDialogKey, parent, maxWidth, maxHeight);

            //Override the close behavior to advance to next step
            bubble.OnClose = () => ShowSequenceStep(sequence, stepIndex + 1, parent, maxWidth, maxHeight);

            bubble.Show();

            //Mark as shown
            dialog.Occurrences++;
            if (dialog.Occurrences >= dialog.Frequency)
            {
                dialog.Visibility = "Hidden";
            }
            dialogs[currentDialogKey] = dialog;
            SaveAsync();
        }
    }

    public class InfoBubble : UserControl
    {        
        public string dialogKey { get; private set; }  
        public CheckBox chkDontShow;
        private Label closeButton;
        public Action OnClose { get; set; }

        public InfoBubble(DialogData dialog, string key, Control parent, int maxWidth, int maxHeight)
        {
            dialogKey = key;

            InitializeBubble(dialog, parent, maxWidth, maxHeight);
        }
        private void InitializeBubble(DialogData dialog, Control parent, int maxWidth, int maxHeight)
        {
            int interval = 10000; //Default Timer interval
            this.BackColor = Color.LightYellow;
            this.Padding = new Padding(8);
            this.MaximumSize = new Size(maxWidth, maxHeight);
            this.AutoSize = true;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Visible = false;

            var panel = new TableLayoutPanel
            {
                Dock = DockStyle.Fill,
                AutoSize = true,
                ColumnCount = 1,
                RowCount = 3
            };

            //Create close button
            closeButton = new Label
            {
                Text = "×",
                Font = new Font(SystemFonts.DefaultFont.FontFamily, 12, FontStyle.Bold),
                ForeColor = Color.DarkGray,
                AutoSize = true,
                Cursor = Cursors.Hand,
                Margin = new Padding(0)
            };

            //Close button events
            closeButton.Click += (s, e) => CloseBubble();
            closeButton.MouseEnter += (s, e) => closeButton.ForeColor = Color.Red;
            closeButton.MouseLeave += (s, e) => closeButton.ForeColor = Color.DarkGray;

            //Title row panel (if title exists)
            var titlePanel = new Panel
            {
                AutoSize = true,
                Height = 25, //Fixed height for the title row
                Dock = DockStyle.Fill,
                Margin = new Padding(0, 0, 0, 5)
            };

            int titleTarget = this.Width;
            int detailTarget = this.Width;
            var lblTitle = new Label
            {
                Text = dialog.Title,
                Font = new Font(SystemFonts.DefaultFont, FontStyle.Bold),
                AutoSize = true,
                Location = new Point(0, 0),
                Anchor = AnchorStyles.Top | AnchorStyles.Left
            };
            if (dialog.Name == "Welcome Message" || dialog.Name == "Edit Create Popup Explanation")
            {
                titleTarget = (int)(this.Width * 2);
                lblTitle.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width * 2) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, (int)(this.Width * 2), parent);
            }
            else if (dialog.Name == "Build Database Walkthrough")
            {
                titleTarget = this.Width;
                lblTitle.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width ) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, (int)(this.Width), parent);
            }

            //Position close button on the right side of title panel
            closeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            titlePanel.Controls.Add(lblTitle);
            titlePanel.Controls.Add(closeButton);

            //Position close button after adding to panel so we know the panel width
            titlePanel.Resize += (s, e) => {
                closeButton.Location = new Point(titlePanel.Width - closeButton.Width - 5, 2);
            };

            //Add click events to title elements
            titlePanel.Click += (s, e) => CloseBubble();
            lblTitle.Click += (s, e) => CloseBubble();

            panel.Controls.Add(titlePanel);
            

            //Main text
            var lblText = new Label
            {
                Text = dialog.Text,
                AutoSize = true,
                MaximumSize = new Size(350, 0),
                Margin = new Padding(0, 0, 0, 8)
            };
            if (dialog.Name == "Welcome Message" || dialog.Name == "Edit Create Popup Explanation")
            {
                detailTarget = (int)(this.Width * 1.5);
            }
            //lblText.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width * 2) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, (int)(this.Width * 1.5), parent);

            //Add click event to text
            lblText.Click += (s, e) => CloseBubble();

            panel.Controls.Add(lblText);

            //Don't show again checkbox
            chkDontShow = new CheckBox
            {
                Text = "Don't show again",
                AutoSize = true,
                Dock = DockStyle.Left
            };

            //Add click event to checkbox (but don't close if they're checking it)
            chkDontShow.Click += (s, e) => {
                //Don't close immediately, let them check the box
                //But close after a short delay if they want
            };

            panel.Controls.Add(chkDontShow);

            //Add click events to main containers
            panel.Click += (s, e) => CloseBubble();
            this.Click += (s, e) => CloseBubble();

            this.Controls.Add(panel);

            //Position bubble relative to the target control
            //Find bgCourt (the common parent)
            Control bgCourt = FindControlByName(parent, "bgCourt");
            //Convert the target control's position to bgCourt's coordinate system
            Point relativePosition = ConvertControlPosition(parent, bgCourt);
            //Set bgCourt as the parent
            this.Parent = bgCourt;
            bgCourt.Controls.SetChildIndex(this, 0);


            if (dialog.Name == "Welcome Message")
            {
                this.Location = new Point(
                    (bgCourt.ClientSize.Width - this.Width) / 2,
                    ((bgCourt.ClientSize.Height - this.Height) / 2) + parent.Height / 2
                );
            }
            else if(dialog.Name == "Edit Create Popup Explanation")
            {
                interval = interval * 12;
                this.Width += (int)(this.Width * .02);
                this.Location = new Point(
                    ((bgCourt.ClientSize.Width - this.Width) / 2) + 350,
                    ((bgCourt.ClientSize.Height - this.Height) / 2) + parent.Height / 2
                );
            }
            else if (dialog.Name == "Build Database Walkthrough")
            {
                this.Location = new Point(
                    (bgCourt.ClientSize.Width - this.Width) / 2,
                    ((bgCourt.ClientSize.Height - this.Height) / 2) + (int)(parent.Height * 1.5)
                );

            }
            else if (dialog.Name.StartsWith("Database Utilities"))
            {
                titleTarget = this.Width;
                lblTitle.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, titleTarget, lblTitle);
                Control lblDbUtil = FindSpecificControl(dialogKey, parent, "Database Utilities");
                this.Location = new Point(
                    //parent.Width,
                    lblDbUtil.Left + lblDbUtil.Width,
                    parent.Top + lblDbUtil.Height
                );
            }
            else if (dialog.Name.StartsWith("Database Overview"))
            {
                titleTarget = (int)(this.Width * .7);
                Control lblDbOverview = FindSpecificControl(dialogKey, parent, "Database Overview");
                Control lblDbUtil = FindSpecificControl(dialogKey, parent, "Database Utilities");
                this.Location = new Point(
                    (parent.Width - this.Width) / 2,
                    //(int)(parent.Top * 2.8)
                    parent.Top + lblDbUtil.Height + (int)(lblDbOverview.Height * 1.5)
                );
            }
            else if (dialog.Name.StartsWith("Database Options Populate"))
            {
                this.AutoSize = true;
                Control listSeasons = FindSpecificControl(dialogKey, parent, "listSeasons");
                this.Location = new Point(
                    listSeasons.Width + (int)(listSeasons.Left),
                    parent.Top + listSeasons.Top + ((listSeasons.Height - this.Height) / 2)
                );
            }
            else if (dialog.Name.StartsWith("Database Options Refresh"))
            {
                this.AutoSize = true;
                Control btnRefresh = FindSpecificControl(dialogKey, parent, "Refresh 2024 data");
                this.Location = new Point(
                    btnRefresh.Left,
                    parent.Top + btnRefresh.Top + btnRefresh.Height
                );
            }
            else if (dialog.Name.StartsWith("Database Options Download"))
            {
                Control listDownloadSeasonData = FindSpecificControl(dialogKey, parent, "listDownloadSeasonData");
                this.Location = new Point(
                    listDownloadSeasonData.Width + (int)(listDownloadSeasonData.Left),
                    parent.Top + listDownloadSeasonData.Top + ((listDownloadSeasonData.Height - this.Height) / 2)
                );
            }
            else if (dialog.Name.StartsWith("Database Options Repair"))
            {
                this.AutoSize = true;
                Control btnRepair = FindSpecificControl(dialogKey, parent, "Repair Db");
                this.Location = new Point(
                    btnRepair.Left,
                    parent.Top + btnRepair.Top + btnRepair.Height
                );
            }
            //lblTitle.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, titleTarget, lblTitle);
            //lblText.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width * 2) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, detailTarget, parent);
            this.Visible = true;

            if (!dialog.Title.Contains(")"))
            {
                //Auto-dismiss timer
                var timer = new Timer { Interval = interval };
                timer.Tick += (s, e) => { timer.Stop(); CloseBubble(); };
                timer.Start();
            }



        }
        public static Control FindControlByName(Control startControl, string name)
        {
            Control current = startControl;
            while (current != null)
            {
                if (current.Name == name || current.Text == name)
                {
                    return current;
                }
                current = current.Parent;
            }
            return null;
        }

        private Point ConvertControlPosition(Control sourceControl, Control targetParent)
        {
            Point absolutePosition = sourceControl.PointToScreen(Point.Empty);
            return targetParent.PointToClient(absolutePosition);
        }

        private void CloseBubble()
        {
            if (chkDontShow.Checked)
            {
                IntroManager.SetVisibility(dialogKey, "Hidden", chkDontShow.Checked);
            }
            this.Hide();
            this.Dispose();
            OnClose?.Invoke();
        }



        public static Control FindSpecificControl(string dialogKey, Control parent, string lookingFor)
        {
            //Find the bgCourt control by searching through all forms
            Control current = parent;
            foreach (Control c in parent.Controls)
            {
                if (c.Name == lookingFor || c.Text == lookingFor)
                {
                    current = c;
                    return current;
                }
            }
            return null;
            //foreach (Form form in Application.OpenForms)
            //{
            //    foreach (Control c in form.Controls)
            //    {
            //        bgCourt = InfoBubble.FindControlByName(c, "bgCourt");
            //        if (bgCourt != null)
            //        {
            //            foreach (Control c2 in c.Controls)
            //            {
            //                if (c2 is InfoBubble bubble && bubble.dialogKey == dialogKey)
            //                {
            //                    bubble.Hide();
            //                    bubble.Dispose();
            //                    bgCourt.Controls.Remove(c2);
            //                    break;
            //                }
            //            }
            //            break;
            //        }
            //    }
            //}

            //if (bgCourt == null)
            //{
            //    return;
            //}

            ////Search through bgCourt's controls for InfoBubble with matching dialogKey
            //for (int i = bgCourt.Controls.Count - 1; i >= 0; i--)
            //{
            //}
        }
    }
}