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
        //This needs to be accessed and set outside of the chat script!
        public string username = "Player";
        StringBuilder chat = new StringBuilder();

        public static Chat instance;

        private void Start()
        {
            instance = this;
        }

        public override void OnStartAuthority()
        {
            Debug.Log($"Started Authority: {hasAuthority}");
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
            CmdSend(message, Color.white);
        }

        [Client]
        public void Send(string message, Color col)
        {
            string msg = $"{message}";
            inputText.text = string.Empty;
            CmdSend(message, col);
        }

        [Command]
        public void CmdSend(string message, Color col)
        {
            Debug.Log($"Issuing command to send to users {message}");
            RpcSend(message, col);
        }

        [ClientRpc]
        public void RpcSend(string message, Color col)
        {
            chat.Append($"{message}\n");
            chatText.color = col;
            chatText.text = chat.ToString();
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