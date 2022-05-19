using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

//[CustomEditor(typeof(EnemyBehaviour))]
public class EnemyBehaviourInspector : Editor
{
    //public VisualTreeAsset _enemyInspectorTree;
    //public override VisualElement CreateInspectorGUI() {
    //    VisualElement newInspector = new VisualElement();
    //    _enemyInspectorTree.CloneTree(newInspector);
    //    return newInspector;
    //}
    //public override void OnInspectorGUI() {
    //    //base.OnInspectorGUI();
    //    EnemyBehaviour inspector = target as EnemyBehaviour;
    //    inspector._hitDetection = EditorGUILayout.ObjectField("Hit Detection", inspector._hitDetection, typeof(Collider), true) as Collider;
    //    inspector.enemyType = EditorGUILayout.EnumPopup("Enemy Type", typeof(EnemyBehaviour.EnemyTypes)) as EnemyBehaviour.EnemyTypes;
    //}
}
