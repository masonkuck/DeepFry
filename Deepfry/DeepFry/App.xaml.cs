using Microsoft.Win32;
using System;
using System.Drawing;
using System.Windows;
using DeepFryCore;

namespace DeepFry
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            OpenFileDialog ofd = new OpenFileDialog();
            if (ofd.ShowDialog() == true)
            {
                DeepFryUtility utility = new DeepFryUtility(ofd.FileName);
                Bitmap deepFried = utility.DeepFry();

                utility.Save(deepFried);
            }

            Environment.Exit(1);
        }

    }
}
