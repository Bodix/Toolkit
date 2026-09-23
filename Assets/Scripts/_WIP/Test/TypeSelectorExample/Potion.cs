using PerfectCore.PerfectFoundation;
using UnityEngine;

// ReSharper disable once CheckNamespace

public class Potion : MonoBehaviour
{
	[SerializeReference, TypeSelector]
	private IEffect _effect;

	private void OnTriggerEnter(Collider other)
	{
		if (other.TryGetComponent(out Health health))
			_effect.Apply(health);
	}
}