using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantVerse.Shared.Models.Invoice;
using TenantVerse.UI.Services;
using MudBlazor;
using Microsoft.AspNetCore.Components;


namespace TenantVerse.UI.Components.Pages.Invoice.PopUpDialog;
public partial class InvoicePaymentDialog
{
    [CascadingParameter]
    private IMudDialogInstance MudDialog { get; set; } = default!;

    [Parameter]
    public InvoiceModel? Invoice { get; set; }

    [Parameter]
    public bool IsReadOnly { get; set; }


    [Inject]
    private InvoicePaymentService InvoicePaymentService { get; set; } = default!;


    private List<InvoicePaymentModel> _payments = new();

    private decimal _totalPaid;

    private decimal _balanceDue;

    private bool _isLoading;

    private string? _message;


    //---------------------Record Payment--------------

    private MudForm? _paymentForm;

    private bool _showPaymentForm;

    private bool _isPaymentSaving;

    private decimal? _paymentAmount;

    private DateTime? _paymentDate = DateTime.Today;

    private string? _paymentMethod;

    private string? _paymentNotes;

    private string? _paymentFormMessage;


    protected override async Task OnInitializedAsync()
    {
        if (Invoice is null)
        {
            _message = "Invoice information is not available.";
            return;
        }

        await LoadPaymentHistoryAsync();
    }


    private async Task LoadPaymentHistoryAsync()
    {
        try
        {
            _isLoading = true;
            _message = null;

            var response =
                await InvoicePaymentService
                    .GetByInvoiceIdAsync(Invoice.InvoiceId);

            if (!response.IsSuccess ||
                response.Data is null)
            {
                _message =
                    response.Message ??
                    "Unable to load payment history.";

                return;
            }

            _payments = response.Data.ToList();

            CalculatePaymentSummary();
        }
        catch (Exception ex)
        {
            _message = ex.Message;
        }
        finally
        {
            _isLoading = false;
        }
    }


    private void CalculatePaymentSummary()
    {
        _totalPaid =
            _payments
                .Where(x =>
                    string.Equals(
                        x.PaymentStatus,
                        "Completed",
                        StringComparison.OrdinalIgnoreCase))
                .Sum(x => x.PaymentAmount);

        _balanceDue =
            Math.Max(
                0,
                Invoice!.TotalPayable - _totalPaid);
    }


    private Color GetPaymentStatusColor(string? status)
    {
        return status?.ToLowerInvariant() switch
        {
            "paid" => Color.Success,
            "completed" => Color.Success,

            "partially paid" => Color.Info,
            "partiallypaid" => Color.Info,

            "pending" => Color.Warning,

            "overdue" => Color.Error,
            "cancelled" => Color.Error,

            _ => Color.Default
        };
    }
    private void Close()
    {
        MudDialog.Close();
    }




    private void OpenRecordPaymentForm()
    {
        _paymentFormMessage = null;

        _paymentAmount = null;

        _paymentDate = DateTime.Today;

        _paymentMethod = null;

        _paymentNotes = null;

        _showPaymentForm = true;
    }

    private void CancelPaymentForm()
    {
        _showPaymentForm = false;

        _paymentFormMessage = null;
    }

    private async Task SavePaymentAsync()
    {
        if (Invoice is null)
        {
            return;
        }

        try
        {
            _paymentFormMessage = null;

            if (_paymentForm is not null)
            {
                await _paymentForm.Validate();

                if (!_paymentForm.IsValid)
                {
                    return;
                }
            }

            if (!_paymentAmount.HasValue ||
                _paymentAmount.Value <= 0)
            {
                _paymentFormMessage =
                    "Payment amount must be greater than zero.";

                return;
            }

            if (_paymentAmount.Value > _balanceDue)
            {
                _paymentFormMessage =
                    $"Payment amount cannot exceed the balance due of ₹{_balanceDue:N2}.";

                return;
            }

            if (!_paymentDate.HasValue)
            {
                _paymentFormMessage =
                    "Payment date is required.";

                return;
            }

            if (string.IsNullOrWhiteSpace(_paymentMethod))
            {
                _paymentFormMessage =
                    "Payment method is required.";

                return;
            }


            _isPaymentSaving = true;


            var request = new CreateInvoicePaymentRequest
            {
                InvoiceId = Invoice.InvoiceId,

                PaymentAmount = _paymentAmount.Value,

                PaymentDate = _paymentDate.Value,

                PaymentMethod = _paymentMethod,

                Notes = _paymentNotes,

                CreatedBy = "System"
            };


            var response =
                await InvoicePaymentService.CreateAsync(request);


            if (response is null ||
                !response.IsSuccess)
            {
                _paymentFormMessage =
                    response?.Message ??
                    "Unable to record payment.";

                return;
            }


            // Payment successfully created

            _showPaymentForm = false;


            // Reload payment history

            await LoadPaymentHistoryAsync();


            // Update invoice payment status locally

            UpdateInvoicePaymentStatus();
        }
        catch (Exception ex)
        {
            _paymentFormMessage = ex.Message;
        }
        finally
        {
            _isPaymentSaving = false;
        }
    }

    private void UpdateInvoicePaymentStatus()
    {
        if (Invoice is null)
        {
            return;
        }

        if (_totalPaid <= 0)
        {
            Invoice.PaymentStatus = "Pending";
        }
        else if (_totalPaid >= Invoice.TotalPayable)
        {
            Invoice.PaymentStatus = "Paid";
        }
        else
        {
            Invoice.PaymentStatus = "Partially Paid";
        }
    }

    
    
}