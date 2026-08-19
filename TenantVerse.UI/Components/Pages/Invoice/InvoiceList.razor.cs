using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.Shared.Models.Invoice;
using TenantVerse.UI.Services;
using TenantVerse.Shared.Models.Tenant;
using TenantVerse.UI.Components.Pages.Invoice.PopUpDialog;
using TenantVerse.UI.Components.Pages.Tenant.PopUpScreen;

namespace TenantVerse.UI.Components.Pages.Invoice;

public partial class InvoiceList : ComponentBase
{
    [Inject]
    private InvoiceService InvoiceService { get; set; } = default!;

    [Inject]
    private StateContainer _StateContainer { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;

    [Inject]
    private IDialogService DialogService { get; set; } = default!;

    private List<InvoiceModel> _invoices = new();
    // private List<InvoiceChargeModel> _charges = new();

    private string _searchString = string.Empty;

    private string? _paymentStatus;

    private bool IsLoading;

    private string? _message;


    private bool _showTenantPopup;

    private TenantModel? _selectedTenant;

    private DateTime? _billingMonth;


    // protected IEnumerable<InvoiceModel> FilteredInvoices =>
    //     string.IsNullOrWhiteSpace(_searchString)
    //         && string.IsNullOrWhiteSpace(_paymentStatus)
    //             ? _invoices
    //             : _invoices.Where(FilterInvoice);


    protected IEnumerable<InvoiceModel> FilteredInvoices =>
    string.IsNullOrWhiteSpace(_searchString)
    && string.IsNullOrWhiteSpace(_paymentStatus)
    && !_billingMonth.HasValue
        ? _invoices
        : _invoices.Where(FilterInvoice);


    protected override async Task OnInitializedAsync()
    {
        await LoadInvoicesAsync();
    }

    private async Task LoadInvoicesAsync()
    {
        try
        {
            IsLoading = true;
            _message = null;

            if (!_StateContainer.Invoice.IsLoaded)
            {
               await Task.Delay(1000);
                var refreshed =
                    await _StateContainer.Invoice.RefreshAsync();

                if (!refreshed)
                {
                    _message = "Unable to load invoices.";
                    return;
                }
            }
            _invoices = _StateContainer.Invoice.Invoices.ToList();
            // _charges = _invoices.SelectMany(x => x.Charges).ToList();
        }
        catch (Exception ex)
        {
            _message = ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }



    // private bool FilterInvoice(InvoiceModel invoice)
    // {
    //     if (!string.IsNullOrWhiteSpace(_searchString))
    //     {
    //         var search =
    //             _searchString.Trim();

    //         var matchesSearch =
    //             (invoice.InvoiceNumber ?? string.Empty)
    //                 .Contains(
    //                     search,
    //                     StringComparison.OrdinalIgnoreCase)

    //             ||

    //             (invoice.PropertyName ?? string.Empty)
    //                 .Contains(
    //                     search,
    //                     StringComparison.OrdinalIgnoreCase)

    //             ||

    //             (invoice.UnitNumber ?? string.Empty)
    //                 .Contains(
    //                     search,
    //                     StringComparison.OrdinalIgnoreCase)

    //             ||

    //             (invoice.TenantName ?? string.Empty)
    //                 .Contains(
    //                     search,
    //                     StringComparison.OrdinalIgnoreCase)

    //             ||

    //             invoice.BillingMonth
    //                 .ToString("MMM yyyy")
    //                 .Contains(
    //                     search,
    //                     StringComparison.OrdinalIgnoreCase)

    //             ||

    //             invoice.DueDate
    //                 .ToString("dd MMM yyyy")
    //                 .Contains(
    //                     search,
    //                     StringComparison.OrdinalIgnoreCase)

    //             ||

    //             invoice.TotalPayable
    //                 .ToString()
    //                 .Contains(
    //                     search,
    //                     StringComparison.OrdinalIgnoreCase)

    //             ||

    //             (invoice.PaymentStatus ?? string.Empty)
    //                 .Contains(
    //                     search,
    //                     StringComparison.OrdinalIgnoreCase);


    //         if (!matchesSearch)
    //         {
    //             return false;
    //         }
    //     }
    //     if (!string.IsNullOrWhiteSpace(_paymentStatus))
    //     {
    //         if (!string.Equals(
    //                 invoice.PaymentStatus,
    //                 _paymentStatus,
    //                 StringComparison.OrdinalIgnoreCase))
    //         {
    //             return false;
    //         }
    //     }


    //     return true;
    // }


    private bool FilterInvoice(InvoiceModel invoice)
{
    // =========================
    // SEARCH
    // =========================

    if (!string.IsNullOrWhiteSpace(_searchString))
    {
        var search = _searchString.Trim();

        var matchesSearch =
            (invoice.InvoiceNumber ?? string.Empty)
                .Contains(search, StringComparison.OrdinalIgnoreCase)

            ||

            (invoice.PropertyName ?? string.Empty)
                .Contains(search, StringComparison.OrdinalIgnoreCase)

            ||

            (invoice.UnitNumber ?? string.Empty)
                .Contains(search, StringComparison.OrdinalIgnoreCase)

            ||

            (invoice.TenantName ?? string.Empty)
                .Contains(search, StringComparison.OrdinalIgnoreCase)

            ||

            invoice.BillingMonth
                .ToString("MMM yyyy")
                .Contains(search, StringComparison.OrdinalIgnoreCase)

            ||

            invoice.DueDate
                .ToString("dd MMM yyyy")
                .Contains(search, StringComparison.OrdinalIgnoreCase)

            ||

            invoice.TotalPayable
                .ToString("N2")
                .Contains(search, StringComparison.OrdinalIgnoreCase)

            ||

            (invoice.PaymentStatus ?? string.Empty)
                .Contains(search, StringComparison.OrdinalIgnoreCase);

        if (!matchesSearch)
        {
            return false;
        }
    }


    // =========================
    // PAYMENT STATUS
    // =========================

    if (!string.IsNullOrWhiteSpace(_paymentStatus))
    {
        if (!string.Equals(
                invoice.PaymentStatus?.Trim(),
                _paymentStatus.Trim(),
                StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }
    }


    // =========================
    // BILLING MONTH
    // =========================

    if (_billingMonth.HasValue)
    {
        var selectedMonth = _billingMonth.Value;

        if (invoice.BillingMonth.Year != selectedMonth.Year ||
            invoice.BillingMonth.Month != selectedMonth.Month)
        {
            return false;
        }
    }


    return true;
}
    private void AddInvoice()
    {
        NavigationManager.NavigateTo("/invoice/create");
    }

    private void EditOrViewInvoice(
        int invoiceId,
        string mode)
    {
        NavigationManager.NavigateTo(
            $"/invoice/{mode}/{invoiceId}");
    }

    private async Task ShowChargesAsync(InvoiceModel invoice)
    {
        var parameters = new DialogParameters
        {
            {
                nameof(InvoiceChargesDialog.Invoice),
                invoice
            },
            {
                nameof(InvoiceChargesDialog.ChargesList),
                invoice.Charges
            }
        };

        var options = new DialogOptions
        {
            MaxWidth = MaxWidth.Large,
            FullWidth = true,
            CloseOnEscapeKey = true
        };

        await DialogService.ShowAsync<InvoiceChargesDialog>(
            "Invoice Charges",
            parameters,
            options);
    }


    private async Task ShowTenantPopup(int tenantId)
    {
        _selectedTenant = await _StateContainer.Tenant.GetTenantAsync(tenantId);
        if (_selectedTenant is null)
        {
            _message = "Tenant details were not found.";
            return;
        }

        _showTenantPopup = true;
    }

    private void CloseTenantPopup()
    {
        _showTenantPopup = false;
        _selectedTenant = null;
    }
    
}