using System;
using System.Collections.Generic;
using System.Linq;
using InfernumMode;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.Map;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;



namespace ElsiNohitMod.Content.BossTracking
{
    public class BossSystem : ModSystem
    {
        public static List<BossInfo> ActiveBosses = new List<BossInfo>();

        public static bool BossAlive => ActiveBosses.Count > 0;

        public static Func<bool> CountsAsBoss = () => { foreach (BossInfo boss in ActiveBosses) { if (!boss.dontCountAsNotBoss) return false; } return true; };

        public static Func<bool> PlayerAlive = () => { foreach (Player player in Main.ActivePlayers) { if (!player.dead) return true; } return false; };

        public static bool WasBRActive = false;

        public static Func<bool> BossRushActive = () => { if (ElsiNohitMod.CalamityLoaded) { if ((bool)ElsiNohitMod.Calamity.Call("GetDifficultyActive", "BossRush")) return true; } return false; };

        public static int dangerous = 0;

        public static int igt;

        public static DateTime rta;

        public static DateTime pausedStart;

        public static bool wasPaused = false;

        public static int totalHits = 0;

        public static int totalDamage = 0;

        public static int totalDodges = 0;

        public static int totalDamageDodged = 0;

        public static bool shown = true;

        public class BossInfo
        {
            public BossInfo(int index, int type, string name, int lifemax, bool bossRush = false)
            {
                this.index = index;
                this.type = type;
                this.name = name;
                maxHP = lifemax;
                maxHP2 = lifemax;
                hitsDealt = 0;
                damageTaken = 0;
                bossRTAStart = DateTime.UtcNow;
                despawned = false;
                dead = false;
                present = true;
                this.bossRush = bossRush;
            }

            public int misc = 0;

            public int index = -1;

            public NPC bossNPC => Main.npc[index];

            public int type;

            public int life = 0;

            public int life2 = 0;

            public int maxHP;

            public int maxHP2;

            public List<int> segmentIndices = new List<int>();

            public int[] EoWList = Array.Empty<int>();

            public int hitsDealt;

            public int damageTaken;

            public int bossIGT = -1;

            public DateTime bossRTAStart;

            public DateTime bossRTAEnd = new DateTime();

            public string name;

            public bool dontCountAsNotBoss = false;

            public bool fuck = false;

            public bool despawned;

            public bool dead;

            public bool deadButNot = false;

            public bool present;

            public bool bossRush;
        }

        // Hooks
        public override void Load()
        {
            On_NPC.NewNPC += BossSetup;
            On_NPC.checkDead += BossDeath;
            On_NPC.CheckActive += BossDespawn;
            On_NPC.NPCLoot += CheckDeathAnim;
        }

        // Initializes boss info on spawn + extra bosses
        private static int BossSetup(On_NPC.orig_NewNPC orig, IEntitySource source, int x, int y, int type, int start, float ai0, float ai1, float ai2, float ai3, int target)
        {
            int returnVal = orig(source, x, y, type, start, ai0, ai1, ai2, ai3, target);
            NPC npc = Main.npc[returnVal];
            // If you're a boss and not blacklisted, not a segment of a whole, EoW (and not a clone from Cal Death mode), or AS, proceed
            if ((npc.boss && !BossSets.NotABoss(npc.type) && (npc.realLife == -1 || npc.realLife == npc.whoAmI)) || (npc.type == NPCID.EaterofWorldsHead && ElsiGlobalNPC.bossOwner[npc.whoAmI] == npc.whoAmI) || npc.type == CalamityID.AquaticScourgeHead)
            {
                shown = false;
                ActiveBosses.Add(new BossInfo(returnVal, npc.type, npc.TypeName, npc.lifeMax));
                int i = ActiveBosses.Count - 1;
                // I HATE YOU
                if (npc.type == NPCID.EaterofWorldsHead)
                {
                    ActiveBosses[i].segmentIndices.Add(npc.whoAmI);
                }
                // Thanks, OnSpawn hook
                if (npc.type == NPCID.BrainofCthulhu)
                {
                    ActiveBosses[i].maxHP2 += ElsiGlobalNPC.BoCmaxHP;
                    ActiveBosses[i].segmentIndices = ElsiGlobalNPC.FuckBoCFuckYou;
                    ElsiGlobalNPC.BoCmaxHP = 0;
                    ElsiGlobalNPC.FuckBoCFuckYou = new List<int>();
                }
                // Core shouldn't count towards the healthbar, unless we're on Infernum. I have no idea what "fuck" does anymore. I think it's to prevent it from counting as instantly despawned.
                if (npc.type == CalamityID.SlimeGod)
                {
                    if (ElsiNohitMod.InfernumActive())
                    {
                        ActiveBosses[i].fuck = true;
                    }
                    else
                    {
                        ActiveBosses[i].maxHP = 0;
                        ActiveBosses[i].maxHP2 = 0;
                    }
                }
                // AS shouldn't count towards the boss timer until it's hostile
                if (npc.type == CalamityID.AquaticScourgeHead && !ElsiNohitMod.InfernumActive())
                {
                    ActiveBosses[i].dontCountAsNotBoss = true;
                    ActiveBosses[i].bossRTAStart = new DateTime();
                }
                // Argus shouldn't count towards the boss timer until it's hostile
                if (ElsiNohitMod.InfernumActive())
                {
                    if (npc.type == CalamityID.Argus || npc.type == CalamityID.Signus)
                    {
                        ActiveBosses[i].dontCountAsNotBoss = true;
                        ActiveBosses[i].bossRTAStart = new DateTime();
                    }
                }

                if (TheConfigForThisMod.Instance.ExtraBosses > 0 && dangerous != npc.type)
                {
                    dangerous = npc.type;
                    for (i = 0; i < TheConfigForThisMod.Instance.ExtraBosses; i++)
                    {
                        NPC.NewNPC(source, x + Main.rand.Next(-100, 100), y + Main.rand.Next(-100, 100), type, start, ai0, ai1, ai2, ai3, target);
                    }
                }
            }
            return returnVal;
        }

