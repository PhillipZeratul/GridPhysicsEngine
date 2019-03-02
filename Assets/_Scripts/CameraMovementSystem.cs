using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;


public class CameraMovementSystem : ComponentSystem
{
    ComponentGroup cameraGroup;
    ComponentGroup mapGroup;


    protected override void OnCreateManager()
    {
        cameraGroup = GetComponentGroup(typeof(Camera), typeof(Initializer));
        mapGroup = GetComponentGroup(typeof(Map));
    }

    protected override void OnUpdate()
    {
        int cameraGroupLength = cameraGroup.CalculateLength();
        int mapGroupLength = mapGroup.CalculateLength();
        if ((cameraGroupLength == 0) ||
            (cameraGroupLength != mapGroupLength))
        {
            return;
        }

        var cameraEntities = cameraGroup.ToEntityArray(Allocator.TempJob);
        var mapEntities = mapGroup.ToEntityArray(Allocator.TempJob);

        Position position;
        for (int i = 0; i < cameraEntities.Length; i++)
        {
            Map map = EntityManager.GetSharedComponentData<Map>(mapEntities[i]);
            position.Value = new float3(map.rows / 2, map.cols / 2, -10f);
            EntityManager.SetComponentData<Position>(cameraEntities[i], position);
            EntityManager.RemoveComponent<Initializer>(cameraEntities[i]);
        }

        cameraEntities.Dispose();
        mapEntities.Dispose();
    }
}
