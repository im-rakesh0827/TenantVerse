using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TenantVerse.UI.Models.Property;
namespace TenantVerse.UI.Services;
public class StateContainer
{
    public PropertyStateContainer Property { get; } = new();
}

public class PropertyStateContainer
{
    public List<PropertyDto> Properties { get; private set; } = new();
    public PropertyDto? SelectedProperty { get; private set; }
    public int PropertyId { get; private set; }
    public bool IsEditMode { get; private set; }

    public bool IsLoaded => Properties.Any();

    public void SetProperties(List<PropertyDto> properties)
    {
        Properties = properties;
    }

    public void SetSelectedProperty(PropertyDto property)
    {
        SelectedProperty = property;
    }

    public void SetPropertyId(int propertyId)
    {
        PropertyId = propertyId;
    }

    public void SetEditMode(bool isEditMode)
    {
        IsEditMode = isEditMode;
    }

    public void Clear()
    {
        Properties.Clear();
        SelectedProperty = null;
        PropertyId = 0;
        IsEditMode = false;
    }
}