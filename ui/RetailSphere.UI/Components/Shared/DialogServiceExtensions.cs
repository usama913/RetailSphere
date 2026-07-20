using System.Threading.Tasks;
using MudBlazor;

namespace RetailSphere.UI.Components.Shared;

/// <summary>
/// Thin wrapper around MudBlazor's built-in IDialogService.ShowMessageBox so every
/// destructive action across the app (delete, deactivate, reverse a payment, etc.)
/// asks the same way with the same wording, instead of firing immediately on click
/// like the original Categories/Suppliers/Customers/etc. pages did.
/// </summary>
public static class DialogServiceExtensions
{
    public static async Task<bool> ConfirmDeleteAsync(this IDialogService dialogService, string itemDescription, string? extraWarning = null)
    {
        var message = $"Are you sure you want to delete {itemDescription}? This action cannot be undone.";
        if (!string.IsNullOrWhiteSpace(extraWarning))
            message += " " + extraWarning;

        var result = await dialogService.ShowMessageBox(
            "Confirm Delete",
            message,
            yesText: "Delete",
            cancelText: "Cancel");

        return result == true;
    }

    public static async Task<bool> ConfirmActionAsync(this IDialogService dialogService, string title, string message, string confirmText = "Confirm")
    {
        var result = await dialogService.ShowMessageBox(
            title,
            message,
            yesText: confirmText,
            cancelText: "Cancel");

        return result == true;
    }
}
