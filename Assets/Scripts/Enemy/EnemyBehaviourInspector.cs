using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

[CustomEditor(typeof(EnemyBehaviour))]
public class EnemyBehaviourInspector : Editor
{
    //public VisualTreeAsset _enemyInspectorTree;
    //public override VisualElement CreateInspectorGUI() {
    //    VisualElement newInspector = new VisualElement();
    //    _enemyInspectorTree.CloneTree(newInspector);
    //    return newInspector;
    //}
    public override void OnInspectorGUI() {
        //base.OnInspectorGUI();
        EnemyBehaviour inspector = target as EnemyBehaviour;
        inspector.enemyType = (EnemyBehaviour.EnemyTypes)EditorGUILayout.EnumPopup("Enemy Type", inspector.enemyType);
        if (inspector.enemyType == EnemyBehaviour.EnemyTypes.shoot) inspector._bulletPrefab = EditorGUILayout.ObjectField("Bullet Prefab", inspector._bulletPrefab, typeof(GameObject), true) as GameObject;
    }
}
