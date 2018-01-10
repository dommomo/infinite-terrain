using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MarkBlockedTiles : MonoBehaviour {

    public TerrainManager Terrain_Manager;
    private float radius = 0.25f;
    private float tileRange = 4;

    void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            return;
        }

        for (float x = transform.position.x - tileRange; x <= transform.position.x + tileRange; x += radius)
        {
            for (float y = transform.position.y - tileRange; y <= transform.position.y + tileRange; y += radius)
            {
                Vector3 worldPos = new Vector3(x, y, 0);
                var mapPos = Terrain_Manager.WorldToMapPosition(worldPos);
                bool isBlocked = false;
                var terrain = Terrain_Manager.SelectTerrain(mapPos.x, mapPos.y);

                if (terrain.NotWalkable)
                {
                    UnityEditor.Handles.DrawWireDisc(worldPos, Vector3.back, radius);
                }
            }
        }
    }

}
