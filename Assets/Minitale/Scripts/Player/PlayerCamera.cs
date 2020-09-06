using Minitale.WorldGen;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minitale.Player
{
    public class PlayerCamera : CameraControl
    {

        public KeyCode Up = KeyCode.W;
        public KeyCode Left = KeyCode.A;
        public KeyCode Down = KeyCode.S;
        public KeyCode Right = KeyCode.D;

        public float movementSpeed = 15f;

        // Start is called before the first frame update
        private void Start()
        {

            if (!hasAuthority)
            {
                gameObject.tag = "OtherPlayer";
                DestroyImmediate(this);
                return;
            }
            Camera.main.transform.parent.SetParent(transform);
            for (int x = -1; x <= 1; x++)
            {
                for (int z = -1; z <= 1; z++)
                {
                    WorldGenerator.generator.GenerateChunkAt(x, 0f, z);
                    WorldGenerator.GetChunkAt(x, 0f, z).RenderChunk(true);
                }
            }
            GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");
            int chosen = Random.Range(0, spawnPoints.Length);
            Vector3 spawn = spawnPoints[chosen].transform.position;
            gameObject.transform.position = spawn;
            Init();
        }

        [Client]
        void Update()
        {
            if (!hasAuthority) return;
            HandleWorld();
            Move();
        }

        private void Move()
        {
            if(Input.GetKey(Up))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z + movementSpeed * Time.deltaTime);
            }
            if (Input.GetKey(Down))
            {
                transform.position = new Vector3(transform.position.x, transform.position.y, transform.position.z - movementSpeed * Time.deltaTime);
            }
            if (Input.GetKey(Left))
            {
                transform.position = new Vector3(transform.position.x - movementSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            }
            if (Input.GetKey(Right))
            {
                transform.position = new Vector3(transform.position.x + movementSpeed * Time.deltaTime, transform.position.y, transform.position.z);
            }
        }
    }
}
