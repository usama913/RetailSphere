using MudBlazor;

namespace RetailSphere.UI.Components.Shared;

/// <summary>
/// One shared DialogOptions instance so every edit dialog in the app opens the same
/// way: responsive width, centered, closeable via the X button or Escape. Pass a
/// different MaxWidth only when a form is genuinely wider (e.g. side-by-side columns).
/// </summary>
public static class AppDialogOptions
{
    public static DialogOptions Default { get; } = new()
    {
        MaxWidth = MaxWidth.Small,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
        Position = DialogPosition.Center,
    };

    public static DialogOptions Medium { get; } = new()
    {
        MaxWidth = MaxWidth.Medium,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
        Position = DialogPosition.Center,
    };

    public static DialogOptions Large { get; } = new()
    {
        MaxWidth = MaxWidth.Large,
        FullWidth = true,
        CloseButton = true,
        CloseOnEscapeKey = true,
        Position = DialogPosition.Center,
    };
}
