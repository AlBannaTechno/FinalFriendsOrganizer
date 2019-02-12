using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FriendOrganizer.Model;
using FriendOrganizer.UI.ViewModel;

namespace FriendOrganizer.UI.Wrapper
{
    public class FriendWrapper:ViewModelBase,INotifyDataErrorInfo
    {
        public FriendWrapper(Friend model)
        {
            Model = model;
        }
        public Friend Model { get; }

        public int Id => Model.Id;

        public string FirstName
        {
            get => Model.FirstName;
            set
            {
                Model.FirstName = value;
                OnPropertyChanged();
            }
        }

        public string LastName
        {
            get => Model.LastName;
            set
            {
                Model.LastName = value;
                OnPropertyChanged();
            }
        }

        public string Email
        {
            get => Model.Email;
            set
            {
                Model.Email = value;
                OnPropertyChanged();
            }
        }

        private readonly Dictionary<string,List<string>> _errorsBtPropertyName=
            new Dictionary<string, List<string>>();

        public IEnumerable GetErrors(string propertyName)
        {
            return _errorsBtPropertyName.ContainsKey(propertyName) ?
                _errorsBtPropertyName[propertyName] :
                null;
        }
        public bool HasErrors => _errorsBtPropertyName.Any();
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        private void OnErrorChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this,new DataErrorsChangedEventArgs(propertyName));
        }

        public void AddError(string propertyName, string error)
        {
            if (!_errorsBtPropertyName.ContainsKey(propertyName))
            {
                _errorsBtPropertyName[propertyName]=new List<string>();
            }

            if (!_errorsBtPropertyName[propertyName].Contains(error))
            {
                _errorsBtPropertyName[propertyName].Add(error);
                OnErrorChanged(propertyName);
            }
        }

        public void ClearError(string propertyName)
        {
            if (_errorsBtPropertyName.ContainsKey(propertyName))
            {
                _errorsBtPropertyName.Remove(propertyName);
                OnErrorChanged(propertyName);
            }
        }
    }
}
