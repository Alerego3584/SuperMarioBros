using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Walking : Animation
{
    public Walking(Texture2D texture, float frameTime)
        : base(texture, "Walking", frameTime)
    {
        // Creiamo 3 frame per l'animazione walking
        frames = new Rectangle[3];
        frames[0] = new Rectangle(240, 0, 16, 16);
        frames[1] = new Rectangle(270, 0, 16, 16);
        frames[2] = new Rectangle(300, 0, 16, 16);
    }
}

