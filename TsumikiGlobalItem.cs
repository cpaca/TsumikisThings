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
    internal class TsumikiGlobalItem : GlobalItem
    {
        private SuperModifier modifiers = new();

        public override bool InstancePerEntity => true;

        public override GlobalItem Clone(Item from, Item to)
        {
            //TsumikiGlobalItem fromGlobalItem = from.GetGlobalItem<TsumikiGlobalItem>();
            //TsumikiGlobalItem ret = new();
            //// the ? is needed because something that's not an accessory won't have super mods
            //ret.modifiers = fromGlobalItem.modifiers.Clone();
            TsumikiGlobalItem ret = new();
            ret.modifiers = modifiers.Clone();
            return ret;
        }

        public override void OnCreate(Item item, ItemCreationContext context)
        {
            modifiers = SuperModifier.createRandom();
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!item.accessory)
            {
                return;
            }
            TsumikisThings.LogDebug("Attempting to call modifiers.GetTooltipLine");
            tooltips.Add(modifiers.GetTooltipLine(TsumikisThings.GetMod()));
            TsumikisThings.LogDebug("How the fuck is an error dropping here");
        }

        // https://github.com/tModLoader/tModLoader/blob/8574f4628b917c932e5bf694e09364be9a02a847/ExampleMod/Content/Items/ExampleInstancedItem.cs
        // I guess this is how to do it?
        public override void SaveData(Item item, TagCompound tag)
        {
            TagCompound superModsTag = modifiers.ToTag();
            tag["superMods"] = superModsTag;
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            modifiers = new SuperModifier(tag.GetCompound("superMods"));
        }
    }
}
