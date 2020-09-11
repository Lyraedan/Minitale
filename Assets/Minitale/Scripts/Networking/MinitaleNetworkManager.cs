using Lyraedan.MirrorChat;
using Minitale.WorldGen;
using Mirror;
using UnityEngine;

namespace Minitale.Networking
{
    public class MinitaleNetworkManager : NetworkManager
    {

        public Transform initialSpawn;

        public override void OnClientConnect(NetworkConnection conn)
        {
            base.OnClientConnect(conn);
            WorldGenerator.generator.ClearChunks();
        }

        public override void OnServerAddPlayer(NetworkConnection conn)
        {
            GameObject player = Instantiate(playerPrefab, initialSpawn.position, initialSpawn.rotation);
            NetworkServer.AddPlayerForConnection(conn, player);

            Debug.Log($"User joiend the server {conn.connectionId}");
        }

        public override void OnServerDisconnect(NetworkConnection conn)
        {
            if(conn.identity.isLocalPlayer)
            {
                Camera.main.transform.parent.SetParent(null);
            }
            Chat chat = GameObject.FindGameObjectWithTag("Player").GetComponent<Chat>();
            chat.Send($"User {conn.connectionId} has left the server!", "#FFFF00");

            Debug.Log($"User left the server {conn.connectionId}");
            base.OnServerDisconnect(conn);
        }

    }
}