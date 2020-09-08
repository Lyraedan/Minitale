using Minitale.WorldGen;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minitale.Player
{
    public class PlayerController : CameraControl
    {
        public KeyCode Up = KeyCode.W;
        public KeyCode Left = KeyCode.A;
        public KeyCode Down = KeyCode.S;
        public KeyCode Right = KeyCode.D;

        public float movementSpeed = 15f;
        public float rotationSpeed = 15f;

        // Start is called before the first frame update
        private void Start()
        {

            if (!hasAuthority)
            {
                Destroy(gameObject.transform.Find("Minimap"));
                gameObject.tag = "OtherPlayer";
                DestroyImmediate(this);
                return;
            }
            //Camera.main.transform.parent.SetParent(transform);
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
            WoWCamera camera = Camera.main.gameObject.AddComponent<WoWCamera>();
            camera.target = transform;
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
            Vector3 dir = new Vector3(0, 0, 0);

            dir.x = Input.GetAxis("Horizontal");
            dir.z = Input.GetAxis("Vertical");

            transform.Translate(dir * movementSpeed * Time.deltaTime);

            if(!Input.GetMouseButton(0))
                RotatePlayer();
        }

        void RotatePlayer()
        {
            var dir = new Vector3(Camera.main.transform.forward.x, 0, Camera.main.transform.forward.z).normalized;
            if (dir.sqrMagnitude < 0.1f) return;

            var target = Quaternion.LookRotation(dir, Vector3.up);
            transform.rotation = Quaternion.Slerp(transform.rotation, target, rotationSpeed * Time.deltaTime);
        }
    }
}
