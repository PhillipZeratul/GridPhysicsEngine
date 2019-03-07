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
        [DeallocateOnJobCompletion, ReadOnly]
        public NativeArray<PhysicsObject> objArray;


        public void Execute(ref Position position, ref PhysicsObject p)
        {
            p.rx += p.dx;
            p.ry += p.dy;
            p.dx *= 0.96f;
            p.dy *= 0.96f;

            // Repel Collision
            for (int i = 0; i < objArray.Length; i++)           
                // If not itself
                if ((System.Math.Abs(p.xx - objArray[i].xx) > 0.01f) || (System.Math.Abs(p.yy - objArray[i].yy) > 0.01f))                
                    // Fast distance check
                    if (math.abs(p.cx - objArray[i].cx) <= 2 && math.abs(p.cy - objArray[i].cy) <= 2)
                    {
                        // Real distance check
                        var dist = math.sqrt((objArray[i].xx - p.xx) * (objArray[i].xx - p.xx) + (objArray[i].yy - p.yy) * (objArray[i].yy - p.yy));
                        if (dist <= p.radius + objArray[i].radius)
                        {
                            float ang = math.atan2(objArray[i].yy - p.yy, objArray[i].xx - p.xx);
                            float force = 0.02f;
                            float repelPower = (p.radius + objArray[i].radius - dist) / (p.radius + objArray[i].radius);
                            p.dx -= math.cos(ang) * repelPower * force;
                            p.dy -= math.sin(ang) * repelPower * force;
                        }
                    }

            // Wall Collision Detection
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

        private bool IsRepel()
        {


            return true;
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
    private ComponentGroup objGroup;


    protected override void OnCreateManager()
    {
        mapGroup = GetComponentGroup(typeof(Map));
        objGroup = GetComponentGroup(typeof(PhysicsObject));
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        var mapEntityArray = mapGroup.ToEntityArray(Allocator.TempJob);
        Map map = EntityManager.GetSharedComponentData<Map>(mapEntityArray[0]);
        var mapArray = new NativeArray<int>(map.mapArray, Allocator.TempJob);

        var objArray = objGroup.ToComponentDataArray<PhysicsObject>(Allocator.TempJob);

        var job = new CollisionJob
        {
            rows = map.rows,
            cols = map.cols,
            mapArray = mapArray,
            objArray = objArray
        };

        mapEntityArray.Dispose();
        return job.Schedule(this, inputDeps);
    }
}
