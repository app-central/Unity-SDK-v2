
namespace AppCentral
{
    #region using
    using System;
    using System.Linq;
    using UnityEngine;
    using Object = UnityEngine.Object;
    using UnityEngine.UI;
    using TMPro;
    using UnityEngine.Serialization;
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
        [SerializeField] private TextMeshProUGUI termsLinkTMP;
        [SerializeField] private TextMeshProUGUI restoreLinkTMP;
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

            SubscriptionWindow.Instance.gameObject.SetActive(true);

            SubscriptionWindow.WindowOpen = true;
        }

        public static void HidePanel()
        {
            SubscriptionWindow.WindowOpen = false;

            SubscriptionWindow.Instance.gameObject.SetActive(false);
        }

        private void CheckAssignments()
        {
            if (this.backgroundImage == null)
            { this.backgroundImage = this.GetComponentsInChildren<Image>()
                                         .FirstOrDefault(image => image.gameObject.name.ToLower().Contains("background")); }
            if (this.foregroundImage == null)
            { this.foregroundImage = this.GetComponentsInChildren<Image>()
                                         .FirstOrDefault(image => image.gameObject.name.ToLower().Contains("foreground")); }
            if (this.topTitleTMP == null)
            { this.topTitleTMP = this.GetComponentsInChildren<TextMeshProUGUI>()
                                     .FirstOrDefault(tmp => tmp.gameObject.name.ToLower().Contains("top")
                                                            && tmp.gameObject.name.ToLower().Contains("title")); }
            if (this.topSubtitleTMP == null)
            { this.topSubtitleTMP = this.GetComponentsInChildren<TextMeshProUGUI>()
                                        .FirstOrDefault(tmp => tmp.gameObject.name.ToLower().Contains("top")
                                                               && tmp.gameObject.name.ToLower().Contains("subtitle")); }
            if (this.termsLinkTMP == null)
            { this.termsLinkTMP = this.GetComponentsInChildren<TextMeshProUGUI>()
                                        .FirstOrDefault(tmp => tmp.gameObject.name.ToLower().Contains("terms")
                                                               && tmp.gameObject.name.ToLower().Contains("button")); }
            if (this.restoreLinkTMP == null)
            { this.restoreLinkTMP = this.GetComponentsInChildren<TextMeshProUGUI>()
                                         .FirstOrDefault(tmp => tmp.gameObject.name.ToLower().Contains("restore")
                                                                && tmp.gameObject.name.ToLower().Contains("button")); }
            if (this.bottomTitleTMP == null)
            { this.bottomTitleTMP = this.GetComponentsInChildren<TextMeshProUGUI>()
                                          .FirstOrDefault(tmp => tmp.gameObject.name.ToLower().Contains("bottom")
                                                                 && tmp.gameObject.name.ToLower().Contains("title")); }
            if (this.bottomSubtitleTMP == null)
            { this.bottomSubtitleTMP = this.GetComponentsInChildren<TextMeshProUGUI>()
                                           .FirstOrDefault(tmp => tmp.gameObject.name.ToLower().Contains("bottom")
                                                                  && tmp.gameObject.name.ToLower().Contains("subtitle")); }
            if (this.subscriptionButtonImage == null)
            { this.subscriptionButtonImage = this.GetComponentsInChildren<Image>()
                                                 .FirstOrDefault(image => image.gameObject.name.ToLower().Contains("subscription")
                                                                          && image.gameObject.name.ToLower().Contains("button")); }
            if (this.subscriptionButtonTMP == null)
            { this.subscriptionButtonTMP = this.GetComponentsInChildren<TextMeshProUGUI>()
                                               .FirstOrDefault(tmp => tmp.gameObject.name.ToLower().Contains("subscribe")
                                                                      && tmp.gameObject.name.ToLower().Contains("tmp")); }
        }

        /// <summary>Read configuration file into window parameters. Called by field context menu.</summary>
        private void ReadConfiguration()
        {
            if (this.backgroundImage != null)
            { this.backgroundImage.sprite = this.subscriptionConfiguration.backgroundImage; }
            if (this.foregroundImage != null)
            { this.foregroundImage.sprite = this.subscriptionConfiguration.foregroundImage; }
            if (this.foregroundImage != null)
            {
                Color foregroundImageColor = this.foregroundImage.color;
                foregroundImageColor.a = this.subscriptionConfiguration.foregroundOpacity;
                this.foregroundImage.color = foregroundImageColor;
            }
            if (this.subscriptionButtonImage != null)
            { this.subscriptionButtonImage.sprite = this.subscriptionConfiguration.subscriptionButtonImage; }
            
            if (this.topTitleTMP != null)
            {this.topTitleTMP.alignment = TextAlignmentOptions.Center;}
            if (this.topSubtitleTMP != null)
            {this.topSubtitleTMP.alignment = TextAlignmentOptions.Center;}
            if (this.subscriptionButtonTMP != null)
            {this.subscriptionButtonTMP.alignment = TextAlignmentOptions.Center;}
            if (this.bottomTitleTMP != null)
            {this.bottomTitleTMP.alignment = TextAlignmentOptions.Center;}
            if (this.bottomSubtitleTMP != null)
            {this.bottomSubtitleTMP.alignment = TextAlignmentOptions.Center;}
            if (this.termsLinkTMP != null)
            {this.termsLinkTMP.alignment = TextAlignmentOptions.MidlineRight;}
            if (this.restoreLinkTMP != null)
            {this.restoreLinkTMP.alignment = TextAlignmentOptions.MidlineLeft;}
            if (this.topTitleTMP != null)
            {this.topTitleTMP.text = this.subscriptionConfiguration.topTitleText;}
            if (this.topSubtitleTMP != null)
            {this.topSubtitleTMP.text = this.subscriptionConfiguration.topSubtitleText;}
            if (this.termsLinkTMP != null)
            {this.termsLinkTMP.text = this.subscriptionConfiguration.topLeftLinkText;}
            if (this.restoreLinkTMP != null)
            {this.restoreLinkTMP.text = this.subscriptionConfiguration.topRightLinkText;}
            if (this.bottomTitleTMP != null)
            {this.bottomTitleTMP.text = this.subscriptionConfiguration.bottomTitleText;}
            if (this.bottomSubtitleTMP != null)
            {this.bottomSubtitleTMP.text = this.subscriptionConfiguration.bottomSubtitleText;}
            if (this.subscriptionButtonTMP != null)
            {this.subscriptionButtonTMP.text = this.subscriptionConfiguration.subscriptionButtonText;}
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