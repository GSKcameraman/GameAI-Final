using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoadNode : MonoBehaviour
{
    public List<GameObject> parent;
    public List<GameObject> child;

    // Start is called before the first frame update
    void Start()
    {
        foreach (GameObject c in child)
        {
            RoadNode n = c.GetComponent<RoadNode>();
            if (n != null)
            {
                n.parent.Add(this.gameObject);
            }
        }
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        foreach (GameObject c in child)
        {
            Debug.DrawLine(this.transform.position, c.transform.position, Color.blue);
        }
    }
}
