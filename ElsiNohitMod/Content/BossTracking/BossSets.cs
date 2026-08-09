using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.NPCs.Ravager;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;



namespace ElsiNohitMod.Content.BossTracking
{
    [ReinitializeDuringResizeArrays]
    public static class BossSets
    {
        private static SetFactory Factory = NPCID.Sets.Factory;

        // Checks to see if the given type should be considered "part" of a boss.
        public static bool Phase(int type)
        {
            if (BossPhase[type]) return true;
            if (BossPhaseCalamity[type]) return true;
            if (ElsiNohitMod.InfernumActive() && BossPhaseInfernum[type]) return true;
            return false;
        }

        public static bool[] BossPhase = Factory.CreateNamedSet("ElsiNohitMod/BossPhase").RegisterBoolSet
        (
            NPCID.Creeper,
            NPCID.EaterofWorldsBody,
            NPCID.EaterofWorldsTail,
            NPCID.SkeletronHand,
            NPCID.PrimeCannon,
            NPCID.PrimeLaser,
            NPCID.PrimeSaw,
            NPCID.PrimeVice,
            NPCID.GolemFistLeft,
            NPCID.GolemFistRight,
            NPCID.GolemHead
        );

        public static bool[] BossPhaseCalamity = (ElsiNohitMod.CalamityLoaded) ? Factory.CreateNamedSet("ElsiNohitMod/BossPhaseCalamity").RegisterBoolSet
        (
            ElsiNohitMod.Calamity.Find<ModNPC>("DesertNuisanceHead").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("DesertNuisanceHeadYoung").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("KingSlimeJewelRuby").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("PerforatorHeadSmall").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("PerforatorHeadMedium").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("PerforatorBodyMedium").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("PerforatorTailMedium").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("PerforatorHeadLarge").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("CryogenShield").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("Cataclysm").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("Catastrophe").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("SoulSeeker").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("AnahitasIceShield").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("RavagerClawLeft").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("RavagerClawRight").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("RavagerHead").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("RavagerLegLeft").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("RavagerLegRight").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("ProfanedGuardianHealer").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("ProfanedGuardianDefender").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("ProvSpawnHealer").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("ProvSpawnDefense").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("ProvSpawnOffense").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("DarkEnergy").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("PolterPhantom").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("BrimstoneHeart").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("SupremeCataclysm").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("SupremeCatastrophe").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("SoulSeekerSupreme").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("CrimulanPaladin").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("EbonianPaladin").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("SplitCrimulanPaladin").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("SplitEbonianPaladin").Type
        ) : Factory.CreateNamedSet("ElsiNohitMod/BossPhaseCalamity").RegisterBoolSet();

        public static bool[] BossPhaseInfernum = (ElsiNohitMod.CalamityLoaded && ElsiNohitMod.InfernumLoaded) ? Factory.CreateNamedSet("ElsiNohitMod/BossPhaseInfernum").RegisterBoolSet
        (
            NPCID.WallofFleshEye,
            ElsiNohitMod.Calamity.Find<ModNPC>("GreatSandShark").Type,
            ElsiNohitMod.Infernum.Find<ModNPC>("HealerShieldCrystal").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("SepulcherHead").Type
        ) : Factory.CreateNamedSet("ElsiNohitMod/BossPhaseInfernum").RegisterBoolSet();

        

        // This is so that addons and such can prevent given types from being considered "part" of the boss.
        public static bool Blacklist(int type)
        {
            if (ElsiNohitMod.InfernumActive() && InfernumBlacklist[type]) return true;
            return false;
        }
        
