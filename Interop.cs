using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

namespace CavalierContours
{
    /// <summary>
    /// Wrapper for the CavalierContours FFI functions, should have no Unity dependencies.
    /// </summary>
    
    
    public class Interop
    {
        private const string DLL_NAME = "cavalier_contours_ffi";

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern IntPtr test();

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int cavc_pline_create(
            CavcVertex[] vertexes,
            uint n_vertexes,
            byte is_closed,
            out IntPtr pline
        );

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int cavc_pline_get_vertex_count(
            IntPtr pline, 
            out uint count);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int cavc_pline_parallel_offset(
            IntPtr pline, 
            double offset, 
            IntPtr options, 
            out IntPtr result);

        [DllImport("cavalier_contours_ffi.dll")]
        private static extern int cavc_pline_get_vertex(IntPtr 
            pline, 
            uint position, 
            out CavcVertex vertex);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int cavc_pline_create_approx_aabbindex(
            IntPtr pline, 
            out IntPtr aabbindex);

        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int cavc_plinelist_get_pline(
            IntPtr plinelist,
            uint position,
            out IntPtr pline);
        
        
        [DllImport(DLL_NAME, CallingConvention = CallingConvention.Cdecl)]
        private static extern int cavc_plinelist_get_count(
            IntPtr plinelist,
            out uint count);
        
        
        public static List<CavcVertex[]> ParallelOffset(CavcVertex[] cavVerts, bool isClosed, double offsetDelta)
        {
            IntPtr plinePointer = GetPline(cavVerts, isClosed);
            return ParallelOffsetInernal(plinePointer, offsetDelta, IntPtr.Zero);
        }
        
        public static List<CavcVertex[]> ParallelOffset(CavcVertex[] cavVerts, bool isClosed, double offsetDelta, OffsetOptions options)
        {
            IntPtr plinePointer = GetPline(cavVerts, isClosed);
            IntPtr optionsPtr = GetOptions(plinePointer, options.PosEqualEps, options.SliceJoinEps, options.OffsetDistEps, options.HandleSelfIntersects);
            var result = ParallelOffsetInernal(plinePointer,  offsetDelta, optionsPtr);
            Marshal.FreeHGlobal(optionsPtr);
            return result;
        }

        private static List<CavcVertex[]> ParallelOffsetInernal(IntPtr plinePointer, double offsetDelta, IntPtr options)
        {
            IntPtr resultListPointer;
            int statusOutput1 = cavc_pline_parallel_offset(plinePointer, offsetDelta, options, out resultListPointer);
            if (statusOutput1 != 0) Debug.Log("Error: cavc_pline_parallel_offset: Rust: Pline is null"); //todo replace with non unity log

            uint offsetPlineCount;
            cavc_plinelist_get_count(resultListPointer, out offsetPlineCount);

            List<CavcVertex[]> result = new();
            
            for (uint i = 0; i < offsetPlineCount; i++)
            {
                uint position = i;
                IntPtr offsettedPlinePointer;
                int statusOutput2 = cavc_plinelist_get_pline(resultListPointer, position, out offsettedPlinePointer);
                if (statusOutput2 != 0) Debug.Log("Error: cavc_pline_parallel_offset: Rust: PlineList is null"); //todo replace with non unity log
                result.Add(VertsFromPline(offsettedPlinePointer));
            }
            return result;
        }
        
        private static IntPtr GetOptions(IntPtr pline, double posEqualEps, double sliceJoinEps, double offsetDistEps, bool HandleSelfIntersects)
        {
            IntPtr aabbindex;
            int result = cavc_pline_create_approx_aabbindex(pline, out aabbindex);
            if (result != 0) Debug.Log("Error: cavc_pline_create_approx_aabbindex: Rust: Pline is null"); //todo replace with non unity log
        
            CavcPlineParallelOffsetO cavcPlineParallelOffsetO = new CavcPlineParallelOffsetO
            {
                AabbIndex = aabbindex, //IntPtr
                PosEqualEps = posEqualEps, //double
                SliceJoinEps = sliceJoinEps, //double
                OffsetDistEps = offsetDistEps, //double
                HandleSelfIntersects = (byte)(HandleSelfIntersects ? 1 : 0) //byte
            };
        
            IntPtr cavcPlineParallelOffsetOPtr = Marshal.AllocHGlobal(Marshal.SizeOf(typeof(CavcPlineParallelOffsetO)));
            Marshal.StructureToPtr(cavcPlineParallelOffsetO, cavcPlineParallelOffsetOPtr, false);
            Marshal.FreeHGlobal(aabbindex);
            
            return cavcPlineParallelOffsetOPtr;
        }
        
        private static IntPtr GetPline(CavcVertex[] cavVerts, bool isClosed)
        {
            IntPtr plinePointer;
            byte is_closed = isClosed ? (byte) 1 : (byte) 0;
            int errorCode = cavc_pline_create(cavVerts, (uint) cavVerts.Length, is_closed, out plinePointer);
            if (errorCode != 0) Debug.Log("Error: cavc_pline_create"); //todo replace with non unity log
            uint count;
            cavc_pline_get_vertex_count(plinePointer, out count);
            Debug.Assert(count == cavVerts.Length); //todo replace with non unity
            return plinePointer;
        }
        
        private static CavcVertex[] VertsFromPline(IntPtr pline)
        {
            uint count;
            cavc_pline_get_vertex_count(pline, out count);
            CavcVertex[] output = new CavcVertex[count];
            for (int i = 0; i < count; i++)
            {
                cavc_pline_get_vertex(pline, (uint) i, out output[i]);
            }
            return output;
        }
    }
    public class OffsetOptions
    {
        public double PosEqualEps = 0.0001;
        public double SliceJoinEps = 0.0001;
        public double OffsetDistEps = 0.0001;
        public bool HandleSelfIntersects = false;
    }
}
