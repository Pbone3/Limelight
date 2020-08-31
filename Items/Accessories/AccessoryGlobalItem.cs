using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Limelight.Items.Accessories
{
    public class AccessoryGlobalItem : GlobalItem
    {
        public override float UseTimeMultiplier(Item item, Player player)
        {
            if (item.summon /*&& ItemID.Sets.SummonerWeaponThatScalesWithAttackSpeed[item.type]*/)
                return player.limelightAccessoryPlayer().whipUseTimeMultiplier;
            return base.UseTimeMultiplier(item, player);
        }
    }
}
