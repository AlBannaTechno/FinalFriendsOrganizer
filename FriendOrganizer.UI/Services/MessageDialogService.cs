﻿using System.Threading.Tasks;
using System.Windows;
using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;

namespace FriendOrganizer.UI.Services
{
    public class MessageDialogService : IMessageDialogService
    {

        private MetroWindow MetroWindow => (MetroWindow)Application.Current.MainWindow;
        public async Task<MessageDialogResult>  ShowOkCancelDialogAsync(string text, string title)
        {

            var result=await MetroWindow.ShowMessageAsync(title,text,MessageDialogStyle.AffirmativeAndNegative);

            return result == MahApps.Metro.Controls.Dialogs.MessageDialogResult.Affirmative?
                MessageDialogResult.Ok :
                MessageDialogResult.Cancel;
        }

        public async Task ShowInfoDialogAsync(string info)
        {
            await MetroWindow.ShowMessageAsync("Info",info);
        }
    }

    public enum MessageDialogResult
    {
        Ok,
        Cancel
    }
}
