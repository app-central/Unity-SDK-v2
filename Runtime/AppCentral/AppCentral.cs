
namespace AppCentral
{
    using UnityEngine;
    using UnityEngine.Networking;

    public class AppCentral
    {
        public static Purchaser purchaser;

        // TODO: Shouldn't need behaviour because purchaser shouldn't be a behaviour either
        // TODO: What exactly is productID?
        public static void Setup(MonoBehaviour behaviour, string productID)
        {
            Purchaser.kProductNameAppleSubscription = productID;
            Purchaser.kProductIDSubscription = productID;
            Debug.Log("Init product " + productID);
            // TODO: Shouldn't this be a const field or, better, in a configuration file?
            UnityWebRequest.Get("https://vnc412s287.execute-api.us-east-1.amazonaws.com/default/unity-tracker?v=1&action=start&appid="
                                + Application.identifier).SendWebRequest();

            AppCentral.purchaser = behaviour.gameObject.AddComponent<Purchaser>();
            AppCentral.purchaser.InitializePurchasing(); // TODO: Either Purchaser should self initialise or not.
        }

        public static bool IsUserSubscribed()
        {
            // TODO: This kind of thing shouldn't be saved in player prefs
            return 1 == PlayerPrefs.GetInt("purchased_subscription");
        }

        // TODO: Is this the main entry point? It's called by Purchaser child too.
        public static void ShowPaywall()
        {
            if (AppCentral.IsUserSubscribed())
            {
                Debug.Log("User already subscribed, not showing paywall");
                return;
            }

            Debug.Log("Showing paywall");
            // TODO: Loop calling if called by Purchaser
            // TODO: Also, should be a method
            PayWallGraphics.Instance.windowOpen = true;
        }
    }
}