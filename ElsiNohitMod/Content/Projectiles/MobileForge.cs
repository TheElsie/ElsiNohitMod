using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;



namespace ElsiNohitMod.Content.Projectiles
{
    internal class MobileForge : ModProjectile
    {
        private static Asset<Texture2D> highlight;
        // Hooks
        public override void Load()
        {
            highlight = Mod.Assets.Request<Texture2D>("Content/Projectiles/MobileForge_Highlight");

            On_Projectile.TryGetContainerIndex += MobileForgeIndex;
            On_Main.TryInteractingWithMoneyTrough += InteractingWithForge;
        }
        public override void SetDefaults()
        {
            Projectile.width = 48;
            Projectile.height = 64;
            Projectile.aiStyle = ProjAIStyleID.FlyingPiggyBank;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 10800;
            Projectile.hide = true;
        }

        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 6;
        }

        // Basically, recreated piggy bank AI but without the cosmetic stuff
        public override bool PreAI()
        {
            Projectile.hide = false;
            Projectile.direction = 1;
            if (++Projectile.frameCounter >= 10)
            {
                Projectile.frameCounter = 0;
                Projectile.frame = ++Projectile.frame % Main.projFrames[Projectile.type];
                for (int k = 0; k < 4; k++)
                {
                    if (Main.rand.Next(2) != 0)
                    {
                        Dust dust = Dust.NewDustDirect(new Vector2(Projectile.position.X + 6, Projectile.position.Y + 46), 36, 8, DustID.Cloud);
                        dust.noGravity = true;
                        dust.alpha = 140;
                        dust.fadeIn = 1.2f;
                        dust.velocity = Vector2.Zero;
                    }
                }
            }

            Main.CurrentFrameFlags.HadAnActiveInteractibleProjectile = true;

            if (Projectile.owner == Main.myPlayer)
            {
                for (int i = 0; i < 1000; i++)
                {
                    if (i != Projectile.whoAmI && Main.projectile[i].active && Main.projectile[i].owner == Projectile.owner && Main.projectile[i].type == Projectile.type)
                    {
                        if (Projectile.timeLeft >= Main.projectile[i].timeLeft)
                        {
                            Main.projectile[i].Kill();
                        }
                        else
                        {
                            Projectile.Kill();
                        }
                    }
                }
            }

            if (Projectile.ai[0] == 0f)
            {
                if ((double)Projectile.velocity.Length() < 0.1)
                {
                    Projectile.velocity.X = 0f;
                    Projectile.velocity.Y = 0f;
                    Projectile.ai[0] = 1f;
                }
                Projectile.velocity *= 0.94f;
            }

            Dust dust2 = Dust.NewDustDirect(new Vector2(Projectile.position.X + 4, Projectile.position.Y + 56), 40, 8, DustID.Cloud, 0, 0, 222, Color.Green);
            dust2.fadeIn = 2f;
            dust2.velocity = new Vector2(0f, 0.2f);
            return false;
        }

        

        // Quick stacking
        private static bool MobileForgeIndex(On_Projectile.orig_TryGetContainerIndex orig, Projectile self, out int containerIndex)
        {
            if (self.type == ModContent.ProjectileType<MobileForge>())
            {
                containerIndex = -4;
                return true;
            }
            else
            {
                return orig(self, out containerIndex);
            }
        }


        // Trick the game into thinking it's working with a piggy bank, but it's secretly one of our projectiles!
        private static int InteractingWithForge(On_Main.orig_TryInteractingWithMoneyTrough orig, Projectile proj)
        {
            bool pre = Main.LocalPlayer.chest == -2;
            int returnVal = orig(proj);

            if (proj.type == ModContent.ProjectileType<MobileForge>())
            {                
                // Copying this directly bc idc
                Matrix matrix = Matrix.Invert(Main.GameViewMatrix.ZoomMatrix);
                Vector2 position = Main.ReverseGravitySupport(Main.MouseScreen);
                Vector2.Transform(Main.screenPosition, matrix);
                Vector2 v = Vector2.Transform(position, matrix) + Main.screenPosition;
                if (proj.Hitbox.Contains(v.ToPoint()))
                {
                    Main.LocalPlayer.cursorItemIconID = ItemID.DefendersForge;
                }
                if (Main.LocalPlayer.chest == -2 && !pre)
                {
                    Main.LocalPlayer.chest = -4;
                }
            }

            return returnVal;
        }
    }
}
