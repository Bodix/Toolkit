using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Toolkit.WContainer
{
	[DefaultExecutionOrder(-1000)]
	public class AutoInjector : MonoBehaviour
	{
		[SerializeField]
		private InjectionMethod _injectionMethod;
		[SerializeField]
		private ParentReference _targetLifetimeScope;

		private void Awake()
		{
			Inject();
		}

		private void Inject()
		{
			if (_targetLifetimeScope.Object != null)
			{
				InjectWithSelectedMethod(_targetLifetimeScope.Object.Container);
			}
			else
			{
				LifetimeScope scope = LifetimeScope.Find<LifetimeScope>();
				if (scope != null)
					InjectWithSelectedMethod(scope.Container);
				else
					Debug.LogError(nameof(LifetimeScope) + " not found");
			}
		}

		private void InjectWithSelectedMethod(IObjectResolver container)
		{
			switch (_injectionMethod)
			{
				default:
				case InjectionMethod.GameObject:
					container.InjectGameObject(gameObject);
					break;
				case InjectionMethod.GameObjectWithoutChildren:
					container.InjectGameObjectWithoutChildren(gameObject);
					break;
			}
		}

		private enum InjectionMethod
		{
			GameObject,
			GameObjectWithoutChildren
		}
	}
}