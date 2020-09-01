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

        #region Localization
        public const string Key_SummonTagDamage = "Mods.Limelight.TooltipPiece.SummonTagDamage";
        public const string Key_SummonCritChance = "Mods.Limelight.TooltipPiece.SummonCritChance";
        public const string Key_Whip = "Mods.Limelight.Whip";
        public const string Key_CommonWhipTooltip = "CommonItemTooltip.Whips";

        public string LocalizedName => TextValue($"{TextValue(Key_Whip)}.{Name}.Name");
        public string LocalizedSpecialEffect => TextValue($"{TextValue(Key_Whip)}.{Name}.SpecialEffect");
        public string LocalizedTagline => TextValue($"{TextValue(Key_Whip)}.{Name}.Tagline");
        #endregion

        #region Overridable Members
        public virtual int TagDamage => 0;
        public virtual int TagCritChance => 0;

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
                DefaultPredicate("Tag damage line"),
                DefaultAction(TagDamage, Language.GetTextValue(Key_SummonTagDamage, TagDamage))
                );

            tooltips.TryModify(
                DefaultPredicate("Tag crit line"),
                DefaultAction(TagCritChance, Language.GetTextValue(Key_SummonCritChance, TagCritChance))
                );

                tooltips.TryModify(
                    DefaultPredicate("Special effect line"),
                    DefaultAction(!LocalizedSpecialEffect.Equals(" ") ? LocalizedSpecialEffect : "")
                    );

                tooltips.TryModify(
                    DefaultPredicate("Tagline line"),
                    DefaultAction(!LocalizedTagline.Equals(" ") ? LocalizedTagline : "")
                    );
        }

        private Func<TooltipLine, bool> DefaultPredicate(string textToFind) => (TooltipLine tt) => tt.text.Equals(textToFind);
        private Action<TooltipLine> DefaultAction(int value, string text) => (TooltipLine tt) => tt.text = (value > 0 ? text : "");
        private Action<TooltipLine> DefaultAction(string text) => (TooltipLine tt) => tt.text = text;
        private string TextValue(string key) => Language.GetTextValue(key);
    }
}
