using System;
using Terraria.ModLoader;

namespace ElsiNohitMod
{
    public class ElsiNohitMod : Mod
    {
        public static bool CalamityLoaded = false;
        public static Mod Calamity = null;

        public static bool InfernumLoaded = false;
        public static Mod Infernum = null;

        public static Func<bool> InfernumActive = () => { if (InfernumLoaded) return (bool)Infernum.Call("GetInfernumActive"); return false; };

        public override void Load()
        {
            CalamityLoaded = ModLoader.TryGetMod("CalamityMod", out Calamity);
            InfernumLoaded = ModLoader.TryGetMod("InfernumMode", out Infernum);
        }
    }
}
