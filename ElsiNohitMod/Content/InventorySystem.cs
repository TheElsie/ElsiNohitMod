using System;
using System.Collections.Generic;
using System.Linq;
using CalamityMod.Buffs.StatDebuffs;
using ElsiNohitMod.Content.BossTracking;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;



namespace ElsiNohitMod.Content
{
    public class InventorySystem : ModPlayer
    {

        public List<int> permaBuffs = new List<int>();



        // Prevents ammo consumption
        public override void OnConsumeAmmo(Item weapon, Item ammo)
        {
            if (TheConfigForThisMod.Instance.InfiniteConsumables && ammo.stack >= TheConfigForThisMod.Instance.ConsumablesThreshold)
            {
                ammo.stack++;
            }
        }



        // Permanent buffs
        public static bool Malnourished;

        public override void PreUpdateBuffs()
        {
            if (TheConfigForThisMod.Instance.PermanentBuffs)
            {
                Malnourished = false;
                for (int i = 0; i != 58; i++)
                {
                    Item item = Player.inventory[i];
                    InventoryBuffs(item, this);

                    if (i < Chest.maxItems)
                    {
                        item = Player.bank.item[i];
                        InventoryBuffs(item, this);

                        item = Player.bank2.item[i];
                        InventoryBuffs(item, this);

                        item = Player.bank3.item[i];
                        InventoryBuffs(item, this);

                        item = Player.bank4.item[i];
                        InventoryBuffs(item, this);
                    }
                }
                foreach (int i in permaBuffs.ToList())
                {
                    int index = Array.IndexOf(Player.buffType, i);
                    if (index < 0 || Player.buffTime[index] < 999999)
                    {
                        Player.ClearBuff(i);
                        permaBuffs.Remove(i);
                        Main.buffNoTimeDisplay[i] = false;
                    }
                }

                if (Malnourished && !Player.wellFed)
                {
                    Player.AddBuff(CalamityID.Malnourished, 999999);
                    if (!permaBuffs.Contains(CalamityID.Malnourished))
                    {
                        permaBuffs.Add(CalamityID.Malnourished);
                        Main.buffNoTimeDisplay[CalamityID.Malnourished] = true;
                    }

                }
            }
        }

        // If selected item is a potion or a buff station, give buff. If more buffs need to be added add here
        public static void InventoryBuffs(Item item, InventorySystem self)
        {
            if (item.stack >= TheConfigForThisMod.Instance.BuffsThreshold)
            {
                Player player = self.Player;
                if (item.buffType != 0)
                {
                    if (item.buffType == CalamityID.Malnourished)
                    {
                        Malnourished = true;
                        return;
                    }

                    player.AddBuff(item.buffType, 999999);
                    if (!self.permaBuffs.Contains(item.buffType))
                    {
                        self.permaBuffs.Add(item.buffType);
                        Main.buffNoTimeDisplay[item.buffType] = true;
                    }
                }
                else if (item.type == ItemID.CrystalBall) { player.AddBuff(BuffID.Clairvoyance, 999999); }
                else if (item.type == ItemID.SliceOfCake) { player.AddBuff(BuffID.SugarRush, 999999); }
                else if (item.type == ItemID.WarTable) { player.AddBuff(BuffID.WarTable, 999999); }
                else if (item.type == ItemID.SharpeningStation) { player.AddBuff(BuffID.Sharpened, 999999); }
                else if (item.type == ItemID.BewitchingTable) { player.AddBuff(BuffID.Bewitched, 999999); }
                else if (item.type == ItemID.AmmoBox) { player.AddBuff(BuffID.AmmoBox, 999999); }
                //if i need to add more, do so right here
            }
        }
    }



    // Prevents item & bait consumption
    public class Consum : GlobalItem
    {
        public override void OnConsumeItem(Item item, Player player)
        {
            if (TheConfigForThisMod.Instance.InfiniteConsumables)
            {
                if (item.stack >= TheConfigForThisMod.Instance.ConsumablesThreshold) item.stack++;
            }
        }

        public override bool? CanConsumeBait(Player player, Item bait)
        {
            if (TheConfigForThisMod.Instance.InfiniteConsumables)
            {
                return false;
            }
            return base.CanConsumeBait(player, bait);
        }
    }
}
