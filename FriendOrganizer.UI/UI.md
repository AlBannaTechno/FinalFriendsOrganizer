#### FriendOrganizer.UI

##### here we will define the core of the project As

* Data : 
  * `Here we define`
    * Repositories : `To contains all logic for fetching data`
    * Lookups : `To fetch just specific elements with id from db`
      * To support navigation purpose
      * Lookups is also a repository
  * All repositories fetch data vai `DataAccess` DbContext
* Event :
  * `Here we define aall events With it's specific eventArgs`
  * All events inherited from PubSubEvent`<Args>` from `Prism Library`
* Resources
  * Has Application Resources/Dictionaries
* Services
* Startup
  * Services is a shared logic like
    * Message dialgo
    * Logging System
* View
  * Here all views
* ViewModel
  * All backend view logic
* Wrapper
  * Wrapper used essentially to track model changes
   So every model should have it's wrapper
  * there course on model treatment we can look at it to gain what's wrapper exactly is