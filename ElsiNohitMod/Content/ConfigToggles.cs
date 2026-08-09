using System.Collections.Generic;
using System.Reflection;
using Microsoft.Xna.Framework;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.GameContent.Events;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;



namespace ElsiNohitMod.Content
{
	public class ConfigToggles : ModSystem
	{

        // Loads hooks
        public override void Load()
        {
            On_Main.HandleMeteorFall += IWillCatchYou;
            On_Main.ShouldNormalEventsBeAbleToStart += StopInvasionSpawning;
            On_Main.StartSlimeRain += StopSlimeRain;
            On_WorldGen.TriggerLunarApocalypse += CultistSucks;
            On_NPC.SpawnNPC += DisableNaturalSpawns;
            On_Player.DropTombstone += KeepTombstone;
            On_NPC.DropTombstoneTownNPC += YouAlsoKeepTombstone;
            On_Projectile.AI_148_StarSpawner += StopStarFall;
            On_NPC.DoDeathEvents_DropBossPotionsAndHearts += DontDropElsiGlobalNPC;
            On_NPC.NPCLoot += DontDropItems;
            On_NPC.NPCLoot_DropMoney += DontDropMoney;
            On_NPC.NPCLoot_DropHeals += DontDropExtraHearts;
            On_NPC.NPCLoot_DropCommonLifeAndMana += DontDropLifeAndMana;
        }



        // Disables meteors falling
        public static void IWillCatchYou(On_Main.orig_HandleMeteorFall orig)
        {
            if (!TheConfigForThisMod.Instance.DisableEvents)
            {
                orig();
            }
        }



        // Disables invasions (Goblin Invasions, Snow Legions, Pirate Invasions, and Martian Madnesses) and moon events (Blood, Eclipse, Pumpkin, Frost)
        // Also disables Slime Rain and Acid Rain
        public override void PreUpdateInvasions()
        {
            if (TheConfigForThisMod.Instance.DisableEvents)
            {
                if (Main.invasionType != 0)
                {
                    Main.invasionType = 0;
                }
                if (Main.bloodMoon)
                {
                    Main.bloodMoon = false;
                    Main.NewText("The Blood Moon fades prematurely...", new Color(50, 255, 130));
                }
                if (Main.eclipse)
                {
                    Main.eclipse = false;
                    Main.NewText("The moon left the oven on back at home...", new Color(50, 255, 130));
                }
                if (Main.pumpkinMoon)
                {
                    Main.pumpkinMoon = false;
                    Main.NewText("The Pumpkin Moon fades prematurely...", new Color(50, 255, 130));
                }
                if (Main.snowMoon)
                {
                    Main.snowMoon = false;
                    Main.NewText("The Frost Moon fades prematurely...", new Color(50, 255, 130));
                }
                if (Main.slimeRain)
                {
                    Main.StopSlimeRain();
                }

                if (ElsiNohitMod.CalamityLoaded)
                {
                    if ((bool)ElsiNohitMod.Calamity.Call("AcidRainActive"))
                    {
                        ElsiNohitMod.Calamity.Call("StopAcidRain");
                    }
                }
            }
        }

        // Disables weather events (Rain, Sandstorm, Wind, Thunderstorm)
        public override void PreUpdateWorld()
        {
            if (TheConfigForThisMod.Instance.DisableEvents)
            {
                if (Main.IsItRaining || Main.IsItStorming)
                {
                    Main.StopRain();
                    Main.cloudAlpha = 0f;
                }
                if (Main.WindyEnoughForKiteDrops)
                {
                    Main.windSpeedTarget = 0;
                    Main.windSpeedCurrent = 0;
                }
                if (Sandstorm.Happening)
                {
                    Sandstorm.StopSandstorm();
                }
            }
        }


        // Blocks natural Solar Eclipses, Pirate Invasions, Goblin Armies, and Blood Moons. Also blocks natural boss spawns, lol.
        private static bool StopInvasionSpawning(On_Main.orig_ShouldNormalEventsBeAbleToStart orig)
		{
			if (TheConfigForThisMod.Instance.DisableEvents) return true;
			return orig();
		}
        

        // Blocks natural Slime Rain (and, until 1.4.5, all Slime Rains)
        private static void StopSlimeRain(On_Main.orig_StartSlimeRain orig, bool announce)
        {
            if (!TheConfigForThisMod.Instance.DisableEvents) orig(announce);
        }


        // Blocks Lunar Events after killing Lunatic Cultist
        private static void CultistSucks(On_WorldGen.orig_TriggerLunarApocalypse orig)
        {
            if (!TheConfigForThisMod.Instance.DisableEvents) orig();
        }

        
        // Prevents NPC spawn logic from running
        private static void DisableNaturalSpawns(On_NPC.orig_SpawnNPC orig)
        {
            if (!TheConfigForThisMod.Instance.DisableSpawns) orig();
        }
        

        // Disables the tombstone spawning code
        private static void KeepTombstone(On_Player.orig_DropTombstone orig, Player self, long coins, NetworkText death, int direction)
        {
            if (!TheConfigForThisMod.Instance.DisableTombstones) orig(self, coins, death, direction);
        }


