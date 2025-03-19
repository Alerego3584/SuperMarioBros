using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public class GoombaDeath : Animation
{
    public GoombaDeath(Texture2D texture)
        : base(texture, "GoombaSquashed", 0.1f)
    {
        // Inizializziamo l'array di frame (uno solo per l'immagine schiacciata).
        frames = new Rectangle[1];

        frames[0] = new Rectangle(60, 8, 16, 8);
    }

    // Un solo frame, quindi volendo potresti anche lasciare l'Update base o svuotarlo per evitare il loop
    public override void Update(GameTime gameTime)
    {
        // Non avanza di frame, resta sempre su quello schiacciato
    }
}
