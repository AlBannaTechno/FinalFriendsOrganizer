using System.Threading.Tasks;

namespace FriendOrganizer.UI.Services.Shared
{
    public interface IMessageDialogService
    {
        Task<MessageDialogResult> ShowOkCancelDialogAsync(string text, string title);

        Task ShowInfoDialogAsync(string info);
    }
}
