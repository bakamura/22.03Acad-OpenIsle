using UnityEditor;
using UnityEngine;

//[CustomPropertyDrawer(typeof(EnemyWave)), CanEditMultipleObjects]
public class EnemyWaveInspector : Editor {
    SerializedProperty enemies, spawnPoints, selectRandomSpawnPoint, spawnInterval;

    public override void OnInspectorGUI() {
        serializedObject.Update();

        EditorGUILayout.PropertyField(enemies, new GUIContent("Enemies In Wave"));
        EditorGUILayout.PropertyField(selectRandomSpawnPoint, new GUIContent("Spawn In Random Point"));
        EditorGUILayout.PropertyField(spawnInterval, new GUIContent("Spawn Interval"));
        if(!selectRandomSpawnPoint.boolValue) EditorGUILayout.PropertyField(spawnPoints, new GUIContent("Spawn Point"));

        serializedObject.ApplyModifiedProperties();
    }

    private void OnEnable() {
        enemies = serializedObject.FindProperty("enemies");
        spawnPoints = serializedObject.FindProperty("spawnPoints");
        selectRandomSpawnPoint = serializedObject.FindProperty("selectRandomSpawnPoint");
        spawnInterval = serializedObject.FindProperty("spawnInterval");
    }
}
