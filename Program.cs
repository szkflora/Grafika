using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;

namespace Szeminarium1
{
    internal static class Program
    {
        private static IWindow graphicWindow;

        private static GL Gl;

        private static uint program;

        private static readonly string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 vPos;
		layout (location = 1) in vec4 vCol;

		out vec4 outCol;
        
        void main()
        {
			outCol = vCol;
            gl_Position = vec4(vPos.x, vPos.y, vPos.z, 1.0);
            //gl_Position = vec4(vPos.x, vPos.y, 1.0);
            // Unhandled Exception: System.Exception: Vertex shader failed to compile: ERROR: 0:13: 'constructor' : not enough data provided for construction
        }
        ";


        private static readonly string FragmentShaderSource = @"
        #version 330 core
        out vec4 FragColor;
		
		in vec4 outCol;

        void main()
        {
            FragColor = outCol;
        }
        ";
        
        static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Title = "lab1-3";
            windowOptions.Size = new Silk.NET.Maths.Vector2D<int>(500, 500);

            graphicWindow = Window.Create(windowOptions);

            graphicWindow.Load += GraphicWindow_Load;
            graphicWindow.Update += GraphicWindow_Update;
            graphicWindow.Render += GraphicWindow_Render;

            graphicWindow.Run();
        }

        private static void GraphicWindow_Load()
        {
            // egszeri beallitasokat
            //Console.WriteLine("Loaded");

            Gl = graphicWindow.CreateOpenGL();

            Gl.ClearColor(System.Drawing.Color.White);

            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, VertexShaderSource);
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            Gl.ShaderSource(fshader, FragmentShaderSource);
            Gl.CompileShader(fshader);

