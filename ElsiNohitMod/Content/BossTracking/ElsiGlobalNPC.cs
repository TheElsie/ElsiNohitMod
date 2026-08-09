using System.Collections.Generic;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;



namespace ElsiNohitMod.Content.BossTracking
{
    // For tracking whether or not an npc originates from a boss and prevents boss aftermath
    public class ElsiGlobalNPC : GlobalNPC
    {
        public static int[] bossOwner = new int[201];


        public static int BoCmaxHP = 0;
        public static List<int> FuckBoCFuckYou = new List<int>();

        // Inheriting parent identity and initializing bosses
        public override void OnSpawn(NPC npc, IEntitySource source)
        {
            if (BossSets.Blacklist(npc.type))
            {
                return;
            }

            // Finds the index of the npc who spawned this npc, if there was one
            bossOwner[npc.whoAmI] = -1;
            if (source is EntitySource_Parent { Entity : NPC parent })
            {
                // this function exists because calamity boc spawns creepers prior to the boss setup logic running,
                // meaning that they add all their info to index 0 and then it gets wiped. at that stage, parent.whoAmI returns null.
                if (npc.type == NPCID.Creeper)
                {
                    BoCmaxHP += npc.lifeMax;
                    FuckBoCFuckYou.Add(npc.whoAmI);
                }
                else
                {
                    int parentOwner = bossOwner[parent.whoAmI];
                    // These three exceptions don't count as bosses, although sg counts as one in basecal, just not infernum (in slime phase that is)
                    if (parentOwner != -1 && (Main.npc[parentOwner].boss || Main.npc[parentOwner].type == NPCID.EaterofWorldsHead || Main.npc[parentOwner].type == CalamityID.SlimeGod || Main.npc[parentOwner].type == CalamityID.AquaticScourgeHead))
                    {
                        bossOwner[npc.whoAmI] = parentOwner;
                        if (BossSets.Phase(npc.type))
                        {
                            foreach (BossSystem.BossInfo boss in BossSystem.ActiveBosses)
                            {
                                if (parentOwner == boss.index)
                                {
                                    if (Main.npc[parentOwner].type == NPCID.EaterofWorldsHead || (boss.type == CalamityID.SlimeGod && !ElsiNohitMod.InfernumActive()) || npc.type == CalamityID.GuardianHealer || npc.type == CalamityID.GuardianDefender)
                                    {
                                        boss.maxHP += npc.lifeMax;
                                    }
                                    boss.maxHP2 += npc.lifeMax;
                                    boss.segmentIndices.Add(npc.whoAmI);
                                }
                            }
                        }
                    }
                }
            }

            // For NPCs spawned from projectiles such as Leviathan
            if (source is EntitySource_Parent { Entity: Projectile parentProj })
            {
                bossOwner[npc.whoAmI] = ElsiGlobalProj.projOwner[parentProj.whoAmI];
            }

            // Desert Scourge uses SpawnOnPlayer instead of a normal method, so it never gets saved as the source. How tedious.
            if (npc.type == CalamityID.DesertNuisanceHead || npc.type == CalamityID.DesertNuisanceHeadYoung)
            {
                foreach (BossSystem.BossInfo boss in BossSystem.ActiveBosses)
                {
                    if (boss.type == CalamityID.DesertScourgeHead)
                    {
                        if (boss.bossNPC.localAI[2] < 3f)
                        {
                            boss.bossNPC.localAI[2]++;
                            bossOwner[npc.whoAmI] = bossOwner[boss.index];
                            boss.maxHP2 += npc.lifeMax;
                            boss.segmentIndices.Add(npc.whoAmI);
                            return;
                        }
                    }
                }
            }
            // Brothers also use a different spawning method, though this one is a special cal function.
            else if (npc.type == CalamityID.SupremeCataclysm || npc.type == CalamityID.SupremeCatastrophe)
            {
                foreach (BossSystem.BossInfo boss in BossSystem.ActiveBosses)
                {
                    if (boss.type == CalamityID.SupremeCalamitas)
                    {
                        if (ElsiNohitMod.InfernumActive())
                        {
                            if (boss.misc < 1f)
                            {
                                boss.misc++;
                                bossOwner[npc.whoAmI] = bossOwner[boss.index];
                                boss.maxHP2 += npc.lifeMax;
                                boss.segmentIndices.Add(npc.whoAmI);
                                return;
                            }
                        }
                        else if (boss.misc < 3f)
                        {   
                            boss.misc++;
                            bossOwner[npc.whoAmI] = bossOwner[boss.index];
                            boss.maxHP2 += npc.lifeMax;
                            boss.segmentIndices.Add(npc.whoAmI);
                            return;
                        }
                    }
                }
            }

            // Initialize bosses and eow/as
            if ((npc.boss || npc.type == NPCID.EaterofWorldsHead || npc.type == CalamityID.AquaticScourgeHead) && !BossSets.NotABoss(npc.type))
            {
                // If Calamity Death mode is enabled, each separate worm will not be treated individually
                if (npc.type == NPCID.EaterofWorldsHead && bossOwner[npc.whoAmI] != -1)
                {
                    return;
                }
                bossOwner[npc.whoAmI] = npc.whoAmI;
            }
        }

        // No aftermath, except during br otherwise the event won't progress
        public override bool PreKill(NPC npc)
        {
            if (TheConfigForThisMod.Instance.DisableAftermath && npc.boss && !BossSystem.BossRushActive())
            {
                return false;
            }
            return base.PreKill(npc);
        }
    }
}
