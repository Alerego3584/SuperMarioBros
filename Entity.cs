using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System.Collections.Generic;

namespace SpriteAnimsTest
{
    public abstract class Entity
    {
        public Vector2 Position;
        public Vector2 Velocity;
        protected float gravity = 0.35f;
        protected int width;
        protected int height;
        public Rectangle BoundingBox;
        protected bool isOnGround = false;


        // Animazione corrente (le classi derivate dovranno inizializzarla)
        protected Animation currentAnimation;

        public Entity(Vector2 initialPosition, int width, int height)
        {
            Position = initialPosition;
            this.width = width;
            this.height = height;
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, width, height);
        }

        // Aggiornamento base: applica fisica, sposta l'entità ed aggiorna l'animazione
        public virtual void Update(GameTime gameTime)
        {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;
            Velocity.Y += gravity;
            Position += Velocity;
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, width, height);
            currentAnimation?.Update(gameTime);
        }

        // Disegna l'entità; il flip orizzontale viene applicato in base alla direzione della velocità
        public virtual void Draw(SpriteBatch spriteBatch, float cameraX, float scale)
        {
            float screenX = (Position.X - cameraX) * scale;
            float screenY = Position.Y * scale;
            SpriteEffects effects = (Velocity.X < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            currentAnimation?.Draw(spriteBatch, new Vector2(screenX, screenY), scale, effects);
        }

        public virtual void Die() {}
    }
}

