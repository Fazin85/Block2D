using System;
using System.Collections.Generic;

namespace Block2D.Server.Commands
{
    public class ChatCommandParser
    {
        private readonly Dictionary<string, Command> _commands;
        private readonly InternalServer _server;

        public ChatCommandParser(InternalServer server)
        {
            _commands = [];
            _server = server;

            LoadCommands();
        }

        private void LoadCommands()
        {
            RegisterCommand("STOP", CommandArgsType.None, PermissionLevel.SuperUser);
            SetAction("STOP", new((str) =>
            {
                _server.Exit();
            }));
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

            _commands.Add(commandName, command);
        }

        public void SetAction(string commandName, Action<string> action)
        {
            Command c = _commands[commandName];
            c.Action = action;
            _commands[commandName] = c;
        }

        private bool TryParseMessage(string commandName, PermissionLevel playerPermissionLevel, out Command command)
        {
            command = default;

            if (!_commands.TryGetValue(commandName, out Command value))
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

        private bool TryExecuteCommand(string commandName, string args, PermissionLevel playerPermissionLevel)
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
            string commandName = GetCommandName(text);
            string args = GetCommandArgs(text, commandName);

            return TryExecuteCommand(commandName, args, playerPermissionLevel);
        }

        public bool TryExecuteCommand(string text, ServerPlayer player, bool log)
        {
            string commandName = GetCommandName(text);
            string args = GetCommandArgs(text, commandName);

            if (log)
            {
                _server.Logger.LogInfo(player.Name + " ran command: " + commandName + " with arguments: " + args);
            }

            return TryExecuteCommand(commandName, args, player.PermissionLevel);
        }

        private string GetCommandArgs(string text, string commandName)
        {
            Command command = _commands[commandName];
            string args = string.Empty;

            if (command.ArgsType != CommandArgsType.None)
            {
                args = text.Remove(0, command.Name.Length + 1);
            }

            return args;
        }

        private string GetCommandName(string message)
        {
            if (message.Contains(' '))
            {
                int index = message.IndexOf(' ');

                message = message.Remove(index, message.Length - index);//remove everything after the first space
            }

            message = message.Replace("/", string.Empty);

            return message;
        }
    }
}
