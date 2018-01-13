using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SaveLoadTest : MonoBehaviour {

    public string SavedCache;

    private Cache _cache;

	// Use this for initialization
	void Start () {
        _cache = Cache.Get();
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown(KeyCode.Alpha1))
        {
            SavedCache = _cache.SerializeCache();
        }
        if (Input.GetKeyDown(KeyCode.Alpha2))
        {
            _cache.DeserializeCache(SavedCache);
        }
    }
}
