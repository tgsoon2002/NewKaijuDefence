using UnityEngine;
using System.Collections;
using System.Linq;
using System.Collections.Generic;

/// <summary>
/// Customized Graph data structure for this game.
/// </summary>
/// <typeparam name="T"></typeparam>
public class GraphADT
{
    #region Data Members

    private Dictionary<int, Vertex> graph;

    #endregion

    #region Setters & Getters

    public int Get_Graph_Size
    {
        get { return graph.Count; }
    }

    public int[] Get_Vertex_ID_List
    {
        get 
        {
            return graph.Keys.ToArray();
        }
    }

    #endregion

    #region Helper Classes

    /// <summary>
    /// This class will serve as a 'vertex' 
    /// representation of a graph data structure
    /// </summary>
    private class Vertex
    {
        public int nodeID;
        public string nodeName;
        public Vector3 objectLocation;
        public float objectHealth;
        public Dictionary<int, bool> nodeNeighbors;

        /// <summary>
        /// Class constructor
        /// </summary>
        /// <param name="id"></param>
        /// <param name="pos"></param>
        /// <param name="health"></param>
        public Vertex(int id, string name, Vector3 pos, float health)
        {
            nodeID = id;
            nodeName = name;
            objectLocation = pos;
            objectHealth = health;
            nodeNeighbors = new Dictionary<int, bool>();
        }

        /// <summary>
        /// Add a neighboring vertex 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="val"></param>
        public void AddNeighbor(int id, bool val)
        {
            if(!nodeNeighbors.ContainsKey(id))
            {
                nodeNeighbors.Add(id, val);
            }
        }

        /// <summary>
        /// Removes this Vertex's neighbors
        /// </summary>
        /// <param name="id"></param>
        public void RemoveNeighbor(int id)
        {
            if(!nodeNeighbors.ContainsKey(id))
            {
                nodeNeighbors.Remove(id);
            }
        }
    }

    #endregion

    #region Main Methods

    /// <summary>
    /// Class constructor
    /// </summary>
    public GraphADT()
    {
        graph = new Dictionary<int, Vertex>();
    }

    /// <summary>
    /// Adds a vertex to the graph
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pos"></param>
    /// <param name="health"></param>
    public void AddNode(int id, string name, Vector3 pos, float health)
    {
        //Declaring local variables
        Vertex tmp;

        //Instantiating a Vertex object.
        tmp = new Vertex(id, name, pos, health);

        //Adding the vertex to our graph.
        graph.Add(id, tmp); 
    }

    /// <summary>
    /// Removing a 
    /// </summary>
    /// <param name="id"></param>
    public void RemoveVertex(int id)
    {
        //First check if the node exists in the graph G
        if(graph.ContainsKey(id))
        {
            //Then get all keys (neighbors) of this node and store
            //it into an array
            var neighborKeys = graph[id].nodeNeighbors.Keys.ToArray();

            //Iterate through this array of keys
            for(int i = 0; i < neighborKeys.Length; i++)
            {
                //Have its neighbors first delete any references to the
                //node we want to delete.
                graph[neighborKeys[i]].nodeNeighbors.Remove(id);
            }

            //Now that's done, finally clear the list of neighbors the
            //selected node contains
            graph[id].nodeNeighbors.Clear();

            //Now, finally remove the node from our graph (dictionary)
            graph.Remove(id);
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void AddUndirectedEdge(int from, int to)
    {
        //Add a 'passable' edge from source node to destination node
        graph[from].AddNeighbor(to, true);

        //Add a 'passable' edge from destination node to source node
        graph[to].AddNeighbor(from, true);      
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="from"></param>
    /// <param name="to"></param>
    public void AddDirectedEdge(int from, int to)
    {
        //Add a 'passable' edge from source node to destination node
        graph[from].AddNeighbor(to, true);

        //Add a 'non-passable' edge from destination node to source node
        graph[to].AddNeighbor(from, false);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    public void RemoveEdgeAtNode(int from, int to)
    {
        //Remove the edge from source node to destination node.
        graph[from].RemoveNeighbor(to);

        //Remove the edge from destination node to source node.
        graph[to].RemoveNeighbor(from);
    }

    /// <summary>
    /// Sets the health value of the specified vertex id.
    /// </summary>
    /// <param name="id"></param>
    /// <param name="health"></param>
    public void SetVertexHealth(int id, float health)
    {
        if(graph.ContainsKey(id))
        {
            graph[id].objectHealth = health;
        }
        else
        {
            Debug.Log("City ID does not exist!");
        }
    }

    public string GetVertexName(int id)
    {
        return graph[id].nodeName;
    }

    public float GetVertexHealth(int id)
    {
        return graph[id].objectHealth;
    }

    public Vector3 GetVertexPosition(int id)
    {
        return graph[id].objectLocation;
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <param name="pos"></param>
    /// <param name="health"></param>
    /// <param name="neighbors"></param>
    public void GetNeighborList(int id, List<int> neighbors)
    {
        //Checks if the key is in the graph
        if(graph.ContainsKey(id))
        {
            //Checks if the specified Vertex even has neighbors
            if(graph[id].nodeNeighbors.Count > 0)
            {
                //Retrieving the dictionary keys associated with the 
                //neighbors dictionary and store it into an array.
                var tmp = graph[id].nodeNeighbors.Keys.ToArray();

                //Iterates through the array of dicitionary keys.
                for(int i = 0; i < tmp.Length; i++)
                {
                    //Finally adding the unique ID of the neighbors.
                    neighbors.Add(tmp[i]);
                }
            }
        }
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="id"></param>
    /// <returns="result"></returns>
    public bool DoesVertexHaveNeighbors(int id)
    {
        //Declaring local variables
        bool result;

        result = (graph[id].nodeNeighbors.Count > 0);
        
        return result;
    }

    #endregion
}