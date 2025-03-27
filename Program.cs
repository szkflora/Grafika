using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.Windowing;
using System;
using System.Numerics;
using Szeminarium;

namespace GrafikaSzeminarium
{
    internal class Program
    {
        private static IWindow graphicWindow;

        private static GL Gl;

        private static ModelObjectDescriptor[] cubes = new ModelObjectDescriptor[27];

        private static CameraDescriptor camera = new CameraDescriptor();

        private static CubeArrangementModel cubeArrangementModel = new CubeArrangementModel();

        private const string ModelMatrixVariableName = "uModel";
        private const string ViewMatrixVariableName = "uView";
        private const string ProjectionMatrixVariableName = "uProjection";

        private static readonly string VertexShaderSource = @"
        #version 330 core
        layout (location = 0) in vec3 vPos;
		layout (location = 1) in vec4 vCol;

        uniform mat4 uModel;
        uniform mat4 uView;
        uniform mat4 uProjection;

		out vec4 outCol;
        
        void main()
        {
			outCol = vCol;
            gl_Position = uProjection*uView*uModel*vec4(vPos.x, vPos.y, vPos.z, 1.0);
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

        private static uint program;

        static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Title = "Grafika szeminárium";
            windowOptions.Size = new Silk.NET.Maths.Vector2D<int>(500, 500);

            graphicWindow = Window.Create(windowOptions);

            graphicWindow.Load += GraphicWindow_Load;
            graphicWindow.Update += GraphicWindow_Update;
            graphicWindow.Render += GraphicWindow_Render;
            graphicWindow.Closing += GraphicWindow_Closing;

            graphicWindow.Run();
        }

        private static void GraphicWindow_Closing()
        {
            for(int i = 0; i < 27; i++)
            {
                cubes[i].Dispose();
            }
            Gl.DeleteProgram(program);
        }

