using Melio.Draw.SharpDX;
using System.Windows;

namespace Melio.Draw.ViewModel
{
	public class MainWindowVM : WorkspaceVM
	{
		public static readonly DependencyProperty ProjectProperty;
		public ProjectMV Project { get { return GetValue(ProjectProperty) as ProjectMV; } set { SetValue(ProjectProperty, value); } }

		#region Constructor
		static MainWindowVM()
		{
			ProjectProperty = DependencyProperty.Register("Project", typeof(ProjectMV), typeof(MainWindowVM));
		}

		public MainWindowVM(DXElement dxElement) : base("Melio Draw")
		{
			if (IsInDesignMode)
				return;
			Project = ProjectMV.CreateNewProject(this, dxElement);
		}

		#endregion
	}
}