using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cache : MonoBehaviour {

    private List<CacheContent> _content = new List<CacheContent>();

    public static Cache Get()
    {
        var cache = GameObject.FindObjectOfType<Cache>();
        if (cache == null)
        {
            GameObject go = new GameObject();
            go.name = "Cache";
            GameObject.DontDestroyOnLoad(cache);
            cache = go.AddComponent<Cache>();
        }
        return cache;
    }

	public void Add(CacheContent item)
    {
        _content.Add(item);
    }

    public IEnumerable<CacheContent> Get(string objectType, Vector3 near, float radius)
    {
        foreach (var c in _content)
        {
            if (c.ObjectType != objectType) continue;
            if (Vector3.Distance(near, c.Location) < radius) yield return c;
        }
    }
}
