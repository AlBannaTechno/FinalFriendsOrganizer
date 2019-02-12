﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using FriendOrganizer.UI.ViewModel;

namespace FriendOrganizer.UI.Wrapper
{
    public class NotifyDataErrorInfoBase:ViewModelBase, INotifyDataErrorInfo
    {
        private readonly Dictionary<string, List<string>> _errorsBtPropertyName =
            new Dictionary<string, List<string>>();

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsBtPropertyName.ContainsKey(propertyName) ?
                _errorsBtPropertyName[propertyName] :
                null;
        }
        public bool HasErrors => _errorsBtPropertyName.Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;
        // https://stackoverflow.com/questions/1062102/practical-usage-of-virtual-functions-in-c-sharp
        protected virtual void OnErrorChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
        }

        protected void AddError(string propertyName, string error)
        {
            if (!_errorsBtPropertyName.ContainsKey(propertyName))
            {
                _errorsBtPropertyName[propertyName] = new List<string>();
            }

            if (!_errorsBtPropertyName[propertyName].Contains(error))
            {
                _errorsBtPropertyName[propertyName].Add(error);
                OnErrorChanged(propertyName);
            }
        }

        protected void ClearError(string propertyName)
        {
            if (_errorsBtPropertyName.ContainsKey(propertyName))
            {
                _errorsBtPropertyName.Remove(propertyName);
                OnErrorChanged(propertyName);
            }
        }
    }
}