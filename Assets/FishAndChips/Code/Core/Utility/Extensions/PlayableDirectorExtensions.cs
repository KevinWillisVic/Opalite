using UnityEngine.Playables;
using System.Threading.Tasks;
using UnityEngine;

namespace FishAndChips
{
    public static class PlayableDirectorExtensions
    {
		#region -- Public Methods --
		public static void PlaySafe(this PlayableDirector director)
		{
			if (director != null && director.playableAsset != null)
			{
				director.Play();
			}
		}

		public static void StopSafe(this PlayableDirector director)
		{
			if (director != null && director.playableAsset != null)
			{
				director.Stop();
			}
		}

		public static async Task AwaitPlayable(this PlayableDirector director)
		{
			if (director == null || director.playableAsset == null)
			{
				return;
			}

			if (director.state != PlayState.Playing)
			{
				director.PlaySafe();

				while (director != null && director.state != PlayState.Playing)
				{
					await Awaitable.EndOfFrameAsync();
				}
			}

			while (director != null && director.state == PlayState.Playing)
			{
				await Awaitable.EndOfFrameAsync();
			}
		}
		#endregion
	}
}
