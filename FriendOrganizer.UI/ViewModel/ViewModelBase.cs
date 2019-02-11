using System.ComponentModel;
using System.Runtime.CompilerServices;
using FriendOrganizer.UI.Annotations;

namespace FriendOrganizer.UI.ViewModel
{
    class ViewModelBase:INotifyPropertyChanged
    {
        #region INotifyPropertyChanged : implementation Generated With Resharper

        public event PropertyChangedEventHandler PropertyChanged;

        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
