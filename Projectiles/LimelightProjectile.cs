using Terraria;
using Terraria.ModLoader;

namespace Limelight.Projectiles
{
    public abstract class LimelightProjectile : ModProjectile
    {
        #region Propreties
        public Player owner => Main.player[projectile.owner];
        #endregion

        #region Overridable Members
        public virtual void SafeSetDefaults() { }
        #endregion

        public sealed override void SetDefaults()
        {
            SafeSetDefaults();
        }
    }
}
