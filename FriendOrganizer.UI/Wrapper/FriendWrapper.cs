using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using FriendOrganizer.Model;
using FriendOrganizer.UI.ViewModel;

namespace FriendOrganizer.UI.Wrapper
{
    public class FriendWrapper:ModelWrapper<Friend>
    {
        public FriendWrapper(Friend model) : base(model)
        {

        }

        public int Id => GetValue<int>();

        public string FirstName
        {
            get => GetValue<string>();
            set
            {
                SetValue<string>(value);
                ValidateProperty();
            }
        }

        public string LastName
        {
            get => GetValue<string>();
            set
            {
                SetValue<string>(value);
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => GetValue<string>();
            set
            {
                SetValue<string>(value);
                OnPropertyChanged();
            }
        }

        private void ValidateProperty([CallerMemberName]string propertyName = null)
        {
            ClearError(propertyName);
            switch (propertyName)
            {
                case nameof(FirstName):
                    if (FirstName.Equals("robot", StringComparison.OrdinalIgnoreCase))
                    {
                        AddError(propertyName, "Robots are not valid friend");
                    }
                    break;
            }
        }
    }
}