        public static bool[] InfernumBlacklist = (ElsiNohitMod.CalamityLoaded && ElsiNohitMod.InfernumLoaded) ? Factory.CreateNamedSet("ElsiNohitMod/InfernumBlacklist").RegisterBoolSet
        (
            NPCID.Creeper,
            NPCID.SkeletronHand,
            NPCID.EaterofWorldsBody,
            NPCID.EaterofWorldsTail,
            ElsiNohitMod.Calamity.Find<ModNPC>("PerforatorBodyMedium").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("PerforatorTailMedium").Type,
            NPCID.PrimeLaser,
            NPCID.PrimeSaw,
            NPCID.PrimeVice,
            ElsiNohitMod.Calamity.Find<ModNPC>("Catastrophe").Type,
            NPCID.GolemFistLeft,
            NPCID.GolemFistRight,
            NPCID.GolemHead,
            ElsiNohitMod.Calamity.Find<ModNPC>("RavagerClawRight").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("RavagerLegRight").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("ProvSpawnHealer").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("ProvSpawnDefense").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("ProvSpawnOffense").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("SupremeCatastrophe").Type
        ) : Factory.CreateNamedSet("ElsiNohitMod/InfernumBlacklist").RegisterBoolSet();



        // This is for things that are labeled as a boss but shouldn't be tracked as one.
        public static bool NotABoss(int type)
        {
            if (NotABossCalamity[type]) return true;
            if (ElsiNohitMod.InfernumActive() && NotABossInfernum[type]) return true;
            return false;
        }

        // This is for anything labeled as a boss that shouldn't be tracked as one.
        public static bool[] NotABossCalamity = (ElsiNohitMod.CalamityLoaded) ? Factory.CreateNamedSet("ElsiNohitMod/NotABossCalamity").RegisterBoolSet
        (
            ElsiNohitMod.Calamity.Find<ModNPC>("DesertScourgeBody").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("DesertScourgeTail").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("FalseBrain").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("BrainIllusion").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("AstrumDeusBody").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("AstrumDeusTail").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("StormWeaverBody").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("StormWeaverTail").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("DevourerofGodsBody").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("DevourerofGodsTail").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("AresGaussNuke").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("AresLaserCannon").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("AresPlasmaFlamethrower").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("AresTeslaCannon").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("ThanatosBody1").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("ThanatosBody2").Type,
            ElsiNohitMod.Calamity.Find<ModNPC>("ThanatosTail").Type
        ) : Factory.CreateNamedSet("ElsiNohitMod/NotABossCalamity").RegisterBoolSet();

        public static bool[] NotABossInfernum = (ElsiNohitMod.CalamityLoaded && ElsiNohitMod.InfernumLoaded) ? Factory.CreateNamedSet("ElsiNohitMod/NotABossInfernum").RegisterBoolSet
        (
            ElsiNohitMod.Infernum.Find<ModNPC>("AresEnergyKatana").Type
        ) : Factory.CreateNamedSet("ElsiNohitMod/NotABossInfernum").RegisterBoolSet();



        // Returns the index of the proper "phase name" for the given boss. A bit screwy for bosses like SCal with multiple, but that's an issue for later.
        public static int PhasePointer(int type)
        {
            int check = PhaseNamePointer[type];
            if (check != -1) return check;

            check = CalPhaseNamePointer[type];
            if (check != -1) return check;

            if (ElsiNohitMod.InfernumActive())
            {
                check = InfPhaseNamePointer[type];
                if (check != -1) return check;
            }

            return 0;
        }

        // Make sure to always use the boss type, not the segment type.
        public static int[] PhaseNamePointer = Factory.CreateNamedSet("ElsiNohitMod/PhaseNamePointer").RegisterIntSet
        (-1,
            NPCID.BrainofCthulhu, 1,
            NPCID.SkeletronHead, 2,
            NPCID.SkeletronPrime, 3,
            NPCID.Golem, 4
        );

