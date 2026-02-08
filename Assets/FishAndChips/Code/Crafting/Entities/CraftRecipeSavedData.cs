using System;

namespace FishAndChips
{
    /// <summary>
    /// Save data for CraftRecipe.
    /// </summary>
    [Serializable]
    public class CraftRecipeSavedData : UnlockableEntitySavedData
    {
        #region -- Constructors --
        public CraftRecipeSavedData(string saveId) : base(saveId)
        {
            // Implement any custom save variables / logic here.
        }
        #endregion
    }
}
