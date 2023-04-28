using IL.Terraria.Graphics;
using log4net;
using log4net.Repository.Hierarchy;
using Microsoft.CodeAnalysis.Operations;
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

        private static readonly ILog logger = TsumikisThings.GetLogger();

        public override bool InstancePerEntity => true;

        public override bool AppliesToEntity(Item entity, bool lateInstantiation)
        {
            return entity.accessory;
        }

        public override GlobalItem Clone(Item from, Item to)
        {
            TsumikiGlobalItem clone = (TsumikiGlobalItem)base.Clone(from, to);
            clone.modifiers = modifiers.Clone();
            return clone;
        }

        public override void OnCreate(Item item, ItemCreationContext context)
        {
            if (context is RecipeCreationContext)
            {
                modifiers = SuperModifier.createRandom();
            }
            else
            {
                // just in case
                modifiers = new();
            }
        }

        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            if (!item.accessory)
            {
                return;
            }
            tooltips.Add(modifiers.GetTooltipLine(TsumikisThings.GetMod()));
        }

        // https://github.com/tModLoader/tModLoader/blob/8574f4628b917c932e5bf694e09364be9a02a847/ExampleMod/Content/Items/ExampleInstancedItem.cs
        // I guess this is how to do it?
        public override void SaveData(Item item, TagCompound tag)
        {
            tag["superMods"] = modifiers.ToTag();
        }

        public override void LoadData(Item item, TagCompound tag)
        {
            modifiers = new SuperModifier(tag.GetCompound("superMods"));
        }

        /**
         * Functions below are simply re-sent to member variables (ie SuperModifier)
         */
        public override void UpdateAccessory(Item item, Player player, bool hideVisual)
        {
            player.GetModPlayer<TsumikiPlayer>().AddModifier(modifiers);
        }
    }
}
