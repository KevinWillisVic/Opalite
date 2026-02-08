using System;

namespace FishAndChips
{
    public class NavigationRequest
    {
        [Flags]
        public enum eRequestStatus
        {
            Ok,
            Wait,
            Reject
        }

        public string RequestName;
        public Action<NavigationRequest> OnRequestComplete;
        public Action<NavigationRequest> OnRequestRejected;
        public INavigatable Navigatable;
    }
}
