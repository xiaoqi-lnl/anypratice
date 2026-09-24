using Galaxy.Api;
using InvulnerabilityIndicator;
using Modding;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UObject = UnityEngine.Object;
using BM = Satchel.BetterMenus;

namespace anypratice
{
    public class anypratice : Mod, IGlobalSettings<settings>, ICustomMenuMod
    {
        internal static anypratice Instance;
        private bool test = true;
        private bool test2 = false;
        private bool test3 = false;
        private bool r0 = false;
        private bool r1 = false;
        private int damage=-5;
        public settings _set = new();
        private Texture2D radianceSkin0;
        private Texture2D radianceSkin1;
        public List<GameObject> CH = new();
        public bool ToggleButtonInsideMenu => true;
        public anypratice() : base("anypratice")
        {
            Instance = this;
        }
        public override string GetVersion()
        {
            return "0.0.0.1";
        }

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            On.PlayMakerFSM.OnEnable += fsm_on;
            ChoiceHooks.Install();
            ModHooks.HeroUpdateHook += baldurfix;
            ModHooks.AfterPlayerDeadHook += carefreeset1;
            ModHooks.CharmUpdateHook += carefreeset;
            UnityEngine.SceneManagement.SceneManager.activeSceneChanged += sceneChanged;
            var stream = typeof(anypratice).Assembly.GetManifestResourceStream("anypratice.Rad0.png");
            if (stream != null)
            {
                byte[] array = new byte[stream.Length];
                stream.Read(array, 0, array.Length);
                stream.Dispose();
                radianceSkin0 = new(1, 1);
                radianceSkin0.LoadImage(array, true);
                r0 = true;
            }
            stream = typeof(anypratice).Assembly.GetManifestResourceStream("anypratice.Rad1png");
            if (stream != null)
            {
                byte[] array = new byte[stream.Length];
                stream.Read(array, 0, array.Length);
                stream.Close();
                radianceSkin1 = new(1, 1);
                radianceSkin1.LoadImage(array, true);
                r1 = true;
            }
        }

        private void carefreeset(PlayerData data, HeroController controller)
        {
            carefreeset1();
            return ;
        }

        private void sceneChanged(Scene arg0, Scene arg1)
        {
            if (_set.on)
            {
                if (_set.carereset)
                {
                    bool flag = arg1.name == "GG_Workshop";
                    if (flag) { carefreeset1(); }
                }
            }
        }

        private void carefreeset1()
        {
            if (_set.on)
            {
                if (_set.carereset)
                {
                    HeroController instance = HeroController.instance;
                    bool flag2 = instance != null;
                    if (flag2)
                    {
                        Log("OK");
                        ReflectionHelper.SetField<HeroController, int>(instance, "hitsSinceShielded", 7);
                    }
                }
            }
            return;
        }

        private void baldurfix()
        {
            if (_set.on)
            {
                if (_set.baldurfix)
                {
                    if (global::PlayerData.instance.blockerHits < 4)
                    {
                        if (Input.GetKeyUp(KeyCode.Backspace))
                        {
                            global::PlayerData.instance.blockerHits = 4;
                        }
                    }
                }
                if (_set.legacycost)
                {
                    if (global::PlayerData.instance.charmCost_32 != 2) PlayerData.instance.charmCost_32 = 2;
                }
                else
                {
                    if (PlayerData.instance.charmCost_32 != 3) PlayerData.instance.charmCost_32 = 3;
                }
                if (_set.indicator)
                {
                    if (!test2)
                    {
                        test2 = true;
                        GameObject.Find("Knight").AddComponent<Indicator>();
                    }
                }
                else
                {
                    if (test2)
                    {
                        Indicator indicator = GameObject.Find("Knight").GetComponent<Indicator>();
                        if (indicator != null) GameObject.Destroy(indicator);
                        test2 = false;
                    }
                }
            }
        }

