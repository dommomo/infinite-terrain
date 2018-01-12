using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Marker {

    public Vector2 location { get; set; }
    public TerrainType terrain { get; set; }
    public bool isCity { get; set; } 
    public float cityMass { get; set; }

    public static IEnumerable<Marker> GetMarkers(float x, float y, int key, TerrainType[] terrains, float cityChance)
    {
        var markers = new Marker[9];
        x = (int)x >> 4;
        y = (int)y >> 4;
        int markerIndex = 0;

        for (int iX = -1; iX < 2; iX++)
        {
            for (int iY = -1; iY < 2; iY++)
            {
                var terrainRand = terrains[RandomHelper.Range(x + iX, y + iY, key, terrains.Length)];
                bool isCityRand = !terrainRand.NotWalkable && cityChance > RandomHelper.Percent(x + iX, y + iY, key);
                float massRand = RandomHelper.Percent(x + iX, y + iY, key) * 8 + 2; //between 2 and 10
                markers[markerIndex++] = new Marker()
                {
                    location = new Vector2((int)(x + iX) << 4, (int)(y + iY) << 4),
                    terrain = terrainRand,
                    isCity = isCityRand,
                    cityMass = massRand
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
                (int) (marker.location.x + location.x),
                (int) (marker.location.y + location.y),
                key) *  8;
            float distance = Vector2.Distance(marker.location, location);
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
