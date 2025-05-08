using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Tales_of_Everlight.Damage;

using static Tales_of_Everlight.Main;

namespace Tales_of_Everlight;

public class Sceleton : Enemy
{
    private const float MOVEMENT_DURATION = 1.5f;
    private const float IDLE_DURATION = 4.0f;

    public Sceleton(ContentManager content, Rectangle rect, Rectangle srect) : base(content, rect, srect)
    {
        Health = 80;
        IdleTexture = content.Load<Texture2D>("Skeleton_idle");
        MovingTexture = content.Load<Texture2D>("Skeleton_walk");
        AttackTexture = content.Load<Texture2D>("Skeleton_attack");
    }

    public Sceleton()
    {
        Health = 80;
    }
    
    public override void TakeDamage(int amount)
    {
        Health -= amount;
        Console.WriteLine($"{GetType().Name} took {amount} damage. Health remaining: {Health}");
        Damaged = true;
    
        if (Health <= 0)
        {
            Health = 0;
            IsDead = true;
        }
    }

    public override void BehaviorHandler(GameTime gameTime)
    {
        if (IsDead) return;

        StateTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;

        switch (State)
        {
            case EnemyState.MovingRight:
                MoveRight();
                if (StateTimer >= MOVEMENT_DURATION)
                {
                    State = EnemyState.Idle;
                    AnimationState = EnemyAnimationState.Idle;
                    StateTimer = 0f;
                    Velocity = Vector2.Zero;
                }

                break;

            case EnemyState.Idle:
                Velocity = Vector2.Zero;
                if (StateTimer >= IDLE_DURATION)
                {
                    State = IsFacingRight ? EnemyState.MovingLeft : EnemyState.MovingRight;
                    AnimationState = EnemyAnimationState.Move;
                    StateTimer = 0f;
                }

                break;

            case EnemyState.MovingLeft:
                MoveLeft();
                if (StateTimer >= MOVEMENT_DURATION)
                {
                    State = EnemyState.Idle;
                    AnimationState = EnemyAnimationState.Idle;
                    StateTimer = 0f;
                    Velocity = Vector2.Zero;
                }

                break;
        }
    }


    // public override void AnimationHandler()
    // {
    //     Columns = 8;
    //     Rows = 1;
    // }

    public override void AnimationHandler()
    {
        switch (AnimationState)
        {
            case EnemyAnimationState.Move:
                CurrentTexture = MovingTexture;
                Columns = 4;
                Rows = 1;
                break;
            case EnemyAnimationState.Idle:
                CurrentTexture = IdleTexture;
                Columns = 4;
                Rows = 1;
                break;
            case EnemyAnimationState.Attack:
                CurrentTexture = AttackTexture;
                Columns = 8;
                Rows = 1;
                break;
        }
    }

    public override void PerformAttack()
    {
        Velocity = Vector2.Zero;
        AnimationState = EnemyAnimationState.Attack;
        IsFacingRight = Main.MainHero.Rect.X >= Rect.X;
        IsAttacking = true;
    }


    public override void SearchEnemy()
    {
        if (Math.Abs(Main.MainHero.Rect.X - Rect.X) < 110 && Math.Abs(Main.MainHero.Rect.Y - Rect.Y)<64)
        {
            //PerformAttack();  
            Console.WriteLine("Enemy found");

            PerformAttack();
        }
    }

    public override void UpdateFrame(GameTime gametime)
    {
        _timeSinceLastFrame += gametime.ElapsedGameTime.TotalSeconds;
        if (_timeSinceLastFrame >= FrameTime)
        {
            _timeSinceLastFrame -= FrameTime;


            if (AnimationState == EnemyAnimationState.Attack)
            {
                if (_currentFrame < _totalFrames - 1)
                {
                    _currentFrame++;
                    if (_currentFrame == 7)
                    {
                        if (Math.Abs(Main.MainHero.Rect.X - Rect.X) < 110 && Math.Abs(Main.MainHero.Rect.Y - Rect.Y)<80)
                        {
                            Console.WriteLine("Ennemy attacked");
                            Attack.ExecuteByEnemy(10);
                        }
                    }
                }
                else
                {
                    _currentFrame = 0;
                    IsAttacking = false;
                    State = EnemyState.Idle;
                    AnimationState = EnemyAnimationState.Idle;
                    StateTimer = 0f;
                    BehaviorHandler(gametime);
                }
            }
            else
            {
                _currentFrame++;
                if (_currentFrame >= _totalFrames)
                {
                    _currentFrame = 0;
                }
            }
        }
    }

    public override void Update(GameTime gameTime)
    {
        if (IsDead)
        {
            _deathAnimationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateFrame(gameTime);
            if (_deathAnimationTime >= _deathAnimationDuration)
            {
                //Main.RemoveEnemy(this);
                ;
            }

            return;
        }


        if (!IsAttacking)
        {
            BehaviorHandler(gameTime);
            SearchEnemy();
        }

        UpdateFrame(gameTime);
    }


    public override void MoveRight()
    {
        Velocity = Velocity with { X = 4f };
        IsFacingRight = true;
    }

    public override void MoveLeft()
    {
        Velocity = Velocity with { X = -4f };
        IsFacingRight = false;
    }

    public override void Draw(SpriteBatch spriteBatch, Vector2 location, Texture2D hitboxTexture)
    {
        AnimationHandler();
        _totalFrames = Columns * Rows;
        int width = CurrentTexture.Width / Columns;
        int height = CurrentTexture.Height / Rows;
        int row = _currentFrame / Columns;
        int column = _currentFrame % Columns;

        // Set source rectangle consistently
        SourceRectangle = new Rectangle(width * column, height * row, width, height);

        // Set destination position based on facing direction
        Vector2 drawPosition;
        if (IsFacingRight)
        {
            drawPosition = new Vector2(location.X-128, location.Y );
            SpriteEffects = SpriteEffects.None;
        }
        else
        {
            drawPosition = new Vector2(location.X - 100, location.Y );
            SpriteEffects = SpriteEffects.FlipHorizontally;
        }

        // Draw with the appropriate color based on state
        if (IsDead)
        {
            float alpha = 1.0f - (_deathAnimationTime / _deathAnimationDuration);
            Color deathColor = Color.White * alpha;
            spriteBatch.Draw(CurrentTexture, drawPosition, SourceRectangle, deathColor, 0f, Vector2.Zero, 1f,
                SpriteEffects,
                0f);
        }
        else if (Damaged)
        {
            spriteBatch.Draw(CurrentTexture, drawPosition, SourceRectangle, Color.Red, 0f, Vector2.Zero, 1f,
                SpriteEffects,
                0f);
            Damaged = false;
        }
        else
        {
            spriteBatch.Draw(CurrentTexture, drawPosition, SourceRectangle, Color.Wheat, 0f, Vector2.Zero, 1f,
                SpriteEffects,
                0f);
        }

        // Draw the hitbox
        DrawBoundingBox(spriteBatch, hitboxTexture);
    }
}