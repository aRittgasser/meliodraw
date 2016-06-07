using System;
using System.Windows;
using System.Windows.Input;

namespace Melio.Draw.ViewModel
{

	public abstract class WorkspaceVM
		: DisplayNameBaseVM, IDisposable
	{

		#region Variablen

		public static readonly DependencyProperty CloseCommandProperty;
		private static readonly DependencyPropertyKey CloseCommandPropertyKey;
		private bool isDisposed;

		#endregion

		#region Eigenschaften

		public ICommand CloseCommand { get { return GetValue(CloseCommandProperty) as ICommand; } private set { SetValue(CloseCommandPropertyKey, value); } }

		#endregion

		#region Ereignisse

		public event EventHandler RequestClose;

		#endregion

		#region ctor

		static WorkspaceVM()
		{
			CloseCommandPropertyKey = DependencyProperty.RegisterReadOnly("CloseCommand", typeof(ICommand), typeof(WorkspaceVM), new PropertyMetadata(null));
			CloseCommandProperty = CloseCommandPropertyKey.DependencyProperty;
		}

		protected WorkspaceVM(string displayName)
			: base(displayName)
		{
			isDisposed = false;
			CloseCommand = new RelayCommand(param => OnRequestClose());
		}

		~WorkspaceVM()
		{
			Dispose(false);
		}

		#endregion

		#region Funktionen

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		protected virtual void Dispose(bool disposing)
		{
			if (!isDisposed)
				isDisposed = true;
		}

		private void OnRequestClose()
		{
			var handler = RequestClose;
			if (handler != null)
				handler(this, EventArgs.Empty);
		}

		#endregion

	}

	public class RelayCommand
		: ICommand
	{

		#region Variablen

		private readonly Action<object> execute;
		private readonly Predicate<object> canExecute;

		#endregion

		#region Ereignisse

		public event EventHandler CanExecuteChanged
		{
			add { CommandManager.RequerySuggested += value; }
			remove { CommandManager.RequerySuggested -= value; }
		}

		#endregion

		#region ctor

		public RelayCommand(Action<object> executeFn) : this(executeFn, null) { }
		public RelayCommand(Action<object> executeFn, Predicate<object> canExecuteFn)
		{
			if (executeFn == null)
				throw new ArgumentNullException("executeFn", "ExecuteFn is null.");
			if (canExecuteFn == null)
				canExecute = param => true;
			else
				canExecute = canExecuteFn;
			execute = executeFn;
		}

		#endregion

		#region Funktionen

		public bool CanExecute(object parameter)
		{
			return canExecute(parameter);
		}

		public void Execute(object parameter)
		{
			execute(parameter);
		}

		#endregion

	}

}
