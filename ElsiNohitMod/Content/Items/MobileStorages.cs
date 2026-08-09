using System.Reflection;
using ElsiNohitMod.Content.Projectiles;
using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace ElsiNohitMod.Content.Items
{
    internal class MobileStorages : ModItem
    {
        private static int safeType => ModContent.ProjectileType<MobileSafe>();
        private static int forgeType => ModContent.ProjectileType<MobileForge>();

        // Hooks
        public override void Load()
        {
            On_Projectile.IsInteractible += YeahItIs;
            On_Main.DrawProj_DrawNormalProjs += DontSkipThis;
        }

        // Recipe because why not
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe()
                .AddIngredient(ItemID.MoneyTrough)
                .AddIngredient(ItemID.Safe)
                .AddIngredient(ItemID.DefendersForge)
                .AddIngredient(ItemID.VoidLens)
                .AddTile(TileID.TinkerersWorkbench)
                .Register();
        }

        public override void SetDefaults()
        {
            Item.useStyle = 1;
            Item.shootSpeed = 4f;
            Item.shoot = ProjectileID.FlyingPiggyBank;
            Item.width = 24;
            Item.height = 24;
            Item.UseSound = SoundID.Item130;
            Item.useAnimation = 28;
            Item.useTime = 28;
            Item.rare = 3;
            Item.value = Item.sellPrice(0, 2);
        }

        // Returning false skips vanilla code
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Vector2 offset = new Vector2(2, 2);
            Projectile.NewProjectile(source, position, offset, ProjectileID.VoidLens, 0, 0);
            Projectile.NewProjectile(source, position, -offset, forgeType, 0, 0);
            offset = new Vector2(2, -2);
            Projectile.NewProjectile(source, position, -offset, ProjectileID.FlyingPiggyBank, 0, 0);
            Projectile.NewProjectile(source, position, offset, safeType, 0, 0);
            return false;
        }



        // I tried replacing the original factory set but it didn't work for whatever reason
        private static bool YeahItIs(On_Projectile.orig_IsInteractible orig, Projectile self)
        {
            if (self.type == safeType || self.type == forgeType)
            {
                return true;
            }
            else
            {
                return orig(self);
            }
        }



        // The logic for money trough and void bag happens in the function we hook. However, the logic terminates the function for
        // every projectile that ISN'T those two before it runs. We need to run it, so we use reflection, since it's private and we
        // can't invoke it normally.
        private static void DontSkipThis(On_Main.orig_DrawProj_DrawNormalProjs orig, Main self, Projectile proj, float poleX, float poleY, Vector2 center, ref Color color)
        {
            orig(self, proj, poleX, poleY, center, ref color);
            if (proj.type == safeType || proj.type == forgeType)
            {
                MoneyTroughOverride.Invoke(null, [proj]);
            }
        }

        private static MethodInfo MoneyTroughOverride => typeof(Main).GetMethod("TryInteractingWithMoneyTrough", BindingFlags.NonPublic | BindingFlags.Static);
    }
}
