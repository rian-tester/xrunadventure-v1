# AR + GPS + Unity

Specs :
Unity Version 2020.3.36f1 --> Upgrade to 2020.3.37f1  
Android and Ios target platform

## Unity Standard package : 
**AR Foundation**
**ARCore XR Plugin**
**Input System**
**Text Mesh Pro**
**Unity UI**

## Custom Assets :
**AR + GPS Location by Daniel Fortes**
**AR Foundation Remote by Kyrylo Kuzyk**
**DOTween by Demigiant**

Final MVP :
- App have GPS feature.
- The AR Object spawned by using wgs84 coordinate system sent from server.
- Camera will cast a Ray that can interact with Gameobject in AR environment.
- Standard authentication system
- Chat support features
- Wallet system
- Languange Localization

Check WIP last update on 16 July 2022, [here](https://drive.google.com/file/d/1JYOjn2o64FYqKFmASXCIW0u1OfPPzRAS/view?usp=sharing).  

Check latest builds on your Android Phone [here](https://drive.google.com/file/d/1JlNanbv5qfPfd79a8Hosuwfva8V5eEne/view?usp=sharing), make sure your Android OS at leaset Android 7 **(Nougat)**.  

Check full UI build and registration system demo [here](https://drive.google.com/file/d/1ZXkAO9EmS6AheJSAVD2j4kScYXn6vLd7/view?usp=sharing).  

## How to Clone the project properly

**1.** Make sure you have Unity 2020.3.36f1 (LTS) or above installed on your Unity Hub. All prior Unity version and 2021 might have some errors when opening the projects.  
**2.** Using manifest.json Unity will auto install all required package except costum package. Project can still work without [AR Foundation Remote 2.0](https://assetstore.unity.com/packages/tools/utilities/ar-foundation-remote-2-0-201106) asset. But make sure you have or bought [AR + GPS Location](https://assetstore.unity.com/packages/tools/integration/ar-gps-location-134882) asset, since this asset are the backbone of the project.   
**3** IF error happen after importing the project, check ARFoundation plugin, Material UI Elements plugin, CountryCodePicker Plugin. There is slight customization on  their code. Better to copy directly from master project for those plugins folders.  
**4** Audio files is not stored in this remote repo, so need to  add the files manually into folder.  
**5** There is free plugin that not coming from Unity Asset Store which UniClipboard [Find it here](https://github.com/sanukin39/UniClipboard).  
**6** Make a static class to store mapbox token and call it from RouteController.cs    


# Version 1.1

## GPS Feature
- Spawn Coin at defined location from AR camera/Player position (front,back,right,left) and all at 10m.
- Adding crosshair and raycast from center of screen with distance 10m to detect AR object
- Animation and color changing on crosshair
- Interacting with coin by screen touch, destroy coin, play sound and store coin value in UI and local variable
- UI sound and animation
- Displaying Lat and Lon player location at UI

## Login System
- UI for Main screen, login screen and register screen
- Carousel feature at main screen
- Radio button feature at register screen
- Web request when logging in and registering
- Scene management from starting app to enter the game flow
- Prompt panel for error and success message
- Sound design for login flow
> Last update 22 July 2022

## Connecting to Server
- PlaceAtLocations for primary spawn mechanic
- Testing, Send and Receive request from API
- Shaping the massive spawn code (MyPlaceAtLocation.cs)
- Calibrating spawn height
- Apply collider + physics to Coin Model

## Solidifying Core Gameplay
- Refactoring PlaceAtLocations script, cleaner code and add event Action for every spawn iteration  
- Refactoring Player, Coin and CoinManager relationship :  
    - Player only trigger blinking and taken in Coin
    - Coin will destroy itself
    - Player activate event action OnCoinTaken
    - CoinManager listen to the event and apply necessary sequences (update local variable, update UI, play sound)
    - Using CoinData class now every Coin Instance placed in GeoLocation has server value
- Lay foundation for next feature : Arrow Power Up (100 m)
> Last Update 26 July 2022

## Experimenting with Mapbox Route 3D
- Costum script "Route Controller" to handle GPS Direction API from Mapbox
- Adjusment from original sample script
- Adding feature for selecting 5 nearest coins for direction API
- Grab Coins and Pick Coin Button for Debugging
> Last Update 27 July 2022

## Connection Lost Safety System Added
- When device lost AR Tracking scene reload, restart AR Placement to zero, no coin available in 3D world.
- When device Location enabled, game sent new request to server with random amount coins and populate new coins.
- If scene reload without location enabled spawn will return with empty result.
- If location enabled after disabled, spawn will still run even with reload or without reload scene

### <a id="3D-Direction-V1"></a> Header
## 3D Direction Feature Applied (Development)
- If there any coins populated in 3D environment
- Once trigger the Route Controller will find GameObject with prefab name starting from the nearest
- Server populate coin by its distance, so that mean the first coin populated are the nearest from player
- By storing 10 prefab by its index name, Route Controller stored list of that gameobject compoenent (PlaceAtLocation)
- using its location property, Route Controller load route using ARLocation MapboxRoute methode and Mapbox API Token.
> Last Update 28 July 2022  

## UI Update
- Adding new coin model
- Game Scene UI applied
- All animation and sound for game scene UI
> Last update 31 July 2022

## Registration system and data storing 
- Connect to server to save new member data
- Checking existing user and logic
- Sending verification code to registrar email
- Verify the code with server
- Logging new member in 
> Last update 2 August 2022

## 100% Apps UI
- Home scene and UI added
- Shop scene and UI added (functionality under development)
- MyAds scene and UI added (functionality under development)
- Setting scene and UI added (functionality under development)
> Last update 5 August 2022

## MyXrun scene (wallet - cards)
- Making card model 
- Setup card fetcher by member number from server
- Apply carousel for cards fetched
- Populate card data into card model
- Adding copy to clipboard function for Android build
- Populate card logo and color based on card file number
> Last update 8 August 2022

## MyXrunScene (wallet - QR)
- Making QR Panel model
- Create QR Image based on card address string
- Adding copy to clipboard function for Android build in QR Panel
- Adding save to gallery function for Android build
- Adding show QR Panel from card model button
- Refactoring for CardFetcher.cs, Card.cs, CardManager.cs, MyQRCreator.cs, CardGenerator.cs, CardData.cs
    - CardFetcher dealing with server call member cards data and populate card model runtime
    - CardManager is holding active/selected card with relation to carousel toggler script
    - CardGenerator is located in card prefab and updating every card UI based on data receive from CardFetcher
    - CardData is costum class used for storing the JSON variable from server also used as public field by CardGenerator and CardManager
    - MyQRGenerator is handling QR Panel by receiving active card data from CardManager and use public function from CardFetcher(GetCardLogo();)   
> Last update 9 August 2022

## 3D Direction Feature Applied (Update)  
- Using Transform.GetChild now RouteController.cs will populate direction based on the first child of ARLocationRoot gameobject (coin prefabs)  
- Pressing next target will grab next index sibling.  
- When pressing previous target on index 0 will give error message.  
- Removing [previous logic](#3D-Direction-V1).  

## MyXrunScene (All transaction history)
- Adding fundamental for transaction history feature    
- Setup TransactionFetcher using cardfetcher member number and active card currency  
- Populate data on scroll rect with transaction prefab  
- Transaction Generator for populate transaction data text  
- Code fixed in my QR generator for bugs (RawImage issue)  
- Setup space for next development (transaction filter button)  
- TransactionFetcher.cs, TransactionGenerator.cs and TransactionData.cs relationship  
    - TransactionFetcher.cs fetch data from server  
    - Instantiate transaction prefab and make it child of content gameobject under ScrollRect gameobject  
    - TransactionFethcer passing data from server to each transaction game object TransactionGenerator component  
    - TransactionGenerator populate UI text element with the data  
    - Important : Destroy all child gameobject from content ScrollRect before populate data  
    - Important : Clear local list allTransactionData  
    - Important : Resize the height in scroll rect content 
    - Important : Adding 200 pixel if the list populate over index 4  

## Player local data system
- When completing login and register using PlayerPrefs store user email  
- In home screen request data to server using email in PlayerPrefs  
- Store that data in static class PlayerDataStatic.cs  
- Use static data anywhere in the game  
- This data should be reset when the game start or logout scene  
- As well the PlayerPrefs need to reset when logout   
> Last update 14 August 2022  

## MyXrunScene (wallet - QR Update)  
- Adding Native Android Share Sheet feature for QR  

## MyXrunScene (By days transaction history)
- UI button for 7,15 and 30 days transaction plus day parameter in button  
- Additional code in CardFetcher.cs for populating transaction by days
> Last Update 18 August 2022

## GPS Feature (Update)
- Making new 10 pre fixed world position (transform) based from camera location

## MyXrunScene (By types transaction history)
- UI button for all, withdraw, received and exchange transaction plus act code parameter in button  
- Additional code in CardFetcher.cs for populating transaction by type using selected button days

## Login/Register System
- Hide and unhide password field character 

## Solidifying Core Gameplay
- Setup Arrow + Hammer abilities in Player.cs  
- Arrow can see 100 m distance for silver coin and catch it with spherecast with 0.5f radius from sphere cast  
- Hammer will call 10 coin from child of ARLocationRoot transform and put them as last child index  
- Temporary mechanic for abilities timeout/disable by deactivating the button via Timer.cs  
- Spawning coin by User member number using PlayerStaticData.cs 
- When player catch coin, calling public method ValidatingCoin() in Coin.cs  
- Validating process and other sequence happening in Coin.cs instance  
> Last Update 21 August 2022  

## OTP Code to email added for Login and Register  
- User will receive 6 digits code in their inbox  
- For logging in with email feature  
- For register process  

## New MyAds /Invetory page  
- Will populate list of saved ads  

## Online Map Added  
- Static Tilemap added  
- Using Raw Image to display the map in UI  
- Default control replaced with Pinch.cs  

## Admobs prefab placed  
- In Main Game page and MyAds Page  
- In main game ads will play if player decide to watch after catching coin  

## QR reader is added  
- Located in send page  

## All pages from original pages are added  
- Setting page  
- Account page  
- T&C page  
- Device setting page  
> Last Update 2 November 2022  