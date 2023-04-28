using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace TsumikisThings
{
    // SuperModifier class. Contains information about Modifiers and handles information about modifiers
    // This way manipulating modifiers everywhere only involves manipulating one class.
    internal class SuperModifier
    {
        private static Random rand = new();
        private static readonly ILog logger = TsumikisThings.GetLogger();

        public double damageBonus; // Damage bonus, in percent.

        
        /// <summary>
        /// All below functions (up to the next summary) are to make sure the modifiers work properly
        /// </summary>
        public void UpdateAccessory(Player player)
        {
            player.GetDamage(DamageClass.Generic) += (float) damageBonus / 100.0f;
        }

        /// <summary>
        /// All below functions are part of making the basic UI and save/load work properly
        /// </summary>

        public SuperModifier()
        {
            damageBonus = 0;
        }

        public override string ToString()
        {
            string ret = "";
            ret += "damageBonus: " + damageBonus;
            return ret;
        }

        public TooltipLine GetTooltipLine(Mod mod)
        {
            string text = "";
            // https://learn.microsoft.com/en-us/dotnet/api/system.double.tostring?view=net-8.0
            if(damageBonus != 0)
            {
                text += "+" + damageBonus.ToString("F2") + "% damage";
            }

            // No super mods.
            if(text == "")
            {
                text = "No super mods.";
            }
            TooltipLine ret = new(mod, "SuperModifier", text);
            return ret;
        }

        public static SuperModifier createRandom()
        {
            SuperModifier ret = new();
            ret.damageBonus = rand.NextDouble() * 300;
            return ret;
        }

        public SuperModifier Clone()
        {
            SuperModifier clone = new SuperModifier();
            clone.damageBonus = damageBonus;
            return clone;
        }

        public TagCompound ToTag()
        {
            TagCompound ret = new()
            {
                {"damageBonus", damageBonus }
            };
            return ret;
        }

        // This is equivalent to "Load data from tag".
        public SuperModifier(TagCompound tag)
        {
            damageBonus = tag.ContainsKey("damageBonus") ? tag.GetAsDouble("damageBonus") : 0;
        }
    }
}
