﻿using Dalamud.Game.ClientState.JobGauge.Types;
using ImGuiNET;
using System;
using System.Text;

namespace BossMod
{
    class WARActions
    {
        private unsafe FFXIVClientStructs.FFXIV.Client.Game.ActionManager* _actionManager = null;

        public bool Enabled = true;
        public WARRotation.State State { get; private set; }
        public WARRotation.Strategy Strategy;
        public WARRotation.AID NextBestAction = WARRotation.AID.HeavySwing;

        public unsafe WARActions()
        {
            _actionManager = FFXIVClientStructs.FFXIV.Client.Game.ActionManager.Instance();
            State = BuildState(WARRotation.AID.None, 0);

            Strategy = new()
            {
                SpendGauge = true,
                EnableUpheaval = true,
                EnableMovement = false,
                NeedChargeIn = 0,
                Aggressive = false,
            };
        }

        public void Update(WARRotation.AID comboLastAction, float comboTimeLeft)
        {
            var currState = BuildState(comboLastAction, comboTimeLeft);
            LogStateChange(State, currState);
            State = currState;
            NextBestAction = Enabled ? WARRotation.GetNextBestAction(State, Strategy) : WARRotation.AID.HeavySwing;
        }

        public void DrawActionHint(bool extended)
        {
            ImGui.Checkbox("Spend mode", ref Strategy.SpendGauge);
            ImGui.Checkbox("Enable movement", ref Strategy.EnableMovement);
            ImGui.Text($"Next: {NextBestAction}, GCD={State.GCD:f1}");
            if (extended)
            {
                ImGui.Checkbox("Enable", ref Enabled);
                ImGui.SliderFloat("Need charge in", ref Strategy.NeedChargeIn, 0, 30);
                ImGui.Text($"Gauge: {State.Gauge}");
                ImGui.Text($"Surging tempest left: {State.SurgingTempestLeft:f1}");
                ImGui.Text($"Nascent chaos left: {State.NascentChaosLeft:f1}");
                ImGui.Text($"Primal rend left: {State.PrimalRendLeft:f1}");
                ImGui.Text($"Inner release left: {State.InnerReleaseLeft:f1} ({State.InnerReleaseStacks} stacks)");
                ImGui.Text($"Combo State: last={State.ComboLastMove}, time left={State.ComboTimeLeft:f1}");
                ImGui.Text($"Infuriate: {State.InfuriateCD:f1}");
                ImGui.Text($"Upheaval: {State.UpheavalCD:f1}");
                ImGui.Text($"Inner Release: {State.InnerReleaseCD:f1}");
                ImGui.Text($"Onslaught: {State.OnslaughtCD:f1}");
                ImGui.Text($"Next GCD: {WARRotation.GetNextBestGCD(State, Strategy)}");
                ImGui.Text($"Next oGCD: {WARRotation.GetNextBestOGCD(State, Strategy)}");
            }
        }

        public unsafe float ActionCooldown(WARRotation.AID action)
        {
            return _actionManager->GetRecastTime(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Spell, (uint)action) - _actionManager->GetRecastTimeElapsed(FFXIVClientStructs.FFXIV.Client.Game.ActionType.Spell, (uint)action);
        }

        public WARRotation.State BuildState(WARRotation.AID comboLastAction, float comboTimeLeft)
        {
            WARRotation.State s = new();
            if (Service.ClientState.LocalPlayer != null)
            {
                s.ComboTimeLeft = comboTimeLeft;
                s.ComboLastMove = comboLastAction;
                s.Gauge = Service.JobGauges.Get<WARGauge>().BeastGauge;

                foreach (var status in Service.ClientState.LocalPlayer.StatusList)
                {
                    switch ((WARRotation.StatusID)status.StatusId)
                    {
                        case WARRotation.StatusID.SurgingTempest:
                            s.SurgingTempestLeft = status.RemainingTime;
                            break;
                        case WARRotation.StatusID.NascentChaos:
                            s.NascentChaosLeft = status.RemainingTime;
                            break;
                        case WARRotation.StatusID.InnerRelease:
                            s.InnerReleaseLeft = status.RemainingTime;
                            s.InnerReleaseStacks = status.StackCount;
                            break;
                        case WARRotation.StatusID.PrimalRend:
                            s.PrimalRendLeft = status.RemainingTime;
                            break;
                    }
                }

                s.InfuriateCD = ActionCooldown(WARRotation.AID.Infuriate);
                s.UpheavalCD = ActionCooldown(WARRotation.AID.Upheaval);
                s.InnerReleaseCD = ActionCooldown(WARRotation.AID.InnerRelease);
                s.OnslaughtCD = ActionCooldown(WARRotation.AID.Onslaught);
                s.GCD = ActionCooldown(WARRotation.AID.HeavySwing);
            }
            return s;
        }

