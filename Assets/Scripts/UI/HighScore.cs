using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HighScore : NetworkBehaviour
    {

        public GameObject ServerButton;

        public void Done(string scoreText)
        {
            var textComp = transform.FindChild("Text").gameObject;
            textComp.SetActive(true);
            textComp.GetComponent<Text>().text = scoreText;
            ServerButton.SetActive(true);

            foreach (var cr in GetComponentsInChildren<CanvasRenderer>())
            {
                cr.SetAlpha(1);
            }
        }

        public void OnButtonClick()
        {

            Debug.Log("clicked button..!");

            NetworkManager.singleton.ServerChangeScene("game");
        }
        
    }
}
