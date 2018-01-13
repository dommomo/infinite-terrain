using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInterior : MonoBehaviour {

    public Vector3 PreviousPosition = Vector3.zero;
    public Vector2 MapPosition = Vector2.zero;
    public int Key = 0;
    public Sprite floor;
    public Sprite wall;
    public Sprite door;
    public Transform Player;
    public int SceneIdForTerrainView = 0;

    private List<Rect> _walls = new List<Rect>();
    private Rect _exit;
    private int _maxWidth;
    private int _maxHeight;
    private int _randomIndex = 0;

	// Use this for initialization
	void Start () {
        var info = GameObject.FindObjectOfType<InsideBuildingStarter>();
        if (info == null)
        {
            info = new GameObject().AddComponent<InsideBuildingStarter>();
        }
        MapPosition = info.MapPosition;
        Key = info.Key;
        PreviousPosition = info.PreviousPosition;
        Destroy(info.gameObject);
        GenerateInterior();
	}

    // Update is called once per frame
    void Update () {
		
	}

    public bool IsBlocked(Rect area)
    {
        foreach (var wall in _walls)
        {
            if (wall.Overlaps(area)) return true;
        }
        return false;
    }

    public bool IsExiting(Rect area)
    {
        return _exit.Overlaps(area);
    }

    void GenerateInterior()
    {
        List<Vector3> applied = new List<Vector3>();
        List<Vector3> walls = new List<Vector3>();
        Vector3 lowest = new Vector3(0, int.MaxValue, 0);

        _maxWidth = Range(12) + 8;  //8 to 20
        _maxHeight = Range(12) + 8;
        int roomCount = Range(4) + 1;

        var prevRoom = RandomRoom();
        for (int roomIndex = 0; roomIndex < roomCount; roomIndex++)
        {
            var newRoom = RandomRoom();
            if(!newRoom.Overlaps(prevRoom))
            {
                roomCount++;
                continue;
            }

            for (int x = 0; x < newRoom.width; x++)
            {
                for (int y = 0; y < newRoom.height; y++)
                {
                    var pos = new Vector3(newRoom.x + x, newRoom.y + y, 0);
                    if (applied.Contains(pos)) continue;
                    applied.Add(pos);
                    if (lowest.y > pos.y) lowest = pos; //set new low - we need to find lowest in order to place door

                    walls.AddRange(new Vector3[]
                        {
                            pos + Vector3.up,
                            pos + Vector3.down,
                            pos + Vector3.left,
                            pos + Vector3.right,
                            pos + Vector3.up + Vector3.left,
                            pos + Vector3.down + Vector3.left,
                            pos + Vector3.up + Vector3.right,
                            pos + Vector3.down + Vector3.right,
                        });

                    var tile = new GameObject();
                    tile.transform.position = pos;
                    var renderer = tile.AddComponent<SpriteRenderer>();
                    renderer.sprite = floor;
                    tile.transform.parent = transform;
                    tile.name = "Floor " + tile.transform.position;
                }
            }
        }
        //fill in walls
        foreach (var wallPos in walls)
        {
            if (applied.Contains(wallPos)) continue;
            applied.Add(wallPos);
            var tile = new GameObject();
            tile.transform.position = wallPos;
            var renderer = tile.AddComponent<SpriteRenderer>();
            renderer.sprite = wall;
            tile.transform.parent = transform;
            tile.name = "Wall " + tile.transform.position;
            if (wallPos + Vector3.up == lowest)
            {
                renderer.sprite = door;
                _exit = new Rect(wallPos, Vector3.one);
            }
            else
            {
                _walls.Add(new Rect(wallPos, Vector2.one));
            }
        }
        //move player to starting spot in room
        var position = applied[0];
        position.z = Player.position.z;
        Player.position = position;
    }

    private int Range(int max)
    {
        return RandomHelper.Range(MapPosition, Key + _randomIndex++, max);
    }

    private Rect RandomRoom()
    {
        return new Rect(Range(_maxWidth / 2),
                        Range(_maxHeight / 2),
                        Range(_maxWidth / 4) + _maxWidth / 4,
                        Range(_maxHeight / 4) + _maxHeight / 4);
    }
}
