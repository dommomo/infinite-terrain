using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour {

    public int HorizontalTiles = 25;
    public int VerticalTiles = 25;
    public int Key = 1;
    public Transform player;
    public float maxDistanceFromPlayer = 7;
    public Vector2 MapOffset;
    public TerrainType[] TerrainTypes;

    private SpriteRenderer[,] _renderers;
    private IEnumerable<Marker> _markers;

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

    public Vector2 WorldToMapPosition(Vector3 worldPosition)
    {
        if (worldPosition.x < 0) worldPosition.x--;
        if (worldPosition.y < 0) worldPosition.y--;
        return new Vector2((int) (worldPosition.x + MapOffset.x), (int) (worldPosition.y + MapOffset.y));
    }

    void RedrawMap()
    {
        transform.position = new Vector3((int)player.position.x, (int)player.position.y, player.position.z);
        _markers = Marker.GetMarkers(transform.position.x, transform.position.y, Key, TerrainTypes.Length);
        var offset = new Vector3(
            transform.position.x - HorizontalTiles / 2, 
            transform.position.y - VerticalTiles / 2, 
            0);
        for (int x = 0; x < HorizontalTiles; x++)
        {
            for (int y = 0; y < VerticalTiles; y++)
            {
                var spriteRenderer = _renderers[x, y];
                var terrain = SelectTerrain(
                    offset.x + x, 
                    offset.y + y);
                spriteRenderer.sprite = terrain.GetTile(offset.x + x, offset.y + y, Key);
                var animator = spriteRenderer.gameObject.GetComponent<Animator>();
                if (terrain.IsAnimated)
                {
                    if (animator == null)
                    {
                        animator = spriteRenderer.gameObject.AddComponent<Animator>();
                        animator.runtimeAnimatorController = terrain.animationController;
                    }
                }
                else
                {
                    if (animator != null)
                    {
                        GameObject.Destroy(animator);
                    }
                }
            }
        }
    }

    public TerrainType SelectTerrain(float x, float y)
    {
        //int index = RandomHelper.Range(x, y, Key, Sprites.Length);
        var marker = Marker.Closest(_markers, new Vector2(x, y), Key);
        return TerrainTypes[marker.TerrainType];
    }
}
