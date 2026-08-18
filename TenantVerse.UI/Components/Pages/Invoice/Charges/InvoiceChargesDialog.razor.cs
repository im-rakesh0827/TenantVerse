using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.UI.Components.Pages.Invoice.Charges;

public partial class InvoiceChargesDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter]
    public InvoiceModel Invoice { get; set; } = new();

    [Parameter]    
    public List<InvoiceChargeModel> ChargesList { get; set; } = new();

    private void Close()
    {
        MudDialog.Close();
    }
}