using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Elephanet
{
    internal static class IdentityFactory
    {
        static readonly ConcurrentDictionary<Type, PropertyInfo> _keyCache = new ConcurrentDictionary<Type, PropertyInfo>();

        public static Guid SetEntityId(object value)
        {
            PropertyInfo propertyInfo;
            if (TryGetIdProperty(value, out propertyInfo) == false)
            {
                throw new EntityException("Entity does not have an ID property of type Guid.");
            }

            try
            {
                var id = (Guid)propertyInfo.GetValue(value, null);
                if (id != Guid.Empty)
                {
                    return id;
                }

                id = Guid.NewGuid();

                propertyInfo.SetValue(value, id, null);
                return id;
            }
            catch (Exception exception)
            {
                throw new EntityException("Could not get or set the ID property of the entity.", exception);
            }
        }

        public static Guid GetEntityId(object value)
        {
            PropertyInfo propertyInfo;
            if (TryGetIdProperty(value, out propertyInfo) == false)
            {
                throw new EntityException("Entity does not have an ID property of type Guid.");
            }

            try
            {
                return (Guid)propertyInfo.GetValue(value, null);
            }
            catch (Exception exception)
            {
                throw new EntityException("Could not get the ID property of the entity.", exception);
            }
        }

        static bool TryGetIdProperty(object value, out PropertyInfo propertyInfo)
        {
            var type = value.GetType();

            if (_keyCache.ContainsKey(type))
            {
                propertyInfo = _keyCache[type];
            }
            else
            {
                propertyInfo = value.GetType().GetProperties(BindingFlags.Public | BindingFlags.Instance)
                                   .Where(x => x.Name.Equals("id", StringComparison.OrdinalIgnoreCase))
                                   .Where(x => x.PropertyType == typeof(Guid))
                                   .FirstOrDefault(x => x.CanRead && x.CanWrite);

                _keyCache[type] = propertyInfo;
            }

            return (propertyInfo != null);
        }
    }
}
