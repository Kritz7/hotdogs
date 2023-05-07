using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class DogUtilities
{
    public static T GetRandom<T>(this List<T> list)
    {
        if (list == null || list.Count == 0)
        {
            Debug.LogWarning("List is empty or null.");
            return default(T);
        }

        int randomIndex = Random.Range(0, list.Count);
        return list[randomIndex];
    }

    public static Vector3 Clerp(this AnimationCurve curve, Vector3 start, Vector3 end, float t)
    {
        return Vector3.Lerp(start, end, curve.Evaluate(t));
    }

    public static float Clerp(this AnimationCurve curve, float start, float end, float t)
    {
        return Mathf.Lerp(start, end, curve.Evaluate(t));
    }

    public static Vector3 GetBezierPoint(Vector3 start, Vector3 end, Vector3 controlPoint, float t)
    {
        t = Mathf.Clamp01(t);
        float oneMinusT = 1f - t;
        return oneMinusT * oneMinusT * start + 2f * oneMinusT * t * controlPoint + t * t * end;
    }
}
