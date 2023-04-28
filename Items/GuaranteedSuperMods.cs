using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace TsumikisThings.Items
{
    internal class GuaranteedSuperMods : ModItem
    {
        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault("Every copy of this item in your inventory guarantees a super-mod on reforge.\n" +
                "However, this also makes reforging 5x more expensive for accessories.\n" +
                "Maximum 8 can be used at once.");
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 14;
            Item.height = 16;

            Item.maxStack = 8;
            Item.value = Item.sellPrice(gold: 25);
        }

        public override void AddRecipes()
        {
            Recipe r = CreateRecipe();
            r.AddIngredient(ItemID.PlatinumCoin, 1);
            r.AddTile(TileID.TinkerersWorkbench); // I mean if you're making this you should have a tinkerer already
            r.Register();
        }
    }
}