        private static void LogStateChange(WARRotation.State prev, WARRotation.State curr)
        {
            // do nothing if not in combat
            if (Service.ClientState.LocalPlayer == null || !Service.ClientState.LocalPlayer.StatusFlags.HasFlag(Dalamud.Game.ClientState.Objects.Enums.StatusFlags.InCombat))
                return;

            // detect expired buffs
            if (curr.InnerReleaseLeft == 0 && prev.InnerReleaseLeft != 0 && prev.InnerReleaseLeft < 1)
                LogMistake("Inner Release expired", curr);
            if (curr.NascentChaosLeft == 0 && prev.NascentChaosLeft != 0 && prev.NascentChaosLeft < 1)
                LogMistake("Nascent Chaos expired", curr);
            if (curr.PrimalRendLeft == 0 && prev.PrimalRendLeft != 0 && prev.PrimalRendLeft < 1)
                LogMistake("Primal Rend expired", curr);
            if (curr.SurgingTempestLeft == 0 && prev.SurgingTempestLeft != 0 && prev.SurgingTempestLeft < 1)
                LogMistake("Surging Tempest expired", curr);

            // detect cast GCDs
            // note: curr.GCD > prev.GCD check doesn't really work, since gcd is updated before things like combo and gauge
            var gcdAction = DetectGCDAction(prev, curr);
            if (gcdAction != WARRotation.AID.None)
            {
                LogCast(true, gcdAction, prev, curr);
                switch (gcdAction)
                {
                    case WARRotation.AID.HeavySwing:
                        if (prev.ComboLastMove == WARRotation.AID.HeavySwing || prev.ComboLastMove == WARRotation.AID.Maim)
                            LogMistake($"wrong combo move - {gcdAction} after {prev.ComboLastMove}", curr);
                        break;
                    case WARRotation.AID.Maim:
                        if (prev.ComboLastMove != WARRotation.AID.HeavySwing)
                            LogMistake($"wrong combo move - {gcdAction} after {prev.ComboLastMove}", curr);
                        if (prev.Gauge > 90)
                            LogMistake($"{gcdAction} overcapped gauge", curr);
                        break;
                    case WARRotation.AID.StormPath:
                        if (prev.ComboLastMove != WARRotation.AID.Maim)
                            LogMistake($"wrong combo move - {gcdAction} after {prev.ComboLastMove}", curr);
                        if (prev.Gauge > 80)
                            LogMistake($"{gcdAction} overcapped gauge", curr);
                        if (prev.SurgingTempestLeft <= 0)
                            LogMistake($"{gcdAction} without ST", curr);
                        break;
                    case WARRotation.AID.StormEye:
                        if (prev.ComboLastMove != WARRotation.AID.Maim)
                            LogMistake($"wrong combo move - {gcdAction} after {prev.ComboLastMove}", curr);
                        if (prev.Gauge > 90)
                            LogMistake($"{gcdAction} overcapped gauge", curr);
                        if (prev.SurgingTempestLeft > 30)
                            LogMistake($"{gcdAction} overcapped ST", curr);
                        break;
                    case WARRotation.AID.FellCleave:
                        if (prev.InfuriateCD < 5)
                            LogMistake($"{gcdAction} overcapped Infuriate CD", curr);
                        break;
                    case WARRotation.AID.InnerChaos:
                        if (prev.Gauge == curr.Gauge)
                            LogMistake($"{gcdAction} under IR", curr);
                        if (prev.InfuriateCD < 5)
                            LogMistake($"{gcdAction} overcapped Infuriate CD", curr);
                        break;
                    case WARRotation.AID.Overpower:
                        if (prev.ComboLastMove == WARRotation.AID.Overpower)
                            LogMistake($"wrong combo move - {gcdAction} after {prev.ComboLastMove}", curr);
                        break;
                    case WARRotation.AID.MythrilTempest:
                        if (prev.ComboLastMove != WARRotation.AID.Overpower)
                            LogMistake($"wrong combo move - {gcdAction} after {prev.ComboLastMove}", curr);
                        if (prev.Gauge > 80)
                            LogMistake($"{gcdAction} overcapped gauge", curr);
                        break;
                }
            }

            // detect cast oGCDs
            if (curr.InfuriateCD > prev.InfuriateCD)
            {
                LogCast(false, WARRotation.AID.Infuriate, prev, curr);
                if (prev.Gauge > 50)
                    LogMistake($"{WARRotation.AID.Infuriate} overcapped gauge", curr);
                if (prev.NascentChaosLeft > 0)
                    LogMistake($"{WARRotation.AID.Infuriate} overwritten existing NC", curr);
                if (prev.InnerReleaseStacks > 0)
                    LogMistake($"{WARRotation.AID.Infuriate} under IR", curr);
            }

            if (curr.OnslaughtCD > prev.OnslaughtCD)
            {
                LogCast(false, WARRotation.AID.Onslaught, prev, curr);
                // note: onslaught without ST is not really a mistake...
            }

            if (curr.UpheavalCD > prev.UpheavalCD)
            {
                LogCast(false, WARRotation.AID.Upheaval, prev, curr);
                if (prev.SurgingTempestLeft <= 0)
                    LogMistake($"{WARRotation.AID.Upheaval} without ST", curr);
            }

            if (curr.InnerReleaseCD > prev.InnerReleaseCD)
            {
                LogCast(false, WARRotation.AID.InnerRelease, prev, curr);
                if (prev.SurgingTempestLeft <= 0)
                    LogMistake($"{WARRotation.AID.InnerRelease} without ST", curr);
                if (prev.NascentChaosLeft > 0)
                    LogMistake($"{WARRotation.AID.InnerRelease} under NC", curr);
                if (prev.SurgingTempestLeft > 50)
                    LogMistake($"{WARRotation.AID.InnerRelease} overcapped ST", curr);
            }
        }

