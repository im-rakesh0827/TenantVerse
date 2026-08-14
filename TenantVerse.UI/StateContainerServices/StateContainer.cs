using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantVerse.UI.Models.Property;
using TenantVerse.Shared.Models.Unit;
using TenantVerse.Shared.Models;
namespace TenantVerse.UI.Services;
public class StateContainer
{
    public PropertyStateContainer Property { get; } = new();
    public UnitStateContainer Unit { get; } = new();
}

public class PropertyStateContainer
{
    public List<PropertyDto> Properties { get; private set; } = new();
    public PropertyDto? SelectedProperty { get; private set; }
    public int PropertyId { get; private set; }
    public bool IsLoaded { get; private set; }

    public void SetProperties(List<PropertyDto> properties)
    {
        Properties = properties;
        IsLoaded=true;
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
}

public class UnitStateContainer
{
    public List<UnitModel> Units { get; private set; } = new();
    public UnitModel? SelectedUnit { get; private set; }
    public int UnitId { get; private set; }
    public int PropertyId { get; private set; }
    public bool IsLoaded { get; private set; }

    public void SetUnits(List<UnitModel> units)
    {
        Units = units;
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
}