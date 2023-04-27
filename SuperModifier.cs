using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TsumikisThings
{
    // SuperModifier class. Contains information about Modifiers and handles information about modifiers
    // This way manipulating modifiers everywhere only involves manipulating one class.
    internal class SuperModifier
    {
        private static Random rand = new();
        private double damageBonus = 0; // Damage bonus, in percent.

        // Creates a blank SuperModifier.
        // To create a random SuperModifier, call SuperModifier.createRandom()
        public SuperModifier() { }

        public SuperModifier(TagCompound tag)
        {
            damageBonus = tag.GetAsDouble("damageBonus");
        }

        public override string ToString()
        {
            string ret = "";
            ret += "damageBonus: " + damageBonus;
            return ret;
        }

        public TooltipLine GetTooltipLine(Mod mod)
        {
            TsumikisThings.LogDebug("Logging TooltipLine...");
            TsumikisThings.LogDebug(damageBonus);
            string text = "";
            // https://learn.microsoft.com/en-us/dotnet/api/system.double.tostring?view=net-8.0
            TsumikisThings.LogDebug(text);
            text += "+" + damageBonus.ToString("F2") + "% damage";
            TooltipLine ret = new(mod, "SuperModifier", text);
            return ret;
        }

        public static SuperModifier createRandom()
        {
            SuperModifier ret = new();
            ret.damageBonus = rand.NextDouble() * 300;
            return ret;
        }

        public TagCompound ToTag()
        {
            TagCompound ret = new TagCompound();
            ret["damageBonus"] = damageBonus;
            return ret;
        }

        public SuperModifier Clone()
        {
            SuperModifier clone = new SuperModifier();
            clone.damageBonus = damageBonus;
            return clone;
        }
    }
}
