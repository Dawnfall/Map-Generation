using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using math;

namespace mapGen
{
    public static class MeshHelper
    {
        public static void PolygonToMesh(MSNodeBase[] meshNodes, List<Vector3> vertices, List<int> indices, HashSet<Edge<MSNodeBase>>[] outlineEdgesSingleAndMultiple)
        {
            if (meshNodes == null || meshNodes.Length < 3)
                return;

            AssignPolygonVertices(meshNodes, vertices);

            List<Triangle<MSNodeBase>> triangles = new List<Triangle<MSNodeBase>>();
            for (int i = 0; i < meshNodes.Length - 2; i++)
            {
                Triangle<MSNodeBase> newTriangle = new Triangle<MSNodeBase>(meshNodes[0], meshNodes[i + 1], meshNodes[i + 2]);
                triangles.Add(newTriangle);
                AssignTriangleIndices(newTriangle, indices);
                DetectEdges(newTriangle, outlineEdgesSingleAndMultiple);
            }
        }

        private static void AssignPolygonVertices(MSNodeBase[] meshNodes, List<Vector3> vertices)
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
        private static void AssignTriangleIndices(Triangle<MSNodeBase> triangle, List<int> indices)
        {
            indices.Add(triangle.a.m_index);
            indices.Add(triangle.b.m_index);
            indices.Add(triangle.c.m_index);
        }

        static void DetectEdges(Triangle<MSNodeBase> triangle, HashSet<Edge<MSNodeBase>>[] outlineEdgesSingleAndMultiple)
        {
            CheckForOutlineEdge(triangle.AB, outlineEdgesSingleAndMultiple);
            CheckForOutlineEdge(triangle.BC, outlineEdgesSingleAndMultiple);
            CheckForOutlineEdge(triangle.CA, outlineEdgesSingleAndMultiple);
        }
        static void CheckForOutlineEdge(Edge<MSNodeBase> edge, HashSet<Edge<MSNodeBase>>[] outlineEdgesSingleAndMultiple)
        {
            if (outlineEdgesSingleAndMultiple[0].Contains(edge))
            {
                outlineEdgesSingleAndMultiple[0].Remove(edge);
                outlineEdgesSingleAndMultiple[1].Add(edge);
            }
            else
                outlineEdgesSingleAndMultiple[0].Add(edge);
        }
    }
}