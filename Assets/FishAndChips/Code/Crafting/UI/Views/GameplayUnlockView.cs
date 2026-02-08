using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.Playables;
using UnityEngine;
using TMPro;

namespace FishAndChips
{
    public class GameplayUnlockView : GameView
    {
		#region -- Inspector --
		public Image ImageVisual;
		public TextMeshProUGUI CraftItemName;
		public PlayableDirector UnlockVisualSequence;
		#endregion

		#region -- Protected Member Vars --
		protected CraftingSystemImageService _imageService;
		#endregion

		#region -- Private Member Vars --
		private List<CraftItemEntity> _qeuedDisplayedEntities = new();
		private bool _waitingForInteraction = false;
		#endregion

		#region -- Private Methods --
		private async void DisplayEntity(CraftItemEntity entity)
		{
			var modelData = entity.CraftItemData.CraftItemModelData;
			if (ImageVisual != null)
			{
				ImageVisual.sprite = _imageService.GetCraftImage(modelData.VisualKey);
			}
			CraftItemName.SetTextSafe(modelData.DisplayName);

			while (gameObject != null && gameObject.activeInHierarchy == false)
			{
				await Awaitable.EndOfFrameAsync();
			}
			await UnlockVisualSequence.AwaitPlayable();

			_waitingForInteraction = true;
			// Wait for user input
			while (_waitingForInteraction == true)
			{
				await Awaitable.EndOfFrameAsync();
			}

			if (_qeuedDisplayedEntities.Count > 0)
			{
				entity = _qeuedDisplayedEntities.Pop(0);
				DisplayEntity(entity);
			}
			else
			{
				LeaveUnlockView();
			}
		}

		private void OnCraftItemUnlocked(CraftItemEntityUnlockEvent gameEvent)
		{
			_qeuedDisplayedEntities.Add(gameEvent.CraftItemEntity);
		}

		private void LeaveUnlockView()
		{
			_uiService.ActivateView(UIEnumTypes.eViewType.GameplaySceneView.ToString());

			// TODO : Recycle depleted item.
			var gameplayView = _uiService.GetView(UIEnumTypes.eViewType.GameplaySceneView.ToString()) as GameplaySceneView;
			if (gameplayView != null && gameplayView.SimpleGameplayBoard != null)
			{
				gameplayView.SimpleGameplayBoard.RecycleUnusableItems();
			}
		}
		#endregion

		#region -- Protected Methods --
		protected override void SubscribeListeners()
		{
			base.SubscribeListeners();
			EventManager.SubscribeEventListener<CraftItemEntityUnlockEvent>(OnCraftItemUnlocked);
		}

		protected override void UnsubsribeListeners()
		{
			base.UnsubsribeListeners();
			EventManager.UnsubscribeEventListener<CraftItemEntityUnlockEvent>(OnCraftItemUnlocked);
		}
		#endregion

		#region -- Public Methods --
		public override void Activate()
		{
			_imageService = CraftingSystemImageService.Instance as CraftingSystemImageService;
			if (_qeuedDisplayedEntities.Count > 0)
			{
				var entity = _qeuedDisplayedEntities.Pop(0);
				DisplayEntity(entity);
			}
			base.Activate();
		}

		public void RecieveUserInteraction()
		{
			_waitingForInteraction = false;
		}
		#endregion
	}
}
