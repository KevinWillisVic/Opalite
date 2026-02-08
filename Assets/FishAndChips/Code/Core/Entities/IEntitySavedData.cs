namespace FishAndChips
{
    /// <summary>
    /// Interface for a game entity that can be saved.
    /// </summary>
    public interface IEntitySavedData : IEntity
    {
        ISavedData SavedData { get; set; }
    }
}
