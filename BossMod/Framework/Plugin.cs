﻿using Dalamud.Data;
using Dalamud.Game.ClientState;
using Dalamud.Game.ClientState.Objects;
using Dalamud.Game.Command;
using Dalamud.IoC;
using Dalamud.Logging;
using Dalamud.Plugin;
using ImGuiNET;
using System.IO;
using System.Numerics;
using System.Reflection;

namespace BossMod
{
    public sealed class Plugin : IDalamudPlugin
    {
        public string Name => "Boss Mod";

        private WorldStateGame _ws { get; } = new();
        private DebugEventLogger _debugLogger;
        private DebugUI? _debugUI;
        private IBossModule? _activeModule;

        private DalamudPluginInterface PluginInterface { get; init; }
        private CommandManager CommandManager { get; init; }
        private Configuration Configuration { get; init; }

        public Plugin(
            [RequiredVersion("1.0")] DalamudPluginInterface pluginInterface,
            [RequiredVersion("1.0")] CommandManager commandManager)
        {
            pluginInterface.Create<Service>();
            Service.LogHandler = (string msg) => PluginLog.Log(msg);
            Camera.Instance = new();

            _debugLogger = new DebugEventLogger(_ws);

            this.PluginInterface = pluginInterface;
            this.CommandManager = commandManager;

            this.Configuration = this.PluginInterface.GetPluginConfig() as Configuration ?? new Configuration();
            this.Configuration.Initialize(this.PluginInterface);

            this.CommandManager.AddHandler("/bmz", new CommandInfo(OnCommand)
            {
                HelpMessage = "Zodiark UI"
            });
            this.CommandManager.AddHandler("/bmd", new CommandInfo(OnCommand)
            {
                HelpMessage = "Debug UI"
            });

            this.PluginInterface.UiBuilder.Draw += DrawUI;
            this.PluginInterface.UiBuilder.OpenConfigUi += DrawConfigUI;

            _ws.CurrentZoneChanged += ZoneChanged;
            ZoneChanged(null, _ws.CurrentZone);
        }

        public void Dispose()
        {
            _ws.CurrentZoneChanged -= ZoneChanged;
            this.CommandManager.RemoveHandler("/bmz");
            this.CommandManager.RemoveHandler("/bmd");
        }

        private void OnCommand(string command, string args)
        {
            // in response to the slash command, just display our main ui
            if (command == "/bmz")
                ZoneChanged(null, ushort.Parse(args));
            else if (command == "/bmd" && _debugUI == null)
                _debugUI = new DebugUI(_ws);
        }

        private void ZoneChanged(object? sender, ushort zone)
        {
            _activeModule?.Dispose();
            _activeModule = null;

            switch (zone)
            {
                case 993:
                    _activeModule = new Zodiark(_ws);
                    break;
                case 1003:
                    _activeModule = new P1S(_ws);
                    break;
            }
            PluginLog.Log($"Activated module: {_activeModule?.GetType().ToString() ?? "none"}");
        }

        private void DrawUI()
        {
            Camera.Instance?.Update();
            _ws.Update();

            if (_debugUI != null)
            {
                ImGui.SetNextWindowSize(new Vector2(300, 200), ImGuiCond.FirstUseEver);
                ImGui.SetNextWindowSizeConstraints(new Vector2(300, 200), new Vector2(float.MaxValue, float.MaxValue));
                bool visible = true;
                if (ImGui.Begin("Boss mod debug UI", ref visible, ImGuiWindowFlags.None))
                {
                    _debugUI!.Draw();
                }
                ImGui.End();
                if (!visible)
                {
                    _debugUI!.Dispose();
                    _debugUI = null;
                }
            }

            if (_activeModule != null)
            {
                ImGui.SetNextWindowSize(new Vector2(400, 400), ImGuiCond.FirstUseEver);
                ImGui.SetNextWindowSizeConstraints(new Vector2(400, 400), new Vector2(float.MaxValue, float.MaxValue));
                bool visible = true;
                if (ImGui.Begin("Boss module", ref visible, ImGuiWindowFlags.NoScrollbar | ImGuiWindowFlags.NoScrollWithMouse))
                {
                    _activeModule!.Draw(Camera.Instance?.CameraAzimuth ?? 0);
                }
                ImGui.End();
                if (!visible)
                {
                    _activeModule!.Dispose();
                    _activeModule = null;
                }
            }
        }

        private void DrawConfigUI()
        {
            //this.PluginUi.SettingsVisible = true;
        }
    }
}