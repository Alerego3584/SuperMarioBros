using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SharpDX.Direct3D9;
using System;

namespace SpriteAnimsTest
{
    public enum PlayerState { Alive, Dying, DeadPause }

    public class Player : Entity {
        // Posizione, velocità e costanti di fisica
        private float jumpStrength = 5.75f;
        private float walkAcceleration = 0.1f;
        private float walkFriction = 0.15f;
        private float walkMaxSpeed = 2.0f;
        private float runAcceleration = 0.25f;
        private float runFriction = 0.2f;
        private float runMaxSpeed = 3.25f;
        private float floorY = 768f; // Set the floor height lower than the screen height

        // Stati per salto e skid (già implementati in precedenza)
        private bool isJumping = false;
        private float jumpTimeCounter = 0f;
        private float maxJumpTime = 0.15f;
        private bool isSkidding = false;
        private float skidTimer = 0f;

        // Stato del Player (per gestire la morte)
        private PlayerState state = PlayerState.Alive;
        private float deathPauseTimer = 0f;
        private Vector2 startPosition;  // Posizione iniziale, per il reset

        private SoundEffect jumpSound;    // Suono da riprodurre al salto
        private SoundEffect deathSound;          // Musica da riprodurre alla morte

        private Song backgroundMusic;

        // Animazioni

        private Idle idleAnimation;
        private Jump jumpAnimation;
        private Skid skidAnimation;
        private Walking walkingAnimation;
        private Walking runningAnimation;  // Walking con animazione 2× più veloce
        private Death deathAnimation;

        public Player(Texture2D marioTexture, Vector2 initialPosition, SoundEffect jumpSound, SoundEffect deathSound, Song backgroundMusic) : base(initialPosition, 16, 16) {
            Position = initialPosition;
            startPosition = initialPosition;
            this.jumpSound = jumpSound;
            this.deathSound = deathSound;

            this.backgroundMusic = backgroundMusic;

            idleAnimation = new Idle(marioTexture);
            jumpAnimation = new Jump(marioTexture);
            skidAnimation = new Skid(marioTexture);
            walkingAnimation = new Walking(marioTexture, 0.1f);
            runningAnimation = new Walking(marioTexture, 0.035f);
            deathAnimation = new Death(marioTexture);

            BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, marioTexture.Width, marioTexture.Height);

            currentAnimation = idleAnimation;
        }

