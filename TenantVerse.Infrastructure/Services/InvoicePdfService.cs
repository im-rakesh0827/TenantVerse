using QuestPDF.Fluent;
using QuestPDF.Helpers;
using QuestPDF.Infrastructure;
using TenantVerse.Application.Interfaces.Services;
using TenantVerse.Shared.Models.Invoice;

namespace TenantVerse.Infrastructure.Services;

public class InvoicePdfService : IInvoicePdfService
{
    public byte[] GenerateInvoicePdf(
        InvoiceModel invoice,
        IEnumerable<InvoiceChargeModel> charges,
        IEnumerable<InvoicePaymentModel> payments)
    {
        var chargeList = charges?.ToList()
            ?? new List<InvoiceChargeModel>();

        var paymentList = payments?.ToList()
            ?? new List<InvoicePaymentModel>();

        var document = Document.Create(container =>
        {
            container.Page(page =>
            {
                page.Size(PageSizes.A4);
                page.Margin(40);

                page.DefaultTextStyle(
                    x => x.FontSize(10));

                page.Header()
                    .Element(container =>
                        ComposeHeader(
                            container,
                            invoice));

                page.Content()
                    .Element(container =>
                        ComposeContent(
                            container,
                            invoice,
                            chargeList,
                            paymentList));

                page.Footer()
                    .AlignCenter()
                    .Text(text =>
                    {
                        text.Span("TENANTVERSE • ");
                        text.CurrentPageNumber();
                        text.Span(" / ");
                        text.TotalPages();
                    });
            });
        });

        return document.GeneratePdf();
    }


    // =========================================================
    // HEADER
    // =========================================================

    private void ComposeHeader(
        IContainer container,
        InvoiceModel invoice)
    {
        container.Column(column =>
        {
            column.Item()
                .AlignCenter()
                .Text("TENANTVERSE RENT INVOICE")
                .Bold()
                .FontSize(20);

            column.Item()
                .PaddingTop(5)
                .AlignCenter()
                .Text(
                    $"Invoice No: {invoice.InvoiceNumber}")
                .FontSize(11);

            column.Item()
                .PaddingTop(10)
                .LineHorizontal(1);
        });
    }


    // =========================================================
    // CONTENT
    // =========================================================

