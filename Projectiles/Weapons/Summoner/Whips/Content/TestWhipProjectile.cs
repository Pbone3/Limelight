﻿using Microsoft.Xna.Framework;
using Terraria;
using Limelight.Utilities;

namespace Limelight.Projectiles.Weapons.Summoner.Whips.Content
{
    public class TestWhipProjectile : BaseWhipProjectile
    {
        public override WhipProjectileProfile Stats => new WhipProjectileProfile(
            rangeMulti: 0.85f,
            ropeColor: Color.Black,
            visuals: (projectile, whipPoints, timeToFlyOut) => {
                // Make sure it doesn't happen before it gets not near the player
                float quotient = useTimer / timeToFlyOut;
                if (Helpers.GetLerpValue(0.1f, 0.7f, quotient, clamped: true) * Helpers.GetLerpValue(0.9f, 0.7f, quotient, clamped: true) > 0.5f && Main.rand.Next(3) != 0)
                {
                    // Doing it every frame is a little absurb, add randomness
                    if (Main.rand.NextBool(3))
                    {
                        // Makes it at the point
                        Rectangle rect = Utils.CenteredRectangle(whipPoints[whipPoints.Count - 1], new Vector2(20f, 20f));
                        Dust.NewDust(rect.TopLeft(), rect.Width, rect.Height, 16);
                    }
                }
            }
            );
    }
}
