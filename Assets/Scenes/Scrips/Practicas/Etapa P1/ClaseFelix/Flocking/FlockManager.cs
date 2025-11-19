using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlockManager : MonoBehaviour
{
    List<Flocker> collection = new List<Flocker>();

    public static FlockManager instance; 

    [SerializeField] Flocker leader;
    public static  Flocker Leader
    {
        get
        {
            return instance.leader;
        }
    }

    public void SetLeader(Flocker _leader)
    {
        leader = _leader;
    }

    private void Awake()
    {
        if(instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);  
        }
    }

    public void AddFlocker(Flocker locker)
    {
        if (!collection.Contains(locker)) 
        {
            collection.Add(locker);
        }
    }

    public List<Flocker> GetAllFlockers()
    {
        return collection;  
    }

    Vector3 dir = Vector3.zero;
    List <Flocker> temp = new List<Flocker>();

    public List<Flocker> GetFlockers(Vector3 pos, float radius, Flocker except = null)
    {
        temp.Clear();
        foreach(var f in collection)
        {
            if (f.Equals(except)) continue;
            dir = f.transform.position - pos;

            if (dir.sqrMagnitude < radius * radius)
            {
                temp.Add(f);
            }
        }
        return temp;    
    }
}
