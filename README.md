# Unity-SDK-v2
Refurbishing the Unity SDK as a UPM package

## How to Install

### Unity In-App Purchasing
- Open Unity project
- "Window/General/Services/In App Purchasing"
- Click the "Off" button to turn it on (Yes, that's stupid UX)
- "Install Latest Version"
- "Import"
- In the Project tab, go to "Plugins\UnityPurchasing", and double click the UnityIAP package there.
- An installer should pop out. Confirm everything.

### AppCentral SDK
- Open Package Manager (Menu > Window > Package Manager)
- Click the + button
- "Add Package from Git URL"
- Enter the Git URL from the AppCentral SDK repository: https://github.com/app-central/Unity-SDK-v2.git
- The package should now appear in your "In Project" packages and in your Project window under the Packages folder.

### In Game
- In Project tab, go to "AppCentral\Panels".
- Drag the desired panel prefab into a canvas.
- Use its ShowPanel() method to show it.
