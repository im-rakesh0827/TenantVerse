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
                                .Text("Rakesh Kumar");

                            left.Item()
                                .Text("Darbhanga, Bihar");
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
}