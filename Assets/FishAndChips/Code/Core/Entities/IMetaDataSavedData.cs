namespace FishAndChips
{
    /// <summary>
    /// Interface for game data that can be saved.
    /// </summary>
    public interface IMetaDataSavedData 
    {
        ISavedData CreateSavedData(string saveId);
    }
}
