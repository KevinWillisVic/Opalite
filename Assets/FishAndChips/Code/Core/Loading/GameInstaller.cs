using UnityEngine;
using System;
using System.Threading.Tasks;

namespace FishAndChips
{
    public abstract class GameInstaller : MonoBehaviour
    {
		#region -- Inspector --
		public new bool DontDestroyOnLoad = true;
		#endregion

		#region -- Protected Methods --
		protected virtual void Awake()
		{
			if (DontDestroyOnLoad == true)
			{
				DontDestroyOnLoad(this);
			}
		}

		protected virtual async void Start()
		{
			await StartGameLoop();
		}

		protected virtual async Awaitable StartGameLoop()
		{
			try
			{
				CreateServicesFromPrefab();
				CreateServices();
				await RunGameInitialization();
			}
			catch (Exception e)
			{
				HandleException(e);
			}
		}

		protected abstract Task RunGameInitialization();

		/// <summary>
		/// Create service instances without prefab.
		/// </summary>
		protected abstract void CreateServices();

		/// <summary>
		/// Instantiate services from prefab.
		/// </summary>
		protected abstract void CreateServicesFromPrefab();

		/// <summary>
		/// Handle exceptions that the game throws.
		/// </summary>
		/// <param name="e">The exception that was thrown.</param>
		protected virtual void HandleException(Exception e)
		{
			Logger.LogException(e);
		}
		#endregion
	}
}
