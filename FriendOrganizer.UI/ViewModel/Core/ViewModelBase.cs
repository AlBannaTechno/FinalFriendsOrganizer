using System.ComponentModel;
using System.Runtime.CompilerServices;
using FriendOrganizer.UI.Annotations;

namespace FriendOrganizer.UI.ViewModel.Core
{
    /**
     * Base Class For All ViewModels class
     * The purpose of this class : to implement [INotifyPropertyChanged] interface
     * To notify the models when changing occurred
     */
    public class ViewModelBase : INotifyPropertyChanged
    {
        /*
         * Implemented from INotifyPropertyChanged : to invoke when property changed
         *  and supply data : as {PropertyChangedEventArgs with propertyName} for it's subscribers
         *  which can subscribe to this event as => instance.PropertyChanged+=event_handler_name
         */
        public event PropertyChangedEventHandler PropertyChanged;

        // next attribute from resharper {not related to our project}
        [NotifyPropertyChangedInvocator]
        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
