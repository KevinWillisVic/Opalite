using UnityEngine;
using System;
using System.Collections.Generic;

namespace FishAndChips
{
    public class AudioDatabase : MonoBehaviour
    {
		#region -- Supporting --
		[Serializable]
		public class SFXMapping
		{
			public BaseAudioTypes.eSFXType Key;
			public AudioClip Clip;
		}

		[Serializable]
		public class MusicMapping
		{
			public BaseAudioTypes.eMusicType Key;
			public AudioClip Clip;
		}
		#endregion

		#region -- Inspector --
		public List<MusicMapping> MusicClips = new();
		public List<SFXMapping> SFXClips = new();
		#endregion
	}
}
