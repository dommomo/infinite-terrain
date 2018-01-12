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
    public Sprite[] Buildings;
    public float cityChance = 0.30f;

    private SpriteRenderer[,] _renderers;
    private IEnumerable<Marker> _markers;
    private List<GameObject> _buildings = new List<GameObject>();

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

    void LoadCity(Marker marker)
    {
        if (!marker.isCity) return;

        int cityMass = (int)marker.cityMass;
        bool[,] addAt = new bool[cityMass * 2, cityMass * 2];

        for (int iArea = 0; iArea < cityMass; iArea++)
        {
            int x1 = RandomHelper.Range(
                marker.location.x + iArea,
                marker.location.y,
                Key,
                cityMass);
            int y1 = RandomHelper.Range(
                marker.location.x,
                marker.location.y + iArea,
                Key,
                cityMass);
            int x2 = RandomHelper.Range(
                marker.location.x + iArea,
                marker.location.y - iArea,
                Key,
                cityMass) + cityMass;
            int y2 = RandomHelper.Range(
                marker.location.x -iArea,
                marker.location.y + iArea,
                Key,
                cityMass) + cityMass;
            for (int x = x1; x < x2 + 1; x++)
            {
                addAt[x, y1] = true;
                addAt[x, y2] = true;
            }
            for (int y = y1; y < y2 + 1; y++)
            {
                addAt[x1, y] = true;
                addAt[x2, y] = true;
            }
            if(RandomHelper.TrueFalse(marker.location, Key + iArea))
            {
                int removeX = RandomHelper.Range(marker.location, iArea - Key, x2 - x1) + x1;
                for (int y = 0; y < cityMass * 2; y++)
                {
                    addAt[removeX, y] = false;
                }
            }
            else
            {
                int removeY = RandomHelper.Range(marker.location, iArea + Key, y2 - y1) + y1;
                for (int x = 0; x < cityMass * 2; x++)
                {
                    addAt[x, removeY] = false;
                }
            }
        }

        for (int x = 0; x < cityMass * 2; x++)
        {
            for (int y = 0; y < cityMass * 2; y++)
            {
                if (!addAt[x, y]) continue; //skip code if this one blank
                var building = new GameObject();
                _buildings.Add(building);

                building.transform.position = new Vector3(
                    marker.location.x - marker.cityMass + x,
                    marker.location.y - marker.cityMass + y,
                    0.01f);
                var renderer = building.AddComponent<SpriteRenderer>();
                renderer.sprite = Buildings[RandomHelper.Range(building.transform.position,
                                                               Key,
                                                               Buildings.Length)];
                building.transform.parent = transform;
                building.name = "Building " + building.transform.position;
            }
        }   
    }

    void RedrawMap()
    {
        transform.position = new Vector3((int)player.position.x, (int)player.position.y, player.position.z);
        _markers = Marker.GetMarkers(transform.position.x, transform.position.y, Key, TerrainTypes, cityChance);
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

        _buildings.ForEach(x => Destroy(x));
        _buildings.Clear();
        foreach (var marker in _markers)
        {
            LoadCity(marker);
        }
    }

    public TerrainType SelectTerrain(float x, float y)
    {
        return Marker.Closest(_markers, new Vector2(x, y), Key).terrain;
    }
}
