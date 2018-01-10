using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker {

    public Vector2 Location { get; set; }
    public int TerrainType { get; set; }

    public static IEnumerable<Marker> GetMarkers(float x, float y, int key, int terrainTypeLength)
    {
        var markers = new Marker[9];
        x = (int)x >> 4;
        y = (int)y >> 4;
        int markerIndex = 0;

        for (int iX = -1; iX < 2; iX++)
        {
            for (int iY = -1; iY < 2; iY++)
            {
                markers[markerIndex++] = new Marker()
                {
                    TerrainType = RandomHelper.Range(x + iX, y + iY, key, terrainTypeLength),
                    Location = new Vector2((int)(x + iX) << 4, (int)(y + iY) << 4)
                };
            }
        }
        return markers;
    }

    public static Marker Closest(IEnumerable<Marker> markers, Vector2 location, int key)
    {
        Marker selected = null;
        float closest = float.MaxValue;
        foreach (var marker in markers)
        {
            float rand = RandomHelper.Percent(
                (int) (marker.Location.x + location.x),
                (int) (marker.Location.y + location.y),
                key) *  8;
            float distance = Vector2.Distance(marker.Location, location);
            distance -= rand;
            if (distance < closest)
            {
                closest = distance;
                selected = marker;
            }
        }

        return selected;
    }

}
