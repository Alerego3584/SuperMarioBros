using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Death : Animation
{
    public Death(Texture2D texture) : base(texture, "Death", 0.2f)
    {
        frames = new Rectangle[1];
        frames[0] = new Rectangle(389, 15, 16, 16);
    }
}
