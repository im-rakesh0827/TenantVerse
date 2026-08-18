
using Microsoft.AspNetCore.Components;
using MudBlazor;
using TenantVerse.Shared.Models.Invoice;
using TenantVerse.UI.Services;
using TenantVerse.UI.Components.Pages.Invoice.PopUpDialog;

namespace TenantVerse.UI.Components.Pages.Invoice;

public partial class UpdateOrViewInvoice : ComponentBase
{
    [Parameter]
    public int InvoiceId { get; set; }
     [Parameter]
    public string mode {get;set;} = string.Empty;

    [Inject]
    private InvoiceService InvoiceService { get; set; } = default!;

     // [Inject]
     // private InvoicePaymentService InvoicePaymentService { get; set; } = default!;

     [Inject]
      private IDialogService DialogService { get; set; } = default!;

    [Inject]
    private StateContainer _StateContainer { get; set; } = default!;

    [Inject]
    private NavigationManager NavigationManager { get; set; } = default!;

    [Inject]
    private ISnackbar Snackbar { get; set; } = default!;




    private MudForm? _form;

    private InvoiceModel? Invoice;

    private bool IsLoading = true;
    private bool _isSaving;

    private string? _message;


    private DateTime? _billingMonth;
    private DateTime? _invoiceDate;
    private DateTime? _dueDate;


//-------------------Invoice Payment------------------
     private List<InvoicePaymentModel> _payments = new();
     private decimal _totalPaid;
     private decimal _balanceDue;
     private bool _isPaymentLoading;
     private bool _isPaymentSaving;
     private string? _paymentMessage;
     private CreateInvoicePaymentRequest _paymentRequest = new()
     {
         PaymentDate = DateTime.Today,
         PaymentMethod = "Cash"
     };


    private bool IsReadOnly =>
        string.Equals(
            mode,
            "view",
            StringComparison.OrdinalIgnoreCase);


    protected override async Task OnInitializedAsync()
    {
        await LoadInvoiceAsync();
    }


    private async Task LoadInvoiceAsync()
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

            var result = (_StateContainer.Invoice.Invoices.FirstOrDefault(x=>x.InvoiceId==InvoiceId));
            if(result!=null){
               Invoice = result;
            }
            else{
                 var response =
                 await InvoiceService.GetByIdAsync(InvoiceId);
                 await Task.Delay(2000);
                if (!response.IsSuccess ||
                   response.Data == null)
               {
                   _message =
                       response.Message ??
                       "Unable to load invoice.";

                   return;
               }
               Invoice = response.Data;
            }
            _billingMonth = Invoice.BillingMonth;
            _invoiceDate = Invoice.InvoiceDate;
            _dueDate = Invoice.DueDate;
          //   await LoadPaymentHistoryAsync();
        }
        catch (Exception ex)
        {
            _message =
                ex.Message;
        }
        finally
        {
            IsLoading = false;
        }
    }


    private bool IsElectricity(
        InvoiceChargeModel charge)
    {
        return charge.ChargeType.Equals(
            "Electricity",
            StringComparison.OrdinalIgnoreCase);
    }


    private decimal CalculateUnits(
        InvoiceChargeModel charge)
    {
        if (!charge.PreviousReading.HasValue ||
            !charge.CurrentReading.HasValue)
        {
            return 0;
        }

        return Math.Max(
            0,
            charge.CurrentReading.Value -
            charge.PreviousReading.Value);
    }


    private decimal CalculateElectricityAmount(
        InvoiceChargeModel charge)
    {
        var units =
            CalculateUnits(charge);

        var rate =
            charge.Rate ?? 0;

        return units * rate;
    }


    private decimal CalculateChargeAmount(
        InvoiceChargeModel charge)
    {
        if (IsElectricity(charge))
        {
            return CalculateElectricityAmount(charge);
        }

        return charge.Amount;
    }


    private decimal CalculateSubtotal()
    {
        if (Invoice == null)
            return 0;

        return Invoice.Charges.Sum(
            CalculateChargeAmount);
    }


    private decimal CalculateTotal()
    {
        if (Invoice == null)
            return 0;

        return
            CalculateSubtotal()
            - Invoice.DiscountAmount
            + Invoice.LateFee;
    }


    private async Task SaveAsync()
    {
        if (Invoice == null)
            return;


        var validationResult =
            await ValidateFormAsync();

        if (!validationResult)
            return;


        try
        {
            _isSaving = true;
            _message = null;


            var request =
                new UpdateInvoiceRequest
                {
                    InvoiceId =
                        Invoice.InvoiceId,

                    BillingMonth =
                        _billingMonth ??
                        Invoice.BillingMonth,

                    InvoiceDate =
                        _invoiceDate ??
                        Invoice.InvoiceDate,

                    DueDate =
                        _dueDate ??
                        Invoice.DueDate,

                    DiscountAmount =
                        Invoice.DiscountAmount,

                    LateFee =
                        Invoice.LateFee,

                    Notes =
                        Invoice.Notes,

                    UpdatedBy =
                        "Rakesh",

                    Charges =
                        Invoice.Charges
                            .Select(charge =>
                                new UpdateInvoiceChargeRequest
                                {
                                    ChargeType =
                                        charge.ChargeType,

                                    Description =
                                        charge.Description,

                                    Amount =
                                        CalculateChargeAmount(
                                            charge),

                                    PreviousReading =
                                        charge.PreviousReading,

                                    CurrentReading =
                                        charge.CurrentReading,

                                    Units =
                                        CalculateUnits(
                                            charge),

                                    Rate =
                                        charge.Rate
                                })
                            .ToList()
                };

            
            var response =
                await InvoiceService.UpdateAsync(
                    request);

            
            if (!response.IsSuccess)
            {
                _message =
                    response.Message ??
                    "Unable to update invoice.";

                Snackbar.Add(
                    _message,
                    Severity.Error);

                return;
            }

            await _StateContainer.Invoice.RefreshAsync();

            await Task.Delay(1000);
            Snackbar.Add(
                "Invoice updated successfully.",
                Severity.Success);

            NavigationManager.NavigateTo($"/invoice");

        }
        catch (Exception ex)
        {
            _message = ex.Message;

            Snackbar.Add(
                ex.Message,
                Severity.Error);
        }
        finally
        {
            _isSaving = false;
        }
    }


    private async Task<bool> ValidateFormAsync()
    {
        if (_form == null)
            return true;

        await _form.Validate();

        return _form.IsValid;
    }


    private void GoToEdit()
    {
        if (Invoice == null)
            return;

        NavigationManager.NavigateTo(
            $"/invoice/edit/{Invoice.InvoiceId}");
    }


    private void GoBack()
    {
        NavigationManager.NavigateTo("/invoice");
    }

private async Task OpenPaymentDetailsAsync()
{
    if (Invoice is null)
    {
        return;
    }

    var parameters = new DialogParameters
    {
        {
            nameof(InvoicePaymentDialog.Invoice),
            Invoice
        },
        {
            nameof(InvoicePaymentDialog.IsReadOnly),
            IsReadOnly
        }
    };

    var options = new DialogOptions
    {
        CloseOnEscapeKey = true,
        MaxWidth = MaxWidth.Medium,
        FullWidth = true,
        BackdropClick = false
    };

    await DialogService.ShowAsync<InvoicePaymentDialog>(
        "Payment Details",
        parameters,
        options);
}

}