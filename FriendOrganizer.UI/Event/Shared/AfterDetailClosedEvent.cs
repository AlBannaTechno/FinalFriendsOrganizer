using Prism.Events;

namespace FriendOrganizer.UI.Event.Shared
{
    public class AfterDetailClosedEvent:PubSubEvent<AfterDetailClosedEventArgs>
    {
    }

    public class AfterDetailClosedEventArgs
    {
        public int Id { get; set; }
        public string ViewModelName { get; set; }
    }
}
