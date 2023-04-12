using System;
using System.Runtime.InteropServices;

namespace CavalierContours
{
    /// <summary>
    /// Blittable struct for passing options to cavc_pline_parallel_offset.
    /// </summary>
    
    
    [StructLayout(LayoutKind.Sequential)]
    public struct CavcPlineParallelOffsetO
    {
        public IntPtr AabbIndex;
        public double PosEqualEps;
        public double SliceJoinEps;
        public double OffsetDistEps;
        public byte HandleSelfIntersects;
    }
    
    
    [StructLayout(LayoutKind.Sequential)]
    public struct CavcPolyline {
        public IntPtr vertex_data;
        public int vertex_count;
        public bool is_closed;
    
        public CavcPolyline(IntPtr vertex_data, int vertex_count, bool is_closed) {
        
            this.vertex_data = vertex_data;
            this.vertex_count = vertex_count;
            this.is_closed = is_closed;
        }
    }
    
    [StructLayout(LayoutKind.Sequential)]
    public struct CavcVertex
    {
        public double X;
        public double Y;
        public double Bulge;
        
        public CavcVertex(double x, double y, double bulge = 0)
        {
            X = x;
            Y = y;
            Bulge = bulge;
        }
    }
}