        // Controls boss death stuff
        private static void BossDeath(On_NPC.orig_checkDead orig, NPC self)
        {
            if (self.active && self.life <= 0)
            {
                // Normal bosses
                if (self.boss && !BossSets.NotABoss(self.type))
                {
                    foreach (BossInfo boss in ActiveBosses)
                    {
                        if (boss.type != NPCID.EaterofWorldsHead && boss.index == self.whoAmI)
                        {
                            boss.bossRTAEnd = DateTime.UtcNow;
                            boss.dead = true;
                            boss.present = false;

                            orig(self);
                            if (self.life > 0)
                            {
                                boss.deadButNot = true;
                            }
                            return;
                        }
                    }
                }

                // For segments dying
                if (ElsiGlobalNPC.bossOwner[self.whoAmI] != -1)
                {
                    foreach (BossInfo boss in ActiveBosses)
                    {
                        if (boss.type != NPCID.EaterofWorldsHead && boss.index == ElsiGlobalNPC.bossOwner[self.whoAmI])
                        {
                            boss.segmentIndices.Remove(self.whoAmI);
                            break;
                        }
                    }
                }

                // You guessed it! Death
                if (self.type == NPCID.EaterofWorldsHead || self.type == NPCID.EaterofWorldsBody || self.type == NPCID.EaterofWorldsTail)
                {
                    foreach (BossInfo boss in ActiveBosses)
                    {
                        if (boss.index == ElsiGlobalNPC.bossOwner[self.whoAmI])
                        {
                            boss.segmentIndices.Remove(self.whoAmI);
                            if (boss.segmentIndices.Count == 0)
                            {
                                boss.bossRTAEnd = DateTime.UtcNow;
                                boss.dead = true;
                                boss.present = false;
                            }
                            break;
                        }
                    }
                }
            }
            orig(self);
        }

        // This is so that custom death animations/desperations don't make the boss end on 0.01% hp.
        // I hope this works. I cannot check every boss with it, only the ones with post-death stuff
        private static void CheckDeathAnim(On_NPC.orig_NPCLoot orig, NPC self)
        {
            foreach (BossInfo boss in ActiveBosses)
            {
                if (boss.index == self.whoAmI)
                {
                    boss.dead = true;
                    boss.deadButNot = false;
                }
            }
            orig(self);
        }

        // Supposedly tracks boss despawning. Doesn't do very well. It's better at other despawning
        private static void BossDespawn(On_NPC.orig_CheckActive orig, NPC self)
        {
            orig(self);
            if (!self.active && self.life > 0)
            {
                bool despawn = false;
                bool isImportant = false;
                foreach (Player player in Main.ActivePlayers) { if (!player.dead) { despawn = true; } }
                foreach (BossInfo boss in ActiveBosses)
                {
                    if (self.boss && !BossSets.NotABoss(self.type))
                    {
                        isImportant = true;
                        if (boss.index == self.whoAmI)
                        {
                            // Slime God shouldn't count as despawned on death unless one of the Paladins despawned
                            float aiCheck = -1;
                            if (ElsiNohitMod.CalamityLoaded)
                            {
                                aiCheck = ((float[])ElsiNohitMod.Calamity.Call("GetCalamityAI", self))[3];
                            }
                            // Slime God shouldn't count as despawned unless one of the Paladins did, though this is largely unnecessary. SCal and Providence should never despawn.
                            if ((!boss.dead && boss.type != CalamityID.SlimeGod) || (boss.type == CalamityID.SlimeGod && aiCheck == 1f))
                            {
                                if (despawn) { boss.despawned = true; }
                                break;
                            }
                            else
                            {
                                despawn = false;
                            }
                        }
                    }
                    else if (boss.segmentIndices.Contains(self.whoAmI))
                    {
                        if (self.type != CalamityID.SoulSeekerSupreme || !ElsiNohitMod.InfernumActive() || !self.dontTakeDamage)
                        {
                            isImportant = true;
                            if (despawn) { boss.despawned = true; }
                        }
                        // I have no fucking idea why this is necessary, but it is. Thanks Infernum
                        if (self.type != CalamityID.SepulcherHead)
                        {
                            boss.segmentIndices.Remove(self.whoAmI);
                        }
                        break;
                    }
                }

                // If it's a piece of a boss and BR isn't active (br has weird logic that can cause fake despawns)
                if (isImportant && despawn && !BossRushActive())
                {
                    Main.NewText($"{self.FullName} has despawned!", TheConfigForThisMod.Instance.FightStatistics.WarningColor);
                }
            }
        }

