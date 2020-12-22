
namespace AppCentral
{
    #region using
    using System;
    using UnityEngine;
    using Object = UnityEngine.Object;
    using UnityEngine.UI;
    using TMPro;
    #endregion

    /// <summary>This is the graphical interface, the view, of the shop.
    /// It should pull a developer uploaded assets to display a custom made shop interface for the user.</summary>
    public class SubscriptionWindow : MonoBehaviour
    {
        #region inspector fields
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Image foregroundImage;
        [SerializeField] private TextMeshProUGUI topTitleTMP;
        [SerializeField] private TextMeshProUGUI topSubtitleTMP;
        [SerializeField] private TextMeshProUGUI topLeftLinkTMP;
        [SerializeField] private TextMeshProUGUI topRightLinkTMP;
        [SerializeField] private TextMeshProUGUI bottomTitleTMP;
        [SerializeField] private TextMeshProUGUI bottomSubtitleTMP;
        [SerializeField] private Image subscriptionButtonImage;
        [SerializeField] private TextMeshProUGUI subscriptionButtonTMP;
        #endregion // inspector fields
        // It must be unique, always available and always accessible. It's the interface's presence in the game.
        public static SubscriptionWindow Instance { get; private set; }
        public static bool WindowOpen { get; private set; }

        private void OpenAppCentralTerms()
        {
            // TODO: Should be a const field or from a configuration file
            Application.OpenURL("https://www.app-central.com/terms");
        }

        public void RestorePurchases()
        {
            AppCentralStoreListener.RestorePurchases();
        }

        public void BuySubscription()
        {
            AppCentralStoreListener.BuyProduct(AppCentralStoreListener.ProductType.Subscription);
        }

        public static void ShowPanel()
        {
            if (AppCentralStoreListener.IsUserSubscribed())
            {
                Debug.Log("User already subscribed, not showing paywall");
                return;
            }

            // Instance.bottomTitleTMP.text = AppCentralStoreListener.LocalizedTitle;
            // Instance.bottomSubtitleTMP.text = AppCentralStoreListener.LocalizedDescription + " just " +
                                              // AppCentralStoreListener.LocalizedPriceString + "/month";
            // Instance.topLeftLinkTMP.text = "Terms";
            // Instance.topRightLinkTMP.text = "Restore";
            // Instance.subscriptionButtonTMP.text = "Subscribe";

            SubscriptionWindow.WindowOpen = true;
        }

        /// <summary>TODO: Hide this panel in some way.</summary>
        private static void HidePanel()
        {
            throw new NotImplementedException();
        }

        private void Awake()
        {
            if (SubscriptionWindow.Instance != null && SubscriptionWindow.Instance != this)
            {
                Object.Destroy(this.gameObject);
                return;
            }

            SubscriptionWindow.Instance = this;
        }
    }
}