using UnityEngine;
using System;

namespace FishAndChips
{
    [Serializable]
    public class GameObjectData
	{
		#region -- Properties --
		public virtual string ID => _id;
		public virtual string ModelId => _modelId;
		public virtual GameObjectModelData ModelData
		{
			get => _modelData;
			set => _modelData = value;
		}
		#endregion

		#region -- Protected Member Vars --
		[SerializeField] protected string _id;
		[SerializeField] protected string _modelId;
		[SerializeField] protected GameObjectModelData _modelData;
		#endregion

		#region -- Public Methods --
		public void SetId(string id)
		{
			_id = id;
		}

		public void SetModelId(string modelId)
		{
			_modelId = modelId;
		}

		public virtual void OnImport()
		{
		}
		#endregion
	}
}
