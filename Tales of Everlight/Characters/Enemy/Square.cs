using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tales_of_Everlight;

public class Square : Enemy
{
    public Square(ContentManager content, Rectangle rect, Rectangle srect) : base(content, rect, srect)
    {
    }

    public Square()
    {
    }
}