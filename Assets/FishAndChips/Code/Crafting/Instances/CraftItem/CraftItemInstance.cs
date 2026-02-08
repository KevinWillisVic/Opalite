using UnityEngine;

namespace FishAndChips
{
	/// <summary>
	/// Core script for CraftItemInstance. Representation of a CraftItem.
	/// </summary>
    public partial class CraftItemInstance : FishScript
    {
		#region -- Properties --
		public IEntity Entity { get; set; }
		public CraftItemEntity CraftItemEntity => Entity != null ? Entity as CraftItemEntity : null;
		public CraftItemData CraftItemData => CraftItemEntity != null ? CraftItemEntity.CraftItemData : null;
		public string InstanceID => CraftItemData != null ? CraftItemData.ID : string.Empty;
		#endregion

		#region -- Public Member Vars --
		public bool IsSelected;
		public bool IsInteractable;
		#endregion

		#region -- Protected Member Vars --
		protected CraftingSystemCraftingService _craftingService;
		#endregion

		#region -- Private Methods --
		private void Update()
		{
			if (CraftItemEntity == null)
			{
				return;
			}

			if (IsInteractable == false)
			{
				return;
			}

			if (IsSelected == true)
			{
				HandleMovement();
			}
		}
		#endregion

		#region -- Protected Methods --
		/// <summary>
		/// Format name of GameObject.
		/// </summary>
		/// <returns>Name to display in hiearchy.</returns>
		protected virtual string FormatName()
		{
			string instanceId = (CraftItemEntity != null) ? CraftItemEntity.InstanceId : string.Empty;
			return $"CraftItemInstance : {instanceId}";
		}

		/// <summary>
		/// Ensure the object is formatted properly.
		/// </summary>
		protected virtual void FormatToDefaultState()
		{
			gameObject.name = FormatName();
			transform.localScale = Vector3.one;
		}
		#endregion

		#region -- Public Methods --
		public virtual void Initialize()
		{
			if (CraftItemEntity == null)
			{
				return;
			}

			if (_craftingService == null)
			{
				_craftingService = CraftingSystemCraftingService.Instance;
			}

			IsSelected = false;
			IsInteractable = true;
			if (_craftingService.IsFinalItem(CraftItemEntity) 
				|| _craftingService.IsDepletedItem(CraftItemEntity))
			{
				IsInteractable = false;
			}

			FormatToDefaultState();
			SetVisual();
		}

		/// <summary>
		/// Add instance back to pool.
		/// </summary>
		/// <param name="immediate">Should the repooling be immediate.</param>
		/// <param name="waitTime">Time before repooling.</param>
		public void Recycle(bool immediate = false, float waitTime = 1)
		{
			IsSelected = false;
			IsInteractable = false;
			EventManager.TriggerEvent(new PositionSaveObjectRemovedEvent(this, immediate, waitTime));
		}

		/// <summary>
		/// Handle selecting instance. Called from EventTrigger component : Pointer Down.
		/// </summary>
		public void OnSelected()
		{
			if (IsInteractable == false)
			{
				return;
			}
			if (IsSelected == true)
			{
				return;
			}
			IsSelected = true;
			AttemptResetCloningVariables();
			EventManager.TriggerEvent(new CraftItemSelectionEvent(this));
		}

		/// <summary>
		/// Handle releasing instance. Called from EventTrigger component : Pointer Up, Drop
		/// </summary>
		public void OnRelease()
		{
			if (IsSelected == false)
			{
				return;
			}
			IsSelected = false;
			EventManager.TriggerEvent(new CraftItemReleasedEvent(this));
		}
		#endregion
	}
}
