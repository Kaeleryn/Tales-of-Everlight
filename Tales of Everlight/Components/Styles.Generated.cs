//Code for Styles (Container)
using GumRuntime;
using MonoGameGum.GueDeriving;
using Gum.Converters;
using Gum.DataTypes;
using Gum.Managers;
using Gum.Wireframe;

using RenderingLibrary.Graphics;

using System.Linq;

namespace Tales_of_Everlight.Components;
partial class Styles : MonoGameGum.Forms.Controls.FrameworkElement
{
    [System.Runtime.CompilerServices.ModuleInitializer]
    public static void RegisterRuntimeType()
    {
        var template = new MonoGameGum.Forms.VisualTemplate((vm, createForms) =>
        {
            var visual = new MonoGameGum.GueDeriving.ContainerRuntime();
            var element = ObjectFinder.Self.GetElementSave("Styles");
            element.SetGraphicalUiElement(visual, RenderingLibrary.SystemManagers.Default);
            if(createForms) visual.FormsControlAsObject = new Styles(visual);
            return visual;
        });
        MonoGameGum.Forms.Controls.FrameworkElement.DefaultFormsTemplates[typeof(Styles)] = template;
        ElementSaveExtensions.RegisterGueInstantiation("Styles", () => 
        {
            var gue = template.CreateContent(null, true) as InteractiveGue;
            return gue;
        });
    }
    public ContainerRuntime Colors { get; protected set; }
    public ColoredRectangleRuntime Black { get; protected set; }
    public ColoredRectangleRuntime DarkGray { get; protected set; }
    public ColoredRectangleRuntime Gray { get; protected set; }
    public ColoredRectangleRuntime LightGray { get; protected set; }
    public ColoredRectangleRuntime White { get; protected set; }
    public ColoredRectangleRuntime PrimaryDark { get; protected set; }
    public ColoredRectangleRuntime Primary { get; protected set; }
    public ColoredRectangleRuntime PrimaryLight { get; protected set; }
    public ColoredRectangleRuntime Success { get; protected set; }
    public ColoredRectangleRuntime Warning { get; protected set; }
    public ColoredRectangleRuntime Danger { get; protected set; }
    public ColoredRectangleRuntime Accent { get; protected set; }
    public TextRuntime Tiny { get; protected set; }
    public TextRuntime Small { get; protected set; }
    public TextRuntime Normal { get; protected set; }
    public TextRuntime Emphasis { get; protected set; }
    public TextRuntime Strong { get; protected set; }
    public TextRuntime H3 { get; protected set; }
    public TextRuntime H2 { get; protected set; }
    public TextRuntime H1 { get; protected set; }
    public ContainerRuntime TextStyles { get; protected set; }
    public TextRuntime Title { get; protected set; }

    public Styles(InteractiveGue visual) : base(visual) { }
    public Styles()
    {



    }
    protected override void ReactToVisualChanged()
    {
        base.ReactToVisualChanged();
        Colors = this.Visual?.GetGraphicalUiElementByName("Colors") as ContainerRuntime;
        Black = this.Visual?.GetGraphicalUiElementByName("Black") as ColoredRectangleRuntime;
        DarkGray = this.Visual?.GetGraphicalUiElementByName("DarkGray") as ColoredRectangleRuntime;
        Gray = this.Visual?.GetGraphicalUiElementByName("Gray") as ColoredRectangleRuntime;
        LightGray = this.Visual?.GetGraphicalUiElementByName("LightGray") as ColoredRectangleRuntime;
        White = this.Visual?.GetGraphicalUiElementByName("White") as ColoredRectangleRuntime;
        PrimaryDark = this.Visual?.GetGraphicalUiElementByName("PrimaryDark") as ColoredRectangleRuntime;
        Primary = this.Visual?.GetGraphicalUiElementByName("Primary") as ColoredRectangleRuntime;
        PrimaryLight = this.Visual?.GetGraphicalUiElementByName("PrimaryLight") as ColoredRectangleRuntime;
        Success = this.Visual?.GetGraphicalUiElementByName("Success") as ColoredRectangleRuntime;
        Warning = this.Visual?.GetGraphicalUiElementByName("Warning") as ColoredRectangleRuntime;
        Danger = this.Visual?.GetGraphicalUiElementByName("Danger") as ColoredRectangleRuntime;
        Accent = this.Visual?.GetGraphicalUiElementByName("Accent") as ColoredRectangleRuntime;
        Tiny = this.Visual?.GetGraphicalUiElementByName("Tiny") as TextRuntime;
        Small = this.Visual?.GetGraphicalUiElementByName("Small") as TextRuntime;
        Normal = this.Visual?.GetGraphicalUiElementByName("Normal") as TextRuntime;
        Emphasis = this.Visual?.GetGraphicalUiElementByName("Emphasis") as TextRuntime;
        Strong = this.Visual?.GetGraphicalUiElementByName("Strong") as TextRuntime;
        H3 = this.Visual?.GetGraphicalUiElementByName("H3") as TextRuntime;
        H2 = this.Visual?.GetGraphicalUiElementByName("H2") as TextRuntime;
        H1 = this.Visual?.GetGraphicalUiElementByName("H1") as TextRuntime;
        TextStyles = this.Visual?.GetGraphicalUiElementByName("TextStyles") as ContainerRuntime;
        Title = this.Visual?.GetGraphicalUiElementByName("Title") as TextRuntime;
        CustomInitialize();
    }
    //Not assigning variables because Object Instantiation Type is set to By Name rather than Fully In Code
    partial void CustomInitialize();
}
