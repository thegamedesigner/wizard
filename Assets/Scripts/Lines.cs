using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Lines : MonoBehaviour
{
	public static List<Line> lines;

	public class Line
	{
		public List<Point> points;
		public int uId = -1;
		public bool dead = false;
		public Runes.Rune rune;
	}

	public class Point
	{
		public Vector3 pos;
		public GameObject linePrefab;
		public Nodes.Node node;
	}

	public static int RegisterNewLine()
	{
		Line line = new Line();
		xa.uIds++;
		line.uId = xa.uIds;
		line.points = new List<Point>();
		lines.Add(line);
		return line.uId;
	}
	/*
	public static Vector3 RoundPos(Vector3 pos)
	{
		//Round to grid
		pos *= xa.gridScale;
		pos.x = Mathf.RoundToInt(pos.x);
		pos.y = Mathf.RoundToInt(pos.y);
		pos.z = Mathf.RoundToInt(pos.z);
		pos /= xa.gridScale;

		return pos;
	}
	*/
	public static Vector3 AddPoint(Nodes.Node n1, int uId)
	{
		int i = GetLineForId(uId);

		Point point = new Point();
		point.pos = n1.pos;

		if (lines[i].points.Count == 0)//Is the first point in the line
		{
			//This is the first point, so register it with the node

			n1.lines.Add(lines[i]);//Add this line to the node's list of lines
			point.node = n1;//Add this node to the point's pointer to the node it's on
		}
		else
		{
			n1.lines.Add(lines[i]);//Add this line to the node's list of lines
			point.node = n1;//Add this node to the point's pointer to the node it's on

			//Is the second point in the line
			Nodes.Node lastNode = lines[i].points[lines[i].points.Count - 1].node;
			
			point.linePrefab = CreateLinePrefab(lastNode, n1, Defines.self.lineColor);

			//Is the last point on this line (probably, now that lines have just 2 points)
		}

		lines[i].points.Add(point);

		return n1.pos;
	}

	public static GameObject CreateLinePrefab(Nodes.Node n1, Nodes.Node n2, Color c1)
	{
		GameObject go = (GameObject)Instantiate(Defines.self.linePrefab, Vector3.zero, new Quaternion(0, 0, 0, 0));
		go.transform.position = n1.pos;
		go.transform.LookAt(n2.pos);
		float dist = Vector3.Distance(n1.pos, n2.pos);
		go.transform.Translate(0, 0, dist * 0.5f);
		go.transform.localScale = new Vector3(1, 1, dist);
		go.transform.localEulerAngles += n1.angles;
		//go.transform.localEulerAngles = new Vector3(go.transform.localEulerAngles.x,go.transform.localEulerAngles.y,n1.angles.z);

		//Assuming it's not a special kind of line, make it the "not yet a rune" color
		SetRendererColor(go.GetComponentInChildren<MeshRenderer>(), Defines.self.lineColor);

		return go;
	}

	public static void SetRendererColor(Renderer renderer, Color c1)
	{
		MaterialPropertyBlock block = new MaterialPropertyBlock();
		block.SetColor("_Color", c1);
		renderer.SetPropertyBlock(block);
	}

	public static void SetLineColor(int uId, Color c1)
	{
		int i = GetLineForId(uId);
		if (i == -1) { Debug.Log("Didn't find line in SetLineColor"); }
		MaterialPropertyBlock block = new MaterialPropertyBlock();
		block.SetColor("_Color", c1);

		for (int a = 0; a < lines[i].points.Count; a++)
		{
			//Set color
			if (lines[i].points[a].linePrefab != null)
			{
				MeshRenderer renderer = lines[i].points[a].linePrefab.GetComponentInChildren<MeshRenderer>();
				renderer.SetPropertyBlock(block);
			}
		}
	}

	public static void SetLineMat(int uId, Material mat)
	{
		int i = GetLineForId(uId);

		for (int a = 0; a < lines[i].points.Count; a++)
		{
			//Set mat
			if (lines[i].points[a].linePrefab != null)
			{
				MeshRenderer renderer = lines[i].points[a].linePrefab.GetComponentInChildren<MeshRenderer>();
				renderer.material = mat;

			}
		}
	}

	public static int GetLineForId(int uId)
	{
		for (int i = 0; i < lines.Count; i++)
		{
			if (lines[i].uId == uId)
			{
				return i;
			}
		}
		return -1;
	}

	public static void DeleteLine(int uId)
	{
		int index = GetLineForId(uId);

		//break any runes that use this line
		Runes.Rune rune = lines[index].rune;
		if (rune != null)
		{
			rune.SetRuneColor(Defines.self.lineColor);

			//go through all the lines in this rune
			for (int a = 0; a < rune.lines.Count; a++)
			{
				rune.lines[a].rune = null;

				//go through all nodes used by this line
			}

			
			//go through all lines in this rune
			for (int i = 0; i < rune.lines.Count; i++)
			{
				rune.lines[i].rune = null;

				//go through all nodes used by this line
				for (int a = 0; a < rune.lines[i].points.Count; a++)
				{
					rune.lines[i].points[a].node.rune = null;
					rune.lines[i].points[a].node.usedByRune = false;
				}
			}

			rune.dead = true;
		}

		//Remove reference from any nodes
		for (int a = 0; a < lines[index].points.Count; a++)
		{
			if (lines[index].points[a].node != null)
			{
				//Find this line in this node
				for (int b = 0; b < lines[index].points[a].node.lines.Count; b++)
				{
					if (lines[index].points[a].node.lines[b].uId == lines[index].uId)
					{
						lines[index].points[a].node.lines.RemoveAt(b);
						break;
					}
				}
			}
		}

		lines[index].dead = true;
		for (int a = 0; a < lines[index].points.Count; a++)
		{
			Destroy(lines[index].points[a].linePrefab);
		}
	}

	public static void ClearTheDead()
	{
		//Clear out dead runes
		for (int a = 0; a < 100; a++)
		{
			bool foundOne = false;
			for (int i = 0; i < Runes.runes.Count; i++)
			{
				if (Runes.runes[i].dead)
				{
					foundOne = true;
					Runes.runes.RemoveAt(i);
					break;
				}

			}
			if (!foundOne)
			{
				break;
			}
		}

		//Clear out dead lines
		for (int a = 0; a < 100; a++)
		{
			bool foundOne = false;
			for (int i = 0; i < lines.Count; i++)
			{
				if (lines[i].dead)
				{
					foundOne = true;
					lines.RemoveAt(i);
					break;
				}

			}
			if (!foundOne)
			{
				break;
			}
		}
	}

	public static void InitLines()
	{
		lines = new List<Line>();
	}

	public static void UpdateLines()
	{
		//Go through all nodes
		Runes.LookForRunes();
	}

	public static void PrintInfo()
	{
		string s = "";

		s += "Lines:\n";
		for (int i = 0; i < lines.Count; i++)
		{
			s += "Line " + lines[i].uId + ", Dead: " + lines[i].dead + ", Rune: " + (lines[i].rune != null);
			if (lines[i].points.Count >= 2)
			{
				s += ", p0: " + lines[i].points[0].pos + ", p1: " + lines[i].points[1].pos;
			}
			else
			{
				s += ", Points.Count: " + lines[i].points.Count;
			}
			s += "\n";
		}

		s += "\n\nRunes:\n";
		for (int i = 0; i < Runes.runes.Count; i++)
		{
			s += "Rune: Dead: " + Runes.runes[i].dead;
			s += " - Lines: ";
			for (int a = 0; a < Runes.runes[i].lines.Count; a++)
			{
				s += Runes.runes[i].lines[a].uId + ", ";
			}
			s += "\n";
		}
		
		s += "\n\nGrids:\n";
		for (int i = 0; i < Nodes.grids.Count; i++)
		{
			s += "Grid: " + Nodes.grids[i].uId + ", Pos: " + Nodes.grids[i].go.transform.position;
			s += "\n";
		}
		/*
		s += "\n\nNodes:\n";
		for (int i = 0; i < Nodes.nodes.Count; i++)
		{
			s += "Node: " + Nodes.nodes[i].pos;
			s += ", Mana: " + (Nodes.nodes[i].mana != null);
			s += ", Special: " + Nodes.nodes[i].specialType;
			if (Nodes.nodes[i].lines.Count > 0)
			{
				s += "\n - Lines: ";
				for (int a = 0; a < Nodes.nodes[i].lines.Count; a++)
				{
					s += Nodes.nodes[i].lines[a].uId + ", ";
				}
			}
			s += "\n";
		}
		*/


		Defines.self.debugText.text = s;

		PrintOtherInfo();
	}

	public static void PrintOtherInfo()//Prints debug info on the right side of the screen
	{
		string s = "";

		s += "\n\nMana:\n";
		for (int i = 0; i < Manas.manas.Count; i++)
		{
			s += "Mana: " + Manas.manas[i].nodePos + ", dir: " + Manas.manas[i].dir + ", Momentum: " + Manas.manas[i].momentum;

			s += "\n";
		}

		Defines.self.debugText2.text = s;
	}
}
