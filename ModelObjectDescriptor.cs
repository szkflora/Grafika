using Silk.NET.OpenGL;
//using Silk.NET.Vulkan;
using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Runtime.CompilerServices;
//using System.Text;
//using System.Threading.Tasks;

namespace GrafikaSzeminarium
{
    internal class ModelObjectDescriptor : IDisposable
    {
        private bool disposedValue;

        public uint Vao { get; private set; }
        public uint Vertices { get; private set; }
        public uint Colors { get; private set; }
        public uint Indices { get; private set; }
        public uint IndexArrayLength { get; private set; }

        private GL Gl;

        // counter clockwise is front facing
        private static float[] vertexArray = new float[] {
                -0.5f, 0.5f, 0.5f,
                0.5f, 0.5f, 0.5f,
                0.5f, 0.5f, -0.5f, // felso
                -0.5f, 0.5f, -0.5f,

                -0.5f, 0.5f, 0.5f,
                -0.5f, -0.5f, 0.5f,
                0.5f, -0.5f, 0.5f,  // szemben levo
                0.5f, 0.5f, 0.5f,

                -0.5f, 0.5f, 0.5f,
                -0.5f, 0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f, // bal oldali
                -0.5f, -0.5f, 0.5f,

                -0.5f, -0.5f, 0.5f,
                0.5f, -0.5f, 0.5f, // also
                0.5f, -0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f,

                0.5f, 0.5f, -0.5f,
                -0.5f, 0.5f, -0.5f,
                -0.5f, -0.5f, -0.5f, // hattal levo
                0.5f, -0.5f, -0.5f,

                0.5f, 0.5f, 0.5f,
                0.5f, 0.5f, -0.5f,
                0.5f, -0.5f, -0.5f, // jobb oldali
                0.5f, -0.5f, 0.5f,

            };

        // az iranyok helyzetek x y z sorrendben
        // x lehet : bal/kozep/jobb
        // y lehet : also/kozep/felso
        // z lehet : hatso/kozep/elulso
        // szinek legyenek :
        // also - piros
        // felso - kek
        // hatso - sarga
        // elulso - zold
        // bal - nanancs
        // jobb - rozsaszin


        // kozep kozep kozep
        public static float[] colorArray1 = new float[] {

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
        };

        // bal also hatso
        public static float[] colorArray2 = new float[] {

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f, // nanacs
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,

                207f / 255f, 0f / 255f, 15f / 255f, 1.0f,
                207f / 255f, 0f / 255f, 15f / 255f, 1.0f,
                207f / 255f, 0f / 255f, 15f / 255f, 1.0f, // piros
                207f / 255f, 0f / 255f, 15f / 255f, 1.0f,

                255f / 255f, 240f / 255f, 0f / 255f, 1.0f,
                255f / 255f, 240f / 255f, 0f / 255f, 1.0f,
                255f / 255f, 240f / 255f, 0f / 255f, 1.0f, // sarga
                255f / 255f, 240f / 255f, 0f / 255f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
        };

        // bal also kozep
        public static float[] colorArray3 = new float[] {

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f, // nanacs
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                255f / 255f, 240f / 255f, 0f / 255f, 1.0f,
                255f / 255f, 240f / 255f, 0f / 255f, 1.0f,
                255f / 255f, 240f / 255f, 0f / 255f, 1.0f, // sarga
                255f / 255f, 240f / 255f, 0f / 255f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
        };

        // bal also elulso
        public static float[] colorArray4 = new float[] {

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                178f / 255f, 222f / 255f, 39f / 255f, 1.0f,
                178f / 255f, 222f / 255f, 39f / 255f, 1.0f,
                178f / 255f, 222f / 255f, 39f / 255f, 1.0f, // zold
                178f / 255f, 222f / 255f, 39f / 255f, 1.0f,

                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f, // nanacs
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,

                207f / 255f, 0f / 255f, 15f / 255f, 1.0f,
                207f / 255f, 0f / 255f, 15f / 255f, 1.0f,
                207f / 255f, 0f / 255f, 15f / 255f, 1.0f, // piros
                207f / 255f, 0f / 255f, 15f / 255f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
        };

