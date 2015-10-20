using System;
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

        private void Start()
        {
            _text = GetComponent<Text>();
            rc = GameObject.FindObjectOfType<RoundKeeper>();
        }

        // Update is called once per frame
        void Update ()
        {
            TimeLeft = rc.TimeLeft;
            _text.text = String.Format("Time left: {0}:{1}", (TimeLeft / 60).ToString("00"), (TimeLeft % 60).ToString("00"));
        }
    }
}
