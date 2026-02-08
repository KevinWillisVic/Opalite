using UnityEngine;
using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;

namespace FishAndChips
{
	public class AudioService : Singleton<AudioService> , IInitializable, ICleanable
	{
		#region -- Inspector --
		public AudioMixer MasterMix;

		public AudioComponent SFXAudioSource;
		public AudioComponent MusicAudioSource;
		public AudioComponent UIAudioSource;

		public AudioDatabase AudioDatabase;
		#endregion

		#region -- Protected Member Vars --
		protected Dictionary<string, AudioClip> Music = new();
		protected Dictionary<string, AudioClip> SFX = new();
		#endregion

		#region -- Private Member Vars --
		private List<Dictionary<string, AudioClip>> _audioDatabases = new();
		#endregion

		#region -- Private Methods --
		private IEnumerator FadeOutRoutine(AudioComponent audioComponent, float duration)
		{
			audioComponent.IsFadingOut = true;
			var audioSource = audioComponent.AudioSource;
			float startingVolume = audioSource.volume;

			while (audioSource.volume > 0)
			{
				audioSource.volume -= startingVolume * UnityEngine.Time.deltaTime / duration;
				yield return null;
			}

			audioComponent.IsFadingOut = false;
			audioSource.Stop();
			audioSource.volume = startingVolume;
		}

		private IEnumerator FadeInRoutine(AudioSource audioSource, float duration)
		{
			float startingVolume = 0.2f;
			audioSource.volume = 0;
			audioSource.Play();

			while (audioSource.volume < 1.0f)
			{
				audioSource.volume += startingVolume * UnityEngine.Time.deltaTime / duration;
				yield return null;
			}
			audioSource.volume = -startingVolume;
		}
		#endregion

		#region -- Public Methods --
		public override void Initialize()
		{
			base.Initialize();
			return;


			SFXAudioSource.Initialize();
			MusicAudioSource.Initialize();
			UIAudioSource.Initialize();

			_audioDatabases.Add(Music);
			_audioDatabases.Add(SFX);

			foreach (var mapping in AudioDatabase.MusicClips)
			{
				Music.Add(mapping.Key.ToString(), mapping.Clip);
			}

			foreach (var mapping in AudioDatabase.SFXClips)
			{
				SFX.Add(mapping.Key.ToString(), mapping.Clip);
			}
		}

		public override void Cleanup()
		{
			base.Cleanup();

			_audioDatabases.Clear();

			Music.Clear();
			SFX.Clear();
		}

		public virtual void Focus(bool focus)
		{
			if (focus)
			{
				Pause();
			}
			else
			{
				UnPause();
			}
		}

		public void Pause()
		{
			SFXAudioSource.Pause();
			MusicAudioSource.Pause();
			UIAudioSource.Pause();
		}

		public void UnPause()
		{
			SFXAudioSource.UnPause();
			MusicAudioSource.UnPause();
			UIAudioSource.UnPause();
		}

		public virtual void PlayUISFX(string sfxName, float startPercentage = 0, float pitch = 1, float startDelay = 0f)
		{
			var clip = TryFetch(sfxName, SFX);
			UIAudioSource.PlayClip(clip, startPercentage, pitch, startDelay);
		}

		public AudioMixerGroup GetAudioMixerGroupFromType(BaseAudioTypes.eAudioSourceType audioSourceType)
		{
			switch (audioSourceType)
			{
				case BaseAudioTypes.eAudioSourceType.SFX:
					{
						return MasterMix.FindMatchingGroups("SFX")[0];
					}
				case BaseAudioTypes.eAudioSourceType.Music:
					{
						return MasterMix.FindMatchingGroups("Music")[0];
					}
				case BaseAudioTypes.eAudioSourceType.UI:
					{
						return MasterMix.FindMatchingGroups("SFX")[0];
					}
			}
			return null;
		}

		public float GetMusicVolume()
		{
			MasterMix.GetFloat(CoreConstants.MusicVolumeString, out var returnValue);
			return returnValue;
		}

		public float GetSFXVolume()
		{
			MasterMix.GetFloat(CoreConstants.SFXVolumeString, out var returnValue);
			return returnValue;
		}

		public void FadeMusic(bool fadeOut, float duration)
		{
			if (fadeOut == true)
			{
				FadeOut(MusicAudioSource, duration);
			}
			else
			{
				FadeIn(MusicAudioSource, duration);
			}
		}

		public void FadeIn(AudioComponent source, float duration)
		{
			StartCoroutine(FadeInRoutine(source.AudioSource, duration));
		}

		public void FadeOut(AudioComponent source, float duration)
		{
			StartCoroutine(FadeOutRoutine(source, duration));
		}

		public AudioClip TryGetClip(string clipName)
		{
			foreach (var database in _audioDatabases)
			{
				var returnClip = TryFetch(clipName, database);
				if (returnClip != null)
				{
					return returnClip;
				}
			}
			return null;
		}

		public AudioClip TryFetch(string clipName, Dictionary<string, AudioClip> data)
		{
			AudioClip returnClip = null;
			data.TryGetValue(clipName, out returnClip);
			return returnClip;
		}
		#endregion
	}
}
