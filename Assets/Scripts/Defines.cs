using UnityEngine;
using System.Collections;

public class Defines : MonoBehaviour
{
	public static Defines self;//Set in main's awake
	
	public TextMesh debugText;
	public TextMesh debugText2;
	public GameObject manaPrefab;
	public GameObject linePrefab;
	public Color lineColor;
	public Color runeColor;
	public Color unactiveRuneColor;
	public Material runeMat;
	public GameObject gridNodePrefab;
}
