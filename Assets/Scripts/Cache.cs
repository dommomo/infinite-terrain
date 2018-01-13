using System.Collections;
using System.Collections.Generic;
using System.Xml.Serialization;
using System.IO;
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

    public string Serialize(object target)
    {
        var ser = new XmlSerializer(target.GetType());
        using (var writer = new StringWriter())
        {
            ser.Serialize(writer, target);
            return writer.ToString();
        }
    }

    public T Deserialize<T>(string content)
    {
        var ser = new XmlSerializer(typeof(T));
        using (var stream = new StringReader(content))
        {
            return (T)ser.Deserialize(stream);
        }
    }

    public string SerializeCache()
    {
        List<string> cacheArray = new List<string>();
        foreach (var item in _content)
        {
            cacheArray.Add(Serialize(item));
        }
        return Serialize(cacheArray.ToArray());
    }

    public void DeserializeCache(string content)
    {
        string[] serList = Deserialize<string[]>(content);
        _content.Clear();
        foreach (string item in serList)
        {
            _content.Add(Deserialize<CacheContent>(item));
        }
    }
}
