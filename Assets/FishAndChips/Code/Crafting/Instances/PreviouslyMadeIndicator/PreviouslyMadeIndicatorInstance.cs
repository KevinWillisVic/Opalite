using UnityEngine;

namespace FishAndChips
{
	/// <summary>
	/// Core script for PreviouslyMadeIndicatorInstance.
	/// </summary>
    public partial class PreviouslyMadeIndicatorInstance : FishScript
    {
		#region -- Properties --
		public IEntity Entity { get; set; }
		public CraftItemEntity CraftItemEntity => Entity != null ? Entity as CraftItemEntity : null;
		#endregion

		#region -- Protected Member Vars --
		// Services.
		protected CraftingSystemPoolingService _poolingService;
		protected CraftingSystemCraftingService _craftingService;
		#endregion

		#region -- Protected Methods --
		/// <summary>
		/// Format name of GameObject.
		/// </summary>
		/// <returns>Name to display in hiearchy.</returns>
		protected virtual string FormatName()
		{
			string instanceId = (CraftItemEntity != null) ? CraftItemEntity.InstanceId : string.Empty;
			return $"PreviouslyMadeIndicator : {instanceId}";
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
		/// <summary>
		/// Setup Instance. Entity must be set first.
		/// </summary>
		public void Initialize()
		{
			if (CraftItemEntity == null)
			{
				return;
			}
			Initialize(CraftItemEntity);
		}

		/// <summary>
		/// Setup Instance using supplied CraftItemEntity.
		/// </summary>
		/// <param name="entity">Entity used to initialize instance.</param>
		public void Initialize(CraftItemEntity entity)
		{
			Entity = entity;
			if (CraftItemEntity == null)
			{
				return;
			}
			_poolingService = CraftingSystemPoolingService.Instance;
			_craftingService = CraftingSystemCraftingService.Instance;

			FormatToDefaultState();
			SetVisual();
			InitializeMovement();
		}
		#endregion
	}
}
