using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using NaughtyAttributes;
using Toolkit.Common.Extensions;
using Toolkit.Resettables;

public class TestBehaviour : MonoBehaviour
{
    // [SerializeField]
    // public List<_Test> listOf;
    // [SerializeField]
    // private SerializedComponentResetter simple;
    // [SerializeField]
    // private List2 list;
    // [SerializeField /*, HideInInspector*/]
    // public SerializedComponentResetter[] array;
    // [SerializeField]
    // public _Test nested;
    
    [Button]
    private void Test()
    {
        
    }
}

[Serializable]
public class SerializableContainer
{
    [SerializeField]
    private SerializedComponentResetter al;
}

[Serializable]
public class SerializableList : IList
{
    public IEnumerator GetEnumerator()
    {
        yield break;
    }

    public void CopyTo(Array array, int index)
    {
    }

    public int Count { get; }
    public bool IsSynchronized { get; }
    public object SyncRoot { get; }

    public int Add(object value)
    {
        return 0;
    }

    public void Clear()
    {
    }

    public bool Contains(object value)
    {
        return false;
    }

    public int IndexOf(object value)
    {
        return 0;
    }

    public void Insert(int index, object value)
    {
    }

    public void Remove(object value)
    {
    }

    public void RemoveAt(int index)
    {
    }

    public bool IsFixedSize { get; }
    public bool IsReadOnly { get; }
    public object this[int index]
    {
        get => null;
        set { }
    }
}