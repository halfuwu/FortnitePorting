using Avalonia;
using Avalonia.Controls;
using Material.Icons;

namespace FortnitePorting.Controls;

public partial class MaterialIconText : UserControl
{
    public static readonly StyledProperty<string> TextProperty = AvaloniaProperty.Register<MaterialIconText, string>(nameof(Text), string.Empty);

    public string Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }

    public static readonly StyledProperty<MaterialIconKind> IconProperty = AvaloniaProperty.Register<MaterialIconText, MaterialIconKind>(nameof(Icon), MaterialIconKind.Folder);

    public MaterialIconKind Icon
    {
        get => GetValue(IconProperty);
        set => SetValue(IconProperty, value);
    }

    public static readonly StyledProperty<int> IconSizeProperty = AvaloniaProperty.Register<MaterialIconText, int>(nameof(IconSize), 20);

    public int IconSize
    {
        get => GetValue(IconSizeProperty);
        set => SetValue(IconSizeProperty, value);
    }

    public MaterialIconText()
    {
        InitializeComponent();
    }
}