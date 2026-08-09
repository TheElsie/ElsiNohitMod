using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;



namespace ElsiNohitMod.Content
{
    public class Defiled : ModPlayer
    {



        public override void Load()
        {
            On_Player.GetWingStats += DefiledStuff;
            On_Player.UpdateEquips += TrueDefiled;
            On_Mount.ResetFlightTime += MountDefiled;
            On_Mount.CanHover += HoverDefiled;
        }



        // Removes wing flight
        private static WingStats DefiledStuff(On_Player.orig_GetWingStats orig, Player self, int wingID)
        {
            if (TheConfigForThisMod.Instance.Defiled == TheConfigForThisMod.DefiledEnum.Disabled)
            {
                return orig(self, wingID);
            }
            else
            {
                return new WingStats(0);
            }
        }

        // Removes rocket boot flight
        private static void TrueDefiled(On_Player.orig_UpdateEquips orig, Player self, int i)
        {
            orig(self, i);
            if (TheConfigForThisMod.Instance.Defiled == TheConfigForThisMod.DefiledEnum.TrueDefiled) { self.rocketBoots = 0; }
        }

        // Removes mount flight
        private static void MountDefiled(On_Mount.orig_ResetFlightTime orig, Mount self, float xvel)
        {
            if (TheConfigForThisMod.Instance.Defiled == TheConfigForThisMod.DefiledEnum.TrueDefiled)
            {
                self._flyTime = 0;
            }
            else
            {
                orig(self, xvel);
            }
        }

        // Removes mount hover
        private static bool HoverDefiled(On_Mount.orig_CanHover orig, Mount self)
        {
            if (TheConfigForThisMod.Instance.Defiled == TheConfigForThisMod.DefiledEnum.TrueDefiled)
            {
                return false;
            }
            else
            {
                return orig(self);
            }
        }
    }
}
