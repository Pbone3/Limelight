using Terraria.ModLoader;

namespace Limelight.Items.Accessories
{
    public class AccessoryPlayer : ModPlayer
    {
        #region Fields
        public float whipUseTimeMultiplier;
        public bool jungleMimicAcc;
        #endregion

        public override void ResetEffects()
        {
            whipUseTimeMultiplier = 1f;
            jungleMimicAcc = false;
        }
    }
}
