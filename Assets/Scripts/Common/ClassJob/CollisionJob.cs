using UnityEngine.Jobs;
using Unity.Collections;
using Unity.Mathematics;
using Unity.Burst;

[BurstCompile]
public struct CollisionJobMultiRadius : IJobParallelForTransform
{
    [ReadOnly] public float deltaTime;
    [ReadOnly] public NativeArray<float> radiusArray;
    [ReadOnly] public NativeArray<float3> collisionObjects;

    public void Execute(int index, TransformAccess transform)
    {
        float3 position = transform.position;

        for (int i = 0; i < collisionObjects.Length; i++)
        {
            if (index == i) continue;

            float3 dir2CheckCol = position - collisionObjects[i];
            float distance = math.length(dir2CheckCol);
            float contactDist = radiusArray[i] + radiusArray[index];

            if (distance < contactDist)
            {
                float3 contactDir = collisionObjects[i] + math.normalize(dir2CheckCol) * contactDist;
                position = math.lerp(position, contactDir, deltaTime * 2f);
            }
        }

        transform.position = position;
    }
}

[BurstCompile]
public struct CollisionJobSameRadius : IJobParallelForTransform
{
    [ReadOnly] public float deltaTime;
    [ReadOnly] public float contactDist;
    [ReadOnly] public NativeArray<float3> collisionObjects;

    public void Execute(int index, TransformAccess transform)
    {
        float3 position = transform.position;

        for (int i = 0; i < collisionObjects.Length; i++)
        {
            if (index == i) continue;

            float3 dir2CheckCol = position - collisionObjects[i];
            float distance = math.length(dir2CheckCol);

            if (distance < contactDist)
            {
                float3 contactDir = collisionObjects[i] + math.normalize(dir2CheckCol) * contactDist;
                position = math.lerp(position, contactDir, deltaTime);
            }
        }

        transform.position = position;
    }
}

