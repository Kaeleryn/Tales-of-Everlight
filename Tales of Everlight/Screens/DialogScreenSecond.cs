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
    partial class DialogScreenSecond
    {
        partial void CustomInitialize()
        {
            TitleScreenButtonCopyInstance.IsFocused = true;
            TitleScreenButtonCopyInstance.Click += HandleExitClicked;
            
            FrameworkElement.GamePadsForUiControl.Clear();
            FrameworkElement.GamePadsForUiControl.AddRange(FormsUtilities.Gamepads);
        }
        private void HandleExitClicked(object sender, EventArgs e)
        {
            Main.isDialog = false;
            GumService.Default.Root.Children.Clear();
        }
    }
}
