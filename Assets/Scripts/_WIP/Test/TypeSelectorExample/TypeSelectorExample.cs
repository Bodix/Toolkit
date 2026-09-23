using System;
using PerfectCore.PerfectFoundation;
using UnityEngine;

// ReSharper disable ArrangeMethodOrOperatorBody

// ReSharper disable CheckNamespace

public interface IEffect
{
	void Apply(Health target);
}

[Serializable]
public class DamageEffect : IEffect
{
	[SerializeField] private int _amount = 10;

	public void Apply(Health target) => target.TakeDamage(_amount);
}

[Serializable]
public class HealEffect : IEffect
{
	[SerializeField] private int _amount = 10;

	public void Apply(Health target) => target.Heal(_amount);
}

[Serializable, TypeSelectorName("Heal Effect (% of max)")]
public class PercentHealEffect : IEffect
{
	[SerializeField, Range(0, 1)] private float _percent = 0.25f;

	public void Apply(Health target) =>
		target.Heal(Mathf.RoundToInt(target.Max * _percent));
}