using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Jump : Animation
{
    public Jump(Texture2D texture) : base(texture, "Jump", 0.2f)
    {
        frames = new Rectangle[1];
        frames[0] = new Rectangle(359, 0, 17, 16);
    }
}

