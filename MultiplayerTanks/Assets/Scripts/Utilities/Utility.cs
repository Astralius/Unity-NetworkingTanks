using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public static class Utility
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

    public static T Random<T>(this IList<T> source)
    {
        return source[UnityEngine.Random.Range(0, source.Count)];
    }

    public static T Random<T>(this IList<T> source, Func<T, bool> predicate)
    {
        return source.Where(predicate).ToList().Random();
    }
}
