﻿using IL.Terraria.GameContent.ObjectInteractions;
using log4net;
using log4net.Repository.Hierarchy;
using System;
using System.Collections.Generic;
using System.IO;
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

        private double damageBonus; // Damage bonus
        private double critChance; // Crit chance
        private double moveSpeed; // Movement speed bonus
        private double ammoConsumption; // Ammo consumption % chance reduction
        private double weaponSize; // Weapon size % increase

        private int defense; // Defense bonus, like the Warding mod
        private int extraMana; // Mana bonus, like the Arcane mod
        private double summonDamageHealAmount; // Chance to heal when summon damage received
        
        /// <summary>
        /// All below functions (up to the next summary) are to make sure the modifiers work properly
        /// </summary>
        public void UpdateAccessory(TsumikiPlayer modPlayer)
        {
            Player player = modPlayer.Player;
            
            // Limitted to seven times the modifier's max value
            player.GetDamage(DamageClass.Generic) += (float) LimitModifier(damageBonus, 0.21);
            player.GetCritChance(DamageClass.Generic) += (float) LimitModifier(critChance, 21.0);
            player.moveSpeed += (float) (LimitModifier(moveSpeed, 0.35));
            // process ammoConsumption in CanConsumeAmmo
            // process weaponSize in ModifyItemScale
            player.statDefense += LimitModifier(defense, 28);
            player.statManaMax2 += LimitModifier(extraMana, 140);
            // summonDamageHealChance processed manually in OnHitNPC
        }

        public void OnHitNPC(TsumikiPlayer modPlayer)
        {
            Player player = modPlayer.Player;
            if (player.HeldItem.DamageType == DamageClass.Summon)
            {
                // Check for Summon Damage Heal
                double randNum = rand.NextDouble();
                StatModifier damageClass = modPlayer.Player.GetDamage(DamageClass.Summon);
                // from here we just hope the flat damage values are negligible

                // 1.00 with no buffs
                double healChance = (damageClass.ApplyTo(1) - damageClass.Flat);

                // 1.00 with no buffs, buffs are double strength
                healChance *= healChance;
                
                // 1.00 with no buffs, buffs are double strength.
                healChance /= 100;

                if (modPlayer.summonHealCooldown <= 0)
                {
                    logger.Debug("Heal chance to beat: " + healChance);
                    if (randNum < healChance)
                    {
                        // not on cooldown
                        modPlayer.summonHealCooldown = 60;
                        double healAmount = LimitModifier(summonDamageHealAmount, 7.0);
                        logger.Debug("Healing for " + healAmount);
                        modPlayer.Heal(healAmount);
                    }
                }
            }
        }

        public bool CanConsumeAmmo()
        {
            return rand.NextDouble() >= LimitModifier(ammoConsumption, 0.21);
        }

        public void ModifyItemScale(ref float scale)
        {
            scale *= (float) (1+LimitModifier(weaponSize, 0.21));
        }

        // Sets this SuperModifier to itself + the other SuperModifier.
        // This also has logic for capping the possible bonuses
        public void Add(SuperModifier oth)
        {
            damageBonus += oth.damageBonus;
            critChance += oth.critChance;
            moveSpeed += oth.moveSpeed;
            ammoConsumption += oth.ammoConsumption;
            weaponSize += oth.weaponSize;
            defense += oth.defense;
            extraMana += oth.extraMana;
            summonDamageHealAmount += oth.summonDamageHealAmount;
        }   

        private int LimitModifier(int modifier, int cap)
        {
            if(modifier < cap)
            {
                return modifier;
            }
            double ratio = ((double)modifier) / cap;
            double adjRatio = Math.Sqrt(ratio); // note that sqrt(x) < x if x > 1
            int adjMod = (int) adjRatio * cap;
            return adjMod;
        }

        private double LimitModifier(double modifier, double cap) {
            if(modifier < cap)
            {
                return modifier;
            }
            double ratio = modifier / cap;
            double adjRatio = Math.Sqrt(ratio);
            double adjMod = adjRatio * cap;
            return adjMod;
        }

        /// <summary>
        /// All below functions are part of making the basic UI and save/load work properly
        /// </summary>

        public SuperModifier()
        {
            damageBonus = 0;
            critChance = 0;
            moveSpeed = 0;
            ammoConsumption = 0;
            weaponSize = 0;
            defense = 0;
            extraMana = 0;
            summonDamageHealAmount = 0;
        }

        public TooltipLine GetTooltipLine(Mod mod)
        {
            string text = "";
            // https://learn.microsoft.com/en-us/dotnet/api/system.double.tostring?view=net-8.0
            if(damageBonus != 0)
            {
                text += "+" + (100.0*damageBonus).ToString("F2") + "% damage\n";
            }
            if (critChance != 0)
            {
                text += "+" + critChance.ToString("F2") + "% crit chance\n";
            }
            if (moveSpeed != 0)
            {
                text += "+" + (100.0*moveSpeed).ToString("F2") + "% move speed\n";
            }
            if (ammoConsumption != 0)
            {
                text += "-" + (100.0*ammoConsumption).ToString("F2") + "% ammo consumption\n";
            }
            if (weaponSize != 0)
            {
                text += "+" + (100.0*weaponSize).ToString("F2") + "% weapon size\n";
            }
            if (defense != 0)
            {
                text += "+" + defense + " defense\n";
            }
            if (extraMana != 0)
            {
                text += "+" + extraMana + " mana\n";
            }
            if (summonDamageHealAmount != 0)
            {
                text += "Chance to heal " + (1.0*summonDamageHealAmount).ToString("F2") + " on hit while holding a summon weapon\n";
            }

            // No super mods.
            if (text == "")
            {
                text = "No super mods.";
            }
            TooltipLine ret = new(mod, "SuperModifier", text);
            return ret;
        }

        public static SuperModifier createRandom()
        {
            SuperModifier ret = new();
            while(rand.NextDouble() < 0.5)
            {
                ret.AddOneModifier();
            }
            return ret;
        }

        // Adds exactly one random modifier to this SuperModifier.
        public void AddOneModifier()
        {
            int choice = rand.Next(0, 8);
            switch (choice)
            {
                case 0:
                    damageBonus += rand.NextDouble() * 0.03;
                    break;
                case 1:
                    critChance += rand.NextDouble() * 3;
                    break;
                case 2:
                    moveSpeed += rand.NextDouble() * 0.05;
                    break;
                case 3:
                    ammoConsumption += rand.NextDouble() * 0.03;
                    break;
                case 4:
                    weaponSize += rand.NextDouble() * 0.03;
                    break;
                case 5:
                    defense += rand.Next(1, 4);
                    break;
                case 6:
                    extraMana += rand.Next(1, 20);
                    break;
                case 7:
                default: // Default shouldn't get called, but just in case.
                    summonDamageHealAmount += rand.NextDouble() * 1.0;
                    break;
            }
        }

        public SuperModifier Clone()
        {
            SuperModifier clone = new()
            {
                damageBonus = damageBonus,
                critChance = critChance,
                moveSpeed = moveSpeed,
                ammoConsumption = ammoConsumption,
                weaponSize = weaponSize,
                defense = defense,
                extraMana = extraMana,
                summonDamageHealAmount = summonDamageHealAmount
            };
            return clone;
        }

        public TagCompound ToTag()
        {
            TagCompound ret = new()
            {
                {"damageBonus", damageBonus },
                {"critChance", critChance},
                {"moveSpeed", moveSpeed},
                {"ammoConsumption", ammoConsumption },
                {"weaponSize", weaponSize },
                {"defense", defense },
                {"extraMana", extraMana},
                {"summonDamageHealAmount", summonDamageHealAmount }
            };
            return ret;
        }

        // This is equivalent to "Load data from tag".
        public SuperModifier(TagCompound tag)
        {
            damageBonus = tag.ContainsKey("damageBonus") ? tag.GetAsDouble("damageBonus") : 0;
            critChance = tag.ContainsKey("critChance") ? tag.GetAsDouble("critChance") : 0;
            moveSpeed = tag.ContainsKey("moveSpeed") ? tag.GetAsDouble("moveSpeed") : 0;
            ammoConsumption = tag.ContainsKey("ammoConsumption") ? tag.GetAsDouble("ammoConsumption") : 0;
            weaponSize = tag.ContainsKey("weaponSize") ? tag.GetAsDouble("weaponSize") : 0;
            defense = tag.ContainsKey("defense") ? tag.GetAsInt("defense") : 0;
            extraMana = tag.ContainsKey("extraMana") ? tag.GetAsInt("extraMana") : 0;
            summonDamageHealAmount = tag.ContainsKey("summonDamageHealAmount") ? tag.GetAsDouble("summonDamageHealAmount") : 0;
        }

        /// <summary>
        /// Network commands.
        /// </summary>
        public void NetSend(Item item, BinaryWriter writer)
        {
            if (item.accessory)
            {
                writer.Write(damageBonus);
                writer.Write(critChance);
                writer.Write(moveSpeed);
                writer.Write(ammoConsumption);
                writer.Write(weaponSize);
                writer.Write(defense);
                writer.Write(extraMana);
                writer.Write(summonDamageHealAmount);
            }
            // otherwise do nothing
        }

        public void NetReceive(Item item, BinaryReader reader)
        {
            if (item.accessory)
            {
                damageBonus = reader.ReadDouble();
                critChance = reader.ReadDouble();
                moveSpeed = reader.ReadDouble();
                ammoConsumption = reader.ReadDouble();
                weaponSize = reader.ReadDouble();
                defense = reader.ReadInt32();
                extraMana = reader.ReadInt32();
                summonDamageHealAmount = reader.ReadDouble();
            }
            // otherwise not an accessory so doesn't apply
        }
    }
}
