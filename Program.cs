using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Windowing;
using System;
using System.Numerics;
using System.Reflection;
using Szeminarium;

namespace GrafikaSzeminarium
{
    internal class Program
    {
        private static IWindow graphicWindow;

        private static GL Gl;

        private static ImGuiController imGuiController;


        private static ModelObjectDescriptor[] cubes = new ModelObjectDescriptor[27];

        private static CameraDescriptor camera = new CameraDescriptor();

        private static CubeArrangementModel cubeArrangementModel = new CubeArrangementModel();

        private const string ModelMatrixVariableName = "uModel";
        private const string NormalMatrixVariableName = "uNormal";
        private const string ViewMatrixVariableName = "uView";
        private const string ProjectionMatrixVariableName = "uProjection";

        private const string LightColorVariableName = "uLightColor";
        private const string LightPositionVariableName = "uLightPos";
        private const string ViewPositionVariableName = "uViewPos";

        private const string ShinenessVariableName = "uShininess";
        private static float shininess = 100;


        private static Vector3 ambientStrength = new Vector3(0.8f, 0.8f, 0.8f);
        private static Vector3 diffuseStrength = new Vector3(0.5f, 0.5f, 0.5f);
        private static Vector3 specularStrength = new Vector3(0.5f, 0.5f, 0.5f);

        private static Vector3 lightColor = new Vector3(1f, 1f, 1f);

        private static uint program;

        static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Title = "Kockak";
            windowOptions.Size = new Silk.NET.Maths.Vector2D<int>(700, 700);

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

            //for(int i = 0; i < 27; i++)
            //{
            //    cubes[i] = new ModelObjectDescriptor();
            //}

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


            imGuiController = new ImGuiController(Gl, graphicWindow, inputContext);


            Gl.ClearColor(System.Drawing.Color.White);

            Gl.Enable(EnableCap.CullFace);
            Gl.CullFace(TriangleFace.Back);

            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthFunc(DepthFunction.Lequal);


            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, GetEmbeddedResourceAsString("Shaders.VertexShader.vert"));
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            Gl.ShaderSource(fshader, GetEmbeddedResourceAsString("Shaders.FragmentShader.frag"));
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

