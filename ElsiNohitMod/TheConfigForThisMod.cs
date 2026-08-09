using System.ComponentModel;
using ElsiNohitMod.Content;
using Microsoft.Xna.Framework;
using Terraria.ModLoader;
using Terraria.ModLoader.Config;

namespace ElsiNohitMod
{
    
	public class TheConfigForThisMod : ModConfig
    {
        public enum DespawnEnum
        {
            Disabled,
            OnDeath,
            OnRespawn
        }

        public enum AttemptEnum
        {
            False,
            Session,
            Total
        }

        public enum TriggerEnum
        {
            Disabled,
            BossesOnly,
            Everything
        }

        public enum NohitEnum
        {
            Default,
            NoDamage,
            NoIFrames
        }

        public enum DefiledEnum
        {
            Disabled,
            NoWingFlight,
            TrueDefiled
        }

        public static TheConfigForThisMod Instance;

        public override ConfigScope Mode => ConfigScope.ClientSide;

        [Header("Toggles")]

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableEvents.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableEvents.Tooltip")]
		public bool DisableEvents;

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableEnemySpawning.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableEnemySpawning.Tooltip")]
        public bool DisableSpawns;

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableTombstones.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableTombstones.Tooltip")]
        public bool DisableTombstones;

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableFallingStars.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableFallingStars.Tooltip")]
        public bool DisableFallingStars;

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableBossDrops.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableBossDrops.Tooltip")]
        public bool DisableBossDrops;

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableItems.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableItems.Tooltip")]
        public bool DisableItems;

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableCoins.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableCoins.Tooltip")]
        public bool DisableCoins;

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableHearts.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableHearts.Tooltip")]
        public bool DisableHearts;

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableManaStars.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Toggles.DisableManaStars.Tooltip")]
        public bool DisableManaStars;



        [Header("Despawns")]


