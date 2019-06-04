using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace mesh
{
    public abstract class ABaseMesh : MonoBehaviour
    {
        public bool m_useCollider;
        public bool m_doVertexColors;
        public bool m_doUV;
        public bool m_doTexArrayIndices;

        protected Mesh m_mesh = null;
        protected MeshCollider m_meshCollider = null;

        protected List<Vector3> m_vertices = new List<Vector3>();
        protected List<int> m_indices = new List<int>();
        protected List<Vector2> m_uv0 = new List<Vector2>();
        protected List<Vector3> m_uv1 = new List<Vector3>();
        protected List<Color> m_vertexColors = new List<Color>();

        protected virtual void Awake()
        {
            GetComponent<MeshFilter>().mesh = m_mesh = new Mesh();
            if (m_useCollider)
                m_meshCollider = gameObject.AddComponent<MeshCollider>();
        }

        protected void Clear()
        {
            m_mesh.Clear();

            m_vertices.Clear();
            m_indices.Clear();
            m_vertexColors.Clear();
            m_uv0.Clear();
            m_uv1.Clear();

            if (m_meshCollider)
                m_meshCollider.sharedMesh = null;
        }

        protected void Apply()
        {
            m_mesh.vertices = m_vertices.ToArray();
            m_mesh.triangles = m_indices.ToArray();

            if (m_doVertexColors)
                m_mesh.colors = m_vertexColors.ToArray();
            if (m_doUV)
                m_mesh.SetUVs(0, m_uv0);
            if (m_doTexArrayIndices)
                m_mesh.SetUVs(1, m_uv1);

            if (m_meshCollider != null)
                m_meshCollider.sharedMesh = m_mesh;

            m_mesh.RecalculateNormals();
        }

        public abstract void Triangulate(int startRow, int lastRow, int startCol, int lastCol);

        //********************
        // Adding data to mesh
        //********************
        protected void AddTriangle(MeshVertex a, MeshVertex b, MeshVertex c, Texture2D noiseTexture, float noiseScale, float strength)
        {
            int vertexIndex = m_vertices.Count;

            m_vertices.Add(PerturbHor(a.m_position, noiseTexture, noiseScale, strength));
            m_vertices.Add(PerturbHor(b.m_position, noiseTexture, noiseScale, strength));
            m_vertices.Add(PerturbHor(c.m_position, noiseTexture, noiseScale, strength));

            m_indices.Add(vertexIndex);
            m_indices.Add(vertexIndex + 1);
            m_indices.Add(vertexIndex + 2);

            if (m_doVertexColors)
            {
                m_vertexColors.Add(a.m_color);
                m_vertexColors.Add(b.m_color);
                m_vertexColors.Add(c.m_color);
            }
            if (m_doUV)
            {
                m_uv0.Add(a.m_uv0);
                m_uv0.Add(b.m_uv0);
                m_uv0.Add(c.m_uv0);
            }
        }
        protected void AddTriangle(MeshVertex a, MeshVertex b, MeshVertex c)
        {
            int vertexIndex = m_vertices.Count;

            m_vertices.Add(a.m_position);
            m_vertices.Add(b.m_position);
            m_vertices.Add(c.m_position);

            m_indices.Add(vertexIndex);
            m_indices.Add(vertexIndex + 1);
            m_indices.Add(vertexIndex + 2);

            if (m_doVertexColors)
            {
                m_vertexColors.Add(a.m_color);
                m_vertexColors.Add(b.m_color);
                m_vertexColors.Add(c.m_color);
            }
            if (m_doUV)
            {
                m_uv0.Add(a.m_uv0);
                m_uv0.Add(b.m_uv0);
                m_uv0.Add(c.m_uv0);
            }
        }

        protected void AddTriangle(MeshVertex a, MeshVertex b, MeshVertex c, Texture2D noiseTexture, float noiseScale, float strength, Vector3 texArrayIndices)
        {
            AddTriangle(a, b, c,noiseTexture,noiseScale, strength);

            m_uv1.Add(texArrayIndices);
            m_uv1.Add(texArrayIndices);
            m_uv1.Add(texArrayIndices);
        }

        protected void AddQuad(MeshVertex a, MeshVertex b, MeshVertex c, MeshVertex d, Texture2D noiseTexture, float noiseScale, float strength)
        {
            AddTriangle(a, b, d, noiseTexture,noiseScale,strength);
            AddTriangle(d, b, c, noiseTexture, noiseScale, strength);
        }
        protected void AddQuad(MeshVertex a, MeshVertex b, MeshVertex c, MeshVertex d,Texture2D noiseTexture, float noiseScale, float strength, Vector3 texArrayIndices)
        {
            AddTriangle(a, b, d, noiseTexture, noiseScale, strength, texArrayIndices);
            AddTriangle(d, b, c, noiseTexture, noiseScale, strength, texArrayIndices);
        }

        protected void AddTriangleFan(MeshVertex center, MeshVertex[] clockwiseVertices, Texture2D noiseTexture, float noiseScale, float strength)
        {
            for (int i = 0; i < clockwiseVertices.Length - 1; i++)
                AddTriangle(center, clockwiseVertices[i], clockwiseVertices[i + 1], noiseTexture, noiseScale, strength);
        }
        protected void AddTriangleFan(MeshVertex center, MeshVertex[] clockwiseVertices, Texture2D noiseTexture, float noiseScale, float strength, Vector3 texArrayIndices)
        {
            for (int i = 0; i < clockwiseVertices.Length - 1; i++)
                AddTriangle(center, clockwiseVertices[i], clockwiseVertices[i + 1], noiseTexture, noiseScale, strength, texArrayIndices);
        }

        protected void AddTriangleStrip(MeshVertex[] edge0, MeshVertex[] edge1, Texture2D noiseTexture, float noiseScale, float strength)
        {
            for (int i = 0; i < edge0.Length - 1; i++)
            {
                AddQuad(edge0[i], edge1[i], edge1[i + 1], edge0[i + 1], noiseTexture, noiseScale, strength);
            }
        }
        protected void AddTriangleStrip(MeshVertex[] edge0, MeshVertex[] edge1, Texture2D noiseTexture, float noiseScale, float strength, Vector3 texArrayIndices)
        {
            for (int i = 0; i < edge0.Length - 1; i++)
            {
                AddQuad(edge0[i], edge1[i], edge1[i + 1], edge0[i + 1], noiseTexture, noiseScale, strength, texArrayIndices);
            }
        }


        //TODO: put this out
        public static Vector3 PerturbHor(Vector3 position, Texture2D perturbTexture, float noiseScale, float strength)
        {
            Vector4 noise = perturbTexture.GetPixelBilinear(position.x * noiseScale, position.z * noiseScale);
            position.x += (noise.x * 2f - 1) * strength;
            position.z += (noise.z * 2f - 1) * strength;
            return position;
        }
        public static Texture2D GenerateGrayScaleTexture(NoiseMap2D noiseMap, FilterMode filterMode)
        {
            Texture2D newTexture = new Texture2D(noiseMap.m_width, noiseMap.m_height);

            Color[] pixels = new Color[noiseMap.m_width * noiseMap.m_height];
            for (int i = 0; i < noiseMap.m_width * noiseMap.m_height; i++)
                pixels[i] = new Color(noiseMap.m_noiseMap[i], noiseMap.m_noiseMap[i], noiseMap.m_noiseMap[i]);

            newTexture.SetPixels(pixels);
            newTexture.filterMode = filterMode;
            newTexture.wrapMode = TextureWrapMode.Clamp;
            newTexture.Apply();

            return newTexture;
        }
        public static Texture2D GenerateRgbTexture(NoiseMap2D textureA, NoiseMap2D textureB, NoiseMap2D textureC, FilterMode filterMode)
        {
            Texture2D newTexture = new Texture2D(textureA.m_width, textureA.m_height);

            Color[] pixels = new Color[textureA.m_width * textureA.m_height];
            for (int i = 0; i < textureA.m_width * textureA.m_height; i++)
                pixels[i] = new Color(textureA.m_noiseMap[i], textureB.m_noiseMap[i], textureB.m_noiseMap[i]);

            newTexture.SetPixels(pixels);
            newTexture.filterMode = filterMode;
            newTexture.wrapMode = TextureWrapMode.Clamp;
            newTexture.Apply();

            return newTexture;
        }
    }
}

