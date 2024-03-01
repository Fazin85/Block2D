﻿using Riptide;
using Riptide.Utils;
using System.Collections.Generic;

namespace Block2D.Server
{
    public class InternalServer : ITickable
    {
        public static InternalServer Instance;
        
        public bool IsRunning
        {
            get => _server == null ? false : _server.IsRunning;
        }

        public World World
        {
            get => _world;
        }

        private Riptide.Server _server;
        private World _world;

        public InternalServer()
        {
            Instance = this;
        }

        public void Start(ushort maxClientCount)
        {
            if(IsRunning)
            {
                return;
            }

            _server = new Riptide.Server();
            
            RiptideLogger.Initialize(Main.Logger.Info, false);

            _server.Start(7777, maxClientCount);
            _server.ClientConnected += OnClientConnected;
        }

        public void OnClientConnected(object sender, ServerConnectedEventArgs args)
        {
            
        }

        public void OnClientDisconnected(object sender, ServerDisconnectedEventArgs args)
        {
            _world.RemovePlayer((ushort)(args.Client.Id));
        }

        public void Tick()
        {
            _server.Update();
        }

        public void Stop()
        {
            _server.Stop();
        }
    }
}