        private void fsm_on(On.PlayMakerFSM.orig_OnEnable orig, PlayMakerFSM self)
        {
            if (_set.on)
            {
                if (self.gameObject.name == "Absolute Radiance" && self.FsmName == "Control")
                {
                    /*if (_set.cycle != 0)
                    {
                        Log("cycle ok");
                        self.gameObject.AddComponent<Cycle>();
                    }*/
                    if (_set.beamlock)
                    {
                        Log("beamlock ok");
                        self.gameObject.LocateMyFSM("Attack Commands").GetAction<RandomFloat>("Aim", 4).min = 0f;
                        self.gameObject.LocateMyFSM("Attack Commands").GetAction<RandomFloat>("Aim", 4).max = 0f;
                    }
                    if (_set.skin)
                    {
                        Log("skin ok");
                         /*if (r0 && r1)
                        {
                            //FindChild(self.gameObject);
                            //foreach (GameObject go in CH) 
                            
                                Material[] materials = self.GetComponent<tk2dSprite>().Collection.materials;
                                if (test)
                                {
                                    foreach (Material mat in materials)
                                    {
                                        TextureUtils.WriteTextureToFile(mat.mainTexture, "C:\\Users\\shownyoung\\Desktop\\temp\\" + mat.mainTexture.name + ".png");
                                    }
                                }
                            
                            if (r0) materials[0].mainTexture = radianceSkin0;
                            if (r1) materials[1].mainTexture = radianceSkin1;
                        }*/
                    }
                    if (_set.orbindicator)
                    {
                        self.gameObject.AddComponent<radIndicators>();
                    }
                    if (_set.abyssremove)
                    {
                        self.gameObject.AddComponent<abyssremover>();
                    }
                    
                }
                /*  if (self.gameObject.name == "Abyss Pit"&&self.FsmName=="Ascend")
                  {
                      if (_set.abyssremove)
                      {
                          self.GetState("Idle").InsertCustomAction(() => { self.gameObject.SetActive(false); }, 0);
                      }
                  }*/

               /* if(self.FsmName == "Superdash")
                {
                    Log("OK");
                }*/
                
            } 
            orig(self);
            
        }
        public void OnLoadGlobal(settings settings)
        {
            _set = settings;
            // 老配置文件里没有 crSlots，反序列化可能是 null
            if (_set.crSlots == null) _set.crSlots = new string[AttackSequence.SlotCount];
        }

        public settings OnSaveGlobal() => _set;

        // ===== 菜单：主菜单保留原有 9 项，自定义招式收进二级页 =====

        private BM.Menu _mainMenu;
        private BM.Menu _attackMenu;
        private MenuScreen _mainScreen;
        private MenuScreen _attackScreen;

        // Satchel 的选项不会自己落盘（Mod.SaveGlobalSettings 是 protected），改完手动存一次
        public void Persist() => SaveGlobalSettings();

        public MenuScreen GetMenuScreen(MenuScreen modListMenu, ModToggleDelegates? toggleDelegates)
        {
            _mainMenu ??= new BM.Menu(GetName(), MainElements());
            _mainScreen = BM.Blueprints.GetCachedMenuScreen(_mainMenu, modListMenu);
            _attackMenu ??= new BM.Menu("自定义招式", AttackElements());
            _attackScreen = BM.Blueprints.GetCachedMenuScreen(_attackMenu, _mainScreen);
            return _mainScreen;
        }

        private BM.Element[] MainElements()
        {
            List<BM.Element> list = new List<BM.Element>();

            list.Add(Bool("总开关", "总开关开启时，其他才有效", v => _set.on = v, () => _set.on, "on"));
            list.Add(Bool("巴德尔修复", "按下BackSpace键来修复巴德尔之壳", v => _set.baldurfix = v, () => _set.baldurfix, "baldurfix"));
            list.Add(Bool("激光锁头", "P3的激光不再有偏移角度", v => _set.beamlock = v, () => _set.beamlock, "beamlock"));
            list.Add(Bool("移除虚空", "P2虚空不再上升", v => _set.abyssremove = v, () => _set.abyssremove, "abyssremove"));
            list.Add(Bool("光球范围显示", "实时显示光球可能出现的范围", v => _set.orbindicator = v, () => _set.orbindicator, "orbindicator"));
            list.Add(Bool("重置无忧概率", "每次切换场景都会重置无忧概率", v => _set.carereset = v, () => _set.carereset, "carereset"));
            list.Add(Bool("两槽快劈", "any挑战中唯一允许的旧护符槽位", v => _set.legacycost = v, () => _set.legacycost, "legacycost"));
            list.Add(Bool("辐光皮肤", "皮肤尚无", v => _set.skin = v, () => _set.skin, "skin"));
            list.Add(Bool("无敌显示", "处于无敌状态时显示绿圈", v => _set.indicator = v, () => _set.indicator, "indicator"));

            list.Add(BM.Blueprints.NavigateToMenu("自定义招式", "辐光招式控制：锁单招 / 锁序列 / P2瞬移点位", () => _attackScreen));

            return list.ToArray();
        }

