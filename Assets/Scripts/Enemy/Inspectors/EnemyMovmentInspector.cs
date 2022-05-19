using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyMovment))]
public class EnemyMovmentInspector : Editor {
    public override void OnInspectorGUI() {
        EnemyMovment inspector = target as EnemyMovment;

        GUILayout.Label("STATUS", EditorStyles.boldLabel);

        inspector._isFlying = EditorGUILayout.Toggle("Is Flying", inspector._isFlying);
        if (inspector._isFlying) {
            inspector._movmentSpeed = EditorGUILayout.FloatField("Movment Speed", inspector._movmentSpeed);
            inspector._rotationSpeed = EditorGUILayout.FloatField("Rotation Speed", inspector._rotationSpeed);
        }
        if (inspector._willGoTowardsPlayer) inspector._detectionRange = EditorGUILayout.FloatField("Detection Range", inspector._detectionRange);
        inspector._canWander = EditorGUILayout.Toggle("Can Wander", inspector._canWander);
        if (inspector._canWander) {
            inspector._randomNavegationArea = EditorGUILayout.FloatField("Navegation Area", inspector._randomNavegationArea);
            inspector.minDistanceFromWanderingPoint = EditorGUILayout.FloatField(new GUIContent("Min Distance From Wandering Point", "the minimal distance it needs to be for a new point generation"), inspector.minDistanceFromWanderingPoint);
            inspector.randomNavPointCooldown = EditorGUILayout.FloatField(new GUIContent("Wandering Point Cooldown", "the interval bettwen moving to a new point"), inspector.randomNavPointCooldown);
        }
        inspector._willGoTowardsPlayer = EditorGUILayout.Toggle(new GUIContent("Follow Player", "needs the EnemyBehaviourScript to work"), inspector._willGoTowardsPlayer);
    }
}
