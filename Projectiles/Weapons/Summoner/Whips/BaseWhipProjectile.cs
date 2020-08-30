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


            public WhipProjectileProfile(float rangeMulti, Color ropeColor, int segments = 20)
            {
                rangeMultiplier = rangeMulti;
                segmentAmounts = segments;
                RopeColor = ropeColor;
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
            switch (projectile.type)
            {
                case 848:
                    {
                        float t2 = useTimer / timeToFlyOut;
                        float num2 = Helpers.GetLerpValue(0.1f, 0.7f, t2, clamped: true) * Helpers.GetLerpValue(0.9f, 0.7f, t2, clamped: true);
                        if (num2 > 0.1f && Main.rand.NextFloat() < num2 / 2f)
                        {
                            _whipPointsForCollision.Clear();
                            FillWhipControlPoints(_whipPointsForCollision);
                            Rectangle r2 = Utils.CenteredRectangle(_whipPointsForCollision[_whipPointsForCollision.Count - 1], new Vector2(30f, 30f));
                            int num3 = Dust.NewDust(r2.TopLeft(), r2.Width, r2.Height, 172, 0f, 0f, 100, default(Color), 1.5f);
                            Main.dust[num3].noGravity = true;
                            Main.dust[num3].velocity.X /= 2f;
                            Main.dust[num3].velocity.Y /= 2f;
                        }

                        break;
                    }
                case 847:
                    {
                        float t4 = useTimer / timeToFlyOut;
                        if (Helpers.GetLerpValue(0.1f, 0.7f, t4, clamped: true) * Helpers.GetLerpValue(0.9f, 0.7f, t4, clamped: true) > 0.5f && Main.rand.Next(3) != 0)
                        {
                            _whipPointsForCollision.Clear();
                            FillWhipControlPoints(_whipPointsForCollision);
                            int num5 = Main.rand.Next(_whipPointsForCollision.Count - 10, _whipPointsForCollision.Count);
                            Rectangle r4 = Utils.CenteredRectangle(_whipPointsForCollision[num5], new Vector2(30f, 30f));
                            int num6 = 57;
                            if (Main.rand.Next(3) == 0)
                                num6 = 43;

                            Dust dust4 = Dust.NewDustDirect(r4.TopLeft(), r4.Width, r4.Height, num6, 0f, 0f, 100, Color.White);
                            dust4.position = _whipPointsForCollision[num5];
                            dust4.fadeIn = 0.3f;
                            Vector2 spinningpoint = _whipPointsForCollision[num5] - _whipPointsForCollision[num5 - 1];
                            dust4.noGravity = true;
                            dust4.velocity *= 0.5f;
                            dust4.velocity += spinningpoint.RotatedBy((float)owner.direction * ((float)Math.PI / 2f));
                            dust4.velocity *= 0.5f;
                        }

                        break;
                    }
                case 849:
                    {
                        float num8 = useTimer / timeToFlyOut;
                        Helpers.GetLerpValue(0.1f, 0.7f, num8, clamped: true);
                        Helpers.GetLerpValue(0.9f, 0.7f, num8, clamped: true);
                        if (num8 > 0.4f && Main.rand.Next(9) != 0)
                        {
                            _whipPointsForCollision.Clear();
                            FillWhipControlPoints(_whipPointsForCollision);
                            Rectangle r6 = Utils.CenteredRectangle(_whipPointsForCollision[_whipPointsForCollision.Count - 1], new Vector2(30f, 30f));
                            Vector2 vector = _whipPointsForCollision[_whipPointsForCollision.Count - 2].DirectionTo(_whipPointsForCollision[_whipPointsForCollision.Count - 1]).SafeNormalize(Vector2.Zero);
                            Dust dust7 = Dust.NewDustDirect(r6.TopLeft(), r6.Width, r6.Height, 191, 0f, 0f, 0, default(Color), 1.3f);
                            dust7.noGravity = true;
                            dust7.velocity += vector * 2f;
                            if (Main.rand.Next(2) == 0)
                            {
                                /*ParticleOrchestrator.RequestParticleSpawn(clientOnly: true, ParticleOrchestraType.BlackLightningSmall, new ParticleOrchestraSettings
                                {
                                    MovementVector = vector,
                                    PositionInWorld = r6.Center.ToVector2()
                                }, owner);*/
                            }

                            Lighting.AddLight(r6.Center.ToVector2(), new Vector3(0.2f, 0f, 0.4f));
                        }

                        break;
                    }
                case 915:
                    {
                        float t6 = useTimer / timeToFlyOut;
                        if (Helpers.GetLerpValue(0.1f, 0.7f, t6, clamped: true) * Helpers.GetLerpValue(0.9f, 0.7f, t6, clamped: true) > 0.1f)
                        {
                            _whipPointsForCollision.Clear();
                            FillWhipControlPoints(_whipPointsForCollision);
                            Rectangle r7 = Utils.CenteredRectangle(_whipPointsForCollision[_whipPointsForCollision.Count - 1], new Vector2(30f, 30f));
                            Vector2 value4 = _whipPointsForCollision[_whipPointsForCollision.Count - 2].DirectionTo(_whipPointsForCollision[_whipPointsForCollision.Count - 1]).SafeNormalize(Vector2.Zero);
                            /*Dust dust8 = Dust.NewDustDirect(r7.TopLeft(), r7.Width, r7.Height, 267, 0f, 0f, 0, Main.hslToRgb(owner.miscCounterNormalized * 9f % 1f, 1f, 0.5f), 1.3f);
                            dust8.velocity *= Main.rand.NextFloat() * 0.8f;
                            dust8.noGravity = true;
                            dust8.scale = 0.9f + Main.rand.NextFloat() * 0.9f;
                            dust8.fadeIn = Main.rand.NextFloat() * 0.9f;
                            dust8.velocity += value4 * 2f;
                            if (dust8.dustIndex != 6000)
                            {
                                Dust dust9 = Dust.CloneDust(dust8);
                                dust9.scale /= 2f;
                                dust9.fadeIn *= 0.85f;
                                dust9.color = new Color(255, 255, 255, 255);
                            }*/
                        }

                        break;
                    }
                case 914:
                    {
                        float t3 = useTimer / timeToFlyOut;
                        float num4 = Helpers.GetLerpValue(0.1f, 0.7f, t3, clamped: true) * Helpers.GetLerpValue(0.9f, 0.7f, t3, clamped: true);
                        if (num4 > 0.1f && Main.rand.NextFloat() < num4 / 2f)
                        {
                            _whipPointsForCollision.Clear();
                            FillWhipControlPoints(_whipPointsForCollision);
                            Rectangle r3 = Utils.CenteredRectangle(_whipPointsForCollision[_whipPointsForCollision.Count - 1], new Vector2(30f, 30f));
                            Vector2 value2 = _whipPointsForCollision[_whipPointsForCollision.Count - 2].DirectionTo(_whipPointsForCollision[_whipPointsForCollision.Count - 1]).SafeNormalize(Vector2.Zero);
                            Dust dust3 = Dust.NewDustDirect(r3.TopLeft(), r3.Width, r3.Height, 39, 0f, 0f, 0, default(Color), 1.2f);
                            dust3.noGravity = (Main.rand.Next(3) == 0);
                            dust3.velocity += value2 * 2f;
                        }

                        break;
                    }
                case 912:
                    {
                        float t5 = useTimer / timeToFlyOut;
                        float num7 = Helpers.GetLerpValue(0.1f, 0.7f, t5, clamped: true) * Helpers.GetLerpValue(0.9f, 0.7f, t5, clamped: true);
                        if (!(num7 > 0.1f) || !(Main.rand.NextFloat() < num7 / 2f))
                            break;

                        _whipPointsForCollision.Clear();
                        FillWhipControlPoints(_whipPointsForCollision);
                        Rectangle r5 = Utils.CenteredRectangle(_whipPointsForCollision[_whipPointsForCollision.Count - 1], new Vector2(30f, 30f));
                        Vector2 value3 = _whipPointsForCollision[_whipPointsForCollision.Count - 2].DirectionTo(_whipPointsForCollision[_whipPointsForCollision.Count - 1]).SafeNormalize(Vector2.Zero);
                        for (int j = 0; j < 3; j++)
                        {
                            Dust dust5 = Dust.NewDustDirect(r5.TopLeft(), r5.Width, r5.Height, 16, 0f, 0f, 0, default(Color), 1.2f);
                            dust5.noGravity = true;
                            dust5.velocity += value3 * 2f;
                        }

                        for (int k = 0; k < 1; k++)
                        {
                            Dust.NewDustDirect(r5.TopLeft(), r5.Width, r5.Height, 13, 0f, 0f, 0, default(Color), 0.8f).velocity += value3 * 2f;
                        }

                        for (int l = 0; l < 3; l++)
                        {
                            if (Main.rand.Next(2) != 0)
                            {
                                Dust dust6 = Dust.NewDustDirect(r5.TopLeft(), r5.Width, r5.Height, 261, 0f, 0f, 0, Color.Transparent, 0.8f);
                                dust6.velocity += value3 * 2f;
                                dust6.velocity *= 0.3f;
                                dust6.noGravity = true;
                            }
                        }

                        Lighting.AddLight(r5.Center.ToVector2(), new Vector3(0.1f, 0.1f, 0.2f));
                        break;
                    }
                case 913:
                    {
                        float t = useTimer / timeToFlyOut;
                        float num = Helpers.GetLerpValue(0.1f, 0.7f, t, clamped: true) * Helpers.GetLerpValue(0.9f, 0.7f, t, clamped: true);
                        if (!(num > 0.1f) || !(Main.rand.NextFloat() < num))
                            break;

                        _whipPointsForCollision.Clear();
                        FillWhipControlPoints(_whipPointsForCollision);
                        Rectangle r = Utils.CenteredRectangle(_whipPointsForCollision[_whipPointsForCollision.Count - 1], new Vector2(20f, 20f));
                        Vector2 value = _whipPointsForCollision[_whipPointsForCollision.Count - 2].DirectionTo(_whipPointsForCollision[_whipPointsForCollision.Count - 1]).SafeNormalize(Vector2.Zero);
                        for (int i = 0; i < 3; i++)
                        {
                            if (Main.rand.Next(3) != 0)
                                continue;

                            if (Main.rand.Next(7) == 0)
                            {
                                Dust dust = Dust.NewDustDirect(r.TopLeft(), r.Width, r.Height, 31);
                                dust.velocity.X /= 2f;
                                dust.velocity.Y /= 2f;
                                dust.velocity += value * 2f;
                                dust.fadeIn = 1f + Main.rand.NextFloat() * 0.6f;
                                dust.noGravity = true;
                                continue;
                            }

                            Dust dust2 = Dust.NewDustDirect(r.TopLeft(), r.Width, r.Height, 6, 0f, 0f, 0, default(Color), 1.2f);
                            dust2.velocity += value * 2f;
                            if (Main.rand.Next(3) != 0)
                            {
                                dust2.fadeIn = 0.7f + Main.rand.NextFloat() * 0.9f;
                                dust2.scale = 0.6f;
                                dust2.noGravity = true;
                            }
                        }

                        break;
                    }
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
