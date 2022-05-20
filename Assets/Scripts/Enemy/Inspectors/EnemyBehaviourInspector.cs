using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(EnemyBehaviour))]
public class EnemyBehaviourInspector : Editor {
    public override void OnInspectorGUI() {
        EnemyBehaviour inspector = target as EnemyBehaviour;
        //start inspector
        GUILayout.Label("COMPONENTS", EditorStyles.boldLabel);

        inspector._hitDetection = EditorGUILayout.ObjectField("Hit Detection", inspector._hitDetection, typeof(Collider), true) as Collider;
        inspector._attackPoint = EditorGUILayout.ObjectField("Attack Point", inspector._attackPoint, typeof(Transform), true) as Transform;
        if (inspector.enemyType == EnemyBehaviour.EnemyTypes.shoot) {
            inspector._bulletPrefab = EditorGUILayout.ObjectField("Bullet Prefab", inspector._bulletPrefab, typeof(GameObject), true) as GameObject;
            inspector._bulletStartPoint = EditorGUILayout.ObjectField("Bullet Start Point", inspector._bulletStartPoint, typeof(Transform), true) as Transform;
        }
        if (inspector.enemyType != EnemyBehaviour.EnemyTypes.passive) inspector._player = EditorGUILayout.LayerField("Player Layer", inspector._player);

        GUILayout.Label("BASE STATUS", EditorStyles.boldLabel);

        inspector.enemyType = (EnemyBehaviour.EnemyTypes)EditorGUILayout.EnumPopup("Enemy Type", inspector.enemyType);
        inspector.actionArea = EditorGUILayout.Vector3Field("Action Area", inspector.actionArea);
        if (inspector.enemyType == EnemyBehaviour.EnemyTypes.shoot) inspector._rotationSpeed = EditorGUILayout.FloatField(new GUIContent("Rotation Speed", "if will not move give this a value, else change this value in the EnemyMovment script"), inspector._rotationSpeed);
        if (inspector.enemyType != EnemyBehaviour.EnemyTypes.passive) {
            inspector._damage = EditorGUILayout.FloatField("Damage", inspector._damage);
            inspector._attackSpeed = EditorGUILayout.FloatField("Attack Speed", inspector._attackSpeed);
            if (inspector.enemyType != EnemyBehaviour.EnemyTypes.shoot) inspector._isKamikaze = EditorGUILayout.Toggle("Is Kamikaze", inspector._isKamikaze);
        }

        GUILayout.Label("RANGE STATUS", EditorStyles.boldLabel);

        if (inspector.enemyType == EnemyBehaviour.EnemyTypes.shoot) {
            inspector._bulletSpeed = EditorGUILayout.FloatField("Bullet Speed", inspector._bulletSpeed);
            inspector._bulletSize = EditorGUILayout.FloatField("Bullet Size", inspector._bulletSize);
            inspector._bulletAmount = EditorGUILayout.IntField("Bullet Amount", inspector._bulletAmount);
            inspector._bulletMaxHeighOffset = EditorGUILayout.FloatField(new GUIContent("Bullet Max Height Offset", "bullet start point in Yaxis + this value = Max Bullet Height"), inspector._bulletMaxHeighOffset);
        }
    }
}
