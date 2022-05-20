using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ConeView : MonoBehaviour
{
    [SerializeField] private float _distance;
    [SerializeField] private float _angle;
    [SerializeField] private float _height;
    [SerializeField] private Color _color = Color.blue;
    Mesh _coneMesh;
    private Mesh CreateConeMesh() {
        Mesh cone = new Mesh();
        int numOfTrianlges = 8;
        int numVertices = numOfTrianlges * 3;

        Vector3[] vertices = new Vector3[numVertices];
        int[] triagles = new int[numVertices];

        Vector3 bottomCenter = Vector3.zero;
        Vector3 bottomLeft = Quaternion.Euler(0, -_angle, 0) * Vector3.forward * _distance;
        Vector3 bottomRight = Quaternion.Euler(0, _angle, 0) * Vector3.forward * _distance;

        Vector3 topCenter = bottomCenter + Vector3.up * _height;
        Vector3 topLeft = bottomLeft + Vector3.up * _height;
        Vector3 topRight = bottomRight + Vector3.up * _height;

        int currentVert = 0;
        //left side
        vertices[currentVert++] = bottomCenter;
        vertices[currentVert++] = bottomLeft;
        vertices[currentVert++] = topLeft;

        vertices[currentVert++] = topLeft;
        vertices[currentVert++] = topCenter;
        vertices[currentVert++] = bottomCenter;

        //right side
        vertices[currentVert++] = bottomCenter;
        vertices[currentVert++] = topCenter;
        vertices[currentVert++] = topRight;

        vertices[currentVert++] = topRight;
        vertices[currentVert++] = bottomRight;
        vertices[currentVert++] = bottomCenter;

        //conection between the right and left
        vertices[currentVert++] = bottomLeft;
        vertices[currentVert++] = topLeft;
        vertices[currentVert++] = topRight;

        vertices[currentVert++] = topRight;
        vertices[currentVert++] = bottomRight;
        vertices[currentVert++] = bottomLeft;

        //bottom
        vertices[currentVert++] = bottomCenter;
        vertices[currentVert++] = bottomLeft;
        vertices[currentVert++] = bottomRight;

        //top
        vertices[currentVert++] = topCenter;
        vertices[currentVert++] = topLeft;
        vertices[currentVert++] = topRight;

        for (int i = 0; i < numVertices; i++) triagles[i] = i;
        cone.vertices = vertices;
        cone.triangles = triagles;
        cone.RecalculateNormals();

        return cone;
    }

    private void OnValidate() {
        _coneMesh = CreateConeMesh();
    }

    private void OnDrawGizmosSelected() {
        if (_coneMesh) {
            Gizmos.color = _color;
            Gizmos.DrawMesh(_coneMesh, transform.position, transform.rotation);
        }
    }

}
