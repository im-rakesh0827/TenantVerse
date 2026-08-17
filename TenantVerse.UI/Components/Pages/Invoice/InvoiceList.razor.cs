using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.Shared.Models.Invoice;
using TenantVerse.UI.Services;


namespace TenantVerse.UI.Components.Pages.Invoice;

public partial class InvoiceList
{
    [Inject]
    private InvoiceService InvoiceService { get; set; } = default!;

    [Inject]
    private StateContainer _StateContainer { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;


    private List<InvoiceListModel> _invoices = new();

    private string _searchString = string.Empty;

    private string? _paymentStatus;

    private bool IsLoading;

    private string? _message;


    // =========================================================
    // FILTERED INVOICES
    // =========================================================

    protected IEnumerable<InvoiceListModel> FilteredInvoices =>
        string.IsNullOrWhiteSpace(_searchString)
            && string.IsNullOrWhiteSpace(_paymentStatus)
                ? _invoices
                : _invoices.Where(FilterInvoice);


    // =========================================================
    // INITIALIZE
    // =========================================================

    protected override async Task OnInitializedAsync()
    {
        await LoadInvoicesAsync();
    }


    // =========================================================
    // LOAD INVOICES
    // =========================================================

    private async Task LoadInvoicesAsync()
    {
        try
        {
            IsLoading = true;

            _message = null;


            // -------------------------------------------------
            // USE CACHE WHEN ALREADY LOADED
            // -------------------------------------------------

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


            // -------------------------------------------------
            // GET INVOICES FROM STATE CONTAINER
            // -------------------------------------------------

            _invoices =
                _StateContainer.Invoice.Invoices
                    .ToList();
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


    // =========================================================
    // FILTER INVOICE
    // =========================================================

    private bool FilterInvoice(InvoiceListModel invoice)
    {
        // -----------------------------------------------------
        // SEARCH FILTER
        // -----------------------------------------------------

        if (!string.IsNullOrWhiteSpace(_searchString))
        {
            var search =
                _searchString.Trim();

            var matchesSearch =
                (invoice.InvoiceNumber ?? string.Empty)
                    .Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)

                ||

                (invoice.PropertyName ?? string.Empty)
                    .Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)

                ||

                (invoice.UnitNumber ?? string.Empty)
                    .Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)

                ||

                (invoice.TenantName ?? string.Empty)
                    .Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)

                ||

                invoice.BillingMonth
                    .ToString("MMM yyyy")
                    .Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)

                ||

                invoice.DueDate
                    .ToString("dd MMM yyyy")
                    .Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)

                ||

                invoice.TotalPayable
                    .ToString()
                    .Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase)

                ||

                (invoice.PaymentStatus ?? string.Empty)
                    .Contains(
                        search,
                        StringComparison.OrdinalIgnoreCase);


            if (!matchesSearch)
            {
                return false;
            }
        }


        // -----------------------------------------------------
        // PAYMENT STATUS FILTER
        // -----------------------------------------------------

        if (!string.IsNullOrWhiteSpace(_paymentStatus))
        {
            if (!string.Equals(
                    invoice.PaymentStatus,
                    _paymentStatus,
                    StringComparison.OrdinalIgnoreCase))
            {
                return false;
            }
        }


        return true;
    }


    // =========================================================
    // ADD INVOICE
    // =========================================================

    private void AddInvoice()
    {
        NavigationManager.NavigateTo("/invoice/create");
    }


    // =========================================================
    // VIEW / EDIT INVOICE
    // =========================================================

    private void EditOrViewInvoice(
        int invoiceId,
        string mode)
    {
        NavigationManager.NavigateTo(
            $"/invoice/{mode}/{invoiceId}");
    }


    // =========================================================
    // STATUS COLOR
    // =========================================================

    private Color GetStatusColor(string? status)
    {
        return status?.Trim().ToLowerInvariant() switch
        {
            "paid" =>
                Color.Success,

            "pending" =>
                Color.Warning,

            "partiallypaid" =>
                Color.Info,

            "partially paid" =>
                Color.Info,

            "overdue" =>
                Color.Error,

            "cancelled" =>
                Color.Error,

            _ =>
                Color.Default
        };
    }

    
}