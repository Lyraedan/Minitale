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

        public float zoom = -15f;
        private Vector3 previousPosition;

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

            //TerrainEditorCamera cameraControl = gameObject.AddComponent<TerrainEditorCamera>();
            //cameraControl.Target = transform;
        }

        [Client]
        void Update()
        {
            if (!hasAuthority) return;
            HandleWorld();
            Move();
            Zoom();
            RotateCamera(1);
        }

        public void RotateCamera(int mouseButton)
        {
            Camera cam = Camera.main;
            if(Input.GetMouseButtonDown(mouseButton))
            {
                previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            }

            if(Input.GetMouseButton(mouseButton))
            {
                Vector3 direction = previousPosition - cam.ScreenToViewportPoint(Input.mousePosition);

                cam.transform.position = transform.position;

                //if (mouseButton == 0) RotatePlayer(cam, direction);

                Debug.Log(cam.transform.rotation.y);
                cam.transform.Rotate(new Vector3(1, 0, 0), direction.y * 180);
                cam.transform.Rotate(new Vector3(0, 1, 0), -direction.x * 180, Space.World);
                cam.transform.Translate(new Vector3(0, 0, zoom));

                previousPosition = cam.ScreenToViewportPoint(Input.mousePosition);
            }
        }

        public void RotatePlayer(Camera cam, Vector3 direction)
        {
            var lookPos = direction;
            lookPos.y = 0f;
            var rotationAngle = Quaternion.LookRotation(-lookPos);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotationAngle, Time.deltaTime * 5f); // 5 is damp
        }

        public void Zoom()
        {
            zoom += Input.GetAxis("Mouse ScrollWheel");
            Camera.main.transform.Translate(new Vector3(0, 0, Mathf.Clamp(zoom, 0, -50f)));
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

    public class TerrainEditorCamera : MonoBehaviour
    {
        public Transform Target;

        public float _x;
        public float _y;
        private float _yOffset;
        private float _xOffset;
        public float _distance = 21f;
        public float _heightOffset;
        public float Deadzone = 0.1f;
        public float XSpeed = 1f;
        public float YSpeed = 1f;

        private float _quaterScreenWidth;

        private const float _deadzone = 0.05f;
        private Vector3 _oldMousePos;
        public Vector3 _moveOffset;

        void Start()
        {
            _moveOffset = new Vector3(0, -1.3f, -2.3f);

            UpdateCameraPosition();
        }

        private void Update()
        {
            float x = 0f;
            float y = 0f;

            var dif = Input.mousePosition - _oldMousePos;

            if (Input.GetMouseButton(1))
            {
                y -= (dif.y / 5f);
                x += (dif.x / 10f);
            }
            if (Input.GetMouseButton(2))
            {
                _moveOffset -= ((transform.right * (dif.x / 30f)) + (transform.up * (dif.y / 15f)));
            }

            _oldMousePos = Input.mousePosition;

            if (Input.mouseScrollDelta.y > 0) _distance -= 7.5f;
            else if (Input.mouseScrollDelta.y < 0) _distance += 7.5f; ;
            SetCameraPosition(x, y);
            UpdateCameraPosition();
        }

        private void FixedUpdate()
        {
            UpdateCameraPosition();
        }

        void UpdateCameraPosition()
        {
            UpdateCamera(_x, _y, _heightOffset);
        }

        void UpdateCamera(float x, float y, float heightOffset)
        {
            transform.rotation = Quaternion.Euler(y, x, 0);
            transform.position = transform.rotation * new Vector3(0f, 0.5f, -_distance) + Target.position + new Vector3(0, heightOffset, 0) + _moveOffset;
        }

        public void UpdateInputs(Vector2 input)
        {
        }

        void SetCameraPosition(float x, float y)
        {
            if (Target == null) return;
            if (Mathf.Abs(x) <= Deadzone && Mathf.Abs(y) <= Deadzone) return;

            var offset = CanMove(
                x * XSpeed,
                y * YSpeed
            );

            _x += offset.x;
            _y += offset.y;

            if (_x < 0f) _x += 360f;
            if (_x > 360f) _x -= 360f;

            _y = Mathf.Clamp(_y, 1f, 89f);
        }

        public void SetCamera(float x, float y)
        {
            /*
            _x = _moveOffset.x + x;
            _y = _moveOffset.y + y;

            if (_x < 0f) _x += 360f;
            if (_x > 360f) _x -= 360f;

            _y = Mathf.Clamp(_y, 1f, 89f);
            */
            UpdateCamera(x, y, _heightOffset);
        }

        //Not going to hit a wall or anything, potentially tell the camera controller if we are
        private Vector2 CanMove(float offsetX, float offsetY)
        {
            var offset = new Vector2(offsetX, offsetY);

            if (!CanMoveToPosition(new Vector3(_y + offsetY, _x, 0))) offset.y = 0f;
            if (!CanMoveToPosition(new Vector3(_y, _x + offsetX, 0))) offset.x = 0f;

            return offset;
        }

        private bool CanMoveToPosition(Vector3 offset)
        {
            var rotation = Quaternion.Euler(offset);
            var position = rotation * new Vector3(0.0f, 0.5f, -_distance) + Target.position + new Vector3(0f, _heightOffset, 0f);
            var direction = position - transform.position;
            var moveDistance = Vector3.Distance(transform.position, position);

            return (!Physics.Raycast(transform.position, direction, out RaycastHit hit, moveDistance + 0.1f));
        }
    }
}
