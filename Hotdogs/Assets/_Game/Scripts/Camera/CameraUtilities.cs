using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System.Linq;

public static class CameraUtilities
{
    private static LayerMask inputIgnoreMask => ~(1 << LayerMask.NameToLayer("Player") | 1 << LayerMask.NameToLayer("PlayerHands"));
    private static bool debugLines = false;

    public static Vector3 GetMouseWorldPosition()
    {
        Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

        if (Physics.Raycast(ray, out RaycastHit hit, inputIgnoreMask))
        {
            return hit.point;
        }

        return Vector3.zero;
    }

    public static List<RaycastResult> CanvasRaycast()
    {
        List<RaycastResult> results = new List<RaycastResult>();

        if (EventSystem.current == null)
            return results;

        PointerEventData pointerData = new PointerEventData(EventSystem.current);
        pointerData.position = Input.mousePosition;
        EventSystem.current.RaycastAll(pointerData, results);

        return results;
    }

    public static List<RaycastHit> SteppedSpherecast(Vector3 origin, Vector3 direction, float distance, float width = 0.05f, int steps = 1)
    {
        return SteppedSphereRaycast(origin, direction, distance, steps, width);
    }

    public static List<RaycastHit> SteppedSpherecastFromScreenCentre(float distance, float width = 0.05f, int steps = 1)
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f));
        return SteppedSphereRaycast(ray.origin, ray.direction, distance, steps, width);
    }

    public static List<RaycastHit> SteppedSpherecastFromScreen(float distance, float hozScreenOffsetPercent = 0.5f, float width = 0.05f, int steps = 1)
    {
        float hozOffset = Mathf.Lerp(0f, Screen.width, hozScreenOffsetPercent);

        Ray ray = Camera.main.ScreenPointToRay(new Vector3(hozOffset, Screen.height * 0.5f));
        return SteppedSphereRaycast(ray.origin, ray.direction, distance, steps, width);
    }

    private static List<RaycastHit> SteppedSphereRaycast(Vector3 origin, Vector3 direction, float distance, int steps, float sphereRadius)
    {
        List<RaycastHit> hits = new List<RaycastHit>();
        float stepDistance = distance / steps;

        for (int i = 0; i < steps; i++)
        {
            float stepDistanceMultiplier = (i / (float)(steps - 1));
            float currentDistance = stepDistance * i;
            Vector3 raycastOrigin = origin + (direction.normalized * currentDistance);
            RaycastHit[] raycastHits = Physics.SphereCastAll(raycastOrigin, sphereRadius, direction, stepDistance, inputIgnoreMask);

            if(debugLines)
            {
                Debug.DrawLine(raycastOrigin, raycastOrigin + direction, Random.ColorHSV(), 2f);
            }

            foreach (RaycastHit hit in raycastHits)
            {
                hits.Add(hit);
            }
        }

        return hits;
    }
}