using System;
using System.Collections.Generic;
using System.IO;
using Modding;
using Newtonsoft.Json;

namespace CustomRadAttacks
{
    public class CustomRadAttacks : Mod, IMenuMod
    {
        internal static CustomRadAttacks Instance;
        internal static CustomRadAttacksSettings Settings = new CustomRadAttacksSettings();

        public override string GetVersion() { return "0.1.0.0"; }

        public CustomRadAttacks() : base("自定义辐光招式")
        {
            Instance = this;
            LoadSettings();
        }

        public override void Initialize(Dictionary<string, Dictionary<string, UnityEngine.GameObject>> preloadedObjects)
        {
            Log("CustomRadAttacks v" + GetVersion() + " init, enabled=" + Settings.Enabled + ", mode=" + Settings.Mode);
        }

        // 配置文件放 mod 自己目录（Mods\自定义辐光招式\Settings.json），整包自包含
        private static string SettingsPath
        {
            get
            {
                string dir = Path.GetDirectoryName(typeof(CustomRadAttacks).Assembly.Location);
                return dir == null ? null : Path.Combine(dir, "Settings.json");
            }
        }

        private static void LoadSettings()
        {
            try
            {
                string path = SettingsPath;
                if (path != null && File.Exists(path))
                {
                    Settings = JsonConvert.DeserializeObject<CustomRadAttacksSettings>(File.ReadAllText(path))
                        ?? new CustomRadAttacksSettings();
                }
            }
            catch (Exception ex)
            {
                Instance.LogError("Settings load failed: " + ex.Message);
                Settings = new CustomRadAttacksSettings();
            }
            if (Settings.Slots == null) Settings.Slots = new string[8];
        }

        internal static void SaveSettings()
        {
            try
            {
                string path = SettingsPath;
                if (path == null) return;
                string dir = Path.GetDirectoryName(path);
                if (dir != null && !Directory.Exists(dir)) Directory.CreateDirectory(dir);
                File.WriteAllText(path, JsonConvert.SerializeObject(Settings, Formatting.Indented));
            }
            catch (Exception ex)
            {
                Instance.LogError("Settings save failed: " + ex.Message);
            }
        }

        public bool ToggleButtonInsideMenu { get { return true; } }

        public List<IMenuMod.MenuEntry> GetMenuData(IMenuMod.MenuEntry? menu)
        {
            return new List<IMenuMod.MenuEntry>
            {
                new IMenuMod.MenuEntry(
                    "启用",
                    new[] { "Off", "On" },
                    "总开关：关掉时全部交还原版辐光",
                    value => { Settings.Enabled = value == 1; SaveSettings(); },
                    () => Settings.Enabled ? 1 : 0),
                new IMenuMod.MenuEntry(
                    "模式",
                    new[] { "随机（原版）", "锁单招", "锁序列" },
                    "随机 = 完全原版；\n锁单招 = 每轮都出指定的那一招；\n锁序列 = 按 8 个槽位逐轮出",
                    value => { Settings.Mode = (ChoiceMode)value; SaveSettings(); },
                    () => (int)Settings.Mode)
            };
        }
    }
}
