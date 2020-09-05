using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Minitale.Player
{
    public class TitleCamera : CameraControl
    {

        public float movementSpeed;

        // Start is called before the first frame update
        void Start()
        {
            Init();
        }

        // Update is called once per frame
        void Update()
        {
            HandleWorld();
            transform.position = new Vector3(transform.position.x + movementSpeed * Time.deltaTime, transform.position.y, transform.position.z + movementSpeed * Time.deltaTime);
        }
    }
}