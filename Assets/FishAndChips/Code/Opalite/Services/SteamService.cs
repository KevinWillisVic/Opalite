using UnityEngine;

namespace FishAndChips
{
	public class SteamService : Singleton<SteamService>, IInitializable
	{
		#region -- Private Methods --
		private void Awake()
		{
			Initialize();
		}

		private void Start()
		{
			SteamTools.Interface.WhenReady(OnSteamInterfaceReady);
		}

		private void OnSteamInterfaceReady()
		{
			Debug.Log("OnSteamInterfaceReady");
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();
			SteamTools.Game.Initialize();
		}
		#endregion
	}
}
