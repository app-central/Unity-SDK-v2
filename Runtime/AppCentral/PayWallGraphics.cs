namespace AppCentral
{
    using UnityEngine;

    public class PayWallGraphics : MonoBehaviour
    {
        // TODO: Shouldn't be handling graphics
        // TODO: And should use new UI anyway
        public static PayWallGraphics Instance { get; private set; }

        private GUIStyle currentStyle;
        private int windowWidth;
        private int windowHeight;
        private Rect windowRect;
        // Turning this on show the GUI
        public bool windowOpen;

        private static Texture2D MakeTex(int width, int height, Color col)
        {
            Color[] pix = new Color[width * height];
            for (int i = 0; i < pix.Length; ++i)
            { pix[i] = col; }

            Texture2D result = new Texture2D(width, height);
            result.SetPixels(pix);
            result.Apply();
            return result;
        }

        private void OnGUI()
        {
            if (this.currentStyle == null || this.currentStyle.normal.background == null)
            { this.currentStyle = new GUIStyle(GUI.skin.box) { normal = { background =
                                                                            PayWallGraphics.MakeTex(2, 2, new Color(0.13f, 0.41f, 0.52f, 0.9f)) } }; }

            if (this.windowOpen)
            {
                this.windowWidth = 600;
                this.windowHeight = 600;
                this.windowRect = new Rect((Screen.width - this.windowWidth) / 2,
                                           (Screen.height - this.windowHeight) / 2, this.windowWidth, this.windowHeight);
                GUI.ModalWindow(0, this.windowRect, this.DoMyWindow, "", this.currentStyle);
            }
        }

        private void DoMyWindow(int windowID)
        {
            GUIStyle myButtonStyle = new GUIStyle(GUI.skin.button) { fontSize = 35 };

            Color color = Color.white;
            color.a = 0.0f;
            GUI.backgroundColor = color;
            if (GUI.Button(new Rect(10, 10, 40, 40), "X", myButtonStyle))
            { this.windowOpen = false; }

            GUIStyle myLabelStyle = new GUIStyle(GUI.skin.label) { fontSize = 35, alignment = TextAnchor.UpperCenter };
            GUI.Label(new Rect(0, 100, this.windowWidth, 300),
                      Purchaser.localizedTitle + "\n" + Purchaser.localizedDescription + "\n\nJust " + Purchaser.localizedPriceString +
                      "/month",
                      myLabelStyle);

            const int ExtraButtonsWidth = 200;
            if (GUI.Button(new Rect(50, this.windowHeight - 100, ExtraButtonsWidth, 50), "Terms", myButtonStyle))
            {
                MonoBehaviour.print("Terms");
                // TODO: Should be a const field or from a configuration file
                Application.OpenURL("https://www.app-central.com/terms");
            }

            if (GUI.Button(new Rect(this.windowWidth - ExtraButtonsWidth - 50, this.windowHeight - 100, ExtraButtonsWidth, 50), "Restore", myButtonStyle))
            {
                MonoBehaviour.print("Restore");
                Purchaser.RestorePurchases();
                this.windowOpen = false;
            }

            color.a = 1.0f;
            GUI.backgroundColor = color;
            GUIStyle purchaseButtonStyle = new GUIStyle(GUI.skin.button) { fontSize = 50 };

            const int ButtonWidth = 400;
            if (GUI.Button(new Rect(this.windowWidth / 2 - ButtonWidth / 2, 300, ButtonWidth, 100), "Subscribe", purchaseButtonStyle))
            {
                MonoBehaviour.print("Purchasing!");
                Purchaser.BuySubscription();
                this.windowOpen = false;
            }
        }

        private void Awake()
        {
            if (PayWallGraphics.Instance == null)
            { PayWallGraphics.Instance = this; }
        }
    }
}