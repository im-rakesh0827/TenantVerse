using TenantVerse.Shared.Models.Tenant;
using TenantVerse.Shared.Models.Unit;
using TenantVerse.Shared.Models.Invoice;

using TenantVerse.Shared.Models.Property;
using TenantVerse.UI.Services;

namespace TenantVerse.UI.Services;

public class StateContainer
{
    public PropertyStateContainer Property { get; }

    public UnitStateContainer Unit { get; }

    public TenantStateContainer Tenant { get; }

    public InvoiceStateContainer Invoice { get; }


    public StateContainer(
        PropertyService propertyService,
        UnitService unitService,
        TenantService tenantService,
        InvoiceService invoiceService)
    {
        Property = new PropertyStateContainer(propertyService);
        Unit = new UnitStateContainer(unitService);
        Tenant = new TenantStateContainer(tenantService);
        Invoice = new InvoiceStateContainer(invoiceService);
    }
}


// ============================================================
// PROPERTY STATE
// ============================================================

public class PropertyStateContainer
{
    private readonly PropertyService _propertyService;


    public PropertyStateContainer(PropertyService propertyService)
    {
        _propertyService = propertyService;
    }


    public List<PropertyDto> Properties { get; private set; } = new();

    public PropertyDto? SelectedProperty { get; private set; }

    public int PropertyId { get; private set; }

    public bool IsLoaded { get; private set; }


    public void SetProperties(List<PropertyDto> properties)
    {
        Properties = properties ?? new List<PropertyDto>();

        IsLoaded = true;
    }


    public void SetSelectedProperty(PropertyDto property)
    {
        SelectedProperty = property;
    }


    public void SetPropertyId(int propertyId)
    {
        PropertyId = propertyId;
    }


    public void ResetLoaded()
    {
        IsLoaded = false;
    }


    public void Clear()
    {
        Properties.Clear();

        SelectedProperty = null;

        PropertyId = 0;

        IsLoaded = false;
    }


    public async Task<bool> RefreshAsync()
    {
        var response = await _propertyService.GetAllAsync();

        if (response is null)
        {
            return false;
        }

        SetProperties(response);

        return true;
    }
}


// ============================================================
// UNIT STATE
// ============================================================

public class UnitStateContainer
{
    private readonly UnitService _unitService;


    public UnitStateContainer(UnitService unitService)
    {
        _unitService = unitService;
    }


    public List<UnitModel> Units { get; private set; } = new();

    public UnitModel? SelectedUnit { get; private set; }

    public int UnitId { get; private set; }

    public int PropertyId { get; private set; }

    public bool IsLoaded { get; private set; }


    public void SetUnits(List<UnitModel> units)
    {
        Units = units ?? new List<UnitModel>();

        IsLoaded = true;
    }


    public void SetSelectedUnit(UnitModel unit)
    {
        SelectedUnit = unit;
    }


    public void SetUnitId(int unitId)
    {
        UnitId = unitId;
    }


    public void SetPropertyId(int propertyId)
    {
        PropertyId = propertyId;
    }


    public void ResetLoaded()
    {
        IsLoaded = false;
    }


    public void Clear()
    {
        Units.Clear();

        SelectedUnit = null;

        UnitId = 0;

        PropertyId = 0;

        IsLoaded = false;
    }


    public async Task<bool> RefreshAsync()
    {
        var response = await _unitService.GetAllAsync();

        if (response is null ||
            !response.IsSuccess ||
            response.Data is null)
        {
            return false;
        }

        SetUnits(response.Data.ToList());

        return true;
    }
}


// ============================================================
// TENANT STATE
// ============================================================

public class TenantStateContainer
{
    private readonly TenantService _tenantService;


    public TenantStateContainer(TenantService tenantService)
    {
        _tenantService = tenantService;
    }


    public List<TenantModel> Tenants { get; private set; } = new();

    public TenantModel? SelectedTenant { get; private set; }

    public int TenantId { get; private set; }

    public bool IsLoaded { get; private set; }


    public void SetTenants(List<TenantModel> tenants)
    {
        Tenants = tenants ?? new List<TenantModel>();

        IsLoaded = true;
    }


    public void SetSelectedTenant(TenantModel tenant)
    {
        SelectedTenant = tenant;
    }


    public void SetTenantId(int tenantId)
    {
        TenantId = tenantId;
    }


    public void ResetLoaded()
    {
        IsLoaded = false;
    }


    public void Clear()
    {
        Tenants.Clear();

        SelectedTenant = null;

        TenantId = 0;

        IsLoaded = false;
    }


    public async Task<bool> RefreshAsync()
    {
        var response = await _tenantService.GetAllAsync();

        if (response is null ||
            !response.IsSuccess ||
            response.Data is null)
        {
            return false;
        }

        SetTenants(response.Data);

        return true;
    }



    // public TenantModel? GetTenant(int tenantId)
    // {
        
    //     return Tenants.FirstOrDefault(
    //         x => x.TenantId == tenantId);
    // }


    public async Task<TenantModel?> GetTenantByIdAsync(int tenantId)
    {
        if (!IsLoaded)
        {
            var response =
            await _tenantService.GetByIdAsync(tenantId);
            if (response is null ||
                !response.IsSuccess ||
                response.Data is null)
            {
                return null;
            }
            return response.Data;  
        }
        return Tenants.FirstOrDefault(x => x.TenantId == tenantId);
    }
}



// ============================================================
// INVOICE STATE
// ============================================================

public class InvoiceStateContainer
{
    private readonly InvoiceService _invoiceService;


    public InvoiceStateContainer(
        InvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
    }


    public List<InvoiceModel> Invoices { get; private set; } = new();

    public InvoiceModel? SelectedInvoice { get; private set; }

    public int InvoiceId { get; private set; }

    public bool IsLoaded { get; private set; }


    public void SetInvoices(
        List<InvoiceModel> invoices)
    {
        Invoices = invoices ?? new List<InvoiceModel>();

        IsLoaded = true;
    }


    public void SetSelectedInvoice(
        InvoiceModel invoice)
    {
        SelectedInvoice = invoice;
    }


    public void SetInvoiceId(
        int invoiceId)
    {
        InvoiceId = invoiceId;
    }


    public void ResetLoaded()
    {
        IsLoaded = false;
    }


    public void Clear()
    {
        Invoices.Clear();

        SelectedInvoice = null;

        InvoiceId = 0;

        IsLoaded = false;
    }


    public async Task<bool> RefreshAsync()
    {
        var response =
            await _invoiceService.GetAllAsync();

        if (response is null ||
            !response.IsSuccess ||
            response.Data is null)
        {
            return false;
        }

        SetInvoices(response.Data);

        return true;
    }
}