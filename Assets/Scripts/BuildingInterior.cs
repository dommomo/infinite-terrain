using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInterior : MonoBehaviour {

    public Vector2 MapPosition = Vector2.zero;
    public int Key = 0;
    public Sprite floor;

	// Use this for initialization
	void Start () {
        var info = GameObject.FindObjectOfType<InsideBuildingStarter>();
        if (info == null)
        {
            info = new GameObject().AddComponent<InsideBuildingStarter>();
        }
        MapPosition = info.MapPosition;
        Key = info.Key;
        Destroy(info.gameObject);
        GenerateInterior();
	}

    // Update is called once per frame
    void Update () {
		
	}

    void GenerateInterior()
    {
        var width = RandomHelper.Range(MapPosition, Key, 5) + 3; //3 to 8
        var height = RandomHelper.Range(MapPosition, Key + 1, 5) + 3; //3 to 8

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tile = new GameObject();
                tile.transform.position = new Vector3(x, y, 0);
                var renderer = tile.AddComponent<SpriteRenderer>();
                renderer.sprite = floor;
                tile.transform.parent = transform;
                tile.name = "Tile " + tile.transform.position;
            }
        }
    }
}