        // Tracks post-death despawns (either forced or as a failsafe) and boss timers + health
        // Also for ending fights early and tracking other midfight boss stuff
        public override void PostUpdateNPCs()
        {
            int check = 0;
            bool isbr = false;
            foreach (BossInfo boss in ActiveBosses.ToArray())
            {
                // Don't show stats until BR finishes
                if (boss.bossRush)
                {
                    isbr = true;
                    if (BossRushActive())
                    {
                        check++;
                    }
                }
                else
                {
                    // I HATE YOU
                    if (boss.type == NPCID.EaterofWorldsHead)
                    {
                        foreach (int i in boss.segmentIndices.ToList())
                        {
                            if (!Main.npc[i].active)
                            {
                                boss.segmentIndices.Remove(i);
                                boss.EoWList = boss.EoWList.Append(Main.npc[i].life).ToArray();
                            }
                        }
                        if (boss.segmentIndices.Count == 0)
                        {
                            if (boss.present)
                            {
                                boss.bossRTAEnd = DateTime.UtcNow;
                                boss.present = false;
                            }
                        }
                        else
                        {
                            boss.life = 0;
                            foreach (int i in boss.segmentIndices)
                            {
                                boss.life += Main.npc[i].life;
                            }
                            foreach (int i in boss.EoWList)
                            {
                                boss.life += i;
                            }
                            boss.life2 = boss.life;
                            if (boss.bossRTAEnd == new DateTime()) boss.bossIGT++;
                            check++;
                        }
                    }
                    // For AS and Argus and inf Signus
                    else if (boss.dontCountAsNotBoss)
                    {
                        // When AS becomes hostile, it's marked as a boss.
                        // Argus attack state is stored in NPC.ai[0]. It doesn't actually change until the spawn animation ends (which we want to include in the timer),
                        // so we just add the length of the animation in frames. Cringe, but I'm not using reflection to get the actual value.
                        if ((boss.type == CalamityID.AquaticScourgeHead || (boss.type == CalamityID.Signus && ElsiNohitMod.InfernumActive()) && boss.bossNPC.boss) || (boss.type == CalamityID.Argus && boss.bossNPC.ai[0] != 0))
                        {
                            boss.bossRTAStart = DateTime.UtcNow;
                            if (boss.type == CalamityID.Argus)
                            {
                                boss.bossRTAStart -= new TimeSpan(10000000 * 140 / 60);
                                boss.bossIGT += 140;

                                if (igt == 0)
                                {
                                    rta -= new TimeSpan(10000000 * 140 / 60);
                                    igt += 140;
                                }
                            }
                            boss.dontCountAsNotBoss = false;
                            shown = false;
                            check++;
                        }
                        if (!boss.bossNPC.active)
                        {
                            ActiveBosses.Remove(boss);
                        }
                    }
                    // Despawns, forced or otherwise.
                    // In Infernum, Slime God doesn't count as a boss until core phase, so we need to prevent it from instantly ending the fight.
                    else if (!boss.bossNPC.active || (!boss.bossNPC.boss && !boss.fuck))
                    {
                        if (boss.present)
                        {
                            boss.bossRTAEnd = DateTime.UtcNow;
                            boss.present = false;

                            // Make sure that all Paladins are inactive; this can only happen if it despawns from distance
                            bool despawn = false;
                            if (boss.type == CalamityID.SlimeGod)
                            {
                                float aiCheck = -1;
                                if (ElsiNohitMod.CalamityLoaded)
                                {
                                    aiCheck = ((float[])ElsiNohitMod.Calamity.Call("GetCalamityAI", boss.bossNPC))[3];
                                }
                                if (boss.segmentIndices.Count == 0 && aiCheck != 1f)
                                {
                                    return;
                                }
                            }

                            foreach (Player player in Main.ActivePlayers)
                            {
                                if (!player.dead) { despawn = true; }
                            }

                            // Prevent P1 Deus from counting as a despawn
                            if (boss.type == CalamityID.AstrumDeusHead)
                            {
                                float aiCheck = -1;
                                if (ElsiNohitMod.CalamityLoaded)
                                {
                                    aiCheck = ((float[])ElsiNohitMod.Calamity.Call("GetCalamityAI", boss.bossNPC))[0];
                                }
                                if (aiCheck == 0f && despawn)
                                {
                                    ActiveBosses.Remove(boss);
                                    return;
                                }
                            }
                            if (despawn) boss.despawned = true;
                        }
                    }
                    // Tracking health, including segments, and timers
                    else
                    {

                        // Slime God Core shouldn't count towards healthbar, as it is unkillable
                        if (boss.type == CalamityID.SlimeGod && !ElsiNohitMod.InfernumActive())
                        {
                            boss.life = 0;
                            boss.life2 = 0;
                        }
                        else
                        {
                            boss.life = boss.bossNPC.life;
                            boss.life2 = boss.bossNPC.life;
                        }

                        // Make Inf Wyrm fight end once terminus appears
                        if (boss.type == CalamityID.PrimordialWyrm && ElsiNohitMod.InfernumActive())
                        {
                            if (AEW(boss.bossNPC))
                            {
                                boss.bossRTAEnd = DateTime.UtcNow;
                                boss.dead = true;
                                boss.deadButNot = false;
                                boss.present = false;
                                continue;
                            }
                        }

                        // Make the segments no longer count once they're all dead
                        if (boss.segmentIndices.Count == 0)
                        {
                            boss.maxHP2 = boss.maxHP;
                        }
                        else
                        {
                            foreach (int i in boss.segmentIndices.ToArray())
                            {
                                if (Main.npc[i].active)
                                {
                                    // Inf CV has some weird bullshit with the Dark Energy at the start
                                    bool yeag = false;
                                    if (ElsiNohitMod.InfernumActive())
                                    {
                                        yeag = CVDE(Main.npc[i]);
                                    }
                                    if (boss.type == CalamityID.CeaselessVoid && yeag)
                                    {
                                        boss.segmentIndices.Remove(i);
                                        continue;
                                    }
                                    if (Main.npc[i].type == CalamityID.GuardianHealer || Main.npc[i].type == CalamityID.GuardianDefender)
                                    {
                                        boss.life += Main.npc[i].life;
                                    }
                                    boss.life2 += Main.npc[i].life;
                                }
                                else
                                {
                                    boss.segmentIndices.Remove(i);
                                }
                            }
                        }

                        // For keeping stats consistent
                        if (boss.type == CalamityID.SlimeGod && !ElsiNohitMod.InfernumActive())
                        {
                            boss.life = boss.life2;
                        }

                        if (boss.bossRTAEnd == new DateTime()) boss.bossIGT++;
                        check++;
                    }
                    if (!PlayerAlive() && TheConfigForThisMod.Instance.DespawnSetting != TheConfigForThisMod.DespawnEnum.Disabled)
                    {
                        if (boss.bossRTAEnd == new DateTime()) boss.bossRTAEnd = DateTime.UtcNow;
                        check = 0;
                    }
                }

                if (!Main.gamePaused && wasPaused)
                {
                    boss.bossIGT++;
                    boss.bossRTAStart += DateTime.UtcNow.Subtract(pausedStart);
                }
            }

            // Has to go here otherwise changing config doesn't update time
            if (wasPaused)
            {
                igt++;
                rta += DateTime.UtcNow.Subtract(pausedStart);
            }
            if (!shown && check == 0) ShowFightStats(isbr);
        }

