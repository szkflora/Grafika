using ImGuiNET;
using Silk.NET.Core;
using Silk.NET.GLFW;
using Silk.NET.Input;
using Silk.NET.Maths;
using Silk.NET.OpenGL;
using Silk.NET.OpenGL.Extensions.ImGui;
using Silk.NET.Vulkan;
using Silk.NET.Windowing;
using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using System.Text.RegularExpressions;

namespace Szeminarium1_24_02_17_2
{
    internal static class Program
    {
        private static CameraDescriptor cameraDescriptor = new();

        private static SnailArrangementModel snailArrangementModel = new();

        private static ButterflyArrangementModel butterflyArrangementModel = new();
        private static FlowerArrangementModel flowerArrangementModel = new();
        private static ImGuiController controller;

        private static IWindow window;

        private static IInputContext inputContext;

        private static GL Gl;

        private static float butTime = 0.0f;
        private static float snailRotationY = 0.0f;
        private static float moveStep = 1f;
        private static float turnStep = 5f * (float)(Math.PI / 180f);
        private static Random rnd = new Random();
        private static int[][] flowers = new int[50][];

        private static Vector3D<float> snailPosition = new(0, 0, 0);
        private static int viewIndex = 0;


        //private static ImGuiController controller;

        private static uint program;

        private static GlObject field;
        private static GlObject snail_body;
        private static GlObject snail_shell;
        private static GlObject butterfly_body;
        private static GlObject butterfly_wing1;
        private static GlObject butterfly_wing2;
        private static GlObject flower1;
        private static GlObject flower2;
        private static GlCube skyBox;

        private static float snailRadius = 0.8f;
        private static float flowerRadius = 0.1f;

        private static float Shininess = 50;

        private const string ModelMatrixVariableName = "uModel";
        private const string NormalMatrixVariableName = "uNormal";
        private const string ViewMatrixVariableName = "uView";
        private const string ProjectionMatrixVariableName = "uProjection";

        private const string TextureUniformVariableName = "uTexture";

        private const string LightColorVariableName = "lightColor";
        private const string LightPositionVariableName = "lightPos";
        private const string ViewPosVariableName = "viewPos";
        private const string ShininessVariableName = "shininess";

        static void Main(string[] args)
        {
            WindowOptions windowOptions = WindowOptions.Default;
            windowOptions.Title = "2 szeminárium";
            windowOptions.Size = new Vector2D<int>(1000, 1000);

            // on some systems there is no depth buffer by default, so we need to make sure one is created
            windowOptions.PreferredDepthBufferBits = 24;

            window = Window.Create(windowOptions);

            window.Load += Window_Load;
            window.Update += Window_Update;
            window.Render += Window_Render;
            window.Closing += Window_Closing;

            window.Run();
        }

        private static void Window_Load()
        {
            //Console.WriteLine("Load");

            // set up input handling
            inputContext = window.CreateInput();
            foreach (var keyboard in inputContext.Keyboards)
            {
                keyboard.KeyDown += Keyboard_KeyDown;
            }

            Gl = window.CreateOpenGL();

            controller = new ImGuiController(Gl, window, inputContext);

            // Handle resizes
            window.FramebufferResize += s =>
            {
                // Adjust the viewport to the new window size
                Gl.Viewport(s);
            };


            Gl.ClearColor(System.Drawing.Color.Black);

            SetUpObjects();

            LinkProgram();

            //Gl.Enable(EnableCap.CullFace);

            Gl.Enable(EnableCap.DepthTest);
            Gl.DepthFunc(DepthFunction.Lequal);
        }

        private static void LinkProgram()
        {
            uint vshader = Gl.CreateShader(ShaderType.VertexShader);
            uint fshader = Gl.CreateShader(ShaderType.FragmentShader);

            Gl.ShaderSource(vshader, ReadShader("VertexShader.vert"));
            Gl.CompileShader(vshader);
            Gl.GetShader(vshader, ShaderParameterName.CompileStatus, out int vStatus);
            if (vStatus != (int)GLEnum.True)
                throw new Exception("Vertex shader failed to compile: " + Gl.GetShaderInfoLog(vshader));

            Gl.ShaderSource(fshader, ReadShader("FragmentShader.frag"));
            Gl.CompileShader(fshader);

            program = Gl.CreateProgram();
            Gl.AttachShader(program, vshader);
            Gl.AttachShader(program, fshader);
            Gl.LinkProgram(program);
            Gl.GetProgram(program, GLEnum.LinkStatus, out var status);
            if (status == 0)
            {
                Console.WriteLine($"Error linking shader {Gl.GetProgramInfoLog(program)}");
            }
            Gl.DetachShader(program, vshader);
            Gl.DetachShader(program, fshader);
            Gl.DeleteShader(vshader);
            Gl.DeleteShader(fshader);
        }

