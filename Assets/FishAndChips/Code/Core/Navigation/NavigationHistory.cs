using System.Collections.Generic;

namespace FishAndChips
{
    public class NavigationHistory
    {
		#region -- Public Member Vars --
		public List<string> RootViewNames = new List<string>();
		public List<string> RequestHistory = new List<string>();
		#endregion

		#region -- Public Methods --
		public void Clear()
		{
			RequestHistory.Clear();
		}

		public void AddRequest(string request)
		{
			if (RootViewNames.Contains(request) == true)
			{
				RequestHistory.Clear();
			}

			int previousIndex = RequestHistory.IndexOf(request);
			if (previousIndex != -1)
			{
				RequestHistory.RemoveAt(previousIndex);
			}
			RequestHistory.Add(request);
		}

		public string Pop()
		{
			int currentRequestCount = RequestHistory.Count;
			if (currentRequestCount == 0)
			{
				return string.Empty;
			}

			string view = RequestHistory[currentRequestCount - 1];
			RequestHistory.RemoveAt(currentRequestCount - 1);
			return view;
		}

		public string Peek()
		{
			int currentRequestCount = RequestHistory.Count;
			if (currentRequestCount == 0)
			{
				return string.Empty;
			}
			return RequestHistory[currentRequestCount - 1];
		}
		#endregion
	}
}