        // This is probably not the best function to do this in, but I don't really care.
        public override void UpdateUI(GameTime gameTime)
        {
            // Account for time while paused
            if (Main.gamePaused && !wasPaused)
            {
                pausedStart = DateTime.UtcNow;
            }
            wasPaused = Main.gamePaused;
        }

        // Timers and resetting after the fight
        public override void PostUpdateEverything()
        {
            if (!WasBRActive && BossRushActive())
            {
                ActiveBosses.Add(new BossInfo(-1, -1, "Boss Rush", -1, true));
            }
            WasBRActive = BossRushActive();
            if (BossAlive && !CountsAsBoss())
            {
                if (!PlayerAlive() && TheConfigForThisMod.Instance.DespawnSetting != TheConfigForThisMod.DespawnEnum.Disabled)
                {
                    return;
                }
                igt++;
            } else
            {
                igt = 0;
                rta = DateTime.UtcNow;
                totalHits = 0;
                totalDamage = 0;
                totalDodges = 0;
                totalDamageDodged = 0;
            }
        }

        // Post-fight stat screen
        public static void ShowFightStats(bool br = false)
        {
            // Important post-fight cleanup
            dangerous = 0;
            shown = true;
            List<BossInfo> bosses = new List<BossInfo>();
            int last = ActiveBosses[ActiveBosses.Count - 1].type;
            foreach (BossInfo boss in ActiveBosses.ToArray())
            {
                if (!boss.dontCountAsNotBoss)
                {
                    // br is marked as inactive the frame before this runs so if you win it doesnt work right
                    if ((!BossRushActive() && !br) || boss.bossRush || (boss.type == last && CalamityConfig.Instance.BRKiller && BossRushActive())) { bosses.Add(boss); }
                    ActiveBosses.Remove(boss);
                }
            }

            // Prevents aquatic scourge from bricking everything lol
            if (bosses.Count != 0)
            {
                int index = 0;
                string nameString = "";

                // OKAY. SO. THESE ARE BASICALLY PLACEHOLDERS
                // if there are multiple copies of the twins or whatever it will not track them separately
                // additionally, chat commands do not function with multiboss fights
                // though attempts there are still tracked normally
                List<int> IDList = new();
                foreach (BossInfo boss in bosses) { IDList.Add(boss.type); }
                if (IDList.Contains(NPCID.Retinazer) || IDList.Contains(NPCID.Spazmatism))
                {
                    nameString = "The Twins";
                }
                else if (IDList.Contains(CalamityID.Anahita) || IDList.Contains(CalamityID.Leviathan))
                {
                    nameString = "Leviathan and Anahita";
                }
                else if (IDList.Contains(CalamityID.Ares) || IDList.Contains(CalamityID.Artemis) || IDList.Contains(CalamityID.Apollo) || IDList.Contains(CalamityID.Thanatos))
                {
                    nameString = "The Exo Mechs";
                }
                else if (IDList.Contains(-1))
                {
                    nameString = "Boss Rush";
                }
                else
                {
                    List<string> namesAlphabetically = new List<string>();
                    foreach (BossInfo boss in bosses) { namesAlphabetically.Add(boss.name); }
                    namesAlphabetically.Sort();
                    foreach (string name in namesAlphabetically) { nameString += name; }
                }
                if (BossNames.Contains(nameString))
                {
                    index = BossNames.IndexOf(nameString);
                    TotalAttempts[index]++;
                    CurrentAttempts[index]++;
                    // This is to make sure there aren't any indexoutofrange exceptions for anyone who used the mod before the list NewSession was added.
                    while (NewSession.Count < index + 1)
                    {
                        NewSession.Add(false);
                    }
                    // Check if the boss was dead last fight or if the boss is currently dead
                    if (TheConfigForThisMod.Instance.NewSession)
                    {
                        if (NewSession[index])
                        {
                            CurrentAttempts[index] = 1;
                        }
                            bool won = true;
                            foreach (BossInfo boss in bosses)
                            {
                                if (!boss.dead)
                                {
                                    won = false;
                                }
                            }
                        if (won)
                        {
                            NewSession[index] = true;
                        }
                        else
                        {
                            NewSession[index] = false;
                        }
                    }
                }
                else
                {
                    BossNames.Add(nameString);
                    TotalAttempts.Add(1);
                    CurrentAttempts.Add(1);
                    NewSession.Add(false);
                    index = BossNames.Count - 1;
                }

                //Show stats
                if (TheConfigForThisMod.Instance.ShowFightStats)
                {
                    Color color = TheConfigForThisMod.Instance.FightStatistics.StatsColor;
                    Color warnColor = TheConfigForThisMod.Instance.FightStatistics.WarningColor;
                    bool[] settings =
                    {
                    TheConfigForThisMod.Instance.FightStatistics.ShowAttempts != TheConfigForThisMod.AttemptEnum.False,
                    TheConfigForThisMod.Instance.FightStatistics.ShowTime,
                    TheConfigForThisMod.Instance.FightStatistics.ShowSlowdown,
                    TheConfigForThisMod.Instance.FightStatistics.ShowHealth,
                    false, //TheConfigForThisMod.Instance.FightStatistics.ShowDPS,
                    TheConfigForThisMod.Instance.FightStatistics.ShowHits,
                    TheConfigForThisMod.Instance.FightStatistics.Specifics,
                    TheConfigForThisMod.Instance.FightStatistics.CombineStats,
                    TheConfigForThisMod.Instance.FightStatistics.DistinguishSegments
                };

                    // Boss title. very badly executed but whatever
                    if (bosses.Count == 1)
                    {
                        if (ElsiNohitMod.CalamityLoaded && bosses[0].type == CalamityID.GuardianCommander)
                        {
                            Main.NewText($"-|- The Profaned Guardians -|-", color);
                        }
                        else
                        {
                            Main.NewText($"-|- {bosses[0].name} -|-", color);
                        }
                    }
                    else
                    {
                        if (bosses.Count == 2 && (bosses[0].type == NPCID.Retinazer || bosses[0].type == NPCID.Spazmatism))
                        {
                            Main.NewText("-|- The Twins -|-", color);
                        }
                        else if (bosses.Count == 4 && (bosses[0].type == NPCID.MoonLordCore || bosses[0].type == NPCID.MoonLordHand || bosses[0].type == NPCID.MoonLordHead))
                        {
                            Main.NewText("-|- The Moon Lord -|-", color);
                        }
                        else if (ElsiNohitMod.CalamityLoaded)
                        {
                            if (bosses.Count == 2 && (bosses[0].type == CalamityID.Leviathan || bosses[0].type == CalamityID.Anahita))
                            {
                                Main.NewText("-|- The Leviathan and Anahita -|-", color);
                            }
                            else if (bosses.Count <= 4 && (bosses[0].type == CalamityID.Ares || bosses[0].type == CalamityID.Thanatos || bosses[0].type == CalamityID.Artemis || bosses[0].type == CalamityID.Apollo))
                            {
                                Main.NewText("-|- The Exo Mechs -|-", color);
                            }
                            else if (bosses[0].bossRush)
                            {
                                Main.NewText("-|- Boss Rush -|-", color);
                            }
                            else
                            {
                                Main.NewText("-|- Fight Info -|-", color);
                            }
                        }
                        else
                        {
                            Main.NewText("-|- Fight Info -|-", color);
                        }
                    }

                    // Attempt and modifier notice
                    if (settings[0])
                    {
                        string modifiers = " - ";
                        if (TheConfigForThisMod.Instance.InstantDeath != TheConfigForThisMod.TriggerEnum.Disabled)
                        {
                            if (TheConfigForThisMod.Instance.Nohit != TheConfigForThisMod.NohitEnum.NoDamage)
                            {
                                modifiers += "NoHit";
                                if (TheConfigForThisMod.Instance.Nohit == TheConfigForThisMod.NohitEnum.NoIFrames) modifiers += ", No IFrames";
                            }
                            else modifiers += "No Damage";
                        }
                        if (TheConfigForThisMod.Instance.Defiled != TheConfigForThisMod.DefiledEnum.Disabled)
                        {
                            if (modifiers.Length > 3)
                            {
                                modifiers += ", ";
                            }
                            if (TheConfigForThisMod.Instance.Defiled == TheConfigForThisMod.DefiledEnum.NoWingFlight)
                            {
                                modifiers += "No Wing Flight";
                            }
                            else
                            {
                                modifiers += "True Defiled";
                            }
                        }
                        Main.NewText($"Attempt {((TheConfigForThisMod.Instance.FightStatistics.ShowAttempts == TheConfigForThisMod.AttemptEnum.Total) ? TotalAttempts[index] : CurrentAttempts[index])}{((modifiers.Length > 3) ? modifiers : "")}", color);
                    }



                    // Normal fight stuff + condensed info
                    bool downedrav = false;
                    if (bosses.Count == 1)
                    {
                        // Boss Rush completion percent
                        if (bosses[0].bossRush)
                        {
                            double brProgress = Math.Round(RuntimeDetours.BRCompletion * 100, 1);
                            Main.NewText($"Progress: {brProgress}%" + (settings[6] && brProgress < 100 ? $" (Tier {RuntimeDetours.CurrentTier})" : ""), color);
                        }
                        else
                        {
                            BossInfo first = bosses[0];
                            if (bosses[0].despawned)
                            {
                                Main.NewText($"Despawned! ({Math.Round((100 * (float)first.life2 / first.maxHP2), 2)}%)", warnColor);
                            }
                            else if (settings[3])
                            {
                                float health;
                                int healthDisplay;
                                if (first.maxHP != first.maxHP2 && !settings[8])
                                {
                                    health = first.dead ? 0 : first.life2;
                                    healthDisplay = first.maxHP2;
                                }
                                else
                                {
                                    health = (first.dead && !first.deadButNot) ? 0 : first.life;
                                    healthDisplay = first.maxHP;
                                }
                                Main.NewText($"Boss Health: {Math.Ceiling(10000 * health / healthDisplay) / 100}%" + (settings[6] && !first.dead ? $"  ({health} / {healthDisplay})" : ""), color);
                                if (first.maxHP != first.maxHP2 && settings[8])
                                {
                                    health = first.dead ? 0 : first.life2 - first.life;
                                    if (health > 0)
                                    {
                                        healthDisplay = first.maxHP2 - first.maxHP;
                                        if (health / healthDisplay < 1)
                                        {
                                            int phaseIndex = BossSets.PhasePointer(first.type);
                                            if (phaseIndex == 9 || phaseIndex == 17)
                                            {
                                                phaseIndex = BossSets.PhasePointer(Main.npc[first.segmentIndices[0]].type);
                                                if (phaseIndex == 0)
                                                {
                                                    phaseIndex = BossSets.PhasePointer(first.type);
                                                    if (phaseIndex == 9 && ElsiNohitMod.InfernumActive())
                                                    {
                                                        phaseIndex = 22;
                                                    }
                                                }
                                            }
                                            Main.NewText($"{BossSets.PhaseNames[phaseIndex]} Health: {Math.Round((100 * health / healthDisplay), 2)}%" + (settings[6] && !first.dead ? $"  ({health} / {healthDisplay})" : ""), color);
                                        }
                                    }
                                }

                                // Post-Provi Ravager notice
                                if (ElsiNohitMod.CalamityLoaded)
                                {
                                    downedrav = (bool)ElsiNohitMod.Calamity.Call("GetBossDowned", "Providence");
                                    if (downedrav && bosses.Count == 1 && bosses[0].type == CalamityID.Ravager)
                                    {
                                        Main.NewText("Post-Providence", color);
                                    }
                                }
                            }
                        }
                    }
                    // Boss rush percentage
                    else if (bosses[0].bossRush)
                    {
                        double brProgress = Math.Round(RuntimeDetours.BRCompletion * 100, 1);
                        Main.NewText($"Progress: {brProgress}%" + (settings[6] && brProgress < 100 ? $" (Tier {RuntimeDetours.CurrentTier})": ""), color);
                    }
                    // If fight info is condensed, combine all the health and show that ratio
                    else if (settings[7])
                    {
                        float health = 0;
                        int healthDisplay = 0;
                        foreach (BossInfo boss in bosses)
                        {
                            health += (boss.dead) ? 0 : boss.life2;
                            healthDisplay += boss.maxHP2;
                        }
                        Main.NewText($"Boss Health: {Math.Round((100 * health / healthDisplay), 2)}%" + (settings[6] && health > 0 ? $"  ({health} / {healthDisplay})" : ""), color);
                    }


                    // If empty message teehee
                    if (!settings[0] && !settings[1] && !settings[3] && !settings[4] && !settings[5])
                    {
                        Main.NewText("What was even the point of enabling fight stats, then? At least have the fight time.", warnColor);
                        settings[1] = true;
                    }

                    // Fight Time. We subtract 0.0166 from rta because it runs one frame after igt stops counting up
                    bool downedscal = false;
                    if (settings[1])
                    {
                        double igtSeconds = Math.Round(((float)igt / 60f), 2);
                        double seconds = Math.Round(igtSeconds % 60, 2);
                        string igtFormatted = $"{Math.Floor(igtSeconds / 60)}:" + ((seconds < 10) ? "0" : "") + seconds.ToString();
                        double rtaSeconds = Math.Round(DateTime.UtcNow.Subtract(rta).TotalSeconds - 0.0166, 2);
                        seconds = Math.Round(rtaSeconds % 60, 2);
                        string rtaFormatted = $"{Math.Floor(rtaSeconds / 60)}:" + ((seconds < 10) ? "0" : "") + seconds.ToString();
                        double slow = Math.Round(100 - 100 * (igtSeconds / rtaSeconds), 2);
                        if (slow < 0) slow = 0;

                        Main.NewText($"Fight Time: {igtFormatted}" + (settings[6] ? $" ({rtaFormatted} RTA" + (settings[2] ? $" - {slow}% Slowdown)" : ")") : (settings[2] ? $" ({slow}% Slowdown)" : "")), color);

                        // Short acceptance notice
                        if (ElsiNohitMod.CalamityLoaded)
                        {
                            downedscal = (bool)ElsiNohitMod.Calamity.Call("GetBossDowned", "SupremeCalamitas");
                            if (downedscal && bosses.Count == 1 && bosses[0].type == CalamityID.SupremeCalamitas && bosses[0].dead)
                            {
                                Main.NewText("Short Acceptance", color);
                            }
                        }
                    }

                    // Hits taken
                    if (settings[5])
                    {
                        Main.NewText($"Hits Taken: {totalHits}" + (settings[6] && totalDamage > 0 ? $" ({totalDamage} Damage Taken)" : ""), color);
                        if (totalDodges > 0)
                        {
                            Main.NewText($"Hits Dodged: {totalDodges}" + (settings[6] && totalDamageDodged > 0 ? $" ({totalDamageDodged} Damage Dodged)" : ""), color);
                        }
                    }

                    // The Hoard
                    BossInfo exotwin = null;
                    if ((!settings[7] || (CalamityConfig.Instance.BRKiller && BossRushActive())) && bosses.Count > 1)
                    {
                        foreach (BossInfo boss in bosses)
                        {
                            if (boss.bossRush)
                            {
                                continue;
                            }

                            // Title. Combine hunters info if specifics is off
                            if ((boss.type == CalamityID.Artemis || boss.type == CalamityID.Apollo) && !settings[6])
                            {
                                if (exotwin == null)
                                {
                                    exotwin = boss;
                                    continue;
                                }

                                Main.NewText("-|- XS-01 Artemis and XS-03 Apollo -|-", color);
                            }
                            else
                            {
                                Main.NewText("-|- " + boss.name + " -|-", color);
                            }

                            if (boss.despawned)
                            {
                                Main.NewText($"Despawned! ({Math.Round((100 * (float)boss.life2 / boss.maxHP2), 2)}%)", warnColor);
                            }
                            // Health + segment health
                            else if (settings[3])
                            {
                                float health;
                                int healthDisplay;
                                if (boss.maxHP != boss.maxHP2 && !settings[8])
                                {
                                    health = boss.dead ? 0 : boss.life2;
                                    healthDisplay = boss.maxHP2;
                                }
                                else
                                {
                                    health = (boss.dead && !boss.deadButNot) ? 0 : boss.life;
                                    healthDisplay = boss.maxHP;
                                }
                                Main.NewText($"Boss Health: {Math.Round((100 * health / healthDisplay), 2)}%" + (settings[6] && !boss.dead ? $"  ({health} / {healthDisplay})" : ""), color);
                                if (boss.maxHP != boss.maxHP2 && settings[8])
                                {
                                    health = boss.dead ? 0 : boss.life2 - boss.life;
                                    if (health > 0)
                                    {
                                        healthDisplay = boss.maxHP2 - boss.maxHP;
                                        if (health / healthDisplay < 1)
                                        {
                                            int phaseIndex = BossSets.PhasePointer(boss.type);
                                            if (phaseIndex == 9 || phaseIndex == 17)
                                            {
                                                phaseIndex = BossSets.PhasePointer(Main.npc[boss.segmentIndices[0]].type);
                                                if (phaseIndex == 0)
                                                {
                                                    phaseIndex = BossSets.PhasePointer(boss.type);
                                                    if (phaseIndex == 9 && ElsiNohitMod.InfernumActive())
                                                    {
                                                        phaseIndex = 22;
                                                    }
                                                }
                                            }
                                            Main.NewText($"{BossSets.PhaseNames[phaseIndex]} Health: {Math.Round((100 * health / healthDisplay), 2)}%" + (settings[6] && !boss.dead ? $"  ({health} / {healthDisplay})" : ""), color);
                                        }
                                    }
                                }

                                // Post-Provi Ravager notice
                                if (ElsiNohitMod.CalamityLoaded)
                                {
                                    downedrav = (bool)ElsiNohitMod.Calamity.Call("GetBossDowned", "Providence");
                                    if (downedrav && boss.type == CalamityID.Ravager)
                                    {
                                        Main.NewText("Post-Providence", color);
                                    }
                                }

                            }
                            // Time
                            if (settings[1])
                            {
                                double igtSeconds = Math.Round(((float)boss.bossIGT / 60f), 2);
                                double seconds = Math.Round(igtSeconds % 60, 2);
                                string igtFormatted = $"{Math.Floor(igtSeconds / 60)}:" + ((seconds < 10) ? "0" : "") + seconds.ToString();
                                double rtaSeconds = Math.Round(boss.bossRTAEnd.Subtract(boss.bossRTAStart).TotalSeconds, 2);
                                seconds = Math.Round(rtaSeconds % 60, 2);
                                string rtaFormatted = $"{Math.Floor(rtaSeconds / 60)}:" + ((seconds < 10) ? "0" : "") + seconds.ToString();
                                double slow = Math.Round(100 - 100 * (igtSeconds / rtaSeconds), 2);
                                if (slow < 0) slow = 0;

                                Main.NewText($"Fight Time: {igtFormatted}" + (settings[6] ? $" ({rtaFormatted} RTA" + (settings[2] ? $" - {slow}% Slowdown)" : ")") : (settings[2] ? $" ({slow}% Slowdown)" : "")), color);
                                if (downedscal && boss.type == CalamityID.SupremeCalamitas && boss.dead)
                                {
                                    Main.NewText("Short Acceptance", color);
                                }
                            }
                            // Exo twins
                            if (settings[5])
                            {
                                if (exotwin != null)
                                {
                                    Main.NewText($"Hits Taken: {boss.hitsDealt + exotwin.hitsDealt}", color);
                                    exotwin = null;
                                }
                                else
                                {
                                    Main.NewText($"Hits Taken: {boss.hitsDealt}" + ((boss.damageTaken > 0) ? settings[6] ? $" ({boss.damageTaken} Damage Taken)" : "" : ""), color);
                                }
                            }
                        }
                    }
                }
            }
        }



