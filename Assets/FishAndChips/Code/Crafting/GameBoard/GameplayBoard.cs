using UnityEngine;
using System.Threading.Tasks;

namespace FishAndChips
{
    public abstract class GameplayBoard : MonoBehaviour
    {
		#region -- Supporting --
		public enum eRecycleState
		{
			CleanState,
			UndoState
		}
		#endregion

		#region -- Properties --
		public float BoundaryWidth => _boundaryWidth;
		public float BoundaryHeight => _boundaryHeight;
		#endregion

		#region -- Inspector --
		public Transform CraftingLayer;
		public Transform DragLayer;
		public Transform PopupLayer;
		#endregion

		#region -- Protected Member Vars --
		protected float _boundaryWidth;
		protected float _boundaryHeight;

		protected eRecycleState _recycleState = eRecycleState.CleanState;
		#endregion

		#region -- Private Methods --
		private void Start()
		{
			Setup();
		}
		#endregion

		#region -- Protected Methods --
		protected virtual async void Setup()
		{
			await Task.CompletedTask;
		}

		protected virtual void SubscribeEventListeners()
		{
			EventManager.SubscribeEventListener<GameResetEvent>(OnResetGame);
		}

		protected virtual void UnsubscribeEventListeners()
		{
			EventManager.UnsubscribeEventListener<GameResetEvent>(OnResetGame);
		}

		protected virtual void OnResetGame(GameResetEvent resetEvent)
		{
			MassRecycle();
		}

		protected virtual void ConfigureDefaultState()
		{
		}
		#endregion

		#region -- Public Methods --
		public virtual Vector2 GetPositionBoundedToCraftingRegionRectangle(Vector2 position, float buffer = 0f)
		{
			float leftBound = -(_boundaryWidth / 2f) + buffer;
			float rightBound = (_boundaryWidth / 2f) - buffer;
			position.x = Mathf.Clamp(position.x, leftBound, rightBound);

			float topBound = (_boundaryHeight / 2f) - buffer;
			float bottomBound = -(_boundaryHeight / 2f) + buffer;
			position.y = Mathf.Clamp(position.y, bottomBound, topBound);

			return position;
		}

		/// <summary>
		/// Recycle all elements on board.
		/// </summary>
		public virtual void MassRecycle()
		{
		}

		/// <summary>
		/// Undo recycle just done.
		/// </summary>
		public virtual void UndoRecycle()
		{
		}
		#endregion
	}
}
