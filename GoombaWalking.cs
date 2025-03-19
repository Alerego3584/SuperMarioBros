using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class GoombaWalking : Animation
{
    public GoombaWalking(Texture2D texture, float frameTime)
        : base(texture, "GoombaWalking", frameTime)
    {
        // Inizializziamo l'array di frame
        frames = new Rectangle[2];

        // Frame 1
        frames[0] = new Rectangle(0, 4, 16, 16);
        // Frame 2
        frames[1] = new Rectangle(30, 4, 16, 16);
    }
}
