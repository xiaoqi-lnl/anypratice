using System;
using System.Linq;

namespace CustomRadAttacks.Tests
{
    internal static class Program
    {
        private static int _failed;

        private static void Check(bool ok, string what)
        {
            if (ok) { Console.WriteLine("PASS  " + what); }
            else { Console.WriteLine("FAIL  " + what); _failed++; }
        }

        private static int Main()
        {
            CatalogTests();
            Console.WriteLine(_failed == 0 ? "ALL PASS" : (_failed + " FAILED"));
            return _failed == 0 ? 0 : 1;
        }

        private static void CatalogTests()
        {
            // 槽位下拉：空 + 9 个名字 = 10 项
            Check(AttackCatalog.SlotOptions().Length == 10, "SlotOptions 共 10 项");
            Check(AttackCatalog.SlotOptions()[0] == AttackCatalog.Empty, "SlotOptions 第 0 项是空槽");

            // P1 锁定招 8 项，且含剑雨、不含横刺
            string[] p1 = AttackCatalog.NamesFor(RadPhase.P1);
            Check(p1.Length == 8, "P1 锁定招 8 项");
            Check(p1.Contains("剑雨"), "P1 有剑雨");
            Check(!p1.Contains("横刺"), "P1 没有横刺");

            // P2 锁定招 8 项，且含横刺、不含剑雨
            string[] p2 = AttackCatalog.NamesFor(RadPhase.P2);
            Check(p2.Length == 8, "P2 锁定招 8 项");
            Check(p2.Contains("横刺"), "P2 有横刺");
            Check(!p2.Contains("剑雨"), "P2 没有剑雨");

            // 阶段落点（design §8.1）
            Check(AttackCatalog.Find("剑雨").EventFor(RadPhase.P1) == "NAIL TOP SWEEP", "剑雨 P1 = NAIL TOP SWEEP");
            Check(AttackCatalog.Find("剑雨").EventFor(RadPhase.P2) == null, "剑雨 P2 不可用");
            Check(AttackCatalog.Find("左横刺").EventFor(RadPhase.P1) == "NAIL L SWEEP", "左横刺 P1 = NAIL L SWEEP");
            Check(AttackCatalog.Find("左横刺").EventFor(RadPhase.P2) == "NAIL LR SWEEP", "左横刺 P2 = NAIL LR SWEEP");
            Check(AttackCatalog.Find("左横刺").P2Dir == -1, "左横刺 P2 强制左");
            Check(AttackCatalog.Find("右横刺").P2Dir == 1, "右横刺 P2 强制右");
            Check(AttackCatalog.Find("横刺").EventFor(RadPhase.P1) == null, "横刺 P1 不可用");
            Check(AttackCatalog.Find("横刺").P2Dir == 0, "横刺 P2 不强制方向");
            Check(AttackCatalog.Find("脸刺").EventFor(RadPhase.P1) == "NAIL FAN", "脸刺 P1 = NAIL FAN");
            Check(AttackCatalog.Find("脸刺").EventFor(RadPhase.P2) == "NAIL FAN", "脸刺 P2 = NAIL FAN");

            // 空 / 未知名字
            Check(AttackCatalog.Find("") == null, "空槽 Find 返回 null");
            Check(AttackCatalog.Find("不存在的招") == null, "未知名字 Find 返回 null");
            Check(AttackCatalog.Find(null) == null, "null Find 返回 null");
        }
    }
}
