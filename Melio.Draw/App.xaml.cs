using System;
using System.Windows;
using System.Windows.Threading;
using Melio.Draw.ViewModel;

namespace Melio.Draw
{
	/// <summary>
	/// Interaktionslogik für "App.xaml"
	/// </summary>
	public partial class App : Application
	{

		#region Variablen

		private MainWindowVM vm;

		#endregion

		#region Ereignishandler

		protected override void OnStartup(StartupEventArgs e)
		{
			base.OnStartup(e);

			bool closeHandlerCalled = false;
			MainWindow window = new MainWindow();
			vm = new MainWindowVM(window.Editor);
            window.Closing += async (sender, args) => {
				if (!closeHandlerCalled)
				{
					args.Cancel = true;
					await window.Dispatcher.BeginInvoke(new Action<object>(vm.CloseCommand.Execute), DispatcherPriority.ApplicationIdle, new object[] { null });
				}
			};
			window.Closed += (sender, args) => {
				if (vm != null)
				{
					vm.Dispose();
					vm = null;
				}
			};
			vm.RequestClose += (sender, args) => {
				// Testen ob etwas gespeichert werden muss
				closeHandlerCalled = true;
				window.Close();
			};
			DispatcherUnhandledException += (sender, args) => {
				if (window != null)
				{
					args.Handled = (MessageBox.Show(window,
						string.Format("Ein unbehandelter Fehler ist aufgetreten\r\nSoll fortgefahren werden?\r\n\r\nFehlerdetails:\r\n{0}", args.Exception),
						"Ein unbehandelter Fehler ist aufgetreten",
						MessageBoxButton.YesNo, MessageBoxImage.Error, MessageBoxResult.No) == MessageBoxResult.Yes);
				}
				else
				{
					args.Handled = false;
				}
			};
			window.DataContext = vm;
			window.Show();
		}

		#endregion

	}
}
