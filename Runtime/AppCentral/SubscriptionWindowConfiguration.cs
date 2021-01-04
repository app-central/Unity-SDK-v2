namespace AppCentral
{
    using System;
    using UnityEngine;

    [Serializable]
    public struct ProductIDs
    {
        public string subscriptionProductID;
    }

    [CreateAssetMenu(menuName = "ScriptableObjects/AppCentral/Subscription Window Configuration", order = 1)]
    public class SubscriptionWindowConfiguration : ScriptableObject
    {
        public ProductIDs productIDs;
        public string topTitleText;
        public string topSubtitleText;
        public string topLeftLinkText;
        public string topRightLinkText;
        public string bottomTitleText;
        public string bottomSubtitleText;
        public string subscriptionButtonText;
        public Sprite backgroundImage;
        public Sprite foregroundImage;
        public float foregroundOpacity = 1.0f;
        public Sprite subscriptionButtonImage;
    }
}