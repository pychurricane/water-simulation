using UnityEngine;
using UnityEditor;

[CustomEditor(typeof(WaveCreator))]
public class WaveCreatorEditor : Editor {
	WaveCreator waveCreator;

	void OnEnable() {
		waveCreator = (WaveCreator)target;
	}
	public override void OnInspectorGUI() {
		// base.OnInspectorGUI();

		serializedObject.Update();

		EditorGUILayout.LabelField("MeshSize");
		// EditorGUILayout.BeginHorizontal();
		// GUILayout.BeginHorizontal();
		waveCreator.xSize = EditorGUILayout.IntField("xSize",waveCreator.xSize);
		waveCreator.zSize = EditorGUILayout.IntField("zSize",waveCreator.zSize);
		// EditorGUILayout.EndHorizontal();
		if(GUILayout.Button("GenerateMesh")) {
			waveCreator.gameObject.GetComponent<MeshFilter>().mesh = OceanMesh.Generate(waveCreator.xSize, waveCreator.zSize);
		}

		// GUILayout.EndHorizontal();
		
		waveCreator.numWaves = EditorGUILayout.IntField("numWaves", waveCreator.numWaves);
		waveCreator.medianAmp = EditorGUILayout.FloatField("medianAmp", waveCreator.medianAmp);
		waveCreator.medianWaveL = EditorGUILayout.FloatField("medianWaveL", waveCreator.medianWaveL);
		waveCreator.speed = EditorGUILayout.FloatField("speed", waveCreator.speed);
		waveCreator.windDeviation = EditorGUILayout.Slider("windDeviation", waveCreator.windDeviation, 0, 90);
		waveCreator.windDir = EditorGUILayout.Vector2Field("windDir", waveCreator.windDir);

		if(GUILayout.Button("GenerateWaves")) {
			waveCreator.GenerateWaves();
		}
		// EditorGUILayout.PropertyField(serializedObject.FindProperty("waves"), true);
		// serializedObject.ApplyModifiedProperties();			// 同步
		
		// SerializedProperty listWaves = serializedObject.FindProperty("waves");
		// EditorGUILayout.PropertyField(listWaves);

		// WaveCreator.numWaves = EditorGUILayout.IntField("numWaves", WaveCreator.numWaves);
		// listWaves.arraySize = WaveCreator.numWaves;

		// EditorGUI.indentLevel += 1;
		// for(int i = 0; i < listWaves.arraySize; i++) {
		// 	EditorGUILayout.PropertyField(listWaves.GetArrayElementAtIndex(i),true);
		// }
		// EditorGUI.indentLevel -=1;

		// serializedObject.ApplyModifiedProperties();
	}
}