        // Infernum weak references, for making sure CV dark energy get properly marked as inactive
        // and that the Pirmordial Wyrm fight stops as soon as the Terminus appears
        [JITWhenModsEnabled("InfernumMode")]
        public static bool CVDE(NPC npc) => npc.Infernum().ExtraAI[1] == 1f;

        [JITWhenModsEnabled("InfernumMode")]
        public static bool AEW(NPC npc) => npc.Infernum().ExtraAI[14] >= 1f ;



        // Saving and loading
        public static List<string> BossNames;

        public static List<int> TotalAttempts;

        public static List<int> CurrentAttempts;

        public static List<bool> NewSession;

        public static List<string> BossRushBosses;

        public static List<int> TotalBossRushDeaths;

        public static List<int> CurrentBossRushDeaths;

        public override void ClearWorld()
        {
            BossNames = new List<string>();
            TotalAttempts = new List<int>();
            CurrentAttempts = new List<int>();
            NewSession = new List<bool>();

            BossRushBosses = new List<string>();
            TotalBossRushDeaths = new List<int>();
            CurrentBossRushDeaths = new List<int>();
        }

        public override void LoadWorldData(TagCompound tag)
        {
            BossNames = tag.GetList<string>("BossNames").ToList();
            TotalAttempts = tag.GetList<int>("TotalAttempts").ToList();
            CurrentAttempts = tag.GetList<int>("CurrentAttempts").ToList();
            NewSession = tag.GetList<bool>("NewSession").ToList();

            BossRushBosses = tag.GetList<string>("BossRushBosses").ToList();
            TotalBossRushDeaths = tag.GetList<int>("TotalBossRushDeaths").ToList();
            CurrentBossRushDeaths = tag.GetList<int>("CurrentBossRushDeaths").ToList();
        }

