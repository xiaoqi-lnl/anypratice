using HutongGames.PlayMaker;
using HutongGames.PlayMaker.Actions;

namespace CustomRadAttacks
{
    internal static class ChoiceHooks
    {
        // 钩子是全局的，靠 FSM 身份过滤，只处理辐光那一个（design §5.3）
        internal const string RadianceGoName = "Absolute Radiance";
        internal const string ChoicesFsmName = "Attack Choices";

        private static readonly AttackSequence Sequence = new AttackSequence();

        internal static void HookChoice(On.HutongGames.PlayMaker.Actions.SendRandomEventV3.orig_OnEnter orig,
                                        SendRandomEventV3 self)
        {
            Fsm fsm = self.Fsm;
            if (fsm == null || fsm.GameObjectName != RadianceGoName || fsm.Name != ChoicesFsmName)
            {
                orig(self);
                return;
            }

            RadPhase phase;
            if (!TryPhase(self, out phase)) { orig(self); return; }

            CustomRadAttacksSettings s = CustomRadAttacks.Settings;
            if (!s.Enabled || s.Mode == ChoiceMode.Random) { orig(self); return; }

            AttackDef def;
            if (s.Mode == ChoiceMode.LockSingle)
            {
                def = AttackCatalog.Find(phase == RadPhase.P1 ? s.LockedA1 : s.LockedA2);
            }
            else
            {
                Sequence.Loop = s.LoopSequence;
                Sequence.LoadSlots(s.Slots);
                def = Sequence.Next(phase);
            }

            string evt = def == null ? null : def.EventFor(phase);
            if (evt == null) { orig(self); return; }   // 空槽 / 本阶段没这招 → 交还原版

            // 不调 orig：原版随机被完全抑制（SendRandomEventV3 的防重复计数也随之冻住，切回随机后自行恢复）
            Send(fsm, evt);
        }

        // 阶段只认状态名，不认 HP —— AnyRadiance 2 会整体改血量阈值（design §7.2）
        private static bool TryPhase(FsmStateAction self, out RadPhase phase)
        {
            phase = RadPhase.P1;
            string st = self.State != null ? self.State.Name : self.Fsm.ActiveStateName;
            if (st == "A1 Choice") { phase = RadPhase.P1; return true; }
            if (st == "A2 Choice") { phase = RadPhase.P2; return true; }
            return false;
        }

        internal static void Send(Fsm fsm, string eventName)
        {
            FsmEvent e = FindEvent(fsm, eventName);
            if (e == null)
            {
                CustomRadAttacks.Instance.LogError("事件找不到: " + eventName);
                return;
            }
            fsm.Event(e);
        }

        internal static FsmEvent FindEvent(Fsm fsm, string name)
        {
            if (fsm.Events != null)
            {
                for (int i = 0; i < fsm.Events.Length; i++)
                {
                    FsmEvent e = fsm.Events[i];
                    if (e != null && e.Name == name) return e;
                }
            }
            return FsmEvent.GetFsmEvent(name);
        }
    }
}
