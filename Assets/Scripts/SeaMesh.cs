﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class SeaMesh  {

	// private static Vector3[] vertices;
	// private static Mesh mesh;
	
	public static Mesh Generate(int xSize, int ySize) {
		Vector3[] vertices;
		Mesh mesh = new Mesh();

		// vertices, uv, tangents
		vertices = new Vector3[(xSize + 1) * (ySize + 1)];
		Vector2[] uv = new Vector2[vertices.Length];
		Vector4[] tangents = new Vector4[vertices.Length];
		Vector4 tangent = new Vector4(1f, 0f, 0f, -1f);

		for(int i = 0, y = 0; y <= ySize; y++) 
			for(int x = 0; x <= xSize; x++, i++) {
				vertices[i] = new Vector3(x, 0, y);
				uv[i] = new Vector2((float)x / xSize, (float)y / ySize);
				tangents[i] = tangent;
			}
		mesh.vertices = vertices;
		mesh.uv = uv;
		mesh.tangents = tangents;

		// triangles
		int[] triangles = new int[xSize * ySize * 6];
		for(int ti = 0, vi = 0, y = 0; y < ySize; y++, vi++) 
			for(int x = 0; x < xSize; x++, ti += 6, vi++) {
				triangles[ti] = vi;
				triangles[ti + 3] = triangles[ti + 2] = vi + 1;
				triangles[ti + 4] = triangles[ti + 1] = vi + xSize + 1;
				triangles[ti + 5] = vi + xSize + 2;
			}
		mesh.triangles = triangles;

		mesh.RecalculateNormals();
		mesh.RecalculateBounds();

		return mesh;
	}

	// void OnDrawGizmos() {
	// 	if(mesh == null) {
	// 		return;
	// 	}

	// 	Gizmos.color = Color.gray;
	// 	Gizmos.DrawWireMesh(mesh, transform.position, transform.rotation);
	// 	// Gizmos.color = Color.black;
	// 	// for (int i = 0; i < vertices.Length; i++) {
	// 	// 	Gizmos.DrawSphere(vertices[i], 0.1f);
	// 	// }
	// }
}
