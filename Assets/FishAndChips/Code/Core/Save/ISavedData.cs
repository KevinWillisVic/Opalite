namespace FishAndChips
{
    /// <summary>
    /// Interface for objects that will be saved.
    /// </summary>
    public interface ISavedData
    {
        // Time the save file was created.
        long TimeCreated { get; }
        // Time the save file was updated.
        long TimeUpdated { get; }

        void Load();
        bool Deserialize();
        void Save();
        void Delete();
    }
}
