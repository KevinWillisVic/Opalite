using UnityEngine;
using System.Threading.Tasks;

namespace FishAndChips
{
    public class GameSceneInstance : FishScript
    {
		#region -- Inspector --
		[Tooltip("Optional wait before showing the gameplay scene view.")]
		public float DelayBeforeShowingView = 0f;
		#endregion

		#region -- Protected Member Vars --
		// Services.
		protected UIService _uiService;
		#endregion

		#region -- Private Methods --
		/// <summary>
		/// Trigger scene start up events.
		/// </summary>
		private void PrepareScene()
		{
			EventManager.TriggerEvent(new PoolPopulationReady());
			EventManager.TriggerEvent(new OnGameSceneReady());
		}
		#endregion

		#region -- Public Methods --
		/// <summary>
		/// Set up game scene UI. Trigger scene start up events.
		/// </summary>
		public async Task BeginGameplay()
		{
			_uiService = UIService.Instance;
			if (DelayBeforeShowingView > 0)
			{
				await Awaitable.WaitForSecondsAsync(DelayBeforeShowingView);
			}
			_uiService.ActivateView(UIEnumTypes.eViewType.GameplaySceneView.ToString());
			PrepareScene();
		}
		#endregion
	}
}
