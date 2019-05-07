using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace helper
{
    public static class MeshHelper
    {
        public static void PolygonToMesh(MeshNodeBase[] meshNodes, List<Vector3> vertices, List<int> indices, HashSet<Edge<MeshNodeBase>>[] outlineEdgesSingleAndMultiple)
        {
            if (meshNodes == null || meshNodes.Length < 3)
                return;

            AssignPolygonVertices(meshNodes, vertices);

            List<Triangle<MeshNodeBase>> triangles = new List<Triangle<MeshNodeBase>>();
            for (int i = 0; i < meshNodes.Length - 2; i++)
            {
                Triangle<MeshNodeBase> newTriangle = new Triangle<MeshNodeBase>(meshNodes[0], meshNodes[i + 1], meshNodes[i + 2]);
                triangles.Add(newTriangle);
                AssignTriangleIndices(newTriangle, indices);
                DetectEdges(newTriangle, outlineEdgesSingleAndMultiple);
            }
        }

        private static void AssignPolygonVertices(MeshNodeBase[] meshNodes, List<Vector3> vertices)
        {
            foreach (var node in meshNodes)
            {
                if (node.m_index == -1)
                {
                    node.m_index = vertices.Count;
                    vertices.Add(node.m_position);
                }
            }
        }
        private static void AssignTriangleIndices(Triangle<MeshNodeBase> triangle, List<int> indices)
        {
            indices.Add(triangle.a.m_index);
            indices.Add(triangle.b.m_index);
            indices.Add(triangle.c.m_index);
        }

        static void DetectEdges(Triangle<MeshNodeBase> triangle, HashSet<Edge<MeshNodeBase>>[] outlineEdgesSingleAndMultiple)
        {
            CheckForOutlineEdge(triangle.AB, outlineEdgesSingleAndMultiple);
            CheckForOutlineEdge(triangle.BC, outlineEdgesSingleAndMultiple);
            CheckForOutlineEdge(triangle.CA, outlineEdgesSingleAndMultiple);
        }
        static void CheckForOutlineEdge(Edge<MeshNodeBase> edge, HashSet<Edge<MeshNodeBase>>[] outlineEdgesSingleAndMultiple)
        {
            if (outlineEdgesSingleAndMultiple[0].Contains(edge))
            {
                outlineEdgesSingleAndMultiple[0].Remove(edge);
                outlineEdgesSingleAndMultiple[1].Add(edge);
            }
            else if (!outlineEdgesSingleAndMultiple[1].Contains(edge))
                outlineEdgesSingleAndMultiple[0].Add(edge);
        }
    }
}