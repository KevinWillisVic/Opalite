using UnityEngine;
using System.Collections.Generic;

namespace FishAndChips
{
    public class SimpleGameplayBoard : GameplayBoard
    {

		#region -- Properties --
		public eRecycleState RecycleState => _recycleState;
		#endregion

		#region -- Inspector --
		public float DefaultPlacementBuffer = 150f;
		public float DefaultPlacementRadius = 150f;
		#endregion

		#region -- Protected Member Vars --
		protected CraftingSystemCraftingService _craftingService;
		#endregion

		#region -- Private Member Vars --

		// Collection of items that are being cleared.
		private List<BoardElementSaveInfo> _itemsBeingCleared = new();
		#endregion

		#region -- Private Methods --
		private void OnDestroy()
		{
			UnsubscribeEventListeners();
		}

		/// <summary>
		/// Listen to a position save event, if we are in the undo state once the player
		/// Positions something we should stop allowing undo.
		/// </summary>
		private void OnPositionSaveEvent(GeneralPositionSaveEvent saveEvent)
		{
			// Prevent undoing once they have placed something.
			if (_recycleState == eRecycleState.UndoState)
			{
				ConfigureDefaultState();
			}
		}

		/// <summary>
		/// Event callback for 
		/// </summary>
		/// <param name="gameEvent"></param>
		private void OnRecycleActionTriggered(RecycleTriggerableEvent gameEvent)
		{
			HandleRecycleAction();
		}
		#endregion

		#region -- Protected Methods --
		protected override async void Setup()
		{
			_craftingService = CraftingSystemCraftingService.Instance;

			SubscribeEventListeners();
			ConfigureDefaultState();

			await Awaitable.EndOfFrameAsync();

			if (CraftingLayer != null)
			{
				CraftingLayer.TryGetComponent<RectTransform>(out var rectTransform);
				if (rectTransform != null)
				{
					_boundaryHeight = rectTransform.rect.height;
					_boundaryWidth = rectTransform.rect.width;
				}
			}
		}

		protected override void ConfigureDefaultState()
		{
			base.ConfigureDefaultState();
			_recycleState = eRecycleState.CleanState;
			_itemsBeingCleared.Clear();
			EventManager.TriggerEvent(new RecycleStateUpdateEvent(_recycleState));
		}

		protected override void OnResetGame(GameResetEvent resetEvent)
		{
			MassRecycle();
			ConfigureDefaultState();
		}

		protected override void SubscribeEventListeners()
		{
			base.SubscribeEventListeners();
			EventManager.SubscribeEventListener<GeneralPositionSaveEvent>(OnPositionSaveEvent);
			EventManager.SubscribeEventListener<RecycleTriggerableEvent>(OnRecycleActionTriggered);
		}

		protected override void UnsubscribeEventListeners()
		{
			base.UnsubscribeEventListeners();
			EventManager.UnsubscribeEventListener<GeneralPositionSaveEvent>(OnPositionSaveEvent);
			EventManager.UnsubscribeEventListener<RecycleTriggerableEvent>(OnRecycleActionTriggered);
		}
		#endregion

		#region -- Public Methods --
		public Vector2 GetPositionOnCircle(Vector2 position,
			bool useDefaultBuffer,
			bool useDefaultRadius, 
			float customBuffer = 0f,
			float customRadius = 1f)
		{
			Vector2 circleVec = Vector2.zero;

			float radius = (useDefaultRadius == true) ? DefaultPlacementRadius : customRadius;
			circleVec = circleVec.AssignRandomPointOnCircle(radius);
			position += circleVec;

			float buffer = (useDefaultBuffer == true) ? DefaultPlacementBuffer : customBuffer;

			return GetPositionBoundedToCraftingRegionRectangle(position, buffer);
		}

		public override void MassRecycle()
		{
			_itemsBeingCleared.Clear();
			var objectsOnBoard = CraftingLayer.GetComponentsInChildren<CraftItemInstance>();
			foreach (var item in objectsOnBoard)
			{
				if (item.gameObject.activeSelf)
				{
					var info = new BoardElementSaveInfo(item.CraftItemData.ID, item);
					_itemsBeingCleared.Add(info);
					// TODO : Check how should remove.
					item.Recycle(true);
				}
			}
			EventManager.TriggerEvent(new GeneralPositionSaveEvent());
			_recycleState = eRecycleState.UndoState;
			EventManager.TriggerEvent(new RecycleStateUpdateEvent(_recycleState));
		}

		public void RecycleUnusableItems()
		{
			var finalItems = new List<CraftItemInstance>();
			var objectsOnBoard = CraftingLayer.GetComponentsInChildren<CraftItemInstance>();
			// TODO: Check to see if a special animation should play.
			foreach (var item in objectsOnBoard)
			{
				if (item.gameObject.activeSelf && _craftingService.IsFinalItem(item) == true)
				{
					item.Recycle(true);
				}
			}

			foreach (var item in objectsOnBoard)
			{
				if (item.gameObject.activeSelf && _craftingService.IsDepletedItem(item) == true)
				{
					item.Recycle(true);
				}
			}
		}

		public override void UndoRecycle()
		{
			// TODO : Prevent spawning in depleted items.
			foreach (var item in _itemsBeingCleared)
			{
				var craftItemEntity = _craftingService.FetchCraftItemEntity(item.ID);
				
				var instance = _craftingService.SpawnAndReturnCraftItemInstance(entity: craftItemEntity,
					position: item.Position,
					triggerSaveEvent: true,
					spawnAnimation: string.Empty);

				instance.gameObject.SetActiveSafe(true);
				instance.PlayAnimation(CraftItemInstance.eCraftItemAnimationKeys.CloneAppear.ToString());
			}
			_itemsBeingCleared.Clear();
			_recycleState = eRecycleState.CleanState;
			EventManager.TriggerEvent(new GeneralPositionSaveEvent());
			EventManager.TriggerEvent(new RecycleStateUpdateEvent(_recycleState));
		}

		public void HandleRecycleAction()
		{
			switch (_recycleState)
			{
				case eRecycleState.CleanState:
					{
						MassRecycle();
						break;
					}
				case eRecycleState.UndoState:
					{
						UndoRecycle();
						break;
					}
			}
		}
		#endregion
	}
}
