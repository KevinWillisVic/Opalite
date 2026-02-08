namespace FishAndChips
{
    public class BaseAudioTypes
    {
		#region -- Enumerations --
		public enum eMusicType
        {
            None,
            Gameplay
        }

        public enum eSFXType
        {
            None,
            ButtonPositive,
            ButtonNegative,
        }

        public enum eAudioSourceType
        {
            None,
            SFX,
            Music,
            UI
        }
        #endregion
    }
}
