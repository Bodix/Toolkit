using System;
using NaughtyAttributes;
using UnityEngine;

namespace Obsolete.Components
{
	[Obsolete("Used for versions before 2022.1 to move GameObjects up in hierarchy in prefab variants")]
	public class SetSiblingIndex : MonoBehaviour
	{
		[SerializeField]
		private int _siblingIndex = 0;

		[Button]
		private void Set()
		{
			transform.SetSiblingIndex(_siblingIndex);
		}
	}
}