using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Idle : Animation
{
    public Idle(Texture2D texture) : base(texture, "Idle", 0.2f)
    {
        frames = new Rectangle[1];
        frames[0] = new Rectangle(209, 0, 16, 16);
    }
}


