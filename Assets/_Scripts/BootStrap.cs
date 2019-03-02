using UnityEngine;
using Unity.Entities;
using Unity.Collections;
using System;

public class BootStrap : MonoBehaviour
{
    public int mapRow;
    public int mapCol;

    private EntityManager entityManager;
    private NativeArray<int> tempMap;


    private void Start()
    {
        entityManager = World.Active.GetOrCreateManager<EntityManager>();
        // Create Map
        CreateMap();
    }

    private void CreateMap()
    {
        EntityArchetype mapArchetype = entityManager.CreateArchetype(
            typeof(Map)
        );

        Entity mapEntity = entityManager.CreateEntity(mapArchetype);
        entityManager.SetSharedComponentData<Map>(mapEntity, new Map
        {
            rows = mapRow,
            cols = mapCol,
            mapArray = RandomizeMap(mapRow, mapCol)
        });

        /// Debug
        LogMap(mapEntity);
    }

    private NativeArray<int> RandomizeMap(int rows, int cols)
    {
        if (rows <= 0 || cols <=0)
        {
            throw new Exception("Map size < 0");
        }

        tempMap = new NativeArray<int>(rows * cols, Allocator.Persistent);
        Unity.Mathematics.Random rnd = new Unity.Mathematics.Random((uint)Guid.NewGuid().GetHashCode());

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                tempMap[i * cols + j] = rnd.NextInt(0, 2);
            }
        }
        return tempMap;
    }

    private void LogMap(Entity mapEntity)
    {
        Map map = entityManager.GetSharedComponentData<Map>(mapEntity);

        for (int i = 0; i < map.rows; i++)
        {
            for (int j = 0; j < map.cols; j++)
            {
                Debug.LogFormat("map[{0}][{1}] = {2}", i, j, map.mapArray[i * map.cols + j]);
            }
        }
    }

    private void OnDestroy()
    {
        tempMap.Dispose();
    }
}
