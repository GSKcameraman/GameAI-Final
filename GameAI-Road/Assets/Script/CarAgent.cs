using JetBrains.Annotations;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

[Serializable]
public class JunctionAstar : IEquatable<JunctionAstar>, IComparable<JunctionAstar>
{
    public GameObject Junction;
    public float g;
    public float h;
    public float f;
    public JunctionAstar parent;
    public override bool Equals(object obj)
    {
        JunctionAstar j = obj as JunctionAstar;
        if (j.Junction.Equals(Junction))
        {
            return true;

        }
        else
        return false;
    }
    public override int GetHashCode()
    {
        return Junction.GetHashCode();
    }
    public bool Equals(JunctionAstar other)
    {
        if (other.Junction == Junction)
        {
            return true;
        }
        else
            return false;
    }

    public int CompareTo(JunctionAstar other)
    {
        if (other is null)
        {
            return 1;
        }
        return f.CompareTo(other.f);
    }
}

public class RoadAstar : IEquatable<RoadAstar>, IComparable<RoadAstar>
{
    public GameObject node;
    public float g;
    public float h;
    public float f;
    public RoadAstar parent;

    public bool Equals(RoadAstar other)
    {
        if (node == other.node)
        {
            return true;
        }
        return false;
    }
    public int CompareTo(RoadAstar other)
    {
        if (other is null)
        {
            return 1;
        }
        return f.CompareTo(other.f);
    }
}

public class CarAgent : MonoBehaviour
{
    public NavMeshAgent agent;
    public GameObject target;
    GameObject[] junctions;
    GameObject[] roadNodes;
    bool setup = false;

    [Range(0, 10)]
    public int weightg;
    [Range(0, 10)]
    public int weighth;


    GameObject searchStart;
    GameObject searchEnd;

    List<JunctionAstar> junctionAstars;
    List<JunctionAstar> active;
    JunctionAstar End;
    List<GameObject> route = new List<GameObject>();
    bool routeFound = false;
    bool routeSet = false;
    GameObject currentJStart;
    GameObject currentJEnd;

    List<RoadAstar> roadAstars;
    List<RoadAstar> activeRoads;
    RoadAstar EndRoad;
    bool roadSet = false;

    bool pressed = false;

    public Button startButton;

    public GameObject Line;
    // Start is called before the first frame update
    void Start()
    {
        //agent = GetComponent<NavMeshAgent>();
        agent.enabled = false;
        junctions = GameObject.FindGameObjectsWithTag("Junction");
        roadNodes = GameObject.FindGameObjectsWithTag("Road");
        agent.enabled = true;
        GameObject close = GetClosestRoadNode(this.transform.position);
        agent.SetDestination(close.transform.position);
        setup = true;

    }

    // Update is called once per frame
    void Update()
    {
        if (Time.time > .5f && !setup)
        {
            agent.enabled = true;
            GameObject close = GetClosestRoadNode(this.transform.position);
            agent.SetDestination(close.transform.position);
            setup = true;
        }
        if (routeSet)
        {
            if (route.Count == 0)
            {
                agent.SetDestination(target.transform.position);
                routeSet = false;
                pressed = false;
                startButton.interactable = true;
            }
            else
            {
                agent.SetDestination(route[0].transform.position);
                Debug.DrawLine(this.transform.position, route[0].transform.position, Color.green);
                LineRenderer renderer = Line.GetComponent<LineRenderer>();
                if (Vector3.Distance(this.transform.position, route[0].transform.position) < 2.0f)
                {
                    route.RemoveAt(0);
                }
            }

        }
        else
        {
            if (Vector3.Distance(this.transform.position, agent.destination) < 2.0f)
            {
                GameObject close = GetClosestRoadNode(this.transform.position);
                agent.SetDestination(close.transform.position);
            }
        }
        LineRenderer l = Line.GetComponent<LineRenderer>();
        l.SetPosition(0, this.transform.position + Vector3.up);
        l.SetPosition(1, agent.destination + Vector3.up);

    }