            //kiegeszites, hibakezeles
            Gl.GetShader(fshader, ShaderParameterName.CompileStatus, out int fStatus);
            if (fStatus != (int)GLEnum.True)
                throw new Exception("Fragment shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            program = Gl.CreateProgram();
            Gl.AttachShader(program, vshader);
            Gl.AttachShader(program, fshader);
            Gl.LinkProgram(program);
            Gl.DetachShader(program, vshader);
            Gl.DetachShader(program, fshader);
            Gl.DeleteShader(vshader);
            Gl.DeleteShader(fshader);

            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(program)}");
            }

        }

        private static void CheckGLError(string function)
        {
            GLEnum errorCode = Gl.GetError();
            if(errorCode != GLEnum.NoError)
            {
                Console.WriteLine($"Error after {function} {errorCode}");
            }
        }

        private static void GraphicWindow_Update(double deltaTime)
        {
            // NO GL
            // make it threadsave
            //Console.WriteLine($"Update after {deltaTime} [s]");
        }

        private static unsafe void GraphicWindow_Render(double deltaTime)
        {
            //Console.WriteLine($"Render after {deltaTime} [s]");

            Gl.Clear(ClearBufferMask.ColorBufferBit);

            uint vao = Gl.GenVertexArray();
            Gl.BindVertexArray(vao);

            float[] vertexArray = new float[] {

                0.0f, -0.5f, 0.0f,
                0.5f, (float)(Math.Sqrt(3)/6 - 0.5), 0.0f,
                0.0f, 0.0f, 0.0f, // 0 1 2

                0.5f, (float)((1 + Math.Sqrt(3)/3)/2 - 0.5), 0.0f, // 1 2 3

                0.5f, (float)((1 + Math.Sqrt(3)/3)/2 - 0.5), 0.0f,
                0.0f, 0.0f, 0.0f,
                0.0f, (float)((1 + 2*Math.Sqrt(3)/3)/2 - 0.5), 0.0f, // 3 2 4

                -0.5f, (float)((1 + Math.Sqrt(3)/3)/2 - 0.5), 0.0f, // 2 4 5

                -0.5f, (float)((1 + Math.Sqrt(3)/3)/2 - 0.5), 0.0f,
                0.0f, 0.0f, 0.0f,
                -0.5f, (float)(Math.Sqrt(3)/6 - 0.5), 0.0f, // 5 2 6

                0.0f, -0.5f, 0.0f, // 2 6 0   - 11

                // a fuggoleges vonal menti pontok
                //-0.5f, (float)(Math.Sqrt((3)/(36)) - 0.5 + (1)/(6)), 0.0f, // M
                //-0.4994432591673f, -0.0196082999766f, 0.0f, // M'
                //-0.5f ,(float)(Math.Sqrt((3)/(36)) - 0.5+ (2)/(6)), 0.0f, // P
                //-0.5026833382331f, 0.1490164678381f, 0.0f, // P'
                //0.0f, -0.5f + 2/6f, 0.0f, // Q
                //0.00056116542f, -0.1408595272938f, 0.0f, // Q'
                //0.0f, -0.5f + 1/6f, 0.0f, // W
                //0.0005142113165f, -0.3082436096906f, 0.0f, // W'
                //0.5f, (float)(Math.Sqrt((3)/(36)) - 0.5 + (1)/(6)), 0.0f, // O
                //0.5001471656535f, -0.0196082999766f, 0.0f, // O'
                //0.5f ,(float)(Math.Sqrt((3)/(36)) - 0.5+ (2)/(6)), 0.0f, // T
                //0.5001471656535f, 0.1422514970046f, 0.0f, //T'

                //-0.5f + 2/6f, (float)(Math.Sqrt((3)/(36)) - ((2*Math.Sqrt((3)/(36)))/(3))), 0.0f, // JJ
                //-0.143340442318f, 0.0825731567574f, 0.0f, // JJ'
                //-0.5f + 1/6f, (float)(Math.Sqrt((3)/(36)) - ((Math.Sqrt((3)/(36)))/(3))), 0.0f, // KK
                //-0.310203750945f, 0.1792115328029f, 0.0f, // KK'
                //1/6f, (float)(Math.Sqrt((3)/(36)) - ((2*Math.Sqrt((3)/(36)))/(3))), 0.0f, // OO
                //2/6f, (float)(Math.Sqrt((3)/(36)) - ((Math.Sqrt((3)/(36)))/(3))), 0.0f, // GG
                //0.3089619045248f, 0.177744405262f, 0.0f, // GG'

                //-0.5f + 1/6f, (float)(Math.Sqrt(((3)/(36))) - 0.5f - ((Math.Sqrt((3)/(36)))/(3))), 0.0f, // TT
                //-0.310203750945f, -0.3215738881519f, 0.0f, // TT'
                //-0.5f + 2/6f, (float)(Math.Sqrt(((3)/(36))) - 0.5f - ((2*Math.Sqrt((3)/(36)))/(3))), 0.0f, // RR
                //-0.143340442318f, -0.4174054434073f, 0.0f // RR'
                //1/6f, (float)(Math.Sqrt(((3)/(36))) - 0.5f - ((2*Math.Sqrt((3)/(36)))/(3))), 0.0f, // II
                //2/6f, (float)(Math.Sqrt(((3)/(36))) - 0.5f - ((Math.Sqrt((3)/(36)))/(3))), 0.0f, // HH
                //0.3107285954116f, -0.3222291156933f, 0.0f, // HH'

                //-0.5f + 1/6f, (float)((1 + Math.Sqrt((3)/(9)))/(2) - 0.5 + Math.Sqrt((3)/(9))/(3)), 0.0f, // UU
                //-0.5f + 2/6f, (float)((1 + Math.Sqrt((3)/(9)))/(2) - 0.5 + 2*Math.Sqrt((3)/(9))/(3)), 0.0f, // FF
                //-0.1857115437702f, 0.4727817833522f, 0.0f, // FF'
                //1/6f, (float)((1 + Math.Sqrt((3)/(9)))/(2) - 0.5 + 2*Math.Sqrt((3)/(9))/(3)), 0.0f, // YY
                // 0.1889137073123f, 0.4675407051271f, 0.0f // YY'
                //2/6f, (float)((1 + Math.Sqrt((3)/(9)))/(2) - 0.5 + Math.Sqrt((3)/(9))/(3)), 0.0f, // VV
                //0.3566381578468f, 0.3716981619645f, 0.0f // VV'

                // KK KK' TT
                -0.5f + 1/6f, (float)(Math.Sqrt(3)/6 - (Math.Sqrt(3)/6/3)), 0.0f, // KK - 12
                -0.310203750945f, 0.1792115328029f, 0.0f,
                -0.5f + 1/6f, (float)(Math.Sqrt(3)/6 - (Math.Sqrt(3)/6/3) - 0.5f), 0.0f,

                // KK' TT TT'

                 //-0.5f + 1/6f, (float)(Math.Sqrt(3)/6 - (Math.Sqrt(3)/6/3) - 0.5f), 0.0f,
                 -0.310203750945f, -0.3215738881519f, 0.0f,
                 //-0.310203750945f, 0.1792115328029f, 0.0f,

                 // KK KK' YY
                 1/6f, (float)((1.0f + Math.Sqrt(3/9))/2 + 2*((1.0f + Math.Sqrt(3/9))/2 - 0.52f)/3), 0.0f, // - 16

                 // YY YY' KK'
                 0.1889137073123f, 0.4675407051271f, 0.0f,

                 // II II' OO
                 1/6f, (float)(Math.Sqrt(3)/6 - (2*Math.Sqrt(3)/6/3) - 0.5), 0.0f,
                 0.1457845628891f, -0.415808067688f, 0.0f,
                 1/6f, (float)(Math.Sqrt(3)/6 - (2*Math.Sqrt(3)/6/3)), 0.0f,

                 // II' OO OO'
                 0.1425898114504f, 0.0825731567574f, 0.0f,

                 // OO OO' UU'
                 -0.3541940372756f, 0.3716981619645f, 0.0f,

                 // OO UU' UU
                 -0.5f + 1/6f, (float)((1.0f + Math.Sqrt(3/9))/2 + ((1.0f + Math.Sqrt(3/9))/2 - 0.85f)/3), 0.0f,

                 // RR RR' JJ
                 -0.5f + 2/6f, (float)(Math.Sqrt(3)/6 - (2*Math.Sqrt(3)/6/3) - 0.5), 0.0f, // -24
                 -0.143340442318f, -0.4174054434073f, 0.0f,
                 -0.5f + 2/6f, (float)(Math.Sqrt(3)/6 - (2*Math.Sqrt(3)/6/3)), 0.0f,

                 // RR' JJ JJ'
                 -0.143340442318f, 0.0825731567574f, 0.0f,

                 // JJ' VV VV'
                 0.3566381578468f, 0.3716981619645f, 0.0f,

                 // JJ JJ' VV
                 2/6f, (float)((1.0f + Math.Sqrt(3/9))/2 + 2*((1.0f + Math.Sqrt(3/9))/2 - 0.67f)/3), 0.0f,

                 // HH HH' GG
                 2/6f, (float)(Math.Sqrt(3)/6 - (Math.Sqrt(3)/6/3) - 0.5f), 0.0f, // -30
                 0.3089619045248f, -0.3222291156933f, 0.0f,
                 2/6f, (float)(Math.Sqrt(3)/6 - (Math.Sqrt(3)/6/3)), 0.0f,

                 // HH' GG GG'
                 0.3089619045248f, 0.177744405262f, 0.0f,

                 // GG GG' FF
                 -0.5f + 2/6f, (float)((1.0f + Math.Sqrt(3/9))/2 + 2*((1.0f + Math.Sqrt(3/9))/2 - 0.52f)/3), 0.0f,

                 // GG' FF FF'
                 -0.1857115437702f, 0.4727817833522f, 0.0f,

                 // P P' Q
                 -0.5f, (float)(Math.Sqrt(3)/6 - 0.5f + 2/6f), 0.0f, // -36
                 -0.502f, 0.1490164678381f, 0.0f,
                 0.0f, -0.5f + 2/6f, 0.0f,

                 // p' Q Q'
                 0.00056116542f, -0.1408595272938f, 0.0f,

                 // Q Q' T
                 0.5f ,(float)(Math.Sqrt(3)/6 - 0.5f + 2/6f), 0.0f,

                 // Q' T T'
                 0.5011615694967f, 0.1485171033727f, 0.0f,

                 // M M' W
                 -0.5f, (float)(Math.Sqrt(3)/6 - 0.5f + 1/6f), 0.0f,
                 -0.4994432591673f, -0.0196082999766f, 0.0f,
                 0.0f, -0.5f + 1/6f, 0.0f,

                 // M' W W'
                 0.0005142113165f, -0.3082436096906f, 0.0f,

                 // W W' O
                 0.5f, (float)(Math.Sqrt(3)/6 - 0.5f + 1/6f), 0.0f,

                 // W' O O'
                 0.5001471656535f, -0.0196082999766f, 0.0f,

            };

            float[] colorArray = new float[] {

                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,

                //1.0f, 0.0f, 0.0f, 1.0f,
                //1.0f, 0.0f, 0.0f, 1.0f,
                1.0f, 0.0f, 0.0f, 1.0f,

                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,

                //0.0f, 1.0f, 0.0f, 1.0f,
                //0.0f, 1.0f, 0.0f, 1.0f,
                0.0f, 1.0f, 0.0f, 1.0f,

                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,

                //0.0f, 0.0f, 1.0f, 1.0f,
                //0.0f, 0.0f, 1.0f, 1.0f,
                0.0f, 0.0f, 1.0f, 1.0f,

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

            uint[] indexArray = new uint[] {
                0, 1, 2,
                1, 2, 3,
                4, 5, 6,
                5, 6, 7,
                8, 9, 10,
                9, 10, 11,

                12, 13, 14,
                13, 14, 15,
                12, 13, 16,
                16, 17, 13,

                18, 19, 20,
                19, 20, 21,
                20, 21, 22,
                20, 22, 23,

                24, 25, 26,
                25, 26, 27,
                26, 27, 29,
                27, 28, 29,

                30, 31, 32,
                31, 32, 33,
                32, 33, 34,
                33, 34, 35,

                36, 37, 38,
                37, 38, 39,
                38, 39, 40,
                39, 40, 41,

                42, 43, 44,
                43, 44, 45,
                44, 45, 46,
                45, 46, 47
            };

            uint vertices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, vertices);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)vertexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(0);
            CheckGLError("binding the vertices");

            uint colors = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ArrayBuffer, colors);
            Gl.BufferData(GLEnum.ArrayBuffer, (ReadOnlySpan<float>)colorArray.AsSpan(), GLEnum.StaticDraw);
            Gl.VertexAttribPointer(1, 4, VertexAttribPointerType.Float, false, 0, null);
            Gl.EnableVertexAttribArray(1);
            CheckGLError("binding the colors");

            uint indices = Gl.GenBuffer();
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, indices);
            Gl.BufferData(GLEnum.ElementArrayBuffer, (ReadOnlySpan<uint>)indexArray.AsSpan(), GLEnum.StaticDraw);
            Gl.BindBuffer(GLEnum.ArrayBuffer, 0);
            Gl.UseProgram(program);
            CheckGLError("binding the indices");

            Gl.DrawElements(GLEnum.Triangles, (uint)indexArray.Length, GLEnum.UnsignedInt, null); // we used element buffer
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);
            Gl.BindVertexArray(vao);
            CheckGLError("binding the vertex array");

            //always unbound the vertex buffer first, so no halfway results are displayed by accident
            Gl.DeleteBuffer(vertices);
            Gl.DeleteBuffer(colors);
            Gl.DeleteBuffer(indices);
            Gl.DeleteVertexArray(vao);
        }
    }
}
