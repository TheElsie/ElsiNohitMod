using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;



namespace ElsiNohitMod.Content.BossTracking
{
    // Shows total attempt counts
    public class TotalAttempts : ModCommand
    {
        // Between the horrid 2016 humor boss aliases and the dogshit coding job this might be the worst thing I've even written
        // I fixed the code :)
        private static Color warning => TheConfigForThisMod.Instance.FightStatistics.WarningColor;
        private static Color color => TheConfigForThisMod.Instance.CommandColor;

        public override CommandType Type => CommandType.Chat;

        public override string Command => "totalattempts";


        public static Dictionary<string, string> Aliases = new Dictionary<string, string>();

        public override void Load()
        {
            AddAlias("King Slime", true, new string[] { "ks", "slimeking" });
            AddAlias("Desert Scourge", true, new string[] { "ds", "scourge" });
            AddAlias("Eye of Cthulhu", true, new string[] { "eoc" }); // I'm not adding "sus" you can't make me
            AddAlias("Crabulon", true, new string[] { });
            AddAlias("Eater of Worlds", true, new string[] { "eow" });
            AddAlias("Brain of Cthulhu", true, new string[] { "boc" });
            AddAlias("The Hive Mind", true, new string[] { "hm", "reddit" });
            AddAlias("The Perforators", true, new string[] { "perfs", "perforatorhive" });
            AddAlias("Queen Bee", true, new string[] { "qb", "b" });
            AddAlias("Deerclops", true, new string[] { "dst", "slop" });
            AddAlias("Skeletron", true, new string[] { "skeleton", "skelly", "sans" });
            AddAlias("The Slime God", true, new string[] { "sg", "goozma", "tidepod" });
            AddAlias("Wall of Flesh", true, new string[] { "wof", "woffle" });
            AddAlias("Dreadnautilus", true, new string[] { "dread", "nautilus", "fakeboss" }); // inf dread
            AddAlias("Queen Slime", true, new string[] { "qs", "slimegirl" });
            AddAlias("Cryogen", true, new string[] { "icecube", "cryoven" });
            AddAlias("The Destroyer", true, new string[] { });
            AddAlias("Aquatic Scourge", true, new string[] { "as", "waterworm" });
            AddAlias("The Twins", true, new string[] { "ret", "spaz", "retinazer", "spazmatism" });
            AddAlias("Brimstone Elemental", true, new string[] { "brimmy", "brimele" });
            AddAlias("Skeletron Prime", true, new string[] { "sprime", "skeleprime", "skeleton prime", "sp", "skellyprime", "metalsans" }); // god
            AddAlias("Calamitas Clone", true, new string[] { "calclone", "clonelamitas", "cal" });
            AddAlias("The Forgotten Shadow of Calamitas", true, new string[] { "calshad", "calamitasshadow", "shadow", "forgottenshadowofcalamitas" }); // inf calshad
            AddAlias("Plantera", true, new string[] { });
            AddAlias("Leviathan and Anahita", true, new string[] { "theleviathanandanahita", "anahitaandtheleviathan", "leviana", "analev", "siren" });
            AddAlias("Golem", true, new string[] { "rock", "lihzarhd" });
            AddAlias("The Plaguebringer Goliath", true, new string[] { "pbg" });
            AddAlias("Empress of Light", true, new string[] { "eol" }); // not doing that last one
            AddAlias("Duke Fishron", true, new string[] { "df" });
            AddAlias("Ravager", true, new string[] { "ravhager" });
            AddAlias("Lunatic Cultist", true, new string[] { "lc" });
            AddAlias("Astrum Deus", true, new string[] { "clowncar", "ad", "astrumadios", "adios" });
            AddAlias("Argus, the Bereft Vassal", true, new string[] { "bereftvassal", "taurusthegreatsandshark", "taurusgreatsandshark", "argusbereftvassal" });
            AddAlias("Moon Lord", true, new string[] { "ml", "moonlordscore", "moonlord'score", "moonman" });
            AddAlias("Guardian Commander", true, new string[] { "profanedguardians", "guards", "donuts" });
            AddAlias("Dragonfolly", true, new string[] { "bumblebirb", "bumblefuck", "birb", "dfolly" });
            AddAlias("Providence, the Profaned Goddess", true, new string[] { });
            AddAlias("Storm Weaver", true, new string[] { "sw" });
            AddAlias("Ceaseless Void", true, new string[] { "cv", "ceaselesspain" });
            AddAlias("Signus", true, new string[] { "signut" });
            AddAlias("Polterghast", true, new string[] { });
            AddAlias("The Old Duke", true, new string[] { "od", "boomer" });
            AddAlias("The Devourer of Gods", true, new string[] { "dog" });
            AddAlias("Yharon, Dragon of Rebirth", true, new string[] { "jungledragonyharon", "yharonresplendantphoenix", "jdy" });
            AddAlias("Primordial Wyrm", true, new string[] { "pw", "adulteidolonwyrm", "aew", "jared" });
            AddAlias("The Exo Mechs", true, new string[] { "exos", "draedon", "ares", "thanatos", "artemis", "apollo", "hunters", "t-hanos", "blender", "ohio" });
            AddAlias("Supreme Witch, Calamitas", true, new string[] { "scal", "supremecalamitas", "sc", "witch" });
            AddAlias("Boss Rush", true, new string[] { "br" });
        }

