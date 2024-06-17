using System;
using System.Reflection.Emit;
using HarmonyLib;
using Microsoft.Xna.Framework;
using StardewValley.Monsters;
using StardewModdingAPI;

namespace PrismaticDinosaur.Patches
{
    internal class MonsterPatcher
    {
        private static IMonitor Monitor;

        internal static void Initialize(IMonitor monitor)
        {
            Monitor = monitor;
        }

        internal static void Apply(Harmony harmony)
        {
            harmony.Patch(
                original: AccessTools.Constructor(typeof(StardewValley.Monsters.Monster), new Type[] {typeof(string), typeof(Vector2)}),
                prefix: new HarmonyMethod(typeof(MonsterPatcher), nameof(MonsterConstructor_Prefix))
            );
        }

        internal static bool MonsterConstructor_Prefix(Monster __instance, ref string name, Vector2 position)
        {
            try
            {
                // 10% change to spawn a prismatic pepper rex instead of a regular pepper rex
                Random dinoRandom = new Random();
                ModEntry.ModMonitor.Log($"Constructor Name: {name}.", LogLevel.Debug);
                if (name == "Pepper Rex" && dinoRandom.NextDouble() < 0.1)
                {
                    name = "JollyLlama.PrismaticDinosaur.Prismatic Pepper Rex";
                }
                return true;   
            }
            catch (Exception ex)
            {
                Monitor.Log($"Failed in {nameof(MonsterConstructor_Prefix)}:\n{ex}", LogLevel.Error);
                return true;
            }
        }
    }
}