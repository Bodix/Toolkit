using UnityEngine;

public static class GizmosCustom
{
    public static void DrawRect(Rect rect, float thickness = 0.01f)
    {
        Gizmos.DrawWireCube(
            new Vector3(rect.center.x, rect.center.y, 0f),
            new Vector3(rect.size.x, rect.size.y, thickness));
    }
}