    void NavigateJunction()
    {
        searchStart = GetClosestConnectedJunction(this.transform.position);
        searchEnd = GetClosestParentJunction(target.transform.position);
        junctionAstars = new List<JunctionAstar>();
        active = new List<JunctionAstar>();
        routeFound = false;
        routeSet = false;
        route = new List<GameObject>();
        JunctionAstar j0 = new JunctionAstar();
        j0.Junction = searchStart;
        j0.g = 0;
        j0.h = Vector3.Distance(searchStart.transform.position, searchEnd.transform.position);
        j0.f = j0.h * weighth;
        //junctionAstars.Add(j0);
        active.Add(j0);
        StartCoroutine(JunctionNav());
    }


    IEnumerator JunctionNav()
    {
        while (active.Count > 0 && !routeFound)
        {
            Debug.Log("SearchingJunction!");
            junctionAstars.Sort();
            active.Sort();
            JunctionAstar current = active[0];
            active.RemoveAt(0);
            JunctionNode jn = current.Junction.GetComponent<JunctionNode>();
            if (current.Junction == searchEnd)
            {
                junctionAstars.Add(current);
                End = current;
                routeFound = true;
                
                currentJStart = current.Junction;
                currentJEnd = GetClosestRoadNode(target.transform.position);
                roadSet = false;
                roadAstars = new List<RoadAstar>();
                activeRoads = new List<RoadAstar>();
                GameObject Jnext = GetClosestConnectedJunction(target.transform.position);
                Road r = currentJStart.GetComponent<JunctionNode>().FindRoad(Jnext);
                roadSet = false;
                foreach (GameObject n in r.roadStart)
                {
                    RoadAstar ra = new RoadAstar();
                    ra.parent = null;
                    ra.node = n;
                    ra.g = 0;
                    ra.h = Vector3.Distance(n.transform.position, currentJEnd.transform.position);
                    ra.f = ra.h * weighth;
                    activeRoads.Add(ra);
                }

                yield return StartCoroutine(RoadNav());
                


                StartCoroutine(FillPath());
                StopCoroutine(JunctionNav());
                yield break;
            }
            foreach (Road r in jn.Roads)
            {
                JunctionAstar ja = new JunctionAstar();
                ja.Junction = r.JunctionEnd;
                ja.g = current.g + 1 + Vector3.Distance(current.Junction.transform.position, r.JunctionEnd.transform.position);
                ja.h = Vector3.Distance(searchEnd.transform.position, r.JunctionEnd.transform.position);
                ja.f = ja.g * weightg + ja.h * weighth;
                ja.parent = current;
                if (active.Contains(ja))
                {
                    int index = active.IndexOf(ja);
                    JunctionAstar comp = active.Find(x => x.Equals(ja));
                    if (comp.f > ja.f)
                    {
                        active[index].f = ja.f;
                        active[index].g = ja.g;
                        //active[index].h = ja.h;
                        active[index].parent = current;
                    }
                }
                else
                {
                    active.Add(ja);
                }


            }
            junctionAstars.Add(current);
            yield return null;

        }
    }

