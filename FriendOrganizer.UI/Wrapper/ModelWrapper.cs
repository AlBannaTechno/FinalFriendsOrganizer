using System;
using System.Runtime.CompilerServices;
using System.Windows.Markup;

namespace FriendOrganizer.UI.Wrapper
{
    public class ModelWrapper<T>: NotifyDataErrorInfoBase
    {
        public ModelWrapper(T model)
        {
            Model = model;
        }
        public T Model { get; }

        protected virtual TValue GetValue<TValue>([CallerMemberName]string propertyName=null)
        {
            return (TValue)typeof(T).GetProperty(propertyName ?? throw new ArgumentNullException(nameof(propertyName)))?.GetValue(Model);
        }

        protected virtual void SetValue<TValue>(TValue value, [CallerMemberName]string propertyName = null)
        {
            typeof(T).GetProperty(propertyName ?? throw new ArgumentNullException(nameof(propertyName)))?.SetValue(Model,value);
            OnPropertyChanged(propertyName);
        }
    }
}
