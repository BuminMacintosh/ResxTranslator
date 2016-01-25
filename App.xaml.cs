using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace ResxTranslator
{
    /// <summary>
    /// App.xaml の相互作用ロジック
    /// </summary>
    public partial class App : Application
    {
		private System.Threading.Mutex mutex = new System.Threading.Mutex(false, "ResxTranslator");

		private void Application_Startup(object sender, StartupEventArgs e)
		{
			if (!mutex.WaitOne(0, false))
			{
				MessageBox.Show(
							"既に起動しています。",
							"二重起動防止",
							MessageBoxButton.OK,
							MessageBoxImage.Information);
				mutex.Close();
				mutex = null;
				this.Shutdown();
			}
			else
			{
				var main = new MainWindow();
				this.MainWindow = main;
				main.Show();
			}
		}

		private void Application_Exit(object sender, ExitEventArgs e)
		{
			if (null != mutex)
			{
				mutex.ReleaseMutex();
				mutex.Close();
				mutex = null;
			}
		}
	}
}
