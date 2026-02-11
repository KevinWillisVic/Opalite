#if STEAMWORKS_NET
using Steamworks;
using Heathen.SteamworksIntegration;
using Heathen.SteamworksIntegration.API;
using UnityEngine;

namespace FishAndChips
{
	public class SteamService : Singleton<SteamService>, IInitializable
	{

		#region -- Private Member Vars --
		private UserData _user;
		#endregion

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
			_user = UserData.Me;

			var userName = _user.Name;
			var userLevel = _user.Level;

			Debug.Log($"<color=green> User Name = {userName}, User Level = {userLevel}");
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
#endif
