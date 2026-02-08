using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace FishAndChips
{
	public class AudioComponent : MonoBehaviour
	{
		#region -- Properties --
		public AudioSource AudioSource
		{
			get
			{
				return (_sources != null && _sources.Count > 0) ? _sources[0] : null;
			}
		}
		#endregion

		#region -- Protected Member Vars --
		protected List<AudioSource> _sources = new();
		#endregion

		#region -- Inspector --
		public BaseAudioTypes.eAudioSourceType AudioSourceType;

		public float MaxPitch = 2;
		public float MinPitch = 0.5f;

		public float FadeOutDuration = 0.25f;
		public bool IsFadingOut;
		#endregion

		#region -- Protected Member Vars --
		protected AudioService _audioService;
		#endregion

		#region -- Private Member Vars --
		private int _activeSourceIndex = -1;
		#endregion

		#region -- Private Methods --
		private bool IsClipPlaying(AudioSource audioSource, string clipName)
		{
			if (audioSource.isPlaying == false || audioSource.clip == null)
			{
				return false;
			}
			return audioSource.clip.name == clipName;
		}
		#endregion

		#region -- Public Methods --
		public virtual void Initialize()
		{
			_audioService = AudioService.Instance;
			_sources = GetComponentsInChildren<AudioSource>(includeInactive: true).ToList();

			var mixerGroup = _audioService.GetAudioMixerGroupFromType(AudioSourceType);
			if (mixerGroup != null)
			{
				foreach (var source in _sources)
				{
					source.outputAudioMixerGroup = mixerGroup;
				}
			}
			_activeSourceIndex = _sources.Count > 0 ? 0 : -1;
		}

		public void PlayClip(string clipName,
			float startPercentage = 0,
			float pitch = 1, 
			float startDelay = 0,
			bool loop = false)
		{
			var clip = _audioService.TryGetClip(clipName);
			if (clip == null)
			{
				return;
			}
			PlayClip(clip, startPercentage, pitch, startDelay, loop);
		}

		public void PlayClip(AudioClip clip, 
			float startPercentage = 0, 
			float pitch = 1, 
			float startDelay = 0,
			bool loop = false)
		{
			if (clip == null)
			{
				return;
			}
			if (_sources.Count == 0)
			{
				Logger.LogError("No audio sources detected");
				return;
			}

			if (_activeSourceIndex < 0 || _activeSourceIndex >= _sources.Count)
			{
				_activeSourceIndex = 0;
			}

			var source = _sources[_activeSourceIndex];

			source.loop = loop;
			source.clip = clip;
			source.volume = 1;

			if (clip != null)
			{
				source.time = Mathf.Lerp(0, source.clip.length, startPercentage);
			}
			source.pitch = Mathf.Clamp(pitch, MinPitch, MaxPitch);
			source.PlayDelayed(startDelay);
			_activeSourceIndex++;
		}

		public async void PlayAfterCurrent(AudioClip clip,
			float startPercentage = 0,
			float pitch = 1, 
			float startDelay = 0,
			bool loop = false)
		{
			float timeRemaining = AudioSource.isPlaying ? AudioSource.clip.length - AudioSource.time : 0;
			await Awaitable.WaitForSecondsAsync(timeRemaining);
			PlayClip(clip, startPercentage, pitch, startDelay, loop);
		}

		public void StopClip(string clipName)
		{
			var clip = _audioService.TryGetClip(clipName);
			if (clip == null)
			{
				return;
			}
			StopClip(clip);
		}

		public void StopClip(AudioClip clip)
		{
			if (clip == null)
			{
				return;
			}
			foreach (var source in _sources)
			{
				if (IsClipPlaying(source, clip.name))
				{
					source.Stop();
				}
			}
		}

		public bool IsClipPlaying(string clipName)
		{
			foreach (var source in _sources)
			{
				if (IsClipPlaying(source, clipName))
				{
					return true;
				}
			}
			return false;
		}

		public void Pause()
		{
			foreach (var source in _sources)
			{
				source.Pause();
			}
		}

		public void UnPause()
		{
			foreach (var source in _sources)
			{
				source.UnPause();
			}
		}

		public void FadeOut()
		{
			if (IsFadingOut == false)
			{
				// TODO : Call fade out.
			}
		}
		#endregion
	}
}
