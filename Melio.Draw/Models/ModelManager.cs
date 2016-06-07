using System.Windows;
using System.Collections.ObjectModel;

namespace Melio.Draw.Models
{
	class ModelManager : DependencyObject
	{
		public static readonly DependencyProperty ItemsProperty;

		public ObservableCollection<BaseModel> Items { get { return GetValue(ItemsProperty) as ObservableCollection<BaseModel>; } set { SetValue(ItemsProperty, value); } }

		static ModelManager(){
			ItemsProperty = DependencyProperty.Register("Items", typeof(ObservableCollection<BaseModel>), typeof(ModelManager));
		}
	}
}