        // bal kozep hatso
        public static float[] colorArray5 = new float[] {

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f, // nanacs
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                255f / 255f, 240f / 255f, 0f / 255f, 1.0f,
                255f / 255f, 240f / 255f, 0f / 255f, 1.0f,
                255f / 255f, 240f / 255f, 0f / 255f, 1.0f, // sarga
                255f / 255f, 240f / 255f, 0f / 255f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
        };

        // bal kozep kozep
        public static float[] colorArray6 = new float[] {

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f, // nanacs
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
        };

        // bal kozep elulso
        public static float[] colorArray7 = new float[] {

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                178f / 255f, 222f / 255f, 39f / 255f, 1.0f,
                178f / 255f, 222f / 255f, 39f / 255f, 1.0f,
                178f / 255f, 222f / 255f, 39f / 255f, 1.0f, // zold
                178f / 255f, 222f / 255f, 39f / 255f, 1.0f,

                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f, // nanacs
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
        };

        // bal felso hatso
        public static float[] colorArray8 = new float[] {

                30f / 255f, 139f / 255f, 195f / 255f, 1.0f,
                30f / 255f, 139f / 255f, 195f / 255f, 1.0f,
                30f / 255f, 139f / 255f, 195f / 255f, 1.0f, // kek
                30f / 255f, 139f / 255f, 195f / 255f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f, // nanacs
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                255f / 255f, 240f / 255f, 0f / 255f, 1.0f,
                255f / 255f, 240f / 255f, 0f / 255f, 1.0f,
                255f / 255f, 240f / 255f, 0f / 255f, 1.0f, // sarga
                255f / 255f, 240f / 255f, 0f / 255f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
        };

        // bal felso kozep
        public static float[] colorArray9 = new float[] {

                30f / 255f, 139f / 255f, 195f / 255f, 1.0f,
                30f / 255f, 139f / 255f, 195f / 255f, 1.0f,
                30f / 255f, 139f / 255f, 195f / 255f, 1.0f, // kek
                30f / 255f, 139f / 255f, 195f / 255f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f, // nanacs
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
        };

        // bal felso elulso
        public static float[] colorArray10 = new float[] {

                30f / 255f, 139f / 255f, 195f / 255f, 1.0f,
                30f / 255f, 139f / 255f, 195f / 255f, 1.0f,
                30f / 255f, 139f / 255f, 195f / 255f, 1.0f, // kek
                30f / 255f, 139f / 255f, 195f / 255f, 1.0f,

                178f / 255f, 222f / 255f, 39f / 255f, 1.0f,
                178f / 255f, 222f / 255f, 39f / 255f, 1.0f,
                178f / 255f, 222f / 255f, 39f / 255f, 1.0f, // zold
                178f / 255f, 222f / 255f, 39f / 255f, 1.0f,

                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f, // nanacs
                243f / 255f, 156f / 255f, 18f / 255f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
                0.0f, 0.0f, 0.0f, 1.0f,
        };

        // bal oldal befejezve

        private static uint[] indexArray = new uint[] {
                0, 1, 2,
                0, 2, 3,

                4, 5, 6,
                4, 6, 7,

                8, 9, 10,
                10, 11, 8,

                12, 14, 13,
                12, 15, 14,

                17, 16, 19,
                17, 19, 18,

                20, 22, 21,
                20, 23, 22
            };

        public unsafe static ModelObjectDescriptor CreateCube(GL Gl, float[] colorArray)
        {
            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            uint vertices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(0);
            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);

            uint colors = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(1);
            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);

            uint indices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);

            return new ModelObjectDescriptor() { Vao = vao, Vertices = vertices, Colors = colors, Indices = indices, IndexArrayLength = (uint)indexArray.Length, Gl = Gl };
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null


                // always unbound the vertex buffer first, so no halfway results are displayed by accident
                Gl.DeleteBuffer(Vertices);
                Gl.DeleteBuffer(Colors);
                Gl.DeleteBuffer(Indices);
                Gl.DeleteVertexArray(Vao);

                disposedValue = true;
            }
        }

        ~ModelObjectDescriptor()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
