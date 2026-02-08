using UnityEngine;
using System;
using System.Collections.Generic;

namespace FishAndChips
{
    [Serializable]
    public class CraftItemData : GameObjectData, IMetaData, IMetaDataSavedData
    {
		#region -- Properties --
		public CraftItemModelData CraftItemModelData => ModelData as CraftItemModelData;
		public List<eCraftItemKeyword> Keywords => _keywords;
		#endregion

		#region -- Protected Member Vars --
		[SerializeField] protected List<eCraftItemKeyword> _keywords = new();
		#endregion

		#region -- Public Methods --
		public virtual IEntity CreateEntity(string instanceId)
		{
			return new CraftItemEntity(instanceId);
		}

		public virtual ISavedData CreateSavedData(string saveId)
		{
			return new CraftItemSavedData(saveId);
		}
#endregion
	}
}
