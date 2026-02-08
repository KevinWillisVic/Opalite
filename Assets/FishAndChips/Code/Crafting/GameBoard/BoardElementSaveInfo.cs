using System;
using UnityEngine;

namespace FishAndChips
{
    [Serializable]
    public class BoardElementSaveInfo
    {
		#region -- Public Member Vars --
		public string ID;
		public Vector2 Position;
		[NonSerialized] public CraftItemInstance RuntimeInstance;
		#endregion

		#region -- Constructors --
		public BoardElementSaveInfo(string id, Vector2 position)
		{
			ID = id;
			Position = position;
		}

		public BoardElementSaveInfo(string id, CraftItemInstance instance)
		{
			ID = id;
			Position = instance.transform.localPosition;
			RuntimeInstance = instance;
		}
		#endregion

		#region -- Public Methods --
		public void RefreshPosition()
		{
			if (RuntimeInstance == null)
			{
				return;
			}
			Position = RuntimeInstance.transform.localPosition;
		}
		#endregion
	}
}
