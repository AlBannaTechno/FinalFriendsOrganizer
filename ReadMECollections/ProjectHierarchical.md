### Here We will Explain <u>Structure Of The Project</u>

### Big Picture On The Project 
#### This project consist of :

#### FriendOrganizer.DataAccess
* Here We define a Database Context
  * Named as [<u>**Name**</u>DbContext.cs]
* Configure Database Configuration 
  * Connection Strings
  * All entity framework configurations
  * Also we must configure database in the main project `FriendOrganizer.UI`
* Contains `Migrations` directory


#### FriendOrganizer.Model
* ##### Define Models
* ##### Configure Validation of fields if we use attributes `Not fluent API` 

#### FriendOrganizer.UI `The Main Project : consists of `
* #### View
  * define all views With minimum code-behind
* #### ViewModel
  * Contains All ViewModels
* #### Startup
  * Contains `Bootstrapper.cs` startup class for Dependency Injection
* #### Wrapper : `Contains Wrappers For Every Model`
        
    * Class
        ```csharp
        public class NotifyDataErrorInfoBase : ViewModelBase, INotifyDataErrorInfo
        ```


* #### Data
* #### Services
* #### Events
