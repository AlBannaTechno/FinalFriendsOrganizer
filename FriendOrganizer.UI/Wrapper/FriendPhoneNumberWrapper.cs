using FriendOrganizer.Model;
using FriendOrganizer.Model.Model;

namespace FriendOrganizer.UI.Wrapper
{
    public class FriendPhoneNumberWrapper : ModelWrapper<FriendPhoneNumber>
    {
        public FriendPhoneNumberWrapper(FriendPhoneNumber model) : base(model)
        {
        }

        public string Number { get => GetValue<string>(); set => SetValue(value); }
    }
}
