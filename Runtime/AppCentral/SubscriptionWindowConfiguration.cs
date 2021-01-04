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
        public string restoreLinkText;
        public string termsLinkText;
        public string encouragementTitleText;
        public string subscriptionButtonText;
        public Sprite backgroundImage;
        public Sprite foregroundImage;
        public Sprite subscriptionButtonImage;
    }
}