using System;

namespace Lyraedan.MirrorChat
{
    public class CommandExecutor
    {

        public string command = string.Empty;
        public Action<object[]> execution;

        public CommandExecutor(string command, Action<object[]> execution)
        {
            this.command = command;
            this.execution = execution;
        }

        public void Execute(object[] parameters)
        {
            execution.Invoke(parameters);
        }
    }
}