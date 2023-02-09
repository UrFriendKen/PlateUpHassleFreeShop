using Kitchen;
using Kitchen.Modules;
using KitchenLib;
using KitchenLib.Event;
using KitchenLib.Utils;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

// Namespace should have "Kitchen" in the beginning
namespace KitchenHassleFreeShop
{
    public class Main : BaseMod
    {
        // guid must be unique and is recommended to be in reverse domain name notation
        // mod name that is displayed to the player and listed in the mods menu
        // mod version must follow semver e.g. "1.2.3"
        public const string MOD_GUID = "IcedMilo.PlateUp.HassleFreeShop";
        public const string MOD_NAME = "Hassle Free Shop";
        public const string MOD_VERSION = "0.1.1";
        public const string MOD_AUTHOR = "IcedMilo";
        public const string MOD_GAMEVERSION = ">=1.1.1";
        // Game version this mod is designed for in semver
        // e.g. ">=1.1.1" current and all future
        // e.g. ">=1.1.1 <=1.2.3" for all from/until

        public const string ENABLED_PREFERENCE_ID = "Enabled";
        internal static bool Mod_Enabled = false;

        public Main() : base(MOD_GUID, MOD_NAME, MOD_AUTHOR, MOD_VERSION, MOD_GAMEVERSION, Assembly.GetExecutingAssembly()) { }

        protected override void Initialise()
        {
            base.Initialise();
            // For log file output so the official plateup support staff can identify if/which a mod is being used
            LogWarning($"{MOD_GUID} v{MOD_VERSION} in use!");
            RegisterPreferences();
            SetupKLPreferencesMenu();
        }

        private static void RegisterPreferences()
        {
            PreferenceUtils.Register<KitchenLib.IntPreference>(MOD_GUID, ENABLED_PREFERENCE_ID, "Enabled");
            PreferenceUtils.Get<KitchenLib.IntPreference>(MOD_GUID, ENABLED_PREFERENCE_ID).Value = 1;
            PreferenceUtils.Load();
            Mod_Enabled = PreferenceUtils.Get<KitchenLib.IntPreference>(MOD_GUID, ENABLED_PREFERENCE_ID).Value == 1;
        }

        private static void SetupKLPreferencesMenu()
        {
            Events.PreferenceMenu_PauseMenu_CreateSubmenusEvent += (s, args) => {
                args.Menus.Add(typeof(HassleFreeShopMenu<PauseMenuAction>), new HassleFreeShopMenu<PauseMenuAction>(args.Container, args.Module_list));
            };
            ModsPreferencesMenu<PauseMenuAction>.RegisterMenu(MOD_NAME, typeof(HassleFreeShopMenu<PauseMenuAction>), typeof(PauseMenuAction));
        }

        protected override void OnUpdate()
        {
            
        }

        #region Logging
        // You can remove this, I just prefer a more standardized logging
        public static void LogInfo(string _log) { Debug.Log($"[{MOD_NAME}] " + _log); }
        public static void LogWarning(string _log) { Debug.LogWarning($"[{MOD_NAME}] " + _log); }
        public static void LogError(string _log) { Debug.LogError($"[{MOD_NAME}] " + _log); }
        public static void LogInfo(object _log) { LogInfo(_log.ToString()); }
        public static void LogWarning(object _log) { LogWarning(_log.ToString()); }
        public static void LogError(object _log) { LogError(_log.ToString()); }
        #endregion
    }
    public class HassleFreeShopMenu<T> : KLMenu<T>
    {
        private static class PreferencesHelper
        {
            public static void Preference_OnChanged(string preferenceID, int f)
            {
                PreferenceUtils.Get<KitchenLib.IntPreference>(Main.MOD_GUID, preferenceID).Value = f;
                PreferenceUtils.Save();
            }
        }

        private Option<int> Enabled;

        public HassleFreeShopMenu(Transform container, ModuleList module_list) : base(container, module_list)
        {
        }

        public override void Setup(int player_id)
        {
            AddLabel("Blueprints and Parcels Auto Open");
            this.Enabled = new Option<int>(
                new List<int>() { 0, 1 },
                PreferenceUtils.Get<KitchenLib.IntPreference>(Main.MOD_GUID, Main.ENABLED_PREFERENCE_ID).Value,
                new List<string>() { "Disabled", "Enabled" });
            Add<int>(this.Enabled).OnChanged += delegate (object _, int f)
            {
                PreferencesHelper.Preference_OnChanged(Main.ENABLED_PREFERENCE_ID, f);
                Main.Mod_Enabled = (f == 1);

            };

            New<SpacerElement>();

            AddButton(base.Localisation["MENU_BACK_SETTINGS"], delegate
            {
                RequestPreviousMenu();
            });
        }
    }

}
