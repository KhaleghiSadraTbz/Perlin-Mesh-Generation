using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using static UnityEditor.Progress;

[ExecuteAlways]
[RequireComponent(typeof(MeshFilter))]
public class MeshGenerator : MonoBehaviour
{
    public Vector2Int Size;
    public Vector2Int Subdivisions;
    public float HeightAdjustment;
    public bool DrawBase;
    public enum GizmoDrawModeEnum { Always, Selected, Never }
    public GizmoDrawModeEnum VertexGizmoDrawMode;
    public List<PerlinProfile> PerlinProfiles = new List<PerlinProfile>();


    int X_Subdivided_Length, Y_Subdivided_Length;
    int X_VertCount, Y_VertCount;
    Vector3[] Mesh_Vertecies = new Vector3[0];
    int[] Mesh_Triangles;
    Vector2[] uv;
    int[] upverts_list, downverts_list;
    int Lost_Vert_Count;

    public Mesh mesh;
    public MeshFilter meshFilter;

    public void Start()
    {
        mesh = new Mesh();
        meshFilter = GetComponent<MeshFilter>();
        meshFilter.sharedMesh = mesh;
        GenerateMesh();
    }

    public void GenerateMesh()
    {
        if(mesh.IsUnityNull())
        {
            mesh = new Mesh();
        }

        if(meshFilter.IsUnityNull())
        {
            meshFilter = GetComponent<MeshFilter>();
        }

        if(meshFilter.sharedMesh.IsUnityNull())
        {
            meshFilter.sharedMesh = mesh;
        }

        if (Size.x < 1) Size.x = 1;
        if (Size.y < 1) Size.y = 1;
        if (Subdivisions.x < 2) Subdivisions.x = 2;
        if (Subdivisions.y < 2) Subdivisions.y = 2;

        X_VertCount = Size.x * Subdivisions.x + 1;
        Y_VertCount = Size.y * Subdivisions.y + 1;
        Y_Subdivided_Length = Subdivisions.y * Size.y;
        X_Subdivided_Length = Subdivisions.x * Size.x;

        int X_Start = 0, X_End = 0, Y_Start = 0, Y_End = 0;


        if (X_Subdivided_Length % 2 == 0)
        {
            X_Start = -(X_Subdivided_Length / 2);
            X_End = (X_Subdivided_Length / 2);
        }
        else
        {
            X_Start = -(X_Subdivided_Length - 1) / 2;
            X_End = ((X_VertCount - 1) / 2) + 1;

        }

        if (Y_Subdivided_Length % 2 == 0)
        {
            Y_Start = -(Y_Subdivided_Length / 2);
            Y_End = (Y_Subdivided_Length / 2);
        }
        else
        {
            Y_Start = -(Y_VertCount - 1) / 2;
            Y_End = ((Y_VertCount - 1) / 2) + 1;
        }


        if (DrawBase)
        {
            Lost_Vert_Count = (X_Subdivided_Length - 1) * (Y_Subdivided_Length - 1);
            Mesh_Vertecies = new Vector3[(2 * (X_Subdivided_Length + 1) * (Y_Subdivided_Length + 1)) - Lost_Vert_Count];
            Mesh_Triangles = new int[(12 * X_Subdivided_Length) + (12 * Y_Subdivided_Length) + (6 * X_Subdivided_Length * Y_Subdivided_Length) + 6];

            upverts_list = new int[((X_Subdivided_Length + 1) * (Y_Subdivided_Length + 1)) - Lost_Vert_Count];
            downverts_list = new int[((X_Subdivided_Length + 1) * (Y_Subdivided_Length + 1)) - Lost_Vert_Count];

        }
        else
        {
            Mesh_Vertecies = new Vector3[(X_Subdivided_Length + 1) * (Y_Subdivided_Length + 1)];
            Mesh_Triangles = new int[6 * X_Subdivided_Length * Y_Subdivided_Length];
        }


        for (int x = X_Start, vert_index = 0, index = 0, bottom_index = (X_Subdivided_Length + 1) * (Y_Subdivided_Length + 1); x <= X_End; x++)
        {
            for (int y = Y_Start; y <= Y_End; y++)
            {
                Mesh_Vertecies[index] = new Vector3(((float)x) / Subdivisions.x, PerlinProfile.calculateHeight(PerlinProfiles,(float)x/Subdivisions.x, (float)y /Subdivisions.y)+HeightAdjustment, ((float)y) / Subdivisions.y);

                if (x == X_Start || x == X_End || y == Y_Start || y == Y_End)
                {
                    if (DrawBase)
                    {
                        Mesh_Vertecies[bottom_index] = new Vector3(((float)x) / Subdivisions.x, -1, ((float)y) / Subdivisions.y);
                        bottom_index++;

                        upverts_list[vert_index] = index;
                        downverts_list[vert_index] = bottom_index - 1;

                        vert_index++;
                    }
                }
                index++;
            }

        }


        uv = new Vector2[Mesh_Vertecies.Length];
        for (int i = 0; i < Mesh_Vertecies.Length; i++)
        {
            uv[i] = new Vector2(Mesh_Vertecies[i].x, Mesh_Vertecies[i].z);
        }


        int tris = 0, vert = 0;

        for (int x = 0; x < X_VertCount - 1; x++)
        {
            for (int y = 0; y < Y_VertCount - 1; y++)
            {
                Mesh_Triangles[tris + 0] = vert + 1;
                Mesh_Triangles[tris + 1] = vert + Y_Subdivided_Length + 1;
                Mesh_Triangles[tris + 2] = vert + 0;
                Mesh_Triangles[tris + 3] = vert + Y_Subdivided_Length + 2;
                Mesh_Triangles[tris + 4] = vert + Y_Subdivided_Length + 1;
                Mesh_Triangles[tris + 5] = vert + 1;

                vert++;
                tris += 6;
            }
            vert++;
        }


        if (DrawBase)
        {
            int Triangle_Index = 6 * X_Subdivided_Length * Y_Subdivided_Length;
            for (int indexis = 0; indexis < Y_Subdivided_Length; indexis++)
            {

                Mesh_Triangles[Triangle_Index + 0] = downverts_list[indexis + 1];
                Mesh_Triangles[Triangle_Index + 1] = upverts_list[indexis + 1];
                Mesh_Triangles[Triangle_Index + 2] = upverts_list[indexis];
                Mesh_Triangles[Triangle_Index + 3] = downverts_list[indexis];
                Mesh_Triangles[Triangle_Index + 4] = downverts_list[indexis + 1];
                Mesh_Triangles[Triangle_Index + 5] = upverts_list[indexis];
                Triangle_Index += 6;
            }

            for (int indexis = upverts_list.Length - 1; indexis > upverts_list.Length - 1 - Y_Subdivided_Length; indexis--)
            {
                Mesh_Triangles[Triangle_Index + 2] = downverts_list[indexis];
                Mesh_Triangles[Triangle_Index + 1] = upverts_list[indexis];
                Mesh_Triangles[Triangle_Index + 0] = upverts_list[indexis - 1];
                Mesh_Triangles[Triangle_Index + 5] = downverts_list[indexis - 1];
                Mesh_Triangles[Triangle_Index + 4] = downverts_list[indexis];
                Mesh_Triangles[Triangle_Index + 3] = upverts_list[indexis - 1];
                Triangle_Index += 6;
            }

            for (int indexis = Y_Subdivided_Length + 1; indexis < upverts_list.Length - Y_Subdivided_Length - 2; indexis += 2)
            {
                Mesh_Triangles[Triangle_Index + 2] = downverts_list[indexis + 2];
                Mesh_Triangles[Triangle_Index + 1] = upverts_list[indexis + 2];
                Mesh_Triangles[Triangle_Index + 0] = upverts_list[indexis];
                Mesh_Triangles[Triangle_Index + 5] = downverts_list[indexis];
                Mesh_Triangles[Triangle_Index + 4] = downverts_list[indexis + 2];
                Mesh_Triangles[Triangle_Index + 3] = upverts_list[indexis];
                Triangle_Index += 6;
            }

            for (int indexis = upverts_list.Length - Y_Subdivided_Length - 2; indexis > Y_Subdivided_Length; indexis -= 2)
            {
                Mesh_Triangles[Triangle_Index + 0] = downverts_list[indexis];
                Mesh_Triangles[Triangle_Index + 1] = upverts_list[indexis];
                Mesh_Triangles[Triangle_Index + 2] = upverts_list[indexis - 2];
                Mesh_Triangles[Triangle_Index + 3] = downverts_list[indexis - 2];
                Mesh_Triangles[Triangle_Index + 4] = downverts_list[indexis];
                Mesh_Triangles[Triangle_Index + 5] = upverts_list[indexis - 2];
                Triangle_Index += 6;
            }

            Mesh_Triangles[Triangle_Index + 2] = downverts_list[0];
            Mesh_Triangles[Triangle_Index + 1] = upverts_list[Y_VertCount];
            Mesh_Triangles[Triangle_Index + 0] = upverts_list[0];
            Mesh_Triangles[Triangle_Index + 5] = downverts_list[0];
            Mesh_Triangles[Triangle_Index + 4] = downverts_list[Y_VertCount];
            Mesh_Triangles[Triangle_Index + 3] = upverts_list[Y_VertCount];
            Triangle_Index += 6;

            Mesh_Triangles[Triangle_Index + 2] = downverts_list[upverts_list.Length - 1];
            Mesh_Triangles[Triangle_Index + 1] = upverts_list[upverts_list.Length - 1 - Y_VertCount];
            Mesh_Triangles[Triangle_Index + 0] = upverts_list[upverts_list.Length - 1];
            Mesh_Triangles[Triangle_Index + 5] = downverts_list[upverts_list.Length - 1];
            Mesh_Triangles[Triangle_Index + 4] = downverts_list[upverts_list.Length - 1 - Y_VertCount];
            Mesh_Triangles[Triangle_Index + 3] = upverts_list[upverts_list.Length - 1 - Y_VertCount];
            Triangle_Index += 6;



            Mesh_Triangles[Triangle_Index + 2] = downverts_list[0];
            Mesh_Triangles[Triangle_Index + 1] = downverts_list[Y_Subdivided_Length];
            Mesh_Triangles[Triangle_Index + 0] = downverts_list[upverts_list.Length - 1];
            Mesh_Triangles[Triangle_Index + 3] = downverts_list[0];
            Mesh_Triangles[Triangle_Index + 4] = downverts_list[upverts_list.Length - Y_VertCount];
            Mesh_Triangles[Triangle_Index + 5] = downverts_list[upverts_list.Length - 1];
        }

        {
            mesh.Clear();
            mesh.vertices = Mesh_Vertecies;
            mesh.triangles = Mesh_Triangles;
            mesh.RecalculateNormals();
            mesh.uv = uv;
            mesh.name = "LandMesh";
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (VertexGizmoDrawMode == GizmoDrawModeEnum.Selected)
        {
            foreach (Vector3 point in Mesh_Vertecies)
            {
                Gizmos.DrawSphere(point, 0.02f);
            }
        }

    }

    private void OnDrawGizmos()
    {
        if (VertexGizmoDrawMode == GizmoDrawModeEnum.Always)
        {
            foreach (Vector3 point in Mesh_Vertecies)
            {
                Gizmos.DrawSphere(point, 0.02f);
            }
        }
    }
}
