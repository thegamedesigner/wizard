using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Nodes : MonoBehaviour
{
	public static List<Nodes.Node> nodes;
	public static List<Nodes.GridQuad> grids;
	public static List<GameObject> iconPool;

	public enum SpecialNodes
	{
		None,
		ManaFountain,
		Blocked,//Rocks or some shit
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
		public Vector3 pos;
		public List<Lines.Line> lines;
		public SpecialNodes specialType = SpecialNodes.None;
		public Manas.Mana mana;//The mana that is using this spot
		public int uId = -1;
		public Node[] connections;
	}

	public static void InitNodes()
	{
		nodes = new List<Node>();
		grids = new List<GridQuad>();

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
			xa.emptyGO.transform.localPosition = new Vector3(-grids[i].scale.x * 0.5f,0,-grids[i].scale.z * 0.5f);

			Debug.Log("Starting grid. " + grids[i].go.transform.position + ", " + xa.emptyGO.transform.localPosition);

			//Now create the nodes
			int xScale = (int)grids[i].scale.x;
			int zScale = (int)grids[i].scale.z;
			for (float x = 0; x <= xScale; x += (1 / xa.gridScale))
			{
				for (float z = 0; z <= zScale; z += (1 / xa.gridScale))
				{
					CreateNode(xa.emptyGO.transform.position);
					xa.emptyGO.transform.LocalAddZ((1/ xa.gridScale));
				}
				xa.emptyGO.transform.LocalSetZ(-grids[i].scale.z * 0.5f);
				xa.emptyGO.transform.LocalAddX((1/ xa.gridScale));
			}

		}
	}

	public static void InitNodeIconPool()
	{
		iconPool = new List<GameObject>();
		for (int i = 0; i < 10000; i++)
		{
			GameObject go = (GameObject)Instantiate(Defines.self.gridNodePrefab);
			go.transform.position = new Vector3(999, 999, 999);
			iconPool.Add(go);
		}
	}


	public static int CreateNode(Vector3 pos)//Creates a basic node where none was
	{
		//Add node to list
		Nodes.Node node = new Nodes.Node();
		node.pos = pos;
		xa.uIds++;
		node.uId = xa.uIds;
		node.lines = new List<Lines.Line>();

		GameObject go = (GameObject)Instantiate(Defines.self.gridNodePrefab);
		go.transform.position = pos;

		nodes.Add(node);
		return nodes.Count - 1;
	}

	public static Node RegisterAtNode(Vector3 pos, int lineIndex)
	{
		int nodeIndex = GetIndexForNodePos(pos);
		if (nodeIndex == -1)//Whoops, no node for the pos exists yet. Create it.
		{
			//Add node to list
			nodeIndex = CreateNode(pos);
		}
		nodes[nodeIndex].lines.Add(Lines.lines[lineIndex]);
		return nodes[nodeIndex];
	}

	public static void RegisterSpecialNode(Vector3 pos, SpecialNodes type)
	{
		int nodeIndex = GetIndexForNodePos(pos);
		if (nodeIndex == -1)//Whoops, no node for the pos exists yet. Create it.
		{
			//Add node to list
			nodeIndex = CreateNode(pos);
		}
		nodes[nodeIndex].specialType = type;
	}

	public static bool CheckConnection(Vector3 v1, Vector3 v2)
	{ return CheckConnection(Nodes.GetIndexForNodePos(v1), v2); }

	public static bool CheckConnection(int n1, Vector3 v2)
	{
		int n2 = Nodes.GetIndexForNodePos(v2);
		if (n1 == -1 || n2 == -1) { return false; }

		//Do they have a matching line uId?
		for (int a = 0; a < nodes[n1].lines.Count; a++)
		{
			for (int b = 0; b < nodes[n2].lines.Count; b++)
			{
				if (nodes[n1].lines[a].rune == null)//Make sure this line isn't used in another rune
				{
					if (nodes[n1].lines[a].uId == nodes[n2].lines[b].uId)
					{
						return true;
					}
				}
			}
		}
		return false;
	}

	public static Lines.Line GetConnection(Vector3 v1, Vector3 v2)//Returns the line connecting these 2 points
	{
		int n1 = Nodes.GetIndexForNodePos(v1);
		int n2 = Nodes.GetIndexForNodePos(v2);
		if (n1 == -1 || n2 == -1) { return null; }

		//Do they have a matching line uId?
		for (int a = 0; a < nodes[n1].lines.Count; a++)
		{
			for (int b = 0; b < nodes[n2].lines.Count; b++)
			{
				if (nodes[n1].lines[a].uId == nodes[n2].lines[b].uId)
				{
					return nodes[n1].lines[a];
				}
			}
		}
		return null;
	}


	public static void UpdateGridPool()
	{
		/*
		//Get the player's pos
		Vector3 center = RoundPos(Player.playerPos);
		center.y = 0.515f;

		//subtract 5
		Vector3 corner = new Vector3(center.x - (50 / gridScale), center.y, center.z - (50 / gridScale));

		int i = 0;
		for (int x = 0; x < 100; x++)
		{
			for (int z = 0; z < 100; z++)
			{
				float x2 = x / gridScale;
				float z2 = z / gridScale;
				gridPool[i].transform.position = new Vector3(x2 + corner.x, center.y, z2 + corner.z);
				i++;
			}
		}
		*/
	}

	public static bool CheckValidDrawLinePos(Vector3 pos)
	{
		//Is this position a place where a draw line can happen?

		int index = GetIndexForNodePos(pos);
		if (index != -1)
		{
			if (nodes[index].specialType == SpecialNodes.ManaFountain ||
			nodes[index].specialType == SpecialNodes.ManaFountain)
			{
				return false;
			}
		}
		return true;
	}

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
	}

}
