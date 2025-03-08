using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Idle : Animation
{
    public Idle(Texture2D texture)
        : base(texture, "Idle", 0.2f)
    {
        // Esempio: x=80, y=0, w=16, h=32 (VALORI FINTI, misurali tu!)
        frames = new Rectangle[1];
        frames[0] = new Rectangle(209, 0, 16, 16);
    }
}