        [Slider, DrawTicks]
        [DefaultValue(DespawnEnum.Disabled)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnSetting.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnSetting.Tooltip")]
        public DespawnEnum DespawnSetting;

        [Expand(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnToggle.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnToggle.Tooltip")]
        public DespawnToggles Despawn = new DespawnToggles();

        public class DespawnToggles
        {
            [DefaultValue(false)]
            [LabelKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnBosses.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnBosses.Tooltip")]
            public bool DespawnBosses;

            [DefaultValue(false)]
            [LabelKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnEnemies.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnEnemies.Tooltip")]
            public bool DespawnEnemies;

            [DefaultValue(false)]
            [LabelKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnFriendlies.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnFriendlies.Tooltip")]
            public bool DespawnFriendlies;

            [DefaultValue(false)]
            [LabelKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnEnemyProj.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnEnemyProj.Tooltip")]
            public bool DespawnEnemyProj;

            [DefaultValue(false)]
            [LabelKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnFriendlyProj.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnFriendlyProj.Tooltip")]
            public bool DespawnFriendlyProj;

            [DefaultValue(false)]
            [LabelKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnMisc.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnMisc.Tooltip")]
            public bool DespawnMisc;

            [DefaultValue(false)]
            [LabelKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnItems.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Despawns.DespawnItems.Tooltip")]
            public bool DespawnItems;
        }


        [Header("Respawns")]


        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Respawns.OverrideTimer.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Respawns.OverrideTimer.Tooltip")]
        public bool OverrideTimer;

        [Range(1,3600)]
        [DefaultValue(180)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Respawns.RespawnTime.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Respawns.RespawnTime.Tooltip")]
        public int RespawnTime;

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Respawns.RespawnHP.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Respawns.RespawnHP.Tooltip")]
        public bool RespawnHP;

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Respawns.ClearChat.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Respawns.ClearChat.Tooltip")]
        public bool ClearChat;

        [Range(0, 100000)]
        [DefaultValue(60)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Respawns.ClearDelay.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Respawns.ClearDelay.Tooltip")]
        public int ClearDelay;

        /* todo, but doesn't work
        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Respawns.ResetMusic.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Respawns.ResetMusic.Tooltip")]
        public bool ResetMusic;*/

        /*//todo
        [LabelKey("$Mods.ElsiNohitMod.Configs.Respawns.ShowAttempt.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Respawns.ShowAttempt.Tooltip")]
        public bool ShowAttempt;

        [DefaultValue(typeof(Color), "167, 76, 225, 99"), ColorHSLSlider(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Respawns.AttemptColor.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Respawns.AttemptColor.Tooltip")]
        public Color AttemptColor;
        
        //shouldnt be post respawn anyways, should be on boss spawn
        [Range(1, 100000)]
        [DefaultValue(60)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Respawns.ShowAttemptLength.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Respawns.ShowAttemptLength.Tooltip")]
        public int ShowAttemptLength;*/

        [Header("Testing")]
        
        [DefaultValue(true)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Testing.ShowStats.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Testing.ShowStats.Tooltip")]
        public bool ShowFightStats;
        
        [Expand(false)]
        [DefaultValue(typeof(FightStats), "true, true, false, true, true")]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Testing.FightStats.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Testing.FightStats.Tooltip")]
        public FightStats FightStatistics = new FightStats();

        [DefaultValue(typeof(Color), "176, 167, 231, 255"), ColorNoAlpha]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Testing.CommandColor.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Testing.CommandColor.Tooltip")]
        public Color CommandColor;

        public class FightStats
        {
            [Slider, DrawTicks]
            [LabelKey("$Mods.ElsiNohitMod.Configs.Testing.ShowAttempts.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Testing.ShowAttempts.Tooltip")]
            public AttemptEnum ShowAttempts = AttemptEnum.Session;

            [LabelKey("$Mods.ElsiNohitMod.Configs.Testing.ShowHealth.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Testing.ShowHealth.Tooltip")]
            public bool ShowHealth = true;

            [LabelKey("$Mods.ElsiNohitMod.Configs.Testing.DistinguishSegments.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Testing.DistinguishSegments.Tooltip")]
            public bool DistinguishSegments = true;

            [LabelKey("$Mods.ElsiNohitMod.Configs.Testing.ShowTime.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Testing.ShowTime.Tooltip")]
            public bool ShowTime = true;

            [LabelKey("$Mods.ElsiNohitMod.Configs.Testing.ShowSlowdown.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Testing.ShowSlowdown.Tooltip")]
            public bool ShowSlowdown = true;

            /*
            //todo. perhaps overly ambitious, we'll see
            [DefaultValue(true)]
            [LabelKey("$Mods.ElsiNohitMod.Configs.Testing.ShowDPS.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Testing.ShowDPS.Tooltip")]
            public bool ShowDPS;*/

            [LabelKey("$Mods.ElsiNohitMod.Configs.Testing.ShowHits.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Testing.ShowHits.Tooltip")]
            public bool ShowHits = true;

            [LabelKey("$Mods.ElsiNohitMod.Configs.Testing.Specifics.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Testing.Specifics.Tooltip")]
            public bool Specifics = false;

            [LabelKey("$Mods.ElsiNohitMod.Configs.Testing.CombineStats.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Testing.CombineStats.Tooltip")]
            public bool CombineStats = true;

            [ColorNoAlpha]
            [LabelKey("$Mods.ElsiNohitMod.Configs.Testing.StatsColor.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Testing.StatsColor.Tooltip")]
            public Color StatsColor = new Color(41, 236, 166, 99);

            [ColorNoAlpha]
            [LabelKey("$Mods.ElsiNohitMod.Configs.Testing.WarningColor.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Testing.WarningColor.Tooltip")]
            public Color WarningColor = new Color(255, 0, 0, 255);
        }

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Testing.Session.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Testing.Session.Tooltip")]
        public bool NewSession;



        [Header("Gameplay")]

        
        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Gameplay.Buffs.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Gameplay.Buffs.Tooltip")]
        public bool PermanentBuffs;

        [Range(1, 9999)]
        [DefaultValue(30)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Gameplay.BuffsThreshold.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Gameplay.BuffsThreshold.Tooltip")]
        public int BuffsThreshold;

        /*
        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Gameplay.MapTeleport.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Gameplay.MapTeleport.Tooltip")]
        public bool MapTeleport;*/

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Gameplay.DisableAftermath.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Gameplay.DisableAftermath.Tooltip")]
        public bool DisableAftermath;

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Gameplay.InfiniteConsumables.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Gameplay.InfiniteConsumables.Tooltip")]
        public bool InfiniteConsumables;

        [Range(1, 9999)]
        [DefaultValue(999)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Gameplay.ConsumablesThreshold.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Gameplay.ConsumablesThreshold.Tooltip")]
        public int ConsumablesThreshold;

        /*
        //todo, except ui is evil
        [DefaultValue(typeof(Color), "12, 21, 110, 255"), ColorHSLSlider(true), ColorNoAlpha]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Gameplay.WindowColor.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Gameplay.WindowColor.Tooltip")]
        public Color WindowColor;*/


        [Header("Nohitting")]


        [Slider, DrawTicks]
        [DefaultValue(TriggerEnum.Disabled)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Nohitting.InstantDeath.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Nohitting.InstantDeath.Tooltip")]
        public TriggerEnum InstantDeath;

        [Slider, DrawTicks]
        [DefaultValue(NohitEnum.Default)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Nohitting.Nohit.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Nohitting.Nohit.Tooltip")]
        public NohitEnum Nohit;

        [Slider, DrawTicks]
        [DefaultValue(DefiledEnum.Disabled)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Nohitting.Defiled.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Nohitting.Defiled.Tooltip")]
        public DefiledEnum Defiled;

        // WORKS: ks, eoc, boc, eow (shockingly), deer, qb, skele, qs, sprime, dest, twins, plant, df, eol, cultist, ml (though very laggy and bugged message) 
        // DOESN'T WORK: wof (hilarious), golem (only seems to have one head)
        // WORKS (cal edition): ds, as
        // DOESN'T WORK (cal edition): exos (summons 2 then no other mechs)
        [DefaultValue(0)]
        [Range(0, 9)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Nohitting.ExtraBosses.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Nohitting.ExtraBosses.Tooltip")]
        public int ExtraBosses;


        [Header("Presets")]


        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Presets.Vanilla.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Presets.Vanilla.Tooltip")]
        public bool Vanilla
        {
            get => !DisableEvents && !DisableSpawns && !DisableTombstones && !DisableFallingStars && !DisableBossDrops && !DisableItems && !DisableCoins && !DisableHearts && !DisableManaStars
                && DespawnSetting == DespawnEnum.Disabled && !OverrideTimer && !RespawnHP && !PermanentBuffs && !DisableAftermath && !InfiniteConsumables
                && InstantDeath == TriggerEnum.Disabled && Defiled == DefiledEnum.Disabled && ExtraBosses == 0;
            set
            {
                if (value)
                {
                    DisableEvents = false;
                    DisableSpawns = false;
                    DisableTombstones = false;
                    DisableFallingStars = false;
                    DisableBossDrops = false;
                    DisableItems = false;
                    DisableCoins = false;
                    DisableHearts = false;
                    DisableManaStars = false;
                    DespawnSetting = DespawnEnum.Disabled;
                    OverrideTimer = false;
                    RespawnHP = false;
                    PermanentBuffs = false;
                    DisableAftermath = false;
                    InfiniteConsumables = false;
                    InstantDeath = TriggerEnum.Disabled;
                    Defiled = DefiledEnum.Disabled;
                    ExtraBosses = 0;
                }
            }
        }

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Presets.QoL.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Presets.QoL.Tooltip")]
        public bool QoL
        {
            get => DespawnSetting == DespawnEnum.OnRespawn && Despawn.DespawnBosses
                && OverrideTimer && RespawnHP && PermanentBuffs && InfiniteConsumables;
            set
            {
                if (value)
                {
                    DespawnSetting = DespawnEnum.OnRespawn;
                    Despawn.DespawnBosses = true;
                    OverrideTimer = true;
                    RespawnHP = true;
                    PermanentBuffs = true;
                    InfiniteConsumables = true;
                }
            }
        }

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Presets.Testing.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Presets.Testing.Tooltip")]
        public bool Testing
        {
            get => DisableEvents && DisableSpawns && DisableTombstones && DisableFallingStars && DisableItems && DisableCoins
                && DespawnSetting == DespawnEnum.OnRespawn && Despawn.DespawnBosses
                && OverrideTimer && RespawnTime == 180 && RespawnHP && !ClearChat
                && ShowFightStats && FightStatistics.ShowTime && FightStatistics.ShowHealth && FightStatistics.ShowSlowdown && FightStatistics.ShowHits
                && PermanentBuffs && DisableAftermath && InfiniteConsumables
                && InstantDeath == TriggerEnum.Disabled && Defiled == DefiledEnum.Disabled && ExtraBosses == 0;
            set
            {
                if (value)
                {
                    DisableEvents = true;
                    DisableSpawns = true;
                    DisableTombstones = true;
                    DisableFallingStars = true;
                    DisableItems = true;
                    DisableCoins = true;
                    DespawnSetting = DespawnEnum.OnRespawn;
                    Despawn.DespawnBosses = true;
                    OverrideTimer = true;
                    RespawnTime = 180;
                    RespawnHP = true;
                    ClearChat = false;
                    ShowFightStats = true;
                    FightStatistics.ShowTime = true;
                    FightStatistics.ShowHealth = true;
                    FightStatistics.ShowSlowdown = true;
                    FightStatistics.ShowHits = true;
                    PermanentBuffs = true;
                    DisableAftermath = true;
                    InfiniteConsumables = true;
                    InstantDeath = TriggerEnum.Disabled;
                    Defiled = DefiledEnum.Disabled;
                    ExtraBosses = 0;
                }
            }
        }

        [DefaultValue(false)]
        [LabelKey("$Mods.ElsiNohitMod.Configs.Presets.Nohitting.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Presets.Nohitting.Tooltip")]
        public bool Nohitting
        {
            get => DisableEvents && DisableSpawns && DisableTombstones && DisableFallingStars && DisableItems && DisableCoins && DisableHearts
                && DespawnSetting == DespawnEnum.OnRespawn && Despawn.DespawnBosses && Despawn.DespawnEnemies && Despawn.DespawnEnemyProj && Despawn.DespawnFriendlyProj && Despawn.DespawnMisc && Despawn.DespawnItems
                && OverrideTimer && RespawnTime == 120 && ClearChat && PermanentBuffs && InfiniteConsumables
                && ShowFightStats && FightStatistics.ShowAttempts == AttemptEnum.Session && FightStatistics.ShowTime && FightStatistics.ShowHealth && FightStatistics.DistinguishSegments && FightStatistics.ShowSlowdown
                && InstantDeath == TriggerEnum.BossesOnly && Nohit == NohitEnum.Default;
            set
            {
                if (value)
                {
                    DisableEvents = true;
                    DisableSpawns = true;
                    DisableTombstones = true;
                    DisableFallingStars = true;
                    DisableItems = true;
                    DisableCoins = true;
                    DisableHearts = true;
                    DespawnSetting = DespawnEnum.OnRespawn;
                    Despawn.DespawnBosses = true;
                    Despawn.DespawnEnemies = true;
                    Despawn.DespawnEnemyProj = true;
                    Despawn.DespawnFriendlyProj = true;
                    Despawn.DespawnMisc = true;
                    Despawn.DespawnItems = true;
                    OverrideTimer = true;
                    RespawnTime = 120;
                    ClearChat = true;
                    ShowFightStats = true;
                    FightStatistics.ShowAttempts = AttemptEnum.Session;
                    FightStatistics.ShowTime = true;
                    FightStatistics.ShowHealth = true;
                    FightStatistics.DistinguishSegments = true;
                    FightStatistics.ShowSlowdown = true;
                    PermanentBuffs = true;
                    InfiniteConsumables = true;
                    InstantDeath = TriggerEnum.BossesOnly;
                    Nohit = NohitEnum.Default;
                }
            }
        }



        private TriggerEnum prevTrigger = TriggerEnum.Disabled;

        private NohitEnum prevNohit = NohitEnum.Default;

        private DefiledEnum prevDefiled = DefiledEnum.Disabled;

        public override void OnChanged()
        {
            if (prevTrigger != Instance.InstantDeath || prevNohit != Nohit || prevDefiled != Defiled)
            {
                prevTrigger = InstantDeath;
                prevNohit = Nohit;
                prevDefiled = Defiled;
                DespawnSystem.Despawner(true);
            }
        }


        // it doesn't make sense to want to change individual death counts for br
        // but it's weird that your death count could exceed your attempt count




        // report inf bug lol



        /// 1.0.1 - Added note for post-Providence Ravager.
        ///       - Made positive food buffs override Malnourishment.
        ///       - Made "New Session on Boss Death" config option actually function.
        ///       - Fixed a bug where Boss Rush would only halfway end if Despawn On Respawn was enabled and the respawn timer was too short.
        ///       - Fixed a bug where rain would not be stopped while events were disabled. Probably.


        // INFERNUM COMPATIBILITY:

        // so, eow p2 and p3 are fucked up - each phase counts as a new boss, however for some reason p3 has both the new ones share the same boss hp lol. i dunno how i should do this tbh



        // codewise - make it so that there's a special list for what should count as part of the main boss rather than a separate segment list (paladins, defender+healer, etc)



        // infernum uses a special static function to spawn projectiles sometimes which has absolutely no way to track the source! amazing
        // btw when this gets fixed revisit hardcoded brother spawn code for inf

        // diverges timewise from nycros if you die without despawning enabled...


        // astra pet
        // primordial artifact from midnights - lets you choose br start position
        // fix multiboss attempt counts
        // calamity config shouldn't be above normal config
        // look into practice mode and practice godmode
        // custom boss death messages
        // heros mod tp
        // Reforging
        // Setting spawnpoint
        // Clearing npcs, projectiles, etc.
        // Make a setting for controlling weather and time of day, instead of just forcing rain and co. into events
        // Setting for stopping thorn growth would be nice
        // Preventing use of event spawn items to begin with would be nice, as well as random weather events, but that's for later
    }

    public class CalamityConfig : ModConfig
{
public override bool Autoload(ref string name)
{
    if (ModLoader.HasMod("CalamityMod"))
    {
        return true;
    }
    return false;
}

public static CalamityConfig Instance;

public override ConfigScope Mode => ConfigScope.ClientSide;

[Header("CalamityNohitting")]

[DefaultValue(true)]
[LabelKey("$Mods.ElsiNohitMod.Configs.Calamity.DebuffInstakill.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Calamity.DebuffInstakill.Tooltip")]
public bool DebuffInstakill;

[DefaultValue(false)]
[LabelKey("$Mods.ElsiNohitMod.Configs.Calamity.SkipTerminus.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Calamity.SkipTerminus.Tooltip")]
public bool SkipTerminus;

[DefaultValue(false)]
[LabelKey("$Mods.ElsiNohitMod.Configs.Calamity.BRKiller.Label"), TooltipKey("$Mods.ElsiNohitMod.Configs.Calamity.BRKiller.Tooltip")]
public bool BRKiller;
}
}
