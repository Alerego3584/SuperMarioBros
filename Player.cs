﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using SharpDX.Direct3D9;
using System;

namespace SpriteAnimsTest
{
    public enum PlayerState { Alive, Dying, DeadPause }

    public class Player
    {
        // Posizione, velocità e costanti di fisica
        public Vector2 Position;
        private Vector2 Velocity;
        private float gravity = 0.35f;
        private float jumpStrength = 5.75f;
        private float walkAcceleration = 0.1f;
        private float walkFriction = 0.15f;
        private float walkMaxSpeed = 2.0f;
        private float runAcceleration = 0.25f;
        private float runFriction = 0.2f;
        private float runMaxSpeed = 3.25f;
        private int width = 16;
        private int height = 16;
        private float floorY = 208.3f;

        // Stati per salto e skid (già implementati in precedenza)
        private bool isOnGround = false;
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
        private Animation currentAnimation;
        private Idle idleAnimation;
        private Jump jumpAnimation;
        private Skid skidAnimation;
        private Walking walkingAnimation;
        private Walking runningAnimation;  // Walking con animazione 2× più veloce
        private Death deathAnimation;

        public Rectangle BoundingBox;

        public Player(Texture2D marioTexture, Vector2 initialPosition, SoundEffect jumpSound, SoundEffect deathSound, Song backgroundMusic)
        {
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

        public async void Update(GameTime gameTime, KeyboardState keyboardState) {
            float delta = (float)gameTime.ElapsedGameTime.TotalSeconds;

            if (state == PlayerState.Alive) {
                // Se si preme R, si innesca la sequenza di morte
                if (keyboardState.IsKeyDown(Keys.R)) {
                    MediaPlayer.Stop();
                    MediaPlayer.IsRepeating = false;
                    deathSound.Play();

                    state = PlayerState.Dying;
                    currentAnimation = deathAnimation;
                    // Ferma il movimento orizzontale e imposta una spinta iniziale verso l'alto
                    Velocity = new Vector2(0, -6.0f);

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

                    foreach (var obstacle in SpriteAnimsTestGame._obstacles) {
                        if (BoundingBox.Intersects(obstacle.BoundingBox)) {
                            // Collisione rilevata
                            Rectangle intersection = Rectangle.Intersect(BoundingBox, obstacle.BoundingBox);

                            if (intersection.Width > intersection.Height) {
                                // Collisione verticale
                                if (BoundingBox.Top < obstacle.BoundingBox.Bottom) {
                                    // Collisione dall'alto

                                    Position.Y = obstacle.BoundingBox.Top - height;
                                    Velocity.Y = 0;
                                    isOnGround = true;
                                }
                                else {
                                    // Collisione dal basso
                                    Position.Y = obstacle.BoundingBox.Bottom;
                                    Velocity.Y = 0;
                                }

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

        private void Reset()
        {
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

        public void Draw(SpriteBatch spriteBatch, float cameraX, float scale)
        {
            float screenX = (Position.X - cameraX) * scale;
            float screenY = Position.Y * scale;
            SpriteEffects effects = (Velocity.X < 0) ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            currentAnimation.Draw(spriteBatch, new Vector2(screenX, screenY), scale, effects);
        }
    }
}








