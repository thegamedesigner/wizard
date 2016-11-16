using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Nodes : MonoBehaviour
{
	public static List<Nodes.Node> nodes;
	public static List<Nodes.GridQuad> grids;
	public static List<GameObject> iconPool;
	public static float connectionDist = 2;

	public enum SpecialNodes
	{
		None,
		ManaFountain,
		Blocked,//Rocks or some such shit
		End
	}

	public class GridQuad
	{
		public int uId = -1;
		public GameObject go;
		public Vector3 scale;
	}

	public class Node
	{
		//A node is a rounded position, that lines register with when they use it as a start/end point
		//It doesn't exist by default, but once at least one line uses it.
		public int uId = -1;
		public Vector3 pos;
		public Vector3 angles;
		public List<Lines.Line> lines;
		public SpecialNodes specialType = SpecialNodes.None;
		public Manas.Mana mana;//The mana that is using this spot
		public List<Node> connections;
		public GridQuad grid;
	}

	public static void InitNodes()
	{
		nodes = new List<Node>();
		grids = new List<GridQuad>();

		//Find gridQuads
		GameObject[] objsWithTag = GameObject.FindGameObjectsWithTag("GridQuad");
		for (int i = 0; i < objsWithTag.Length; i++)
		{
			GridQuad grid = new GridQuad();
			grid.go = objsWithTag[i];
			grid.scale = objsWithTag[i].GetComponentInChildren<GridQuadScript>().GetScale();
			xa.uIds++;
			grid.uId = xa.uIds;
			grids.Add(grid);
		}

		//Create nodes for all gridQuads
		for (int i = 0; i < grids.Count; i++)
		{
			xa.emptyGO.transform.parent = grids[i].go.transform;
			xa.emptyGO.transform.localPosition = Vector3.zero;
			xa.emptyGO.transform.localEulerAngles = Vector3.zero;

			xa.emptyGO.transform.localPosition = new Vector3(-grids[i].scale.x * 0.5f, 0, -grids[i].scale.z * 0.5f);

			Debug.Log("Starting grid. " + grids[i].go.transform.position + ", " + xa.emptyGO.transform.localPosition);

			//Now create the nodes
			int xScale = (int)grids[i].scale.x;
			int zScale = (int)grids[i].scale.z;
			for (float x = 0; x <= xScale; x += (1 / xa.gridScale))
			{
				for (float z = 0; z <= zScale; z += (1 / xa.gridScale))
				{
					Vector3 spawnPos = xa.emptyGO.transform.position;
					
					xa.emptyGO.transform.LocalSetY(0.02f);
					spawnPos = xa.emptyGO.transform.position;
					CreateNode(spawnPos, grids[i].go.GetComponent<Info>().lineAngle, grids[i]);
					
					xa.emptyGO.transform.LocalSetY(0);
					xa.emptyGO.transform.LocalAddZ((1 / xa.gridScale));
				}
				xa.emptyGO.transform.LocalSetZ(-grids[i].scale.z * 0.5f);
				xa.emptyGO.transform.LocalAddX((1 / xa.gridScale));
			}
		}

		//Connect all nodes within X dist of each other
		for (int a = 0; a < nodes.Count; a++)
		{
			//do a distance check with all other nodes
			for (int b = 0; b < nodes.Count; b++)
			{
				if (nodes[a].uId != nodes[b].uId)
				{
					//do a distance check
					if (Vector3.Distance(nodes[a].pos, nodes[b].pos) < connectionDist)
					{
						//Then connect them
						//Debug.Log("Nodes[a].uid: " + nodes[a].uId + ", Nodes[b].uid: " + nodes[b].uId);
						//Debug.DrawLine(nodes[a].pos, new Vector3(0,2,0), Color.magenta, 100);
						//Debug.DrawLine(nodes[b].pos, new Vector3(0,2,0), Color.magenta, 100);
						//Debug.Log("Nodes[a].connections: " + (nodes[a].connections != null));
						//Debug.Log("Nodes[b].connections: " + (nodes[b].connections != null));
						nodes[a].connections.Add(nodes[b]);
						nodes[b].connections.Add(nodes[a]);
					}
				}
			}
		}
	}

	public static Node FindNearestNode(Vector3 pos)
	{
		float nearest = 9999;
		Node result = null;
		for (int i = 0; i < nodes.Count; i++)
		{
			float dist = Vector3.Distance(nodes[i].pos, pos) ;
			if (dist < nearest)
			{
				nearest = dist;
				result = nodes[i];
			}
		}
		return result;
	}

	public static void DisplayNodeDebug()
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			//Debug.DrawLine(nodes[i].pos, new Vector3(0, 2, 0), Color.magenta);
			//Show the connections of this node
			//for (int a = 0; a < Nodes.nodes[i].connections.Count; a++)
			//{
			//Debug.DrawLine(Nodes.nodes[i].pos,Nodes.nodes[i].connections[a].pos,Color.blue);
			//}
		}
	}

	public static Node CreateNode(Vector3 pos, Vector3 angles, GridQuad grid)//Creates a basic node where none was
	{
		//Add node to list
		Node node = new Node();
		node.pos = pos;
		xa.uIds++;
		node.uId = xa.uIds;
		node.lines = new List<Lines.Line>();
		node.connections = new List<Node>();
		node.angles = angles;
		node.grid = grid;

		GameObject go = Instantiate(Defines.self.gridNodePrefab);
		go.transform.position = pos;

		//Debug.DrawLine(pos, new Vector3(0, 2, 0), Color.blue, 100);
		nodes.Add(node);
		Debug.Log("Nodes count: " + nodes.Count);
		return node;
	}
	

	public static void RegisterSpecialNode(Node node, SpecialNodes type)
	{
		node.specialType = type;
	}

	public static bool CheckConnection(Node n1, Node n2)
	{
		if (n1 == null || n2 == null) { return false; }

		//Do they have a matching line uId?
		for (int a = 0; a < n1.lines.Count; a++)
		{
			for (int b = 0; b < n2.lines.Count; b++)
			{
				if (n1.lines[a].rune == null)//Make sure this line isn't used in another rune
				{
					if (n1.lines[a].uId == n2.lines[b].uId)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static Lines.Line GetConnection(Node n1, Node n2)
	{
		if (n1 == null || n2 == null) { return null; }

		//Do they have a matching line uId?
		for (int a = 0; a < n1.lines.Count; a++)
		{
			for (int b = 0; b < n2.lines.Count; b++)
			{
				if (n1.lines[a].uId == n2.lines[b].uId)
				{
					return n1.lines[a];
				}
			}
		}
		return null;
	}

	public static bool CheckValidNode(Node node)
	{
		if(node.specialType == SpecialNodes.ManaFountain ||
			node.specialType == SpecialNodes.Blocked)
		{
			return false;
		}

		return true;
	}
	/*
	public static bool CheckNodePos(Vector3 pos)
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			if (nodes[i].pos == pos)
			{
				return true;
			}
		}
		return false;
	}

	public static int GetIndexForNodePos(Vector3 pos)
	{
		for (int i = 0; i < nodes.Count; i++)
		{
			if (nodes[i].pos == pos)
			{
				return i;
			}
		}
		return -1;
	}*/

}
