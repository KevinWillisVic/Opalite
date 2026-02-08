namespace FishAndChips
{
    /// <summary>
    /// Interface for game data.
    /// </summary>
    public interface IMetaData : IMetaDataStaticData
    {
        IEntity CreateEntity(string instanceId);
    }
}
