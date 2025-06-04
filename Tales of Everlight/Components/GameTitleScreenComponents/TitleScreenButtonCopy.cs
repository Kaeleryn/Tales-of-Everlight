using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;
using RenderingLibrary.Graphics;
using System.Linq;
using System.Windows.Forms;
using MonoGameGum;
using Keys = Microsoft.Xna.Framework.Input.Keys;

namespace Tales_of_Everlight.Components
{
    partial class TitleScreenButtonCopy
    {
        partial void CustomInitialize()
        {
            Click += (_, _) => IsFocused = true;
            KeyDown += (obj,e) =>
            {
                if (e.Key == Microsoft.Xna.Framework.Input.Keys.Enter)
                {
                    PerformClick(GumService.Default.Keyboard);
                }
            };
        }
    }
}