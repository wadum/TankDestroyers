using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using Assets.Scripts.UI;
using Assets.Scripts.Variables;
using UnityEngine;
using UnityEngine.Networking;
using PlayerController = Assets.Scripts.PlayerController;

public class RoundKeeper : NetworkBehaviour
{

    private float _endTime;
    [SyncVar]
    public long TimeLeft;

    [SyncVar] 
    public string ScoreText;

    private Dictionary<short, int> Score = new Dictionary<short, int>(); 


    public override void OnStartServer()
    {
        base.OnStartServer();
        _endTime = Time.time + 60*5;
    }

    // Update is called once per frame
	void Update () {
        if(!isServer)
            return;

        TimeLeft = (long) (_endTime - Time.time);

        if (TimeLeft < 0)
        {

            var players = GameObject.FindGameObjectsWithTag(Constants.Tags.Player);


            var sb = new StringBuilder();
            var scorekeys = Score.Keys.OrderBy(k => -Score[k]);
            foreach (var scoreKey in scorekeys)
            {
               var player = players.FirstOrDefault(p => p.GetComponent<NetworkIdentity>().netId.Value == scoreKey);
                if (player != null)
                {
                    sb.AppendFormat("{0} : {1}\n", player.GetComponent<PlayerController>().PlayerName, Score[scoreKey]);
                }
            }
            ScoreText = sb.ToString();
            RpcShowHightscore();
	    }

	}

    [ClientRpc]
    void RpcShowHightscore()
    {
        GameObject.FindGameObjectWithTag(Constants.Tags.HighScore).GetComponent<HighScore>().Done(ScoreText);
        foreach (var player in GameObject.FindGameObjectsWithTag(Constants.Tags.Player))
        {
            player.GetComponent<PlayerController>().DisableMovement();
        }
    }

    public void AddScoreTo(short playerId)
    {
        if (!isServer)
        {
            Debug.LogError("Would this ever happen??!");
        }

        int val = 0;
        if (Score.TryGetValue(playerId, out val))
            Score[playerId] = ++val;
        else
            Score[playerId] = 1;
    }
}
