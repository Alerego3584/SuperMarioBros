using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System.Collections.Generic;

namespace SpriteAnimsTest {
    public class Obstacle {
        public Vector2 Position { get; set; }
        public Texture2D Texture { get; set; }
        public Rectangle BoundingBox { get; set; }

        public Obstacle(Texture2D texture, Vector2 position) {
            Texture = texture;
            Position = position;
        }

        public void Draw(SpriteBatch spriteBatch, float cameraX, float scale) {
            spriteBatch.Draw(Texture, Position * scale - new Vector2(cameraX * scale, 0), null, Color.White, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, Texture.Width, Texture.Height);
        }
    }
}
