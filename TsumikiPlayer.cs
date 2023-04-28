using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace TsumikisThings
{
    internal class TsumikiPlayer : ModPlayer
    {
        SuperModifier modifiers = new();
        internal int summonHealCooldown = 0;

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
    }
}
