using System;
using System.Collections.Generic;
using UnityEngine;
using Object = UnityEngine.Object;

public class PoolMono<T> where T : MonoBehaviour
{
    public T prefab { get; }
    public bool autoExpand {  get; set; }
    public Transform container { get; }
    private List<T> pool;

    public PoolMono(T prefab, int count, Transform container)
    {
        this.prefab = prefab;
        this.container = container;

        this.CreatePool(count);
    }

    private void CreatePool(int count)
    {
        this.pool = new List<T>();

        for (int i = 0; i < count; i++)
        {
            this.CreateObject();
        }
    }
    private T CreateObject(bool isActiveByDefault = false)
    {
        var createdObject = Object.Instantiate(prefab, container.transform, container);
        createdObject.gameObject.SetActive(isActiveByDefault);
        this.pool.Add(createdObject);
        return createdObject;
    }
    public bool HasFreeElement(out T element)
    {
        foreach (var mono in pool)
        {
            if(!mono.gameObject.activeInHierarchy){//   поменял, в оригинале ! нету
                element = mono;
                mono.gameObject.SetActive(true);
                return true;
            }
        }
        element = null;
        return false;
    }

    public T GetFreeElement()
    {
        if(this.HasFreeElement(out var element))
            return element;
        if(this.autoExpand)
            return this.CreateObject(true);

        throw new Exception($"There is no free elements in pool of type {typeof(T)}");
    }

    public void Clear()
    {
        pool.Clear();
    }
    public void OfAllElement()
    {
        for (int i = 0;i < pool.Count; i++)
        {
            pool[i].gameObject.SetActive(false);
        }
    }
    public int GetActiveElement()
    {
        int active = 0;
        for (int i = 0; i < pool.Count; i++)
        {
            if (pool[i].gameObject.activeInHierarchy)
            {
                active++;
            }
        }
        return active;
    }
}
