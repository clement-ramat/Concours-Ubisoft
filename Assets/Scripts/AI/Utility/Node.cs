using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : MonoBehaviour
{
    public List<Edge> edges;
    public bool desirable = true;
    public int id;
    public bool explored = false;
}

public class Graph
{
    public List<Node> nodes;
}