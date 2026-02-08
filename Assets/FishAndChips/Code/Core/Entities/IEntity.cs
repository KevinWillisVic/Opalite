namespace FishAndChips
{
    /// <summary>
    /// Interface for a game entity.
    /// </summary>
    public interface IEntity
    {
        string InstanceId { get; }
        string StaticId { get; }
        void Initialize();
        void Cleanup();
        IMetaData Data { get; set; }
    }
}
