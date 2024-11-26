using System.Collections.Generic;
using System.IO;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;

namespace Tales_of_Everlight
{
    public class Main : Game
    {
        // Graphics device manager
        private GraphicsDeviceManager _graphics;

        // Sprite batch for drawing
        private SpriteBatch _spriteBatch;

        // Camera for the game
        private Camera _camera;

        // Level 1 of the game
        private Level1 _level1;

        // Flags for menu and HUD visibility
        private bool _isMainMenuVisible;
        private bool _isHudVisible;

        // Textures for the main hero, square, rectangle, and HUD
        private Texture2D _mainHeroSprite;
        private Texture2D _squareSprite;
        private Texture2D _rectangleTexture;

        private Texture2D _hudTexture;

        // Font for the HUD
        private SpriteFont _hudFont;

        // Background color
        private Color _backgroundColor = new(145, 221, 207, 255);

        // Main hero and square objects
        private MainHero _mainHero = new();
        private Square _square = new();

        // Keyboard states
        private KeyboardState _keyState;
        private KeyboardState _previousKeyState;

        // Tile size constant
        private const int Tilesize = 64;

        // List of intersections and dictionaries for foreground and collisions
        private List<Rectangle> intersections;
        private Dictionary<Vector2, int> foreground;

        private Dictionary<Vector2, int> collisions;

        // Texture for hitboxes
        private Texture2D _hitboxTexture;

        // Constructor
        public Main()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            _graphics.PreferredBackBufferHeight = 1080;
            _graphics.PreferredBackBufferWidth = 1920;

            _camera = new Camera(new Rectangle(0, 0, _graphics.PreferredBackBufferWidth,
                _graphics.PreferredBackBufferHeight));
            intersections = new List<Rectangle>();
            _level1 = new Level1();
        }

        // Initializes the game
        protected override void Initialize()
        {
            base.Initialize();
            _previousKeyState = Keyboard.GetState();
            _graphics.IsFullScreen = true;
            _graphics.ApplyChanges();

            _mainHero.Position = new Vector2(100, 100);
            _square.Position = new Vector2(1000, 100);
        }

