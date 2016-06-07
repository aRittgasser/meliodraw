using System.Windows;
using System.Windows.Input;
using System.Windows.Interactivity;

namespace Melio.Draw.Commands
{

	public sealed class InvokeCommandAction
		: TriggerAction<DependencyObject>
	{

		#region Variablen

		public static readonly DependencyProperty CommandProperty;
		public static readonly DependencyProperty CommandParameterProperty;

		#endregion

		#region Eigenschaften

		public ICommand Command { get { return (ICommand)GetValue(CommandProperty); } set { SetValue(CommandProperty, value); } }
		public object CommandParameter { get { return GetValue(CommandParameterProperty); } set { SetValue(CommandParameterProperty, value); } }

		#endregion

		#region ctor

		static InvokeCommandAction()
		{
			CommandProperty = DependencyProperty.Register("Command", typeof(ICommand), typeof(InvokeCommandAction));
			CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(InvokeCommandAction));
		}

		#endregion

		#region Funktionen

		protected override void Invoke(object parameter)
		{
			if (AssociatedObject == null)
				return;
			var command = Command;
			if (command == null)
				return;
			if (CommandParameter != null)
			{
				if (command.CanExecute(CommandParameter))
					command.Execute(CommandParameter);
			}
			else
			{
				if (command.CanExecute(parameter))
					command.Execute(parameter);
			}
		}

		#endregion

	}

}
