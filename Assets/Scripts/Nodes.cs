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

	public class Connection
	{
		public int x;//The relPos from the main node
		public int y;
		public int z;

		public int x2;//The relPos from the main node, for the second node
		public int y2;
		public int z2;

		public Node n1;
		public Node n2;
		public Runes.NodeTags tag = Runes.NodeTags.None;

		public Lines.Line line;//The line that connects these 2 nodes
	}

	public class RelCon
	{
		public int x;
		public int y;
		public int z;
		public Node node;
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
		public Runes.Rune rune;
		public bool usedByRune = false;
		public List<Lines.Line> lines;
		public SpecialNodes specialType = SpecialNodes.None;
		public Manas.Mana mana;//The mana that is using this spot
		public List<Node> connections;
		public GridQuad grid;
		public List<RelCon> relCons = new List<RelCon>();//
		public Runes.NodeTags tag = Runes.NodeTags.None;//A temporary holding place for a tag when a rune is created. Can be overwritten. Look at Runes.Rune.NodeAndTag.tag for the permant storage of tags
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
						nodes[a].connections.Add(nodes[b]);
						nodes[b].connections.Add(nodes[a]);
					}
				}
			}
		}

		//Fill relCon grid
		for (int a = 0; a < nodes.Count; a++)
		{
			for (int b = 0; b < nodes[a].connections.Count; b++)
			{
				//Loop through all of this node's connections, and store their relCon dist
				Vector3 pos = nodes[a].connections[b].pos;
				Vector3 relPos = pos - nodes[a].pos;
				//Debug.Log("NodePos: " + nodes[a].pos + ", NodeB: " + nodes[a].connections[b].pos + ", RelPos: " + relPos);

				RelCon relCon = new RelCon();
				relCon.x = Mathf.RoundToInt(relPos.x * xa.gridScale);
				relCon.y = Mathf.RoundToInt(relPos.y * xa.gridScale);//So I can store & check the relPos as ints, not floats
				relCon.z = Mathf.RoundToInt(relPos.z * xa.gridScale);
				relCon.node = nodes[a].connections[b];
				nodes[a].relCons.Add(relCon);
				//	nodes[a].relCon
			}
		}
	}

	public static Connection Tag(int x, int y, int z, Runes.NodeTags tag)
	{
		Connection c = new Connection();
		c.x = x;
		c.y = y;
		c.z = z;
		c.tag = tag;
		return c;
	}

	public static Connection Con(int x, int y, int z, int x2, int y2, int z2)
	{
		Connection c = new Connection();
		c.x = x;
		c.y = y;
		c.z = z;
		c.x2 = x2;
		c.y2 = y2;
		c.z2 = z2;
		return c;
	}

	public static List<Connection> CheckRuneMulti(List<Connection> connections, Node node)
	{
		List<Connection> result = null;

		int[] resetX = new int[connections.Count];
		int[] resetY = new int[connections.Count];
		int[] resetZ = new int[connections.Count];
		int[] resetX2 = new int[connections.Count];
		int[] resetY2 = new int[connections.Count];
		int[] resetZ2 = new int[connections.Count];

		for (int i = 0; i < connections.Count; i++)
		{
			resetX[i] = connections[i].x;
			resetY[i] = connections[i].y;
			resetZ[i] = connections[i].z;
			resetX2[i] = connections[i].x2;
			resetY2[i] = connections[i].y2;
			resetZ2[i] = connections[i].z2;
		}

		//A1
		result = CheckRune(connections, node);//A1

		//A2
		if (result == null)
		{
			for (int i = 0; i < connections.Count; i++)
			{
				connections[i].x = resetZ[i];
				connections[i].y = resetY[i];
				connections[i].z = -resetX[i];

				connections[i].x2 = resetZ2[i];
				connections[i].y2 = resetY2[i];
				connections[i].z2 = -resetX2[i];
			}

			result = CheckRune(connections, node);
		}

		//A3
		if (result == null)
		{
			for (int i = 0; i < connections.Count; i++)
			{
				connections[i].x = -resetX[i];
				connections[i].y = resetY[i];
				connections[i].z = -resetZ[i];

				connections[i].x2 = -resetX2[i];
				connections[i].y2 = resetY2[i];
				connections[i].z2 = -resetZ2[i];
			}
			result = CheckRune(connections, node);
		}

		//A4
		if (result == null)
		{
			for (int i = 0; i < connections.Count; i++)
			{
				connections[i].x = -resetZ[i];
				connections[i].y = resetY[i];
				connections[i].z = resetX[i];

				connections[i].x2 = -resetZ2[i];
				connections[i].y2 = resetY2[i];
				connections[i].z2 = resetX2[i];
			}
			result = CheckRune(connections, node);
		}

		//B1
		if (result == null)
		{
			for (int i = 0; i < connections.Count; i++)
			{
				connections[i].x = -resetX[i];
				connections[i].y = resetY[i];
				connections[i].z = resetZ[i];

				connections[i].x2 = -resetX2[i];
				connections[i].y2 = resetY2[i];
				connections[i].z2 = resetZ2[i];
			}
			result = CheckRune(connections, node);
		}

		//B2
		if (result == null)
		{
			for (int i = 0; i < connections.Count; i++)
			{
				connections[i].x = resetZ[i];
				connections[i].y = resetY[i];
				connections[i].z = resetX[i];

				connections[i].x2 = resetZ2[i];
				connections[i].y2 = resetY2[i];
				connections[i].z2 = resetX2[i];
			}
			result = CheckRune(connections, node);
		}

		//B3
		if (result == null)
		{
			for (int i = 0; i < connections.Count; i++)
			{
				connections[i].x = resetX[i];
				connections[i].y = resetY[i];
				connections[i].z = -resetZ[i];

				connections[i].x2 = resetX2[i];
				connections[i].y2 = resetY2[i];
				connections[i].z2 = -resetZ2[i];
			}
			result = CheckRune(connections, node);
		}

		//B4
		if (result == null)
		{
			for (int i = 0; i < connections.Count; i++)
			{
				connections[i].x = -resetZ[i];
				connections[i].y = resetY[i];
				connections[i].z = -resetX[i];

				connections[i].x2 = -resetZ2[i];
				connections[i].y2 = resetY2[i];
				connections[i].z2 = -resetX2[i];
			}
			result = CheckRune(connections, node);
		}



		return result;
	}

	public static List<Connection> CheckRune(List<Connection> connections, Node node)
	{
		List<Connection> result = new List<Connection>();

		//Check that all connections on list are matched to RelCons that this node has.
		//(Can't have a node that larger then it's center node's connections)
		for (int a = 0; a < connections.Count; a++)
		{
			if (connections[a].tag != Runes.NodeTags.None) { continue; }//Skip connections that are just tags
			bool found = false;
			//Loop through all of this node's connections
			for (int b = 0; b < node.relCons.Count; b++)
			{
				if (connections[a].x == node.relCons[b].x &&
					connections[a].y == node.relCons[b].y &&
					connections[a].z == node.relCons[b].z)
				{
					found = true;
					Connection c = new Connection();
					c.x = connections[a].x;
					c.y = connections[a].y;
					c.z = connections[a].z;
					c.x2 = connections[a].x2;
					c.y2 = connections[a].y2;
					c.z2 = connections[a].z2;

					result.Add(c);//add it to the list
					break;//Stop looking, we've found this one
						  //Debug.DrawLine(Nodes.nodes[i].pos, Nodes.nodes[i].relCons[a].node.pos, Color.blue);
				}
			}
			if (!found)
			{
				//Failed. Return nothing.
				//s += "Failed because I didn't find one of the required relCon nodes. " + connections[a].x + ", " + connections[a].y + ", " + connections[a].z + ", Vec2: " + connections[a].x2 + ", " + connections[a].y2 + ", " + connections[a].z2;
				//Debug.Log(s);
				return null;
			}
		}

		//Now make sure that all of these connections are connected to the ones that they say they should be
		//Just as relCons for now, later as lines as well

		//Find the nodes for the result list
		for (int a = 0; a < result.Count; a++)
		{
			//Find the first connection
			for (int b = 0; b < node.relCons.Count; b++)
			{
				if (result[a].x == 0 &&
					result[a].y == 0 &&
					result[a].z == 0)
				{
					result[a].n1 = node;
				}
				else if (result[a].x == node.relCons[b].x &&
					result[a].y == node.relCons[b].y &&
					result[a].z == node.relCons[b].z)
				{
					result[a].n1 = node.relCons[b].node;
				}

				if (result[a].x2 == 0 &&
					result[a].y2 == 0 &&
					result[a].z2 == 0)
				{
					result[a].n2 = node;
				}
				else if (result[a].x2 == node.relCons[b].x &&
					result[a].y2 == node.relCons[b].y &&
					result[a].z2 == node.relCons[b].z)
				{
					result[a].n2 = node.relCons[b].node;
				}
			}
		}

		//Ok, now check all of these connections have lines, and have nodes/all info needed.

		//Find the nodes for the result list
		for (int a = 0; a < result.Count; a++)
		{
			//If for whatever reason, any of the results don't have a node, then fail.
			if (result[a].n1 == null || result[a].n2 == null)
			{
				//s += "Failed because n1 or n2 was null";
				//Debug.Log(s);
				return null;
			}

			//Check that this connection exists as a line
			for (int i = 0; i < result[a].n1.lines.Count; i++)
			{
				for (int b = 0; b < result[a].n1.lines[i].points.Count; b++)
				{
					//Does n1 have a line that connects to n2?
					if (result[a].n1.lines[i].points[b].node.uId == result[a].n2.uId)
					{
						//If yes, then let's store this line
						result[a].line = result[a].n1.lines[i];
					}
				}
			}
			if (result[a].line == null)
			{
				//s += "Failed because a required line didn't exist";
				//Debug.Log(s);
				return null;
			}//Couldn't find a line that connects these 2 nodes
		}


		//Check none of the lines or nodes are used in another rune
		//go through all lines in this rune
		for (int i = 0; i < result.Count; i++)
		{
			if (result[i].n1.usedByRune) { /*s += "Failed because n1 was used in another rune"; Debug.Log(s); */return null; }
			if (result[i].n2.usedByRune) { /*s += "Failed because n2 was used in another rune"; Debug.Log(s);*/ return null; }
			if (result[i].line.rune != null) {/* s += "Failed because line was used in another rune"; Debug.Log(s);*/ return null; }
		}
		/*
		//Print result
		string s = "connections printout: \n";
		for (int i = 0; i < connections.Count; i++)//Add all the nodes to rune.nodes
		{
			s += "\n" + i;
			s += ", x: " + connections[i].x;
			s += ", y: " + connections[i].y;
			s += ", z: " + connections[i].z;
			s += ", x2: " + connections[i].x2;
			s += ", y2: " + connections[i].y2;
			s += ", z2: " + connections[i].z2;
			s += ", n1: " + (connections[i].n1 != null);
			s += ", n2: " + (connections[i].n2 != null);
			s += ", line: " + (connections[i].line != null);
			s += ", tag: " + connections[i].tag;
		}
		Debug.Log(s);
			*/
		//Loop through the connections and set the node's tags
		for (int a = 0; a < connections.Count; a++)
		{
			if (connections[a].tag == Runes.NodeTags.None) { continue; }//This isn't a tagging

			//Ok, this is a tagged node.

			//Loop through all of this node's relCon connections
			for (int b = 0; b < node.relCons.Count; b++)
			{
				Node foundNode = null;
				if (connections[a].x == 0 &&
					connections[a].y == 0 &&
					connections[a].z == 0)
				{
					node.tag = connections[a].tag;
					foundNode = node;
				}
				else if (connections[a].x == node.relCons[b].x &&
					connections[a].y == node.relCons[b].y &&
					connections[a].z == node.relCons[b].z)
				{
					node.relCons[b].node.tag = connections[a].tag;
					foundNode = node.relCons[b].node;
				}
				
				//Alright. I've tagged this node. Now is it on the result list?
				if (foundNode != null)
				{
					//Debug.Log("FoundNode: " + foundNode.uId + ", " + foundNode.tag);
					//Check if this node is already on the result list
					bool foundOnList = false;
					for(int c = 0;c < result.Count;c ++)
					{
						if(result[c].n1 != null) { if(result[c].n1.uId == foundNode.uId) {foundOnList = true;break; } }
						if(result[c].n2 != null) { if(result[c].n2.uId == foundNode.uId) {foundOnList = true;break;} }
					}
					if(!foundOnList)
					{
						//Isn't a line-connected node, so we have to add it manually to the list
						Connection c = new Connection();
						c.n1 = foundNode;
						c.tag = foundNode.tag;//Just to make this entry have a unique thing so I can check for it
						result.Add(c);//add it to the list
					}
					break;//Stop looking, we found one
				}
			}
		}

		//Debug.Log(s);
		return result;
	}

	public static Node FindNearestNode(Vector3 pos)
	{
		float nearest = 9999;
		Node result = null;
		for (int i = 0; i < nodes.Count; i++)
		{
			float dist = Vector3.Distance(nodes[i].pos, pos);
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
		//Debug.Log("Nodes count: " + nodes.Count);
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
		if (node.specialType == SpecialNodes.ManaFountain ||
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
