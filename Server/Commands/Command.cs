using System;

namespace Block2D.Server.Commands
{
    public struct Command
    {
        public string Name;
        public CommandArgsType ArgsType;
        public Action<string> Action;
        public PermissionLevel RequiredPermissionLevel;
    }
}
