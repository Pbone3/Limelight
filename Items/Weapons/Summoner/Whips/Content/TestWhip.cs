using Limelight.Projectiles.Weapons.Summoner.Whips.Content;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Limelight.Items.Weapons.Summoner.Whips.Content
{
    public class TestWhip : BaseWhip
    {
        //spublic override string ItemName => "Test whip";
        public override int TagDamage => 5;
        public override int TagCritChance => 10;
        public override string SpecialEffect => "Does whatever pbone needs";
        //public override string Tagline => "'Example GOD'"; <I need to test not having a line>

        public override ShopProfile ShopStats => new ShopProfile(
            rare: ItemRarityID.Green,
            Item.sellPrice(1, 2, 3, 4));
        public override WhipProfile Stats => new WhipProfile(
            dmg: 100,
            kb: 10,
            velocity: 16f,
            animationTime: 30,
            proj: ModContent.ProjectileType<TestWhipProjectile>()
            );
    }
}
