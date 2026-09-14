using System;
using PerfectCore;
using UnityEngine;

namespace _WIP.Test
{
	[Serializable]
	public class _TestClass
	{
		[SerializeField]
		private int _someInt;
		[SerializeReference, TypeSelector]
		private _NestedTestClass _someObj;

		[Serializable]
		public class _NestedTestClass : _TestClass
		{
		}
	}
}