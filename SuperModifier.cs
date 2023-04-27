using log4net;
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
        private static readonly ILog logger = TsumikisThings.GetLogger();

        public double damageBonus = 0; // Damage bonus, in percent.

        // Creates a blank SuperModifier.
        // To create a random SuperModifier, call SuperModifier.createRandom()
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
            logger.Debug("SuperModifier attempting to construct TagCompound");
            TagCompound ret = new()
            {
                {"damageBonus", damageBonus }
            };
            logger.Debug("SuperModifier ToTag Returning");
            return ret;
        }

        public void SaveData(TagCompound tag)
        {
            tag.Add("superModDamageBonus", damageBonus);
        }

        public void LoadData(TagCompound tag)
        {
            damageBonus = tag.ContainsKey("superModDamageBonus") ? tag.GetAsDouble("superModDamageBonus") : 0;
        }
    }
}
