using System;
using System.Windows;
using Melio.Draw.Models;
using System.Collections.ObjectModel;
using SharpDX.Direct3D11;
using Melio.Draw.SharpDX;

namespace Melio.Draw.ViewModel
{
	public class SelectionManager : DependencyObject
	{

		public static readonly DependencyProperty FocusedItemProperty;
		public static readonly DependencyProperty SelectedItemsProperty;
		public static readonly DependencyProperty AllItemsProperty;

		private static void OnFocusedItemChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			SelectionManager manager = o as SelectionManager;
			if (manager != null)
				manager.OnFocusedItemChanged(e.OldValue as BaseModel, e.NewValue as BaseModel);
		}
		private static void OnSelectedItemsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			SelectionManager manager = o as SelectionManager;
			if (manager != null)
				manager.OnSelectedItemsChanged(e.OldValue as ObservableCollection<BaseModel>, e.NewValue as ObservableCollection<BaseModel>);
		}
		private static void OnAllItemsChanged(DependencyObject o, DependencyPropertyChangedEventArgs e)
		{
			SelectionManager manager = o as SelectionManager;
			if (manager != null)
				manager.OnAllItemsChanged(e.OldValue as ObservableCollection<BaseModel>, e.NewValue as ObservableCollection<BaseModel>);
		}

		protected void OnFocusedItemChanged(BaseModel oldValue, BaseModel newValue)
		{

		}

		protected void OnSelectedItemsChanged(ObservableCollection<BaseModel> oldValue, ObservableCollection<BaseModel> newValue)
		{

		}

		protected void OnAllItemsChanged(ObservableCollection<BaseModel> oldValue, ObservableCollection<BaseModel> newValue)
		{

		}

		public BaseModel FocusedItem
		{
			get
			{
				return GetValue(FocusedItemProperty) as BaseModel;
			}
			set
			{
				if(value != GetValue(FocusedItemProperty))
					SetValue(FocusedItemProperty, value);
			}
		}

		public ObservableCollection<BaseModel> SelectedItems
		{
			get
			{
				return (ObservableCollection<BaseModel>)GetValue(SelectedItemsProperty);
			}
			set
			{
				SetValue(SelectedItemsProperty, value);
			}
		}

		public ObservableCollection<BaseModel> AllItems
		{
			get
			{
				return (ObservableCollection<BaseModel>)GetValue(AllItemsProperty);
			}
			set
			{
				SetValue(AllItemsProperty, value);
			}
		}

		static SelectionManager()
		{
			FocusedItemProperty = DependencyProperty.Register("FocusedItem", typeof(BaseModel), typeof(SelectionManager), new UIPropertyMetadata(null, OnFocusedItemChanged));
			SelectedItemsProperty = DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<BaseModel>), typeof(SelectionManager), new UIPropertyMetadata(new ObservableCollection<BaseModel>(), OnSelectedItemsChanged));
			AllItemsProperty = DependencyProperty.Register("AllItems", typeof(ObservableCollection<BaseModel>), typeof(SelectionManager), new UIPropertyMetadata(new ObservableCollection<BaseModel>(), OnAllItemsChanged));
		}

		public SelectionManager()
		{

		}

		/// <summary>
		/// Sets the specified model to be the only selected <see cref="BaseModel"/>.
		/// </summary>
		/// <param name="model">The model.</param>
		public bool SetSelection(BaseModel model)
		{
			if (FocusedItem != model)
				FocusedItem = model;

			if (SelectedItems.Count > 1)
			{
				SelectedItems.Clear();
				SelectedItems.Add(model);
				return true;
			}
			else if (SelectedItems.Count == 1)
			{
				if (SelectedItems[0] != model)
				{
					SelectedItems.Clear();
					SelectedItems.Add(model);
					return true;
				}
				return false;
			}
			else
			{
				SelectedItems.Add(model);
				return true;
			}
		}
		/// <summary>
		/// Adds the specified model to the list of selected <see cref="BaseModel"/>.
		/// </summary>
		/// <param name="model">The model.</param>
		public bool AddSelection(BaseModel model)
		{
			if(!SelectedItems.Contains(model)){
				SelectedItems.Add(model);
				return true;
            }
			return false;
        }
		/// <summary>
		/// Removes the specified model of the list of selected <see cref="BaseModel"/>.
		/// </summary>
		/// <param name="model">The model.</param>
		public bool RemoveSelection(BaseModel model)
		{
			var success = SelectedItems.Remove(model);
			if(FocusedItem == model)
			{
				if (SelectedItems.Count > 0)
					FocusedItem = SelectedItems[0];
				else
					FocusedItem = null;
            }
			return success;
        }
		/// <summary>
		/// Resets this instance such that nothing is seleted anymore
		/// </summary>
		public bool DeSelectAll()
		{
			if (SelectedItems.Count > 0) {
				SelectedItems.Clear();
				FocusedItem = null;
                return true;
			}
			return false;
        }
		/// <summary>
		/// Toggles the selection state of the specified <see cref="BaseModel"/>.
		/// </summary>
		/// <param name="model">The model.</param>
		public void ToggleSelection(BaseModel model)
		{
			if (SelectedItems.Contains(model))
				SelectedItems.Remove(model);
			else
				SelectedItems.Add(model);
        }
		/// <summary>
		/// Adds a new <see cref="BaseModel"/>.
		/// </summary>
		/// <param name="model">The model.</param>
		public void AddItem(BaseModel model)
		{
			AllItems.Add(model);
			if (SelectedItems.Count > 0)
				SelectedItems.Clear();
			SelectedItems.Add(model);
			FocusedItem = model;
        }
		/// <summary>
		/// Removes the <see cref="BaseModel"/>.
		/// </summary>
		/// <param name="model">The model.</param>
		/// <returns></returns>
		public bool RemoveItem(BaseModel model)
		{
			if(SelectedItems.Contains(model))
			{
				SelectedItems.Remove(model);
            }
			if(FocusedItem == model)
			{
				FocusedItem = null;
            }
			return AllItems.Remove(model);
		}
		/// <summary>
		/// Renders all items.
		/// </summary>
		/// <param name="deviceContext">The device context.</param>
		/// <param name="camera">The camera.</param>
		public void Render(DeviceContext deviceContext, OrthogonalCamera camera)
		{
			foreach (var model in AllItems)
			{
				model.RenderContent(deviceContext, camera);
			}
			foreach (var model in SelectedItems)
			{
				model.RenderBoundingBox(deviceContext, camera);
			}
		}

		public bool DeleteSelected()
		{
			bool result = SelectedItems.Count > 0;
			foreach(var item in SelectedItems)
			{
				AllItems.Remove(item);
			}
			SelectedItems.Clear();
			FocusedItem = null;
            return result;
		}

		public bool SelectAll()
		{
			bool result = AllItems.Count > SelectedItems.Count;

			foreach (var item in AllItems)
			{
				if (!SelectedItems.Contains(item))
					SelectedItems.Add(item);
            }

			return result;
		}
    }
}
