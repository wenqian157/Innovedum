using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ObjReader : MonoBehaviour
{
    public class Obj
    {
        public string name;
        public List<float[]> vertices;
        public List<int[]> faces;

        public void FlipYZ()
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                float[] v = vertices[i];
                vertices[i] = new float[] { v[0], v[2], v[1] };
            }
        }
        public void FlipFaces()
        {
            for (int i = 0; i < faces.Count; i++)

            {
                int[] face = faces[i];
                int[] face2 = new int[face.Length];

                for (int j = 0; j < face.Length; j++)
                {
                    face2[j] = face[face.Length - 1 - j];

                }
                faces[i] = face2;
            }
        }
        // to import obj already flip yz when it is exported from rhino
        // this step correct it's coordinates in unity
        // this is from practical test
        // detail reason needs to be further solve
        public void AdjustCords()
        {
            for (int i = 0; i < vertices.Count; i++)
            {
                float[] v = vertices[i];
                vertices[i] = new float[] { -v[0], v[1], v[2] };
            }
            for (int i = 0; i < faces.Count; i++)
            {
                int[] face = faces[i];
                int[] face2 = new int[face.Length];

                for (int j = 0; j < face.Length; j++)
                {
                    face2[j] = face[face.Length - 1 - j];
                }
                faces[i] = face2;
            }
        }
        public int[] FlattenedTriangles()
        {
            int[] indices = new int[faces.Count * 3];
            int index = 0;
            foreach (int[] face in faces)
            {
                indices[index] = face[0];
                index++;
                indices[index] = face[1];
                index++;
                indices[index] = face[2];
                index++;
            }
            return indices;
        }
    }
    
    public static GameObject ObjToMeshObject(string objString)
    {
        Obj obj = LoadObj(objString);
        Mesh mesh = ObjToMesh(obj, false);
        GameObject meshGO = InitMeshObject(mesh);

        return meshGO;
    }
    private static Obj LoadObj(string objstring)
    {
        Obj myObj = new Obj();
        myObj.vertices = new List<float[]>();
        myObj.faces = new List<int[]>();

        string[] data = objstring.Split(new string[] { ",", "\n" }, StringSplitOptions.None);

        foreach (string line in data)
        {
            // read vertices
            if (line.StartsWith("v "))
            {
                var vertexData = line.Substring(2).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var vertex = new float[]
                {
                    float.Parse(vertexData[0]),
                    float.Parse(vertexData[1]),
                    float.Parse(vertexData[2])
                };
                myObj.vertices.Add(vertex);
            }
            // read ObjFaces
            else if (line.StartsWith("f "))
            {
                var faceData = line.Substring(2).Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                var face = new int[faceData.Length];
                for (int i = 0; i < faceData.Length; i++)
                {
                    // Subtracting 1 to convert OBJ's 1-based indexing to C#'s 0-based indexing
                    face[i] = int.Parse(faceData[i].Split('/')[0]) - 1;
                }
                myObj.faces.Add(face);
            }
        }

        return myObj;
    }
    private static Mesh ObjToMesh(Obj myObj, bool flipYZ=true)
    {
        Mesh mesh = new Mesh();
        mesh.indexFormat = UnityEngine.Rendering.IndexFormat.UInt32;
        if (flipYZ)
        {
            myObj.FlipYZ();
            myObj.FlipFaces();
        }
        myObj.AdjustCords(); // detail see the function comments
        //add vercites
        mesh.vertices = UnityVerticesFromObj(myObj.vertices);
        // TODO color
        //add faces
        mesh.triangles = myObj.FlattenedTriangles();
        // add norms
        mesh.RecalculateNormals();
        return mesh;
    }
    private static GameObject InitMeshObject(Mesh mesh)
    {
        GameObject GO = new GameObject();
        MeshFilter meshFilter = GO.GetComponent<MeshFilter>();
        if (meshFilter == null)
        {
            meshFilter = GO.AddComponent<MeshFilter>();
        }
        meshFilter.mesh = mesh;
        MeshRenderer meshRenderer = GO.GetComponent<MeshRenderer>();
        if (meshRenderer == null)
        {
            meshRenderer = GO.AddComponent<MeshRenderer>();
        }
        //meshRenderer.material = new Material(Shader.Find("Standard"));
        meshRenderer.material = Resources.Load<Material>("White");

        return GO;
    }
    private static Vector3[] UnityVerticesFromObj(List<float[]> vertices)
    {
        Vector3[] unityVerticies = new Vector3[vertices.Count];
        for (int i = 0; i < vertices.Count; i++)
        {
            unityVerticies[i] = new Vector3(vertices[i][0], vertices[i][1], vertices[i][2]);
        }
        return unityVerticies;
    }
}