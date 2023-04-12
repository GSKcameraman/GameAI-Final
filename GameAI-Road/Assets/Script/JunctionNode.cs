using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Road
{
    public GameObject JunctionStart;
    public GameObject JunctionEnd;
    public List<GameObject> road;
}
public class JunctionNode : MonoBehaviour
{
    public List<GameObject> ConnectedJuctions;
    public List<Road> Roads;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public Road FindRoad(GameObject JunctionEnd)
    {
        foreach (Road road in Roads)
        {
            if (JunctionEnd == road.JunctionEnd)
            {
                return road;
            }

        }

        return null;
    }
}
