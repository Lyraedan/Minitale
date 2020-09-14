using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Lyraedan.MirrorChat
{
    public class Channel
    {

        public string name = string.Empty;
        private List<string> messages = new List<string>();

        public Channel(string name)
        {
            this.name = name;
        }

        public string[] GetStashedMessages()
        {
            return messages.ToArray();
        }

        public int GetMessageCount()
        {
            return messages.Count;
        }

        public void StashMessage(string message)
        {
            messages.Add(message);
        }

    }
}