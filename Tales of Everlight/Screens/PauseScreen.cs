using System;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

using System.Linq;
using MonoGameGum;
using MonoGameGum.Forms;
using MonoGameGum.Forms.Controls;

namespace Tales_of_Everlight.Screens
{
    partial class PauseScreen
    {
        partial void CustomInitialize()
        {
            TitleScreenButtonInstance.IsFocused = true;
            TitleScreenButtonInstance.Click += (x, y) =>
            {
                Main.isPaused = false;
                GumService.Default.Root.Children.Clear();
            };
            
            TitleScreenButtonInstance2.Click += (_,_) => GoToScreen(new MainMenu());
            FrameworkElement.GamePadsForUiControl.Clear();
            FrameworkElement.GamePadsForUiControl.AddRange(FormsUtilities.Gamepads);
        }
        private void GoToScreen(FrameworkElement newScreen)
        {
            GumService.Default.Root.Children.Clear();
            newScreen.AddToRoot();
        }
    }
}
