using System;
using System.Collections.Generic;
using System.Linq;
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
                await Task.Delay(10); //Show splash for at least 1 second

                //Switch to main form on UI thread
                splash.Invoke(new Action(() =>
                {
                    splash.Hide();
                    mainForm.Show();
                    splash.Close(); //Clean up splash
                    splash.Dispose();
                }));
            });

            Application.Run(mainForm);


            //Application.Run(new Main());
        }
    }
}