using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.UI.Components.Pages.Invoice;

public partial class InvoiceChargesDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter]
    public InvoiceModel Invoice { get; set; } = new();

    private void Close()
    {
        MudDialog.Close();
    }
}