        public override void Update(GameTime gameTime) {
            KeyboardState keyboardState = Keyboard.GetState();
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (state == PlayerState.Alive) {
                // Se si preme R, si innesca la sequenza di morte
                if (keyboardState.IsKeyDown(Keys.R) || Position.Y > 240) {
                    Die();
                }
                else {
                    // --- Gestione del movimento orizzontale ---
                    bool isRunning = keyboardState.IsKeyDown(Keys.LeftShift);
                    bool pressingLeft = keyboardState.IsKeyDown(Keys.Left);
                    bool pressingRight = keyboardState.IsKeyDown(Keys.Right);

                    float currentAcceleration = isRunning ? runAcceleration : walkAcceleration;
                    float currentFriction = isRunning ? runFriction : walkFriction;
                    float currentMaxSpeed = isRunning ? runMaxSpeed : walkMaxSpeed;

                    if (pressingRight)
                        Velocity.X += currentAcceleration;
                    else if (pressingLeft)
                        Velocity.X -= currentAcceleration;
                    else {
                        if (Velocity.X > 0) {
                            Velocity.X -= currentFriction;
                            if (Velocity.X < 0) Velocity.X = 0;
                        }
                        else if (Velocity.X < 0) {
                            Velocity.X += currentFriction;
                            if (Velocity.X > 0) Velocity.X = 0;
                        }
                    }

                    if (Velocity.X > currentMaxSpeed) Velocity.X = currentMaxSpeed;
                    if (Velocity.X < -currentMaxSpeed) Velocity.X = -currentMaxSpeed;

                    // --- Rilevamento cambio direzione per Skid ---
                    bool changingToLeft = (Velocity.X > 0 && pressingLeft);
                    bool changingToRight = (Velocity.X < 0 && pressingRight);
                    if ((changingToLeft || changingToRight) && !isSkidding && isOnGround) {
                        isSkidding = true;
                        skidTimer = 0.15f;
                        currentAnimation = skidAnimation;
                    }
                    if (isSkidding) {
                        skidTimer -= delta;
                        if (skidTimer <= 0f)
                            isSkidding = false;
                    }

                    Position.X += Velocity.X;
                    BoundingBox.X = (int)Position.X;

                    // --- Gestione del salto con altezza variabile ---
                    if (keyboardState.IsKeyDown(Keys.Z)) {
                        if (isOnGround && !isJumping) {
                            jumpSound.Play();

                            isJumping = true;
                            jumpTimeCounter = maxJumpTime;
                            Velocity.Y = -jumpStrength;
                            isOnGround = false;
                        }
                        else if (isJumping && jumpTimeCounter > 0) {
                            Velocity.Y = -jumpStrength;
                            jumpTimeCounter -= delta;
                        }
                    }
                    else {
                        isJumping = false;
                    }

                    Velocity.Y += gravity;
                    Position.Y += Velocity.Y;

                    BoundingBox.Y = (int)Position.Y;

                    // Store previous position
                    Vector2 previousPosition = new Vector2(Position.X, Position.Y - Velocity.Y);
                    
                    foreach (var obstacle in SpriteAnimsTestGame._obstacles) {
                        // Update player's bounding box to match current position
                        BoundingBox = new Rectangle((int)Position.X, (int)Position.Y, width, height);

                        if (BoundingBox.Intersects(obstacle.BoundingBox)) {
                            // Check if we're moving downward (falling)
                            if (Velocity.Y > 0) {
                                // Check if previous position was above the obstacle
                                if ((previousPosition.Y + height) <= obstacle.BoundingBox.Y + 2) {
                                    // Landing on top of obstacle
                                    Position.Y = obstacle.BoundingBox.Top - height;
                                    Velocity.Y = 0;
                                    isOnGround = true;
                                }
                            }
                            // Check if we're moving upward (jumping)
                            else if (Velocity.Y < 0) {
                                // Only consider bottom collisions if player's top is near obstacle's bottom
                                // and player is actually colliding with the obstacle's bottom
                                if (Position.Y <= obstacle.BoundingBox.Bottom &&
                                    Position.Y >= obstacle.BoundingBox.Bottom - 5 &&
                                    Position.X + width > obstacle.BoundingBox.Left &&
                                    Position.X < obstacle.BoundingBox.Right) {
                                    // Hitting obstacle from below
                                    Position.Y = obstacle.BoundingBox.Bottom;
                                    Velocity.Y = 0;
                                    isJumping = false;
                                }
                            }

                            // Check horizontal collisions
                            // Moving right
                            if (Velocity.X > 0) {
                                // Make sure we're actually at the left edge, not just anywhere inside
                                if (Position.X + width >= obstacle.BoundingBox.Left &&
                                    Position.X + width <= obstacle.BoundingBox.Left + 5 &&
                                    !(Position.Y + height <= obstacle.BoundingBox.Top || Position.Y >= obstacle.BoundingBox.Bottom)) {
                                    Position.X = obstacle.BoundingBox.Left - width;
                                    Velocity.X = 0;
                                }
                            }
                            // Moving left
                            else if (Velocity.X < 0) {
                                // Make sure we're actually at the right edge, not just anywhere inside
                                if (Position.X <= obstacle.BoundingBox.Right &&
                                    Position.X >= obstacle.BoundingBox.Right - 5 &&
                                    !(Position.Y + height <= obstacle.BoundingBox.Top || Position.Y >= obstacle.BoundingBox.Bottom)) {
                                    Position.X = obstacle.BoundingBox.Right;
                                    Velocity.X = 0;
                                }
                            }

                            // Update bounding box after position changes
                            BoundingBox.X = (int)Position.X;
                            BoundingBox.Y = (int)Position.Y;
                        }
                    }

                    foreach (var goomba in SpriteAnimsTestGame.goombas)
                    {
                        if (!goomba.IsDead && BoundingBox.Intersects(goomba.BoundingBox))
                        {
                            // Se il Player sta cadendo e la parte inferiore del Player è quasi sopra la parte superiore del Goomba...
                            if (Velocity.Y > 0 && BoundingBox.Bottom <= goomba.BoundingBox.Top + 5)
                            {
                                // Il Player schiaccia il Goomba
                                goomba.Squash();
                                // Piccolo rimbalzo verso l'alto per il Player
                                Velocity = new Vector2(Velocity.X, -2f);
                            }
                            else
                            {
                                // Collisione laterale o dal basso: il Player muore
                                Die();
                            }
                        }
                    }

                    // Collisione con il pavimento (solo in modalità Alive)
                    float maxY = floorY - height;
                    if (Position.Y > maxY) {
                        Position.Y = maxY;
                        Velocity.Y = 0f;
                        isOnGround = true;
                    }

                    if (Position.X < 0) Position.X = 0;
                    if (Position.X > 3376 - width) Position.X = 3376 - width;

                    // --- Scelta dell'animazione (se non in skid) ---
                    if (!isSkidding) {
                        if (!isOnGround)
                            currentAnimation = jumpAnimation;
                        else {
                            if (Math.Abs(Velocity.X) > 0)
                                currentAnimation = isRunning ? runningAnimation : walkingAnimation;
                            else
                                currentAnimation = idleAnimation;
                        }
                    }

                    if (Position.X > 3168 && Position.X < 3184 && Position.Y > 70)
                    {
                        SpriteAnimsTestGame.win = true;
                    }
                }
            }
            else if (state == PlayerState.Dying) {
                // Durante la sequenza di morte ignoriamo gli input.
                // Aggiorniamo la velocità verticale con la gravità per simulare la caduta.
                Velocity.Y += gravity;
                Position.Y += Velocity.Y;

                // Quando Mario esce dallo schermo (in basso, qui usiamo 768 come altezza della finestra),
                // passiamo allo stato DeadPause.
                if (Position.Y > 768) {
                    state = PlayerState.DeadPause;
                    deathPauseTimer = 3f;
                }
            }
            else if (state == PlayerState.DeadPause) {
                deathPauseTimer -= delta;
                if (deathPauseTimer <= 0)
                    Reset();
            }

            currentAnimation.Update(gameTime);
        }

        private void Reset() {
            // Resetta la posizione e lo stato del Player al valore iniziale
            Position = startPosition;
            Velocity = Vector2.Zero;
            state = PlayerState.Alive;
            currentAnimation = idleAnimation;
            isJumping = false;
            isSkidding = false;

            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.Play(backgroundMusic);
        }

        public override void Die()
        {
            // Se il Player è ancora vivo, cambia stato a Dying
            if (state == PlayerState.Alive)
            {
                MediaPlayer.Stop();
                MediaPlayer.IsRepeating = false;
                deathSound.Play();

                state = PlayerState.Dying;
                currentAnimation = deathAnimation;
                // Ferma il movimento orizzontale e applica una spinta verticale verso l'alto
                Velocity = new Vector2(0, -6f);
            }
        }
    }
}








