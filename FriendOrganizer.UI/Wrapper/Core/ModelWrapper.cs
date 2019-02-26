using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Runtime.CompilerServices;

namespace FriendOrganizer.UI.Wrapper.Core
{
    /**
     * This class created to be a general wrapper for all models
     *  Because we decide to track-changes and validate errors out of model section [MVVM]
     * So this class Should achive Next :
     *  1- Has a version of the real model [entity container]
     *  2- Work as a proxy to set and get valus from the real model version by using reflection to :
     *      1- Prevent direct manipulation of Model propreites
     *      2- Do some automatic stuffs when we set or get a value like
     *          1- DataAnnotations
     *          2- Custom validation : from subClass
     *  3- this class should allow all it's drived classess [subClasses] to
     *      1- get/set values of the model
     *      2- add a custom rules to validation logic
     *
     * --------------------------------------------------------------
     *
     * This class inherited from {NotifyDataErrorInfoBase} class for two reasons
     *  1- use : OnPropertyChanged when any property change because of {NotifyDataErrorInfoBase}
     *      => inherited from {ViewModelBase} which implement {INotifyPropertyChanged} and has [OnPropertyChanged()]
     *  2- use : [AddError()/ClearErrors()] when set model values : and those properites comes from
     *      {NotifyDataErrorInfoBase} which implement {INotifyDataErrorInfo}
     */
    public abstract class ModelWrapper<T> : NotifyDataErrorInfoBase
    {
        /**
         * The Main Constructor with Generic T as an argument represent the model
         */
        protected ModelWrapper(T model)
        {
            Model = model;
        }

        /**
         * Model : we prevent set because we must prevent any outside manipulation
         *  Because of this class will not use on it's own and we will not create an instance of it [abstract]
         *  it will always inherited so we will pass the modl to it <model> and we will add
         *  a validation logic specific to this model => by ovveride : ValidateProperty()
         *  so we can't allow replacing this model
         */
        public T Model { get; }

        /**
         * Get Value from the model by propertyName with reflection
         * [CallerMemberName] : attribute allow compiler to pass caller member {eg. property} name when we call this function
         *      without passing propertyName parameter
         */
        protected virtual TValue GetValue<TValue>([CallerMemberName]string propertyName = null)
        {
            return (TValue)typeof(T).GetProperty(propertyName ?? throw new ArgumentNullException(nameof(propertyName)))?.GetValue(Model);
        }

        /**
         * Set a Value to the model by propertyName with reflection
         * Because we here set a new value we need to          *  1- call OnPropertyChanged() to event all subscribbers
         *  2- Validate this property after the chaging
         */
        protected virtual void SetValue<TValue>(TValue value, [CallerMemberName]string propertyName = null)
        {
            typeof(T).GetProperty(propertyName ?? throw new ArgumentNullException(nameof(propertyName)))?.SetValue(Model, value);
            OnPropertyChanged(propertyName);
            ValidatePropertyInternal(propertyName, value);
        }

        /**
         * Created : to do a validation on the new value seted to custom property on Multi steps
         *  1- Clear all old errors for this property
         *  2- Validate Data Annotations : we should add those annotation as attribute to {model fields class} [MVVM]
         *  3- Validate Custom Errors defined in the subClass
         *  4- TODO : Support validation when we set the restriction from FluentApi also
         */
        private void ValidatePropertyInternal(string propertyName, object currentValue)
        {
            // Clear previous errors for this property
            ClearErrors(propertyName);

            // Validate Data Annotations Errors
            ValidateDataAnnotations(propertyName, currentValue);

            // Validate Custom error : proxy method
            ValidateCustomErrors(propertyName);
        }

        private void ValidateDataAnnotations(string propertyName, object currentValue)
        {
            var context = new ValidationContext(Model) { MemberName = propertyName };
            var results = new List<ValidationResult>();
            Validator.TryValidateProperty(currentValue, context, results);

            foreach (var result in results)
            {
                AddError(propertyName, result.ErrorMessage);
            }
        }

        /**
         * this method defined as proxy between this class and the subclass
         * so we pass the property name to ValidateProperty() which is the real method for custom validations
         */
        private void ValidateCustomErrors(string propertyName)
        {
            var errors = ValidateProperty(propertyName);
            if (errors != null)
            {
                foreach (var error in errors)
                {
                    AddError(propertyName, error);
                }
            }
        }

        /**
         * This method used to add custom validation logic from the subClass
         * we will not make this method abstract because of : for any reason
         * we may decide to not add any custom validation logicc
         * but if we decide to do that we can just overriding it in the subClass
         *
         * Note : no need here to pass the value to this method because this method
         *  will written in the subClass : and we do a vlidation after we save the value
         *  so overriding of this method in subClass will depend on the real property getter
         *  to do the validation
         * For Example :
         *
         * protected override IEnumerable<string> ValidateProperty(string propertyName)
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
         *
         * Notic we use yield because we use IEnumerable and this will work perfectly with asyn to
         * make a performance better because IEnumerable don't fetch all data at once but one by one
         * Also Note : when IEnumerable don't yield any thing this means it's will return null and this is OK
         */
        protected virtual IEnumerable<string> ValidateProperty(string propertyName)
        {
            // This method should ovverided in the subClass : so we put it protected virtual
            return null;
        }
    }
}
