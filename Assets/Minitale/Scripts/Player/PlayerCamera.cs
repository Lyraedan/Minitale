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
                GameObject[] spawnPoints = GameObject.FindGameObjectsWithTag("SpawnPoint");


                //TODO Make it move the player to a spawn point AFTER the level has generated
                int chosen = Random.Range(0, spawnPoints.Length);

                Vector3 spawn = spawnPoints[chosen].transform.position;
                gameObject.transform.position = spawn;

                gameObject.tag = "OtherPlayer";
                DestroyImmediate(this);
                return;
            }
            Camera.main.transform.parent.SetParent(transform);
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
