using System.Collections.Generic;
using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

public class RoundKeeper : NetworkBehaviour
{

    private float _endTime;
    [SyncVar]
    public long TimeLeft;

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
            NetworkManager.singleton.ServerChangeScene("game");
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
