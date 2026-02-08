using UnityEngine.Playables;
using System;

namespace FishAndChips
{
    [Serializable]
    public class AnimationLookupKey
    {
        #region -- Inspector --
        public string Key;
        public PlayableDirector Director;
        #endregion
    }
}
