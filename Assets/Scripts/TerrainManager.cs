﻿using System.Collections;
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
    public BuildingType[] BuildingTypes;
    public float cityChance = 0.30f;
    public string SceneNameForInsideBuilding = "InsideBuilding";

    private SpriteRenderer[,] _renderers;
    private IEnumerable<Marker> _markers;
    private List<ActiveBuildingType> _buildings = new List<ActiveBuildingType>();
    private Cache _cache;

	// Use this for initialization
	void Start ()
    {
        _cache = Cache.Get();
        var starter = GameObject.FindObjectOfType<TerrainStarter>();
        if (starter != null)
        {
            player.position = starter.PreviousPosition;
            Destroy(starter.gameObject);
        }

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

    public ActiveBuildingType GetBuilding(Vector2 mapPos)
    {
        foreach (var building in _buildings)
        {
            var bLoc = building.transform.position;
            if (mapPos.x >= bLoc.x - 1 && mapPos.y >= bLoc.y - 1
                && mapPos.x < bLoc.x && mapPos.y < bLoc.y)
            {
                return building;
            }
        }
        return null;
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
                CreateBuilding(new Vector3(
                            marker.location.x - marker.cityMass + x,
                            marker.location.y - marker.cityMass + y,
                            0.01f));
            }
        }   
    }

    public ActiveBuildingType CreateBuilding(Vector3 location)
    {
        var building = new GameObject();
        var active = building.AddComponent<ActiveBuildingType>();
        _buildings.Add(active);

        building.transform.position = location;
        var renderer = building.AddComponent<SpriteRenderer>();
        var buildingInfo = BuildingTypes[RandomHelper.Range(building.transform.position,
                                                       Key,
                                                       BuildingTypes.Length)];
        renderer.sprite = buildingInfo.Tile;
        active.BuildingTypeInUse = buildingInfo;
        building.transform.parent = transform;
        building.name = "Building " + building.transform.position;

        return active;
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

        _buildings.ForEach(x => Destroy(x.gameObject));
        _buildings.Clear();
        foreach (var marker in _markers)
        {
            LoadCity(marker);
        }
        foreach (var item in _cache.Get("Building", transform.position, HorizontalTiles / 2))
        {
            CreateBuilding(item.Location);
        }
    }

    public TerrainType SelectTerrain(float x, float y)
    {
        return Marker.Closest(_markers, new Vector2(x, y), Key).terrain;
    }
}