        private static void GraphicWindow_Load()
        {
            Gl = graphicWindow.CreateOpenGL();

            var inputContext = graphicWindow.CreateInput();
            foreach (var keyboard in inputContext.Keyboards)
            {
                keyboard.KeyDown += Keyboard_KeyDown;
            }

            for(int i = 0; i < 27; i++)
            {
                cubes[i] = new ModelObjectDescriptor();
            }

            cubes[0] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray1);
            cubes[1] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray2);
            cubes[2] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray3);
            cubes[3] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray4);
            cubes[4] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray5);
            cubes[5] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray6);
            cubes[6] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray7);
            cubes[7] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray8);
            cubes[8] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray9);
            cubes[9] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray10);
            cubes[10] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray11);
            cubes[11] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray12);
            cubes[12] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray13);
            cubes[13] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray14);
            cubes[14] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray15);
            cubes[15] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray16);
            cubes[16] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray17);
            cubes[17] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray18);
            cubes[18] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray19);
            cubes[19] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray20);
            cubes[20] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray21);
            cubes[21] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray22);
            cubes[22] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray23);
            cubes[23] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray24);
            cubes[24] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray25);
            cubes[25] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray26);
            cubes[26] = ModelObjectDescriptor.CreateCube(Gl, ModelObjectDescriptor.colorArray27);

            Gl.ClearColor(System.Drawing.Color.White);

            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(TriangleFace.Back);

            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthFunc(DepthFunction.Lequal);


            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, VertexShaderSource);
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            Gl.ShaderSource(fshader, FragmentShaderSource);
            Gl.CompileShader(fshader);
            Gl.GetShader(fshader, ShaderParameterName.CompileStatus, out int fStatus);
            if (fStatus != (int)GLEnum.True)
                throw new Exception("Fragment shader failed to compile: " + Gl.GetShaderInfoLog(fshader));

            program = Gl.CreateProgram();
            Gl.AttachShader(program, vshader);
            Gl.AttachShader(program, fshader);
            Gl.LinkProgram(program);

            Gl.DetachShader(program, vshader);
            Gl.DetachShader(program, fshader);
            Gl.DeleteShader(vshader);
            Gl.DeleteShader(fshader);
            if ((ErrorCode)Gl.GetError() != ErrorCode.NoError)
            {
            }

            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(program)}");
            }
        }

        private static void Keyboard_KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            switch (key)
            {
                case Key.Left:
                    camera.DecreaseZYAngle();
                    break;
                case Key.Right:
                    camera.IncreaseZYAngle();
                    break;
                case Key.Down:
                    camera.IncreaseDistance();
                    break;
                case Key.Up:
                    camera.DecreaseDistance();
                    break;
                case Key.U:
                    camera.IncreaseZXAngle();
                    break;
                case Key.D:
                    camera.DecreaseZXAngle();
                    break;
                case Key.Space:
                    cubeArrangementModel.AnimationEnabled = !cubeArrangementModel.AnimationEnabled;
                    break;
            }
        }

        private static void GraphicWindow_Update(double deltaTime)
        {
            // NO OpenGL
            // make it threadsafe
            cubeArrangementModel.AdvanceTime(deltaTime);
        }

        private static unsafe void GraphicWindow_Render(double deltaTime)
        {
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            Gl.Clear(ClearBufferMask.DepthBufferBit);

            Gl.UseProgram(program);

            var viewMatrix = Matrix4X4.CreateLookAt(camera.Position, camera.Target, camera.UpVector);
            SetMatrix(viewMatrix, ViewMatrixVariableName);

            var projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView<float>((float)(Math.PI / 2), 1024f / 768f, 0.1f, 100f);
            SetMatrix(projectionMatrix, ProjectionMatrixVariableName);


            var modelMatrixCenterCube = Matrix4X4.CreateScale((float)cubeArrangementModel.CenterCubeScale);
            SetMatrix(modelMatrixCenterCube, ModelMatrixVariableName);
            // kozep kozep kozep
            DrawModelObject(cubes[0]);

            //Matrix4X4<float> diamondScale = Matrix4X4.CreateScale(0.25f);
            //Matrix4X4<float> rotx = Matrix4X4.CreateRotationX((float)Math.PI / 4f);
            //Matrix4X4<float> rotz = Matrix4X4.CreateRotationZ((float)Math.PI / 4f);
            //Matrix4X4<float> roty = Matrix4X4.CreateRotationY((float)cubeArrangementModel.DiamondCubeLocalAngle);
            //Matrix4X4<float> trans = Matrix4X4.CreateTranslation(1f, 1f, 0f);
            //Matrix4X4<float> rotGlobalY = Matrix4X4.CreateRotationY((float)cubeArrangementModel.DiamondCubeGlobalYAngle);
            //Matrix4X4<float> dimondCubeModelMatrix = diamondScale * rotx * rotz * roty * trans * rotGlobalY;
            //SetMatrix(dimondCubeModelMatrix, ModelMatrixVariableName);
            //DrawModelObject(cube);

            // bal also hatso
            Matrix4X4<float> trans1 = Matrix4X4.CreateTranslation(-1.1f, -1.1f, -1.1f);
            SetMatrix(trans1, ModelMatrixVariableName);
            DrawModelObject(cubes[1]);

            // bal also kozep
            Matrix4X4<float> trans2 = Matrix4X4.CreateTranslation(-1.1f, -1.1f, 0f);
            SetMatrix(trans2, ModelMatrixVariableName);
            DrawModelObject(cubes[2]);

            // bal also elulso
            Matrix4X4<float> trans3 = Matrix4X4.CreateTranslation(-1.1f, -1.1f, 1.1f);
            SetMatrix(trans3, ModelMatrixVariableName);
            DrawModelObject(cubes[3]);

            // bal kozep hatso
            Matrix4X4<float> trans4 = Matrix4X4.CreateTranslation(-1.1f, 0f, -1.1f);
            SetMatrix(trans4, ModelMatrixVariableName);
            DrawModelObject(cubes[4]);

            // bal kozep kozep
            Matrix4X4<float> trans5 = Matrix4X4.CreateTranslation(-1.1f, 0f, 0f);
            SetMatrix(trans5, ModelMatrixVariableName);
            DrawModelObject(cubes[5]);

            // bal kozep elulso
            Matrix4X4<float> trans6 = Matrix4X4.CreateTranslation(-1.1f, 0f, 1.1f);
            SetMatrix(trans6, ModelMatrixVariableName);
            DrawModelObject(cubes[6]);

            // bal felso hatso
            Matrix4X4<float> trans7 = Matrix4X4.CreateTranslation(-1.1f, 1.1f, -1.1f);
            SetMatrix(trans7, ModelMatrixVariableName);
            DrawModelObject(cubes[7]);

            // bal felso kozep
            Matrix4X4<float> trans8 = Matrix4X4.CreateTranslation(-1.1f, 1.1f, 0f);
            SetMatrix(trans8, ModelMatrixVariableName);
            DrawModelObject(cubes[8]);

            // bal felso elulso
            Matrix4X4<float> trans9 = Matrix4X4.CreateTranslation(-1.1f, 1.1f, 1.1f);
            SetMatrix(trans9, ModelMatrixVariableName);
            DrawModelObject(cubes[9]);

            // kozep also hatso
            Matrix4X4<float> trans10 = Matrix4X4.CreateTranslation(0f, -1.1f, -1.1f);
            SetMatrix(trans10, ModelMatrixVariableName);
            DrawModelObject(cubes[10]);

            // kozep also kozep
            Matrix4X4<float> trans11 = Matrix4X4.CreateTranslation(0f, -1.1f, 0f);
            SetMatrix(trans11, ModelMatrixVariableName);
            DrawModelObject(cubes[11]);

            // kozep also elulso
            Matrix4X4<float> trans12 = Matrix4X4.CreateTranslation(0f, -1.1f, 1.1f);
            SetMatrix(trans12, ModelMatrixVariableName);
            DrawModelObject(cubes[12]);

            // kozep kozep hatso
            Matrix4X4<float> trans13 = Matrix4X4.CreateTranslation(0f, 0f, -1.1f);
            SetMatrix(trans13, ModelMatrixVariableName);
            DrawModelObject(cubes[13]);

            // kozep kozep elulso
            Matrix4X4<float> trans14 = Matrix4X4.CreateTranslation(0f, 0f, 1.1f);
            SetMatrix(trans14, ModelMatrixVariableName);
            DrawModelObject(cubes[14]);

            // kozep felso hatso
            Matrix4X4<float> trans15 = Matrix4X4.CreateTranslation(0f, 1.1f, -1.1f);
            SetMatrix(trans15, ModelMatrixVariableName);
            DrawModelObject(cubes[15]);

            // kozep felso kozep
            Matrix4X4<float> trans16 = Matrix4X4.CreateTranslation(0f, 1.1f, 0f);
            SetMatrix(trans16, ModelMatrixVariableName);
            DrawModelObject(cubes[16]);

            // kozep felso elulso
            Matrix4X4<float> trans17 = Matrix4X4.CreateTranslation(0f, 1.1f, 1.1f);
            SetMatrix(trans17, ModelMatrixVariableName);
            DrawModelObject(cubes[17]);

            // jobb also hatso
            Matrix4X4<float> trans18 = Matrix4X4.CreateTranslation(1.1f, -1.1f, -1.1f);
            SetMatrix(trans18, ModelMatrixVariableName);
            DrawModelObject(cubes[18]);

            // jobb also kozep
            Matrix4X4<float> trans19 = Matrix4X4.CreateTranslation(1.1f, -1.1f, 0f);
            SetMatrix(trans19, ModelMatrixVariableName);
            DrawModelObject(cubes[19]);

            // jobb also elulso
            Matrix4X4<float> trans20 = Matrix4X4.CreateTranslation(1.1f, -1.1f, 1.1f);
            SetMatrix(trans20, ModelMatrixVariableName);
            DrawModelObject(cubes[20]);

            // jobb kozep hatso
            Matrix4X4<float> trans21 = Matrix4X4.CreateTranslation(1.1f, 0f, -1.1f);
            SetMatrix(trans21, ModelMatrixVariableName);
            DrawModelObject(cubes[21]);

            // jobb kozep kozep
            Matrix4X4<float> trans22 = Matrix4X4.CreateTranslation(1.1f, 0f, 0f);
            SetMatrix(trans22, ModelMatrixVariableName);
            DrawModelObject(cubes[22]);

            // jobb kozep elulso
            Matrix4X4<float> trans23 = Matrix4X4.CreateTranslation(1.1f, 0f, 1.1f);
            SetMatrix(trans23, ModelMatrixVariableName);
            DrawModelObject(cubes[23]);

            // jobb felso hatso
            Matrix4X4<float> trans24 = Matrix4X4.CreateTranslation(1.1f, 1.1f, -1.1f);
            SetMatrix(trans24, ModelMatrixVariableName);
            DrawModelObject(cubes[24]);

            // jobb felso kozep
            Matrix4X4<float> trans25 = Matrix4X4.CreateTranslation(1.1f, 1.1f, 0f);
            SetMatrix(trans25, ModelMatrixVariableName);
            DrawModelObject(cubes[25]);

            // jobb felso elulso
            Matrix4X4<float> trans26 = Matrix4X4.CreateTranslation(1.1f, 1.1f, 1.1f);
            SetMatrix(trans26, ModelMatrixVariableName);
            DrawModelObject(cubes[26]);
        }

        private static unsafe void DrawModelObject(ModelObjectDescriptor modelObject)
        {
            Gl.BindVertexArray(modelObject.Vao);
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, modelObject.Indices);
            Gl.DrawElements(PrimitiveType.Triangles, modelObject.IndexArrayLength, DrawElementsType.UnsignedInt, null);
            Gl.BindBuffer(GLEnum.ElementArrayBuffer, 0);
            Gl.BindVertexArray(0);
        }

        private static unsafe void SetMatrix(Matrix4X4<float> mx, string uniformName)
        {
            int location = Gl.GetUniformLocation(program, uniformName);
            if (location == -1)
            {
                throw new Exception($"{ViewMatrixVariableName} uniform not found on shader.");
            }

            Gl.UniformMatrix4(location, 1, false, (float*)&mx);
            CheckError();
        }

        public static void CheckError()
        {
            var error = (ErrorCode)Gl.GetError();
            if (error != ErrorCode.NoError)
                throw new Exception("GL.GetError() returned " + error.ToString());
        }
    }
}