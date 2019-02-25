namespace FriendOrganizer.UI.Services
{
    public interface IMessageDialogService
    {
        MessageDialogResult ShowOkCancelDialog(string text, string title);

        void ShowInfoDialog(string info);
    }
}
