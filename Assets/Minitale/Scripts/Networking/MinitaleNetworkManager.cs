using Minitale.Player;
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
            GameObject player = Instantiate(playerPrefab, initialSpawn.position, initialSpawn.rotation);
            NetworkServer.AddPlayerForConnection(conn, player);
            /*
            if(conn.identity.isLocalPlayer)
            {
                GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
                Debug.Log($"Spawn points found {spawnPoints.Length}");

                /*
                //TODO Make it move the player to a spawn point AFTER the level has generated
                int chosen = Random.Range(0, spawnPoints.Length);
                Debug.Log($"Chose spawn {chosen}");

                Vector3 spawn = spawnPoints[chosen].transform.position;
                conn.identity.gameObject.transform.position = spawn;
                */
                /*
                conn.identity.gameObject.tag = "Player";
                Camera.main.transform.parent.SetParent(conn.identity.gameObject.transform);
            } else
            {
                conn.identity.gameObject.tag = "OtherPlayer";
                DestroyImmediate(conn.identity.gameObject.GetComponent<PlayerCamera>());
            }
            */

            Debug.Log($"User joiend the server {conn.connectionId}");
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if(conn.identity.isLocalPlayer)
            {
                Camera.main.transform.parent.SetParent(null);
            }
            Debug.Log($"User left the server {conn.connectionId}");
            base.OnServerDisconnect(conn);
        }

    }
}