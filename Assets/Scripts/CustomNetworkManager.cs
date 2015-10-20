using UnityEngine;
using UnityEngine.Networking;

namespace Assets.Scripts
{
    public class CustomNetworkManager : NetworkManager
    {

        private static int NumberOfPlayers = 0;

        public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
        {


            GameObject player;
            
            switch (NumberOfPlayers++%4)
            {
                case 0: player = (GameObject)Instantiate(playerPrefab, new Vector3(-240, 0, -240), Quaternion.identity);
                    break;
                case 1: player = (GameObject)Instantiate(playerPrefab, new Vector3(235, 0, -240), Quaternion.identity);
                    break;
                case 2: player = (GameObject)Instantiate(playerPrefab, new Vector3(235, 0, 235), new Quaternion(0,90,0,0));
                    break;
                case 3: player = (GameObject)Instantiate(playerPrefab, new Vector3(-240, 0, 235), new Quaternion(0, 270, 0, 0));
                    break;
                default: player = (GameObject)Instantiate(playerPrefab, Vector3.zero, Quaternion.identity);
                    break;

            }

            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
        }

        

    }
}