        // Disables town NPC tombstones on hardcore, lol
        private static void YouAlsoKeepTombstone(On_NPC.orig_DropTombstoneTownNPC orig, NPC self, NetworkText death)
        {
            if (!TheConfigForThisMod.Instance.DisableTombstones) orig(self, death);
        }


        // Prevents falling stars
        private static void StopStarFall(On_Projectile.orig_AI_148_StarSpawner orig, Projectile self)
        {
            if (!TheConfigForThisMod.Instance.DisableFallingStars) orig(self);
        }

        // Disables boss potions and heart drops
        private static void DontDropElsiGlobalNPC(On_NPC.orig_DoDeathEvents_DropBossPotionsAndHearts orig, NPC self, ref string name)
        {
            if (!TheConfigForThisMod.Instance.DisableBossDrops) orig(self, ref name);
        }

        // Disable item drops, albeit a little weirdly
        private static void DontDropItems(On_NPC.orig_NPCLoot orig, NPC self)
        {
            if ((TheConfigForThisMod.Instance.DisableItems && !self.boss) || (TheConfigForThisMod.Instance.DisableBossDrops && self.boss))
            {
                for (int i = 0; i < 10000; i++)
                {
                    NPCLoader.blockLoot.Add(i);
                }
            }
            orig(self);
        }


        // Disables coin drops
        private static void DontDropMoney(On_NPC.orig_NPCLoot_DropMoney orig, NPC self, Player player)
        {
            if ((!TheConfigForThisMod.Instance.DisableCoins && !self.boss) || (!TheConfigForThisMod.Instance.DisableBossDrops && self.boss)) orig(self, player);
        }

        // Prevents or allows hearts and stars to drop
        private static void DontDropExtraHearts(On_NPC.orig_NPCLoot_DropHeals orig, NPC self, Player player)
        {
            if (TheConfigForThisMod.Instance.DisableHearts)
            {
                if (!TheConfigForThisMod.Instance.DisableManaStars)
                {
                    if (!NPCID.Sets.NeverDropsResourcePickups[self.type] && player.RollLuck(6) == 0 && self.lifeMax > 1 && self.damage > 0)
                    {
                        if (Main.rand.Next(2) == 0 && player.statMana < player.statManaMax2)
                        {
                            Item.NewItem(self.GetSource_Loot(), (int)self.position.X, (int)self.position.Y, self.width, self.height, 184);
                        }
                    }
                }
            }
            else
            {
                orig(self, player);
            }
        }


        // Prevents or allows mana stars to drop if hearts are still enabled
        private static void DontDropLifeAndMana(On_NPC.orig_NPCLoot_DropCommonLifeAndMana orig, NPC self, Player player)
        {
            if (TheConfigForThisMod.Instance.DisableManaStars)
            {
                if (!NPCID.Sets.NeverDropsResourcePickups[self.type] && player.RollLuck(6) == 0 && self.lifeMax > 1 && self.damage > 0)
                {
                    if (Main.rand.Next(2) == 0 && player.statLife < player.statLifeMax2)
                    {
                        Item.NewItem(self.GetSource_Loot(), (int)self.position.X, (int)self.position.Y, self.width, self.height, 58);
                    }
                }
            }
            else
            {
                orig(self, player);
            }
        }

		// Blocks natural Martian Invasions
		public override void PreUpdateNPCs()
		{
			if (TheConfigForThisMod.Instance.DisableEvents)
			{
				foreach (NPC npc in Main.npc)
				{
					if (npc.type == NPCID.MartianProbe)
					{
						if (npc.ai[0] == 2f) if ((npc.position.Y + npc.velocity.Y < (float)(-npc.height) || npc.ai[1] + 1 >= 180f) && Main.netMode != NetmodeID.MultiplayerClient) npc.StrikeInstantKill();
					}
				}
			}
		}


        // Supposedly disables Torch God
        public override void PostUpdatePlayers()
        {
            if (TheConfigForThisMod.Instance.DisableEvents)
            {
                foreach (Player player in Main.player) player.happyFunTorchTime = false;
            }
        }
	}

    // Failed IL edit
    /*
    public class ILTest : ModSystem
    {
        public override void Load()
        {
            //IL_WorldGen.Check3x3 += NoLarvaSpawns;
        }

        // Broken IL edit designed to disable QB spawns. Well, I say broken- the edit itself did work, but I couldn't figure out how to feed the config option in.
        private void NoLarvaSpawns(ILContext il)
        {
            try
            {
                var c = new ILCursor(il);
                for (int i = 0; i < 3; i++) { c.GotoNext(i => i.MatchLdcR4(1)); }
                c.GotoNext(MoveType.Before, i => i.MatchPop());

                var label = il.DefineLabel();
                c.Emit(Pop);
                c.Emit(Ldc_I4_1);
                //c.EmitDelegate(() => TheConfigForThisMod.Instance.DisableSpawns);
                c.Emit(Brtrue, label);

                for (int i = 0; i < 3; i++) { c.GotoNext(i => i.MatchLdloc(19)); }
                c.GotoNext(MoveType.Before, i => i.MatchLdcI4(0));
                c.MarkLabel(label);
            }
            catch (Exception e) { MonoModHooks.DumpIL(ModContent.GetInstance<ElsiNohitMod>(), il); }
        }
    }*/
}

