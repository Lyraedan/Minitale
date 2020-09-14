using Mirror;
using System;
using System.Collections.Generic;
using System.Text;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lyraedan.MirrorChat
{
    public class Chat : NetworkBehaviour
    {
        #region Public vars
        public bool highlightHost = true;
        public KeyCode sendKey = KeyCode.Return;
        public TMP_Text channelDisplay;
        public TMP_Text chatText = null;
        public TMP_InputField inputText = null;
        public string username = "Player";
        public string commandPrefix = "/";
        public string[] startupChannels = { "Global", "Team", "Whisper" };
        #endregion

        #region Static vars
        private static event Action<string> onMessage;
        public static Chat instance;
        #endregion

        #region Channel vars
        private Dictionary<string, Channel> channels = new Dictionary<string, Channel>();
        public Channel currentlyActiveChannel;
        #endregion

        #region Command vars
        private Dictionary<string, CommandExecutor> commands = new Dictionary<string, CommandExecutor>();
        #endregion

        #region Init
        private void Awake()
        {
            instance = this;
        }

        private void Start()
        {
            CommandExecutor test = new CommandExecutor("test", (_params) => UpdateChat($"Success! {_params[0]}\n"));
            RegisterCommand(test);
        }

        public override void OnStartAuthority()
        {
            onMessage += UpdateChat;

            for(int i = 0; i < startupChannels.Length; i++)
            {
                string key = startupChannels[i].ToLower();
                channels.Add(key, new Channel(startupChannels[i]));

                CommandExecutor switchCommand = new CommandExecutor(key, (_params) =>
                {
                    SwitchChannel(key);
                });

                RegisterCommand(switchCommand);
            }
            currentlyActiveChannel = channels[startupChannels[0].ToLower()];
        }
        #endregion

        #region Main chat functionality

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
            if (highlightHost)
            {
                message = (netIdentity.isServer ? $"<color=#40E0D0>{message}</color>" : message);
            }
            bool isCommand = CheckCommand(input);
            if (!isCommand)
                CmdSend(currentlyActiveChannel.name, message);
        }

        [Client]
        public void Send(string message, string col = "#FF0000")
        {
            string msg = $"<color={col}>{message}</color>";
            inputText.text = string.Empty;
            bool isCommand = CheckCommand(message);
            if (!isCommand)
                CmdSend(currentlyActiveChannel.name, msg);
        }

        [Command]
        public void CmdSend(string channel, string message)
        {
            RpcSend(channel, message);
        }

        [ClientRpc]
        public void RpcSend(string channel, string message)
        {
            if (currentlyActiveChannel.name.Equals(channel))
            {
                onMessage?.Invoke($"{message}\n");
                currentlyActiveChannel.StashMessage(message);
            }
            else
            {
                channels[channel].StashMessage(message);
            }
        }

        #endregion

        #region Channel functionality
        public void SwitchChannel(string channelName)
        {
            if (currentlyActiveChannel.name.Equals(channelName)) return;

            if (!channels.ContainsKey(channelName.ToLower()))
            {
                CreateChannel(channelName.ToLower(), channelName);
            }

            currentlyActiveChannel = channels[channelName.ToLower()];

            channelDisplay.text = $"<color=#00FF00>Channel: {currentlyActiveChannel.name}</color>";

            //Clear the chatbox and load up the stash
            chatText.text = string.Empty;
            foreach(string message in currentlyActiveChannel.GetStashedMessages())
            {
                UpdateChat($"{message}\n");
            }
        }

        public void CreateChannel(string channelKey, string channelName)
        {
            if (!commands.ContainsKey(channelKey))
            {
                channels.Add(channelKey, new Channel(channelName));
            }
        }
        #endregion

        #region Command functionality

        public void RegisterCommand(CommandExecutor cmd)
        {
            RegisterCommand(cmd.command, cmd.execution);
        }

        public void RegisterCommand(string command, Action<object[]> execution)
        {
            if (!commands.ContainsKey(command.ToLower()))
            {
                commands.Add(command.ToLower(), new CommandExecutor(command, execution));
            }
        }

        public void UnregisterCommand(CommandExecutor cmd)
        {
            UnregisterCommand(cmd.command);
        }

        public void UnregisterCommand(string command)
        {
            if (commands.ContainsKey(command.ToLower()))
            {
                commands.Remove(command);
            }
        }

        bool CheckCommand(string input)
        {
            if (input.StartsWith(commandPrefix))
            {
                var start = DateTime.UtcNow;
                string cmd = input.Substring(1).Split(' ')[0];
                if (commands.ContainsKey(cmd.ToLower()))
                {
                    string message = $"<color=#00FF00>Executing command {cmd}!</color>\n";
                    currentlyActiveChannel.StashMessage(message);
                    UpdateChat($"{message}\n");
                    int startIndex = (cmd.Length + 2 < input.Length ? cmd.Length + 2 : 1);
                    commands[cmd.ToLower()].Execute(input.Substring(startIndex).ToLower().Split(' '));
                    Debug.Log($"<color=#00FFFF>{cmd}:</color> <color=#00FF00>{(DateTime.UtcNow - start).TotalMilliseconds}ms</color>");
                    return true;
                }
                else
                {
                    string message = "<color=#FF0000>Invalid command!</color>\n";
                    currentlyActiveChannel.StashMessage(message);
                    UpdateChat($"{message}\n");
                    Debug.Log($"<color=#00FFFF>{cmd}:</color> <color=#00FF00>{(DateTime.UtcNow - start).TotalMilliseconds}ms</color>");
                    return true;
                }
            }
            return false;
        }

        public void ExecuteCommand(string command, object[] parameters)
        {
            string paramsToPass = string.Empty;
            foreach(string param in parameters)
            {
                paramsToPass += $" {param}";
            }
            Send($"/{command}{paramsToPass}");
        }
        #endregion

        #region GETTERS

        public TMP_Text GetChannelDisplay(GameObject owner)
        {
            return owner.transform.Find("Canvas").Find("ChannelDisplay").GetComponent<TMP_Text>();
        }

        public TMP_Text GetText(GameObject owner)
        {
            return owner.transform.Find("Canvas").Find("Panel").Find("Scroll View").Find("Viewport").Find("Text (TMP)").GetComponent<TMP_Text>();
        }

        public TMP_InputField GetInput(GameObject owner)
        {
            return owner.transform.Find("Canvas").Find("Panel").Find("InputField (TMP)").GetComponent<TMP_InputField>();
        }
        #endregion
    }
}