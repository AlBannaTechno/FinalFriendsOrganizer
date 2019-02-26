using FriendOrganizer.Model.Model;
using FriendOrganizer.UI.Wrapper.Core;

namespace FriendOrganizer.UI.Wrapper.Shared
{
    public class ProgrammingLanguageWrapper:ModelWrapper<ProgrammingLanguage>
    {
        public ProgrammingLanguageWrapper(ProgrammingLanguage model) : base(model)
        {
        }

        public int Id => Model.Id;

        public string Name
        {
            get => GetValue<string>();
            set => SetValue(value);
        }
    }
}
