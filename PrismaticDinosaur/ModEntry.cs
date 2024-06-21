using HarmonyLib;
using StardewModdingAPI;
using PrismaticDinosaur.Patches;

namespace PrismaticDinosaur
{
    internal sealed class ModEntry : Mod
    {
        internal static IMonitor ModMonitor { get; private set; } = null!;

        internal static IModHelper ModHelper { get; private set; } = null!;
        internal static Harmony Harmony { get; private set; } = null!;
        public override void Entry(IModHelper helper)
        {
            ModMonitor = Monitor;
            ModHelper = helper;

            var harmony = new Harmony(this.ModManifest.UniqueID);
            MonsterPatcher.Apply(ModMonitor, harmony);
            DinoMonsterPatcher.Apply(ModMonitor, harmony);
        }
    }
}
