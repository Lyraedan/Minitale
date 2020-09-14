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
        [Tooltip("What key should be used to send messages from the input box")]public KeyCode sendKey = KeyCode.Return;
        [HideInInspector] public TMP_Text channelDisplay;
        [HideInInspector] public TMP_Text chatText = null;
        [HideInInspector] public TMP_InputField inputText = null;
        [Tooltip("What should the player appear as in the chat?")] public string username = "Player";
        [Tooltip("What should the command prefix be? - Default is /")] public string commandPrefix = "/";
        [Tooltip("What key/token should the chat consider to be used as the whisper trigger? - Private messaging")] public string whisperKey = "Whisper";
        [Tooltip("What channels should exist on startup?")] public string[] startupChannels = { "Global", "Team", "Whisper" };
        #endregion

        #region Static vars
        private static event Action<string> onMessage;
        public static Chat instance;
        #endregion

        #region Channel vars
        private Dictionary<string, Channel> channels = new Dictionary<string, Channel>();
        [HideInInspector] public Channel currentlyActiveChannel;
        #endregion

        #region Command vars
        private Dictionary<string, CommandExecutor> commands = new Dictionary<string, CommandExecutor>();
        #endregion

        #region Init
        public override void OnStartAuthority()
        {
            instance = this;
            onMessage += UpdateChat;
        }

        public void Setup(GameObject owner, bool defaultSetup = true) 
        {
            for (int i = 0; i < startupChannels.Length; i++)
            {
                string key = startupChannels[i].ToLower();
                channels.Add(key, new Channel(startupChannels[i]));

                CommandExecutor switchCommand = new CommandExecutor(key, (_params) =>
                {
                    if (key.Contains(whisperKey))
                    {
                        string whisperKey = GetWhisperKey(username, (string)_params[0]);
                        SwitchChannel(whisperKey);
                    }
                    else
                    {
                        SwitchChannel(key);
                    }
                });

                RegisterCommand(switchCommand);
            }

            currentlyActiveChannel = channels[startupChannels[0].ToLower()];

            if(defaultSetup)
            {
                chatText = GetText(owner);
                inputText = GetInput(owner);
                channelDisplay = GetChannelDisplay(owner);
                channelDisplay.text = $"<color=#00FF00>Channel: {currentlyActiveChannel.name}</color>";
                inputText.onEndEdit.AddListener(delegate
                {
                    Send();
                });
            }
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
            if (instance.currentlyActiveChannel.name.Equals(channel))
            {
                onMessage?.Invoke($"{message}\n");
                instance.currentlyActiveChannel.StashMessage(message);
            }
            else
            {
                if (!instance.channels.ContainsKey(channel)) instance.CreateChannel(channel, channel);
                instance.channels[channel].StashMessage(message);
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

        public string GetWhisperKey(string sender, string recipient)
        {
            string[] names = { sender, recipient };
            Array.Sort(names, (a, b) => string.Compare(a, b));
            return $"{whisperKey}_{names[0]}_{names[1]}";
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

        #region Getters

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