        public override void SaveWorldData(TagCompound tag)
        {
            tag["BossNames"] = BossNames;
            tag["TotalAttempts"] = TotalAttempts;
            tag["CurrentAttempts"] = CurrentAttempts;
            tag["NewSession"] = NewSession;

            tag["BossRushBosses"] = BossRushBosses;
            tag["TotalBossRushDeaths"] = TotalBossRushDeaths;
            tag["CurrentBossRushDeaths"] = CurrentBossRushDeaths;
        }
    }
    
    // fuck
    public class Teleporter : ModSystem
    {
        // Map teleporter
        public override void PostDrawFullscreenMap(ref string mouseText)
        {
            if (false)//(Main.mouseRight)
            {
                // damn it
                Vector2 cursor = new Vector2(Main.mouseX, Main.mouseY);
                Vector2 cursorPos = 16 * (Main.MouseScreen / Main.mapFullscreenScale);

                if (cursorPos.X < 0) cursorPos.X = 0;
                if (cursorPos.X > 16 * Main.maxTilesX) cursorPos.X = 16 * Main.maxTilesX;
                if (cursorPos.Y < 0) cursorPos.Y = 0;
                if (cursorPos.Y > 16 * Main.maxTilesY) cursorPos.Y = 16 * Main.maxTilesY;

                Player player = Main.player[Main.myPlayer];
                player.position = cursorPos;
            }
        }
    }
}