using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Tales_of_Everlight;

public class Square : Enemy
{
    public Square(Texture2D texture, Vector2 position, int rows, int columns) : base(texture, position, rows, columns)
    {
    }
    
    public Square()
    {
    }
}