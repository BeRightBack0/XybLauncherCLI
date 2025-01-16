Assets:

-DlssHandler.cs - Downloads default redirect
-AccountHandler.cs - Account manager
-ClientManager.cs - Start Client function (add more flexibility to it)
-FilesManager.cs - Handles build download process
-Functions.cs - Functions for errors, not used right now  16.01.25
-ServerHandler.cs - Old Start Server function not used and to be redone later

Other:
-AccountParser.cs -  Passes account info like email and password
-AsyncStreamReader.cs - Takes output from the executable to our output 
-PathParser.cs - Passes a current selected version path for the ClientManager.cs (later for ServerManager.cs)
-SigScan.cs - Used by Patcher.cs for patching the executable
Win32.cs - Provides integration with the Windows API for process and memory manipulation


Root Folder: 
-Injector.cs - Utilized by ClientManager.cs to inject the redirect into the Fortnite executable
-Patcher.cs - Called by ClientManager.cs to patch the Fortnite executable
-Program.cs - Entry Point for the app
-VersionHandler.cs - VersionManager.cs
