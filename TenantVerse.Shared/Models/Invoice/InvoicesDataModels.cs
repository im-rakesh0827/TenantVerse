using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace TenantVerse.Shared.Models.Invoice;
public class CreateInvoiceRequest
{
    public int PropertyId { get; set; }
    public int UnitId { get; set; }
    public int TenantId { get; set; }
    public DateTime BillingMonth { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal MonthlyRent { get; set; }
    public decimal PreviousReading { get; set; }
    public decimal CurrentReading { get; set; }
    public decimal ElectricityRate { get; set; }
    public decimal MaintenanceCharge { get; set; }
    public decimal WaterCharge { get; set; }
    public decimal LateFee { get; set; }
    public decimal Discount { get; set; }
    public string? Notes { get; set; }
    public string? CreatedBy { get; set; }
}
public class CreateInvoiceResponse
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;
    public decimal TotalPayable { get; set; }
}



public class UpdateInvoiceRequest
{
    public int InvoiceId { get; set; }

    public DateTime BillingMonth { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }

    public decimal DiscountAmount { get; set; }
    public decimal LateFee { get; set; }

    public string? Notes { get; set; }

    public string? UpdatedBy { get; set; }

    public List<UpdateInvoiceChargeRequest> Charges { get; set; } = new();
}

public class InvoiceModel
{
    public int InvoiceId { get; set; }
    public string InvoiceNumber { get; set; } = string.Empty;

    public int PropertyId { get; set; }
    public string PropertyName { get; set; } = string.Empty;

    public int UnitId { get; set; }
    public string UnitNumber { get; set; } = string.Empty;

    public int TenantId { get; set; }
    public string TenantName { get; set; } = string.Empty;

    public DateTime BillingMonth { get; set; }
    public DateTime InvoiceDate { get; set; }
    public DateTime DueDate { get; set; }

    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal LateFee { get; set; }
    public decimal TotalPayable { get; set; }

    public string PaymentStatus { get; set; } = string.Empty;

    public string? Notes { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }

    public List<InvoiceChargeModel> Charges { get; set; } = new();
    public List<InvoicePaymentModel> Payments { get; set; } = new();
}


public class InvoiceChargeModel
{
    public int InvoiceChargeId { get; set; }

    public int InvoiceId { get; set; }

    public string ChargeType { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public decimal? PreviousReading { get; set; }

    public decimal? CurrentReading { get; set; }

    public decimal? Units { get; set; }

    public decimal? Rate { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }
    public string? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }
    public string? UpdatedBy { get; set; }
}

public class UpdateInvoiceChargeRequest
{
    public string ChargeType { get; set; } = string.Empty;

    public string? Description { get; set; }

    public decimal Amount { get; set; }

    public decimal? PreviousReading { get; set; }

    public decimal? CurrentReading { get; set; }

    public decimal? Units { get; set; }

    public decimal? Rate { get; set; }
}



public class InvoicePaymentModel
{
    public int InvoicePaymentId { get; set; }

    public int InvoiceId { get; set; }

    public decimal PaymentAmount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string PaymentStatus { get; set; } = string.Empty;

    public string? TransactionReference { get; set; }

    public string? Notes { get; set; }

    public bool IsActive { get; set; }

    public DateTime CreatedOn { get; set; }

    public string? CreatedBy { get; set; }

    public DateTime? UpdatedOn { get; set; }

    public string? UpdatedBy { get; set; }
}

public class CreateInvoicePaymentResponse
{
    public int InvoicePaymentId { get; set; }

    public int InvoiceId { get; set; }

    public decimal PaymentAmount { get; set; }

    public decimal TotalPayable { get; set; }

    public decimal TotalPaid { get; set; }

    public decimal BalanceDue { get; set; }

    public string PaymentStatus { get; set; } = string.Empty;
}


public class CreateInvoicePaymentRequest
{
    public int InvoiceId { get; set; }

    public decimal PaymentAmount { get; set; }

    public DateTime PaymentDate { get; set; }

    public string PaymentMethod { get; set; } = string.Empty;

    public string? TransactionReference { get; set; }

    public string? Notes { get; set; }

    public string? CreatedBy { get; set; }
}