using System;
using System.Reflection;
using Terraria;
using Terraria.DataStructures;
using Terraria.Localization;
using Terraria.ModLoader;



namespace ElsiNohitMod.Content.BossTracking
{
    public class PlayerSystem : ModPlayer
    {
        private int timer = -1;

        private bool infernoDeath = false;
        private bool maelstromDeath = false;



        public override void Load()
        {
            On_Player.UpdateImmunity += NoIFrames;
            On_Player.Hurt_PlayerDeathReason_int_int_refHurtInfo_bool_bool_int_bool_float_float_float += TrackDodges;
        }



        // Removes IFrames
        private static void NoIFrames(On_Player.orig_UpdateImmunity orig, Player self)
        {
            if (TheConfigForThisMod.Instance.Nohit == TheConfigForThisMod.NohitEnum.NoIFrames)
            {
                self.immuneTime = 0;
                self.immune = false;
                for (int i = 0; i < self.hurtCooldowns.Length; i++)
                {
                    if (self.hurtCooldowns[i] > 0)
                    {
                        self.hurtCooldowns[i] = 0;
                    }
                }
            }
            orig(self);
        }



        // Tracks dodges
        private static double TrackDodges(On_Player.orig_Hurt_PlayerDeathReason_int_int_refHurtInfo_bool_bool_int_bool_float_float_float orig, Player self, PlayerDeathReason damageSource, int Damage, int hitDirection, out Player.HurtInfo info, bool pvp = false, bool quiet = false, int cooldownCounter = -1, bool dodgeable = true, float armorPenetration = 0f, float scalingArmorPenetration = 0f, float knockback = 4.5f)
        {
            bool dodged = !self.shimmering && !self.creativeGodMode && !PlayerLoader.ImmuneTo(self, damageSource, cooldownCounter, dodgeable);
            double track = orig(self, damageSource, Damage, hitDirection, out info, pvp, quiet, cooldownCounter, dodgeable, armorPenetration, scalingArmorPenetration, knockback);
            if (dodged && !info.Cancelled && track == 0.0 && info.Damage > 0)
            {
                BossSystem.totalDodges++;
                BossSystem.totalDamageDodged += info.Damage;
            }
            return track;
        }



        // Heals the player and resets the clear timer. Also has useless code for attempt popups, but that should probably be an "on summon" thing anyways...
        public override void OnRespawn()
        {
            if (TheConfigForThisMod.Instance.ClearChat)
            {
                timer = TheConfigForThisMod.Instance.ClearDelay;
            }
            /*if (TheConfigForThisMod.Instance.ShowAttempt)
            {
                AdvancedPopupRequest attemptNumber = new AdvancedPopupRequest();
                attemptNumber.Text = "placeholder until actually functional lol"; // duh
                attemptNumber.DurationInFrames = TheConfigForThisMod.Instance.ShowAttemptLength;
                attemptNumber.Velocity.Y = 1f;
                attemptNumber.Color = TheConfigForThisMod.Instance.AttemptColor;
                Vector2 spawnpos = Player.position; // weird position
                PopupText.NewText(attemptNumber, spawnpos);
            }*/
            if (TheConfigForThisMod.Instance.RespawnHP) { Player.statLife = Player.statLifeMax2; }
            /*if (TheConfigForThisMod.Instance.ResetMusic)
            {
                for (int i = 0; i < Main.musicFade.Length; i++)
                {
                    Main.musicFade[i] = 0;
                }
            }*/
        }



