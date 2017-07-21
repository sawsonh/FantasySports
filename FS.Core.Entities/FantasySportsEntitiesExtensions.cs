using System;
using System.Linq;

namespace FS.Core.Entities
{
    public static class FantasySportsEntitiesExtensions
    {
        public static bool CompareTo<T>(this object test1, T test2, params string[] ignores)
        {
            foreach (var property in test1.GetType().GetProperties())
            {
                if (ignores.Any(ignore => ignore.Equals(property.Name, StringComparison.OrdinalIgnoreCase)))
                    continue;
                if (property.PropertyType.Namespace.Equals("System.Collections.Generic")
                    || property.PropertyType.Namespace.Equals("FS.Core.Entities"))
                    continue;
                object value1 = property.GetValue(test1, null);
                object value2 = property.GetValue(test2, null);
                if (property.PropertyType == typeof(string))
                {
                    if (Convert.ToString(value1).Equals(Convert.ToString(value2)))
                        continue;
                }
                if (value1 == null)
                    if (value2 != null)
                        return false;
                    else
                        continue;
                if (!value1.Equals(value2))
                {
                    if (property.PropertyType == typeof(DateTime) || property.PropertyType == typeof(DateTime?))
                    {
                        if (DateTime.Compare(Convert.ToDateTime(value1), Convert.ToDateTime(value2)) != 0)
                            return false;
                    }
                    else
                        return false;
                }
            }
            return true;
        }

        public static void CopyTo<T>(this object test1, T test2, params string[] ignores)
        {
            foreach (var property in test1.GetType().GetProperties())
            {
                if (ignores.Any(ignore => ignore.Equals(property.Name, StringComparison.OrdinalIgnoreCase)))
                    continue;
                object value1 = property.GetValue(test1, null);
                property.SetValue(test2, value1);
            }
        }

    }
}