    private void ComposeContent(
        IContainer container,
        InvoiceModel invoice,
        List<InvoiceChargeModel> charges,
        List<InvoicePaymentModel> payments)
    {
        container.Column(column =>
        {
            // =================================================
            // LANDLORD + INVOICE INFORMATION
            // =================================================

            column.Item()
                .PaddingTop(15)
                .Row(row =>
                {
                    row.RelativeItem()
                        .Column(left =>
                        {
                            left.Item()
                                .Text("Landlord")
                                .Bold();

                            left.Item()
                                .Text(invoice.Property?.OwnerName);

                            left.Item()
                                .Text(invoice.Property?.City +", "+invoice.Property?.State);
                        });

                    row.RelativeItem()
                        .Column(right =>
                        {
                            right.Item()
                                .Text(
                                    $"Invoice Date: " +
                                    $"{invoice.InvoiceDate:dd MMM yyyy}");

                            right.Item()
                                .Text(
                                    $"Billing Month: " +
                                    $"{invoice.BillingMonth:MMM yyyy}");

                            right.Item()
                                .Text(
                                    $"Due Date: " +
                                    $"{invoice.DueDate:dd MMM yyyy}");

                            right.Item()
                                .Text(
                                    $"Status: " +
                                    $"{(invoice.PaymentStatus ?? "Pending")}")
                                .Bold();
                        });
                });


            // =================================================
            // TENANT INFORMATION
            // =================================================

            column.Item()
                .PaddingTop(20)
                .Text("Tenant Details")
                .Bold()
                .FontSize(13);

            column.Item()
                .PaddingTop(5)
                .Border(1)
                .Padding(10)
                .Column(tenant =>
                {
                    tenant.Item()
                        .Text(
                            $"Tenant: " +
                            $"{invoice.TenantName ?? "-"}");

                    tenant.Item()
                        .Text(
                            $"Property: " +
                            $"{invoice.PropertyName ?? "-"}");

                    tenant.Item()
                        .Text(
                            $"Flat: " +
                            $"{invoice.UnitNumber ?? "-"}");
                });


            // =================================================
            // CHARGES
            // =================================================

            column.Item()
                .PaddingTop(20)
                .Text("Charge Details")
                .Bold()
                .FontSize(13);

            column.Item()
                .PaddingTop(5)
                .Table(table =>
                {
                    table.ColumnsDefinition(columns =>
                    {
                        columns.RelativeColumn(2);
                        columns.RelativeColumn(4);
                        columns.RelativeColumn(2);
                    });

                    table.Header(header =>
                    {
                        header.Cell()
                            .Element(HeaderCell)
                            .Text("Charge Type");

                        header.Cell()
                            .Element(HeaderCell)
                            .Text("Description");

                        header.Cell()
                            .Element(HeaderCell)
                            .AlignRight()
                            .Text("Amount");
                    });

                    foreach (var charge in charges)
                    {
                        table.Cell()
                            .Element(BodyCell)
                            .Text(
                                charge.ChargeType ?? "-");

                        table.Cell()
                            .Element(BodyCell)
                            .Text(
                                charge.Description ?? "-");

                        table.Cell()
                            .Element(BodyCell)
                            .AlignRight()
                            .Text(
                                $"₹{charge.Amount:N2}");
                    }
                });


            // =================================================
            // ELECTRICITY
            // =================================================

            var electricity =
                charges.FirstOrDefault(x =>
                    string.Equals(
                        x.ChargeType,
                        "Electricity",
                        StringComparison.OrdinalIgnoreCase));

            if (electricity is not null)
            {
                column.Item()
                    .PaddingTop(20)
                    .Text("Electricity Details")
                    .Bold()
                    .FontSize(13);

                column.Item()
                    .PaddingTop(5)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                            columns.RelativeColumn();
                        });

                        table.Header(header =>
                        {
                            header.Cell()
                                .Element(HeaderCell)
                                .Text("Previous");

                            header.Cell()
                                .Element(HeaderCell)
                                .Text("Current");

                            header.Cell()
                                .Element(HeaderCell)
                                .Text("Units");

                            header.Cell()
                                .Element(HeaderCell)
                                .Text("Rate");
                        });

                        table.Cell()
                            .Element(BodyCell)
                            .Text(
                                electricity.PreviousReading?
                                    .ToString("N2") ?? "-");

                        table.Cell()
                            .Element(BodyCell)
                            .Text(
                                electricity.CurrentReading?
                                    .ToString("N2") ?? "-");

                        table.Cell()
                            .Element(BodyCell)
                            .Text(
                                electricity.Units?
                                    .ToString("N2") ?? "-");

                        table.Cell()
                            .Element(BodyCell)
                            .Text(
                                electricity.Rate.HasValue
                                    ? $"₹{electricity.Rate.Value:N2}"
                                    : "-");
                    });
            }


            // =================================================
            // PAYMENT SUMMARY
            // =================================================

            column.Item()
                .PaddingTop(20)
                .AlignRight()
                .Column(total =>
                {
                    total.Item()
                        .Text(
                            $"Total Payable: " +
                            $"₹{invoice.TotalPayable:N2}")
                        .Bold()
                        .FontSize(14);

                    var totalPaid =
                        payments
                            .Where(x =>
                                string.Equals(
                                    x.PaymentStatus,
                                    "Completed",
                                    StringComparison.OrdinalIgnoreCase))
                            .Sum(x => x.PaymentAmount);

                    var balance =
                        invoice.TotalPayable - totalPaid;

                    if (balance < 0)
                    {
                        balance = 0;
                    }

                    total.Item()
                        .Text(
                            $"Total Paid: " +
                            $"₹{totalPaid:N2}");

                    total.Item()
                        .Text(
                            $"Balance Due: " +
                            $"₹{balance:N2}")
                        .Bold();
                });


            // =================================================
            // PAYMENT HISTORY
            // =================================================

            if (payments.Count > 0)
            {
                column.Item()
                    .PaddingTop(20)
                    .Text("Payment History")
                    .Bold()
                    .FontSize(13);

                column.Item()
                    .PaddingTop(5)
                    .Table(table =>
                    {
                        table.ColumnsDefinition(columns =>
                        {
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                            columns.RelativeColumn(2);
                        });

                        table.Header(header =>
                        {
                            header.Cell()
                                .Element(HeaderCell)
                                .Text("Date");

                            header.Cell()
                                .Element(HeaderCell)
                                .Text("Method");

                            header.Cell()
                                .Element(HeaderCell)
                                .Text("Status");

                            header.Cell()
                                .Element(HeaderCell)
                                .AlignRight()
                                .Text("Amount");
                        });

                        foreach (var payment in payments)
                        {
                            table.Cell()
                                .Element(BodyCell)
                                .Text(
                                    payment.PaymentDate
                                        .ToString("dd MMM yyyy"));

                            table.Cell()
                                .Element(BodyCell)
                                .Text(
                                    payment.PaymentMethod ?? "-");

                            table.Cell()
                                .Element(BodyCell)
                                .Text(
                                    payment.PaymentStatus ?? "-");

                            table.Cell()
                                .Element(BodyCell)
                                .AlignRight()
                                .Text(
                                    $"₹{payment.PaymentAmount:N2}");
                        }
                    });
            }


            // =================================================
            // NOTES
            // =================================================

            if (!string.IsNullOrWhiteSpace(invoice.Notes))
            {
                column.Item()
                    .PaddingTop(20)
                    .Text("Notes")
                    .Bold();

                column.Item()
                    .PaddingTop(5)
                    .Text(invoice.Notes);
            }


            // =================================================
            // PAYMENT REMINDER
            // =================================================

            column.Item()
                .PaddingTop(25)
                .AlignCenter()
                .Text(
                    "Please make the payment before the due date.")
                .Bold();
        });
    }


    // =========================================================
    // TABLE HEADER
    // =========================================================

    private static IContainer HeaderCell(
        IContainer container)
    {
        return container
            .Background(Colors.Grey.Lighten2)
            .Border(1)
            .Padding(5);
    }


    // =========================================================
    // TABLE BODY
    // =========================================================

    private static IContainer BodyCell(
        IContainer container)
    {
        return container
            .Border(1)
            .Padding(5);
    }


