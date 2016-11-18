using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Gun : MonoBehaviour
{
	public Camera cam;
	public LayerMask mask;
	public LayerMask mouseVsNodesMask;

	bool drawing = false;
	int lineId = -1;
	int lastLineId = -1;
	Vector3 startPos;

	void Start()
	{

	}

	void Update()
	{
		CheckErase();
		CheckUndo();
		CheckDrawLine();

	}

	void CheckDrawLine()
	{
		if (Input.GetKey(KeyCode.V))
		{
			Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000, mask))
			{
				Vector3 point = hit.point;

				//Find a nearby node
				for (int i = 0; i < Nodes.nodes.Count; i++)
				{
					if (Vector3.Distance(Nodes.nodes[i].pos, point) < 0.25f)
					{
						//Show the connections of this node
						/*for (int a = 0; a < Nodes.nodes[i].connections.Count; a++)
						{
							Debug.DrawLine(Nodes.nodes[i].pos, Nodes.nodes[i].connections[a].pos, Color.blue);
							Debug.DrawLine(Nodes.nodes[i].pos, new Vector3(0, 2, 0), Color.magenta, 1);
						}*/

						//Find a relCon with -2x
						for (int a = 0; a < Nodes.nodes[i].relCons.Count; a++)
						{
							/*if (Nodes.nodes[i].relCons[a].x == -2 &&
								Nodes.nodes[i].relCons[a].y == 0 &&
								Nodes.nodes[i].relCons[a].z == 1)
							{
								Debug.DrawLine(Nodes.nodes[i].pos, Nodes.nodes[i].relCons[a].node.pos, Color.blue);
							}*/

							List<Nodes.Connection> connections = new List<Nodes.Connection>();
							
							connections.Add(Nodes.Con(-2,0,1,0,0,0));
							connections.Add(Nodes.Con(0,0,2,0,0,0));
							connections.Add(Nodes.Con(2,0,0,0,0,0));
							connections.Add(Nodes.Con(-1,0,-1,0,0,0));
							connections.Add(Nodes.Con(0,0,-2,2,0,-2));
							connections.Add(Nodes.Con(2,0,-2,0,0,-2));

							List<Nodes.Connection> result = Nodes.CheckRune(connections, Nodes.nodes[i]);
							if (result != null && result.Count > 0)
							{
								for (int c = 0; c < result.Count; c++)
								{
									if(result[c].n1 != null && result[c].n2 != null)
									{
										Debug.DrawLine(result[c].n1.pos, result[c].n2.pos, Color.blue);
									}
								}
							}
							else
							{
								//if(result == null) { Debug.Log("result is null");}
								//else if(result.Count <= 0) { Debug.Log("result has a count of zero");}
								//else {Debug.Log("result failed for another reason"); }
								
							}
						}




						break;
					}
				}
			}
		}

		if (Input.GetMouseButtonDown(0))
		{
			//Started drawing!
			drawing = true;

			//start drawing lines
			Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000, mask))
			{
				Nodes.Node n = Nodes.FindNearestNode(hit.point);

				//check this is a valid pos
				if (Nodes.CheckValidNode(n))
				{
					lineId = Lines.RegisterNewLine();
					startPos = n.pos;
					Lines.AddPoint(n, lineId);
				}
				else
				{
					//else, fail out
					drawing = false;
				}
			}

		}
		if (Input.GetMouseButtonUp(0))
		{
			if (drawing)
			{
				//Add last point
				Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 1000, mask))
				{
					Nodes.Node n = Nodes.FindNearestNode(hit.point);

					//check this is a valid pos
					if (Nodes.CheckValidNode(n))
					{
						Lines.AddPoint(n, lineId);
						//Debug.DrawLine(startPos, point, Color.red, 100);
						//Debug.DrawLine(ray.origin, hit.point, Color.blue, 100);
						//Debug.DrawLine(startPos, hit.point, Color.blue, 100);
					}
				}

				//Released!
				lastLineId = lineId;
				drawing = false;
				lineId = -1;
			}
		}
		if (Input.GetMouseButton(0))
		{
			if (drawing)
			{
				Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
				RaycastHit hit;
				if (Physics.Raycast(ray, out hit, 1000, mask))
				{
					Vector3 point = hit.point;
					Debug.DrawLine(startPos, point, Color.red);
				}
			}
		}
	}

	void CheckErase()
	{

		//Erase
		if (Input.GetKey(KeyCode.E))
		{
			Ray ray = cam.ViewportPointToRay(new Vector3(0.5f, 0.5f, 0));
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 1000, mask))
			{
				for (int a = 0; a < Lines.lines.Count; a++)
				{
					if (Lines.lines[a].dead) { continue; }
					for (int b = 0; b < Lines.lines[a].points.Count; b++)
					{
						if (Vector3.Distance(Lines.lines[a].points[b].pos, hit.point) < 0.12f)
						{
							Lines.DeleteLine(Lines.lines[a].uId);
						}
					}
				}
			}
		}
	}

	void CheckUndo()
	{
		//Undo
		if (Input.GetKeyDown(KeyCode.Z))
		{
			if (Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl))
			{
				if (lastLineId != -1)
				{
					Lines.DeleteLine(lastLineId);
					lastLineId = -1;
				}
			}
		}
	}

}
