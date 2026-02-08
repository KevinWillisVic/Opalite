namespace FishAndChips
{
    /// <summary>
    /// Interface for an asset that will hold game data.
    /// </summary>
    /// <typeparam name="TData">Data that implements IMetaDataStaticData.</typeparam>
    public interface IMetadataAsset<out TData> where TData : IMetaDataStaticData
    {
        TData Data { get; }

#if UNITY_EDITOR
        void OnImport();
#endif
    }
}
