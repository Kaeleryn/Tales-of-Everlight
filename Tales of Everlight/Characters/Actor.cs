using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Vector2 = Microsoft.Xna.Framework.Vector2;

namespace Tales_of_Everlight;

public enum AnimationState
{
    Running,
    Jumping,
    Attacking
}

public abstract class Actor
{
    private bool _isFacingRight = true; // Indicates if the actor is facing right
    private Texture2D _runningTexture; // Texture of the actor
    private Texture2D _jumpingTexture; // Texture of the actor
    private Texture2D _attackTexture; // Texture of the actor
    private Texture2D _currentTexture;
    
    public AnimationState AnimationState { get; set; }  // Animation state of the actor

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
    public bool IsOnGround { get; set; } // Indicates if the actor is on the ground


    public Vector2 Velocity
    {
        get => _velocity;
        set => _velocity = value;
    } // Velocity property

    private int Rows { get; set; } = 1; // Number of rows in the texture
    private int Columns { get; set; } // Number of columns in the texture
    
    
    private int _currentFrame; // Current frame of the animation
    private int _totalFrames; // Total frames in the animation
    private int _stepsDone { get; set; } // Number of steps done in the animation
    public int StepsDone => _stepsDone; // Number of steps done in the animation
    private double _timeSinceLastFrame; // Time since the last frame update


    protected Actor(ContentManager content, Rectangle rect, Rectangle srect)
    {
        
        
        
        _currentFrame = 0;
        

        _runningTexture = content.Load<Texture2D>("animatedSprite");
        _jumpingTexture = content.Load<Texture2D>("animatedSpriteJumping");

        _rect = rect;
        _srect = srect;
        _velocity = Vector2.Zero;
    }

    // Default constructor
    protected Actor() => _velocity = Vector2.Zero;


    public void HandleMovement(KeyboardState keystate, KeyboardState previousState, GameTime gameTime)
    {
        //от дивлюсі і наче не костиль, але може і костиль

        HandleVerticalMovement(keystate, previousState, gameTime);
        HandleHorisontalMovement(keystate, previousState, gameTime);
    }

    public void HandleVerticalMovement(KeyboardState keystate, KeyboardState previousState, GameTime gameTime)
    {
        _velocity.Y += Gravity;

        _velocity.Y = Math.Min(20.0f, _velocity.Y);

        if (IsOnGround && keystate.IsKeyDown(Keys.Space) && !previousState.IsKeyUp(Keys.Space))
        {
            _velocity.Y = -15;
            AnimationState = AnimationState.Jumping;
            IsOnGround = false;

        }

        // try
        // {
        //     if(!IsOnGround)
        //     {
        //         UpdateFrame(gameTime);
        //     }
        // } catch (Exception e)
        // {
        //     Console.WriteLine("Саня, ти даун");
        // }
        
    }


    public void HandleHorisontalMovement(KeyboardState keystate, KeyboardState previousState, GameTime gameTime)
    {
        // _velocity.X = 0.0f;
        Decelerate();
      


        if (keystate.IsKeyDown(Keys.D))
        {
            _velocity.X = 10;
            _isFacingRight = true;
            IsMoving = true;
            UpdateFrame(gameTime);
            if(IsOnGround)AnimationState = AnimationState.Running;
        }
        else if (keystate.IsKeyDown(Keys.A))
        {
            _velocity.X = -10;
            _isFacingRight = false;
            IsMoving = true;
            UpdateFrame(gameTime);
            if(IsOnGround)AnimationState = AnimationState.Running;
        }else
        {
            IsMoving = false;
        }
    }

    public void Decelerate()
    {
        _velocity.X *= 0.8f;

        if (_velocity.X < 0.5f && _velocity.X > -0.5f)
            _velocity.X = 0;
    }


    // Updates the animation frame
    private void UpdateFrame(GameTime gameTime)
    {
        _timeSinceLastFrame += gameTime.ElapsedGameTime.TotalSeconds;
        
        if (_timeSinceLastFrame >= FrameTime)
        {
            _timeSinceLastFrame -= FrameTime;

            if (AnimationState == AnimationState.Jumping && !IsOnGround)
            {
                // Increment the frame if not already at the last frame
                if (_currentFrame < _totalFrames - 1)
                {
                   // _currentFrame++;
                   _currentFrame = (_currentFrame + 1) % _totalFrames;
                }
            }
            else
            {
                // Default behavior for other animations
                _currentFrame = (_currentFrame + 1) % _totalFrames;

                // Optionally increment steps for running or attacking
                if (AnimationState == AnimationState.Running && (_currentFrame == 0 || _currentFrame == 3))
                {
                    _stepsDone++;
                }
            }
            
        }
    }


    public void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(_runningTexture, _rect, _srect, Color.White);
    }


    // public void Draw(SpriteBatch spriteBatch, Vector2 location)
    // {
    //   // if(!IsOnGround) DrawJumping(spriteBatch, location);
    //   // else if(IsMoving)DrawRunning(spriteBatch, location);
    //    DrawJumping(spriteBatch, location);
    // }


    // Draws the actor
    public void Draw(SpriteBatch spriteBatch, Vector2 location)
    {
        switch (AnimationState)
        {
            case AnimationState.Running:
                _currentTexture = _runningTexture;
                Columns = 6;
                break;
            case AnimationState.Jumping:
                _currentTexture = _jumpingTexture;
                Columns = 7;
                break;
            case AnimationState.Attacking:
                _currentTexture = _attackTexture;
                Columns = 5; // Example value
                break;
        }
        
        if(IsOnGround && !IsMoving)
        {
            _currentFrame = 0;
            _currentTexture = _runningTexture;
            Columns = 6;
        }

        _totalFrames = Columns * Rows;
        int width = _currentTexture.Width / Columns;
        int height = _currentTexture.Height / Rows;
        int row = _currentFrame / Columns;
        int column = _currentFrame % Columns;

        Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
        SpriteEffects spriteEffects = _isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
        Rectangle destinationRectangle = new Rectangle((int)location.X - 80, (int)location.Y, width, height);

        spriteBatch.Draw(_currentTexture, destinationRectangle, sourceRectangle, Color.White, 0f, Vector2.Zero, spriteEffects, 0f);
    }

    
    // public void DrawJumping(SpriteBatch spriteBatch, Vector2 location
    //     //, Texture2D hitboxTexture
    // )
    // {
    //     
    //     _totalFrames = JumpingColumns * Rows;
    //     int width = _runningTexture.Width / JumpingColumns;
    //     int height = _runningTexture.Height / Rows;
    //     int row = _currentFrame / JumpingColumns;
    //     int column = _currentFrame % JumpingColumns;
    //
    //     Rectangle sourceRectangle = new Rectangle(width * column, height * row, width, height);
    //     SpriteEffects spriteEffects = _isFacingRight ? SpriteEffects.None : SpriteEffects.FlipHorizontally;
    //     Rectangle destinationRectangle = new Rectangle((int)location.X - 80, (int)location.Y, width, height);
    //
    //     spriteBatch.Draw(_jumpingTexture, destinationRectangle, sourceRectangle, Color.White, 0f, Vector2.Zero, spriteEffects,
    //         0f);
    //     //DrawBoundingBox(spriteBatch, hitboxTexture);
    // }


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