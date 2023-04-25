using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
public static class CameraUtilities
{
    private static LayerMask inputIgnoreMask => ~(1 << LayerMask.NameToLayer("Player"));

    private static RaycastHit[] SteppedSphereRaycast(Vector3 origin, Vector3 direction, float distance, int steps, float sphereRadius)
    {
        List<RaycastHit> hits = new List<RaycastHit>();
        float stepDistance = distance / steps;

        for (int i = 0; i < steps; i++)
        {
            float stepDistanceMultiplier = ((float)i / (float)(steps - 1));
            float currentDistance = stepDistance * i;
            Vector3 raycastOrigin = origin + (direction.normalized * currentDistance);
            RaycastHit[] raycastHits = Physics.SphereCastAll(raycastOrigin, sphereRadius, direction, stepDistance, inputIgnoreMask);

            foreach (RaycastHit hit in raycastHits)
            {
                hits.Add(hit);
            }
        }

        // Sort the hits based on distance
        hits.Sort((a, b) => {
            float distanceToA = Vector3.Distance(origin, a.point);
            float distanceToB = Vector3.Distance(origin, b.point);
            return distanceToA.CompareTo(distanceToB);
        });

        return hits.ToArray();
    }

    public static RaycastHit[] SteppedSpherecast()
    {
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width * 0.5f, Screen.height * 0.5f));
        return SteppedSphereRaycast(ray.origin, ray.direction, 2f, 5, 0.05f);
    }

    public static RaycastHit[] SpherecastFromScreenCentre()
    {
        RaycastHit[] hits;
        Ray ray = Camera.main.ScreenPointToRay(new Vector3(Screen.width*0.5f, Screen.height*0.5f));
        Debug.DrawRay(ray.origin, ray.direction * 2f, Random.ColorHSV(), 3f);
        hits = Physics.SphereCastAll(ray, 0.05f, 2f, inputIgnoreMask);

        return hits;
    }

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
}