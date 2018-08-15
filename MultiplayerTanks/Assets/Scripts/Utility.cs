using UnityEngine;

public class Utility : MonoBehaviour
{
    public static Vector3 ScreenToElevatedWorldPoint(Vector3 screenPoint, float elevation)
    {
        var ray = Camera.main.ScreenPointToRay(screenPoint);
        var plane = new Plane(Vector3.up, Vector3.up * elevation);

        float distance;
        return plane.Raycast(ray, out distance)
            ? ray.GetPoint(distance)
            : Vector3.zero;
    }
}
