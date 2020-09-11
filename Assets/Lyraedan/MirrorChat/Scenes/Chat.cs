using Mirror;
using System;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lyraedan.MirrorChat {
    public class Chat : NetworkBehaviour
    {
        public KeyCode sendKey = KeyCode.Return;
        public TMP_Text chatText = null;
        public TMP_InputField inputText = null;
        public string username = "Player";
        private static event Action<string> onMessage;

        public static Chat instance;

        private void Start()
        {
            instance = this;
        }

        public override void OnStartAuthority()
        {
            onMessage += UpdateChat;
        }

        void UpdateChat(string message)
        {
            chatText.text += message;
        }

        [Client]
        public void Send()
        {
            if (!Input.GetKeyDown(sendKey)) return;
            string input = inputText.text;
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                Debug.LogError("Input is empty or whitespace!");
                return;
            }
            string message = $"<b>[{username}]:</b> {input}";
            inputText.text = string.Empty;
            Debug.Log($"Sending message: {message}");
            CmdSend(message);
        }

        [Client]
        public void Send(string message, string col = "#FF0000")
        {
            string msg = $"<color={col}>{message}</color>";
            inputText.text = string.Empty;
            CmdSend(msg);
        }

        [Command]
        public void CmdSend(string message)
        {
            RpcSend(message);
        }

        [ClientRpc]
        public void RpcSend(string message)
        {
            onMessage?.Invoke($"{message}\n");
        }

        public TMP_Text GetText(GameObject owner)
        {
            return owner.transform.Find("Canvas").Find("Panel").Find("Scroll View").Find("Viewport").Find("Text (TMP)").GetComponent<TMP_Text>();
        }

        public TMP_InputField GetInput(GameObject owner)
        {
            return owner.transform.Find("Canvas").Find("Panel").Find("InputField (TMP)").GetComponent<TMP_InputField>();
        }
    }
}