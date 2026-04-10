using Evolutex.Evolunity.Patterns;
using UnityEngine;
using VContainer;

namespace Toolkit.WContainer
{
	public static class VContainerExtensions
	{
		public static void InjectGameObjectWithoutChildren(this IObjectResolver objectResolver, GameObject gameObject)
		{
			// TODO: Optimize it with cached list and GetComponents method overload. [#optimization]
			foreach (Component component in gameObject.GetComponents<Component>())
				objectResolver.Inject(component);
		}

		public static void RegisterInstanceOptional<T>(this IContainerBuilder containerBuilder, T instance)
		{
			containerBuilder.RegisterInstance(new Optional<T>(instance));
		}
	}
}