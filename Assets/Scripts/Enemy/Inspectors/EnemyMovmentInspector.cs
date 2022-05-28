using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyMovment))]
[CanEditMultipleObjects]
public class EnemyMovmentInspector : Editor {
    SerializedProperty isFlying, movmentSpeed, rotationSpeed, willGoTowardsPlayer, detectionRange, viewAngle, FOVColor, canWander, randomNavegationArea, minDistanceFromWanderingPoint, randomNavPointCooldown;
    public override void OnInspectorGUI() {
        //EnemyMovment inspector = target as EnemyMovment;
        GUILayout.Label("STATUS", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(isFlying, new GUIContent("Is Flying"));
        //inspector._isFlying = EditorGUILayout.Toggle("Is Flying", inspector._isFlying);
        if (isFlying.boolValue) {
            EditorGUILayout.PropertyField(movmentSpeed, new GUIContent("Movment Speed"));
            EditorGUILayout.PropertyField(rotationSpeed, new GUIContent("Rotation Speed"));
            //inspector._movmentSpeed = EditorGUILayout.FloatField("Movment Speed", inspector._movmentSpeed);
            //inspector._rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", inspector._rotationSpeed);
        }
        if (willGoTowardsPlayer.boolValue) {
            EditorGUILayout.PropertyField(detectionRange, new GUIContent("Detection Range"));
            EditorGUILayout.PropertyField(viewAngle, new GUIContent("View Angle"));
            //EditorGUILayout.PropertyField(FOVColor, new GUIContent("FOVColor"));
            //inspector._detectionRange = EditorGUILayout.FloatField("Detection Range", inspector._detectionRange);
            //inspector._viewAngle = EditorGUILayout.FloatField("Detection Angle", inspector._viewAngle);
            //inspector._FOVcolor = EditorGUILayout.ColorField("FOV Debug Color", inspector._FOVcolor);
        }
        EditorGUILayout.PropertyField(canWander, new GUIContent("Can Wander"));
        //inspector._canWander = EditorGUILayout.Toggle("Can Wander", inspector._canWander);
        if (canWander.boolValue) {
            EditorGUILayout.PropertyField(randomNavegationArea, new GUIContent("Random Navegation Area"));
            EditorGUILayout.PropertyField(minDistanceFromWanderingPoint, new GUIContent("Min Distance From Wandering Point", "the minimal distance it needs to be for a new point generation"));
            EditorGUILayout.PropertyField(randomNavPointCooldown, new GUIContent("Random NavPoint Cooldown", "the interval bettwen moving to a new point"));
            //inspector._randomNavegationArea = EditorGUILayout.FloatField("Navegation Area", inspector._randomNavegationArea);
            //inspector._minDistanceFromWanderingPoint = EditorGUILayout.FloatField(new GUIContent("Min Distance From Wandering Point", "the minimal distance it needs to be for a new point generation"), inspector._minDistanceFromWanderingPoint);
            //inspector._randomNavPointCooldown = EditorGUILayout.FloatField(new GUIContent("Wandering Point Cooldown", "the interval bettwen moving to a new point"), inspector._randomNavPointCooldown);
        }
        EditorGUILayout.PropertyField(willGoTowardsPlayer, new GUIContent("Follow Player", "needs the EnemyBehaviourScript to work"));
        //inspector._willGoTowardsPlayer = EditorGUILayout.Toggle(new GUIContent("Follow Player", "needs the EnemyBehaviourScript to work"), inspector._willGoTowardsPlayer);

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
    }
}