        public static int[] CalPhaseNamePointer = (ElsiNohitMod.CalamityLoaded) ? Factory.CreateNamedSet("ElsiNohitMod/CalPhaseNamePointer").RegisterIntSet
        (-1,
            ElsiNohitMod.Calamity.Find<ModNPC>("DesertScourgeHead").Type, 5,
            NPCID.KingSlime, 6,
            ElsiNohitMod.Calamity.Find<ModNPC>("PerforatorHive").Type, 7,
            ElsiNohitMod.Calamity.Find<ModNPC>("Cryogen").Type, 8,
            ElsiNohitMod.Calamity.Find<ModNPC>("CalamitasClone").Type, 9,
            ElsiNohitMod.Calamity.Find<ModNPC>("SoulSeeker").Type, 18,
            ElsiNohitMod.Calamity.Find<ModNPC>("Anahita").Type, 10,
            ElsiNohitMod.Calamity.Find<ModNPC>("RavagerBody").Type, 11,
            ElsiNohitMod.Calamity.Find<ModNPC>("ProfanedGuardianCommander").Type, 12,
            ElsiNohitMod.Calamity.Find<ModNPC>("Providence").Type, 13,
            ElsiNohitMod.Calamity.Find<ModNPC>("CeaselessVoid").Type, 14,
            ElsiNohitMod.Calamity.Find<ModNPC>("Polterghast").Type, 15,
            ElsiNohitMod.Calamity.Find<ModNPC>("BrimstoneHeart").Type, 16,
            ElsiNohitMod.Calamity.Find<ModNPC>("SupremeCalamitas").Type, 17,
            ElsiNohitMod.Calamity.Find<ModNPC>("SoulSeekerSupreme").Type, 18
        ) : Factory.CreateNamedSet("ElsiNohitMod/CalPhaseNamePointer").RegisterIntSet(0);

        public static int[] InfPhaseNamePointer = (ElsiNohitMod.CalamityLoaded && ElsiNohitMod.InfernumLoaded) ? Factory.CreateNamedSet("ElsiNohitMod/InfPhaseNamePointer").RegisterIntSet
        (-1,
            ElsiNohitMod.Calamity.Find<ModNPC>("SlimeGodCore").Type, 19,
            NPCID.WallofFlesh, 20,
            ElsiNohitMod.Infernum.Find<ModNPC>("BereftVassal").Type, 21,
            ElsiNohitMod.Calamity.Find<ModNPC>("SepulcherHead").Type, 23
        ) : Factory.CreateNamedSet("ElsiNohitMod/InfPhaseNamePointer").RegisterIntSet(0);

        // The phase names that the index points to.
        public static string[] PhaseNames =
        {
            "Placeholder",              // 0
            "Creepers",                 // 1
            "Arms",                     // 2
            "Arms",                     // 3
            "Limbs",                    // 4
            "Nuisances",                // 5
            "Jewel",                    // 6
            "Perforator",               // 7
            "Ice Shield",               // 8
            "Constructs",               // 9
            "Ice Shield",               // 10
            "Limbs",                    // 11
            "Shield",                  // 12
            "Guardians",                // 13
            "Dark Energy",              // 14
            "Polterghast Clone",        // 15
            "Hearts",                   // 16
            "Brothers",                 // 17
            "Seekers",                  // 18
            "Paladins",                 // 19
            "Eyes",                     // 20
            "Taurus",                   // 21
            "Shadows",                  // 22
            "Sepulcher"                 // 23
        };
    }

    public class CalamityID : ModSystem
    {
        public static int DesertScourgeHead = -1;
        public static int DesertNuisanceHead = -1;
        public static int DesertNuisanceHeadYoung = -1;
        public static int SlimeGod = -1;
        public static int AquaticScourgeHead = -1;
        public static int Leviathan = -1;
        public static int Anahita = -1;
        public static int Ravager = -1;
        public static int AstrumDeusHead = -1;
        public static int GuardianHealer  = -1;
        public static int GuardianDefender  = -1;
        public static int GuardianCommander = -1;
        public static int Providence  = -1;
        public static int CeaselessVoid  = -1;
        public static int DevourerofGodsHead  = -1;
        public static int Ares = -1;
        public static int Thanatos = -1;
        public static int Artemis = -1;
        public static int Apollo = -1;
        public static int SupremeCalamitas = -1;
        public static int SupremeCataclysm = -1;
        public static int SupremeCatastrophe = -1;

        public static int HolyInferno = -1;
        public static int VulnHex = -1;

        public static int Malnourished = -1;

        public static int BREndProj = -1;


