using Microsoft.Xna.Framework;

namespace Limelight.Projectiles.Weapons.Summoner.Whips.Content
{
    public class TestWhipProjectile : BaseWhipProjectile
    {
        public override WhipProjectileProfile Stats => new WhipProjectileProfile(
            rangeMulti: 1f,
            ropeColor: Color.Black
            );
    }
}
