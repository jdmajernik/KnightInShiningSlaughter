using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour {

    //The behaviour script for all the nodes (spawned in gameObjects used for pathfinding)
    //basically nodes act as switches, storing all connected nodes in each node which the pathfinder

    //private Variables
    public List<Node> connections;//the location of all connected nodes
    private string[] connectionType; //a string array holding in the type of the connection (for the PATH script) //TODO add in PATH script
    private int currentLayer;
    private float nodeDist; //distance between nodes
    private bool nodeSetup = true;
    private float jumpHeight = 30f;

    //public Variables
    public enum nodeType { normal, ledgeR, ledgeL, jump };
    public nodeType thisNodeType = nodeType.normal;//this determines the type of node
    public Vector3 location;//public location variable to check while pathfinding
    public bool connectionsSetUp;

    private void Awake()
    {
        connectionsSetUp = false;
    }
    private void Start()
    {
        location = transform.position;
    }


    public IEnumerator setUpConnections(float distance, int layerMask, GameObject[] allNodes)
    {
        //sets up proximity-based connections
        nodeDist = distance;
        currentLayer = layerMask;
        int count = 0;
        Collider2D[] nearNodes = Physics2D.OverlapCircleAll(transform.position, distance*1.25f, currentLayer);
        foreach (Collider2D node in nearNodes)
        {
            Node thisNode = node.gameObject.GetComponent<Node>();
            if (node.transform.position != transform.position)
            { 
                   
                if (!connections.Contains(thisNode))
                {
                    connections.Add(thisNode);
                    Debug.DrawLine(transform.position, node.transform.position, Color.red, 50f, false);
                    count++;

                }
            }
        }
        if(thisNodeType == nodeType.ledgeR)
        {
            for(float a = degreeToRads(0); a<degreeToRads(50); a+=degreeToRads(1))
            {
                Vector2 dir = new Vector2(Mathf.Sin(a), -Mathf.Cos(a)).normalized*100;
                float offsetDist = 1.5f;
                Vector3 offset = new Vector3(offsetDist, -offsetDist,0);
                RaycastHit2D hit = Physics2D.Raycast(transform.position+offset, dir);
                Debug.DrawRay(transform.position+offset, dir, Color.blue, 2);
                if (Physics2D.Raycast(transform.position + offset, dir))
                {
                    if (hit.collider.isTrigger && hit.collider.transform.position != transform.position && hit.collider.gameObject.tag == "node")
                    {
                        GameObject nodeHit = hit.collider.gameObject;
                        connections.Add(nodeHit.GetComponent<Node>());
                        nodeHit.GetComponent<Node>().addConnection(gameObject.GetComponent<Node>());
                        Debug.Log("Ledge node distance - " + Vector3.Distance(hit.collider.transform.position, transform.position));
                        if (Vector3.Distance(nodeHit.transform.position, transform.position) <= jumpHeight)
                        {
                            //if the other node is within jump height, it adds this nod to the other's connection list and sets that node to a jump node
                            nodeHit.GetComponent<Node>().thisNodeType = Node.nodeType.jump;

                        }
                        Debug.DrawLine(transform.position, nodeHit.transform.position, Color.red, 50f, false);
                        count++;
                        break;
                    }
                }

            }
        }
        else if(thisNodeType == nodeType.ledgeL)
        {
            for (float a = degreeToRads(0); a > degreeToRads(-50); a -= degreeToRads(1))
            {
                Vector2 dir = new Vector2(Mathf.Sin(a), -Mathf.Cos(a)).normalized * 100;
                float offsetDist = 1.5f;
                Vector3 offset = new Vector3(-offsetDist, -offsetDist, 0);
                RaycastHit2D hit = Physics2D.Raycast(transform.position + offset, dir);
                Debug.DrawRay(transform.position + offset, dir, Color.blue, 2);
                if (Physics2D.Raycast(transform.position + offset, dir))
                {
                    if (hit.collider.isTrigger && hit.collider.transform.position != transform.position && hit.collider.gameObject.tag == "node")
                    {
                        GameObject nodeHit = hit.collider.gameObject;
                        //Debug.Log("Ledge node distance - " + Vector3.Distance(hit.collider.transform.position, transform.position));
                        connections.Add(nodeHit.GetComponent<Node>());
                        nodeHit.GetComponent<Node>().addConnection(gameObject.GetComponent<Node>());
                        if (Vector3.Distance(nodeHit.transform.position, transform.position) <= jumpHeight)
                        {
                            //if the other node is within jump height, it adds this nod to the other's connection list and sets that node to a jump node
                            nodeHit.GetComponent<Node>().thisNodeType = Node.nodeType.jump;

                        }
                        Debug.DrawLine(transform.position, hit.transform.position, Color.red, 50f, false);
                        count++;
                        break;

                    }
                }

            }
            yield return null;
        }
        //yield return null;
    }
    public void addConnection(Node addedNode)
    {
        //adds a node at the end of the connections array (for ledges)
        if(!connections.Contains(addedNode))
        {
            connections.Add(addedNode);
        }
    }
    public void addJumpConnection(Transform otherNode)
    {
        //TODO add in a connection for jump nodes (also maybe switch the nodetype depending on the location?)
    }
    public void addFallConnection()
    {
        //TODO add in a search for lower nodes and connect them (if they exist)
    }
    
    //All of these TODOs are probably going to end up in platformerPath, but since it doesn't exist and I need to write them down, here they areS
        //TODO: set up a path between two nodes
        //TODO: set up a pathfinder to find a path between two points
        //TODO: set up a jump node behavior
        //TODO: set up a dropdown node behavior(?)
    private float degreeToRads (float degreeIn)
    {
        return (degreeIn * (Mathf.PI * 2))/360;
    }
}
