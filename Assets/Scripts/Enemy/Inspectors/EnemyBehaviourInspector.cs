using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyBehaviour))]
[CanEditMultipleObjects]
public class EnemyBehaviourInspector : Editor {
    SerializedProperty hitDetection, attackPoint, enemyType, bulletPrefab, bulletStartPoint, player, actionArea, rotationSpeed, damage, attackSpeed, isKamikaze, bulletSpeed, bulletSize, bulletAmount, bulletMaxHeighOffset, showAttackArea;
    public override void OnInspectorGUI() {
        //start inspector
        GUILayout.Label("COMPONENTS", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(hitDetection, new GUIContent("Hit Detection"));
        EditorGUILayout.PropertyField(attackPoint, new GUIContent("Attack Point"));
        if (enemyType.enumValueIndex == (int)EnemyBehaviour.EnemyTypes.shoot) EditorGUILayout.PropertyField(bulletPrefab, new GUIContent("Bullet Prefab"));        
        if (enemyType.enumValueIndex != (int)EnemyBehaviour.EnemyTypes.passive) EditorGUILayout.PropertyField(player, new GUIContent("Player Layer"));

        GUILayout.Label("BASE STATUS", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(enemyType, new GUIContent("Enemy Type"));
        EditorGUILayout.PropertyField(actionArea, new GUIContent("Action Area"));
        if (enemyType.enumValueIndex == (int)EnemyBehaviour.EnemyTypes.shoot) EditorGUILayout.PropertyField(rotationSpeed, new GUIContent("Rotation Speed", "if will not move give this a value, else change this value in the EnemyMovment script"));
        if (enemyType.enumValueIndex != (int)EnemyBehaviour.EnemyTypes.passive) {
            EditorGUILayout.PropertyField(damage, new GUIContent("Damage"));
            EditorGUILayout.PropertyField(attackSpeed, new GUIContent("Attack Speed"));
            if (enemyType.enumValueIndex != (int)EnemyBehaviour.EnemyTypes.shoot) EditorGUILayout.PropertyField(isKamikaze, new GUIContent("Is Kamikaze"));
        }

        GUILayout.Label("RANGE STATUS", EditorStyles.boldLabel);

        if (enemyType.enumValueIndex == (int)EnemyBehaviour.EnemyTypes.shoot) {
            EditorGUILayout.PropertyField(bulletSpeed, new GUIContent("Bullet Speed"));
            EditorGUILayout.PropertyField(bulletSize, new GUIContent("Bullet Size"));
            EditorGUILayout.PropertyField(bulletAmount, new GUIContent("Bullet Amount"));
            EditorGUILayout.PropertyField(bulletMaxHeighOffset, new GUIContent("Bullet Max Height Offset", "bullet start point in Yaxis + this value = Max Bullet Height"));
        }

        GUILayout.Label("COMPONENTS", EditorStyles.boldLabel);
        EditorGUILayout.PropertyField(showAttackArea, new GUIContent("Show Attack Area Gizmo"));

        serializedObject.ApplyModifiedProperties();
    }
    public void OnEnable() {
        hitDetection = serializedObject.FindProperty("_hitDetection");
        attackPoint = serializedObject.FindProperty("_attackPoint");
        enemyType = serializedObject.FindProperty("_enemyType");
        bulletPrefab = serializedObject.FindProperty("_bulletPrefab");
        bulletStartPoint = serializedObject.FindProperty("_bulletStartPoint");
        player = serializedObject.FindProperty("_player");
        actionArea = serializedObject.FindProperty("_actionArea");
        rotationSpeed = serializedObject.FindProperty("_rotationSpeed");
        damage = serializedObject.FindProperty("_damage");
        attackSpeed = serializedObject.FindProperty("_attackSpeed");
        isKamikaze = serializedObject.FindProperty("_isKamikaze");
        bulletSpeed = serializedObject.FindProperty("_bulletSpeed");
        bulletSize = serializedObject.FindProperty("_bulletSize");
        bulletAmount = serializedObject.FindProperty("_bulletAmount");
        bulletMaxHeighOffset = serializedObject.FindProperty("_bulletMaxHeighOffset");
        showAttackArea = serializedObject.FindProperty("_showAttackArea");
    }
}
