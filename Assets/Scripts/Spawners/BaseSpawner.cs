using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public abstract class BaseSpawner<T> : MonoBehaviour where T : Object
{
    public T Prefab;
    public uint Amount = 1;
    public Transform Parent;

    [SerializeField]
    private SpawnMethod spawnMethod = SpawnMethod.Start;

    private readonly List<T> buffer = new List<T>();
        
    public event Action<List<T>> Spawned;

    private void Start()
    {
        if (spawnMethod == SpawnMethod.Start)
            Spawn();
    }

    private void Update()
    {
        if (spawnMethod == SpawnMethod.Update)
            Spawn();
    }

    public void Spawn()
    {
        buffer.Clear();
            
        for (int i = 0; i < Amount; i++)
            buffer.Add(GetClone());
            
        Spawned?.Invoke(buffer);
    }

    public virtual T GetClone()
    {
        return Instantiate(Prefab, Parent ? Parent : transform);
    }

    public enum SpawnMethod
    {
        Manual,
        Start,
        Update
    }
}