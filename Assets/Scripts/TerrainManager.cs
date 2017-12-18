using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour {

    public Sprite[] Sprites;
    public int HorizontalTiles = 25;
    public int VerticalTiles = 25;
    public int Key = 1;
    public Transform player;
    public float maxDistanceFromPlayer = 7;

    private SpriteRenderer[,] _renderers;

	// Use this for initialization
	void Start ()
    {
        int sortIndex = 0;
        var offset = new Vector3(0 - HorizontalTiles / 2, 0 - VerticalTiles / 2, 0);
        _renderers = new SpriteRenderer[HorizontalTiles, VerticalTiles];
        for (int x = 0; x < HorizontalTiles; x++)
        {
            for (int y = 0; y < VerticalTiles; y++)
            {
                var tile = new GameObject();
                tile.transform.position = new Vector3(x, y, 0) + offset;
                var renderer = _renderers[x, y] = tile.AddComponent<SpriteRenderer>();
                renderer.sortingOrder = sortIndex--;
                tile.name = "Terrain " + tile.transform.position;
                tile.transform.parent = transform;
            }
        }

        RedrawMap();
	}

    void Update()
    {
        if(maxDistanceFromPlayer < Vector3.Distance(player.position, transform.position))
        {
            RedrawMap();
        }
    }

    void RedrawMap()
    {
        transform.position = new Vector3((int)player.position.x, (int)player.position.y, player.position.z);
        for (int x = 0; x < HorizontalTiles; x++)
        {
            for (int y = 0; y < VerticalTiles; y++)
            {
                var spriteRenderer = _renderers[x, y];
                spriteRenderer.sprite = SelectRandomSprite((int)transform.position.x + x, (int)transform.position.y + y);
            }
        }
    }

    public Sprite SelectRandomSprite(int x, int y)
    {
        return Sprites[RandomHelper.Range(x, y, Key, Sprites.Length)];
    }
}
