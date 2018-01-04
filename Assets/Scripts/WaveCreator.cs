using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
[ExecuteInEditMode]
public class WaveCreator : MonoBehaviour {
	

	[System.Serializable]
	public struct Wave 
	{
			public float amplitude;
			public Vector2 direction;
			public float speed;
			public float waveLength;
	};
	// [Header("MeshSize")]
	public  int xSize;
	public int zSize;
	// public Mesh mesh;
	public Material material;
	// private Renderer renderer;

	public int numWaves = 2;
	// public Wave[] waves;

	public List<Wave> listWave;
	ComputeBuffer waveBuffer;
	public Vector2 windDir;
	public float windDeviation;
	public float medianAmp;
	public float medianWaveL;
	public float speed;
	// public List<Wave> listWaves;
	void Awake() {
		// mesh initial
		GetComponent<MeshFilter>().mesh  = OceanMesh.Generate(xSize, zSize);
		// this.material = Resources.Load("Materials/water", typeof(Material)) as Material;
		
		material = GetComponent<Renderer>().sharedMaterial;
	}
	
	void Start () {
		transform.position = new Vector3(0,0,0);
		
		GenerateWaves();

	}
	
	void Update () {
		
	}

	List<Wave> CreateWaves(float medianAmp, float medianWaveL, float speed, int numWaves) {
		List<Wave> newlistWave = new List<Wave>();
		for(int i = 0; i < numWaves; i++) {
			Wave newWave = new Wave();
			newWave.waveLength = Random.Range(medianWaveL / 2.0f, medianWaveL * 2.0f);
			newWave.amplitude = (medianAmp / medianWaveL) * newWave.waveLength;
			newWave.direction = GetRandomDir(windDir, windDeviation);
			newWave.speed = speed;

			
			newlistWave.Add(newWave);
			Debug.Log("create waves");
		}
		return newlistWave;
	}

	Vector2 GetRandomDir(Vector2 windDir, float windDeviation) {
		float theta = Vector2.Angle(windDir, Vector2.right);
		// degeree to radian
		float minTheta = Mathf.Deg2Rad * (theta - windDeviation);
		float maxTheta = Mathf.Deg2Rad * (theta + windDeviation);

		theta = Random.Range(minTheta, maxTheta);
		return new Vector2(Mathf.Cos(theta), Mathf.Sin(theta));
	}

	// public void GenerateWaves(Material material,List<Wave> listWave, float medianAmp, float medianWaveL, float speed, int numWaves)  {
	// 	material = GetComponent<Renderer>().sharedMaterial;
	// 	listWave = CreateWaves(medianAmp, medianWaveL, speed, numWaves);

	// 	if(this.waveBuffer != null) {
	// 		this.waveBuffer.Release();
	// 	}
	// 	this.waveBuffer  = new ComputeBuffer(numWaves, sizeof(float) * 5);
	// 	this.waveBuffer.SetData(listWave.ToArray());
	// 	material.SetBuffer("_waveBuffer", waveBuffer);
	// 	material.SetInt("_numWaves", numWaves);
	// }
	public void GenerateWaves()  {
		// material = GetComponent<Renderer>().sharedMateriaf l;
		this.listWave = CreateWaves(medianAmp, medianWaveL, speed, numWaves);

		if(this.waveBuffer != null) {
			this.waveBuffer.Release();
		}
		this.waveBuffer  = new ComputeBuffer(numWaves, sizeof(float) * 5);
		this.waveBuffer.SetData(listWave.ToArray());
		this.material.SetBuffer("_waveBuffer", waveBuffer);
		this.material.SetInt("_numWaves", numWaves);
	}


	void OnDestroy() {
		if(waveBuffer != null) {
		this.waveBuffer.Release();
		}
	}
}
