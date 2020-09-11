using Lyraedan.MirrorChat;
using Minitale.WorldGen;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

namespace Minitale.Player
{
    public class PlayerController : CameraControl
    {
        [Header("UI Elements")]
        public GameObject chat;
        [Header("Controls")]
        public KeyCode Up = KeyCode.W;
        public KeyCode Left = KeyCode.A;
        public KeyCode Down = KeyCode.S;
        public KeyCode Right = KeyCode.D;
        public KeyCode sprint = KeyCode.LeftShift;

        [Header("Player properties")]
        public float movementSpeed = 15f;
        public float sprintingSpeed = 30f;
        public float rotationSpeed = 15f;

        private bool sprinting = false;

        // Start is called before the first frame update
        private void Start()
        {

            if (!hasAuthority)
            {
                Destroy(gameObject.transform.Find("Minimap"));
                gameObject.tag = "OtherPlayer";
                //Destroy(GetComponent<Chat>()); // Seriously wtf

                DestroyImmediate(this);
                return;
            }

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
            SetupChat();
            GetComponent<Chat>().Send($"<b>{GetComponent<Chat>().username} joined the game!", "#FFFF00");
        }

        void SetupChat()
        {
            GameObject chatGO = Instantiate(chat, new Vector3(0, 0, 0), Quaternion.identity);
            Chat chatInstance = GetComponent<Chat>();
            chatInstance.username = $"User_{netIdentity.netId}";
            chatInstance.chatText = chatInstance.GetText(chatGO);
            chatInstance.inputText = chatInstance.GetInput(chatGO);
            chatInstance.inputText.onEndEdit.AddListener(delegate
            {
                chatInstance.Send();
            });
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
            sprinting = Input.GetKeyDown(sprint);

            Vector3 dir = new Vector3(0, 0, 0);

            dir.x = Input.GetAxis("Horizontal");
            dir.z = Input.GetAxis("Vertical");

            transform.Translate(dir * (sprinting ? sprintingSpeed : movementSpeed) * Time.deltaTime);

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
