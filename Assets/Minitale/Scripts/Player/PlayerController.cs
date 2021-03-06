﻿using Lyraedan.MirrorChat;
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
            gameObject.AddComponent<Raycast>();
            WoWCamera camera = Camera.main.gameObject.AddComponent<WoWCamera>();
            camera.target = transform;
            SetupChat();
            GetComponent<Chat>().chatText.text = string.Empty;
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
            Interaction();
        }

        void Interaction()
        {
            if (Input.GetKeyDown(KeyCode.Space))
            {
                Debug.Log("Left click");
                if (Raycast.instance.GetHit().transform == null) return;
                Debug.Log("Raycasting");
                if (Raycast.instance.GetHit().collider.gameObject.tag.Equals("Breakable"))
                {
                    Debug.Log("Hit breakable");
                    GameObject go = Raycast.instance.GetHit().collider.gameObject;
                    if (Raycast.instance.IsWithinRange(go.transform.position, 15f))
                    {
                        DestroyObject(go);
                    }
                }
            }
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


        [Client]
        public void DestroyObject(NetworkIdentity id)
        {
            Debug.Log($"[Client] Destorying object {id}");
            CmdDestroyObject(id);
        }

        [Command]
        public void CmdDestroyObject(NetworkIdentity id)
        {
            Debug.Log($"[Command] Destorying object {id}");
            RpcDestroyObject(id);
        }

        [ClientRpc]
        public void RpcDestroyObject(NetworkIdentity id)
        {
            Debug.Log($"[RPC] Destorying object {id}");
            NetworkServer.Destroy(id.gameObject);
        }
    }
}
