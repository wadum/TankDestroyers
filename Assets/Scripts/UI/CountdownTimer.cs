using System;
using System.Linq;
using Assets.Scripts.Variables;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

namespace Assets.Scripts
{
    public class CountdownTimer : NetworkBehaviour
    {
        public long TimeLeft;
        private Text _text;
        private RoundKeeper rc;
        private PlayerController pc;

        private void Start()
        {
            _text = GetComponent<Text>();
            rc = FindObjectOfType<RoundKeeper>();
            FindPlayer();
        }


        private void FindPlayer()
        {
            var player = GameObject.FindGameObjectsWithTag(Constants.Tags.Player)
                .FirstOrDefault(p => p.GetComponent<PlayerController>().isLocalPlayer);
            if(player != null)
                pc = player.GetComponent<PlayerController>();
        }

        // Update is called once per frame
        private void Update()
        {
            if (pc == null)
            {
                FindPlayer();
            }
            else
            {
                TimeLeft = rc.TimeLeft;
                _text.text = String.Format("{2} \nTime left: {0}:{1}", (TimeLeft/60).ToString("00"),
                    (TimeLeft%60).ToString("00"), pc.PlayerName);
            }
        }
    }
}
