using NLog;
using System;
using System.Collections.Generic;

namespace Block2D.Server.Commands
{
    public class ChatCommandParser
    {
        private readonly Dictionary<string, Command> _commands;
        private readonly ServerLogger _logger;

        public ChatCommandParser(ServerLogger logger)
        {
            _commands = [];
            _logger = logger;
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

            _commands.Add("/" + commandName, command);
        }

        public void SetAction(string commandName, Action<string> task)
        {
            Command c = _commands[commandName];
            c.Action = task;
            _commands[commandName] = c;
        }

        public bool TryParseMessage(string message, PermissionLevel playerPermissionLevel, out Command command)
        {
            command = default;

            string commandName = GetCommandFirst(message);

            if (!_commands.TryGetValue("/" + commandName, out Command value))
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

        public bool TryExecuteCommand(string text, PermissionLevel playerPermissionLevel)
        {
            string commandName = GetCommandFirst(text);
            string args = text.Remove(0, commandName.Length + 1);

            return TryExecuteCommand(commandName, args, playerPermissionLevel);
        }

        public bool TryExecuteCommand(string text, ServerPlayer player, bool log)
        {
            string commandName = GetCommandFirst(text);
            string args = text.Remove(0, commandName.Length + 1);

            if(log)
            {
                _logger.LogInfo(player.Name + " ran command: " + commandName + " with arguments: " + args);
            }

            return TryExecuteCommand(commandName, args, player.PermissionLevel);
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
