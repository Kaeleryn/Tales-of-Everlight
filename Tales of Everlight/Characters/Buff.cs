using Microsoft.Xna.Framework;
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
    public string Name { get; set; }
    public Texture2D Texture { get; set; }
    public const float DURATION  = 30f;
    public float TimeLeft { get; set; } = DURATION;
    
    public bool IsActive { get; set; }
    public float Amount { get; set; }


    public Buff(BuffType type, string name, Texture2D texture, float amount)
    {
        Type = type;
        Name = name;
        Texture = texture;
        Amount = amount;
        
        
    }
    
    
    private void ApplyEffect()
    {
        switch (Type)
        {
            case BuffType.Heal:
                Heal((int)Amount);
                // Healing is instant, so we can deactivate the buff immediately
                IsActive = false;
                break;
            case BuffType.IncreaseDamage:
                MainHero.Damage += (int)Amount;
                break;
            case BuffType.IncreaseSpeed:
                MainHero.Speed += Amount;
                break;
        }
    }
    
    private void RemoveEffect()
    {
        // Only remove effects for buffs that need to be reversed
        switch (Type)
        {
            case BuffType.IncreaseDamage:
                MainHero.Damage -= (int)Amount;
                break;
            case BuffType.IncreaseSpeed:
                MainHero.Speed -= Amount;
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
            return;
            
        // Decrease the timer
        TimeLeft -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Check if the buff has expired
        if (TimeLeft <= 0)
        {
            RemoveEffect();
            IsActive = false;
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