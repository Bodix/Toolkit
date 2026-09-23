using UnityEngine;

public class Health : MonoBehaviour
{
	[SerializeField] private int _max = 100;

	public int Max => _max;
	public int Current { get; private set; }

	private void Awake() => Current = _max;
	public void TakeDamage(int amount) => Current = Mathf.Max(Current - amount, 0);
	public void Heal(int amount) => Current = Mathf.Min(Current + amount, _max);
}