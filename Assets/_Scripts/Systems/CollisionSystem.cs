using Unity.Entities;
using Unity.Collections;
using Unity.Transforms;
using Unity.Mathematics;
using Unity.Jobs;
using Unity.Burst;


public class CollisionSystem : JobComponentSystem
{
    [BurstCompile]
    struct CollisionJob : IJobProcessComponentData<Position, PhysicsObject>
    {
        [DeallocateOnJobCompletion]
        public NativeArray<int> mapArray;
        private int dirX, dirY;


        public void Execute(ref Position position, ref PhysicsObject p)
        {
            dirX = (int)math.sign(p.dx);
            dirY = (int)math.sign(p.dy);

            p.rx += p.dx;
            p.ry += p.dy;

            if (p.rx > 0.5)  { p.rx--; p.cx++; }
            if (p.rx < -0.5) { p.rx++; p.cx--; }
            if (p.ry > 0.5)  { p.ry--; p.cy++; }
            if (p.ry < -0.5) { p.ry++; p.cy--; }

            p.xx = p.cx + p.rx;
            p.yy = p.cy + p.ry;

            position.Value = new float3(p.xx, p.yy, -1f);
        }
    }

    private ComponentGroup mapGroup;


    protected override void OnCreateManager()
    {
        mapGroup = GetComponentGroup(typeof(Map));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var mapEntityArray = mapGroup.ToEntityArray(Allocator.TempJob);
        Map map = EntityManager.GetSharedComponentData<Map>(mapEntityArray[0]);
        var mapArray = new NativeArray<int>(map.mapArray, Allocator.TempJob);

        var job = new CollisionJob
        {
            mapArray = mapArray
        };

        mapEntityArray.Dispose();
        return job.Schedule(this, inputDeps);
    }
}
