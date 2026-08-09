using System;
using System.Collections.Generic;
using System.Reflection;
using CalamityMod.Projectiles.Typeless;
using ElsiNohitMod.Content.BossTracking;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoMod.RuntimeDetour;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;



namespace ElsiNohitMod.Content
{
    public class RuntimeDetours : ModSystem
    {
        // Hooks must be stored in a static list, otherwise they will considered garbage data
        private static List<Hook> Hooks = new List<Hook>();

        // Adds hooks
        public override void Load()
        {

            Hooks.Add(new Hook(PreKill, PenetrateRevives));

            if (ModLoader.HasMod("CalamityMod"))
            {
                Hooks.Add(new Hook(AcidRainEvent, AcidRain));
                Hooks.Add(new Hook(TerminusSetDefaults, SkipIntro));
                Hooks.Add(new Hook(DrawProgressText, BRProgress));
                Hooks.Add(new Hook(CreateTierAnimation, BRTier));
            }
        }



        // This is to make sure instakill penetrates all revives. Don't need a weak reference here
        public MethodInfo PreKill => typeof(PlayerLoader).GetMethod("PreKill", BindingFlags.Public | BindingFlags.Static);

        private delegate bool orig_PreKill(Player player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource);

        // Hooks the method that loads hooks, lmao. If any of the hooks return false (aka they cancelled the death) and
        // the player is above 0 hp (they got revived), kill them anyways
        private static bool PenetrateRevives(orig_PreKill orig, Player player, double damage, int hitDirection, bool pvp, ref bool playSound, ref bool genGore, ref PlayerDeathReason damageSource)
        {
            bool KillFailed = orig(player, damage, hitDirection, pvp, ref playSound, ref genGore, ref damageSource);
            if (!KillFailed && player.statLife > 0)
            {
                if (TheConfigForThisMod.Instance.Nohit != TheConfigForThisMod.NohitEnum.NoDamage && (TheConfigForThisMod.Instance.InstantDeath == TheConfigForThisMod.TriggerEnum.Everything || (TheConfigForThisMod.Instance.InstantDeath == TheConfigForThisMod.TriggerEnum.BossesOnly && BossSystem.BossAlive)))
                {
                    player.statLife = 0;
                    return true;
                }
            }
            return KillFailed;
        }



        // This is a weak reference to Calamity. For it to function, there needs to be the line "weakReference = CalamityMod" in build.txt
        // [JITWhenModsEnabled()] prevents tmod from crashing on load if Calamity is unloaded. This is necessary for weak references to work. Do not omit this.
        [JITWhenModsEnabled("CalamityMod")]
        public MethodInfo AcidRainEvent => typeof(CalamityMod.Events.AcidRainEvent).GetMethod("TryStartEvent", BindingFlags.Public | BindingFlags.Static);

        // Name of this function doesn't matter, only the parameters
        private delegate void orig_TryStartEvent(bool forceRain);

        // Standard hook. Prevents Acid Rain
        private static void AcidRain(orig_TryStartEvent orig, bool forceRain = false)
        {
            if (!TheConfigForThisMod.Instance.DisableEvents)
            {
                orig(forceRain);
            }
            return;
        }



        // Skips Terminus holdout animation
        [JITWhenModsEnabled("CalamityMod")]
        public MethodInfo TerminusSetDefaults => typeof(CalamityMod.Projectiles.Typeless.TerminusHoldout).GetMethod("SetDefaults", BindingFlags.Public | BindingFlags.Instance);

        private delegate void orig_TerminusSetDefaults(ModProjectile self);

        private static void SkipIntro(orig_TerminusSetDefaults orig, ModProjectile self)
        {
            orig(self);
            self.Projectile.timeLeft = newLifetime;
        }

        public static int newLifetime => (CalamityConfig.Instance.SkipTerminus && !BossSystem.BossRushActive()) ? 1 : 300;


        // Gets BR progress %. This weak reference allows us to hook the parent function of BossRushUI that it uses for drawing the progress bar
        [JITWhenModsEnabled("CalamityMod")]
        public MethodInfo DrawProgressText => typeof(CalamityMod.UI.InvasionProgressUI).GetMethod("DrawProgressText", BindingFlags.Public | BindingFlags.Instance);

        // From there, we get the property info of the variable we want
        [JITWhenModsEnabled("CalamityMod")]
        public static PropertyInfo CompletionRatio => typeof(CalamityMod.UI.BossRushUI).GetProperty("CompletionRatio", BindingFlags.Public | BindingFlags.Instance);

        public static float BRCompletion = 0;

        private delegate void orig_DrawProgressText(object self, SpriteBatch spriteBatch, float yScale, Vector2 baseBarDrawPosition, int barOffsetY, out Vector2 newBarPosition);

        // Finally, we hook the function and get the value from the object making use of it. However, since Acid Rain also uses this function,
        // we need to use [JITWhenModsEnabled] in order to directly reference the target class. If not, then it will throw and exception on Acid Rain,
        // or fail to load if Calamity isn't enabled.
        [JITWhenModsEnabled("CalamityMod")]
        private static void BRProgress(orig_DrawProgressText orig, object self, SpriteBatch spriteBatch, float yScale, Vector2 baseBarDrawPosition, int barOffsetY, out Vector2 newBarPosition)
        {
            orig(self, spriteBatch, yScale, baseBarDrawPosition, barOffsetY, out newBarPosition);
            if (self is CalamityMod.UI.BossRushUI)
            {
                BRCompletion = (float)CompletionRatio.GetValue(self);
            }
        }



        // This is for getting the current BR tier. Kinda janky, I know.
        [JITWhenModsEnabled("CalamityMod")]
        public static MethodInfo CreateTierAnimation => typeof(CalamityMod.Events.BossRushEvent).GetMethod("CreateTierAnimation", BindingFlags.Public | BindingFlags.Static);

        private delegate void orig_CreateTierAnimation(int tier);

        [JITWhenModsEnabled("CalamityMod")]
        private static void BRTier(orig_CreateTierAnimation orig, int tier)
        {
            CurrentTier = tier;
            orig(tier);
        }

        public static int CurrentTier = 0;



        // Hooks MUST be disposed of, otherwise unload issues may occur
        public override void Unload()
        {
            for (int i = 0; i < Hooks.Count; i++)
            {
                Hooks[i].Dispose();
                Hooks[i] = null;
            }
            Hooks = new List<Hook>();
        }
    }
}

