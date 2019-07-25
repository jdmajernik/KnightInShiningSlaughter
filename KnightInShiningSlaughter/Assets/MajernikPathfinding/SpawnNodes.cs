using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpawnNodes : MonoBehaviour {

    //public Variables
    public float nodeDistance;
    public string tagNodes = "Node";
    public string platformTag;
    public GameObject[] nodes;

    //private Variables
    public GameObject nodePrefab = Resources.Load("Prefabs/node") as GameObject; // the reference to the node prefab (to be instantiated later) 

    public SpawnNodes(string tag)
    {
        platformTag = tag; //requires tag for the platform objects
    }
    private void Awake()
    {
        GameObject[] platforms =  GameObject.FindGameObjectsWithTag(platformTag);
        foreach(GameObject platform in platforms)
        {
            Vector2 platformSize = platform.GetComponent<SpriteRenderer>().bounds.size;//the size of the platform object
            //Debug.Log("Platform Size - " + platformSize);
            Vector3 nodeSpawnPoint = new Vector3(0,0,0);// the location of the node to spawn in
            float b = platformSize.x-1;
            for (float a = 1; a<platformSize.x; a+=nodeDistance)
            {
                if(a<(platformSize.x/2))
                {
                    
                    //spawns in nodes from the left of the platform
                    nodeSpawnPoint = new Vector3(a-(platformSize.x/2), (platformSize.y/2)+2,0) + platform.transform.position;
                    if (Physics2D.OverlapCircle(nodeSpawnPoint, 1.5f, 1 << LayerMask.NameToLayer("Platforms")) || Physics2D.OverlapCircle(nodeSpawnPoint, 1.5f, 1 << LayerMask.NameToLayer("node")))
                    {
                    }
                    else
                    {
                        GameObject newNode = Instantiate(nodePrefab);
                        newNode.transform.position = nodeSpawnPoint;
                        if (a == 1)
                        {
                            newNode.GetComponent<Node>().thisNodeType = Node.nodeType.ledgeL; //makes the new node a ledge node if it's on the end
                        }
                        //count++;
                    }
                }
                if(b>(platformSize.x/2))
                {
                    //spawns in nodes from the right of the platform
                    nodeSpawnPoint = new Vector3(b-(platformSize.x / 2), (platformSize.y/2)+2, 0) + platform.transform.position;
                    if (Physics2D.OverlapCircle(nodeSpawnPoint, 1.5f, 1 << LayerMask.NameToLayer("Platforms"))|| Physics2D.OverlapCircle(nodeSpawnPoint, 1.5f, 1 << LayerMask.NameToLayer("node")))
                    {
                    }
                    else
                    {
                        GameObject newNode = Instantiate(nodePrefab);
                        newNode.transform.position = nodeSpawnPoint;
                        if (b == platformSize.x-1)
                        {
                            newNode.GetComponent<Node>().thisNodeType = Node.nodeType.ledgeR; //makes the new node a ledge node if it's on the end
                        }
                        
                    }
                    b -= nodeDistance;
                }
                
            }
            if (!Physics2D.OverlapCircle(nodeSpawnPoint, 1.5f, 1 << LayerMask.NameToLayer("Platforms")) || !Physics2D.OverlapCircle(nodeSpawnPoint, 1.5f, 1 << LayerMask.NameToLayer("node")))
            {
                nodeSpawnPoint = new Vector3(0, (platformSize.y / 2) + 2, 0) + platform.transform.position;
                GameObject middleNode = Instantiate(nodePrefab);
                middleNode.transform.position = nodeSpawnPoint;
            }
        }
       
    }
    private void Start()
    {
        StartCoroutine(makeAllNodeConnections(tagNodes));//makes all the node connections after spawning all of them in
    }
    public IEnumerator makeAllNodeConnections(string nodeTag)
    {
        //makes all gameObjects with the nodeTag tag start
        nodes = GameObject.FindGameObjectsWithTag(nodeTag);
        foreach (GameObject node in nodes)
        {
            Node nodeSetup = node.GetComponent<Node>();
            StartCoroutine(nodeSetup.setUpConnections(nodeDistance, 1<<LayerMask.NameToLayer("node"), nodes));//hopefully this works
            nodeSetup.connectionsSetUp = true;
        }
        yield return null;
    }
}
