using System;
using System.Linq;
using System.Reflection;

namespace TheGarden.Core;

public static class ReflectionHelper
{
    public static PropertyInfo? AsProperty(this string propName, Type type)
    {
        ArgumentNullException.ThrowIfNull(propName, nameof(propName));
        ArgumentNullException.ThrowIfNull(type, nameof(type));
        propName = propName.ToLower();
        
        var selectedType = 
            from prop in type.GetProperties()
            let name = prop.Name.ToLower()
            let cost = LevenshteinDistance(name, propName)
            where cost < 3
            orderby cost
            select prop;
        
        return selectedType.FirstOrDefault();
    }
    
    public static MethodInfo? AsMethod(this string methodName, Type type)
    {
        ArgumentNullException.ThrowIfNull(methodName, nameof(methodName));
        ArgumentNullException.ThrowIfNull(type, nameof(type));
        methodName = methodName.ToLower();
        
        var selectedType = 
            from method in type.GetMethods()
            let name = method.Name.ToLower()
            let cost = LevenshteinDistance(name, methodName)
            where cost < 3
            orderby cost
            select method;
        
        return selectedType.FirstOrDefault();
    }

    public static Type AsType(this string typeName)
    {
        ArgumentNullException.ThrowIfNull(typeName, nameof(typeName));
        typeName = typeName.ToLower();

        var assembly = Assembly.GetEntryAssembly()
            ?? throw new Exception($"Unknow type '{typeName}'.");
        
        var types = assembly.GetTypes();
        var selectedType = 
            from type in types
            let name = type.Name.ToLower()
            orderby LevenshteinDistance(name, typeName)
            select type;
        
        return selectedType.FirstOrDefault()
            ?? throw new Exception($"Unknow type '{typeName}'.");
    }

    static int LevenshteinDistance(string str1, string str2)
    {
        var n = str1.Length + 1;
        var m = str2.Length + 1;
        var dp = new int[n, m];

        for (int i = 0; i < n; i++)
            dp[i, 0] = i;

        for (int j = 0; j < m; j++)
            dp[0, j] = j;

        for (int i = 1; i < n; i++)
        {
            for (int j = 1; j < m; j++)
            {
                var removeCost = dp[i - 1, j] + 1;
                var insertCost = dp[i, j - 1] + 1;
                var substuCost = dp[i - 1, j - 1] + (str1[i - 1] == str2[j - 1] ? 0 : 1);

                dp[i, j] = int.Min(
                    int.Min(removeCost, insertCost),
                    substuCost
                );
            }
        }

        return dp[n - 1, m - 1];
    }
}