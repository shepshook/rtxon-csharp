using System.Dynamic;

namespace RtxOn.Console.Extensions;

public static class DynamicExtensions
{
    public static bool DoesPropertyExist(dynamic entity, string name)
    {
        if (entity is ExpandoObject)
        { 
            return ((IDictionary<string, object>)entity).ContainsKey(name); 
        }

        return entity.GetType().GetProperty(name) != null;
    }
}