        public static void AddAlias(string boss, bool init, params string[] args)
        {
            foreach (string alias in args)
            {
                Aliases.Add(alias, boss);
            }
            if (init) Aliases.Add(boss.ToLower().Replace(" ", ""), boss);
        }

        
        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length == 0)
            {
                int deaths = 0;
                foreach (int i in BossSystem.TotalAttempts)
                {
                    deaths += i;
                }
                caller.Reply($"Total attempts: {deaths}", color);
            }
            else
            {
                FindBosses(caller, BossSystem.BossNames, args, func);
            }
        }

        public static void FindBosses(CommandCaller caller, List<string> compareTo, string[] args, Func<string, int, string> Lambda)
        {
            string test = "";
            string bossName = default;
            bool success;
            string failed = "";
            List<string> blacklist = new List<string>();
            for (int i = 0; i < args.Length; i++)
            {
                test = "";
                for (int j = 0; j < args.Length - i; j++)
                {
                    test += args[i + j];
                    success = Aliases.TryGetValue(test, out bossName);
                    if (!success)
                    {
                        bossName = Aliases.FirstOrDefault(key => key.Key.Contains(test)).Value;
                    }
                    if (bossName != default)
                    {
                        test = "";
                        i += j;
                        break;
                    }
                }
                if (bossName == default)
                {
                    failed += args[i] + " ";
                }
                else
                {
                    if (failed != "")
                    {
                        if (!blacklist.Contains(failed))
                        {
                            caller.Reply($"Alias \"{failed.Substring(0, failed.Length - 1)}\" not recognized!", warning);
                            blacklist.Add(failed);
                        }
                        failed = "";
                    }
                    if (!blacklist.Contains(bossName))
                    {
                        int index = compareTo.IndexOf(bossName);
                        if (index != -1)
                        {
                            caller.Reply(Lambda(bossName, index), color);
                        }
                        else
                        {
                            caller.Reply($"{bossName}: 0", color);
                        }
                        blacklist.Add(bossName);
                    }
                }
            }
            if (failed != "" && !blacklist.Contains(failed))
            {
                caller.Reply($"Alias \"{failed.Substring(0, failed.Length - 1)}\" not recognized!", warning);
            }
        }

        private static Func<string, int, string> func = (bossName, index) => $"{bossName}: {BossSystem.TotalAttempts[index]}";
    }
    // Aliases
    public class TotalAttempt : TotalAttempts
    {
        public override string Command => "totalattempt";

        public override void Load()
        {
        }
    }
    public class Total : TotalAttempts
    {
        public override string Command => "total";

        public override void Load()
        {
        }
    }



    // Show session attempt counts
    public class SessionAttempts : ModCommand
    {
        private Color warning => TheConfigForThisMod.Instance.FightStatistics.WarningColor;
        private Color color => TheConfigForThisMod.Instance.CommandColor;
        private Dictionary<string, string> Aliases => TotalAttempts.Aliases;

        public override CommandType Type => CommandType.Chat;

        public override string Command => "sessionattempts";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // Show all deaths to individual bosses
            if (args.Length == 0)
            {
                if (BossSystem.BossRushBosses.Count > 0)
                {
                    caller.Reply("Session attempts:", color);
                    for (int i = 0; i < BossSystem.BossNames.Count; i++)
                    {
                        caller.Reply($"{BossSystem.BossNames[i]}: {BossSystem.CurrentAttempts[i]}", color);
                    }
                }
                else
                {
                    caller.Reply("No boss deaths logged!", warning);
                }
            }
            else
            {
                TotalAttempts.FindBosses(caller, BossSystem.BossNames, args, func);
            }
        }

        private static Func<string, int, string> func = (bossName, index) => $"{bossName}: {BossSystem.CurrentAttempts[index]}";
    }
    // Aliases
    public class SessionAttempt : SessionAttempts
    {
        public override string Command => "sessionattempts";
    }
    public class Session : SessionAttempts
    {
        public override string Command => "session";
    }
    public class Attempts : SessionAttempts
    {
        public override string Command => "attempts";
    }
    public class Attempt : SessionAttempts
    {
        public override string Command => "attempt";
    }



    // Starts a new session
    public class StartNewSession : ModCommand
    {
        private Color warning => TheConfigForThisMod.Instance.FightStatistics.WarningColor;
        private Color color => TheConfigForThisMod.Instance.CommandColor;
        private Dictionary<string, string> Aliases => TotalAttempts.Aliases;

        private static bool brClear;

        public override CommandType Type => CommandType.Chat;

        public override string Command => "startnewsession";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // New session for everyone
            brClear = false;
            if (args.Length == 0)
            {
                if (BossSystem.BossRushBosses.Count > 0)
                {
                    for (int i = 0; i < BossSystem.BossNames.Count; i++)
                    {
                        BossSystem.CurrentAttempts[i] = 0;
                        if (BossSystem.BossNames[i] == "Boss Rush")
                        {
                            brClear = true;
                        }
                    }
                    caller.Reply("New session created!", color);
                }
                else
                {
                    caller.Reply("Sure, but there's nothing logged to even change lol", warning);
                }
            }
            else
            {
                // New session for specific bosses
                TotalAttempts.FindBosses(caller, BossSystem.BossNames, args, func);
            }
            if (brClear)
            {
                for (int i = 0; i < BossSystem.CurrentBossRushDeaths.Count; i++)
                {
                    BossSystem.CurrentBossRushDeaths[i] = 0;
                }
            }
        }

        private static Func<string, int, string> func = (bossName, index) =>
        {
            BossSystem.CurrentAttempts[index] = 0;
            if (BossSystem.BossNames[index] == "Boss Rush")
            {
                brClear = true;
            }
            return $"New session started for {bossName}.";
        };
    }
    // Aliases
    public class NewSession : StartNewSession
    {
        public override string Command => "newsession";
    }
    public class StartNew : StartNewSession
    {
        public override string Command => "startnew";
    }
    public class StartSession : StartNewSession
    {
        public override string Command => "startsession";
    }



    // Sets session attempt counts
    public class SetAttempts : ModCommand
    {
        private Color warning => TheConfigForThisMod.Instance.FightStatistics.WarningColor;
        private Color color => TheConfigForThisMod.Instance.CommandColor;

        public override CommandType Type => CommandType.Chat;

        public override string Command => "setattempts";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length == 0)
            {
                caller.Reply("No arguments identified!", warning);
            }
            else if (args.Length == 1)
            {
                // Nuke everything
                if (args[0] == 0.ToString())
                {
                    for (int i = 0; i < BossSystem.CurrentAttempts.Count; i++)
                    {
                        BossSystem.CurrentAttempts[i] = 0;
                    }
                    for (int i = 0; i < BossSystem.CurrentBossRushDeaths.Count; i++)
                    {
                        BossSystem.CurrentBossRushDeaths[i] = 0;
                    }
                    caller.Reply("All session attempts cleared!", color);
                }
                else
                {
                    caller.Reply("Second argument missing.", warning);
                }
            }
            else
            {
                // Make sure there's a number to set it to
                if (!int.TryParse(args[args.Length - 1], out int newAttempts))
                {
                    caller.Reply("Could not interpret. Please use the format \"/setattempts [boss name] [desired attempt count]\".", warning);
                }
                else
                {
                    string bossName = null;
                    string test = "";
                    for (int i = 0; i < args.Length - 1; i++)
                    {
                        if (bossName == null)
                        {
                            test += args[i];
                        }
                        else
                        {
                            break;
                        }
                        if (!TotalAttempts.Aliases.TryGetValue(test, out bossName))
                        {
                            bossName = TotalAttempts.Aliases.FirstOrDefault(alias => alias.Key.Contains(test)).Value;
                        }
                    }
                    if (bossName == null)
                    {
                        caller.Reply($"Alias \"{test}\" could not be identified.", warning);
                    }
                    else
                    {
                        int index = BossSystem.BossNames.IndexOf(bossName);
                        if (index != -1)
                        {
                            BossSystem.CurrentAttempts[index] = newAttempts;
                            if (bossName == "Boss Rush" && newAttempts == 0)
                            {
                                for (int i = 0; i < BossSystem.CurrentBossRushDeaths.Count; i++)
                                {
                                    BossSystem.CurrentBossRushDeaths[i] = 0;
                                }
                            }
                        }
                        caller.Reply($"{bossName} session attempt counter successfully changed to {newAttempts}.", color);
                    }
                }
            }
        }
    }
    // Aliases
    public class SetAttempt : SetAttempts
    {
        public override string Command => "setattempt";
    }



    // Sets total attempt counts
    public class SetTotalAttempts : ModCommand
    {
        private Color warning => TheConfigForThisMod.Instance.FightStatistics.WarningColor;
        private Color color => TheConfigForThisMod.Instance.CommandColor;

        public override CommandType Type => CommandType.Chat;

        public override string Command => "settotalattempts";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length == 0)
            {
                caller.Reply("No arguments identified!", warning);
            }
            else if (args.Length == 1)
            {
                // Nuke everything
                if (args[0] == 0.ToString())
                {
                    BossSystem.BossNames = new List<string>();
                    BossSystem.BossRushBosses = new List<string>();

                    BossSystem.TotalAttempts = new List<int>();
                    BossSystem.TotalBossRushDeaths = new List<int>();
                    BossSystem.CurrentAttempts = new List<int>();

                    BossSystem.CurrentBossRushDeaths = new List<int>();
                    caller.Reply("All attempts cleared!", color);
                }
                else
                {
                    caller.Reply("Second argument missing.", warning);
                }
            }
            else
            {
                // Make sure there's a number to set it to
                if (!int.TryParse(args[args.Length - 1], out int newAttempts))
                {
                    caller.Reply("Could not interpret. Please use the format \"/settotalattempts [boss name] [desired attempt count]\".", warning);
                }
                else
                {
                    string bossName = null;
                    string test = "";
                    for (int i = 0; i < args.Length - 1; i++)
                    {
                        if (bossName == null)
                        {
                            test += args[i];
                        }
                        else
                        {
                            break;
                        }
                        if (!TotalAttempts.Aliases.TryGetValue(test, out bossName))
                        {
                            bossName = TotalAttempts.Aliases.FirstOrDefault(alias => alias.Key.Contains(test)).Value;
                        }
                    }
                    if (bossName == null)
                    {
                        caller.Reply($"Alias \"{test}\" could not be identified.", warning);
                    }
                    else
                    {
                        int index = BossSystem.BossNames.IndexOf(bossName);
                        if (index != -1)
                        {
                            BossSystem.TotalAttempts[index] = newAttempts;
                            if (BossSystem.CurrentAttempts[index] > newAttempts)
                            {
                                BossSystem.CurrentAttempts[index] = newAttempts;
                            }
                            if (bossName == "Boss Rush" && newAttempts == 0)
                            {
                                for (int i = 0; i < BossSystem.TotalBossRushDeaths.Count; i++)
                                {
                                    BossSystem.TotalBossRushDeaths[i] = 0;
                                    BossSystem.CurrentBossRushDeaths[i] = 0;
                                }
                            }
                        }
                        caller.Reply($"{bossName} total attempt counter successfully changed to {newAttempts}.", color);
                    }
                }
            }
        }
    }
    // Aliases
    public class SetTotalAttempt : SetTotalAttempts
    {
        public override string Command => "settotalattempt";
    }



    // Show session boss rush deaths
    public class BossRush : ModCommand
    {
        private Color warning => TheConfigForThisMod.Instance.FightStatistics.WarningColor;
        private Color color => TheConfigForThisMod.Instance.CommandColor;
        private Dictionary<string,string> Aliases => TotalAttempts.Aliases;

        public override CommandType Type => CommandType.Chat;

        public override string Command => "bossrush";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            // Show all deaths to individual bosses
            if (args.Length == 0)
            {
                if (BossSystem.BossRushBosses.Count > 0)
                {
                    caller.Reply("Total session Boss Rush deaths:", color);
                    for (int i = 0; i < BossSystem.BossRushBosses.Count; i++)
                    {
                        caller.Reply($"{BossSystem.BossRushBosses[i]}: {BossSystem.CurrentBossRushDeaths[i]}", color);
                    }
                }
                else
                {
                    caller.Reply("No boss deaths logged!", warning);
                }
            }
            else
            {
                TotalAttempts.FindBosses(caller, BossSystem.BossRushBosses, args, func);
            }
        }

        private static Func<string, int, string> func = (bossName, index) => $"{bossName}: {BossSystem.CurrentBossRushDeaths[index]}";
    }
    // Aliases
    public class BR : BossRush
    {
        public override string Command => "br";
    }
    public class BossRushDeaths : BossRush
    {
        public override string Command => "bossrushdeaths";
    }
    public class BRDeaths : BossRush
    {
        public override string Command => "brdeaths";
    }
    public class BossRushDeath : BossRush
    {
        public override string Command => "bossrushdeath";
    }
    public class BRDeath : BossRush
    {
        public override string Command => "brdeath";
    }



    // Show total boss rush deaths
    public class BossRushTotal : ModCommand
    {
        private Color warning => TheConfigForThisMod.Instance.FightStatistics.WarningColor;
        private Color color => TheConfigForThisMod.Instance.CommandColor;
        private Dictionary<string, string> Aliases => TotalAttempts.Aliases;

        public override CommandType Type => CommandType.Chat;

        public override string Command => "bossrushtotal";

        public override void Action(CommandCaller caller, string input, string[] args)
        {
            if (args.Length == 0)
            {
                if (BossSystem.BossRushBosses.Count > 0)
                {
                    caller.Reply("Total Boss Rush deaths:", color);
                    for (int i = 0; i < BossSystem.BossRushBosses.Count; i++)
                    {
                        caller.Reply($"{BossSystem.BossRushBosses[i]}: {BossSystem.TotalBossRushDeaths[i]}", color);
                    }
                }
                else
                {
                    caller.Reply("No boss deaths logged!", warning);
                }
            }
            else
            {
                TotalAttempts.FindBosses(caller, BossSystem.BossRushBosses, args, func);
            }
        }

        private static Func<string, int, string> func = (bossName, index) => $"{bossName}: {BossSystem.TotalBossRushDeaths[index]}";
    }
    // Aliases
    public class TBR : BossRushTotal
    {
        public override string Command => "tbr";
    }
    public class TotalBossRushDeaths : BossRushTotal
    {
        public override string Command => "totalbossrushdeaths";
    }
    public class TotalBossRushDeath : BossRushTotal
    {
        public override string Command => "totalbossrushdeath";
    }
    public class TotalBRDeaths : BossRushTotal
    {
        public override string Command => "totalbrdeaths";
    }
    public class TotalBRDeath : BossRushTotal
    {
        public override string Command => "totalbrdeath";
    }
}
