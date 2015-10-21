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

    public float GameLength = 10;

    private Dictionary<short, int> _score = new Dictionary<short, int>(); 


    public override void OnStartServer()
    {
        base.OnStartServer();
        Reset();
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
            var scorekeys = _score.Keys.OrderBy(k => -_score[k]);
            foreach (var scoreKey in scorekeys)
            {
               var player = players.FirstOrDefault(p => p.GetComponent<NetworkIdentity>().netId.Value == scoreKey);
                if (player != null)
                {
                    sb.AppendFormat("{0} : {1}\n", player.GetComponent<PlayerController>().PlayerName, _score[scoreKey]);
                }
            }
            ScoreText = sb.ToString();
            RpcShowHightscore();
	    }

	}

    public void Reset()
    {
        _endTime = Time.time + GameLength;
        _score = new Dictionary<short, int>();
        RpcResetHighScore();
    }

    [ClientRpc]
    void RpcResetHighScore()
    {
        GameObject.FindGameObjectWithTag(Constants.Tags.HighScore).GetComponent<HighScore>().Reset();
        foreach (var player in GameObject.FindGameObjectsWithTag(Constants.Tags.Player))
        {
            player.GetComponent<PlayerController>().EnableMovement();
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
        if (_score.TryGetValue(playerId, out val))
            _score[playerId] = ++val;
        else
            _score[playerId] = 1;
    }
}
