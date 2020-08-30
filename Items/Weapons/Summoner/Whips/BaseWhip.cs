using Limelight.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Limelight.Items.Weapons.Summoner.Whips
{
    public abstract class BaseWhip : LimelightItem
    {
        #region Structs
        public struct WhipProfile
        {
            public int damage;
            public float knockBack;
            public float shootSpeed;
            public int useTime;
            public int shoot;

            public WhipProfile(int dmg, float kb, float velocity, int animationTime, int proj)
            {
                damage = dmg;
                knockBack = kb;
                shootSpeed = velocity;
                useTime = animationTime;
                shoot = proj;
            }
        }
        #endregion

        #region Localization Keys
        public const string Key_SummonTagDamage = "Mods.Limelight.TooltipPiece.SummonTagDamage";
        public const string Key_SummonCritChance = "Mods.Limelight.TooltipPiece.SummonCritChance";
        public const string Key_CommonWhipTooltip = "CommonItemTooltip.Whips";
        #endregion

        #region Overridable Members
        public virtual string ItemName => "";
        public virtual int TagDamage => 0;
        public virtual int TagCritChance => 0;
        public virtual string SpecialEffect => "";
        public virtual string Tagline => "";

        public virtual WhipProfile Stats => default;
        public virtual void SaferSetDefaults() { }
        #endregion

        public override void SetStaticDefaults()
        {
            Tooltip.SetDefault(
                "Tag damage line" +
                "\nTag crit line" +
                "\nYour summons will focus struck enemies" + /* $"\n{Language.GetTextValue(Key_CommonWhipTooltip)}" */
                "\nSpecial effect line" +
                "\nTagline line"
                );
        }

        public sealed override void SafeSetDefaults()
        {
            // 1.4 TODO: item.DefaultToWhip, item.SetShopValues

            item.autoReuse = false;
            item.useStyle = ItemUseStyleID.SwingThrow;
            item.useAnimation = item.useTime = Stats.useTime;
            item.shoot = Stats.shoot;
            //item.UseSound = SoundID.Item152;
            item.UseSound = SoundID.Item1;
            item.noMelee = true;
            item.summon = true;
            item.noUseGraphic = true;
            item.damage = Stats.damage;
            item.knockBack = Stats.knockBack;
            item.shootSpeed = Stats.shootSpeed;

            SaferSetDefaults();
        }

        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            tooltips.TryModify(
                (TooltipLine tt) => tt.Name.Equals("ItemName"),
                (TooltipLine tt) => tt.text = ItemName
                );

            tooltips.TryModify(
                ModifyTooltips_DefaultPredicate("Tag damage line"),
                ModifyTooltips_DefaultAction(TagDamage, $"{TagDamage} {Language.GetTextValue(Key_SummonTagDamage)}")
                );

            tooltips.TryModify(
                ModifyTooltips_DefaultPredicate("Tag crit line"),
                ModifyTooltips_DefaultAction(TagCritChance, $"{TagCritChance} {Language.GetTextValue(Key_SummonCritChance)}")
                );

            tooltips.TryModify(
                ModifyTooltips_DefaultPredicate("Special effect line"),
                ModifyTooltips_DefaultAction(SpecialEffect)
                );

            tooltips.TryModify(
                ModifyTooltips_DefaultPredicate("Tagline line"),
                ModifyTooltips_DefaultAction(Tagline)
                );
        }

        private Func<TooltipLine, bool> ModifyTooltips_DefaultPredicate(string textToFind) => (TooltipLine tt) => tt.text.Equals(textToFind);
        private Action<TooltipLine> ModifyTooltips_DefaultAction(int value, string text) => (TooltipLine tt) => tt.text = (value > 0 ? text : "");
        private Action<TooltipLine> ModifyTooltips_DefaultAction(string text) => (TooltipLine tt) => tt.text = text;
    }
}
