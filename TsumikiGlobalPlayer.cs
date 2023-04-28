using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace TsumikisThings
{
    internal class TsumikiGlobalPlayer : ModPlayer
    {
        SuperModifier modifiers = new();

        public override void ResetEffects()
        {
            base.ResetEffects();
            modifiers = new SuperModifier();
        }

        // Docs say this is called after Update Accessories
        // and this is basically supposed to be UpdateAccessories, just done here so the head super-modifier can cap things
        public override void UpdateEquips()
        {
            base.UpdateEquips();
            modifiers.UpdateAccessory(Player);
        }

        public void AddModifier(SuperModifier modifier)
        {
            modifiers.Add(modifier);
        }
    }
}
