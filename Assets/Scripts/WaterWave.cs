using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaterWave : MonoBehaviour {

Mesh mesh0;
 int verticesNum; //mesh的顶点数
 Vector3[] myVertices; // 保存临时生成的顶点坐标
 public float speed = 1.0f,waveHeight=1.0f;


 void Start () {
 mesh0 = GameObject.Find("Plane").GetComponent<MeshFilter>().mesh;
 verticesNum = mesh0.vertexCount;
 myVertices = new Vector3[verticesNum];
 }

 
 void Update () {
 for (int i = 0; i < verticesNum; i++)
 {
 myVertices[i] = new Vector3(mesh0.vertices[i].x,
 Mathf.Sin(Time.time*speed+mesh0.vertices[i].x)*waveHeight,
 mesh0.vertices[i].z);
 }
 mesh0.vertices = myVertices;
 mesh0.RecalculateBounds();
 mesh0.RecalculateNormals();
 }

 void OnDrawGizmos() {
    // Gizmos.DrawMesh(mesh0, transform.position, transform.rotation);
    
    Gizmos.DrawWireMesh(mesh0, transform.position, transform.rotation);
 }
}