        private static WARRotation.AID DetectGCDAction(WARRotation.State prev, WARRotation.State curr)
        {
            if (curr.ComboTimeLeft > prev.ComboTimeLeft)
                return curr.ComboLastMove; // correct combo actions
            // incorrect combo actions or expiration set last-move to none and time to 0

            if (curr.Gauge < prev.Gauge)
                return prev.NascentChaosLeft > 0 ? WARRotation.AID.InnerChaos : WARRotation.AID.FellCleave;

            if (curr.PrimalRendLeft == 0 && prev.PrimalRendLeft > 1)
                return WARRotation.AID.PrimalRend;

            if (curr.InnerReleaseStacks < prev.InnerReleaseStacks && prev.InnerReleaseLeft > 1)
                return prev.NascentChaosLeft > 0 ? WARRotation.AID.InnerChaos : WARRotation.AID.FellCleave;

            // unknown...
            return WARRotation.AID.None;
        }

        private static void LogCast(bool isGCD, WARRotation.AID action, WARRotation.State prev, WARRotation.State curr)
        {
            var sb = new StringBuilder($"[AR] Cast {(isGCD ? "GCD" : "oGCD")}: {action}");
            if (curr.Gauge < prev.Gauge)
                sb.Append(" (spent gauge)");
            if (curr.InnerReleaseStacks < prev.InnerReleaseStacks && prev.InnerReleaseLeft > 1)
                sb.Append(" (spent IR stack)");
            sb.Append($" [{StateString(curr)}]");
            Service.Log(sb.ToString());
        }

        private static void LogMistake(string text, WARRotation.State state)
        {
            Service.Log($"[AR] Mistake: {text} [{StateString(state)}]");
        }

        private static string StateString(WARRotation.State s)
        {
            return $"g={s.Gauge}, ST={s.SurgingTempestLeft:f1}, NC={s.NascentChaosLeft:f1}, PR={s.PrimalRendLeft:f1}, IR={s.InnerReleaseStacks}/{s.InnerReleaseLeft:f1}, IRCD={s.InnerReleaseCD:f1}, InfCD={s.InfuriateCD:f1}, UphCD={s.UpheavalCD:f1}, OnsCD={s.OnslaughtCD:f1}, GCD={s.GCD:f1}";
        }
    }
}