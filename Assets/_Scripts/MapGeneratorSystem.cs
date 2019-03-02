using Unity.Collections;
using Unity.Entities;
using UnityEngine;
using System;


public class MapGeneratorSystem : ComponentSystem
{
    ComponentGroup mapGeneratorGroup;


    protected override void OnCreateManager()
    {
        mapGeneratorGroup = GetComponentGroup(typeof(Map), typeof(Initializer));
    }

    protected override void OnUpdate()
    {
        using (var mapGeneratorEntities = mapGeneratorGroup.ToEntityArray(Allocator.TempJob))
        {
            foreach (var generator in mapGeneratorEntities)
            {
                var map = EntityManager.GetSharedComponentData<Map>(generator);
                map = RndInitMap(map);
                LogMap(map);
                EntityManager.SetSharedComponentData<Map>(generator, map);
                EntityManager.RemoveComponent<Initializer>(generator);
            }
        }
    }

    private Map RndInitMap(Map map)
    {
        int rows = map.rows;
        int cols = map.cols;

        if (rows <= 0 || cols <= 0)
        {
            throw new Exception("Map size < 0");
        }

        // They say NativeArray will be faster in builds though slow in editor than array, so I'll just keep it.
        // https://forum.unity.com/threads/native-arrays-approximately-an-order-of-magnitude-slower-than-arrays.535019/#post-3526924
        var mapArray = new NativeArray<int>(rows * cols, Allocator.TempJob);
        Unity.Mathematics.Random rnd = new Unity.Mathematics.Random((uint)Guid.NewGuid().GetHashCode());

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                mapArray[i * cols + j] = rnd.NextInt(0, 2);
            }
        }

        map.mapArray = mapArray.ToArray();
        mapArray.Dispose();
        return map;
    }

    private void LogMap(Map map)
    {
        for (int i = 0; i < map.rows; i++)
        {
            for (int j = 0; j < map.cols; j++)
            {
                Debug.LogFormat("map[{0}][{1}] = {2}", i, j, map.mapArray[i * map.cols + j]);
            }
        }
    }
}
