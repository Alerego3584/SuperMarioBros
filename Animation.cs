using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

public abstract class Animation
{
    protected Texture2D texture;
    protected float frameTime;     // Tempo tra un frame e l’altro
    protected float timer;         // Tempo accumulato per avanzare di frame
    protected int currentFrame;    // Indice del frame corrente
    protected Rectangle[] frames;  // Lista dei rettangoli che definiscono i frame

    public string Name { get; protected set; }

    // Se vuoi mostrare il frame in debug, aggiungiamo una proprietà pubblica
    public int CurrentFrameIndex => currentFrame;

    public Animation(Texture2D texture, string name, float frameTime)
    {
        this.texture = texture;
        this.Name = name;
        this.frameTime = frameTime;
        this.timer = 0f;
        this.currentFrame = 0;
    }

    public virtual void Update(GameTime gameTime)
    {
        // Avanziamo il timer
        timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        // Se superiamo il tempo stabilito, passiamo al frame successivo
        if (timer >= frameTime)
        {
            timer = 0f;
            currentFrame++;

            // Se abbiamo superato l’ultimo frame, torniamo al primo
            if (currentFrame >= frames.Length)
                currentFrame = 0;
        }
    }

    public virtual void Draw(SpriteBatch spriteBatch, Vector2 position, float scale = 1f, SpriteEffects effects = SpriteEffects.None)
    {
        spriteBatch.Draw(
            texture,
            position,
            frames[currentFrame],
            Color.White,
            0f,
            Vector2.Zero,
            scale,
            effects,
            0f
        );
    }
}

