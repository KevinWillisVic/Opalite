namespace FishAndChips
{
    public interface INavigatable
    {
        NavigationRequest.eRequestStatus SystemRequestingNavigation(string destination);
        bool DoesConsumeBackRequest();
        bool AddToHistory();
        bool IsRoot();
    }
}
