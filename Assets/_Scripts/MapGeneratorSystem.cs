using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
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
                //LogMap(map);
                EntityManager.SetSharedComponentData<Map>(generator, map);
                EntityManager.RemoveComponent<Initializer>(generator);
                SpawnMap(map);
            }
        }
    }

    private Map RndInitMap(Map map)
    {
        if (map.rows <= 0 || map.cols <= 0)
        {
            throw new Exception("Map size < 0");
        }
        // rows and cols should be odd numbers
        map.rows = (map.rows % 2) == 0 ? map.rows + 1 : map.rows;
        map.cols = (map.cols % 2) == 0 ? map.cols + 1 : map.cols;

        map.mapArray = new int[map.rows * map.cols];
        Unity.Mathematics.Random rnd = new Unity.Mathematics.Random((uint)Guid.NewGuid().GetHashCode());

        for (int i = 0; i < map.rows; i++)
            for (int j = 0; j < map.cols; j++)
                map.mapArray[i * map.cols + j] = rnd.NextInt(0, 2);

        return map;
    }

    private void SpawnMap(Map map)
    {
        // Create an entity from the prefab set on the spawner component.
        var prefab = map.wallPrefab;
        Position position;

        for (int i = 0; i < map.rows; i++)
        {
            for (int j = 0; j < map.cols; j++)
            {
                if (map.mapArray[i * map.cols + j] == 1)
                {
                    var entity = EntityManager.Instantiate(prefab);
                    position.Value = new Unity.Mathematics.float3(i, j, 0f);
                    EntityManager.SetComponentData(entity, position);
                }
            }
        }

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