        // Clears chat and instakills from cal debuffs
        public override void PostUpdate()
        {
            if (timer >= 0)
            {
                timer--;
                if (timer < 0)
                {
                    for (int i = 0; i < 11; i++) Main.NewText("");
                }
            }
            if (TheConfigForThisMod.Instance.InstantDeath == TheConfigForThisMod.TriggerEnum.Everything || TheConfigForThisMod.Instance.InstantDeath == TheConfigForThisMod.TriggerEnum.BossesOnly && BossSystem.BossAlive)
            {
                if (CalamityConfig.Instance.DebuffInstakill)
                {
                    if (ElsiNohitMod.CalamityLoaded)
                    {
                        if (Player.HasBuff(CalamityID.HolyInferno))
                        {
                            PlayerDeathReason source = new PlayerDeathReason();
                            infernoDeath = true;
                            Player.Hurt(source, 1, 0);
                        }
                        if (Player.HasBuff(CalamityID.VulnHex))
                        {
                            PlayerDeathReason source = new PlayerDeathReason();
                            maelstromDeath = true;
                            Player.Hurt(source, 1, 0);
                        }
                    }
                }
            }
        }



        // Instakills on negative regen (debuffs or whatever)
        public override void UpdateBadLifeRegen()
        {
            base.UpdateBadLifeRegen();
            if (TheConfigForThisMod.Instance.Nohit == TheConfigForThisMod.NohitEnum.NoDamage && Player.lifeRegen < 0)
            {
                Player.statLife = -1000;
            }
        }

        // Deals with instant killing and dodges
        public override void ModifyHurt(ref Player.HurtModifiers modifiers)
        {
            if (TheConfigForThisMod.Instance.InstantDeath == TheConfigForThisMod.TriggerEnum.Everything || TheConfigForThisMod.Instance.InstantDeath == TheConfigForThisMod.TriggerEnum.BossesOnly && BossSystem.BossAlive)
            {
                Player.HurtModifiers placeholder = modifiers;
                modifiers = new Player.HurtModifiers
                {
                    DamageSource = placeholder.DamageSource,
                    PvP = placeholder.PvP,
                    CooldownCounter = placeholder.CooldownCounter,
                    Dodgeable = TheConfigForThisMod.Instance.Nohit == TheConfigForThisMod.NohitEnum.NoDamage && placeholder.Dodgeable ? true : false,
                    HitDirection = placeholder.HitDirection,
                    SourceDamage = placeholder.SourceDamage,
                    IncomingDamageMultiplier = placeholder.IncomingDamageMultiplier,
                    FinalDamage = placeholder.FinalDamage,
                    ArmorPenetration = placeholder.ArmorPenetration,
                    ScalingArmorPenetration = placeholder.ScalingArmorPenetration,
                    Knockback = placeholder.Knockback,
                    KnockbackImmunityEffectiveness = placeholder.KnockbackImmunityEffectiveness,
                };
                modifiers.FinalDamage.Flat += Main.rand.Next(73292, 83718);
                modifiers.FinalDamage.Flat *= Main.rand.Next(5301, 7463);
                modifiers.FinalDamage.Flat += Main.rand.Next(583950, 723094);
                modifiers.SetMaxDamage(999999999);
            }
        }



        // Tracks player damage stats
        public override void OnHurt(Player.HurtInfo info)
        {
            if (BossSystem.BossAlive)
            {
                BossSystem.totalHits++;
                BossSystem.totalDamage += info.Damage;
                PlayerDeathReason reason = info.DamageSource;
                if (reason.SourceNPCIndex != -1)
                {
                    FindBoss(reason.SourceNPCIndex, info);
                }
                else if (reason.SourceProjectileLocalIndex != -1)
                {
                    FindBoss(ElsiGlobalProj.projOwner[reason.SourceProjectileLocalIndex], info);
                }
            }
        }

        void FindBoss(int index, Player.HurtInfo info)
        {
            if (index != -1 && ElsiGlobalNPC.bossOwner[index] != -1)
            {
                foreach (BossSystem.BossInfo boss in BossSystem.ActiveBosses)
                {
                    if (ElsiGlobalNPC.bossOwner[index] == boss.index)
                    {
                        boss.hitsDealt++;
                        boss.damageTaken += info.Damage;
                        break;
                    }
                }
            }
        }

