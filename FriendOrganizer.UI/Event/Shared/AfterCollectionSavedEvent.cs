using Prism.Events;

namespace FriendOrganizer.UI.Event.Shared
{
    public class AfterCollectionSavedEvent:PubSubEvent<AfterCollectionSavedEventArgs>
    {
    }

    public class AfterCollectionSavedEventArgs
    {
        public string ViewModelName { get; set; }
    }
}
