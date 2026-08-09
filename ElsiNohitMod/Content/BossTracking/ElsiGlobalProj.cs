using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;



namespace ElsiNohitMod.Content.BossTracking
{
    // For tracking whether or not a projectile originates from a boss
    public class ElsiGlobalProj : GlobalProjectile
    {
        public static int[] projOwner = new int[1001];
        public override void OnSpawn(Projectile proj, IEntitySource source)
        {
            projOwner[proj.whoAmI] = -1;
            if (source is EntitySource_Parent { Entity : NPC parent })
            {
                projOwner[proj.whoAmI] = parent.whoAmI;
            }

            if (source is EntitySource_Parent { Entity : Projectile parentProj })
            {
                projOwner[proj.whoAmI] = projOwner[parentProj.whoAmI];
            }
        }
    }
}
