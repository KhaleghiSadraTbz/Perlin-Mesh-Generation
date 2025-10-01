using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PerlinProfile // : MonoBehaviour
{
    public Vector2 Tiling;

    public Vector2 Offset;

    public float Amplitude;

    public static float calculateHeight(List<PerlinProfile> profiles, float x, float y)
    {
        float height = 0f;
        profiles.ForEach(profile =>
        {
            float xCoord = (x + profile.Offset.x) * profile.Tiling.x;
            float yCoord = (y + profile.Offset.y) * profile.Tiling.y;
            height += Mathf.PerlinNoise(xCoord, yCoord) * profile.Amplitude;
        });
        return height;
    }
}
