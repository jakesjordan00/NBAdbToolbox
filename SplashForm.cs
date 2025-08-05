using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NBAdbToolbox
{
    public partial class SplashForm : Form
    {
        public SplashForm()
        {
            InitializeComponent();
            SetupSplash();
        }
        private void SetupSplash()
        {
            // Remove title bar and borders
            this.FormBorderStyle = FormBorderStyle.None;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.Black; //Use a color you'll never use
            this.TransparencyKey = Color.Black; //Make that color transparent

            // Add your splash image (use any existing image from your Content\Images folder)
            string projectRoot = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug\", "").Replace(@"\bin\Release\", "");
            string imagePath = Path.Combine(projectRoot, @"Content\Images", "Success.png"); // Use any existing image

            PictureBox splashImage = new PictureBox();
            if (File.Exists(imagePath))
            {
                splashImage.Image = Image.FromFile(imagePath);
                splashImage.SizeMode = PictureBoxSizeMode.Zoom;
                splashImage.BackColor = Color.Transparent;
                splashImage.Dock = DockStyle.Fill;
                this.Controls.Add(splashImage);
            }

            // Loading text
            Label loadingLabel = new Label();
            loadingLabel.Text = "Loading NBAdbToolbox...";
            loadingLabel.ForeColor = Color.White;
            loadingLabel.BackColor = Color.Black;
            loadingLabel.Font = new Font("Arial", 14);
            loadingLabel.TextAlign = ContentAlignment.MiddleCenter;
            //loadingLabel.Dock = DockStyle.Bottom;
            loadingLabel.Height = 50;
            loadingLabel.AutoSize = true;
            this.Controls.Add(loadingLabel);
            loadingLabel.Top = splashImage.Top + (splashImage.Height / 2);
            loadingLabel.Left = (splashImage.Width - loadingLabel.Width) / 2;
            this.Controls.SetChildIndex(loadingLabel, 0);
        }
    }
}
