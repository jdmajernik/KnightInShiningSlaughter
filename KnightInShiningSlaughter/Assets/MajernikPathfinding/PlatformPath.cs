using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatformPath : MonoBehaviour {

    //public variables
    public Vector3[] vectorPath;
    //private variables
    public List<Node> nodePath;
    public int count;
    public float time;

    private List<Node> nodePathBackwards;//a list of nodes starting with the end position of the path
    private int nodeLayerMask;

    private void Awake()
    {
        nodeLayerMask = 1 << LayerMask.NameToLayer("node");
        nodePath = new List<Node>();
        nodePathBackwards = new List<Node>();
    }

    public void createPath(Vector3 startPos, Vector3 endPos)
    {
        time = Time.time;
        if (Vector3.Distance(startPos,endPos)<2)
        {
            return; //avoids a null reference error
        }
        nodePath.Clear();
        Node lastNode = null;//instantiate it as empty for testing purposes
        Node nextNode = null;
        Node endNode;
        Node thisNode;
        //the node positions for the backwards path
        Vector3 lastBackNodePos = new Vector3(0, 0, 0);//instantiate it as empty for testing purposes
        Vector3 nextBackNodePos = new Vector3(0, 0, 0);
        Vector3 thisBackNodePos;
        bool foundPath = false;
        count = 0; //counter variable
        nodePath.Add(findNearestNode(startPos));
        endNode = findNearestNode(endPos);
        nodePathBackwards.Add(findNearestNode(endPos));
        //starts off the "ThisNodePos" variables as the first node in each path
        thisNode = nodePath[0];
        lastNode = thisNode;
        thisBackNodePos = nodePathBackwards[0].location;
        //Debug.Log("Looking for path");

        while (!foundPath)
        {   
            if(!nodePath[0].connectionsSetUp)
            {
                break;
            }
            //nodePath.Add(nodePath[0].connections[0]);
            bool firstPos = true;
            for (int a = 0; a < nodePath[count].connections.Count; a++)
            {
                if (nodePath[count].connections[a] != lastNode)
                {
                    if (firstPos)
                    {
                        nextNode = nodePath[count].connections[a];
                        firstPos = false;
                    }
                    else if (Vector3.Distance(nodePath[count].connections[a].location, endPos) < Vector3.Distance(nextNode.location, endPos))
                    {
                        nextNode = nodePath[count].connections[a];
                    }

                }
            }
            //After finding the next node to jump to, it sets all the variables
            lastNode = thisNode;
            thisNode = nextNode;
            drawLine(thisNode.location, lastNode.location);//draws a debug line CAN BE REMOVED LATER
            if (!nodePath.Contains(nextNode))
            {
                nodePath.Add(nextNode);
            }
            firstPos = true;
            count++;
            if(count>5)
            {
                //break;
            }
            if (thisNode == endNode)
            {
                //if the node positions are the same, exits the loop
                foundPath = true;
                break; //a bit redundant, but better to err on the side of caution, right?
            }
        }
        drawPath();
        makeVectorPath();
    }
    private void convertToVector ()
    {

    }
    private Node findNearestNode(Vector3 location)
    {
        float radius = 1;
        GameObject nearestNode = null;
        if(Physics2D.OverlapPoint(location,nodeLayerMask))
        {
            nearestNode = Physics2D.OverlapPoint(location, nodeLayerMask).gameObject;
            return nearestNode.GetComponent<Node>();
        }
        while(!Physics2D.OverlapCircle(location, radius, nodeLayerMask))
        {
            radius++;
            if(radius>1000)
            {
                return null; //returns null if the search radius exceeds a set variable
            }
        }
        Collider2D[] allNearNodes = Physics2D.OverlapCircleAll(location, radius, nodeLayerMask);
        foreach(Collider2D node in allNearNodes)
        {
            if(nearestNode == null)
            {
                nearestNode = node.gameObject;
            }
            else
            {
                if(Vector3.Distance(nearestNode.transform.position, location)>= Vector3.Distance(node.transform.position, location))
                {
                    nearestNode = node.gameObject;
                }
            }
        }
        if(nearestNode!= null)
        {
            return nearestNode.GetComponent<Node>();
        }
        else
        {
            return null;
        }
    }
    private void drawLine (Vector3 startPos, Vector3 endPos)
    {
        //Debug.DrawLine(startPos, endPos, Color.green, .1f, false);
    }
    private void drawPath ()
    {
        for(int a = 1; a<nodePath.Count; a++)
        {
            Debug.DrawLine(nodePath[a-1].location, nodePath[a].location, Color.green, .5f);
        }
    }
    private void makeVectorPath ()
    {
        vectorPath = new Vector3[nodePath.Count - 1];//the vectors between the nodes (-1 for array count -1 for being between the nodes)
        for(int a = 0; a<nodePath.Count-1; a++)
        {
            /*if(nodePath[a].thisNodeType == Node.nodeType.jump)
            {
                vectorPath[a] = new Vector3(0, 1, 0);
            }
            else
            {*/
            vectorPath[a] = (nodePath[a + 1].location - nodePath[a].location).normalized; //generates a normalized vector between the two nodes
            
        }
    }
    public bool nextNode (int currentWaypoint, float distance)
    {
        if(Vector3.Distance(transform.position, nodePath[currentWaypoint+1].location)<=distance)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    public Vector3 jumpVel(float time, Vector3 point1, Vector3 point3, float jumpHeight)
    {
        Vector3 endPos = new Vector3(0,0,0);//the end poisition to feed back to the object
        Vector3 point2 = new Vector3((point1.x + point3.x) / 2, point3.y + jumpHeight); //creates a point between point1(start point) and point2 (end point)
        float n = (1 - time); //replaces 1-t in the bezier quadratic equation
        //endPos = (1-t)2*point1 + 2(1-t)t*point2 + t2*point3
        //endPos = (n^2)*point1 + 2*n*time*point2 + (time^2)*point3
        Debug.DrawLine(point1, point2, Color.gray, 5.0f);
        Debug.DrawLine(point2, point3, Color.gray, 5.0f);
        endPos = ((n*n)*point1) + (2*n*time*point2) + ((time*time)*point3); //basically an equation for a quadratic bezier curve (that I found online and modified slightly) using time from t=0 to t=1
        Debug.Log("output position - "+endPos);
        return endPos; //really hopeful this works
    }
}
