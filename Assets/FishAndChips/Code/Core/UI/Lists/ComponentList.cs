using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace FishAndChips
{
	public class ComponentList<T> : MonoBehaviour where T : ComponentListItem
	{
		#region -- Properties --
		public virtual List<T> ListItems { get; set; } = new();
		public int Count => ListItems.Count;

		public Action OnListFilled { get; set; }
		public Action OnAllItemAnimationsCompleteAction { get; set; }
		public Action<T> OnListItemHeld { get; set; }
		public Action<T> OnListItemSelected { get; set; }
		#endregion

		#region -- Inspector --
		public T ListItemPrefab;
		public Transform ListItemParent;
		public bool AllowMultiSelect = false;
		public bool AllowAnimatingItemsIn = false;
		public float DelayBetweenItemAnimation = 0.1f;
		#endregion

		#region -- Protected Member Vars --
		protected bool _initializeItemsOnListFilled = true;
		#endregion

		#region -- Protected Methods --
		protected virtual T InstantiateItem(object listObject)
		{
			return Instantiate(original: ListItemPrefab, parent: ListItemParent, worldPositionStays: false);
		}

		protected virtual void ParentItem(T item)
		{
			if (ListItemParent != item.transform.parent)
			{
				item.transform.SetParent(parent: ListItemParent, worldPositionStays: false);
			}
		}

		protected virtual void DestroyListItems()
		{
			if (this == null || gameObject == null || ListItems == null)
			{
				return;
			}

			for (int i = Count - 1; i >= 0; i--)
			{
				var item = ListItems[i];
				if (item == null)
				{
					continue;
				}
				item.ClearItem();
				DestroyImmediate(item.gameObject);
			}
		}

		protected async Task AnimateItemsIn()
		{
			if (AllowAnimatingItemsIn == false)
			{
				OnAllItemAnimationsComplete();
				return;
			}

			List<Task> tasks = new();
			for (int i = Count - 1; i >= 0; i--)
			{
				var item = ListItems[i];

				if (item == null || item.EntryPlayable == null)
				{
					continue;
				}

				item.SetActiveSafe(false);
				// Animation should turn the object on.
				//await item.AnimateIn(DelayBetweenItemAnimation);
				tasks.Add(item.AnimateIn(DelayBetweenItemAnimation));
			}
			await Task.WhenAll(tasks.ToArray());

			OnAllItemAnimationsComplete();
		}

		protected virtual void PreInitializeItem(T item)
		{
			if (item == null)
			{
				return;
			}

			item.OnItemSelected -= OnListItemSelectedInternal;
			item.OnItemSelected += OnListItemSelectedInternal;

			item.OnItemHeld -= OnItemHeldInternal;
			item.OnItemHeld += OnItemHeldInternal;

			item.VisuallySelected(false);
		}

		protected virtual void OnAllItemAnimationsComplete()
		{
			OnAllItemAnimationsCompleteAction.FireSafe();
		}

		protected void OnItemHeldInternal(ComponentListItem selectedItem)
		{
			//foreach (var item in ListItems)
			//{
			//	item.VisuallySelected(selectedItem == item);
			//}
			OnListItemHeld.FireSafe(selectedItem as T);
		}

		protected virtual void OnListFilledInternal()
		{
		}

		protected virtual void OnListItemSelectedInternal(ComponentListItem selectedItem)
		{
			foreach (var item in ListItems)
			{
				item.VisuallySelected(item == selectedItem || (AllowMultiSelect && item.IsVisuallySelected));
			}
			OnListItemSelected.FireSafe(selectedItem as T);
		}
		#endregion

		#region -- Public Methods --
		public virtual async void FillList<ObjectType>(List<ObjectType> objects, bool clear = true, Action<T> preInitFunction = null)
		{
			if (clear == true)
			{
				ClearList();
			}

			int length = objects.Count;
			for (int i = 0; i < length; i++)
			{
				AddListItem(objects[i], preInitFunction);
			}

			if (_initializeItemsOnListFilled == true)
			{
				foreach (var item in ListItems)
				{
					if (item == null)
					{
						continue;
					}
					try
					{
						item.Initialize();
					}
					catch (Exception e)
					{
						Logger.LogException(e);
					}
				}
			}

			UpdateItemIndecies();
			await AnimateItemsIn();
			OnListFilledInternal();
			OnListFilled.FireSafe();
		}

		public virtual void ClearList()
		{
			DestroyListItems();
			ListItems.Clear();
		}

		public virtual void SortItems()
		{
		}

		public virtual T AddListItem(object item, Action<T> preInitFunction = null)
		{
			T newItem = default(T);
			newItem = InstantiateItem(item);

			if (newItem == null)
			{
				return default;
			}

			ParentItem(newItem);
			newItem.Index = Count;
			newItem.ListObject = item;

			PreInitializeItem(newItem);
			preInitFunction.FireSafe(newItem);

			ListItems.Add(newItem);
			return newItem;
		}

		public virtual void AddListItem(T item, int index)
		{
			if (ListItems.Contains(item))
			{
				return;
			}
			PreInitializeItem(item);
			ListItems.Add(item);
			item.Index = index;
		}

		public virtual void RemoveItem(T item, bool destroyObject = true)
		{
			if (ListItems.Contains(item) == false)
			{
				return;
			}
			ListItems.Remove(item);
			if (destroyObject == true)
			{
				Destroy(item.gameObject);
			}
			UpdateItemIndecies();
		}

		public virtual void SetSelectedFromObject(object obj)
		{
			int length = Count;
			for (int i = 0; i < length; i++)
			{
				var item = ListItems[i];
				if (item == null)
				{
					continue;
				}
				bool selected = (item.ListObject == obj);
				if (selected == true)
				{
					item.Selected();
				}
			}
		}

		public virtual T GetListItemFromObject(object obj)
		{
			int length = Count;
			for (int i = 0; i < length; i++)
			{
				var item = ListItems[i];
				if (item == null)
				{
					continue;
				}
				bool selected = (item.ListObject == obj);
				if (selected == true)
				{
					return item;
				}
			}
			return null;
		}

		public virtual void UpdateItemIndecies()
		{
			int length = Count;
			for (int i = 0; i < length; i++)
			{
				var item = ListItems[i];
				if (item != null)
				{
					item.Index = i;
				}
			}
		}

		public virtual void UpdateList()
		{
			int length = Count;
			for (int i = 0; i < length; i++)
			{
				var item = ListItems[i];
				if (item != null)
				{
					item.UpdateItem();
				}
			}
		}

		public virtual void RecalculateLayout()
		{
			var layoutGroup = ListItemParent != null ? ListItemParent.GetComponent<LayoutGroup>() : null;
			if (layoutGroup == null)
			{
				return;
			}

			layoutGroup.CalculateLayoutInputHorizontal();
			layoutGroup.CalculateLayoutInputVertical();

			layoutGroup.SetLayoutHorizontal();
			layoutGroup.SetLayoutVertical();
		}
		#endregion
	}
}