        // Loads the game content
        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);

            _mainHeroSprite = Content.Load<Texture2D>("animatedSprite");
            _squareSprite = Content.Load<Texture2D>("enemy1");
            _mainHero = new MainHero(_mainHeroSprite, new Vector2(500, 1000), 1, 6);
            _square = new Square(_squareSprite, new Vector2(1000, 100), 5, 1);

            _hudTexture = Content.Load<Texture2D>("hud+");
            _hudFont = Content.Load<SpriteFont>("hudFont");

            _rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
            _rectangleTexture.SetData(new Color[] { new(255, 0, 0, 255) });

            _hitboxTexture = new Texture2D(GraphicsDevice, 1, 1);
            _hitboxTexture.SetData(new[] { Color.White });

            _level1.Initialize(Content);
        }

        // Updates the game state
        protected override void Update(GameTime gameTime)
        {
            HandleInput();

            if (!_isMainMenuVisible)
            {
                UpdateCameraPosition();
                UpdateGameElements(gameTime);
            }

            base.Update(gameTime);
        }

        // Handles user input
        private void HandleInput()
        {
            KeyboardState currentKeyState = Keyboard.GetState();

            if (currentKeyState.IsKeyDown(Keys.Delete) && !_previousKeyState.IsKeyDown(Keys.Delete))
                Exit();

            if (currentKeyState.IsKeyDown(Keys.I) && !_previousKeyState.IsKeyDown(Keys.I))
                _isHudVisible = !_isHudVisible;

            if (currentKeyState.IsKeyDown(Keys.Escape) && !_previousKeyState.IsKeyDown(Keys.Escape))
                _isMainMenuVisible = !_isMainMenuVisible;

            _previousKeyState = currentKeyState;
        }

        // Updates the camera position based on the main hero's position
        private void UpdateCameraPosition()
        {
            Vector2 characterPosition = _mainHero.Position;

            if (characterPosition.X >= (_graphics.PreferredBackBufferWidth / 2) && characterPosition.X <= 3000)
            {
                _camera.Position = new Vector2(characterPosition.X, _camera.Position.Y);
            }
        }

        // Updates the game elements
        private void UpdateGameElements(GameTime gameTime)
        {
            _mainHero.CollisionHandler(_level1.Collisions, Tilesize);
            _mainHero.MovementHandler(gameTime);

            _square.CollisionHandler(_level1.Collisions, Tilesize);
            _square.MovementHandler();
        }

        // Draws the game elements
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(_backgroundColor);

            if (_isMainMenuVisible)
            {
                DrawMainMenu();
            }
            else
            {
                DrawGameElements();
                DrawHudElements();
            }

            base.Draw(gameTime);
        }

        // Draws the game elements
        private void DrawGameElements()
        {
            _spriteBatch.Begin(transformMatrix: _camera.GetTransformation());

            _mainHero.Draw(_spriteBatch, _mainHero.Position, _hitboxTexture);
            _square.Draw(_spriteBatch, _square.Position, _hitboxTexture);
            _level1.Draw(_spriteBatch);

            foreach (var rect in intersections)
            {
                DrawRectHollow(_spriteBatch, new Rectangle(rect.X * Tilesize, rect.Y * Tilesize, Tilesize, Tilesize),
                    4);
            }

            _spriteBatch.End();
        }

        // Draws the main menu
        private void DrawMainMenu()
        {
            _spriteBatch.Begin();

            // Draw the main menu background
            _spriteBatch.Draw(_rectangleTexture,
                new Rectangle(0, 0, _graphics.PreferredBackBufferWidth, _graphics.PreferredBackBufferHeight),
                Color.Black * 0.5f);

            // Draw the main menu text
            _spriteBatch.DrawString(_hudFont, "THE GAME IS PAUSED", new Vector2(800, 300), Color.White);
            _spriteBatch.DrawString(_hudFont, "A, D: Move the main hero", new Vector2(800, 450), Color.White);
            _spriteBatch.DrawString(_hudFont, "Space: Jump", new Vector2(800, 500), Color.White);
            _spriteBatch.DrawString(_hudFont, "Escape: Pause", new Vector2(800, 550), Color.White);
            _spriteBatch.DrawString(_hudFont, "I: Toggle HUD visibility", new Vector2(800, 600), Color.White);
            _spriteBatch.DrawString(_hudFont, "Delete: Exit the game", new Vector2(800, 650), Color.White);
            _spriteBatch.End();
        }

        // Draws the HUD elements
        private void DrawHudElements()
        {
            _spriteBatch.Begin();

            if (_isHudVisible)
            {
                _spriteBatch.DrawString(_hudFont, $"Velocity: {_mainHero.Velocity}", new Vector2(10, 0), Color.White);
                _spriteBatch.DrawString(_hudFont, $"isMoving: {_mainHero.IsMoving}", new Vector2(10, 30), Color.White);
                _spriteBatch.DrawString(_hudFont, $"isJumping: {_mainHero.IsJumping}", new Vector2(10, 60), Color.White);
            }

            _spriteBatch.End();
        }

        // Loads the map from a file
        private Dictionary<Vector2, int> LoadMap(string filepath)
        {
            Dictionary<Vector2, int> result = new Dictionary<Vector2, int>();

            using (StreamReader reader = new StreamReader(filepath))
            {
                int y = 0;
                string line;
                while ((line = reader.ReadLine()) != null)
                {
                    string[] items = line.Split(',');

                    for (int x = 0; x < items.Length; x++)
                    {
                        if (int.TryParse(items[x], out int value) && value > -1)
                        {
                            result[new Vector2(x, y)] = value;
                        }
                    }

                    y++;
                }
            }

            return result;
        }

        // Draws a hollow rectangle
        private void DrawRectHollow(SpriteBatch spriteBatch, Rectangle rect, int thickness)
        {
            spriteBatch.Draw(_rectangleTexture, new Rectangle(rect.X, rect.Y, rect.Width, thickness), Color.White);
            spriteBatch.Draw(_rectangleTexture, new Rectangle(rect.X, rect.Bottom - thickness, rect.Width, thickness),
                Color.White);
            spriteBatch.Draw(_rectangleTexture, new Rectangle(rect.X, rect.Y, thickness, rect.Height), Color.White);
            spriteBatch.Draw(_rectangleTexture, new Rectangle(rect.Right - thickness, rect.Y, thickness, rect.Height),
                Color.White);
        }
    }
}