using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyMovment))]
[CanEditMultipleObjects]
public class EnemyMovmentInspector : Editor {
    SerializedProperty isFlying, movmentSpeed, rotationSpeed, willGoTowardsPlayer, detectionRange, viewAngle, FOVColor, canWander, randomNavegationArea, minDistanceFromWanderingPoint, randomNavPointCooldown, showConeView, showRandomNavegationPoint;
    public override void OnInspectorGUI() {
        GUILayout.Label("STATUS", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(isFlying, new GUIContent("Is Flying"));
        if (isFlying.boolValue) {
            EditorGUILayout.PropertyField(movmentSpeed, new GUIContent("Movment Speed"));
            EditorGUILayout.PropertyField(rotationSpeed, new GUIContent("Rotation Speed"));
        }
        if (willGoTowardsPlayer.boolValue) {
            EditorGUILayout.PropertyField(detectionRange, new GUIContent("Detection Range"));
            EditorGUILayout.PropertyField(viewAngle, new GUIContent("View Angle"));
        }
        EditorGUILayout.PropertyField(canWander, new GUIContent("Can Wander"));
        if (canWander.boolValue) {
            EditorGUILayout.PropertyField(randomNavegationArea, new GUIContent("Random Navegation Area"));
            EditorGUILayout.PropertyField(minDistanceFromWanderingPoint, new GUIContent("Min Distance From Wandering Point", "the minimal distance it needs to be for a new point generation"));
            EditorGUILayout.PropertyField(randomNavPointCooldown, new GUIContent("Random NavPoint Cooldown", "the interval bettwen moving to a new point"));
        }
        EditorGUILayout.PropertyField(willGoTowardsPlayer, new GUIContent("Follow Player", "needs the EnemyBehaviourScript to work"));

        GUILayout.Label("DEBUG", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(showConeView, new GUIContent("Show Cone View Gizmo"));
        EditorGUILayout.PropertyField(showRandomNavegationPoint, new GUIContent("Show Random Nav Point Gizmo"));

        serializedObject.ApplyModifiedProperties();
    }

    public void OnEnable() {
        isFlying = serializedObject.FindProperty("_isFlying");
        movmentSpeed = serializedObject.FindProperty("_movmentSpeed");
        rotationSpeed = serializedObject.FindProperty("_rotationSpeed");
        willGoTowardsPlayer = serializedObject.FindProperty("_willGoTowardsPlayer");
        detectionRange = serializedObject.FindProperty("_detectionRange");
        viewAngle = serializedObject.FindProperty("_viewAngle");
        FOVColor = serializedObject.FindProperty("_FOVColor");
        canWander = serializedObject.FindProperty("_canWander");
        randomNavegationArea = serializedObject.FindProperty("_randomNavegationArea");
        minDistanceFromWanderingPoint = serializedObject.FindProperty("_minDistanceFromWanderingPoint");
        randomNavPointCooldown = serializedObject.FindProperty("_randomNavPointCooldown");
        showConeView = serializedObject.FindProperty("_showConeView");
        showRandomNavegationPoint = serializedObject.FindProperty("_showRandomNavegationPoint");
    }
}
