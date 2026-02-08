using System;
using System.Linq;
using System.Collections.Generic;

namespace FishAndChips
{
    [Serializable]
    public class BoardSaveInfo : SavedData
    {
        #region -- Public Member Vars --
        public List<BoardElementSaveInfo> SavedElements = new();
        #endregion

        #region -- Constructors --
        public BoardSaveInfo(string saveId) : base(saveId)
        {
        }
        #endregion

        #region -- Public Methods --
        public BoardElementSaveInfo GetBoardElementInfo(CraftItemInstance instance)
        {
            return SavedElements.FirstOrDefault(e => e.RuntimeInstance == instance);
        }

        public void TrackElement(CraftItemInstance instance)
        {
            var trackedElement = GetBoardElementInfo(instance);
            if (trackedElement == null)
            {
                trackedElement = new BoardElementSaveInfo(instance.CraftItemData.ID, instance);
                SavedElements.Add(trackedElement);
            }
            Save();
        }

        public void UntrackElement(CraftItemInstance instance)
        {
            var trackedElement = GetBoardElementInfo(instance);
            if (trackedElement != null)
            {
                SavedElements.Remove(trackedElement);
            }
            Save();
        }

		public override void Save()
		{
            int length = SavedElements.Count - 1;
            for (int i = length; i >= 0; i--)
            {
                // Clean up any elements before save.
                var savedElement = SavedElements[i];
                if (savedElement == null || savedElement.RuntimeInstance == null)
                {
                    SavedElements.RemoveAt(i);
                    continue;
                }
                savedElement.RefreshPosition();
            }
			base.Save();
		}

		public override void Reset()
		{
			base.Reset();
            SavedElements.Clear();
            Save();
		}
		#endregion
	}
}