        private static string ReadShader(string shaderFileName)
        {
            using (Stream shaderStream = typeof(Program).Assembly.GetManifestResourceStream("projekt.Shaders." + shaderFileName))
            using (StreamReader shaderReader = new StreamReader(shaderStream))
                return shaderReader.ReadToEnd();
        }


        private static void Keyboard_KeyDown(IKeyboard keyboard, Key key, int arg3)
        {
            switch (key)
            {
                case Key.Left:
                    //cameraDescriptor.DecreaseZYAngle();
                    snailRotationY += turnStep;
                    snailPosition.X += (float)Math.Sin(snailRotationY) * moveStep;
                    snailPosition.Z += (float)Math.Cos(snailRotationY) * moveStep;
                    UpdateCamera();
                    break;
                    ;
                case Key.Right:
                    // cameraDescriptor.IncreaseZYAngle();
                    snailRotationY -= turnStep;
                    snailPosition.X += (float)Math.Sin(snailRotationY) * moveStep;
                    snailPosition.Z += (float)Math.Cos(snailRotationY) * moveStep;
                    UpdateCamera();
                    break;
                case Key.Up:
                    //cameraDescriptor.IncreaseDistance();
                    snailPosition.X += (float)Math.Sin(snailRotationY) * moveStep;
                    snailPosition.Z += (float)Math.Cos(snailRotationY) * moveStep;
                    UpdateCamera();
                    break;
            }
        }

        private static void Window_Update(double deltaTime)
        {
            //Console.WriteLine($"Update after {deltaTime} [s].");
            // multithreaded
            // make sure it is threadsafe
            // NO GL calls
            snailArrangementModel.AdvanceTime(deltaTime);
            butterflyArrangementModel.AdvanceTime(deltaTime);
            butTime += (float)deltaTime;

            controller.Update((float)deltaTime);
        }

        private static unsafe void Window_Render(double deltaTime)
        {
            //Console.WriteLine($"Render after {deltaTime} [s].");

            // GL here
            Gl.Clear(ClearBufferMask.ColorBufferBit);
            Gl.Clear(ClearBufferMask.DepthBufferBit);


            Gl.UseProgram(program);

            SetViewMatrix();
            SetProjectionMatrix();

            SetLightColor();
            SetLightPosition();
            SetViewerPosition();
            SetShininess();

            //DrawPulsingTeapot();

            DrawSkyBox();
            DrawField();
            //DrawCube();
            DrawPulsingSnail();
            DrawPulsingButterfly();
            DrawFlowers();

            ImGuiNET.ImGui.Begin("View properties",
                ImGuiWindowFlags.AlwaysAutoResize | ImGuiWindowFlags.NoTitleBar);
            ImGuiNET.ImGui.Text("Views");
            string[] viewNames = { "first person", "top down follow" };
            ImGuiNET.ImGui.Combo("Views", ref viewIndex, viewNames, viewNames.Length);
            ImGuiNET.ImGui.End();

            if (viewIndex == 0)
            {
                cameraDescriptor.Mode = CameraMode.FirstPerson;
            }
            else
            {
                cameraDescriptor.Mode = CameraMode.TopDownFollow;
            }
            UpdateCamera();

            controller.Render();
        }

        private static unsafe void DrawSkyBox()
        {
            Matrix4X4<float> modelMatrix = Matrix4X4.CreateScale(400f);
            SetModelMatrix(modelMatrix);
            Gl.BindVertexArray(skyBox.Vao);

            int textureLocation = Gl.GetUniformLocation(program, TextureUniformVariableName);
            if (textureLocation == -1)
            {
                throw new Exception($"{TextureUniformVariableName} uniform not found on shader.");
            }
            // set texture 0
            Gl.Uniform1(textureLocation, 0);

            Gl.ActiveTexture(TextureUnit.Texture0);
            Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (float)GLEnum.Linear);
            Gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (float)GLEnum.Linear);
            Gl.BindTexture(TextureTarget.Texture2D, skyBox.Texture.Value);

            Gl.DrawElements(GLEnum.Triangles, skyBox.IndexArrayLength, GLEnum.UnsignedInt, null);
            Gl.BindVertexArray(0);

