using System;
using System.Collections.Generic;
using System.Linq;
using Toolkit.Resettables.Components;
using UnityEngine;

namespace Toolkit.Resettables
{
    public static class Resettables
    {
        private static IEnumerable<Type> _componentResetterTypes;

        private static IEnumerable<Type> componentResetterTypes =>
            _componentResetterTypes ?? (_componentResetterTypes = GetComponentResetterTypes());

        private static IEnumerable<Type> GetComponentResetterTypes() =>
            from domainAssembly in AppDomain.CurrentDomain.GetAssemblies()
            where !domainAssembly.IsDynamic
            from assemblyType in domainAssembly.GetExportedTypes()
            where assemblyType.BaseType != null && assemblyType.BaseType.IsGenericType &&
                  assemblyType.BaseType.GetGenericTypeDefinition() == typeof(ComponentResetter<>)
            select assemblyType;

        public static IComponentResetter<T> AddComponentResetterToGameObject<T>(GameObject gameObject, T component)
            where T : Component
        {
            Type componentResetterGenericType = typeof(ComponentResetter<>).MakeGenericType(component.GetType());

            try
            {
                Type componentResetterType = componentResetterTypes
                    .First(x => x.IsSubclassOf(componentResetterGenericType));

                return (IComponentResetter<T>) gameObject.AddComponent(componentResetterType);
            }
            catch (InvalidOperationException e)
            {
                Debug.LogWarning(nameof(Resettables) + " package does not have a Resetter for component " +
                                 component.GetType().Name + ". Try to add custom Resetter derived from " +
                                 nameof(ComponentResetter<T>) + "<" + component.GetType().Name + ">");
                throw;
            }
        }

        public static bool HasResetterForComponent(Component component)
        {
            Type componentResetterGenericType = typeof(ComponentResetter<>).MakeGenericType(component.GetType());

            return componentResetterTypes.Any(x => x.IsSubclassOf(componentResetterGenericType));
        }
    }
}