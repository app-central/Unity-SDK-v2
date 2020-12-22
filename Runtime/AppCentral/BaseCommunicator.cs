using UnityEngine;
using UnityEngine.Networking;

namespace AppCentral
{
    public class BaseCommunicator
    {
        public static void SendApplicationStartRequest()
        {
            UnityWebRequest.Get("https://vnc412s287.execute-api.us-east-1.amazonaws.com/default/unity-tracker?v=1&action=start&appid="
                                + Application.identifier).SendWebRequest();
        }
    }
}