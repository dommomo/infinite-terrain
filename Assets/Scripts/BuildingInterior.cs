using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildingInterior : MonoBehaviour {

    public Vector2 MapPosition = Vector2.zero;
    public int Key = 0;
    public Sprite floor;
    public Transform Player;

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
        Destroy(info.gameObject);
        GenerateInterior();
	}

    // Update is called once per frame
    void Update () {
		
	}

    void GenerateInterior()
    {
        List<Vector3> applied = new List<Vector3>();

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
                    var tile = new GameObject();
                    tile.transform.position = pos;
                    var renderer = tile.AddComponent<SpriteRenderer>();
                    renderer.sprite = floor;
                    tile.transform.parent = transform;
                    tile.name = "Tile " + tile.transform.position;
                }
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
