using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Media;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace NBAdbToolbox
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            //Show splash screen immediately so user knows app is starting
            SplashForm splash = new SplashForm();
            splash.Show();
            Application.DoEvents(); //Force splash to render

            //Create main form - this calls Main() constructor which does all the initialization
            Main mainForm = new Main();

            //Give a brief moment for any slow operations to complete
            Task.Run(async () =>
            {
                await Task.Delay(10); //Show splash for at least 10ms

                //Switch to main form on UI thread
                splash.Invoke(new Action(() =>
                {
                    splash.Hide();
                    mainForm.Show();
                    splash.Close(); //Clean up splash
                    splash.Dispose();
                }));
            });

            if (PlaySound())
            {
                string projectRoot = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug\", "").Replace(@"\bin\Release\", "");
                string soundPath = Path.Combine(projectRoot, @"Content/Sounds", "Swish.wav");
                if (File.Exists(soundPath))
                {
                    using (SoundPlayer player = new SoundPlayer(soundPath))
                    {
                        player.Play(); //asynch
                    }
                }
            }
            Application.Run(mainForm);
        }

        public static bool PlaySound()
        {
            try
            {
                string projectRoot = AppDomain.CurrentDomain.BaseDirectory.Replace(@"\bin\Debug\", "").Replace(@"\bin\Release\", "");
                string settingsPath = Path.Combine(projectRoot, @"Content", "settings.json");

                if (File.Exists(settingsPath))
                {
                    string json = File.ReadAllText(settingsPath);
                    var settings = JsonConvert.DeserializeObject<Settings>(json);
                    return settings.Sound != "Muted";
                }
                return true;
            }
            catch
            {
                return true;
            }
        }
    }
}