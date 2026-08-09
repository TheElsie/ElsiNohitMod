using ElsiNohitMod.Content.BossTracking;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;



namespace ElsiNohitMod.Content
{
    public class DespawnSystem : ModPlayer
    {
        // Despawn on death
        public override void Kill(double damage, int direction, bool pvp, PlayerDeathReason source)
        {
            if (TheConfigForThisMod.Instance.DespawnSetting == TheConfigForThisMod.DespawnEnum.OnDeath) Despawner();
        }

        // Despawn on respawn
        public override void OnRespawn()
        {
            if (TheConfigForThisMod.Instance.DespawnSetting == TheConfigForThisMod.DespawnEnum.OnRespawn)
            {
                Despawner();
            }

        }

        // Despawn logic
        public static void Despawner(bool forceBosses = false)
        {
            bool[] settings =
            {
                TheConfigForThisMod.Instance.Despawn.DespawnBosses || forceBosses,
                TheConfigForThisMod.Instance.Despawn.DespawnEnemies,
                TheConfigForThisMod.Instance.Despawn.DespawnFriendlies,
                TheConfigForThisMod.Instance.Despawn.DespawnEnemyProj,
                TheConfigForThisMod.Instance.Despawn.DespawnFriendlyProj,
                TheConfigForThisMod.Instance.Despawn.DespawnMisc,
                TheConfigForThisMod.Instance.Despawn.DespawnItems
            };

            foreach (NPC npc in Main.ActiveNPCs)
            {
                if (npc.boss) { if (settings[0]) { npc.active = false; } }
                else
                {
                    if (npc.friendly || npc.lifeMax <= 5)
                    {
                        if (settings[2]) { npc.active = false; }
                    }
                    else
                    {
                        if (settings[1]) { npc.active = false; }
                    }
                }
            }

            foreach (Projectile proj in Main.ActiveProjectiles)
            {
                if (proj.type != CalamityID.BREndProj)
                {
                    if (proj.hostile)
                    {
                        if (settings[3]) { proj.active = false; }
                    }
                    else
                    {
                        if (settings[4]) { proj.active = false; }
                    }
                }
            }

            if (settings[5])
            {
                foreach (Dust dust in Main.dust) { dust.active = false; }
                foreach (Gore gore in Main.gore) { gore.active = false; }
                foreach (CombatText text in Main.combatText)
                {
                    if (text.color != CombatText.DamagedFriendly && text.color != CombatText.DamagedFriendlyCrit && text.color != CombatText.LifeRegenNegative)
                    {
                        text.active = false;
                    }
                }
            }

            if (settings[6])
            {
                foreach (Item item in Main.ActiveItems) { item.active = false; }
            }

            if (forceBosses)
            {

                if (BossSystem.BossRushActive())
                {
                    ElsiNohitMod.Calamity.Call("SetDifficultyActive", "BossRush", false);
                    Main.NewText("Nohit settings can't be changed during Boss Rush.", TheConfigForThisMod.Instance.FightStatistics.WarningColor);
                }
                else if (BossSystem.BossAlive)
                {
                    Main.NewText("Nohit settings can't be changed during boss fights.", TheConfigForThisMod.Instance.FightStatistics.WarningColor);
                }
            }
        }
    }
}
