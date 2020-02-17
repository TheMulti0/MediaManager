``Note: This project consists of a rich API, but a very simple implementation. The code is not documented.``

# MediaManager
---
### Overview

##### *“The social media user manager built for mass use”*

MediaManager is a multi-layer project that automates actions on a large group of social media accounts.

The project targets .NET Core 3.1 and .NET Standard 2.1

The project consists of 3 layers:

 - **[MediaManager.Api](https://github.com/TheMulti0/MediaManager/tree/master/MediaManager.Api)** - *“The Providers API”* - Defines the contract for interacting with specific social media providers. (Does not include an implementation)

 - **[MediaManager](https://github.com/TheMulti0/MediaManager/tree/master/MediaManager)** - Uses the providers API to execute actions on the social media users.

 - **[MediaManager.Web](https://github.com/TheMulti0/MediaManager/tree/master/MediaManager.Web)** - ASP.NET Core, MVC web application with Razor views. Provides a user-interface for MediaManager, handles authentication with social media providers.
---
### Current Implementation

The project currently implements full support for Twitter (using OAuth2 user-tokens).

The MediaManager implementation currently allows the following operations;
 - Operate a custom operation on all given social media users.
 - Check given social media users for new posts, like the new ones, and validate that the post has not been operated on (and support for doing so periodically).

### Website demonstration

![alt text](https://i.imgur.com/2oYCzPF.png)

![alt text](https://i.imgur.com/eL7b9ve.png)



The following page will only appear for users configured with the admin role:

![alt text](https://i.imgur.com/2ZKDrGf.png)