/*
 * Copyright (C) 2020 by Evolutex - All Rights Reserved
 * Unauthorized copying of this file, via any medium is strictly prohibited
 * Proprietary and confidential
 * Written by Bogdan Nikolaev <bodix321@gmail.com>
*/

using UnityEngine;

public class Spawner : MonoBehaviour
{
    public GameObject Prefab;
    public uint Amount = 1;
    public Transform Parent;

    [SerializeField]
    private SpawnMethod spawnMethod = SpawnMethod.Start;

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

    public virtual void Spawn()
    {
        for (int i = 0; i < Amount; i++)
            Instantiate(Prefab, Parent ? Parent : transform);
    }

    public enum SpawnMethod
    {
        Manual,
        Start,
        Update
    }
}