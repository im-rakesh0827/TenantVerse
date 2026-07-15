using TenantVerse.UI.Models.Property;

namespace TenantVerse.UI.Services;

public class PropertyState
{
    public List<PropertyDto> Properties { get; private set; } = new();
    public PropertyDto? SelectedProperty { get; set; }
    public int _PropertyId{get; set;}
    public bool _IsEditMode{get; set;} = false;

    public bool IsLoaded => Properties.Any();

    public void Set(List<PropertyDto> properties)
    {
        Properties = properties;
    }

    public void Set(PropertyDto property){
        SelectedProperty = property;
    }

    public void Clear()
    {
        Properties.Clear();
    }
}