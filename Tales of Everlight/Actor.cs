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

    private const float Gravity = 1.2f; // Gravity constant
    private const double FrameTime = 0.1; // Time in seconds between frames

    // Bounding box of the actor
    private Rectangle BoundingBox => new Rectangle((int)Position.X + 70, (int)Position.Y,
        _texture.Width / Columns - 160, _texture.Height / Rows);

    public bool IsMoving { get; set; } = true; // Indicates if the actor is moving
    public bool IsJumping { get; set; } = true; // Indicates if the actor is jumping

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
    private double _timeSinceLastFrame; // Time since the last frame update

    // Constructor with parameters
    protected Actor(Texture2D texture, Vector2 position, int rows, int columns)
    {
        this._texture = texture;
        Position = position;
        Rows = rows;
        Columns = columns;
        _currentFrame = 0;
        _totalFrames = Rows * Columns;
        _velocity = Vector2.Zero;
        _timeSinceLastFrame = 0;
    }

    // Default constructor
    protected Actor() => _velocity = Vector2.Zero;

    // Handles collisions with the environment
    public void CollisionHandler(Dictionary<Vector2, int> collisions, int tileSize)
    {
        _position += _velocity;
        HandleVerticalCollisions(collisions, tileSize);
        HandleHorizontalCollisions(collisions, tileSize);
    }

    // Handles vertical collisions
    private void HandleVerticalCollisions(Dictionary<Vector2, int> collisions, int tileSize)
    {
        var intersections = GetIntersectingTiles(BoundingBox, tileSize);
        bool hasVerticalCollision = false;

        foreach (var rect in intersections)
        {
            if (collisions.TryGetValue(new Vector2(rect.X, rect.Y), out _))
            {
                Rectangle collision = new Rectangle(rect.X * tileSize, rect.Y * tileSize, tileSize, tileSize);

                if (_velocity.Y > 0.0f)
                {
                    _position.Y = collision.Top - BoundingBox.Height;
                    IsJumping = false;
                }
                else if (_velocity.Y < 0.0f)
                {
                    _position.Y = collision.Bottom;
                }

                _velocity.Y = 0;
                hasVerticalCollision = true;
            }
        }

        if (!hasVerticalCollision)
        {
            IsJumping = true;
        }
    }

    // Handles horizontal collisions
    private void HandleHorizontalCollisions(Dictionary<Vector2, int> collisions, int tileSize)
    {
        var intersections = GetIntersectingTiles(BoundingBox, tileSize);

        foreach (var rect in intersections)
        {
            if (collisions.TryGetValue(new Vector2(rect.X, rect.Y), out _))
            {
                Rectangle collision = new Rectangle(rect.X * tileSize, rect.Y * tileSize, tileSize, tileSize);

                if (_velocity.X > 0.0f)
                {
                    _position.X = collision.Left - BoundingBox.Width - 70; // Adjust for bounding box offset
                    IsMoving = false;
                }
                else if (_velocity.X < 0.0f)
                {
                    _position.X = collision.Right - 70; // Adjust for bounding box offset
                    IsMoving = false;
                }

                _velocity.X = 0;
            }
        }
    }

    // Handles the movement of the actor
    public void MovementHandler(GameTime gameTime)
    {
        var keyboardState = Keyboard.GetState();
        HandleHorizontalMovement(keyboardState, gameTime);
        HandleJumping(keyboardState);
        ApplyGravity();
        ClampPosition();
    }

    // Handles horizontal movement based on keyboard input
    private void HandleHorizontalMovement(KeyboardState keyboardState, GameTime gameTime)
    {
        if (keyboardState.IsKeyDown(Keys.D) && _position.X < 2560 - _texture.Width / Columns)
        {
            MoveRight(gameTime);
        }
        else if (keyboardState.IsKeyDown(Keys.A) && _position.X > 0)
        {
            MoveLeft(gameTime);
        }
        else
        {
            Decelerate();
        }
    }

    // Moves the actor to the right
    private void MoveRight(GameTime gameTime)
    {
        _velocity.X = 15f;
        IsMoving = true;
        UpdateFrame(gameTime);
        _isFacingRight = true;
    }

    // Moves the actor to the left
    private void MoveLeft(GameTime gameTime)
    {
        _velocity.X = -15f;
        IsMoving = true;
        UpdateFrame(gameTime);
        _isFacingRight = false;
    }

    // Decelerates the actor
    private void Decelerate()
    {
        _velocity.X *= 0.8f;
        if (Math.Abs(_velocity.X) < 0.1f) // Threshold to stop the velocity
        {
            _velocity.X = 0f;
        }

        IsMoving = false;
        _currentFrame = 0;
    }

    // Updates the animation frame
    private void UpdateFrame(GameTime gameTime)
    {
        _timeSinceLastFrame += gameTime.ElapsedGameTime.TotalSeconds;
        if (_timeSinceLastFrame >= FrameTime)
        {
            _currentFrame = (_currentFrame + 1) % _totalFrames;
            _timeSinceLastFrame -= FrameTime;
        }
    }

    // Handles jumping based on keyboard input
    private void HandleJumping(KeyboardState keyboardState)
    {
        if (keyboardState.IsKeyDown(Keys.Space) && !IsJumping)
        {
            IsJumping = true;
            _velocity.Y = -20;
            _position.Y += _velocity.Y;
        }
    }

    // Applies gravity to the actor
    private void ApplyGravity()
    {
        if (IsJumping)
        {
            _velocity.Y += Gravity;
        }
        else
        {
            _velocity.Y = 0f;
        }
    }

    // Clamps the position of the actor to prevent it from falling off the screen
    private void ClampPosition()
    {
        if (_position.Y > 800)
        {
            _position.Y = 800;
            IsJumping = false;
        }
    }

    // Gets the intersecting tiles for collision detection
    private List<Rectangle> GetIntersectingTiles(Rectangle target, int tileSize)
    {
        List<Rectangle> intersections = new();

        int leftTile = target.Left / tileSize;
        int rightTile = (target.Right - 1) / tileSize;
        int topTile = target.Top / tileSize;
        int bottomTile = (target.Bottom - 1) / tileSize;

        for (int x = leftTile; x <= rightTile; x++)
        {
            for (int y = topTile; y <= bottomTile; y++)
            {
                intersections.Add(new Rectangle(x, y, tileSize, tileSize));
            }
        }

        return intersections;
    }

    // Draws the actor
    public void Draw(SpriteBatch spriteBatch, Vector2 location, Texture2D hitboxTexture)
    {
        int width = _texture.Width / Columns;
        int height = _texture.Height / Rows;
        int row = _currentFrame / Columns;
        int column = _currentFrame % Columns;

        Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
        SpriteEffects spriteEffects = _isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        Rectangle destinationRectangle = new Rectangle((int)location.X, (int)location.Y, width, height);

        spriteBatch.Draw(_texture, destinationRectangle, sourceRectangle, Color.White, 0f, Vector2.Zero, spriteEffects,
            0f);
        DrawBoundingBox(spriteBatch, hitboxTexture);
    }

    // Draws the bounding box of the actor
    private void DrawBoundingBox(SpriteBatch spriteBatch, Texture2D hitboxTexture)
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