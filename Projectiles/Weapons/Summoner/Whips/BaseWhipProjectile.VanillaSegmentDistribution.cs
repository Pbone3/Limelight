using Microsoft.Xna.Framework;
using Terraria;
using System.Collections.Generic;
using Limelight.Utilities;

namespace Limelight.Projectiles.Weapons.Summoner.Whips
{
    public partial class BaseWhipProjectile : LimelightProjectile
    {
        // TODO: port old whip drawing?
        public partial struct WhipProjectileProfile
        {
            public delegate bool DistributeSegments(ref int step, ref Rectangle frame, ref int height, ref Vector2 origin, ref float scale, ref List<Vector2> whipPoints, ref Projectile projectile);

            public static DistributeSegments Coolwhip = CoolwhipDistribution;
            public static bool CoolwhipDistribution(ref int step, ref Rectangle frame, ref int height, ref Vector2 origin, ref float scale, ref List<Vector2> whipPoints, ref Projectile projectile)
            {
                switch (step)
                {
                    case 0:
                        origin.Y -= 4f;
                        break;
                    case 3:
                    case 5:
                    case 7:
                        frame.Y = height;
                        break;
                    case 9:
                    case 11:
                    case 13:
                        frame.Y = height * 2;
                        break;
                    case 15:
                    case 17:
                        frame.Y = height * 3;
                        break;
                    case 19:
                        frame.Y = height * 4;
                        break;
                    default:
                        return false;
                }

                return true;
            }

            public static DistributeSegments FireWhip = FireWhupDistribution;
            public static bool FireWhupDistribution(ref int step, ref Rectangle frame, ref int height, ref Vector2 origin, ref float scale, ref List<Vector2> whipPoints, ref Projectile projectile)
            {
                switch (step)
                {
                    case 0:
                        origin.Y -= 4f;
                        break;
                    case 3:
                    case 5:
                    case 7:
                        frame.Y = height;
                        break;
                    case 9:
                    case 11:
                    case 13:
                        frame.Y = height * 2;
                        break;
                    case 15:
                    case 17:
                        frame.Y = height * 3;
                        break;
                    case 19:
                        frame.Y = height * 4;
                        break;
                    default:
                        return false;
                }

                return true;
            }

            public static DistributeSegments RainbowWhip = RainbowWhipDistribution;
            public static bool RainbowWhipDistribution(ref int step, ref Rectangle frame, ref int height, ref Vector2 origin, ref float scale, ref List<Vector2> whipPoints, ref Projectile projectile)
            {
                switch (step)
                {
                    case 0:
                        origin.Y -= 4f;
                        break;
                    case 39:
                        frame.Y = height * 4;
                        break;
                    default:
                        frame.Y = height * (1 + step % 3);
                        return (step % 2 == 0);
                }

                return true;
            }

            public static DistributeSegments ThornWhip = ThornWhipDistribution;
            public static bool ThornWhipDistribution(ref int step, ref Rectangle frame, ref int height, ref Vector2 origin, ref float scale, ref List<Vector2> whipPoints, ref Projectile projectile)
            {
                switch (step)
                {
                    case 0:
                        origin.Y -= 4f;
                        break;
                    case 19:
                        frame.Y = height * 4;
                        scale = 1.1f;
                        break;
                    default:
                        frame.Y = height * (1 + step % 3);
                        scale = 0.8f;
                        break;
                }

                return true;
            }

            public static DistributeSegments WhipSword = WhipSwordDistribution;
            public static bool WhipSwordDistribution(ref int step, ref Rectangle frame, ref int height, ref Vector2 origin, ref float scale, ref List<Vector2> whipPoints, ref Projectile projectile)
            {
                switch (step)
                {
                    case 0:
                        origin.Y -= 4f;
                        break;
                    case 3:
                    case 5:
                    case 7:
                        frame.Y = height;
                        break;
                    case 9:
                    case 11:
                    case 13:
                        frame.Y = height * 2;
                        break;
                    case 15:
                    case 17:
                        frame.Y = height * 3;
                        break;
                    case 19:
                        frame.Y = height * 4;
                        break;
                    default:
                        return false;
                }

                return true;
            }

