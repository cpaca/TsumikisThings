using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using TsumikisThings.Items;

namespace TsumikisThings
{
    internal class TsumikiPlayer : ModPlayer
    {
        SuperModifier modifiers = new();
        internal int summonHealCooldown = 0;
        double fractionalHealth = 0;

        public override void ResetEffects()
        {
            base.ResetEffects();
            modifiers = new SuperModifier();
        }

        public override void PreUpdate()
        {
            base.PreUpdate();
            // reminder that this is called 60 times per second.
            if(summonHealCooldown > 0)
            {
                summonHealCooldown -= 1;
            }
        }

        // Docs say this is called after Update Accessories
        // and this is basically supposed to be UpdateAccessories, just done here so the head super-modifier can cap things
        public override void UpdateEquips()
        {
            base.UpdateEquips();
            modifiers.UpdateAccessory(this);
        }

        public override void OnHitAnything(float x, float y, Entity victim)
        {
            base.OnHitAnything(x, y, victim);
            modifiers.OnHitAnything(this);
        }

        public override bool CanConsumeAmmo(Item weapon, Item ammo)
        {
            return modifiers.CanConsumeAmmo();
        }

        public override void ModifyItemScale(Item item, ref float scale)
        {
            modifiers.ModifyItemScale(ref scale);
        }

        public void AddModifier(SuperModifier modifier)
        {
            modifiers.Add(modifier);
        }

        // For counting the number of GuaranteedSuperMods
        // since I'm not sure when UpdateInventory is called and/or reset
        internal int CountGuaranteedSuperMods()
        {
            Item[] inv = Player.inventory;
            int numSuperMods = 0;
            foreach(Item item in inv)
            {
                ModItem modItem = item.ModItem;
                if(modItem == null)
                {
                    continue;
                }
                if(modItem is GuaranteedSuperMods)
                {
                    // this item is a guarnteedSuperMods:
                    numSuperMods += item.stack;
                    // hard cap at 8
                    if(numSuperMods > 8)
                    {
                        return 8;
                    }
                }
            }
            return numSuperMods;
        }

        internal double CalcReforgeMult()
        {
            // 2.5x since 50% (1/2) chance of super mod
            int numSuperMods = CountGuaranteedSuperMods();
            bool discountCard = Player.discount;
            // TsumikisThings.GetLogger().Debug("ApplyDiscount state: " + discountCard);
            // Did it this way so I could log things more easily.
            double ret;
            if (discountCard)
            {
                ret = Math.Pow(1.75, (double) numSuperMods);
            }
            else
            {
                ret = Math.Pow(2.0, (double) numSuperMods);
            }
            // TsumikisThings.GetLogger().Debug("Ret: " + ret);
            return ret;
        }

        /// <summary>
        /// This differs from player.Heal() in that it accepts non-integer values and will, over time,
        /// add them together
        /// </summary>
        /// <param name="val"></param>
        public void Heal(double val)
        {
            fractionalHealth += val;
            int numToHeal = (int)fractionalHealth;
            if (numToHeal > 0)
            {
                Player.Heal(numToHeal);
                fractionalHealth -= numToHeal;
            }
        }
    }
}
