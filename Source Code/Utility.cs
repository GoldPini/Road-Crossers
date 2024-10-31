using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utility 
{
    /// <summary>
    /// Determines whether the given position vector is inside the current screen coundries
    /// </summary>
    /// <param name="worldPosition"></param>
    /// <param name="camera"></param>
    /// <returns>true if the given position is within the current screen view, false otherwise</returns>
    public static bool IsInsideViewScreen(this Vector3 worldPosition, Camera camera)
    {
        Vector3 playerOnScreenPos = camera.WorldToScreenPoint(worldPosition);
        return !(playerOnScreenPos.x < 0 || playerOnScreenPos.x > Screen.width ||
                 playerOnScreenPos.y < 0 || playerOnScreenPos.y > Screen.height);            
    }
}
