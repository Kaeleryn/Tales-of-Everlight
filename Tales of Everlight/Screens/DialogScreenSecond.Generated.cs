//Code for DialogScreenSecond
using GumRuntime;
using MonoGameGum.GueDeriving;
using Tales_of_Everlight.Components;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

using System.Linq;

namespace Tales_of_Everlight.Screens;
partial class DialogScreenSecond : MonoGameGum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new MonoGameGum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new MonoGameGum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("DialogScreenSecond");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new DialogScreenSecond(visual);
            visual.Width = 0;
            visual.WidthUnits = Gum.DataTypes.DimensionUnitType.RelativeToParent;
            visual.Height = 0;
            visual.HeightUnits = Gum.DataTypes.DimensionUnitType.RelativeToParent;
            return visual;
        });
        MonoGameGum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(DialogScreenSecond)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("DialogScreenSecond", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public SpriteRuntime SpriteInstance { get; protected set; }
    public TextRuntime TextInstance { get; protected set; }
    public ContainerRuntime ContainerInstance { get; protected set; }
    public ColoredRectangleRuntime ColoredRectangleInstance { get; protected set; }
    public ContainerRuntime ContainerInstance1 { get; protected set; }
    public TitleScreenButtonCopy TitleScreenButtonCopyInstance { get; protected set; }

    public DialogScreenSecond(InteractiveGue visual) : base(visual) { }
    public DialogScreenSecond()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        SpriteInstance = this.Visual?.GetGraphicalUiElementByName("SpriteInstance") as SpriteRuntime;
        TextInstance = this.Visual?.GetGraphicalUiElementByName("TextInstance") as TextRuntime;
        ContainerInstance = this.Visual?.GetGraphicalUiElementByName("ContainerInstance") as ContainerRuntime;
        ColoredRectangleInstance = this.Visual?.GetGraphicalUiElementByName("ColoredRectangleInstance") as ColoredRectangleRuntime;
        ContainerInstance1 = this.Visual?.GetGraphicalUiElementByName("ContainerInstance1") as ContainerRuntime;
        TitleScreenButtonCopyInstance = MonoGameGum.Forms.GraphicalUiElementFormsExtensions.GetFrameworkElementByName<TitleScreenButtonCopy>(this.Visual,"TitleScreenButtonCopyInstance");
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
