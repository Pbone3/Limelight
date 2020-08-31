using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Limelight.Items.Accessories
{
    public static class AccessoryHelper
    {
        public static AccessoryPlayer limelightAccessoryPlayer(this Player player) => player.GetModPlayer<AccessoryPlayer>();
    }
}
