using System;
using System.Collections.Generic;

namespace Block2D.Server.Commands
{
    public class ChatCommandParser
    {
        private readonly Dictionary<string, Command> Commands;

        public ChatCommandParser()
        {
            Commands = [];
        }

        public void RegisterCommand(string commandName, CommandArgsType argsType, PermissionLevel requiredPermissionLevel)
        {
            Command command = new()
            {
                Name = commandName,
                ArgsType = argsType,
                Action = null,
                RequiredPermissionLevel = requiredPermissionLevel
            };

            Commands.Add(commandName, command);
        }

        public void SetAction(string commandName, Action<string> task)
        {
            Command c = Commands[commandName];
            c.Action = task;
            Commands[commandName] = c;
        }

        public bool TryParseMessage(string message, PermissionLevel playerPermissionLevel, out Command command)
        {
            command = default;

            string commandName = GetCommandFirst(message);

            if (!Commands.TryGetValue(commandName, out Command value))
            {
                return false;
            }

            command = value;

            if ((int)playerPermissionLevel >= (int)command.RequiredPermissionLevel)
            {
                return true;
            }

            return false;
        }

        public bool TryExecuteCommand(string commandName, string args, PermissionLevel playerPermissionLevel)
        {
            if (TryParseMessage(commandName, playerPermissionLevel, out Command command))
            {
                command.Action.Invoke(args);

                return true;
            }

            return false;
        }

        public static string GetCommandFirst(string message)
        {
            if (!message.Contains(' '))
            {
                return message;
            }

            string value = message;

            int index = value.IndexOf(' ');

            value = value.Remove(index, value.Length - index);//remove everything after the first space

            return value;
        }
    }
}
