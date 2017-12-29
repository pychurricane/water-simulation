using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]

public class CreateWaves : MonoBehaviour {
	public float phase;		// Speed * 2pi / L
	public float amplitude;
	public float direction;
	public float speed;
	public float waveLength;
	public  int xSize;
	public int zSize;

	private Mesh mesh;

	void Awake() {
		
		mesh = SeaMesh.Generate(xSize, zSize);
		GetComponent<MeshFilter>().mesh = mesh;
		
	}
	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
