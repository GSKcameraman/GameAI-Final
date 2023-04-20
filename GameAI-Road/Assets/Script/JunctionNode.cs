using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class Road
{
    
    public GameObject JunctionEnd;
    public List<GameObject> roadStart;
    public int speed = 5;

}
public class JunctionNode : MonoBehaviour
{
    
    public List<Road> Roads;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Road r in Roads)
        {
            //Debug.DrawLine(this.transform.position, r.JunctionEnd.transform.position, Color.cyan);
            foreach (GameObject s in r.roadStart)
            {
                //Debug.Log(s);
                s.GetComponent<RoadNode>().parent.Add(this.gameObject);
            }
        }

    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (Road r in Roads)
        {
            Debug.DrawLine(this.transform.position, r.JunctionEnd.transform.position, Color.cyan);
            foreach(GameObject s in r.roadStart)
            {
                //Debug.Log(s);
                Debug.DrawLine(this.transform.position, s.transform.position, Color.red);
            }
        }
    }

    public Road FindRoad(GameObject JunctionEnd)
    {
        foreach (Road road in Roads)
        {
            if (JunctionEnd.Equals(road.JunctionEnd) )
            {
                return road;
            }

        }

        return null;
    }
}
