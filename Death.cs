using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class Death : Animation
{
    public Death(Texture2D texture)
        : base(texture, "Death", 0.2f)
    {
        // Qui ipotizziamo che il frame per la morte si trovi in X:400, Y:0 con dimensioni 16×16.
        frames = new Rectangle[1];
        frames[0] = new Rectangle(389, 15, 16, 16);
    }
}
