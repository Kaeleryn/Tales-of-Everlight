using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Mail;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using Microsoft.Xna.Framework.Input;

namespace Tales_of_Everlight;

public class Main : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Camera _camera;
    private Level1 _level1;

    private bool isHudVisible = false;

    private Texture2D mainHeroSprite;
    private Texture2D squareSprite;
    private Texture2D rectangleTexture;
    private Texture2D hudTexture;

    private Color backgroundColor = new Color(145, 221, 207, 255);

    private MainHero mainHero = new MainHero();
    private Square square = new Square();

    private KeyboardState keyState;

    private const int TILESIZE = 64;


    private List<Rectangle> intersections;

    private Dictionary<Vector2, int> LoadMap(string filepath)
    {
        Dictionary<Vector2, int> result = new();

        StreamReader reader = new(filepath);
        int y = 0;
        string line;
        while ((line = reader.ReadLine()) != null)
        {
            string[] items = line.Split(',');

            for (int x = 0; x < items.Length; x++)
            {
                if (int.TryParse(items[x], out int value))
                {
                    if (value > -1)
                    {
                        result[new Vector2(x, y)] = value;
                    }
                }
            }

            y++;
        }

        return result;
    } // яка  зчитує карту з файлу

    private Dictionary<Vector2, int> foreground;
    private Dictionary<Vector2, int> collisions;
    private Texture2D hitboxTexture;

    public Main()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        _graphics.PreferredBackBufferHeight = 1080;
        _graphics.PreferredBackBufferWidth = 1920
            ;

        _camera = new Camera(new Rectangle(0, 0, _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight));

        // foreground = LoadMap("Content/map/level1_foreground.csv");
        // collisions = LoadMap("Content/map/level1_collisions.csv");

        intersections = new();
        _level1 = new Level1();
    }

    protected override void Initialize()
    {
        // TODO: Add your initialization logic here

        base.Initialize();

        _graphics.IsFullScreen = true;
        _graphics.ApplyChanges();

        mainHero.Position = new Vector2(100, 100);
        square.Position = new Vector2(1000, 100);
        
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);

        mainHeroSprite = Content.Load<Texture2D>("animatedSprite");
        squareSprite = Content.Load<Texture2D>("enemy1");
        mainHero = new MainHero(mainHeroSprite, new Vector2(500, 1000), 1, 6);
        square = new Square(squareSprite, new Vector2(1000, 100), 5, 1);

   
        hudTexture = Content.Load<Texture2D>("hud+");


        rectangleTexture = new Texture2D(GraphicsDevice, 1, 1);
        rectangleTexture.SetData(new Color[] { new(255, 0, 0, 255) });

        hitboxTexture = new Texture2D(GraphicsDevice, 1, 1);
        hitboxTexture.SetData(new[] { Color.White });
        
        _level1.Initialize(Content);


        // TODO: use this.Content to load your game content here
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        Vector2 characterPosition = mainHero.Position;

        if (characterPosition.X >= (_graphics.PreferredBackBufferWidth / 2) && characterPosition.X <= 3000)
        {
            _camera.Position = new Vector2(characterPosition.X, _camera.Position.Y);
        }


        // Update camera position based on player movement
        
        if (Keyboard.GetState().IsKeyDown(Keys.I)) isHudVisible = !isHudVisible;


        mainHero.CollisionHandler(_level1.Collisions, TILESIZE);
        mainHero.MovementHandler();
        
        square.CollisionHandler(_level1.Collisions, TILESIZE);
        square.MovementHandler();


        // TODO: Add your update logic here

        base.Update(gameTime);
    }

    public List<Rectangle> getIntersectingTilesHorizontal(Rectangle target)
    {
        List<Rectangle> intersections = new();

        int leftTile = target.Left / TILESIZE;
        int rightTile = (target.Right - 1) / TILESIZE;
        int topTile = target.Top / TILESIZE;
        int bottomTile = (target.Bottom - 1) / TILESIZE;

        for (int x = leftTile; x <= rightTile; x++)
        {
            for (int y = topTile; y <= bottomTile; y++)
            {
                intersections.Add(new Rectangle(x, y, TILESIZE, TILESIZE));
            }
        }

        return intersections;
    }

    public List<Rectangle> getIntersectingTilesVertical(Rectangle target)
    {
        List<Rectangle> intersections = new();

        int leftTile = target.Left / TILESIZE;
        int rightTile = (target.Right - 1) / TILESIZE;
        int topTile = target.Top / TILESIZE;
        int bottomTile = (target.Bottom - 1) / TILESIZE;

        for (int x = leftTile; x <= rightTile; x++)
        {
            for (int y = topTile; y <= bottomTile; y++)
            {
                intersections.Add(new Rectangle(x, y, TILESIZE, TILESIZE));
            }
        }

        return intersections;
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(backgroundColor);
        // TODO: Add your drawing code here

        _spriteBatch.Begin(transformMatrix: _camera.GetTransformation()); //spritebatch begin

        mainHero.Draw(_spriteBatch, mainHero.Position, hitboxTexture);
        square.Draw(_spriteBatch, square.Position, hitboxTexture);

        
        _level1.Draw(_spriteBatch);

        // foreach (var item in foreground)
        // {
        //     int display_tilesize = 64;
        //     int num_tiles_per_row = 16;
        //     int pixel_tilesize = 64;
        //
        //     Rectangle drect = new(
        //         (int)item.Key.X * display_tilesize,
        //         (int)item.Key.Y * display_tilesize,
        //         display_tilesize,
        //         display_tilesize
        //     );
        //
        //
        //     int x = item.Value % num_tiles_per_row;
        //     int y = item.Value / num_tiles_per_row;
        //
        //     Rectangle src = new(
        //         x * pixel_tilesize,
        //         y * pixel_tilesize,
        //         pixel_tilesize,
        //         pixel_tilesize
        //     );
        //
        //     _spriteBatch.Draw(textureAtlas_foreground, drect, src, Color.White);
        // }
        //
        // foreach (var item in collisions)
        // {
        //     int display_tilesize = 64;
        //     int num_tiles_per_row = 3;
        //     int pixel_tilesize = 64;
        //
        //     Rectangle drect = new(
        //         (int)item.Key.X * display_tilesize,
        //         (int)item.Key.Y * display_tilesize,
        //         display_tilesize,
        //         display_tilesize
        //     );
        //
        //
        //     int x = item.Value % num_tiles_per_row;
        //     int y = item.Value / num_tiles_per_row;
        //
        //     Rectangle src = new(
        //         x * pixel_tilesize,
        //         y * pixel_tilesize,
        //         pixel_tilesize,
        //         pixel_tilesize
        //     );
        //
        //     // _spriteBatch.Draw(textureAtlas_collisions, drect, src, Color.White);
        // }

        foreach (var rect in intersections)
        {
            DrawRectHollow(
                _spriteBatch,
                new Rectangle(
                    rect.X * TILESIZE,
                    rect.Y * TILESIZE,
                    TILESIZE,
                    TILESIZE
                ),
                4
            );
        }

        if(isHudVisible) _spriteBatch.Draw(hudTexture, new Vector2(0, 0), Color.White);
        
        _spriteBatch.End(); //spritebatch end 

        base.Draw(gameTime);
    }

    public void DrawRectHollow(SpriteBatch spriteBatch, Rectangle rect, int thickness)
    {
        spriteBatch.Draw(
            rectangleTexture,
            new Rectangle(
                rect.X,
                rect.Y,
                rect.Width,
                thickness
            ),
            Color.White
        );
        spriteBatch.Draw(
            rectangleTexture,
            new Rectangle(
                rect.X,
                rect.Bottom - thickness,
                rect.Width,
                thickness
            ),
            Color.White
        );
        spriteBatch.Draw(
            rectangleTexture,
            new Rectangle(
                rect.X,
                rect.Y,
                thickness,
                rect.Height
            ),
            Color.White
        );
        spriteBatch.Draw(
            rectangleTexture,
            new Rectangle(
                rect.Right - thickness,
                rect.Y,
                thickness,
                rect.Height
            ),
            Color.White
        );
    }
}