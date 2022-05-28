using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyBehaviour))]
[CanEditMultipleObjects]
public class EnemyBehaviourInspector : Editor {
    SerializedProperty hitDetection, attackPoint, enemyType, bulletPrefab, bulletStartPoint, player, actionArea, rotationSpeed, damage, attackSpeed, isKamikaze, bulletSpeed, bulletSize, bulletAmount, bulletMaxHeighOffset;
    public override void OnInspectorGUI() {
        //EnemyBehaviour inspector = target as EnemyBehaviour;
        //start inspector
        GUILayout.Label("COMPONENTS", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(hitDetection, new GUIContent("Hit Detection"));
        EditorGUILayout.PropertyField(attackPoint, new GUIContent("Attack Point"));
        //inspector._hitDetection = EditorGUILayout.ObjectField("Hit Detection", inspector._hitDetection, typeof(Collider), true) as Collider;
        //inspector._attackPoint = EditorGUILayout.ObjectField("Attack Point", inspector._attackPoint, typeof(Transform), true) as Transform;
        if (enemyType.enumValueIndex == (int)EnemyBehaviour.EnemyTypes.shoot) {// inspector._enemyType == EnemyBehaviour.EnemyTypes.shoot
            EditorGUILayout.PropertyField(bulletPrefab, new GUIContent("Bullet Prefab"));
            EditorGUILayout.PropertyField(bulletStartPoint, new GUIContent("Bullet Start Point"));
            //inspector._bulletPrefab = EditorGUILayout.ObjectField("Bullet Prefab", inspector._bulletPrefab, typeof(GameObject), true) as GameObject;
            //inspector._bulletStartPoint = EditorGUILayout.ObjectField("Bullet Start Point", inspector._bulletStartPoint, typeof(Transform), true) as Transform;
        }
        if (enemyType.enumValueIndex != (int)EnemyBehaviour.EnemyTypes.passive) EditorGUILayout.PropertyField(player, new GUIContent("Player Layer"));//inspector._player = EditorGUILayout.LayerField("Player Layer", inspector._player);

        GUILayout.Label("BASE STATUS", EditorStyles.boldLabel);

        EditorGUILayout.PropertyField(enemyType, new GUIContent("Enemy Type"));
        EditorGUILayout.PropertyField(actionArea, new GUIContent("Action Area"));
        //inspector._enemyType = (EnemyBehaviour.EnemyTypes)EditorGUILayout.EnumPopup("Enemy Type", inspector._enemyType);
        //inspector._actionArea = EditorGUILayout.Vector3Field("Action Area", inspector._actionArea);
        if (enemyType.enumValueIndex == (int)EnemyBehaviour.EnemyTypes.shoot) EditorGUILayout.PropertyField(rotationSpeed, new GUIContent("Rotation Speed", "if will not move give this a value, else change this value in the EnemyMovment script"));// inspector._rotationSpeed = EditorGUILayout.FloatField(new GUIContent("Rotation Speed", "if will not move give this a value, else change this value in the EnemyMovment script"), inspector._rotationSpeed);
        if (enemyType.enumValueIndex != (int)EnemyBehaviour.EnemyTypes.passive) {
            EditorGUILayout.PropertyField(damage, new GUIContent("Damage"));
            EditorGUILayout.PropertyField(attackSpeed, new GUIContent("Attack Speed"));
            //inspector._damage = EditorGUILayout.FloatField("Damage", inspector._damage);
            //inspector._attackSpeed = EditorGUILayout.FloatField("Attack Speed", inspector._attackSpeed);
            if (enemyType.enumValueIndex != (int)EnemyBehaviour.EnemyTypes.shoot) EditorGUILayout.PropertyField(isKamikaze, new GUIContent("Is Kamikaze"));//inspector._isKamikaze = EditorGUILayout.Toggle("Is Kamikaze", inspector._isKamikaze);
        }

        GUILayout.Label("RANGE STATUS", EditorStyles.boldLabel);

        if (enemyType.enumValueIndex == (int)EnemyBehaviour.EnemyTypes.shoot) {
            EditorGUILayout.PropertyField(bulletSpeed, new GUIContent("Bullet Speed"));
            EditorGUILayout.PropertyField(bulletSize, new GUIContent("Bullet Size"));
            EditorGUILayout.PropertyField(bulletAmount, new GUIContent("Bullet Amount"));
            EditorGUILayout.PropertyField(bulletMaxHeighOffset, new GUIContent("Bullet Max Height Offset", "bullet start point in Yaxis + this value = Max Bullet Height"));
            //inspector._bulletSpeed = EditorGUILayout.FloatField("Bullet Speed", inspector._bulletSpeed);
            //inspector._bulletSize = EditorGUILayout.FloatField("Bullet Size", inspector._bulletSize);
            //inspector._bulletAmount = EditorGUILayout.IntField("Bullet Amount", inspector._bulletAmount);
            //inspector._bulletMaxHeighOffset = EditorGUILayout.FloatField(new GUIContent("Bullet Max Height Offset", "bullet start point in Yaxis + this value = Max Bullet Height"), inspector._bulletMaxHeighOffset);
        }
        
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
    }
}
