using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using System;
using System.Collections.Generic;
using Limelight.Utilities;
using System.IO;
using Microsoft.Xna.Framework.Graphics;

namespace Limelight.Projectiles.Weapons.Summoner.Whips
{
    public abstract class BaseWhipProjectile : LimelightProjectile
    {
        #region Structs
        public struct WhipProjectileProfile
        {
            public int segmentAmounts;
            public float rangeMultiplier;
            public Color RopeColor;
            public Action<Projectile, List<Vector2>, float> Visuals;

            public WhipProjectileProfile(float rangeMulti, Color ropeColor, int segments = 20, Action<Projectile, List<Vector2>, float> visuals = default)
            {
                rangeMultiplier = rangeMulti;
                segmentAmounts = segments;
                RopeColor = ropeColor;
                Visuals = visuals;
            }

            // Note: Keep in 1.4, relies on custom struct
            public void GetWhipInfo(Projectile projectile, out float timeToFlyOut, out int segments, out float rangeMulti)
            {
                timeToFlyOut = Main.player[projectile.owner].itemAnimationMax * projectile.MaxUpdates;
                segments = segmentAmounts;
                rangeMulti = rangeMultiplier;
            }
        }
        #endregion

        #region Constants
        // TODO 1.4: Built in
        public static Vector2[] OffsetsPlayerOnhand = new Vector2[20] {
            new Vector2(6f, 19f),
            new Vector2(5f, 10f),
            new Vector2(12f, 10f),
            new Vector2(13f, 17f),
            new Vector2(12f, 19f),
            new Vector2(5f, 10f),
            new Vector2(7f, 17f),
            new Vector2(6f, 16f),
            new Vector2(6f, 16f),
            new Vector2(6f, 16f),
            new Vector2(6f, 17f),
            new Vector2(7f, 17f),
            new Vector2(7f, 17f),
            new Vector2(7f, 17f),
            new Vector2(8f, 17f),
            new Vector2(9f, 16f),
            new Vector2(9f, 12f),
            new Vector2(8f, 17f),
            new Vector2(7f, 17f),
            new Vector2(7f, 17f)
        };
        #endregion

        #region Overridable Members
        public virtual void SaferSetDefaults() { }
        public virtual void SafeOnHitNPC(NPC target, int damage, float knockback, bool crit) { }
        public virtual WhipProjectileProfile Stats => default;
        #endregion

        public override void SafeSetDefaults()
        {
            projectile.Size = new Vector2(18f);
            projectile.friendly = true;
            projectile.penetrate = -1;
            projectile.tileCollide = false;
            projectile.scale = 1f;
            projectile.ownerHitCheck = true;
            projectile.extraUpdates = 1;
            projectile.usesLocalNPCImmunity = true;
            projectile.localNPCHitCooldown = -1;

            SaferSetDefaults();
        }

        public override void OnHitNPC(NPC target, int damage, float knockback, bool crit)
        {
            // Target hit NPCs
            if (target.active && target.whoAmI > 0 && target.whoAmI < 1000 && target.type != NPCID.TargetDummy)
                owner.MinionAttackTargetNPC = target.whoAmI;
            SafeOnHitNPC(target, damage, knockback, crit);
        }

        #region AI related members
        List<Vector2> _whipPointsForCollision = new List<Vector2>();
        public float useTimer { get => projectile.ai[0]; set => projectile.ai[0] = value; }

        #region Methods
        // TODO 1.4: Method in 1.4
        public static Vector2 GetPlayerArmPosition(Projectile projectile)
        {
            Player player = Main.player[projectile.owner];
            Vector2 value = OffsetsPlayerOnhand[player.bodyFrame.Y / 56] * 2f;
            if (player.direction != 1)
                value.X = (float)player.bodyFrame.Width - value.X;

            if (player.gravDir != 1f)
                value.Y = (float)player.bodyFrame.Height - value.Y;

            value -= new Vector2(player.bodyFrame.Width - player.width, player.bodyFrame.Height - 42) / 2f;
            return player.RotatedRelativePoint(player.MountedCenter - new Vector2(20f, 42f) / 2f + value + Vector2.UnitY * player.gfxOffY);
        }

