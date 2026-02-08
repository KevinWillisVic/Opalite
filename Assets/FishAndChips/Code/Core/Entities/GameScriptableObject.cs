namespace FishAndChips
{
	/// <summary>
	/// ScriptableobjectData for a game related object.
	/// </summary>
    public class GameScriptableObject : ScriptableObjectData, IMetadataAsset<IMetaData>
	{
		#region -- Properties --
		public IMetaData Data { get; }
		#endregion

#if UNITY_EDITOR
		public virtual void OnImport()
		{
		}
#endif
	}
}
