﻿using System;

namespace CSharpGL
{
    internal class ZeroIndexPointInTriangleStripSearcher : ZeroIndexPointSearcher
    {
        /// <summary>
        ///
        /// </summary>
        /// <param name="arg"></param>
        /// <param name="lastVertexId"></param>
        /// <param name="picker"></param>
        /// <returns></returns>
        internal override uint Search(PickingEventArgs arg,
            uint lastVertexId, ZeroIndexPicker picker)
        {
            OneIndexBuffer buffer = GLBuffer.Create(IndexBufferElementType.UInt, 3, DrawMode.Points, BufferUsage.StaticDraw);
            unsafe
            {
                var array = (uint*)buffer.MapBuffer(MapBufferAccess.WriteOnly);
                array[0] = lastVertexId - 0;
                array[1] = lastVertexId - 1;
                array[2] = lastVertexId - 2;
                buffer.UnmapBuffer();
            }
            picker.Node.Render4InnerPicking(arg, buffer);
            uint id = ColorCodedPicking.ReadStageVertexId(arg.X, arg.Y);

            buffer.Dispose();

            if (lastVertexId - 2 <= id && id <= lastVertexId - 0)
            { return id; }
            else
            { throw new Exception("This should not happen!"); }
        }
    }
}