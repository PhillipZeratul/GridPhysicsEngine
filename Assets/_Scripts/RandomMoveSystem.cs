using Unity.Entities;
using Unity.Jobs;
using Unity.Transforms;
using Unity.Burst;
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;
using System;


public class RandomMoveSystem : JobComponentSystem
{
    Unity.Mathematics.Random rnd;


    [BurstCompile]
    struct RandomMoveJob : IJobProcessComponentData<Position, Obstacle>
    {
        public float dT;
        public Unity.Mathematics.Random rnd;

        public void Execute(ref Position position, [ReadOnly] ref Obstacle obstacle)
        {
            float moveX = rnd.NextFloat(-obstacle.maxRndMoveSpeed, obstacle.maxRndMoveSpeed) * dT;
            float moveY = rnd.NextFloat(-obstacle.maxRndMoveSpeed, obstacle.maxRndMoveSpeed) * dT;
            position.Value = position.Value + new float3(moveX, moveY, 0f);
        }
    }

    protected override JobHandle OnUpdate(JobHandle inputDeps)
    {
        rnd.InitState((uint)Guid.NewGuid().GetHashCode());

        var job = new RandomMoveJob
        {
            dT = Time.deltaTime,
            rnd = rnd
        };

        return job.Schedule(this, inputDeps);
    }
}