        // Note: Keep in 1.4, relies on custom  struct
        public void FillWhipControlPoints(List<Vector2> controlPoints)
        {
            Stats.GetWhipInfo(projectile, out float timeToFlyOut, out int segments, out float rangeMultiplier);
            float num = useTimer / timeToFlyOut;
            float num2 = 0.5f;
            float num3 = 1f + num2;
            float num4 = (float)Math.PI * 10f * (1f - num * num3) * (float)(-projectile.spriteDirection) / (float)segments;
            float num5 = num * num3;
            float num6 = 0f;
            if (num5 > 1f)
            {
                num6 = (num5 - 1f) / num2;
                num5 = MathHelper.Lerp(1f, 0f, num6);
            }

            float num7 = useTimer - 1f;
            num7 = (float)(Main.player[projectile.owner].HeldItem.useAnimation * 2) * num;
            float num8 = projectile.velocity.Length() * num7 * num5 * rangeMultiplier / (float)segments;
            float num9 = 1f;
            Vector2 playerArmPosition = GetPlayerArmPosition(projectile);
            Vector2 vector = playerArmPosition;
            float num10 = 0f - (float)Math.PI / 2f;
            Vector2 value = vector;
            float num11 = 0f + (float)Math.PI / 2f + (float)Math.PI / 2f * (float)projectile.spriteDirection;
            Vector2 value2 = vector;
            float num12 = 0f + (float)Math.PI / 2f;
            controlPoints.Add(playerArmPosition);
            for (int i = 0; i < segments; i++)
            {
                float num13 = (float)i / (float)segments;
                float num14 = num4 * num13 * num9;
                Vector2 vector2 = vector + num10.ToRotationVector2() * num8;
                Vector2 vector3 = value2 + num12.ToRotationVector2() * (num8 * 2f);
                Vector2 vector4 = value + num11.ToRotationVector2() * (num8 * 2f);
                float num15 = 1f - num5;
                float num16 = 1f - num15 * num15;
                Vector2 value3 = Vector2.Lerp(vector3, vector2, num16 * 0.9f + 0.1f);
                Vector2 value4 = Vector2.Lerp(vector4, value3, num16 * 0.7f + 0.3f);
                Vector2 spinningpoint = playerArmPosition + (value4 - playerArmPosition) * new Vector2(1f, num3);
                float num17 = num6;
                num17 *= num17;
                Vector2 item = spinningpoint.RotatedBy(projectile.rotation + 4.712389f * num17 * (float)projectile.spriteDirection, playerArmPosition);
                controlPoints.Add(item);
                num10 += num14;
                num12 += num14;
                num11 += num14;
                vector = vector2;
                value2 = vector3;
                value = vector4;
            }
        }
        #endregion
        #endregion

        public override void AI()
        {
            // Get variables required for AI
            Stats.GetWhipInfo(projectile, out float timeToFlyOut, out int _, out float _);

            // Rotation and increment the timer
            projectile.rotation = projectile.velocity.ToRotation() + (float)MathHelper.PiOver2;
            useTimer++;

            // Update position and spriteDirection, so it doesn't look stupid
            projectile.Center = GetPlayerArmPosition(projectile) + projectile.velocity * (useTimer - 1f);
            projectile.spriteDirection = (!(Vector2.Dot(projectile.velocity, Vector2.UnitX) < 0f)) ? 1 : (-1);

            // Kill once animation's done
            if (useTimer >= timeToFlyOut || owner.itemAnimation == 0)
            {
                projectile.Kill();
                return;
            }

            // Set owner fields related to item use animation
            owner.heldProj = projectile.whoAmI;
            owner.itemAnimation = owner.itemAnimationMax - (int)(useTimer / (float)projectile.MaxUpdates);
            owner.itemTime = owner.itemAnimation;

            // Triggers halfway through, playing craking sound
            if (useTimer == (int)(timeToFlyOut / 2f))
            {
                _whipPointsForCollision.Clear();
                FillWhipControlPoints(_whipPointsForCollision);
                Vector2 position = _whipPointsForCollision[_whipPointsForCollision.Count - 1];
                Main.PlaySound(/*SoundID.Item153*/SoundID.Item1, position);
            }

            // TODO: Seems to be whip related visuals, adapt (action in the profile)
            if (Stats.Visuals != default)
            {
                _whipPointsForCollision.Clear();
                FillWhipControlPoints(_whipPointsForCollision);
                Stats.Visuals.Invoke(projectile, _whipPointsForCollision, timeToFlyOut);
            }
        }

