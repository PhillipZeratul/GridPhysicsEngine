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
        public int rows, cols;
        [DeallocateOnJobCompletion, ReadOnly]
        public NativeArray<int> mapArray;
        private int dirX, dirY;


        public void Execute(ref Position position, ref PhysicsObject p)
        {
            dirX = (int)math.sign(p.dx);
            dirY = (int)math.sign(p.dy);

            p.rx += p.dx;
            p.ry += p.dy;
            p.dx *= 0.96f;
            p.dy *= 0.96f;

            // Collision Detection
            if (p.rx >= 0.2f && IsWall(p.cx + 1, p.cy))
            {
                p.rx = 0.2f;
                p.dx = 0; // stop movement
            }
            if (p.rx <= -0.2f && IsWall(p.cx - 1, p.cy))
            {
                p.rx = -0.2f;
                p.dx = 0;
            }
            if (p.ry >= 0.2f && IsWall(p.cx, p.cy + 1))
            {
                p.ry = 0.2f;
                p.dy = 0;
            }
            if (p.ry <= -0.2f && IsWall(p.cx, p.cy - 1))
            {
                p.ry = -0.2f;
                p.dy = 0;
            }

            if (p.rx > 0.5f) { p.rx--; p.cx++; }
            if (p.rx < -0.5f) { p.rx++; p.cx--; }
            if (p.ry > 0.5f) { p.ry--; p.cy++; }
            if (p.ry < -0.5f) { p.ry++; p.cy--; }

            p.xx = p.cx + p.rx;
            p.yy = p.cy + p.ry;

            position.Value = new float3(p.xx, p.yy, -1f);
        }

        // x: col (left -> right), y: row (down -> up)
        private bool IsWall(int x, int y)
        {
            if (x < 0 || x > cols || y < 0 || y > rows)
                throw new System.Exception("Position outside wall bound!");


            if (mapArray[y * cols + x] == 1)
                return true;
            else
                return false;
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
            rows = map.rows,
            cols = map.cols,
            mapArray = mapArray
        };

        mapEntityArray.Dispose();
        return job.Schedule(this, inputDeps);
    }
}
