

using System.Collections.Generic;
using System.Linq;
using Sirenix.OdinInspector.Editor.TypeSearch;
using Unity.VisualScripting;
using UnityEngine;

namespace CavalierContours
{
    /// <summary>
    /// Unity friendly wrapper for the wrapper
    /// </summary>
    
    public static class CavcUnity
    {
        public static List<Vector2[]> ParralelOffset(Vector2[] input, float offsetDelta, bool isClosed)
        {
            return Interop.ParallelOffset(input.ToCavcVertexArray(), isClosed, (double)offsetDelta)
                .Select(cavcVertex => cavcVertex.ToVector2Array())
                .ToList();
        }

        public static List<Vector2[]> ParralelOffset(Vector2[] input, float offsetDelta, bool isClosed, OffsetOptions options)
        {
            return Interop.ParallelOffset(input.ToCavcVertexArray(), isClosed, (double)offsetDelta, options)
                .Select(cavcVertex => cavcVertex.ToVector2Array())
                .ToList();
        }
        public static Vector2[] ToVector2Array(this CavcVertex[] cavcVertices)
        {
            var vector2s = new Vector2[cavcVertices.Length];
            for (var i = 0; i < cavcVertices.Length; i++)
            {
                vector2s[i] = new Vector2((float) cavcVertices[i].X, (float) cavcVertices[i].Y);
            }
            return vector2s;
        }
        
        public static CavcVertex[] ToCavcVertexArray(this Vector2[] vector2s)
        {
            var cavcVertices = new CavcVertex[vector2s.Length];
            for (var i = 0; i < vector2s.Length; i++)
            {
                cavcVertices[i] = new CavcVertex(vector2s[i].x, vector2s[i].y);
            }
            return cavcVertices;
        }


    }
}