        public static int Argus = -1;
        public static int Signus = -1;
        public static int SepulcherHead = -1;
        public static int SoulSeekerSupreme = -1;
        public static int PrimordialWyrm = -1;
        public override void SetStaticDefaults()
        {
            if (ElsiNohitMod.CalamityLoaded)
            {
                DesertScourgeHead = ElsiNohitMod.Calamity.Find<ModNPC>("DesertScourgeHead").Type;
                DesertNuisanceHead = ElsiNohitMod.Calamity.Find<ModNPC>("DesertNuisanceHead").Type;
                DesertNuisanceHeadYoung = ElsiNohitMod.Calamity.Find<ModNPC>("DesertNuisanceHeadYoung").Type;

                SlimeGod = ElsiNohitMod.Calamity.Find<ModNPC>("SlimeGodCore").Type;

                AquaticScourgeHead = ElsiNohitMod.Calamity.Find<ModNPC>("AquaticScourgeHead").Type;

                Leviathan = ElsiNohitMod.Calamity.Find<ModNPC>("Leviathan").Type;
                Anahita = ElsiNohitMod.Calamity.Find<ModNPC>("Anahita").Type;

                Ravager = ElsiNohitMod.Calamity.Find<ModNPC>("RavagerBody").Type;

                AstrumDeusHead = ElsiNohitMod.Calamity.Find<ModNPC>("AstrumDeusHead").Type;

                GuardianHealer = ElsiNohitMod.Calamity.Find<ModNPC>("ProfanedGuardianHealer").Type;
                GuardianDefender = ElsiNohitMod.Calamity.Find<ModNPC>("ProfanedGuardianDefender").Type;
                GuardianCommander = ElsiNohitMod.Calamity.Find<ModNPC>("ProfanedGuardianCommander").Type;

                Providence = ElsiNohitMod.Calamity.Find<ModNPC>("Providence").Type;

                CeaselessVoid = ElsiNohitMod.Calamity.Find<ModNPC>("CeaselessVoid").Type;

                DevourerofGodsHead = ElsiNohitMod.Calamity.Find<ModNPC>("DevourerofGodsHead").Type;

                Ares = ElsiNohitMod.Calamity.Find<ModNPC>("AresBody").Type;
                Thanatos = ElsiNohitMod.Calamity.Find<ModNPC>("ThanatosHead").Type;
                Artemis = ElsiNohitMod.Calamity.Find<ModNPC>("Artemis").Type;
                Apollo = ElsiNohitMod.Calamity.Find<ModNPC>("Apollo").Type;
                SupremeCalamitas = ElsiNohitMod.Calamity.Find<ModNPC>("SupremeCalamitas").Type;
                SupremeCataclysm = ElsiNohitMod.Calamity.Find<ModNPC>("SupremeCataclysm").Type;
                SupremeCatastrophe = ElsiNohitMod.Calamity.Find<ModNPC>("SupremeCatastrophe").Type;


                HolyInferno = ElsiNohitMod.Calamity.Find<ModBuff>("HolyInferno").Type;
                VulnHex = ElsiNohitMod.Calamity.Find<ModBuff>("VulnerabilityHex").Type;

                Malnourished = ElsiNohitMod.Calamity.Find<ModBuff>("Malnourished").Type;

                BREndProj = ElsiNohitMod.Calamity.Find<ModProjectile>("BossRushFailureEffectThing").Type;

                if (ElsiNohitMod.InfernumLoaded)
                {
                    Argus = ElsiNohitMod.Infernum.Find<ModNPC>("BereftVassal").Type;

                    Signus = ElsiNohitMod.Calamity.Find<ModNPC>("Signus").Type;

                    SepulcherHead = ElsiNohitMod.Calamity.Find<ModNPC>("SepulcherHead").Type;
                    SoulSeekerSupreme = ElsiNohitMod.Calamity.Find<ModNPC>("SoulSeekerSupreme").Type;

                    PrimordialWyrm = ElsiNohitMod.Calamity.Find<ModNPC>("PrimordialWyrmHead").Type;
                }
            }
        }

    }
}
