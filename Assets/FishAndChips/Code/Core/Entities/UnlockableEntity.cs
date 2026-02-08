namespace FishAndChips
{
	/// <summary>
	/// Base class for entity that can be unlocked.
	/// </summary>
    public abstract class UnlockableEntity : IEntitySavedData
    {
		#region -- Properties --
		public IMetaData Data { get; set; }
		public ISavedData SavedData { get; set; }

		public UnlockableEntitySavedData UnlockableEntitySavedData
		{
			get
			{
				if (_unlockableEntitySavedData == null)
				{
					_unlockableEntitySavedData = SavedData as UnlockableEntitySavedData;
				}
				return _unlockableEntitySavedData;
			}
		}

		/// <summary>
		/// Id passed in when constructing entity.
		/// </summary>
		public string InstanceId { get; }

		/// <summary>
		/// Id set from data.
		/// </summary>
		public virtual string StaticId => (Data != null) ? Data.ID : string.Empty;

		/// <summary>
		/// Unlock state of the entity.
		/// </summary>
		public bool Unlocked => UnlockableEntitySavedData.Unlocked;
		/// <summary>
		/// Hint state of the entity.
		/// </summary>
		public bool HintGiven => UnlockableEntitySavedData.HintGiven;
		#endregion

		#region -- Private Member Vars --
		private UnlockableEntitySavedData _unlockableEntitySavedData = null;
		#endregion

		#region -- Constructors --
		public UnlockableEntity(string instanceId)
		{
			InstanceId = instanceId;
			_unlockableEntitySavedData = SavedData as UnlockableEntitySavedData;
		}
		#endregion

		#region -- Public Methods --
		/// <summary>
		/// Initialize entity.
		/// </summary>
		public virtual void Initialize()
		{
		}

		/// <summary>
		/// Cleanup entity.
		/// </summary>
		public virtual void Cleanup()
		{
		}

		/// <summary>
		/// Handle save state on game reset / object reset.
		/// </summary>
		public virtual void Reset()
		{
		}

		/// <summary>
		/// Set unlocked state of the entity.
		/// </summary>
		/// <param name="state">Unlock state of the entity.</param>
		public virtual void SetUnlockState(bool state)
		{
			UnlockableEntitySavedData.SetUnlockedState(state);
		}

		/// <summary>
		/// Set hint state of the entity.
		/// </summary>
		/// <param name="state">Hint state of entity.</param>
		public virtual void SetHintState(bool state)
		{
			UnlockableEntitySavedData.SetHintGivenState(state);
		}
		#endregion
	}
}
