using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ToolTip;
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
        public static Dictionary<string, DialogData> dialogs;
        public static string dialogsPath;
        public static bool loaded = false;
        public static int currentStep = 0;

        public static void Initialize(string configPath)
        {
            dialogsPath = Path.Combine(configPath, "NewUserIntro.json");
        }

        public static void EnsureLoaded()
        {
            if (loaded)
            {
                return;
            } 

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
            if (DontShow.Contains(dialogKey))
            {
                return false;
            }

            return dialog.Visibility == "Visible" && dialog.Occurrences < dialog.Frequency;
        }

        public static void ShowInfoBubble(string dialogKey, Control parent, int maxWidth, int maxHeight, int windowWidth, int windowHeight)
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
            var bubble = new InfoBubble(dialog, dialogKey, parent, maxWidth, maxHeight, windowWidth, windowHeight);
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

        public static void SaveAsync()
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
            DontShow = "";
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

        public static void ShowTutorialSequence(string dialogKey, Control parent, int maxWidth, int maxHeight, int windowWidth, int windowHeight)
        {
            EnsureLoaded();
            if (!dialogs.TryGetValue(dialogKey, out DialogData dialog) || dialog.TutorialSequence == null)
            {
                ShowInfoBubble(dialogKey, parent, maxWidth, maxHeight, windowWidth, windowHeight);
                return;
            }

            ShowSequenceStep(dialog.TutorialSequence, 0, parent, maxWidth, maxHeight, windowWidth, windowHeight);
        }
        public static string lastDialogKey = "";
        public static string nextDialogKey = "";
        public static string DontShow = "";
        public static void ShowSequenceStep(string[] sequence, int stepIndex, Control parent, int maxWidth, int maxHeight, int windowWidth, int windowHeight)
        {
            if (stepIndex >= sequence.Length)
            {
                return; //End of sequence
            }

            string currentDialogKey = sequence[stepIndex];
            if (stepIndex != 0)
            {
                lastDialogKey = sequence[stepIndex - 1];
            }
            if (!ShouldShow(currentDialogKey))
            {
                //Skip this step and go to next
                ShowSequenceStep(sequence, stepIndex + 1, parent, maxWidth, maxHeight, windowWidth, windowHeight);
                return;
            }
            DontShow += currentDialogKey;


            var dialog = dialogs[currentDialogKey];
            dialogPublic = dialog;
            sequencePublic = sequence;

            //Create bubble with step counter in title
            string stepTitle = $"{dialog.Title} ({stepIndex + 1}/{sequence.Length})";
            var modifiedDialog = dialog;
            modifiedDialog.Title = stepTitle;

            //Create the bubble but with a callback to show next step
            if(stepIndex == 1) //Database Overview
            {
                //maxWidth = 0;
                maxHeight = (int)(windowHeight * .25);
                //maxHeight = 0;
            }
            else if(stepIndex == 2) //Populate Db
            {
                maxHeight = (int)(windowHeight * .1);
            }
            else if (stepIndex == 3) //Refresh Db
            {
                maxHeight = (int)(windowHeight * .11);
            }
            else if (stepIndex == 4) //Download data
            {
                maxHeight = (int)(windowHeight * .135);
            }
            else if (stepIndex == 5) //Repair Db
            {
                maxHeight = (int)(windowHeight * .11);
            }



            //Override the close behavior to advance to next step
            var bubble = new InfoBubble(modifiedDialog, currentDialogKey, parent, maxWidth, maxHeight, windowWidth, windowHeight);
            bubble.OnClose = () => ShowSequenceStep(sequence, stepIndex + 1, parent, maxWidth, maxHeight, windowWidth, windowHeight);
            bubble.Show();
            
            //HideSpecificBubble(currentDialogKey);

            currentStep++;
            //Mark as shown
            dialog.Occurrences++;
            if (dialog.Occurrences >= dialog.Frequency)
            {
                dialog.Visibility = "Hidden";
            }
            dialogs[currentDialogKey] = dialog;
            dialogPublic = dialog;
            sequencePublic = sequence;
            SaveAsync();
        }
        public static DialogData dialogPublic = new DialogData();
        public static string[] sequencePublic;

        public static void TutorialCheckChange()
        {
            if (InfoBubble.chkDontShowTutorial.Checked)
            {
                dialogPublic.Visibility = "Hidden";
                for (int i = 0; i < 6; i++)
                {
                    string tutorialDialogKey = sequencePublic[i];
                    var tutorialDialog = dialogs[tutorialDialogKey];
                    tutorialDialog.Visibility = "Hidden";
                    dialogs[tutorialDialogKey] = tutorialDialog;
                }
            }
            else
            {                
                dialogPublic.Visibility = "Visible";
                for (int i = 0; i < 5; i++)
                {
                    string tutorialDialogKey = sequencePublic[i];
                    var tutorialDialog = dialogs[tutorialDialogKey];
                    tutorialDialog.Visibility = "Visible";
                    dialogs[tutorialDialogKey] = tutorialDialog;
                }

            }
            SaveAsync();
        }

    }

    public class InfoBubble : UserControl
    {        
        public string dialogKey { get; set; }  
        public static CheckBox chkDontShow;
        public static CheckBox chkDontShowTutorial;
        public Label closeButton;
        public Action OnClose { get; set; }
        public Control bgCourtPublic = new Control();
        public int currentStep = 0;

        public InfoBubble(DialogData dialog, string key, Control parent, int maxWidth, int maxHeight, int windowWidth, int windowHeight)
        {
            dialogKey = key;

            InitializeBubble(dialog, parent, maxWidth, maxHeight, windowWidth, windowHeight);
        }
        public void InitializeBubble(DialogData dialog, Control parent, int maxWidth, int maxHeight, int windowWidth, int windowHeight)
        {
            int interval = 30000; //Default Timer interval - 30 seconds
            this.BackColor = Color.LightYellow;
            this.Padding = new Padding(4, 8, 8, 0);
            this.MaximumSize = new Size(maxWidth, maxHeight);
            this.AutoSize = true;
            this.BorderStyle = BorderStyle.FixedSingle;
            this.Visible = false;
            this.Name = dialogKey + "Dialog";
            float closeButtonFont = 0;
            if (windowWidth < 1700)
            {
                closeButtonFont = 10f;
            }
            else
            {
                closeButtonFont = 12f;
            }

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
                Font = new Font(SystemFonts.DefaultFont.FontFamily, closeButtonFont, FontStyle.Bold),
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
                Margin = new Padding(0, 0, 0, 4)
            };

            int titleTarget = this.Width;
            int textTarget = this.Width;
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
                if(windowWidth < 1700)
                {
                    titleTarget = (int)(this.Width * 1.3);
                }
                lblTitle.Font = Main.SetFontSize("Segoe UI", ((float)(titleTarget) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, titleTarget, parent);

            }
            else if (dialog.Name == "Build Database Walkthrough")
            {
                titleTarget = this.Width;
                if (windowWidth < 1700)
                {
                    titleTarget = (int)(this.Width * .9);
                }
                lblTitle.Font = Main.SetFontSize("Segoe UI", ((float)(titleTarget) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, titleTarget, parent);
            }

            //Position close button on the right side of title panel
            closeButton.Anchor = AnchorStyles.Top | AnchorStyles.Right;

            titlePanel.Controls.Add(lblTitle);
            titlePanel.Controls.Add(closeButton);

            //Position close button after adding to panel so we know the panel width
            titlePanel.Resize += (s, e) => {
                closeButton.Location = new Point(titlePanel.Width - closeButton.Width - 5, 2);
            };
            panel.Controls.Add(titlePanel);
            

            //Main text
            var lblText = new Label
            {
                Text = dialog.Text,
                AutoSize = true,
                MaximumSize = new Size(maxWidth - (int)(maxWidth * .1), maxHeight),
                Margin = new Padding(4, 0, 0, 0)
            };
            if (dialog.Name == "Welcome Message")
            {
                textTarget = (int)(this.Width * 1.5);
                if (windowWidth < 1700)
                {
                    textTarget = this.Width;
                }
            }
            else if (dialog.Name == "Edit Create Popup Explanation")
            {
                textTarget = (int)(this.Width * 1.4);
                if (windowWidth < 1700)
                {
                    textTarget = (int)(this.Width * .9);
                }
            }
            panel.Controls.Add(lblText);

            //Don't show again checkbox
            var checkboxPanel = new TableLayoutPanel
            {
                AutoSize = true,
                Dock = DockStyle.Left,
                Height = 10,
                Margin = new Padding(0, 0, 0, 8),
                ColumnCount = 2,
                RowCount = 1
            };

            chkDontShow = new CheckBox
            {
                Text = "Don't show again",
                AutoSize = true,
                Dock = DockStyle.Left
            };
            checkboxPanel.Controls.Add(chkDontShow);

            if (this.Name.StartsWith("Database"))
            {
                chkDontShowTutorial = new CheckBox
                {
                    Text = "Don't show tutorial sequence again",
                    AutoSize = true,
                    Dock = DockStyle.Right
                };
                checkboxPanel.Controls.Add(chkDontShowTutorial);
            }
            panel.Controls.Add(checkboxPanel);

            this.Controls.Add(panel);

            //Find bgCourt (the common parent)
            Control bgCourt = FindControlByName(parent, "bgCourt");
            bgCourtPublic = FindControlByName(parent, "bgCourt");
            Control pnlWelcome = FindSpecificControl(bgCourt, "pnlWelcome");
            Control pnlDbUtil = FindSpecificControl(bgCourt, "pnlDbUtil");
            Control pnlLoad = FindSpecificControl(bgCourt, "pnlLoad");
            Control pnlDbLibrary = FindSpecificControl(bgCourt, "pnlDbLibrary");
            //Set bgCourt as the parent
            this.Parent = bgCourt;
            bgCourt.Controls.SetChildIndex(this, 0);


            if (windowWidth < 1700)
            {
                textTarget = (int)(this.Width * .9);
            }
            if (dialog.Name == "Welcome Message")
            {
                if (windowWidth < 1700)
                {
                    textTarget = (int)(this.Width);
                }
                lblText.Font = Main.SetFontSize("Segoe UI", ((float)(textTarget) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, textTarget, parent);
                this.AutoSize = true;
                this.Location = new Point(
                    (bgCourt.Width - this.Width) / 2,
                    pnlWelcome.Top + parent.Top + parent.Height + (int)(parent.Height * .1)
                );
            }
            else if(dialog.Name == "Edit Create Popup Explanation")
            {
                interval = interval * 12;
                lblText.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width * 2) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, textTarget, parent);
                int bW = this.Width; //368, unchanged after autosize
                int bH = this.Height;//420, unchanged
                lblText.AutoSize = true;
                this.Location = new Point(
                    ((bgCourt.ClientSize.Width - this.Width) / 2) + this.Width,
                    ((bgCourt.ClientSize.Height - this.Height) / 2) + parent.Height / 2
                );
            }
            else if (dialog.Name == "Build Database Walkthrough")
            {
                titleTarget = (int)(this.Width * .9);
                lblTitle.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, titleTarget, lblTitle);
                textTarget = (int)(this.Width * 2);
                lblText.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, textTarget, lblText);
                this.Location = new Point(
                    (bgCourt.ClientSize.Width - this.Width) / 2,
                    ((bgCourt.ClientSize.Height - this.Height) / 2) + (int)(parent.Height * 1.5)
                );

            }
            else if (dialog.Name.StartsWith("Database Utilities"))
            {
                titleTarget = (int)(this.Width * .9);
                lblTitle.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, titleTarget, lblTitle);
                textTarget = (int)(this.Width * 1.25);
                lblText.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, textTarget, lblText);
                Control lblDbUtil = FindSpecificControl(parent, "Database Utilities");
                this.Location = new Point(
                    //parent.Width,
                    lblDbUtil.Left + lblDbUtil.Width,
                    parent.Top + lblDbUtil.Height
                );
            }
            else if (dialog.Name.StartsWith("Database Overview"))
            {
                titleTarget = (int)(this.Width * .65);
                lblTitle.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, titleTarget, lblTitle);
                textTarget = (int)(this.Width * 1.25);
                lblText.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, textTarget, lblText);
                Control lblDbOverview = FindSpecificControl(parent, "Database Overview");
                Control lblDbUtil = FindSpecificControl(parent, "Database Utilities");
                lblText.AutoSize = true;
                this.Location = new Point(
                    (parent.Width - this.Width) / 2,
                    //(int)(parent.Top * 2.8)
                    parent.Top + lblDbUtil.Height + (int)(lblDbOverview.Height * 1.5)
                );
            }
            else if (dialog.Name.StartsWith("Database Options Populate"))
            {
                titleTarget = this.Width;
                lblTitle.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, titleTarget, lblTitle);
                textTarget = (int)(this.Width * 1.25);
                lblText.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, textTarget, lblText);
                Control listSeasons = FindSpecificControl(parent, "listSeasons");
                this.Location = new Point(
                    listSeasons.Width + (int)(listSeasons.Left),
                    parent.Top + listSeasons.Top + ((listSeasons.Height - this.Height) / 2)
                );
            }
            else if (dialog.Name.StartsWith("Database Options Refresh"))
            {
                titleTarget = this.Width;
                lblTitle.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, titleTarget, lblTitle);
                textTarget = (int)(this.Width * 1.75);
                lblText.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, textTarget, lblText);
                Control btnRefresh = FindSpecificControl(pnlDbUtil, "btnRefresh");
                this.Location = new Point(
                    btnRefresh.Left,
                    parent.Top + btnRefresh.Top + btnRefresh.Height
                );
            }
            else if (dialog.Name.StartsWith("Database Options Download"))
            {
                titleTarget = (int)(this.Width);
                lblTitle.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, titleTarget, lblTitle);
                textTarget = (int)(this.Width * 1.55);
                lblText.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, textTarget, lblText);
                Control listDownloadSeasonData = FindSpecificControl(parent, "listDownloadSeasonData");
                this.Location = new Point(
                    listDownloadSeasonData.Width + (int)(listDownloadSeasonData.Left),
                    parent.Top + listDownloadSeasonData.Top + ((listDownloadSeasonData.Height - this.Height) / 2)
                );
            }
            else if (dialog.Name.StartsWith("Database Options Repair"))
            {
                titleTarget = (int)(this.Width);
                lblTitle.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, titleTarget, lblTitle);
                textTarget = (int)(this.Width * 1.5);
                lblText.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, textTarget, lblText);
                Control btnRepair = FindSpecificControl(parent, "btnRepair");
                this.Location = new Point(
                    btnRepair.Left,
                    parent.Top + btnRepair.Top + btnRepair.Height
                );
            }
            //lblTitle.Font = Main.SetFontSize("Segoe UI", ((float)(this.Width) / (96 / 12)) * (72 / 12) / 2, FontStyle.Bold, titleTarget, lblTitle);
            this.Visible = true;

            if (!dialog.Title.Contains(")"))
            {
                //Auto-dismiss timer
                var timer = new Timer { Interval = interval };
                timer.Tick += (s, e) => { timer.Stop(); CloseBubble(); };
                timer.Start();
            }


            //Add click events to main containers
            if (dialog.TutorialStep == 0)
            {
                panel.Click += (s, e) => CloseBubble();
                this.Click += (s, e) => CloseBubble();
                titlePanel.Click += (s, e) => CloseBubble();
                lblTitle.Click += (s, e) => CloseBubble();
                lblText.Click += (s, e) => CloseBubble();
                bgCourt.Click += (s, e) => CloseBubble();
                pnlWelcome.Click += (s, e) => CloseBubble();
                pnlDbUtil.Click += (s, e) => CloseBubble();
                pnlDbLibrary.Click += (s, e) => CloseBubble();
            }
            else
            {
                panel.Click += (s, e) => CloseBubble();
                this.Click += (s, e) => CloseBubble();
                titlePanel.Click += (s, e) => CloseBubble();
                lblTitle.Click += (s, e) => CloseBubble();
                lblText.Click += (s, e) => CloseBubble();
                bgCourt.Click += (s, e) => CloseBubble();
                pnlWelcome.Click += (s, e) => CloseBubble();
                pnlDbUtil.Click += (s, e) => CloseBubble();
                pnlDbLibrary.Click += (s, e) => CloseBubble();
            }
            if(dialog.TutorialStep != 0)
            {
                chkDontShowTutorial.CheckedChanged += (s, e) => {
                    IntroManager.TutorialCheckChange();
                };
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


        public void CloseBubble()
        {
            if (this.IsDisposed || this.Disposing)
            {
                return;
            }
            if (chkDontShow.Checked)
            {
                IntroManager.SetVisibility(dialogKey, "Hidden", chkDontShow.Checked);
            }
            Control bgCourt = FindControlByName(this.Parent, "bgCourt");
            if (bgCourt == null)
            {
                bgCourt = bgCourtPublic;
            }
            Control pnlWelcome = FindSpecificControl(bgCourt, "pnlWelcome");
            Control pnlDbUtil = FindSpecificControl(bgCourt, "pnlDbUtil");
            Control pnlDbLibrary = FindSpecificControl(bgCourt, "pnlDbLibrary");
            bgCourt.Click -= (s, e) => CloseBubble();
            pnlWelcome.Click -= (s, e) => CloseBubble();
            pnlDbUtil.Click -= (s, e) => CloseBubble();
            pnlDbLibrary.Click -= (s, e) => CloseBubble();
            if (this.Parent != null)
            {
                this.Parent.Controls.Remove(this);
            }
            this.Dispose();
            
            OnClose?.Invoke();
        }


        public static Control FindSpecificControl(Control parent, string lookingFor)
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