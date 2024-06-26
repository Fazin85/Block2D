﻿using Block2D.Server.Networking;
using Block2D.Server.World;
using Riptide;
using System.Threading;

namespace Block2D.Server
{
    public class InternalServer
    {
        public bool IsRunning
        {
            get => _server != null && _server.IsRunning;
        }

        public ServerLogger Logger { get; private set; }

        public ServerWorld World { get; private set; }

        public ServerState State { get; private set; }

        public ServerMessageHandler MessageHandler { get; private set; }

        public ServerAssetManager AssetManager { get; private set; }

        public bool ShouldExit { get; private set; }

        private Riptide.Server _server;
        private Thread _executionThread;

        public InternalServer()
        {
            State = ServerState.Inactive;
            Logger = new();
            MessageHandler = new(this);
            AssetManager = new();
        }

        public void Start()
        {
            if (IsRunning)
            {
                return;
            }

            ShouldExit = false;

            _executionThread = new(Run);
            _executionThread.Start();
        }

        public void Exit()
        {
            if (IsRunning)
            {
                ShouldExit = true;
                Stop();
            }
        }

        public void Send(Message message, ushort toClientId)
        {
            _server.Send(message, toClientId);
        }

        public void SendToAll(Message message)
        {
            _server.SendToAll(message);
        }

        private void Stop()
        {
            _server.Stop();

            AssetManager.UnloadAllMods();
        }

        private void Run()
        {
            Setup(20);

            while (!ShouldExit)
            {
                if (_server.IsRunning)
                {
                    //do ticking stuff here
                    World.Update();

                    _server.Update();
                }

                Thread.Sleep(16);
            }
        }

        private void Setup(ushort maxClientCount)
        {
            if (State == ServerState.Starting)
            {
                return;
            }

            State = ServerState.Starting;

            if (IsRunning)
            {
                return;
            }

            AssetManager.LoadContent();

            World = new(AssetManager, this, "DevWorld");

            World.LoadContent();

            _server = new Riptide.Server();

            _server.MessageReceived += OnMessageReceived;
            _server.ClientConnected += OnClientConnected;
            _server.ClientDisconnected += OnClientDisconnected;

            _server.Start(7777, maxClientCount, useMessageHandlers: false);

            State = ServerState.Loaded;
        }

        private void OnMessageReceived(object sender, MessageReceivedEventArgs e)
        {
            switch (e.MessageId)
            {
                case (ushort)ServerMessageID.HandlePlayerJoin:
                    MessageHandler.HandlePlayerJoin(e.FromConnection.Id, e.Message);
                    break;
                case (ushort)ServerMessageID.ReceivePosition:
                    MessageHandler.ReceivePosition(e.FromConnection.Id, e.Message);
                    break;
                case (ushort)ServerMessageID.HandleChunkRequest:
                    MessageHandler.HandleChunkRequest(e.FromConnection.Id, e.Message);
                    break;
                case (ushort)ServerMessageID.ReceiveChatMessage:
                    MessageHandler.HandleChatMessage(e.FromConnection.Id, e.Message);
                    break;
                case (ushort)ServerMessageID.ReceivePlayerDisconnect:
                    MessageHandler.HandlePlayerDisconnect(e.FromConnection.Id);
                    break;
            }
        }

        private void OnClientConnected(object sender, ServerConnectedEventArgs args) { }

        private void OnClientDisconnected(object sender, ServerDisconnectedEventArgs args)
        {
            World.RemovePlayer(args.Client.Id);
        }
    }
}
