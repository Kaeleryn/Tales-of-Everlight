using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Tales_of_Everlight.Damage;

namespace Tales_of_Everlight;

public class FrostGolem : Enemy
{
     private const float MOVEMENT_DURATION = 2.0f;
    private const float IDLE_DURATION = 2.0f;
    private const float ATTACK_COOLDOWN = 3f;
    private float AttackTimer = 0f;

    public FrostGolem(ContentManager content, Rectangle rect, Rectangle srect) : base(content, rect, srect)
    {
        Health = 150;
        MaxHealth = Health;
        IdleTexture = content.Load<Texture2D>("Ghoul_idle");
        MovingTexture = content.Load<Texture2D>("Ghoul_walk");
        AttackTexture = content.Load<Texture2D>("Ghoul_attack");
    }

    public FrostGolem()
    {
        Health = 150;
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
                Columns = 1;
                Rows = 8;
                break;
            case EnemyAnimationState.Idle:
                CurrentTexture = IdleTexture;
                Columns = 1;
                Rows = 4;
                break;
            case EnemyAnimationState.Attack:
                CurrentTexture = AttackTexture;
                Columns = 1;
                Rows = 6;
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
        if (Math.Abs(Main.MainHero.Rect.X - Rect.X) < 90 && Math.Abs(Main.MainHero.Rect.Y - Rect.Y)<100)
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
                    if (_currentFrame == 5)
                    {
                        if (Math.Abs(Main.MainHero.Rect.X - Rect.X) < 80&& Math.Abs(Main.MainHero.Rect.Y - Rect.Y)<100)
                        {
                            Console.WriteLine("Ennemy attacked");
                            Attack.ExecuteByEnemy(20);
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
                    AttackTimer = 0f;
                   // IsFacingRight = Main.MainHero.Rect.X <= Rect.X;
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
        
        AttackTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        if (IsDead)
        {
            _deathAnimationTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
            UpdateFrame(gameTime);
            if (_deathAnimationTime >= _deathAnimationDuration)
            {
                //Main.RemoveEnemy(this);
            }

            return;
        }
        if (Damaged)
        {
            DamagedTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            if(DamagedTimer >= 0.5f)
            {
                Damaged = false;
                DamagedTimer = 0f;
            }
            
        }


        if (!IsAttacking)
        {
            BehaviorHandler(gameTime);
            if(AttackTimer >= ATTACK_COOLDOWN)
            {
                SearchEnemy();
            }
            
        }

        UpdateFrame(gameTime);
    }


    public override void MoveRight()
    {
        Velocity = Velocity with { X = 3f };
        IsFacingRight = true;
    }

    public override void MoveLeft()
    {
        Velocity = Velocity with { X = -3f };
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
            drawPosition = new Vector2(location.X, location.Y-15 );
            SpriteEffects = SpriteEffects.None;
        }
        else
        {
            drawPosition = new Vector2(location.X , location.Y-15 );
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
            
        }
        else
        {
            spriteBatch.Draw(CurrentTexture, drawPosition, SourceRectangle, Color.White, 0f, Vector2.Zero, 1f,
                SpriteEffects,
                0f);
        }

        // Draw the hitbox
        DrawBoundingBox(spriteBatch, hitboxTexture);
    }
    
}