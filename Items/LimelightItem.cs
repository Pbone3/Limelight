using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.DataStructures;
using Terraria.ModLoader;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Limelight.Items
{
    public abstract class LimelightItem : ModItem
    {
        #region Structs
        public struct ShopProfile
        {
            public byte rarity;
            public int value;

            public ShopProfile(byte rare, int cost)
            {
                rarity = rare;
                value = cost;
            }
        }
        #endregion

        #region Propreties
        public Texture2D texture => Main.itemTexture[item.type];
        #endregion

        #region Overridable Members
        public virtual void SafeSetDefaults() { }
        public virtual ShopProfile ShopStats => default;
        #endregion

        public sealed override void SetDefaults()
        {
            if (Limelight.Autosize) SetDefaults_Autosize();

            item.rare = ShopStats.rarity;
            item.value = ShopStats.value;

            SafeSetDefaults();
        }

        private void SetDefaults_Autosize()
        {
            if (Main.itemAnimationsRegistered.Contains(item.type))
            {
                DrawAnimationVertical animation = Main.itemAnimations[item.type] as DrawAnimationVertical;
                item.Size = new Vector2(texture.Width / animation.FrameCount, texture.Height);
                return;
            }

            item.Size = texture.Size();
        }
    }
}
