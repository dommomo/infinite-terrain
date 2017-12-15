﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour {

    public Sprite TileableImage;
    public int HorizontalTiles = 25;
    public int VerticalTiles = 25;

	// Use this for initialization
	void Start ()
    {
        var offset = new Vector3(0 - HorizontalTiles / 2, 0 - VerticalTiles / 2, 0);

        for (int x = 0; x < HorizontalTiles; x++)
        {
            for (int y = 0; y < VerticalTiles; y++)
            {
                var tile = new GameObject();
                tile.transform.position = new Vector3(x, y, 0) + offset;
                var spriteRenderer = tile.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = TileableImage;
                tile.name = "Terrain " + tile.transform.position;
                tile.transform.parent = transform;
            }
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}
}