            public static DistributeSegments WhipMace = WhipMaceDistribution;
            public static bool WhipMaceDistribution(ref int step, ref Rectangle frame, ref int height, ref Vector2 origin, ref float scale, ref List<Vector2> whipPoints, ref Projectile projectile)
            {
                bool ret = false;

                if (step == 0)
                {
                    origin.Y -= 4f;
                    ret = true;
                }
                else if (step % 2 == 0)
                {
                    int num = 1;
                    if (step > 10)
                        num = 2;

                    if (step > 20)
                        num = 3;

                    frame.Y = height * num;
                    ret = true;
                }

                if (step == whipPoints.Count - 2)
                {
                    frame.Y = height * 4;
                    scale = 1.3f;

                    BaseWhipProjectile whip = projectile.modProjectile as BaseWhipProjectile;
                    whip.Profile.GetWhipInfo(projectile, out float timeToFlyOut, out int _, out float _);

                    float t = projectile.ai[0] / timeToFlyOut;
                    float amount = Helpers.GetLerpValue(0.1f, 0.7f, t, clamped: true) * Helpers.GetLerpValue(0.9f, 0.7f, t, clamped: true);
                    scale = MathHelper.Lerp(0.5f, 2f, amount);
                    ret = true;
                }

                return ret;
            }

            public static DistributeSegments WhipScythe = WhipScytheDistribution;
            public static bool WhipScytheDistribution(ref int step, ref Rectangle frame, ref int height, ref Vector2 origin, ref float scale, ref List<Vector2> whipPoints, ref Projectile projectile)
            {
                bool ret = false;

                if (step == 0)
                {
                    origin.Y -= 4f;
                    ret = true;
                }
                else if (step % 2 == 0)
                {
                    int segmentNum = 1;
                    if (step > 10)
                        segmentNum = 2;

                    if (step > 20)
                        segmentNum = 3;

                    frame.Y = height * segmentNum;
                    ret = true;
                }

                if (step == whipPoints.Count - 2)
                {
                    frame.Y = height * 4;
                    scale = 1.3f;

                    BaseWhipProjectile whip = projectile.modProjectile as BaseWhipProjectile;
                    whip.Profile.GetWhipInfo(projectile, out float timeToFlyOut, out int _, out float _);

                    float t = projectile.ai[0] / timeToFlyOut;
                    float amount = Helpers.GetLerpValue(0.1f, 0.7f, t, clamped: true) * Helpers.GetLerpValue(0.9f, 0.7f, t, clamped: true);
                    scale = MathHelper.Lerp(0.5f, 1.5f, amount);
                    ret = true;
                }

                return ret;
            }

            public static DistributeSegments WhipBland;
            public static bool WhipBlandDistribution(ref int step, ref Rectangle frame, ref int height, ref Vector2 origin, ref float scale, ref List<Vector2> whipPoints, ref Projectile projectile)
            {
                bool ret = false;

                if (step == 0)
                {
                    origin.Y -= 4f;
                    ret = true;
                }
                else
                {
                    int num = 1;
                    if (step > 10)
                        num = 2;

                    if (step > 20)
                        num = 3;

                    frame.Y = height * num;
                    ret = true;
                }

                if (step == whipPoints.Count - 2)
                {
                    frame.Y = height * 4;
                    scale = 1.3f;

                    BaseWhipProjectile whip = projectile.modProjectile as BaseWhipProjectile;
                    whip.Profile.GetWhipInfo(projectile, out float timeToFlyOut, out int _, out float _);

                    float t = projectile.ai[0] / timeToFlyOut;
                    float amount = Helpers.GetLerpValue(0.1f, 0.7f, t, clamped: true) * Helpers.GetLerpValue(0.9f, 0.7f, t, clamped: true);
                    scale = MathHelper.Lerp(0.5f, 1.5f, amount);
                    ret = true;
                }

                return ret;
            }
        }
    }
}
