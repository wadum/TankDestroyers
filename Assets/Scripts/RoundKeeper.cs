using System.Net.Sockets;
using UnityEngine;
using UnityEngine.Networking;

public class RoundKeeper : NetworkBehaviour
{

    private float _endTime;
    [SyncVar]
    public long TimeLeft;


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
        Debug.Log("Player with id " + playerId + " got a point");

    }
}