            CheckError();
            Gl.BindTexture(TextureTarget.Texture2D, 0);
            CheckError();
        }


        private static unsafe void DrawField()
        {
            var modelMatrixForField = Matrix4X4.CreateScale(1f, 1f, 1f) * Matrix4X4.CreateTranslation(0f, -0.46f, 0f);
            SetModelMatrix(modelMatrixForField);
            Gl.BindVertexArray(field.Vao);
            Gl.DrawElements(GLEnum.Triangles, field.IndexArrayLength, GLEnum.UnsignedInt, null);
            Gl.BindVertexArray(0);
        }

        private static unsafe void DrawPulsingSnail()
        {
            // set material uniform to rubber

            var modelMatrixForSnail =
                Matrix4X4.CreateTranslation(0f, -3.1f, 0f) *
                Matrix4X4.CreateRotationY(snailRotationY) *
                Matrix4X4.CreateScale(0.15f) *
                Matrix4X4.CreateTranslation(snailPosition);

            uint textureId = TextureLoader.LoadTextureFromResource(Gl, "projekt.Resources.body_texture.jpg");
            Gl.BindTexture(TextureTarget.Texture2D, textureId);
            SetModelMatrix(modelMatrixForSnail);
            Gl.BindVertexArray(snail_body.Vao);
            Gl.DrawElements(GLEnum.Triangles, snail_body.IndexArrayLength, GLEnum.UnsignedInt, null);
            Gl.BindVertexArray(0);

            uint textureId2 = TextureLoader.LoadTextureFromResource(Gl, "projekt.Resources.shell_texture.jpg");
            Gl.BindTexture(TextureTarget.Texture2D, textureId2);
            SetModelMatrix(modelMatrixForSnail);
            Gl.BindVertexArray(snail_shell.Vao);
            Gl.DrawElements(GLEnum.Triangles, snail_shell.IndexArrayLength, GLEnum.UnsignedInt, null);
            Gl.BindVertexArray(0);

        }

        private static unsafe void GenerateFlowerTypes()
        {

            for (int i = 0; i < 50; i ++)
            {
                flowers[i] = new int[3];
                flowers[i][0] = rnd.Next(2);
                flowers[i][1] = rnd.Next(-50, 51);
                flowers[i][2] = rnd.Next(-50, 51);
            }

        }

        private static unsafe void DrawFlowers()
        {
            var originalModelMatrixForFlower =
               Matrix4X4.CreateTranslation(0f, -9.0f, 0f) *
               Matrix4X4.CreateScale(0.05f);

            uint textureId3 = TextureLoader.LoadTextureFromResource(Gl, "projekt.Resources.flower_texture.jpg");

            var originalModelMatrixForFlower2 =
               Matrix4X4.CreateScale(0.05f);

            uint textureId4 = TextureLoader.LoadTextureFromResource(Gl, "projekt.Resources.yellow_texture.png");
            
            for (int i = 0; i < 50; i ++)
            {
                if (flowers[i][0] == -1) continue;

                float dx = snailPosition.X - flowers[i][1];
                float dz = snailPosition.Z - flowers[i][2];
                float distance_2 = dx * dx + dz * dz;
                float distance = snailRadius + flowerRadius;

                if(distance_2 < distance * distance)
                {
                    flowers[i][0] = -1;
                }

                if (flowers[i][0] == -1) continue;

                if (flowers[i][0] == 0)
                {
                    var modelMatrixForFlower = originalModelMatrixForFlower * Matrix4X4.CreateTranslation(flowers[i][1], 0f, flowers[i][2]);
                    Gl.BindTexture(TextureTarget.Texture2D, textureId3);
                    SetModelMatrix(modelMatrixForFlower);
                    Gl.BindVertexArray(flower1.Vao);
                    Gl.DrawElements(GLEnum.Triangles, flower1.IndexArrayLength, GLEnum.UnsignedInt, null);
                    Gl.BindVertexArray(0);
                }
                else
                {
                    var modelMatrixForFlower2 = originalModelMatrixForFlower2 * Matrix4X4.CreateTranslation(flowers[i][1], 0f, flowers[i][2]);
                    Gl.BindTexture(TextureTarget.Texture2D, textureId4);
                    SetModelMatrix(modelMatrixForFlower2);
                    Gl.BindVertexArray(flower2.Vao);
                    Gl.DrawElements(GLEnum.Triangles, flower2.IndexArrayLength, GLEnum.UnsignedInt, null);
                    Gl.BindVertexArray(0);
                }
            }
        }


        private static unsafe void DrawPulsingButterfly()
        {
            float yOffset = 1.0f + (float)(0.15f * Math.Sin(butTime * 1.8f));

            Matrix4X4<float> scale = Matrix4X4.CreateScale(0.6f);
            Matrix4X4<float> trans = Matrix4X4.CreateTranslation(3f, 1.5f, 0f);
            Matrix4X4<float> rotGlobY = Matrix4X4.CreateRotationY((float)butterflyArrangementModel.AngleRevolutionOnGlobalY);
            Matrix4X4<float> pulsing = Matrix4X4.CreateTranslation(0, yOffset, 0);
            Matrix4X4<float> bodyMatrix = scale * trans * pulsing * rotGlobY;

            Gl.BindTexture(TextureTarget.Texture2D, 0);
            SetModelMatrix(bodyMatrix);
            Gl.BindVertexArray(butterfly_body.Vao);
            Gl.DrawElements(GLEnum.Triangles, butterfly_body.IndexArrayLength, GLEnum.UnsignedInt, null);
            Gl.BindVertexArray(0);

            float wingAngle = (float)(10.0f * Math.Sin(butTime * 5.0f));
            Matrix4X4<float> leftWingMotion = Matrix4X4.CreateRotationZ(wingAngle * (float)(Math.PI / 180f));

            var leftWingMatrix = scale * leftWingMotion * trans * pulsing * rotGlobY;

            SetModelMatrix(leftWingMatrix);
            Gl.BindVertexArray(butterfly_wing1.Vao);
            Gl.DrawElements(GLEnum.Triangles, butterfly_wing1.IndexArrayLength, GLEnum.UnsignedInt, null);
            Gl.BindVertexArray(0);

            Matrix4X4<float> rightWingMotion = Matrix4X4.CreateRotationZ(-wingAngle * (float)(Math.PI / 180f));

            var rightWingMatrix = scale * rightWingMotion * trans * pulsing * rotGlobY;

            SetModelMatrix(rightWingMatrix);
            Gl.BindVertexArray(butterfly_wing2.Vao);
            Gl.DrawElements(GLEnum.Triangles, butterfly_wing2.IndexArrayLength, GLEnum.UnsignedInt, null);
            Gl.BindVertexArray(0);
        }

        private static unsafe void SetLightColor()
        {
            int location = Gl.GetUniformLocation(program, LightColorVariableName);

            if (location == -1)
            {
                throw new Exception($"{LightColorVariableName} uniform not found on shader.");
            }

            Gl.Uniform3(location, 1f, 1f, 1f);
            CheckError();
        }

        private static unsafe void SetLightPosition()
        {
            int location = Gl.GetUniformLocation(program, LightPositionVariableName);

            if (location == -1)
            {
                throw new Exception($"{LightPositionVariableName} uniform not found on shader.");
            }

            Gl.Uniform3(location, 0f, 10f, 0f);
            CheckError();
        }

        private static unsafe void SetViewerPosition()
        {
            int location = Gl.GetUniformLocation(program, ViewPosVariableName);

            if (location == -1)
            {
                throw new Exception($"{ViewPosVariableName} uniform not found on shader.");
            }

            Gl.Uniform3(location, cameraDescriptor.Position.X, cameraDescriptor.Position.Y, cameraDescriptor.Position.Z);
            CheckError();
        }

        private static unsafe void SetShininess()
        {
            int location = Gl.GetUniformLocation(program, ShininessVariableName);

            if (location == -1)
            {
                throw new Exception($"{ShininessVariableName} uniform not found on shader.");
            }

            Gl.Uniform1(location, Shininess);
            CheckError();
        }

        private static unsafe void SetModelMatrix(Matrix4X4<float> modelMatrix)
        {
            int location = Gl.GetUniformLocation(program, ModelMatrixVariableName);
            if (location == -1)
            {
                throw new Exception($"{ModelMatrixVariableName} uniform not found on shader.");
            }

            Gl.UniformMatrix4(location, 1, false, (float*)&modelMatrix);
            CheckError();

            var modelMatrixWithoutTranslation = new Matrix4X4<float>(modelMatrix.Row1, modelMatrix.Row2, modelMatrix.Row3, modelMatrix.Row4);
            modelMatrixWithoutTranslation.M41 = 0;
            modelMatrixWithoutTranslation.M42 = 0;
            modelMatrixWithoutTranslation.M43 = 0;
            modelMatrixWithoutTranslation.M44 = 1;

            Matrix4X4<float> modelInvers;
            Matrix4X4.Invert<float>(modelMatrixWithoutTranslation, out modelInvers);
            Matrix3X3<float> normalMatrix = new Matrix3X3<float>(Matrix4X4.Transpose(modelInvers));
            location = Gl.GetUniformLocation(program, NormalMatrixVariableName);
            if (location == -1)
            {
                throw new Exception($"{NormalMatrixVariableName} uniform not found on shader.");
            }
            Gl.UniformMatrix3(location, 1, false, (float*)&normalMatrix);
            CheckError();
        }

        private static unsafe void SetUpObjects()
        {
            float[] nicestColorEver = [227f / 255f, 115f / 255f, 131f / 255f, 1.0f];
            float[] black = [0f, 0f, 0f, 1.0f];

            //teapot = ObjResourceReader.CreateObjWithColor(Gl, face1Color, "skybox.Resources.teapot.obj");

            float[] fieldColor = [88/256f,
                                  199/256f,
                                  54/256f,
                                  1f];
            field = GlCube.CreateSquare(Gl, fieldColor);
            //cube = GlCube.CreateCubeWithFaceColors(Gl, black, black, black, black, black, black);
            snail_body = ObjResourceReader.CreateObjWithColor(Gl, nicestColorEver, "projekt.Resources.snail_body.obj");
            snail_shell = ObjResourceReader.CreateObjWithColor(Gl, nicestColorEver, "projekt.Resources.snail_shell.obj");
            butterfly_body = ObjResourceReader.CreateObjWithColor(Gl, black, "projekt.Resources.body.obj");
            butterfly_wing1 = ObjResourceReader.CreateObjWithColor(Gl, nicestColorEver, "projekt.Resources.wing1.obj");
            butterfly_wing2 = ObjResourceReader.CreateObjWithColor(Gl, nicestColorEver, "projekt.Resources.wing2.obj");
            flower1 = ObjResourceReader.CreateObjWithColor(Gl, nicestColorEver, "projekt.Resources.pink_flower.obj");
            flower2 = ObjResourceReader.CreateObjWithColor(Gl, nicestColorEver, "projekt.Resources.flower1.obj");
            skyBox = GlCube.CreateInteriorCube(Gl, "");
            GenerateFlowerTypes();
            //UpdateCamera();

        }
        private static void UpdateCamera()
        {
            cameraDescriptor.SnailPosition = snailPosition;
            Console.WriteLine($"X: {cameraDescriptor.SnailPosition.X}, Y: {cameraDescriptor.SnailPosition.Y}, Z: {cameraDescriptor.SnailPosition.Z}");
            cameraDescriptor.SnailForward = new Vector3D<float>(
                    (float)Math.Sin(snailRotationY),
                    0,
                    (float)Math.Cos(snailRotationY)
                );
            Console.WriteLine($"Xx: {cameraDescriptor.Target.X}, Yy: {cameraDescriptor.Target.Y}, Zz: {cameraDescriptor.Target.Z}");
            Console.WriteLine($"Xxx: {cameraDescriptor.Position.X}, Yyy: {cameraDescriptor.Position.Y}, Zzz: {cameraDescriptor.Position.Z}");
        }

        private static void Window_Closing()
        {
            field.ReleaseGlObject();
            snail_body.ReleaseGlObject();
            snail_shell.ReleaseGlObject();
            butterfly_body.ReleaseGlObject();
            butterfly_wing1.ReleaseGlObject();
            butterfly_wing2.ReleaseGlObject();
            flower1.ReleaseGlObject();
            flower2.ReleaseGlObject();
        }

        private static unsafe void SetProjectionMatrix()
        {
            var projectionMatrix = Matrix4X4.CreatePerspectiveFieldOfView<float>((float)Math.PI / 4f, 1024f / 768f, 0.1f, 1000);
            int location = Gl.GetUniformLocation(program, ProjectionMatrixVariableName);

            if (location == -1)
            {
                throw new Exception($"{ViewMatrixVariableName} uniform not found on shader.");
            }

            Gl.UniformMatrix4(location, 1, false, (float*)&projectionMatrix);
            CheckError();
        }

        private static unsafe void SetViewMatrix()
        {
            var viewMatrix = Matrix4X4.CreateLookAt(cameraDescriptor.Position, cameraDescriptor.Target, cameraDescriptor.UpVector);
            int location = Gl.GetUniformLocation(program, ViewMatrixVariableName);

            if (location == -1)
            {
                throw new Exception($"{ViewMatrixVariableName} uniform not found on shader.");
            }

            Gl.UniformMatrix4(location, 1, false, (float*)&viewMatrix);
            CheckError();
        }

        public static void CheckError()
        {
            var error = (Silk.NET.OpenGL.ErrorCode)Gl.GetError();
            if (error != Silk.NET.OpenGL.ErrorCode.NoError)
                throw new Exception("GL.GetError() returned " + error.ToString());
        }
    }
}