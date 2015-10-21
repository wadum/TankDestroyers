using System.Linq;
using Assets.Scripts.Variables;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts.UI
{
    public class HighScore : NetworkBehaviour
    {

        public GameObject ServerButton;
        private GameObject _pc;

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

            _pc = GameObject.FindGameObjectsWithTag(Constants.Tags.Player).FirstOrDefault(p => p.GetComponent<PlayerController>().isLocalPlayer);
        }

        public void Reset()
        {
            var textComp = transform.FindChild("Text").gameObject;
            textComp.SetActive(false);
            ServerButton.SetActive(false);

            foreach (var cr in GetComponentsInChildren<CanvasRenderer>())
            {
                cr.SetAlpha(0);
            }
        }

        public void OnButtonClick()
        {
            Debug.Log("clicked button..!");
            _pc.GetComponent<PlayerController>().CmdRestartLevel();
        }
        
    }
}
