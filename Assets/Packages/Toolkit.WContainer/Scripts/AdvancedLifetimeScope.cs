using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Toolkit.WContainer
{
	public class AdvancedLifetimeScope : LifetimeScope
	{
		[SerializeField]
		protected List<Component> autoInjectComponents;

		protected override void Configure(IContainerBuilder builder)
		{
			builder.RegisterBuildCallback(AutoInjectComponents);
		}

		private void AutoInjectComponents(IObjectResolver container)
		{
			if (autoInjectComponents == null)
				return;

			foreach (Component targetComponent in autoInjectComponents.Where(target => target != null))
				container.Inject(targetComponent);
		}
	}
}