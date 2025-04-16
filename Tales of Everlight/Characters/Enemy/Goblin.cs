using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace Tales_of_Everlight;

public class Goblin : Enemy
{
    public Goblin(ContentManager content, Rectangle rect, Rectangle srect) : base(content, rect, srect)
    {
    }

    public Goblin()
    {
    }
}