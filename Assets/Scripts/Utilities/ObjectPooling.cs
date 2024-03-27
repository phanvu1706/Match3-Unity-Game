using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using System.Security.Cryptography;
using JetBrains.Annotations;
using UnityEngine;
using UnityEngine.UIElements;

/*

Tạo pool chứa view của các cell tránh instance destroy nhiều lần

*/

public class ObjectPooling : MonoBehaviour
{
    public static ObjectPooling Instance { private set; get; }

    private Transform m_transform;

    private struct PoolObject
    {
        public GameObject prefab;
        public Transform container;
    }

    private Dictionary<string, PoolObject> poolObjects = new Dictionary<string, PoolObject>();

    private void Awake()
    {
        Instance = this;
        m_transform = transform;
    }

    public void PreloadObject(string name, int count)
    {
        if (poolObjects.ContainsKey(name))
            return;

        GameObject prefab = Resources.Load<GameObject>(name);
        if (prefab)
        {
            PoolObject poolObject = new PoolObject()
            {
                prefab = prefab,
                container = new GameObject(name).transform
            };

            poolObject.container.SetParent(m_transform);

            for (int i = 0; i < count; ++i)
            {
                GameObject go = Instantiate(prefab);
                go.transform.SetParent(poolObject.container);
                go.SetActive(false);
            }

            poolObjects.Add(name, poolObject);
        }
    }

    public GameObject GetObject(string name)
    {
        if (poolObjects.ContainsKey(name))
        {
            PoolObject po = poolObjects[name];
            if (po.container.childCount > 0)
            {
                GameObject go = po.container.GetChild(0).gameObject;
                go.transform.SetParent(null);
                go.SetActive(true);

                return go;
            }

            return Instantiate(po.prefab);
        }

        GameObject prefab = Resources.Load<GameObject>(name);
        if (prefab)
        {
            PoolObject poolObject = new PoolObject()
            {
                prefab = prefab,
                container = new GameObject(name).transform
            };

            poolObject.container.SetParent(m_transform);
            poolObjects.Add(name, poolObject);

            return Instantiate(prefab);
        }

        return null;
    }

    public void ReturnObject(string name, GameObject go)
    {
        if (poolObjects.ContainsKey(name))
        {
            PoolObject po = poolObjects[name];
            go.transform.SetParent(po.container);
            go.transform.localScale = Vector3.one;
            go.SetActive(false);
        }
        else
        {
            Destroy(go);
        }
    }
}
