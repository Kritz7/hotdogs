using System;
using System.Collections;
using UnityEngine;

public static class CoroutineHelper
{
    public static void PerformAfterDuration(this MonoBehaviour monoBehaviour, Action callback, float duration)
    {
        monoBehaviour.StartCoroutine(PerformAfterDurationIE(duration, callback));
    }

    public static IEnumerator PerformAfterDurationIE(float duration, Action callback)
    {
        yield return new WaitForSeconds(duration);
        callback?.Invoke();
    }

    public static void PerformAfterFrames(this MonoBehaviour monoBehaviour, int frameCount, Action callback)
    {
        monoBehaviour.StartCoroutine(PerformAfterFramesIE(frameCount, callback));
    }

    public static IEnumerator PerformAfterFramesIE(int frameCount, Action callback)
    {
        for (int i = 0; i < frameCount; i++)
        {
            yield return null;
        }

        callback?.Invoke();
    }

    public static void PerformAfterCondition(this MonoBehaviour monoBehaviour, Action callback, Func<bool> condition)
    {
        monoBehaviour.StartCoroutine(PerformAfterConditionIE(condition, callback));
    }

    public static IEnumerator PerformAfterConditionIE(Func<bool> condition, Action callback)
    {
        yield return new WaitUntil(condition);
        callback?.Invoke();
    }
}
