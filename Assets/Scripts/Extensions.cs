using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Extensions
{
    public static bool InLineOfSightOf(this Vector3 startPos, Vector3 endPos, LayerMask obstacleLayer)
    {
        return !Physics.Linecast(startPos, endPos, obstacleLayer);
    }
}
