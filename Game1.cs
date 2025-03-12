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
        private Texture2D floorTX;
        private Texture2D blockTX;
        private Texture2D brickTX;
        private Texture2D luckyblockTX;
        private Texture2D tubesmallTX;
        private Texture2D tubemediumTX;
        private Texture2D tubelargeTX;

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

            floorTX = Content.Load<Texture2D>("floor");
            blockTX = Content.Load<Texture2D>("block");
            brickTX = Content.Load<Texture2D>("brick");
            luckyblockTX = Content.Load<Texture2D>("luckyblock");
            tubesmallTX = Content.Load<Texture2D>("tubesmall");
            tubemediumTX = Content.Load<Texture2D>("tubemedium");
            tubelargeTX = Content.Load<Texture2D>("tubelarge");

            // Crea e aggiungi ostacoli alla lista
            for (float x = 0; x <= 1096; x += 16) {
                _obstacles.Add(new Obstacle(floorTX, new Vector2(x, 224)));
                _obstacles.Add(new Obstacle(floorTX, new Vector2(x, 208)));
            }
            for (float x = 1136; x <= 2432; x += 16) {
                _obstacles.Add(new Obstacle(floorTX, new Vector2(x, 224)));
                _obstacles.Add(new Obstacle(floorTX, new Vector2(x, 208)));    
            }
            for (float x = 2480; x <= 3360; x += 16) {
                _obstacles.Add(new Obstacle(floorTX, new Vector2(x, 224)));
                _obstacles.Add(new Obstacle(floorTX, new Vector2(x, 208)));    
            }

            _obstacles.Add(new Obstacle(brickTX, new Vector2(320, 144)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(352, 144)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(384, 144)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1232, 144)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1264, 144)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1280, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1296, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1312, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1328, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1344, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1360, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1376, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1392, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1456, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1472, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1488, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1504, 144)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1600, 144)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1616, 144)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1888, 144)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1936, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1952, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(1968, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(2048, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(2064, 144)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(2080, 144)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(2096, 80)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(2688, 144)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(2704, 144)));
            _obstacles.Add(new Obstacle(brickTX, new Vector2(2736, 144)));


            _obstacles.Add(new Obstacle(luckyblockTX, new Vector2(256, 144)));
            _obstacles.Add(new Obstacle(luckyblockTX, new Vector2(336, 144)));
            _obstacles.Add(new Obstacle(luckyblockTX, new Vector2(352, 80)));
            _obstacles.Add(new Obstacle(luckyblockTX, new Vector2(368, 144)));
            _obstacles.Add(new Obstacle(luckyblockTX, new Vector2(1248, 144)));
            _obstacles.Add(new Obstacle(luckyblockTX, new Vector2(1504, 80)));
            _obstacles.Add(new Obstacle(luckyblockTX, new Vector2(1696, 144)));
            _obstacles.Add(new Obstacle(luckyblockTX, new Vector2(1744, 144)));
            _obstacles.Add(new Obstacle(luckyblockTX, new Vector2(1744, 80)));
            _obstacles.Add(new Obstacle(luckyblockTX, new Vector2(1792, 144)));
            _obstacles.Add(new Obstacle(luckyblockTX, new Vector2(2064, 80)));
            _obstacles.Add(new Obstacle(luckyblockTX, new Vector2(2080, 80)));


            _obstacles.Add(new Obstacle(blockTX, new Vector2(2144, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2160, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2160, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2176, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2176, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2176, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2192, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2192, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2192, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2192, 144)));

            _obstacles.Add(new Obstacle(blockTX, new Vector2(2240, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2240, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2240, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2240, 144)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2256, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2256, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2256, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2272, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2272, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2288, 192)));

            _obstacles.Add(new Obstacle(blockTX, new Vector2(2368, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2384, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2384, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2400, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2400, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2400, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2416, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2416, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2416, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2416, 144)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2432, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2432, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2432, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2432, 144)));

            _obstacles.Add(new Obstacle(blockTX, new Vector2(2480, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2480, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2480, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2480, 144)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2496, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2496, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2496, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2512, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2512, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2528, 192)));

            _obstacles.Add(new Obstacle(blockTX, new Vector2(2896, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2912, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2912, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2928, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2928, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2928, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2944, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2944, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2944, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2944, 144)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2960, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2960, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2960, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2960, 144)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2960, 128)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2976, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2976, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2976, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2976, 144)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2976, 128)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2976, 112)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2992, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2992, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2992, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2992, 144)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2992, 128)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2992, 112)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(2992, 96)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3008, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3008, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3008, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3008, 144)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3008, 128)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3008, 112)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3008, 96)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3008, 80)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3024, 192)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3024, 176)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3024, 160)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3024, 144)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3024, 128)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3024, 112)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3024, 96)));
            _obstacles.Add(new Obstacle(blockTX, new Vector2(3024, 80)));

            _obstacles.Add(new Obstacle(blockTX, new Vector2(3168, 192)));


            _obstacles.Add(new Obstacle(tubesmallTX, new Vector2(448, 176)));
            _obstacles.Add(new Obstacle(tubesmallTX, new Vector2(2608, 176)));
            _obstacles.Add(new Obstacle(tubesmallTX, new Vector2(2864, 176)));
            _obstacles.Add(new Obstacle(tubemediumTX, new Vector2(608, 160)));
            _obstacles.Add(new Obstacle(tubelargeTX, new Vector2(736, 144)));
            _obstacles.Add(new Obstacle(tubelargeTX, new Vector2(912, 144)));


        // Aggiungi altri ostacoli qui
        }

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