    IEnumerator FillPath()
    {
        while (!routeSet)
        {
            Debug.Log("Filling Roads");
            route.Insert(0, End.Junction);
            GameObject Junc = End.Junction;
            if (End.parent == null)
            {
                roadAstars = new List<RoadAstar>();
                activeRoads = new List<RoadAstar>();
                
                currentJStart = GetClosestRoadNode(this.transform.position);
                currentJEnd = Junc;
                RoadAstar ra = new RoadAstar();
                ra.parent = null;
                ra.node = currentJStart;
                ra.g = 0;
                ra.h = Vector3.Distance(currentJStart.transform.position, currentJEnd.transform.position);
                ra.f = ra.h * weighth;
                activeRoads.Add(ra);

                roadSet = false;
                yield return StartCoroutine(RoadNav());
                
                routeSet = true;
                StopCoroutine(FillPath());
                yield break;
            }
            else
            {
                End = End.parent;
                currentJStart = End.Junction;
                currentJEnd = Junc;
                roadAstars = new List<RoadAstar>();
                activeRoads = new List<RoadAstar>();
                Road r = currentJStart.GetComponent<JunctionNode>().FindRoad(currentJEnd);
                roadSet = false;
                foreach (GameObject n in r.roadStart)
                {
                    RoadAstar ra = new RoadAstar();
                    ra.parent = null;
                    ra.node = n;
                    ra.g = 0;
                    ra.h = Vector3.Distance(n.transform.position, currentJEnd.transform.position);
                    ra.f = ra.h * weighth;
                    activeRoads.Add(ra);
                }
                yield return StartCoroutine(RoadNav());

            }
        }
    }

    IEnumerator RoadNav()
    {   
        while (activeRoads.Count > 0 && !roadSet)
        {
            activeRoads.Sort();
            RoadAstar ro = activeRoads[0];
            activeRoads.RemoveAt(0);
            RoadNode rn = ro.node.GetComponent<RoadNode>();
           // if (rn == null) continue;
            
            foreach (GameObject c in rn.child)
            {
                if (c.Equals(currentJEnd))
                {
                    route.Insert(0, ro.node);
                    while (ro.parent != null)
                    {
                        
                        ro = ro.parent;
                        route.Insert(0, ro.node);
                    }
                    roadSet = true;
                    StopCoroutine(RoadNav());
                    yield break;
                }
                if (c.CompareTag("Junction"))
                {
                    continue;
                }

                RoadAstar ron = new RoadAstar();
                ron.parent = ro;
                ron.node = c;
                ron.g = ro.g + Vector3.Distance(c.transform.position, ro.node.transform.position);
                ron.h = Vector3.Distance(c.transform.position, currentJEnd.transform.position);
                ron.f = ron.h * weighth + ron.g * weightg;
                if (activeRoads.Contains(ron))
                {
                    int index = activeRoads.IndexOf(ron);
                    if (activeRoads[index].f > ron.f)
                    {
                        activeRoads[index].f = ron.f;
                        activeRoads[index].g = ron.g;
                        activeRoads[index].parent = ro;
                    }
                }
                else
                {
                    activeRoads.Add(ron);
                }

            }
            roadAstars.Add(ro);
            yield return null;

        }
    }
    GameObject GetClosestJunction(Vector3 position)
    {
        GameObject j = null;
        float dist = float.MaxValue;
        foreach (GameObject junction in junctions)
        {
            float d = Vector3.Distance(position, junction.transform.position);
            if (d < dist)
            {
                dist = d;
                j = junction;
            }
        }
        return j;
    }

    GameObject GetClosestRoadNode(Vector3 position)
    {
        GameObject r = null;
        float d = float.MaxValue;
        foreach(GameObject road in roadNodes)
        {
            float dist = Vector3.Distance(road.transform.position, position);
            if (dist < d)
            {
                d = dist;
                r = road;
            }
        }
        return r;
    }
    

    GameObject GetClosestConnectedJunction(Vector3 position)
    {
        GameObject Road = GetClosestRoadNode(position);
        while (Road.CompareTag("Road"))
        {
            Road = Road.GetComponent<RoadNode>().child[0];
        }
        return Road;
    }

    GameObject GetClosestParentJunction(Vector3 position)
    {
        GameObject Road = GetClosestRoadNode(position);
        while (Road.CompareTag("Road"))
        {
            Road = Road.GetComponent<RoadNode>().parent[0];
        }
        return Road;

    }

    public void StartPath()
    {
        if (!pressed)
        {
            startButton.interactable = false;
            pressed = true;
            NavigateJunction();

        }
    }
}
