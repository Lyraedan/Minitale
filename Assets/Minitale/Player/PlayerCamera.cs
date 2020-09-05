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

        public float movementSpeed = 50f; // 30 seems good for a player speed

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {
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
