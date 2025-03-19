using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SpriteAnimsTest
{
    public enum GoombaState { Walking, Squashed, Dead }

    public class Goomba : Entity
    {
        private float speed;
        private int direction;
        private bool isActive;

        // Stati del Goomba
        private GoombaState state;

        private float squashedTimer = 0.5f; // Tempo prima che scompaia dopo essere schiacciato

        // Animazioni
        private GoombaWalking walkingAnimation;
        private GoombaDeath squashedAnimation;

        public Goomba(Texture2D texture, Vector2 initialPosition, int initialDirection = -1, float speed = 0.5f) : base(initialPosition, 16, 16) 
        {
            this.direction = initialDirection;
            this.speed = speed;

            walkingAnimation = new GoombaWalking(texture, speed);
            squashedAnimation = new GoombaDeath(texture);

            // All'avvio, il Goomba cammina
            state = GoombaState.Walking;
            currentAnimation = walkingAnimation;

            isActive = true;
        }

        public bool IsDead => state == GoombaState.Dead;

        public void Update(GameTime gameTime, float cameraX, float visibleWidth) {
            // Se è morto, non facciamo nulla
            if (state == GoombaState.Dead)
                return;

            // Se è schiacciato, avviamo il timer per farlo scomparire
            if (state == GoombaState.Squashed) {
                squashedTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (squashedTimer <= 0) {
                    state = GoombaState.Dead;
                }
            }
            else {
                // Stato Walking: si muove orizzontalmente
                Velocity.X = speed * direction;
                base.Update(gameTime);  // Applica gravità, animazione, ecc.

                // Se esce dallo schermo, non lo disegniamo più
                if (Position.X + width < cameraX || Position.X > cameraX + visibleWidth) {
                    isActive = false;
                }                
                else {
                    isActive = true;
                }
                    
                Vector2 previousPosition = new Vector2(Position.X, Position.Y - Velocity.Y);

                foreach (Obstacle obstacle in SpriteAnimsTestGame._obstacles) {

                    BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, width, height);

                    if (BoundingBox.Intersects(obstacle.BoundingBox)) {

                        if ((previousPosition.Y + height) <= obstacle.BoundingBox.Y + 2) {
                            Position.Y = obstacle.BoundingBox.Top - height;
                            Velocity.Y = 0;
                            isOnGround = true;
                        }

                        if (Velocity.X > 0) {
                                if (Position.X + width >= obstacle.BoundingBox.Left && Position.X + width <= obstacle.BoundingBox.Left + 5 && 
                                !(Position.Y + height <= obstacle.BoundingBox.Top || Position.Y >= obstacle.BoundingBox.Bottom)) {
                                    Position.X = obstacle.BoundingBox.Left - width;
                                    direction = -direction;
                                }
                            }
                        else if (Velocity.X < 0) {
                            if (Position.X <= obstacle.BoundingBox.Right && Position.X >= obstacle.BoundingBox.Right - 5 && 
                            !(Position.Y + height <= obstacle.BoundingBox.Top || Position.Y >= obstacle.BoundingBox.Bottom)) {
                                Position.X = obstacle.BoundingBox.Right;
                                direction = -direction;
                            }
                        }

                        BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, width, height);

                        BoundingBox.X = (int)Position.X;
                        BoundingBox.Y = (int)Position.Y;
                    }
                }
            }
        }

        // Se viene schiacciato dall'alto
        public void Squash()
        {
            if (state == GoombaState.Walking)
            {
                state = GoombaState.Squashed;
                currentAnimation = squashedAnimation;
                Velocity = Vector2.Zero;  // Si ferma
            }
        }

        // Disegno del Goomba
        public override void Draw(SpriteBatch spriteBatch, float cameraX, float scale)
        {
            // Se è morto o fuori dallo schermo, non disegnare
            if (!isActive || state == GoombaState.Dead)
                return;

            base.Draw(spriteBatch, cameraX, scale);
        }
    }
}


