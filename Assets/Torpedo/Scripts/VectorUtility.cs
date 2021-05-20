using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class VectorUtility
{
    public static float distance(Vector2 a1, Vector2 a2)
    {
        return Mathf.Sqrt((a2.x - a1.x) * (a2.x - a1.x) + (a2.y - a1.y) * (a2.y - a1.y));
    }

    public static float magnitude(Vector2 a1, Vector2 a2)
    {
        return Mathf.Sqrt((a2.x - a1.x) * (a2.x - a1.x) + (a2.y - a1.y) * (a2.y - a1.y));
    }

    public static float DotProduct(Vector2 a, Vector2 b)
    {
        return a.x * b.x + a.y * b.y;
    }

    public static void SetMagnitude(ref Vector2 toSet, float newMagnitude, float oldMagnitude)
    {
        toSet.x = toSet.x * newMagnitude / oldMagnitude;
        toSet.y = toSet.y * newMagnitude / oldMagnitude;
    }

    public static void Limit(ref Vector2 toLimit, float minForce, float maxForce)
    {
        float magnitude = toLimit.magnitude;
        if (magnitude > maxForce)
        {
            SetMagnitude(ref toLimit, maxForce, magnitude);
        }
        else if (magnitude < minForce)
        {
            SetMagnitude(ref toLimit, minForce, toLimit.magnitude);
        }
    }
}
