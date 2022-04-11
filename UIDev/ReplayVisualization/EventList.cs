﻿using BossMod;
using ImGuiNET;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace UIDev
{
    class EventList
    {
        private Replay _replay;
        private Tree _tree = new();
        private OpList _opList;

        public EventList(Replay r)
        {
            _replay = r;
            _opList = new(r, _tree);
        }

        public void Draw()
        {
            foreach (var n in _tree.Node("Full data"))
            {
                DrawContents(null);
            }
            foreach (var e in _tree.Nodes(_replay.Encounters, e => ($"{ModuleRegistry.TypeForOID(e.OID)}: {e.InstanceID:X}, zone={e.Zone}, start={e.Time.Start:O}, duration={e.Time}", false)))
            {
                DrawContents(e);
                DrawPlayerActions(e);
            }
        }

        private void DrawContents(Replay.Encounter? filter)
        {
            var moduleType = filter != null ? ModuleRegistry.TypeForOID(filter.OID) : null;
            var oidType = moduleType?.Module.GetType($"{moduleType.Namespace}.OID");
            var aidType = moduleType?.Module.GetType($"{moduleType.Namespace}.AID");
            var sidType = moduleType?.Module.GetType($"{moduleType.Namespace}.SID");
            var iidType = moduleType?.Module.GetType($"{moduleType.Namespace}.IconID");
            var tidType = moduleType?.Module.GetType($"{moduleType.Namespace}.TetherID");
            var reference = filter?.Time.Start ?? _replay.Ops.First().Timestamp;
            var tp = TimePrinter(reference);
            var actions = filter != null ? _replay.EncounterActions(filter) : _replay.Actions;
            var statuses = filter != null ? _replay.EncounterStatuses(filter) : _replay.Statuses;
            var tethers = filter != null ? _replay.EncounterTethers(filter) : _replay.Tethers;
            var icons = filter != null ? _replay.EncounterIcons(filter) : _replay.Icons;
            var envControls = filter != null ? _replay.EncounterEnvControls(filter) : _replay.EnvControls;

            //foreach (var n in _tree.Node("Raw ops"))
            //{
            //    _opList.Draw(filter != null ? _replay.Ops.SkipWhile(o => o.Timestamp < filter.Time.Start).TakeWhile(o => o.Timestamp <= filter.Time.End) : _replay.Ops, reference);
            //}

            foreach (var n in _tree.Node("Participants"))
            {
                if (filter == null)
                {
                    DrawParticipants(_replay.Participants, actions, statuses, tp, reference, filter, aidType, sidType);
                }
                else
                {
                    foreach (var (oid, list) in _tree.Nodes(filter.Participants, kv => ($"{kv.Key:X} '{oidType?.GetEnumName(kv.Key)}' ({kv.Value.Count} objects)", false)))
                    {
                        DrawParticipants(list, actions, statuses, tp, reference, filter, aidType, sidType);
                    }
                }
            }

            bool haveActions = actions.Any();
            Func<Replay.Action, bool> actionIsCrap = a => a.Source?.Type is ActorType.Player or ActorType.Pet or ActorType.Chocobo;
            foreach (var n in _tree.Node("Interesting actions", !haveActions))
            {
                DrawActions(actions.Where(a => !actionIsCrap(a)), tp, aidType);
            }
            foreach (var n in _tree.Node("Other actions", !haveActions))
            {
                DrawActions(actions.Where(actionIsCrap), tp, aidType);
            }

            bool haveStatuses = statuses.Any();
            Func<Replay.Status, bool> statusIsCrap = s => (s.Source?.Type is ActorType.Player or ActorType.Pet or ActorType.Chocobo) || (s.Target?.Type is ActorType.Pet or ActorType.Chocobo);
            foreach (var n in _tree.Node("Interesting statuses", !haveStatuses))
            {
                DrawStatuses(statuses.Where(s => !statusIsCrap(s)), tp, sidType);
            }
            foreach (var n in _tree.Node("Other statuses", !haveStatuses))
            {
                DrawStatuses(statuses.Where(statusIsCrap), tp, sidType);
            }

            foreach (var n in _tree.Node("Tethers", !tethers.Any()))
            {
                _tree.LeafNodes(tethers, t => $"{tp(t.Time.Start)} + {t.Time}: {t.ID} ({tidType?.GetEnumName(t.ID)}) @ {ReplayUtils.ParticipantString(t.Source)} -> {ReplayUtils.ParticipantString(t.Target)}");
            }

            foreach (var n in _tree.Node("Icons", !icons.Any()))
            {
                _tree.LeafNodes(icons, i => $"{tp(i.Timestamp)}: {i.ID} ({iidType?.GetEnumName(i.ID)}) @ {ReplayUtils.ParticipantString(i.Target)}");
            }

            foreach (var n in _tree.Node("EnvControls", !envControls.Any()))
            {
                _tree.LeafNodes(envControls, ec => $"{tp(ec.Timestamp)}: {ec.Feature:X8}.{ec.Index:X2} = {ec.State:X8}");
            }
        }

        private void DrawParticipants(IEnumerable<Replay.Participant> list, IEnumerable<Replay.Action> actions, IEnumerable<Replay.Status> statuses, Func<DateTime, string> tp, DateTime reference, Replay.Encounter? filter, Type? aidType, Type? sidType)
        {
            foreach (var p in _tree.Nodes(list, p => ($"{ReplayUtils.ParticipantString(p)}: spawn at {tp(p.Existence.Start)}, despawn at {tp(p.Existence.End)}", p.Casts.Count == 0 && !p.HasAnyActions && !p.HasAnyStatuses && !p.IsTargetOfAnyActions && p.TargetableHistory.Count == 0)))
            {
                if (p.Casts.Count > 0)
                {
                    foreach (var n in _tree.Node("Casts"))
                    {
                        DrawCasts(p.Casts, reference, aidType);
                    }
                }
                if (p.HasAnyActions)
                {
                    foreach (var an in _tree.Node("Actions"))
                    {
                        DrawActions(actions.Where(a => a.Source == p), tp, aidType);
                    }
                }
                if (p.IsTargetOfAnyActions)
                {
                    foreach (var an in _tree.Node("Affected by actions"))
                    {
                        DrawActions(actions.Where(a => a.Targets.Any(t => t.Target == p)), tp, aidType);
                    }
                }
                if (p.HasAnyStatuses)
                {
                    foreach (var an in _tree.Node("Statuses"))
                    {
                        DrawStatuses(statuses.Where(s => s.Target == p), tp, sidType);
                    }
                }
                if (p.TargetableHistory.Count > 0)
                {
                    foreach (var an in _tree.Node("Targetable"))
                    {
                        _tree.LeafNodes(p.TargetableHistory, r => $"{tp(r.Key)} = {r.Value}");
                    }
                }
            }
        }

        private string CastString(Replay.Cast c, DateTime reference, DateTime prev, Type? aidType)
        {
            return $"{new Replay.TimeRange(reference, c.Time.Start)} ({new Replay.TimeRange(prev, c.Time.Start)}) + {c.ExpectedCastTime + 0.3f:f2} ({c.Time}): {c.ID} ({aidType?.GetEnumName(c.ID.ID)}) @ {ReplayUtils.ParticipantString(c.Target)} / {Utils.Vec3String(c.Location)}";
        }

        private void DrawCasts(IEnumerable<Replay.Cast> list, DateTime reference, Type? aidType)
        {
            var prev = reference;
            foreach (var c in _tree.Nodes(list, c => (CastString(c, reference, prev, aidType), true)))
            {
                prev = c.Time.End;
            }
        }

        private string ActionString(Replay.Action a, Func<DateTime, string> tp, Type? aidType)
        {
            return $"{tp(a.Timestamp)}: {a.ID} ({aidType?.GetEnumName(a.ID.ID)}): {ReplayUtils.ParticipantPosRotString(a.Source, a.Timestamp)} -> {ReplayUtils.ParticipantString(a.MainTarget)} {Utils.Vec3String(a.TargetPos)} ({a.Targets.Count} affected)";
        }

        private void DrawActions(IEnumerable<Replay.Action> list, Func<DateTime, string> tp, Type? aidType)
        {
            foreach (var a in _tree.Nodes(list, a => (ActionString(a, tp, aidType), a.Targets.Count == 0)))
            {
                foreach (var t in _tree.Nodes(a.Targets, t => (ReplayUtils.ParticipantPosRotString(t.Target, a.Timestamp), false)))
                {
                    _tree.LeafNodes(t.Effects, ReplayUtils.ActionEffectString);
                }
            }
        }

        private string StatusString(Replay.Status s, Func<DateTime, string> tp, Type? sidType)
        {
            return $"{tp(s.Time.Start)} + {s.InitialDuration:f2} / {s.Time}: {Utils.StatusString(s.ID)} ({sidType?.GetEnumName(s.ID)}) ({s.StartingExtra:X}) @ {ReplayUtils.ParticipantString(s.Target)} from {ReplayUtils.ParticipantString(s.Source)}";
        }

        private void DrawStatuses(IEnumerable<Replay.Status> statuses, Func<DateTime, string> tp, Type? sidType)
        {
            _tree.LeafNodes(statuses, s => StatusString(s, tp, sidType));
        }

        private Func<DateTime, string> TimePrinter(DateTime start)
        {
            return t => new Replay.TimeRange(start, t).ToString();
        }

        private void OpenPlayerActions(Replay.Encounter enc, Class pcClass, Replay.Participant? pc = null)
        {
            var planner = new PlayerActions(_replay, enc, pcClass, pc);
            var w = WindowManager.CreateWindow($"Player actions timeline", planner.Draw, () => { }, () => true);
            w.SizeHint = new(600, 600);
            w.MinSize = new(100, 100);
        }

        private void DrawPlayerActions(Replay.Encounter enc)
        {
            foreach (var n in _tree.Node("Player actions timeline", false))
            {
                foreach (var c in AbilityDefinitions.Classes.Keys)
                {
                    if (ImGui.Button(c.ToString()))
                    {
                        OpenPlayerActions(enc, c);
                    }
                    foreach (var (p, _) in enc.PartyMembers.Where(pc => pc.Item2 == c))
                    {
                        ImGui.SameLine();
                        if (ImGui.Button($"{p.Name}##{p.InstanceID:X}"))
                        {
                            OpenPlayerActions(enc, c, p);
                        }
                    }
                }
            }
        }
    }
}