        // Handles funny death messages and respawn timer
        public override void Kill(double damage, int hitDirection, bool pvp, PlayerDeathReason damageSource)
        {
            if (TheConfigForThisMod.Instance.OverrideTimer) { Player.respawnTimer = TheConfigForThisMod.Instance.RespawnTime; }
            if (TheConfigForThisMod.Instance.InstantDeath == TheConfigForThisMod.TriggerEnum.Everything || TheConfigForThisMod.Instance.InstantDeath == TheConfigForThisMod.TriggerEnum.BossesOnly && BossSystem.BossAlive)
            {
                string player = Player.name;

                if (maelstromDeath)
                {
                    maelstromDeath = false;

                    string[] maelstromDeaths =
                    {
                        $"{player} was devastated by uncontrollable, hateful magic.",
                        $"{player} was ruined by peerless witchcraft.",
                        $"{player}'s soul crumbled from the scorn and contempt.",
                        $"{player} was tormented by unfathomable grief.",
                        $"{player} was brought low by the cold fires of lament.",
                        $"{player} was shown a shallow, cruel epiphany. You know, I'm just copying and pasting these from the source code\nwithout any accompanying logic, there's like a 20% chance you're seeing the completely wrong death message.",
                        $"{player} couldn't accept their bleak, uncaring reality.",
                        $"{player} joined the Whispering Maelstrom.",
                        $"{player} was reduced to a faceless screamer.",
                        $"{player}'s soul was siphoned by the Whispering Maelstrom."
                    };

                    damageSource.CustomReason = NetworkText.FromLiteral(maelstromDeaths[Main.rand.Next(maelstromDeaths.Length)]);
                }
                else if (infernoDeath)
                {
                    infernoDeath = false;
                    string[] infernoDeaths =
                    {
                        $"{player} was incinerated by the profaned backdraft.",
                        $"{player} fell prey to their sins.",
                        $"{player} burst into sinless ash.",
                        $"{player} was purified by the profaned flame.",
                        $"{player} was.",
                        $"Huh. I've never actually seen someone die to that. This is all Doze's fault!"
                    };

                    damageSource.CustomReason = NetworkText.FromLiteral(infernoDeaths[Main.rand.Next(infernoDeaths.Length)]);
                }
                else if (BossSystem.BossRushActive())
                {
                    string[] brDeaths =
                    {
                        $"{player} failed the test.",
                        $"{player} was too weak.",
                        $"{player} was insufficient.",
                        $"{player} was unworthy.",
                        $"{player} was disappointing.",
                        $"{player} was insignificant.",
                        $"{player} was nothing.",
                        $"{player} failed.",
                        $"{player} couldn't do it.",
                        $"{player} did not meet expectations.",
                        $"{player} was no more than a distraction.",
                        $"{player} was obliterated by the Primordial Light.",
                        $"{player} was reduced to nothing by the Primordial Light.",
                        $"{player} was entirely ionized by the Primordial Light.",
                        $"{player} was erased by the Primordial Light.",
                        $"{player} was devastated by illusions.",
                        $"{player} was massacred by illusions.",
                        $"{player} was annihilated by brilliant illusions.",
                        $"{player} fell prey to virtuous radiance.",
                        $"{player} is no longer.",
                        $"{player}'s strings were cut by the Primordial Light.",
                        $"Go beat P5 or something",
                        $"Why are you doing this to yourself",
                        $"Dude, Xeroc's gonna be so pissed..."
                    };

                    damageSource.CustomReason = NetworkText.FromLiteral(brDeaths[Main.rand.Next(brDeaths.Length)]);
                }
                else
                {
                    string[] closeDeaths = Array.Empty<string>();

                    float totalHP = 0;
                    float maxHP = 0;
                    foreach (var boss in BossSystem.ActiveBosses)
                    {
                        totalHP += boss.life2;
                        maxHP += boss.maxHP2;
                    }
                    if (maxHP > 0 && totalHP / maxHP < 0.25)
                    {
                        closeDeaths = (totalHP / maxHP < 0.1) ? (totalHP / maxHP < 0.02) ?
                        [
                            "OMGGGG HI #close-calls-and-failures! And also I'm sincerely sorry",
                            $"{player} was murdered. Brutally.",
                            ":(",
                            "Ough, rough.",
                            $"{player} met The Horse."
                        ] :
                        [
                            "Gosh, I hate it when this happens.",
                            $"{player} should persevere.",
                            $"{player} died, but extra tragically this time.",
                            $"{player} basically made it. Don't give up now.",
                            "Hang in there just a little longer, you almost made it!"
                        ] :
                        [
                            $"{player} died, but tragically this time.",
                            $"{player} collapsed from exhaustion.",
                            "Do it again, I wasn't looking!",
                            $"{player} nearly made it. Don't give up now."
                        ];
                    }

                    string[] randomDeaths =
                    {
                        $"{player} stubbed their toe.",
                        $"{player} won't be making it to the moon.",
                        $"{player} was straight up atomized. There's genuinely nothing left.",
                        "What even killed you?",
                        $"{player} didn't see that coming.",
                        $"Ouch, that's gotta hurt.",
                        $"{player} can see the gaps between grains of sand.",
                        $"{player} was cooked into a delicious stew.",
                        $"{player} doesn't know!",
                        $"you lost the game",
                        $"Better remove those thorns next time! Probably thorns. Or maybe it was fall damage?"
                    };

                    string killer;
                    if (damageSource.SourceNPCIndex != -1) { killer = Main.npc[damageSource.SourceNPCIndex].FullName; }
                    else if (damageSource.SourceProjectileLocalIndex != -1) { killer = Main.projectile[damageSource.SourceProjectileLocalIndex].Name; }
                    else if (damageSource.SourcePlayerIndex != -1) { killer = Main.player[damageSource.SourcePlayerIndex].name; }
                    else
                    {
                        damageSource.CustomReason = NetworkText.FromLiteral((closeDeaths.Length > 0) ? closeDeaths[Main.rand.Next(closeDeaths.Length)] : randomDeaths[Main.rand.Next(randomDeaths.Length)]);
                        return;
                    }

                    if (maxHP > 0 && totalHP / maxHP < 0.25)
                    {
                        closeDeaths = (totalHP / maxHP < 0.1) ? (totalHP / maxHP < 0.02) ?
                        [
                            $"OMGGGG HI #close-calls-and-failures! And also I'm sincerely sorry. You were killed by {killer}.",
                            $"{player} was murdered by {killer}. Brutally.",
                            $"No comment this time. {player} was killed by {killer}.",
                            $"Gosh, that's unlucky. You'll get it next time. ({killer})",
                            $"{player} was destroyed by {killer}. You should take a break. It helps, I promise."
                        ] :
                        [
                            $"{killer} utterly oblitera- oh, dang. That's unfortunate. Don't give in just yet.",
                            $"Ah, {killer}'s anti-{player} technique. They haven't used this since the Heian era.",
                            $"{player}, I realize this moment may not be the most... convenient for a heart to heart, but I had to wait until your... {killer} was otherwise, occupied.",
                            $"Hang in there just a little longer, you almost made it! ({killer})",
                            $"Keep determined. You've got this. ({killer}"
                        ] :
                        [
                            $"AND HERE COMES {killer} WITH THE STEEL CHAIR!",
                            $"{player} was betrayed by {killer}.",
                            $"{killer} called you mean names! You're not going to let them get away with that, are you?",
                            $"{player} was destroyed by {killer}; little does {killer} know, next attempt, {player} is going to win!",
                            $"I think I'm putting too much effort into this... nobody even reads death messages. Besides {killer}, of course, but they suck and don't count."
                        ];

                        damageSource.CustomReason = NetworkText.FromLiteral(closeDeaths[Main.rand.Next(closeDeaths.Length)]);
                        return;
                    }

                    string[] importantDeaths =
                    {
                        $"{player} was utterly annihilated by {killer}.",
                        $"{player} was reduced to a fine powder by {killer}.",
                        $"{player} was told an unamusing joke by {killer}.",
                        $"{player} was disassembled on the molecular level by {killer}.",
                        $"{player} was drawn from memory by {killer}.",
                        $"{player} was absolutely destroyed by {killer})",
                        $"{player} was dismantled by {killer}.",
                        $"{player} was turned into a wall ornament by {killer}.",
                        $"{player} was turned into a coat rack by {killer}.",
                        $"{player} was turned into a bulletin board by {killer}.",
                        $"{player} was returned to slime by {killer}.",
                        $"{player} tripped over their own feet, and {killer} saw!",
                        $"{player} got retconned by {killer}.",
                        $"{player} was soulkilled by {killer}.",
                        $"{player} was subatomically disassembled by {killer}.",
                        $"{player} was made into a picture frame by {killer}.",
                        $"{killer} gave {player} a closed-casket funeral.",
                        $"{killer} mistook {player} for company property, oops!",
                        $"{killer} turned {player} from biology into physics.",
                        $"{player} was shoved into a hydraulic press by {killer}. Twice.",
                        $"{killer} took {player} to a chiropractor.",
                        $"{killer} taught {player} about past tense.",
                        $"{killer} gave {player} a knuckle sandwich, but instead of a knuckle, it was a thermonuclear bomb",
                        $"{player} was... uh, sorry, I wasn't looking.",
                        $"Talk to your doctor to see if {player} is right for you! Side effects may include: nausea, {killer}, sensitivity to bright lights, complete and utter annihilation, and headaches.",
                        $"{player} asked {killer} out on a date in #mountains-of-salt."
                    };
                    /* to implement
                       $"{player} was Returned to Slime." //sg
                       $"{player} was Returned to Sli- wait, that's not the right boss!" //king slime
                       $"{player} was Returned to- oh what the hell that's not even a boss" //biome mimic
                       $"You know, one of the aliases for Queen Slime is "slimegirl". Does... does anyone actually call her that??" //qs
                       $"{player} forgot to gatekeep, gaslight, and girlboss." //scal
                       $"{player} died to... probably some bullshit" //exos
                       $"why are you even doing this" //primordial wyrm
                       $"Don't get cocky, kid!" //dog
                    */

                    damageSource.CustomReason = NetworkText.FromLiteral(importantDeaths[Main.rand.Next(importantDeaths.Length)]);
                }
            }
            if (BossSystem.BossRushActive())
            {
                if (ElsiNohitMod.CalamityLoaded)
                {
                    string brkiller = NPC.GetFullnameByID(CurrentBoss);
                    if (!BossSystem.BossRushBosses.Contains(brkiller))
                    {
                        BossSystem.BossRushBosses.Add(brkiller);
                        BossSystem.TotalBossRushDeaths.Add(1);
                        BossSystem.CurrentBossRushDeaths.Add(1);
                    }
                    else
                    {
                        int index = BossSystem.BossRushBosses.IndexOf(brkiller);
                        BossSystem.TotalBossRushDeaths[index]++;
                        BossSystem.CurrentBossRushDeaths[index]++;
                    }
                }
            }
        }

        [JITWhenModsEnabled("CalamityMod")]
        public static PropertyInfo CurrentlyFoughtBoss => typeof(CalamityMod.Events.BossRushEvent).GetProperty("CurrentlyFoughtBoss", BindingFlags.Public | BindingFlags.Static);

        [JITWhenModsEnabled("CalamityMod")]
        public static int CurrentBoss => (int)CurrentlyFoughtBoss.GetValue(null);
    }
}