public Task<byte[]> GenerateInvoicePdfAsync(InvoiceModel invoice)
{
    var document = Document.Create(container =>
    {
        container.Page(page =>
        {
            page.Size(PageSizes.A4);
            page.MarginHorizontal(35);
            page.MarginVertical(30);

            page.DefaultTextStyle(x =>
                x.FontSize(9)
                 .FontColor(Colors.Grey.Darken3));

            // =========================================================
            // HEADER
            // =========================================================

            page.Header()
                .PaddingBottom(15)
                .Row(row =>
                {
                    // Left - TenantVerse / Landlord
                    row.RelativeItem()
                        .Column(column =>
                        {
                            column.Item()
                                .Text("TENANTVERSE")
                                .FontSize(24)
                                .Bold()
                                .FontColor(Colors.Blue.Darken2);

                            column.Item()
                                .Text("Property & Rental Management")
                                .FontSize(9)
                                .FontColor(Colors.Grey.Darken1);

                            column.Item()
                                .PaddingTop(8)
                                .Text(invoice.Property?.OwnerName)
                                .Bold()
                                .FontSize(10);

                            column.Item()
                                .Text(invoice.Property?.City+", "+invoice.Property?.State)
                                .FontSize(9);
                        });

                    // Right - Invoice title
                    row.ConstantItem(180)
                        .AlignRight()
                        .Column(column =>
                        {
                            column.Item()
                                .Text("RENT INVOICE")
                                .FontSize(22)
                                .Bold()
                                .FontColor(Colors.Grey.Darken3);

                            column.Item()
                                .PaddingTop(5)
                                .Text(invoice.InvoiceNumber)
                                .FontSize(11)
                                .Bold();

                            column.Item()
                                .Text($"Invoice Date: {invoice.InvoiceDate:dd MMM yyyy}")
                                .FontSize(9);

                            column.Item()
                                .Text($"Due Date: {invoice.DueDate:dd MMM yyyy}")
                                .FontSize(9);
                        });
                });

            // =========================================================
            // CONTENT
            // =========================================================

            page.Content()
                .Column(column =>
                {
                    column.Spacing(12);

                    // -------------------------------------------------
                    // INVOICE INFORMATION
                    // -------------------------------------------------

                    column.Item()
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Background(Colors.Grey.Lighten4)
                        .Padding(10)
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .Column(info =>
                                {
                                    info.Item()
                                        .Text("BILLING MONTH")
                                        .FontSize(8)
                                        .Bold()
                                        .FontColor(Colors.Grey.Darken1);

                                    info.Item()
                                        .PaddingTop(3)
                                        .Text(invoice.BillingMonth.ToString("MMMM yyyy"))
                                        .FontSize(10)
                                        .Bold();
                                });

                            row.RelativeItem()
                                .Column(info =>
                                {
                                    info.Item()
                                        .Text("INVOICE DATE")
                                        .FontSize(8)
                                        .Bold()
                                        .FontColor(Colors.Grey.Darken1);

                                    info.Item()
                                        .PaddingTop(3)
                                        .Text(invoice.InvoiceDate.ToString("dd MMM yyyy"))
                                        .FontSize(10)
                                        .Bold();
                                });

                            row.RelativeItem()
                                .Column(info =>
                                {
                                    info.Item()
                                        .Text("DUE DATE")
                                        .FontSize(8)
                                        .Bold()
                                        .FontColor(Colors.Grey.Darken1);

                                    info.Item()
                                        .PaddingTop(3)
                                        .Text(invoice.DueDate.ToString("dd MMM yyyy"))
                                        .FontSize(10)
                                        .Bold();
                                });

                            row.RelativeItem()
                                .Column(info =>
                                {
                                    info.Item()
                                        .Text("PAYMENT STATUS")
                                        .FontSize(8)
                                        .Bold()
                                        .FontColor(Colors.Grey.Darken1);

                                    info.Item()
                                        .PaddingTop(3)
                                        .Text(invoice.PaymentStatus)
                                        .FontSize(10)
                                        .Bold()
                                        .FontColor(GetPaymentStatusColor(invoice.PaymentStatus));
                                });
                        });

                    // -------------------------------------------------
                    // BILL TO / PROPERTY
                    // -------------------------------------------------

                    column.Item()
                        .Row(row =>
                        {
                            // BILL TO
                            row.RelativeItem()
                                .PaddingRight(6)
                                .Border(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .Padding(10)
                                .Column(card =>
                                {
                                    card.Item()
                                        .Text("BILL TO")
                                        .FontSize(9)
                                        .Bold()
                                        .FontColor(Colors.Blue.Darken2);

                                    card.Item()
                                        .PaddingTop(8)
                                        .Text(invoice.TenantName)
                                        .FontSize(11)
                                        .Bold();

                                    // Add these only if your model contains them.
                                    // card.Item().Text(invoice.TenantPhone);
                                    // card.Item().Text(invoice.TenantEmail);
                                });

                            // PROPERTY
                            row.RelativeItem()
                                .PaddingLeft(6)
                                .Border(1)
                                .BorderColor(Colors.Grey.Lighten2)
                                .Padding(10)
                                .Column(card =>
                                {
                                    card.Item()
                                        .Text("PROPERTY")
                                        .FontSize(9)
                                        .Bold()
                                        .FontColor(Colors.Blue.Darken2);

                                    card.Item()
                                        .PaddingTop(8)
                                        .Text(invoice.PropertyName)
                                        .FontSize(11)
                                        .Bold();

                                    card.Item()
                                        .PaddingTop(3)
                                        .Text($"Flat: {invoice.UnitNumber}")
                                        .FontSize(9);
                                });
                        });

                    // -------------------------------------------------
                    // CHARGES TABLE
                    // -------------------------------------------------

                    column.Item()
                        .Text("CHARGE DETAILS")
                        .FontSize(11)
                        .Bold()
                        .FontColor(Colors.Grey.Darken3);

                    column.Item()
                        .Table(table =>
                        {
                            table.ColumnsDefinition(columns =>
                            {
                                columns.RelativeColumn(3.5f);
                                columns.RelativeColumn(1.3f);
                                columns.RelativeColumn(1.3f);
                                columns.RelativeColumn(1.2f);
                                columns.RelativeColumn(1.5f);
                            });

                            // Header
                            table.Header(header =>
                            {
                                header.Cell()
                                    .Background(Colors.Blue.Darken2)
                                    .Padding(7)
                                    .Text("DESCRIPTION")
                                    .FontSize(8)
                                    .Bold()
                                    .FontColor(Colors.White);

                                header.Cell()
                                    .Background(Colors.Blue.Darken2)
                                    .Padding(7)
                                    .AlignRight()
                                    .Text("UNITS")
                                    .FontSize(8)
                                    .Bold()
                                    .FontColor(Colors.White);

                                header.Cell()
                                    .Background(Colors.Blue.Darken2)
                                    .Padding(7)
                                    .AlignRight()
                                    .Text("RATE")
                                    .FontSize(8)
                                    .Bold()
                                    .FontColor(Colors.White);

                                header.Cell()
                                    .Background(Colors.Blue.Darken2)
                                    .Padding(7)
                                    .AlignRight()
                                    .Text("TYPE")
                                    .FontSize(8)
                                    .Bold()
                                    .FontColor(Colors.White);

                                header.Cell()
                                    .Background(Colors.Blue.Darken2)
                                    .Padding(7)
                                    .AlignRight()
                                    .Text("AMOUNT")
                                    .FontSize(8)
                                    .Bold()
                                    .FontColor(Colors.White);
                            });

                            foreach (var charge in invoice.Charges)
                            {
                                table.Cell()
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(7)
                                    .Text(charge.Description);

                                table.Cell()
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(7)
                                    .AlignRight()
                                    .Text(
                                        charge.Units.HasValue
                                            ? charge.Units.Value.ToString("N0")
                                            : "-");

                                table.Cell()
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(7)
                                    .AlignRight()
                                    .Text(
                                        charge.Rate.HasValue
                                            ? $"₹{charge.Rate.Value:N2}"
                                            : "-");

                                table.Cell()
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(7)
                                    .AlignRight()
                                    .Text(charge.ChargeType);

                                table.Cell()
                                    .BorderBottom(1)
                                    .BorderColor(Colors.Grey.Lighten2)
                                    .Padding(7)
                                    .AlignRight()
                                    .Text($"₹{charge.Amount:N2}");
                            }
                        });

                    // -------------------------------------------------
                    // ELECTRICITY DETAILS
                    // -------------------------------------------------

                    var electricityCharge = invoice.Charges
                        .FirstOrDefault(x =>
                            string.Equals(
                                x.ChargeType,
                                "Electricity",
                                StringComparison.OrdinalIgnoreCase));

                    if (electricityCharge != null)
                    {
                        column.Item()
                            .Text("ELECTRICITY DETAILS")
                            .FontSize(11)
                            .Bold()
                            .FontColor(Colors.Grey.Darken3);

                        column.Item()
                            .Border(1)
                            .BorderColor(Colors.Grey.Lighten2)
                            .Padding(10)
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                    columns.RelativeColumn();
                                });

                                table.Header(header =>
                                {
                                    AddElectricityHeader(
                                        header.Cell(),
                                        "PREVIOUS READING");

                                    AddElectricityHeader(
                                        header.Cell(),
                                        "CURRENT READING");

                                    AddElectricityHeader(
                                        header.Cell(),
                                        "UNITS");

                                    AddElectricityHeader(
                                        header.Cell(),
                                        "RATE");

                                    AddElectricityHeader(
                                        header.Cell(),
                                        "AMOUNT");
                                });

                                table.Cell()
                                    .PaddingTop(7)
                                    .AlignCenter()
                                    .Text(
                                        electricityCharge.PreviousReading
                                            ?.ToString("N0") ?? "-");

                                table.Cell()
                                    .PaddingTop(7)
                                    .AlignCenter()
                                    .Text(
                                        electricityCharge.CurrentReading
                                            ?.ToString("N0") ?? "-");

                                table.Cell()
                                    .PaddingTop(7)
                                    .AlignCenter()
                                    .Text(
                                        electricityCharge.Units
                                            ?.ToString("N0") ?? "-");

                                table.Cell()
                                    .PaddingTop(7)
                                    .AlignCenter()
                                    .Text(
                                        electricityCharge.Rate.HasValue
                                            ? $"₹{electricityCharge.Rate.Value:N2}"
                                            : "-");

                                table.Cell()
                                    .PaddingTop(7)
                                    .AlignCenter()
                                    .Text(
                                        $"₹{electricityCharge.Amount:N2}")
                                    .Bold();
                            });
                    }

                    // -------------------------------------------------
                    // SUMMARY
                    // -------------------------------------------------

                    column.Item()
                        .AlignRight()
                        .Width(280)
                        .Column(summary =>
                        {
                            summary.Item()
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text("Subtotal");

                                    row.ConstantItem(110)
                                        .AlignRight()
                                        .Text($"₹{invoice.SubTotal:N2}");
                                });

                            summary.Item()
                                .PaddingTop(5)
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text("Discount");

                                    row.ConstantItem(110)
                                        .AlignRight()
                                        .Text(
                                            $"-₹{invoice.DiscountAmount:N2}");
                                });

                            summary.Item()
                                .PaddingTop(5)
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text("Late Fee");

                                    row.ConstantItem(110)
                                        .AlignRight()
                                        .Text($"₹{invoice.LateFee:N2}");
                                });

                            summary.Item()
                                .PaddingTop(8)
                                .BorderTop(1)
                                .BorderColor(Colors.Grey.Darken1)
                                .PaddingTop(8)
                                .Row(row =>
                                {
                                    row.RelativeItem()
                                        .Text("TOTAL PAYABLE")
                                        .FontSize(12)
                                        .Bold();

                                    row.ConstantItem(110)
                                        .AlignRight()
                                        .Text($"₹{invoice.TotalPayable:N2}")
                                        .FontSize(12)
                                        .Bold()
                                        .FontColor(Colors.Blue.Darken2);
                                });
                        });

                    // -------------------------------------------------
                    // PAYMENT HISTORY
                    // -------------------------------------------------

                    if (invoice.Payments != null &&
                        invoice.Payments.Any())
                    {
                        column.Item()
                            .Text("PAYMENT HISTORY")
                            .FontSize(11)
                            .Bold()
                            .FontColor(Colors.Grey.Darken3);

                        column.Item()
                            .Table(table =>
                            {
                                table.ColumnsDefinition(columns =>
                                {
                                    columns.RelativeColumn(1.5f);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(2);
                                    columns.RelativeColumn(1.5f);
                                });

                                table.Header(header =>
                                {
                                    AddPaymentHeader(
                                        header.Cell(),
                                        "DATE");

                                    AddPaymentHeader(
                                        header.Cell(),
                                        "METHOD");

                                    AddPaymentHeader(
                                        header.Cell(),
                                        "REFERENCE");

                                    AddPaymentHeader(
                                        header.Cell(),
                                        "AMOUNT");
                                });

                                foreach (var payment in invoice.Payments)
                                {
                                    table.Cell()
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(7)
                                        .Text(
                                            payment.PaymentDate
                                                .ToString("dd MMM yyyy"));

                                    table.Cell()
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(7)
                                        .Text(payment.PaymentMethod ?? "-");

                                    table.Cell()
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(7)
                                        .Text("-");

                                    table.Cell()
                                        .BorderBottom(1)
                                        .BorderColor(Colors.Grey.Lighten2)
                                        .Padding(7)
                                        .AlignRight()
                                        .Text(
                                            $"₹{payment.PaymentAmount:N2}");
                                }
                            });
                    }

                    // -------------------------------------------------
                    // PAYMENT SUMMARY
                    // -------------------------------------------------

                    decimal totalPaid = invoice.Payments?.Sum(
                        x => x.PaymentAmount) ?? 0;

                    decimal balanceDue =
                        invoice.TotalPayable - totalPaid;

                    column.Item()
                        .Border(1)
                        .BorderColor(Colors.Grey.Lighten2)
                        .Background(Colors.Grey.Lighten4)
                        .Padding(10)
                        .Row(row =>
                        {
                            row.RelativeItem()
                                .Column(payment =>
                                {
                                    payment.Item()
                                        .Text("AMOUNT PAID")
                                        .FontSize(8)
                                        .Bold()
                                        .FontColor(Colors.Grey.Darken1);

                                    payment.Item()
                                        .PaddingTop(3)
                                        .Text($"₹{totalPaid:N2}")
                                        .FontSize(11)
                                        .Bold();
                                });

                            row.RelativeItem()
                                .AlignRight()
                                .Column(payment =>
                                {
                                    payment.Item()
                                        .AlignRight()
                                        .Text("BALANCE DUE")
                                        .FontSize(8)
                                        .Bold()
                                        .FontColor(Colors.Grey.Darken1);

                                    payment.Item()
                                        .PaddingTop(3)
                                        .AlignRight()
                                        .Text($"₹{balanceDue:N2}")
                                        .FontSize(13)
                                        .Bold()
                                        .FontColor(
                                            balanceDue > 0
                                                ? Colors.Red.Darken1
                                                : Colors.Green.Darken1);
                                });
                        });

                    // -------------------------------------------------
                    // PAYMENT REMINDER
                    // -------------------------------------------------

                    column.Item()
                        .PaddingTop(5)
                        .Border(1)
                        .BorderColor(Colors.Blue.Lighten3)
                        .Background(Colors.Blue.Lighten5)
                        .Padding(10)
                        .Text(
                            $"Payment is due on or before " +
                            $"{invoice.DueDate:dd MMM yyyy}. " +
                            "Please ensure the outstanding amount is paid " +
                            "within the due date.")
                        .FontSize(9);

                    // -------------------------------------------------
                    // THANK YOU
                    // -------------------------------------------------

                    column.Item()
                        .PaddingTop(8)
                        .AlignCenter()
                        .Text("Thank you for your payment.")
                        .FontSize(9)
                        .Italic()
                        .FontColor(Colors.Grey.Darken1);
                });

            // =========================================================
            // FOOTER
            // =========================================================

            page.Footer()
                .BorderTop(1)
                .BorderColor(Colors.Grey.Lighten2)
                .PaddingTop(8)
                .Row(row =>
                {
                    row.RelativeItem()
                        .Text("Generated by TenantVerse")
                        .FontSize(8)
                        .FontColor(Colors.Grey.Darken1);

                    row.RelativeItem()
                        .AlignRight()
                        .Text(text =>
                        {
                            text.Span("Page ")
                                .FontSize(8);

                            text.CurrentPageNumber()
                                .FontSize(8);

                            text.Span(" of ")
                                .FontSize(8);

                            text.TotalPages()
                                .FontSize(8);
                        });
                });
        });
    });

    return Task.FromResult(document.GeneratePdf());
}


// =============================================================
// HELPER METHODS
// =============================================================

private static void AddElectricityHeader(
    IContainer container,
    string text)
{
    container
        .Background(Colors.Grey.Lighten3)
        .Padding(6)
        .AlignCenter()
        .Text(text)
        .FontSize(7)
        .Bold()
        .FontColor(Colors.Grey.Darken2);
}

private static void AddPaymentHeader(
    IContainer container,
    string text)
{
    container
        .Background(Colors.Blue.Darken2)
        .Padding(7)
        .Text(text)
        .FontSize(8)
        .Bold()
        .FontColor(Colors.White);
}

    private static string GetPaymentStatusColor(string? status)
    {
        return status?.ToUpperInvariant() switch
        {
            "PAID" => Colors.Green.Darken1,
            "PARTIALLY PAID" => Colors.Orange.Darken2,
            "OVERDUE" => Colors.Red.Darken1,
            "VOID" => Colors.Grey.Darken1,
            _ => Colors.Orange.Darken2
        };
    }
}