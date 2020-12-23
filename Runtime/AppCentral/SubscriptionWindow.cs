
namespace AppCentral
{
    #region using
    using System;
    using System.Linq;
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
        [ContextMenuItem("Run Configuration", "ReadConfiguration")]
        [SerializeField] private SubscriptionWindowConfiguration subscriptionConfiguration;
        #endregion // inspector fields

        // It must be unique, always available and always accessible. It's the interface's presence in the game.
        public static SubscriptionWindow Instance { get; private set; }
        public static bool WindowOpen { get; private set; }

        public void OpenAppCentralTerms()
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
            SubscriptionWindow.WindowOpen = false;

            throw new NotImplementedException();
        }

        private void CheckAssignments()
        {
            if (this.backgroundImage == null)
            { this.backgroundImage = this.GetComponentsInChildren<Image>()
                                         .First(image => image.gameObject.name.ToLower().Contains("background")); }
            if (this.foregroundImage == null)
            { this.foregroundImage = this.GetComponentsInChildren<Image>()
                                         .First(image => image.gameObject.name.ToLower().Contains("foreground")); }
            if (this.topTitleTMP == null)
            { this.topTitleTMP = this.GetComponentsInChildren<TextMeshProUGUI>()
                                     .First(tmp => tmp.gameObject.name.ToLower().Contains("top")
                                                                    && tmp.gameObject.name.ToLower().Contains("title")); }
            if (this.topSubtitleTMP == null)
            { this.topSubtitleTMP = this.GetComponentsInChildren<TextMeshProUGUI>()
                                        .First(tmp => tmp.gameObject.name.ToLower().Contains("top")
                                                                     && tmp.gameObject.name.ToLower().Contains("subtitle")); }
            if (this.topLeftLinkTMP == null)
            { this.topLeftLinkTMP = this.GetComponentsInChildren<TextMeshProUGUI>()
                                        .First(tmp => tmp.gameObject.name.ToLower().Contains("top")
                                                                        && tmp.gameObject.name.ToLower().Contains("left")
                                                                        && tmp.gameObject.name.ToLower().Contains("button")); }
            if (this.topRightLinkTMP == null)
            { this.topRightLinkTMP = this.GetComponentsInChildren<TextMeshProUGUI>()
                                         .First(tmp => tmp.gameObject.name.ToLower().Contains("top")
                                                                        && tmp.gameObject.name.ToLower().Contains("right")
                                                                        && tmp.gameObject.name.ToLower().Contains("button")); }
            if (this.bottomTitleTMP == null)
            { this.bottomTitleTMP = this.GetComponentsInChildren<TextMeshProUGUI>()
                                          .First(tmp => tmp.gameObject.name.ToLower().Contains("bottom")
                                                                        && tmp.gameObject.name.ToLower().Contains("title")); }
            if (this.bottomSubtitleTMP == null)
            { this.bottomSubtitleTMP = this.GetComponentsInChildren<TextMeshProUGUI>()
                                           .First(tmp => tmp.gameObject.name.ToLower().Contains("bottom")
                                                                        && tmp.gameObject.name.ToLower().Contains("subtitle")); }
            if (this.subscriptionButtonImage == null)
            { this.subscriptionButtonImage = this.GetComponentsInChildren<Image>()
                                                 .First(image => image.gameObject.name.ToLower().Contains("subscription")
                                                                    && image.gameObject.name.ToLower().Contains("button")); }
            if (this.subscriptionButtonTMP == null)
            { this.subscriptionButtonTMP = this.GetComponentsInChildren<TextMeshProUGUI>()
                                               .First(tmp => tmp.gameObject.name.ToLower().Contains("subscribe")
                                                                            && tmp.gameObject.name.ToLower().Contains("tmp")); }
        }

        private void ReadConfiguration()
        {
            this.backgroundImage.sprite = this.subscriptionConfiguration.backgroundImage;
            this.foregroundImage.sprite = this.subscriptionConfiguration.foregroundImage;
            Color foregroundImageColor = this.foregroundImage.color;
            foregroundImageColor.a = this.subscriptionConfiguration.foregroundOpacity;
            this.foregroundImage.color = foregroundImageColor;
            this.subscriptionButtonImage.sprite = this.subscriptionConfiguration.subscriptionButtonImage;

            this.topTitleTMP.alignment = this.topSubtitleTMP.alignment = this.subscriptionButtonTMP.alignment
                = this.bottomTitleTMP.alignment = this.bottomSubtitleTMP.alignment = TextAlignmentOptions.Center;
            this.topLeftLinkTMP.alignment = TextAlignmentOptions.MidlineRight;
            this.topRightLinkTMP.alignment = TextAlignmentOptions.MidlineLeft;
            this.topTitleTMP.text = this.subscriptionConfiguration.topTitleText;
            this.topSubtitleTMP.text = this.subscriptionConfiguration.topSubtitleText;
            this.topLeftLinkTMP.text = this.subscriptionConfiguration.topLeftLinkText;
            this.topRightLinkTMP.text = this.subscriptionConfiguration.topRightLinkText;
            this.bottomTitleTMP.text = this.subscriptionConfiguration.bottomTitleText;
            this.bottomSubtitleTMP.text = this.subscriptionConfiguration.bottomSubtitleText;
            this.subscriptionButtonTMP.text = this.subscriptionConfiguration.subscriptionButtonText;
        }

        private void OnValidate()
        {
            this.CheckAssignments();
        }

        private void Awake()
        {
            if (SubscriptionWindow.Instance != null && SubscriptionWindow.Instance != this)
            {
                Object.DestroyImmediate(this.gameObject);
                return;
            }

            SubscriptionWindow.Instance = this;
        }
    }
}