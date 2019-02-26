using FriendOrganizer.Model.Model;
using FriendOrganizer.UI.Wrapper.Core;

namespace FriendOrganizer.UI.Wrapper.Shared
{
    public class FriendPhoneNumberWrapper : ModelWrapper<FriendPhoneNumber>
    {
        public FriendPhoneNumberWrapper(FriendPhoneNumber model) : base(model)
        {
        }

        public string Number { get => GetValue<string>(); set => SetValue(value); }
    }
}
