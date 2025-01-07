using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Tales_of_Everlight;

public abstract class Actor
{
    private bool _isFacingRight = true; // Indicates if the actor is facing right
    private Texture2D _texture; // Texture of the actor
    private Vector2 _position; // Position of the actor
    private Vector2 _velocity; // Velocity of the actor
    private Rectangle _rect { get; set; } // Rectangle of the actor

    public Rectangle Rect
    {
        get => _rect;
        set => _rect = value;
    }

    private Rectangle _srect { get; set; } // Source rectangle of the actor

    public Rectangle Srect
    {
        get => _srect;
        set => _srect = value;
    }

    private const float Gravity = 0.65f; // Gravity constant
    private const double FrameTime = 0.1; // Time in seconds between frames

    // Bounding box of the actor
    private Rectangle _boundingBox;

    public Rectangle BoundingBox
    {
        // get => new Rectangle((int)Position.X + 70, (int)Position.Y, _texture.Width / Columns - 160, _texture.Height / Rows);
        get => Rect;
        set => _boundingBox = Rect;
    }

    public bool IsMoving { get; set; } = true; // Indicates if the actor is moving
    public bool IsJumping { get; set; } = true; // Indicates if the actor is jumping
    public bool IsOnGround { get; set; } // Indicates if the actor is on the ground

    public Vector2 Position
    {
        get => _position;
        set => _position = value;
    } // Position property

    public Vector2 Velocity
    {
        get => _velocity;
        set => _velocity = value;
    } // Velocity property

    private int Rows { get; set; } // Number of rows in the texture
    private int Columns { get; set; } // Number of columns in the texture
    private int _currentFrame; // Current frame of the animation
    private int _totalFrames; // Total frames in the animation
    private int _stepsDone { get; set; } // Number of steps done in the animation
    public int StepsDone => _stepsDone; // Number of steps done in the animation
    private double _timeSinceLastFrame; // Time since the last frame update

    // Constructor with parameters
    // protected Actor(Texture2D texture, Vector2 position, int rows, int columns)
    // {
    //     this._texture = texture;
    //     Position = position;
    //     Rows = rows;
    //     Columns = columns;
    //     _currentFrame = 0;
    //     _totalFrames = Rows * Columns;
    //     _velocity = Vector2.Zero;
    //     _timeSinceLastFrame = 0;
    // }

    protected Actor(Texture2D texture, Rectangle rect, Rectangle srect, int rows, int columns)
    {
        Rows = rows;
        Columns = columns;
        _currentFrame = 0;
        _totalFrames = Rows * Columns;

        _texture = texture;
        _rect = rect;
        _srect = srect;
        _velocity = Vector2.Zero;
    }

    // Default constructor
    protected Actor() => _velocity = Vector2.Zero;


    public void HandleMovement(KeyboardState keystate, KeyboardState previousState, GameTime gameTime)
    {
        //от дивлюсі і наче не костиль, але може і костиль

        _velocity.X = 0.0f;

        _velocity.Y += Gravity;

        _velocity.Y = Math.Min(20.0f, _velocity.Y);


        if (keystate.IsKeyDown(Keys.D))
        {
            _velocity.X = 10;
            _isFacingRight = true;
            UpdateFrame(gameTime);
        }
        else if (keystate.IsKeyDown(Keys.A))
        {
            _velocity.X = -10;
            _isFacingRight = false;
            UpdateFrame(gameTime);
        }
        else if (IsOnGround && keystate.IsKeyDown(Keys.Space) && !previousState.IsKeyUp(Keys.Space))
        {
            _velocity.Y = -15;
        }
    }


    // Updates the animation frame
    private void UpdateFrame(GameTime gameTime)
    {
        _timeSinceLastFrame += gameTime.ElapsedGameTime.TotalSeconds;
        if (_timeSinceLastFrame >= FrameTime)
        {
            _currentFrame = (_currentFrame + 1) % _totalFrames;
            _timeSinceLastFrame -= FrameTime;
            _stepsDone += _currentFrame == 0 ? 1 : 0;
            _stepsDone += _currentFrame == 3 ? 1 : 0;
        }
    }

    public void Draw(SpriteBatch spriteBatch)
    {
        // spriteBatch.Begin();

        spriteBatch.Draw(_texture, _rect, _srect, Color.White);

        //spriteBatch.End();
    }


    // Draws the actor
    public void Draw(SpriteBatch spriteBatch, Vector2 location
        //, Texture2D hitboxTexture
    )
    {
        int width = _texture.Width / Columns;
        int height = _texture.Height / Rows;
        int row = _currentFrame / Columns;
        int column = _currentFrame % Columns;

        Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
        SpriteEffects spriteEffects = _isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        Rectangle destinationRectangle = new Rectangle((int)location.X-80, (int)location.Y, width, height);

        spriteBatch.Draw(_texture, destinationRectangle, sourceRectangle, Color.White, 0f, Vector2.Zero, spriteEffects,
            0f);
        //DrawBoundingBox(spriteBatch, hitboxTexture);
    }


    // Draws the bounding box of the actor
    public void DrawBoundingBox(SpriteBatch spriteBatch, Texture2D hitboxTexture)
    {
        Rectangle boundingBox = BoundingBox;
        spriteBatch.Draw(hitboxTexture, new Rectangle(boundingBox.X, boundingBox.Y, boundingBox.Width, 1),
            Color.Red); // Top
        spriteBatch.Draw(hitboxTexture, new Rectangle(boundingBox.X, boundingBox.Y, 1, boundingBox.Height),
            Color.Red); // Left
        spriteBatch.Draw(hitboxTexture,
            new Rectangle(boundingBox.X, boundingBox.Y + boundingBox.Height - 1, boundingBox.Width, 1),
            Color.Red); // Bottom
        spriteBatch.Draw(hitboxTexture,
            new Rectangle(boundingBox.X + boundingBox.Width - 1, boundingBox.Y, 1, boundingBox.Height),
            Color.Red); // Right
    }
}