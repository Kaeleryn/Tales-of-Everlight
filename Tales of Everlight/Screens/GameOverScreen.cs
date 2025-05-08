using System;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

using System.Linq;
using System.Threading.Tasks;
using MonoGameGum;
using MonoGameGum.Forms;
using MonoGameGum.Forms.Controls;

namespace Tales_of_Everlight.Screens
{
    partial class GameOverScreen
    {
        partial void CustomInitialize()
        {
            TitleScreenButtonInstance.IsFocused = true;
            TitleScreenButtonInstance.Click += (x, y) =>
            {
                Main.InitGame(x,y);
                GoToScreen(new DialogScreen());
                ChangeIsDialog();
            };
            
            TitleScreenButtonInstance2.Click += HandleExitClicked;
            FrameworkElement.GamePadsForUiControl.Clear();
            FrameworkElement.GamePadsForUiControl.AddRange(FormsUtilities.Gamepads);
        }
        
        private async void ChangeIsDialog()
        {
            await Task.Delay(50);
            Main.isDialog = true;
        }
        private void GoToScreen(FrameworkElement newScreen)
        {
            GumService.Default.Root.Children.Clear();
            newScreen.AddToRoot();
        }
        private void HandleExitClicked(object sender, EventArgs e)
        {
            Main.Instance.Exit();
        }
    }
}
