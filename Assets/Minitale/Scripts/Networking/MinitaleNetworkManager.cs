using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minitale.Networking
{
    public class MinitaleNetworkManager : NetworkManager
    {

        public Transform initialSpawn;

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            //base.OnServerAddPlayer(conn);
            Debug.Log($"User joiend the server {conn.connectionId}");
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            Debug.Log($"User left the server {conn.connectionId}");
            base.OnServerDisconnect(conn);
        }

    }
}