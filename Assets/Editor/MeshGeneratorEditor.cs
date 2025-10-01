using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(MeshGenerator))]
class DecalMeshHelperEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        if (GUILayout.Button("Generate Mesh"))
        {
          ((MeshGenerator)target).GenerateMesh();
        }
    }
}