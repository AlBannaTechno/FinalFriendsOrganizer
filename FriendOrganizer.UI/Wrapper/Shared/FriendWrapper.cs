﻿using System;
using System.Collections.Generic;
using FriendOrganizer.Model.Model;
using FriendOrganizer.UI.Wrapper.Core;

namespace FriendOrganizer.UI.Wrapper.Shared
{
    public class FriendWrapper : ModelWrapper<Friend>
    {
        public FriendWrapper(Friend model) : base(model)
        {
        }

        public int Id => GetValue<int>();

        public string FirstName { get => GetValue<string>(); set => SetValue<string>(value); }

        public string LastName { get => GetValue<string>(); set => SetValue<string>(value); }

        public string Email { get => GetValue<string>(); set => SetValue<string>(value); }

        public int? FavoriteLanguageId { get => GetValue<int?>(); set => SetValue<int?>(value); }

        protected override IEnumerable<string> ValidateProperty(string propertyName)
        {
            switch (propertyName)
            {
                case nameof(FirstName):
                    if (FirstName.Equals("robot", StringComparison.OrdinalIgnoreCase))
                    {
                        yield return "Robots are not valid friend";
                    }
                    break;
            }
        }
    }
}
