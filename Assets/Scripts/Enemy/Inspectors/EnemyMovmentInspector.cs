using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyMovment))]
[CanEditMultipleObjects]
public class EnemyMovmentInspector : Editor {
    SerializedProperty isFlying, movmentSpeed, rotationSpeed, followPlayer, detectionRange, viewAngle, FOVColor, canWander, randomNavegationArea, minDistanceFromWanderingPoint, randomNavPointCooldown, showConeView, showRandomNavegationArea;
    public override void OnInspectorGUI() {
        GUILayout.Label("STATUS", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(isFlying, new GUIContent("Is Flying"));
        if (isFlying.boolValue) {
            EditorGUILayout.PropertyField(movmentSpeed, new GUIContent("Movment Speed"));
            EditorGUILayout.PropertyField(rotationSpeed, new GUIContent("Rotation Speed"));
        }
        if (followPlayer.boolValue) {
            EditorGUILayout.PropertyField(detectionRange, new GUIContent("Detection Range"));
            EditorGUILayout.PropertyField(viewAngle, new GUIContent("View Angle"));
        }
        EditorGUILayout.PropertyField(canWander, new GUIContent("Can Wander"));
        if (canWander.boolValue) {
            EditorGUILayout.PropertyField(randomNavegationArea, new GUIContent("Random Navegation Area"));
            //EditorGUILayout.PropertyField(minDistanceFromWanderingPoint, new GUIContent("Min Distance From Wandering Point", "the minimal distance it needs to be for a new point generation"));
            EditorGUILayout.PropertyField(randomNavPointCooldown, new GUIContent("Random NavPoint Cooldown", "the interval bettwen moving to a new point"));
        }
        EditorGUILayout.PropertyField(followPlayer, new GUIContent("Follow Player", "needs the EnemyBehaviourScript to work"));

        GUILayout.Label("DEBUG", EditorStyles.boldLabel);

        if (followPlayer.boolValue) {
            EditorGUILayout.PropertyField(showConeView, new GUIContent("Show Cone View Gizmo"));
            //EditorGUILayout.PropertyField(FOVColor, new GUIContent("View Cone Color"));
        }
        if(canWander.boolValue) EditorGUILayout.PropertyField(showRandomNavegationArea, new GUIContent("Show Random Nav Point Gizmo"));

        serializedObject.ApplyModifiedProperties();
    }

    public void OnEnable() {
        isFlying = serializedObject.FindProperty("_isFlying");
        movmentSpeed = serializedObject.FindProperty("_movmentSpeed");
        rotationSpeed = serializedObject.FindProperty("_rotationSpeed");
        followPlayer = serializedObject.FindProperty("_followPlayer");
        detectionRange = serializedObject.FindProperty("_detectionRange");
        viewAngle = serializedObject.FindProperty("_viewAngle");
        FOVColor = serializedObject.FindProperty("_FOVColor");
        canWander = serializedObject.FindProperty("_canWander");
        randomNavegationArea = serializedObject.FindProperty("_randomNavegationArea");
        //minDistanceFromWanderingPoint = serializedObject.FindProperty("_minDistanceFromWanderingPoint");
        randomNavPointCooldown = serializedObject.FindProperty("_randomNavPointCooldown");
        showConeView = serializedObject.FindProperty("_showConeView");
        showRandomNavegationArea = serializedObject.FindProperty("_showRandomNavegationArea");
    }
}
