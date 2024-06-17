using System;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using StardewValley;
using StardewValley.Monsters;
using StardewValley.Extensions;
using StardewModdingAPI;

namespace PrismaticDinosaur.Patches
{
    internal class DinoMonsterPatcher
    {
        private static IMonitor Monitor;

        internal static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        internal static void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(StardewValley.Monsters.DinoMonster), nameof(StardewValley.Monsters.DinoMonster.getExtraDropItems)),
                prefix: new HarmonyMethod(typeof(DinoMonsterPatcher), nameof(getExtraDropItems_Prefix))
            );
            harmony.Patch(
                original: AccessTools.DeclaredMethod(typeof(StardewValley.Monsters.DinoMonster), nameof(StardewValley.Monsters.DinoMonster.draw)),
                transpiler: new HarmonyMethod(typeof(DinoMonsterPatcher), nameof(DinoMonsterPatcher.draw_Transpiler))
            );
        }

        internal static bool getExtraDropItems_Prefix(DinoMonster __instance, ref List<Item> __result)
        {
            try
            {
                if (__instance.Name == "JollyLlama.PrismaticDinosaur.Prismatic Pepper Rex")
                {
                    List<Item> extra_items = new List<Item>();
                    if (Game1.random.NextDouble() < 0.10000000149011612)
                    {
                        extra_items.Add(ItemRegistry.Create("(O)JollyLlama.PrismaticDinosaur.PrismaticDinosaurEgg"));
                    }
                    else
                    {
                        List<Item> non_egg_items = new List<Item>();
                        non_egg_items.Add(ItemRegistry.Create("(O)580"));
                        non_egg_items.Add(ItemRegistry.Create("(O)583"));
                        non_egg_items.Add(ItemRegistry.Create("(O)584"));
                        extra_items.Add(Game1.random.ChooseFrom(non_egg_items));
                    }
                    ModEntry.ModMonitor.Log($"Instance Name: {__instance.Name}, {extra_items[0].Name}.", LogLevel.Debug);
                    __result = extra_items;
                    return false;
                }  
                return true;
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(getExtraDropItems_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }

        internal static IEnumerable<CodeInstruction> draw_Transpiler(IEnumerable<CodeInstruction> instructions, ILGenerator il)
        {
            var code = instructions.ToList();
            try
            {
                var matcher = new CodeMatcher(code, il);
                matcher.MatchStartForward(
                    new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Color), "get_White"))
                ).ThrowIfNotMatch("Could not find proper entry point for draw_Transpiler in DinoMonster");
                
                matcher.InsertAndAdvance(
                    new CodeInstruction(OpCodes.Ldarg_0)
                );
                matcher.Set(OpCodes.Call, AccessTools.Method(typeof(DinoMonsterPatcher), "GetPrismaticColorforPrismaticDinoMonster"));
                return matcher.InstructionEnumeration();
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(draw_Transpiler)}:\n{ex}", LogLevel.Error);
                return code;
            }
        }

        internal static Color GetPrismaticColorforPrismaticDinoMonster(DinoMonster __instance)
        {
            if (__instance.Name == "JollyLlama.PrismaticDinosaur.Prismatic Pepper Rex") return Utility.GetPrismaticColor();
            return Color.White;
        }
    }
}