using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;


namespace Tales_of_Everlight.Characters;
public enum BuffType
{
    Heal,
    IncreaseDamage,
    IncreaseSpeed,
}

public class Buff
{
    
    public BuffType Type;
   
    public Texture2D Texture { get; set; }
    public Vector2 Position { get; set; }
    public const float DURATION  = 15f;
    public float TimeLeft { get; set; } = DURATION;
    
    public bool IsActive { get; set; }
    public float Amount { get; set; }


    public Buff(BuffType type, Vector2 position, float amount, ContentManager content)
    {
        Type = type;
        Position = position;
        Amount = amount;
        
        switch (type)
        {
            case BuffType.Heal:
                Texture = content.Load<Texture2D>("buff_health");
                break;
            case BuffType.IncreaseDamage:
                Texture = content.Load<Texture2D>("buff_damage");
                break;
            case BuffType.IncreaseSpeed:
                Texture = content.Load<Texture2D>("buff_speed");
                break;
        }
        
    }


    public void ApplyEffect()
    {
        switch (Type)
        {
            case BuffType.Heal:
                Heal((int)Amount);
                // Healing is instant, so we can deactivate the buff immediately
                IsActive = true;
                break;
            case BuffType.IncreaseDamage:
                MainHero.Damage += (int)Amount;
                IsActive = true;
                break;
            case BuffType.IncreaseSpeed:
                MainHero.Speed += Amount;
                IsActive = true;
                break;
        }
    }
    
    private void RemoveEffect()
    {
        // Only remove effects for buffs that need to be reversed
        switch (Type)
        {
            case BuffType.IncreaseDamage:
                MainHero.Damage = MainHero.StandardDamage;
                break;
            case BuffType.IncreaseSpeed:
                MainHero.Speed = MainHero.StandardSpeed;
                break;
            // Heal doesn't need to be reversed as it's an instant effect
        }
    }
    
    public void Update(GameTime gameTime)
    {
        if (!IsActive)
            return;
            
        // If it's a healing buff, it's already been applied and deactivated
        if (Type == BuffType.Heal)
            RemoveEffect();
            
        // Decrease the timer
        TimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Check if the buff has expired
        if (TimeLeft <= 0)
        {
            RemoveEffect();
            
        }
    }
    

    public void Heal(int amount)
    {
        MainHero.Health += amount;
        if (MainHero.Health > MainHero.MaxHealth)
        {
            MainHero.Health = MainHero.MaxHealth;
        }
    }
    
    
    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        if (!IsActive)
        {
            spriteBatch.Draw(Texture, position, Color.White);
        }
    }
    
    
}