        private static string GetEmbeddedResourceAsString(string resourceRelativePath)
        {
            string resourceFullPath = Assembly.GetExecutingAssembly().GetName().Name + "." + resourceRelativePath;

            using (var resStream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceFullPath))
            using (var resStreamReader = new System.IO.StreamReader(resStream))
            {
                var text = resStreamReader.ReadToEnd();
                return text;
            }
        }

        private static void Keyboard_KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            switch (key)
            {
                case Key.C:
                    //camera.DecreaseZXAngle();
                    camera.RotateInPlace = true;
                    break;
                // forgatas y tengely korul
                case Key.Left:
                    camera.DecreaseZXAngle();
                    //camera.RotateLeft();
                    break;
                case Key.Right:
                    camera.IncreaseZXAngle();
                    break;
                // elore tolas a z tengelyen
                case Key.Down:
                    camera.DecreaseDistance();
                    break;
                case Key.Up:
                    camera.IncreaseDistance();
                    break;
                // forgatas x tengely korul
                case Key.D:
                    //camera.IncreaseZYAngle();
                    camera.MoveDown();
                    break;
                case Key.U:
                    //camera.DecreaseZYAngle();
                    camera.MoveUp();
                    break;
                case Key.L:
                    camera.MoveLeft();
                    break;
                case Key.R:
                    camera.MoveRight();
                    break;
                case Key.I:
                    camera.RotateRight();
                    break;
                case Key.J:
                    camera.RotateLeft();
                    break;
                case Key.M:
                    camera.RotateUp();
                    break;
                case Key.N:
                    camera.RotateDown();
                    break;
                case Key.Space:
                    //cubeArrangementModel.GlobalRotationX += (float)(Math.PI / 2);
                    cubeArrangementModel.AnimationEnabled = true;
                    cubeArrangementModel.Forward = true;
                    break;
                case Key.Backspace:
                    cubeArrangementModel.AnimationEnabled = true;
                    cubeArrangementModel.Forward = false;
                    break;
            }
        }

        private static void GraphicWindow_Update(double deltaTime)
        {
            // NO OpenGL
            // make it threadsafe
            cubeArrangementModel.AdvanceTime(deltaTime);
            imGuiController.Update((float)deltaTime);

        }

        private static unsafe void GraphicWindow_Render(double deltaTime)
        {

            Gl.Clear(ClearBufferMask.ColorBufferBit);
            Gl.Clear(ClearBufferMask.DepthBufferBit);

            Gl.UseProgram(program);

            SetUniform3("ambientStrength", ambientStrength);
            SetUniform3("diffuseStrength", diffuseStrength);
            SetUniform3("specularStrength", specularStrength);

            SetUniform3(LightColorVariableName, lightColor);
            SetUniform3(LightPositionVariableName, new Vector3(0f, 1.2f, 0f));
            SetUniform3(ViewPositionVariableName, new Vector3(camera.Position.X, camera.Position.Y, camera.Position.Z));
            SetUniform1(ShinenessVariableName, shininess);


            var viewMatrix = Matrix4X4.CreateLookAt(camera.Position, camera.Target, camera.UpVector);
            SetMatrix(viewMatrix, ViewMatrixVariableName);

            var projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView<float>((float)(Math.PI / 2), 1024f / 768f, 0.1f, 100f);
            SetMatrix(projectionMatrix, ProjectionMatrixVariableName);


            var modelMatrixCenterCube = Matrix4X4.CreateScale((float)cubeArrangementModel.CenterCubeScale);
            SetModelMatrix(modelMatrixCenterCube);

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

            //float angle = (float)(Math.PI / 2);
            Matrix4X4<float> rotationMatrix = Matrix4X4.CreateRotationX(cubeArrangementModel.GlobalRotationX);

            // bal also hatso
            Matrix4X4<float> trans1 = Matrix4X4.CreateTranslation(-1.1f, -1.1f, -1.1f);
            SetModelMatrix(trans1 * rotationMatrix);
            DrawModelObject(cubes[1]);

            // bal also kozep
            Matrix4X4<float> trans2 = Matrix4X4.CreateTranslation(-1.1f, -1.1f, 0f);
            SetModelMatrix(trans2 * rotationMatrix);
            DrawModelObject(cubes[2]);

            // bal also elulso
            Matrix4X4<float> trans3 = Matrix4X4.CreateTranslation(-1.1f, -1.1f, 1.1f);
            SetModelMatrix(trans3 * rotationMatrix);
            DrawModelObject(cubes[3]);

            // bal kozep hatso
            Matrix4X4<float> trans4 = Matrix4X4.CreateTranslation(-1.1f, 0f, -1.1f);
            SetModelMatrix(trans4 * rotationMatrix);
            DrawModelObject(cubes[4]);

            // bal kozep kozep
            Matrix4X4<float> trans5 = Matrix4X4.CreateTranslation(-1.1f, 0f, 0f);
            SetModelMatrix(trans5 * rotationMatrix);
            DrawModelObject(cubes[5]);

            // bal kozep elulso
            Matrix4X4<float> trans6 = Matrix4X4.CreateTranslation(-1.1f, 0f, 1.1f);
            SetModelMatrix(trans6 * rotationMatrix);
            DrawModelObject(cubes[6]);

            // bal felso hatso
            Matrix4X4<float> trans7 = Matrix4X4.CreateTranslation(-1.1f, 1.1f, -1.1f);
            SetModelMatrix(trans7 * rotationMatrix);
            DrawModelObject(cubes[7]);

            // bal felso kozep
            Matrix4X4<float> trans8 = Matrix4X4.CreateTranslation(-1.1f, 1.1f, 0f);
            SetModelMatrix(trans8 * rotationMatrix);
            DrawModelObject(cubes[8]);

            // bal felso elulso
            Matrix4X4<float> trans9 = Matrix4X4.CreateTranslation(-1.1f, 1.1f, 1.1f);
            SetModelMatrix(trans9 * rotationMatrix);
            DrawModelObject(cubes[9]);

            // kozep also hatso
            Matrix4X4<float> trans10 = Matrix4X4.CreateTranslation(0f, -1.1f, -1.1f);
            SetModelMatrix(trans10);
            DrawModelObject(cubes[10]);

            // kozep also kozep
            Matrix4X4<float> trans11 = Matrix4X4.CreateTranslation(0f, -1.1f, 0f);
            SetModelMatrix(trans11);
            DrawModelObject(cubes[11]);

            // kozep also elulso
            Matrix4X4<float> trans12 = Matrix4X4.CreateTranslation(0f, -1.1f, 1.1f);
            SetModelMatrix(trans12);
            DrawModelObject(cubes[12]);

            // kozep kozep hatso
            Matrix4X4<float> trans13 = Matrix4X4.CreateTranslation(0f, 0f, -1.1f);
            SetModelMatrix(trans13);
            DrawModelObject(cubes[13]);

            // kozep kozep elulso
            Matrix4X4<float> trans14 = Matrix4X4.CreateTranslation(0f, 0f, 1.1f);
            SetModelMatrix(trans14);
            DrawModelObject(cubes[14]);

            // kozep felso hatso
            Matrix4X4<float> trans15 = Matrix4X4.CreateTranslation(0f, 1.1f, -1.1f);
            SetModelMatrix(trans15);
            DrawModelObject(cubes[15]);

            // kozep felso kozep
            Matrix4X4<float> trans16 = Matrix4X4.CreateTranslation(0f, 1.1f, 0f);
            SetModelMatrix(trans16);
            DrawModelObject(cubes[16]);

            // kozep felso elulso
            Matrix4X4<float> trans17 = Matrix4X4.CreateTranslation(0f, 1.1f, 1.1f);
            SetModelMatrix(trans17);
            DrawModelObject(cubes[17]);

            // jobb also hatso
            Matrix4X4<float> trans18 = Matrix4X4.CreateTranslation(1.1f, -1.1f, -1.1f);
            SetModelMatrix(trans18);
            DrawModelObject(cubes[18]);

            // jobb also kozep
            Matrix4X4<float> trans19 = Matrix4X4.CreateTranslation(1.1f, -1.1f, 0f);
            SetModelMatrix(trans19);
            DrawModelObject(cubes[19]);

            // jobb also elulso
            Matrix4X4<float> trans20 = Matrix4X4.CreateTranslation(1.1f, -1.1f, 1.1f);
            SetModelMatrix(trans20);
            DrawModelObject(cubes[20]);

            // jobb kozep hatso
            Matrix4X4<float> trans21 = Matrix4X4.CreateTranslation(1.1f, 0f, -1.1f);
            SetModelMatrix(trans21);
            DrawModelObject(cubes[21]);

            // jobb kozep kozep
            Matrix4X4<float> trans22 = Matrix4X4.CreateTranslation(1.1f, 0f, 0f);
            SetModelMatrix(trans22);
            DrawModelObject(cubes[22]);

            // jobb kozep elulso
            Matrix4X4<float> trans23 = Matrix4X4.CreateTranslation(1.1f, 0f, 1.1f);
            SetModelMatrix(trans23);
            DrawModelObject(cubes[23]);

            // jobb felso hatso
            Matrix4X4<float> trans24 = Matrix4X4.CreateTranslation(1.1f, 1.1f, -1.1f);
            SetModelMatrix(trans24);
            DrawModelObject(cubes[24]);

            // jobb felso kozep
            Matrix4X4<float> trans25 = Matrix4X4.CreateTranslation(1.1f, 1.1f, 0f);
            SetModelMatrix(trans25);
            DrawModelObject(cubes[25]);

            // jobb felso elulso
            Matrix4X4<float> trans26 = Matrix4X4.CreateTranslation(1.1f, 1.1f, 1.1f);
            SetModelMatrix(trans26);
            DrawModelObject(cubes[26]);

            ImGuiNET.ImGui.Begin("Lighting", ImGuiNET.ImGuiWindowFlags.AlwaysAutoResize | ImGuiNET.ImGuiWindowFlags.NoCollapse);

            ImGuiNET.ImGui.Text("Light Color");
            ImGuiNET.ImGui.SliderFloat3("Light Color", ref lightColor, 0f, 1f);

            ImGuiNET.ImGui.End();
            imGuiController.Render();

        }

        private static unsafe void SetModelMatrix(Matrix4X4<float> modelMatrix)
        {
            SetMatrix(modelMatrix, ModelMatrixVariableName);

            // set also the normal matrix
            int location = Gl.GetUniformLocation(program, NormalMatrixVariableName);
            if (location == -1)
            {
                throw new System.Exception($"{NormalMatrixVariableName} uniform not found on shader.");
            }

            // G = (M^-1)^T
            var modelMatrixWithoutTranslation = new Matrix4X4<float>(modelMatrix.Row1, modelMatrix.Row2, modelMatrix.Row3, modelMatrix.Row4);
            modelMatrixWithoutTranslation.M41 = 0;
            modelMatrixWithoutTranslation.M42 = 0;
            modelMatrixWithoutTranslation.M43 = 0;
            modelMatrixWithoutTranslation.M44 = 1;

            Matrix4X4<float> modelInvers;
            Matrix4X4.Invert<float>(modelMatrixWithoutTranslation, out modelInvers);
            Matrix3X3<float> normalMatrix = new Matrix3X3<float>(Matrix4X4.Transpose(modelInvers));

            Gl.UniformMatrix3(location, 1, false, (float*)&normalMatrix);
            CheckError();
        }

        private static unsafe void SetUniform1(string uniformName, float uniformValue)
        {
            int location = Gl.GetUniformLocation(program, uniformName);
            if (location == -1)
            {
                throw new System.Exception($"{uniformName} uniform not found on shader.");
            }

            Gl.Uniform1(location, uniformValue);
            CheckError();
        }

        private static unsafe void SetUniform3(string uniformName, Vector3 uniformValue)
        {
            int location = Gl.GetUniformLocation(program, uniformName);
            if (location == -1)
            {
                throw new System.Exception($"{uniformName} uniform not found on shader.");
            }

            Gl.Uniform3(location, uniformValue);
            CheckError();
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
            Gl.UseProgram(program);

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