using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace LearnOpenGL_TK
{
    class Game : GameWindow
    {
        double time = 0.0;

        Camera camera;

        Sprite sprite;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) { }

        // 这里相当于 Start
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            GL.Enable(EnableCap.DepthTest);

            sprite = new Sprite("awesomeface.png", new float[] { 0.0f, 0.0f, 1.0f, 1.0f });

            camera = new Camera(this, new Vector3(0, 0, 3f), new Vector3(0, 1, 0));

            base.OnLoad(e);
        }

        // 这里相当于 Update
        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            if (input.IsAnyKeyDown)
            {
                if (input.IsKeyDown(Key.W))
                {
                    camera.ProcessKeyboard(Camera_Movement.FORWARD, (float)e.Time);
                }

                if (input.IsKeyDown(Key.A))
                {
                    camera.ProcessKeyboard(Camera_Movement.LEFT, (float)e.Time);
                }

                if (input.IsKeyDown(Key.S))
                {
                    camera.ProcessKeyboard(Camera_Movement.BACKWARD, (float)e.Time);
                }

                if (input.IsKeyDown(Key.D))
                {
                    camera.ProcessKeyboard(Camera_Movement.RIGHT, (float)e.Time);
                }
            }

            base.OnUpdateFrame(e);
        }

        // 鼠标移动的时候会被调用
        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            Console.WriteLine("{0},{1}", e.XDelta, e.YDelta);
            camera.ProcessMouseMovement(e.XDelta, e.YDelta);

            base.OnMouseMove(e);
        }

        // 鼠标滚轮转动的时候会被调用
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            camera.ProcessMouseScroll(e.DeltaPrecise);

            base.OnMouseWheel(e);
        }

        // 这里相当于 Render
        protected override void OnRenderFrame(FrameEventArgs e)
        {
            time += 4.0 * e.Time;

            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            sprite.Draw(Vector3.Zero, 0, camera.GetViewMatrix(), camera.GetProjectionMatrix());

            Context.SwapBuffers();

            base.OnRenderFrame(e);
        }
        
        // 窗口大小改变的时候会被调用
        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }


        // 窗口关闭的时候会被调用
        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            sprite.Dispose();
            base.OnUnload(e);
        }
    }
}
