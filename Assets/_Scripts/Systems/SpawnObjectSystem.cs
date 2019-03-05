using Unity.Entities;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Transforms;
using System;


public class SpawnObjectSystem : ComponentSystem
{
    ComponentGroup spawnerGroup;
    ComponentGroup mapGroup;


    protected override void OnCreateManager()
    {
        spawnerGroup = GetComponentGroup(typeof(Spawner), typeof(Initializer));
        mapGroup = GetComponentGroup(typeof(Map));
    }

    protected override void OnUpdate()
    {
        int spawnerLength = spawnerGroup.CalculateLength();
        int mapLength = mapGroup.CalculateLength();

        if (spawnerLength <= 0 || mapLength <= 0 || spawnerLength != mapLength)
            return;

        var spawnerEntities = spawnerGroup.ToEntityArray(Allocator.TempJob);
        var mapEntities = mapGroup.ToEntityArray(Allocator.TempJob);

        var rnd = new Unity.Mathematics.Random((uint)Guid.NewGuid().GetHashCode());
        Position position;
        PhysicsObject physicsObject = new PhysicsObject();

        for (int k = 0; k < spawnerEntities.Length; k++)
        {
            Map map = EntityManager.GetSharedComponentData<Map>(mapEntities[k]);
            Spawner spawner = EntityManager.GetSharedComponentData<Spawner>(spawnerEntities[k]);

            for (int i = 1; i < map.rows - 1; i++)
                for (int j = 1; j < map.cols - 1; j++)
                {
                    if ((i == map.rows / 2) && (j == map.cols / 2))
                    {
                        var entity = EntityManager.Instantiate(spawner.playerPrefab);
                        physicsObject.cx = i;
                        physicsObject.cy = j;
                        EntityManager.SetComponentData(entity, physicsObject);
                    }
                    else if (map.mapArray[i * map.cols + j] == 0)
                    {
                        if (rnd.NextFloat(0f, 1f) > 0.3f)
                        {
                            var entity = EntityManager.Instantiate(spawner.obstaclePrefab);
                            physicsObject.cx = i;
                            physicsObject.cy = j;
                            EntityManager.SetComponentData(entity, physicsObject);
                        }
                    }
                }

            EntityManager.RemoveComponent<Initializer>(spawnerEntities[k]);
        }

        spawnerEntities.Dispose();
        mapEntities.Dispose();
    }
}