        #region Drawing
        public override bool PreDraw(SpriteBatch spriteBatch, Color lightColor)
        {
            List<Vector2> whipPoints = new List<Vector2>();
            FillWhipControlPoints(whipPoints);

            DrawRope(whipPoints, spriteBatch);
            DrawWhip(whipPoints, spriteBatch);

            return false;
        }

        public void DrawRope(List<Vector2> whipPoints, SpriteBatch spriteBatch)
        {
            Vector2 start = whipPoints[0];
            Texture2D fishingLineTex = Main.fishingLineTexture;
            Rectangle fishingLineTextureFrame = fishingLineTex.Frame();
            Vector2 origin = new Vector2(fishingLineTextureFrame.Width / 2, 2f);
            Color originalColor = Stats.RopeColor;

            for (int i = 0; i < whipPoints.Count - 1; i++)
            {
                Vector2 query = whipPoints[i];
                Vector2 difference = whipPoints[i + 1] - query;
                float rotation = difference.ToRotation() - (float)MathHelper.PiOver2;
                Point lightPoint = query.ToTileCoordinates();

                Color color = Lighting.GetColor(lightPoint.X, lightPoint.Y, originalColor);
                Vector2 scale = new Vector2(1f, (difference.Length() + 2f) / (float)fishingLineTextureFrame.Height);

                spriteBatch.Draw(fishingLineTex, start - Main.screenPosition, fishingLineTextureFrame, color, rotation, origin, scale, SpriteEffects.None, 0f);
                start += difference;
            }
        }
        public Vector2 DrawWhip(List<Vector2> whipPoints, SpriteBatch spriteBatch)
        {
            Texture2D value = Main.projectileTexture[projectile.type];
            Rectangle frame = value.Frame(1, 5);
            Vector2 vector = frame.Size() / 2f;
            Vector2 vector2 = whipPoints[0];
            int height = frame.Height;
            frame.Height -= 2;

            for (int i = 0; i < whipPoints.Count - 1; i++)
            {
                bool flag = true;
                Vector2 origin = vector;
                switch (i)
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
                        flag = false;
                        break;
                }

                Vector2 vector3 = whipPoints[i];
                Vector2 vector4 = whipPoints[i + 1] - vector3;
                if (flag)
                {
                    float rotation = vector4.ToRotation() - (float)Math.PI / 2f;
                    Point lightPosition = vector3.ToTileCoordinates();
                    Color drawColor = projectile.GetAlpha(Lighting.GetColor(lightPosition.X, lightPosition.Y));

                    spriteBatch.Draw(value, vector2 - Main.screenPosition, frame, drawColor, rotation, origin, 1f, SpriteEffects.None, 0f);
                }

                vector2 += vector4;
            }

            return vector2;
        }
        #endregion

        #region Net Send/Recieve
        public override void SendExtraAI(BinaryWriter writer)
        {
            writer.Write(_whipPointsForCollision.Count);
            foreach (Vector2 vector in _whipPointsForCollision)
                writer.WriteVector2(vector);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            int expectedLength = reader.ReadInt32();
            _whipPointsForCollision.Clear();

            try {
                for (int i = 0; i < expectedLength; i++)
                    _whipPointsForCollision[i] = reader.ReadVector2();
            }
            catch (Exception e) {
                // Check if it's caused by a read overflow or overflow
                bool containsUnderflow = e.Message.Contains("underflow");
                bool containsOverflow = e.Message.Contains("overflow");

                if (e is IOException && (containsUnderflow || containsOverflow))
                {
                    mod.Logger.Error($"Read {(containsUnderflow ? (containsOverflow ? "underflow and overflow" : "underflow") : "overflow")} occured while sending whip collision data. Attempting to use defaults...");
                }

                FillWhipControlPoints(_whipPointsForCollision);
            }
        }
        #endregion
    }
}
