using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;



namespace ElsiNohitMod.Content.Projectiles
{
    internal class MobileSafe : ModProjectile
    {
        public override void Load()
        {
            On_Projectile.TryGetContainerIndex += MobileSafeIndex;
            On_Main.TryInteractingWithMoneyTrough += InteractingWithSafe;
        }
        public override void SetDefaults()
        {
            Projectile.width = 26;
            Projectile.height = 28;
            Projectile.aiStyle = ProjAIStyleID.FlyingPiggyBank;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10800;
            Projectile.hide = true;
        }

        public override void AI()
        {
            Projectile.hide = false;
            Projectile.direction = 1;
            Projectile.spriteDirection = 1;
        }

        // I think this only matters for quick stacking lol
        private static bool MobileSafeIndex(On_Projectile.orig_TryGetContainerIndex orig, Projectile self, out int containerIndex)
        {
            if (self.type == ModContent.ProjectileType<MobileSafe>())
            {
                containerIndex = -3; // defender's forge is -4
                return true;
            }
            else
            {
                return orig(self, out containerIndex);
            }
        }

        // Trick the game into thinking it's working with a piggy bank, but it's secretly one of our projectiles!
        private static int InteractingWithSafe(On_Main.orig_TryInteractingWithMoneyTrough orig, Projectile proj)
        {
            bool pre = Main.LocalPlayer.chest == -2;
            int returnVal = orig(proj);

            if (proj.type == ModContent.ProjectileType<MobileSafe>())
            {            
                // Copying this directly bc idc
                Matrix matrix = Matrix.Invert(Main.GameViewMatrix.ZoomMatrix);
                Vector2 position = Main.ReverseGravitySupport(Main.MouseScreen);
                Vector2.Transform(Main.screenPosition, matrix);
                Vector2 v = Vector2.Transform(position, matrix) + Main.screenPosition;
                if (proj.Hitbox.Contains(v.ToPoint()))
                {
                    Main.LocalPlayer.cursorItemIconID = ItemID.Safe;
                }
                if (Main.LocalPlayer.chest == -2 && !pre)
                {
                    Main.LocalPlayer.chest = -3;
                }
            }
            return returnVal;
        }
    }
}
