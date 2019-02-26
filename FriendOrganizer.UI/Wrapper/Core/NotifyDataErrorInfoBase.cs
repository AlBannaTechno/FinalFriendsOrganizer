using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using FriendOrganizer.UI.ViewModel.Core;

namespace FriendOrganizer.UI.Wrapper.Core
{
    /**
     * The purpose of this class  to be a base class for all models to support error notification functionality
     * So here we need two things
     * 1- implement INotifyDataErrorInfo : this will make wpf trigger [Validation.HasError] property
     *      when ErrorsChanged?.Invoke occurred
     * 2- implement ViewModelBase {which implement INotifyPropertyChanged} to rais a notification
     *      when HasErrors property changed
     *          we will use this later to check if changes comes from error or another field
     */
    public class NotifyDataErrorInfoBase : ViewModelBase, INotifyDataErrorInfo
    {
        /*
         * Erorrs Dictionary 
         */
        private readonly Dictionary<string, List<string>> _errorsByPropertyName =
            new Dictionary<string, List<string>>();

        /**
         * Implemented from : INotifyDataErrorInfo interface
         */
        public IEnumerable GetErrors(string propertyName)
        {
            // Check if there is an errors in this property [propertyName] to return it or just return nul
            return _errorsByPropertyName.ContainsKey(propertyName) ?
                _errorsByPropertyName[propertyName] :
                null;
        }

        /*
         * Implemented from INotifyDataErrorInfo interface : to return true if there is any error
         *  : this based on how we implement Add/Clear Error
         */
        public bool HasErrors => _errorsByPropertyName.Any();

        /**
         * Implemented from INotifyDataErrorInfo interface : to support invoke ErrorsChanged event supplied with propertyName
         *  inside DataErrorsChangedEventArgs when error changed[Add-remove]
         * Wpf no matter how we implement this invoking {here we implement it inside OnErrorsChanged}
         *  it's just subscribed to this event
         * Also we can subscribe to it like any event
         *      as => instance.ErrorsChanged+=event_handler_name
         */
        public event EventHandler<DataErrorsChangedEventArgs> ErrorsChanged;

        // https://stackoverflow.com/questions/1062102/practical-usage-of-virtual-functions-in-c-sharp
        protected virtual void OnErrorsChanged(string propertyName)
        {
            ErrorsChanged?.Invoke(this, new DataErrorsChangedEventArgs(propertyName));
            // Call OnPropertyChanged from ViewModelBase : to TODO {???}
            base.OnPropertyChanged(nameof(HasErrors));
        }

        /**
         * Used to add error : by check if property exeist if just add this error if it not exist for this proerty
         *  if not {no error for this property} : create new list for erros then add this error to it
         * finally : call OnErrorsChanged() with the name of this property
         */
        protected void AddError(string propertyName, string error)
        {
            if (!_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName[propertyName] = new List<string>();
            }

            if (!_errorsByPropertyName[propertyName].Contains(error))
            {
                _errorsByPropertyName[propertyName].Add(error);
                OnErrorsChanged(propertyName);
            }
        }

        /**
         * Used : to clear all errors of specific property
         * finally : call OnErrorsChanged() with the name of this property
         */
        protected void ClearErrors(string propertyName)
        {
            if (_errorsByPropertyName.ContainsKey(propertyName))
            {
                _errorsByPropertyName.Remove(propertyName);
                OnErrorsChanged(propertyName);
            }
        }
    }
}
