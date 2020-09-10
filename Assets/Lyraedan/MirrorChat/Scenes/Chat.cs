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
        public TMP_Text inputField = null;
        //This needs to be accessed and set outside of the chat script!
        public string username = "Player";
        StringBuilder chat = new StringBuilder();

        public static Chat instance;

        private void Start()
        {
            instance = this;
        }

        [Client]
        public void Send()
        {
            Debug.Log($"Sending input = {inputField.text}");
            if (!Input.GetKeyDown(sendKey))
            {
                Debug.LogError("Wrong key");
                return;
            }
            string input = inputField.text;
            if (string.IsNullOrEmpty(input) || string.IsNullOrWhiteSpace(input))
            {
                Debug.LogError("Input is empty or whitespace!");
                return;
            }
            string message = $"<b>[{username}]:</b> {input}";
            CmdSendMessageToUsers(message);
            inputField.text = string.Empty;
        }

        [Command]
        public void CmdSendMessageToUsers(string message)
        {
            RpcRecieveMessage(message);
        }

        public void RpcRecieveMessage(string message)
        {
            chat.Append($"{message}\n");
            chatText.text = chat.ToString();
        }
    }
}