        private BM.Element[] AttackElements()
        {
            List<BM.Element> list = new List<BM.Element>();

            list.Add(Bool("启用", "", v => _set.crOn = v, () => _set.crOn, "cr_on"));

            list.Add(Opt("P1 配置", "",
                new string[] { "随机（原版）", "锁单招", "锁序列" },
                i => _set.crModeA1 = (ChoiceMode)i, () => (int)_set.crModeA1, "cr_mode_a1"));

            list.Add(Opt("P2 配置", "",
                new string[] { "随机（原版）", "锁单招", "锁序列" },
                i => _set.crModeA2 = (ChoiceMode)i, () => (int)_set.crModeA2, "cr_mode_a2"));

            string[] p1 = AttackCatalog.NamesFor(RadPhase.P1);
            string[] p2 = AttackCatalog.NamesFor(RadPhase.P2);

            list.Add(Opt("P1锁定招", "仅 P1 配置=锁单招时生效", p1,
                i => _set.crA1 = p1[i], () => AttackCatalog.IndexOf(p1, _set.crA1), "cr_a1"));

            list.Add(Opt("P2锁定招", "仅 P2 配置=锁单招时生效", p2,
                i => _set.crA2 = p2[i], () => AttackCatalog.IndexOf(p2, _set.crA2), "cr_a2"));

            list.Add(Opt("轮播序列", "",
                new string[] { "关闭（交还原版）", "开启（8 槽轮播）" },
                i => _set.crLoop = i == 1, () => _set.crLoop ? 1 : 0, "cr_loop"));

            string[] slotOptions = AttackCatalog.SlotOptions();
            for (int i = 0; i < AttackSequence.SlotCount; i++)
            {
                int slot = i;   // 闭包捕获：必须复制到局部变量
                list.Add(Opt("槽位 " + (slot + 1), "留空则跳到下个槽位", slotOptions,
                    v => _set.crSlots[slot] = slotOptions[v] == AttackCatalog.Empty ? null : slotOptions[v],
                    () => AttackCatalog.IndexOf(slotOptions, _set.crSlots[slot] ?? AttackCatalog.Empty),
                    "cr_slot" + slot));
            }

            list.Add(Bool("允许相同瞬移点", "",
                v => _set.crTeleRepeat = v, () => _set.crTeleRepeat, "cr_tele_repeat"));

            string[] teleOptions = TeleOptions();
            list.Add(Opt("指定 P2 瞬移点位", "",
                teleOptions,
                i => _set.crTelePos = i,
                () => _set.crTelePos < 0 || _set.crTelePos > 10 ? 0 : _set.crTelePos,
                "cr_tele_pos"));

            return list.ToArray();
        }

        private BM.HorizontalOption Bool(string name, string description, Action<bool> set, Func<bool> get, string id)
        {
            return BM.Blueprints.HorizontalBoolOption(name, description, v => { set(v); Persist(); }, get,
                Language.Language.Get("MOH_ON", "MainMenu"), Language.Language.Get("MOH_OFF", "MainMenu"), id);
        }

        private BM.HorizontalOption Opt(string name, string description, string[] values, Action<int> set, Func<int> get, string id)
        {
            return new BM.HorizontalOption(name, description, values, v => { set(v); Persist(); }, get, id);
        }

        private static string[] TeleOptions()
        {
            string[] opts = new string[11];
            opts[0] = "不锁（原版随机）";
            for (int i = 1; i <= 10; i++) opts[i] = "第 " + i + " 点";
            return opts;
        }

        void FindChild(GameObject child)
        {




            //利用for循环 获取物体下的全部子物体
            for (int c = 0; c < child.transform.childCount; c++)
            {
                //如果子物体下还有子物体 就将子物体传入进行回调查找 直到物体没有子物体为止
                if (child.transform.GetChild(c).childCount > 0)
                {
                    FindChild(child.transform.GetChild(c).gameObject);

                }
                CH.Add(child.transform.GetChild(c).gameObject);


            }
        }
    }
}