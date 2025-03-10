using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;
using Windows.Devices.Geolocation;

namespace SpriteAnimsTest {
    public class SpriteAnimsTestGame : Game {
        private GraphicsDeviceManager _graphics;
        private SpriteBatch _spriteBatch;

        // Texture del livello (3376×240)
        private Texture2D _levelTexture;

        // Dimensioni della finestra
        private int screenWidth = 1024;
        private int screenHeight = 768;

        // Variabili per lo scrolling
        private float cameraX = 0f;
        private float maxCameraX = 0f;
        private float scale = 1f;
        private float visibleWidthInTexture = 0f;

        // Player: usa il nuovo Player che gestisce fisica e animazioni
        public Player _player;
        private Texture2D marioTexture;  // Sprite sheet di Mario

        // Musica di sottofondo
        private Song _backgroundMusic;

        // Variabili per la pausa
        private bool isPaused = false;
        private bool wasPPressed = false;
        private SpriteFont pauseFont; // Font personalizzato per la scritta "PAUSE"

        // Variabili per gli orstacoli
        public static List<Obstacle> _obstacles = new List<Obstacle> { };
        private Texture2D obstacleTexture;

        public SpriteAnimsTestGame() {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        protected override void Initialize() {
            _graphics.PreferredBackBufferWidth = screenWidth;
            _graphics.PreferredBackBufferHeight = screenHeight;
            _graphics.ApplyChanges();

            base.Initialize();
        }

        protected override void LoadContent() {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            // Carica la texture del livello
            _levelTexture = Content.Load<Texture2D>("1-1overworld");

            // Calcola il fattore di scala e le dimensioni visibili della texture
            scale = (float)screenHeight / _levelTexture.Height;
            visibleWidthInTexture = screenWidth / scale;
            maxCameraX = _levelTexture.Width - visibleWidthInTexture;
            if (maxCameraX < 0) maxCameraX = 0;

            // Carica la musica di sottofondo
            _backgroundMusic = Content.Load<Song>("groundtheme");
            MediaPlayer.IsRepeating = true;
            MediaPlayer.Volume = 0.5f;
            MediaPlayer.Play(_backgroundMusic);

            // Carica la sprite sheet di Mario (sostituisce il placeholder rosso)
            marioTexture = Content.Load<Texture2D>("mario");

            // Carica i suoni
            SoundEffect jumpSound = Content.Load<SoundEffect>("smb_jump-small");
            SoundEffect deathSound = Content.Load<SoundEffect>("smb_mariodie");

            // Istanzia il player con la sprite sheet di Mario
            _player = new Player(marioTexture, new Vector2(50, 100), jumpSound, deathSound, _backgroundMusic);

            // Carica il font personalizzato (aggiungi PauseFont.spritefont al Content Pipeline)
            pauseFont = Content.Load<SpriteFont>("pauseFont");

            obstacleTexture = Content.Load<Texture2D>("floor");

            // Crea e aggiungi ostacoli alla lista
            for (float x = 0; x <= 200; x += 16) {
                _obstacles.Add(new Obstacle(obstacleTexture, new Vector2(x, 224)));
                _obstacles.Add(new Obstacle(obstacleTexture, new Vector2(x, 208)));
                _obstacles.Add(new Obstacle(obstacleTexture, new Vector2(x, 130)));
            }
            for (float x = 1600; x <= 1800; x += 16) {
                _obstacles.Add(new Obstacle(obstacleTexture, new Vector2(x, 192)));
            }
        }
        // Aggiungi altri ostacoli qui


        protected override void Update(GameTime gameTime) {
            KeyboardState keyboardState = Keyboard.GetState();

            // Gestione della pausa:
            bool pPressed = keyboardState.IsKeyDown(Keys.P);
            if (pPressed && !wasPPressed) {
                isPaused = !isPaused; // Inverte lo stato di pausa
                if (!isPaused) {
                    MediaPlayer.Resume(); // Riprende la musica quando si esce dalla pausa
                }
            }
            wasPPressed = pPressed;

            // Se non siamo in pausa, aggiorna la logica di gioco
            if (!isPaused) {
                // Aggiorna il player (fisica e animazioni)
                _player.Update(gameTime, keyboardState);

                // La camera segue il player centrando la visuale orizzontalmente
                cameraX = _player.Position.X - (visibleWidthInTexture / 2);
                if (cameraX < 0) cameraX = 0;
                if (cameraX > maxCameraX) cameraX = maxCameraX;
            }

            // È buona norma comunque chiamare base.Update
            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime) {
            GraphicsDevice.Clear(Color.CornflowerBlue);

            // ---- Disegno del livello (con point clamp per il pixel art) ----
            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.PointClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone
            );

            Rectangle sourceRect = new Rectangle(
                (int)cameraX,
                0,
                (int)visibleWidthInTexture,
                _levelTexture.Height
            );

            Rectangle destRect = new Rectangle(
                0,
                0,
                (int)(visibleWidthInTexture * scale),
                (int)(_levelTexture.Height * scale)
            );

            _spriteBatch.Draw(_levelTexture, destRect, sourceRect, Color.White);
            _spriteBatch.End();

            // ---- Disegno del player (Mario) con linear filtering per l'antialiasing ----
            _spriteBatch.Begin(
                SpriteSortMode.Deferred,
                BlendState.AlphaBlend,
                SamplerState.LinearClamp,
                DepthStencilState.Default,
                RasterizerState.CullNone
            );

            _player.Draw(_spriteBatch, cameraX, scale);

            // Disegna gli ostacoli
            foreach (var obstacle in _obstacles) {
                obstacle.Draw(_spriteBatch, cameraX, scale);
            }

            _spriteBatch.End();

            // ---- Disegno della scritta "PAUSE" se il gioco è in pausa ----
            if (isPaused) {
                MediaPlayer.Pause(); // Ferma la musica in pausa
                _spriteBatch.Begin();
                string pauseText = "PAUSE";
                Vector2 textSize = pauseFont.MeasureString(pauseText);
                Vector2 textPos = new Vector2((screenWidth - textSize.X) / 2, (screenHeight - textSize.Y) / 2);
                _spriteBatch.DrawString(pauseFont, pauseText, textPos, Color.White);
                _spriteBatch.End();
            }

            base.Draw(gameTime);
        }
    }
}
