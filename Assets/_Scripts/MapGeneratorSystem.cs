using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;
using Unity.Mathematics;
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
                //map = WhiteRndInitMap(map);
                map = WorleyInitMap(map);
                //LogMap(map);
                EntityManager.SetSharedComponentData<Map>(generator, map);
                EntityManager.RemoveComponent<Initializer>(generator);
                SpawnMap(map);
            }
        }
    }

    // Map generation with white noise
    private Map WhiteRndInitMap(Map map)
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
        FillOutsideWall(map.mapArray, map.rows, map.cols);
        return map;
    }

    // Map generation with Worley noise
    private Map WorleyInitMap(Map map)
    {
        if (map.rows <= 0 || map.cols <= 0)
        {
            throw new Exception("Map size < 0");
        }
        // rows and cols should be odd numbers
        map.rows = (map.rows % 2) == 0 ? map.rows + 1 : map.rows;
        map.cols = (map.cols % 2) == 0 ? map.cols + 1 : map.cols;

        map.mapArray = new int[map.rows * map.cols];
        var rnd = new Unity.Mathematics.Random((uint)Guid.NewGuid().GetHashCode());
        float2 start = new float2(rnd.NextFloat2());

        for (int i = 0; i < map.rows; i++)
            for (int j = 0; j < map.cols; j++)
            {
                float value = noise.cellular(start + new float2(i, j)).y;
                //Debug.LogFormat("noise[{0}][{1}] = {2}", i, j, value);
                map.mapArray[i * map.cols + j] = value > 0.77 ? 1 : 0;
            }
        FillOutsideWall(map.mapArray, map.rows, map.cols);
        GeneratePlayerSpawnPoint(map.mapArray, map.rows, map.cols);
        return map;
    }

    private void FillOutsideWall(int[] mapArray, int rows, int cols)
    {
        for (int i = 0; i < rows; i++)
        {
            mapArray[i * cols] = 1;
            mapArray[i * cols + cols - 1] = 1;
        }

        for (int j = 0; j < cols; j++)
        {
            mapArray[j] = 1;
            mapArray[rows * (cols - 1) + j] = 1;
        }
    }

    // Make a 3x3 hole at the middle of the map
    private void GeneratePlayerSpawnPoint(int[] mapArray, int rows, int cols)
    {
        int x = rows / 2 * cols;
        int y = cols / 2;
        mapArray[x + y] = 0;
        mapArray[x + y + 1] = 0;
        mapArray[x + y - 1] = 0;
        mapArray[x + cols + y] = 0;
        mapArray[x + cols + y + 1] = 0;
        mapArray[x + cols + y - 1] = 0;
        mapArray[x - cols + y] = 0;
        mapArray[x - cols + y + 1] = 0;
        mapArray[x - cols + y - 1] = 0;
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
                    position.Value = new float3(i, j, 0f);
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
