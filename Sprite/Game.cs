﻿using System;
using System.Drawing;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;

namespace LearnOpenGL_TK
{
    //We can now move around objects. However, how can we move our "camera", or modify our perspective?
    //In this tutorial, I'll show you how to setup a full projection/view/model (PVM) matrix.
    //In addition, we'll make the rectangle rotate over time.
    class Game : GameWindow
    {
        float[] vertices =
        {
            //Position          Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left 
        };

        uint[] indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        int ElementBufferObject;
        int VertexBufferObject;
        int VertexArrayObject;

        Shader shader;
        Texture texture;
        Texture texture2;

        //We create a double to hold how long has passed since the program was opened.
        double time = 0.0;

        // Camera object that holds view and projection matrices.
        Camera camera;

        // Sprite to draw
        Sprite sprite;

        public Game(int width, int height, string title) : base(width, height, GraphicsMode.Default, title) { }

        
        protected override void OnLoad(EventArgs e)
        {
            GL.ClearColor(0.2f, 0.3f, 0.3f, 1.0f);

            //We enable depth testing here. If you try to draw something more complex than one plane without this,
            //you'll notice that polygons further in the background will occasionally be drawn over the top of the ones in the foreground.
            //Obviously, we don't want this, so we enable depth testing. We also clear the depth buffer in GL.Clear over in OnRenderFrame.
            GL.Enable(EnableCap.DepthTest);

            sprite = new Sprite("awesomeface.png");

            //VertexBufferObject = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ArrayBuffer, VertexBufferObject);
            //GL.BufferData(BufferTarget.ArrayBuffer, vertices.Length * sizeof(float), vertices, BufferUsageHint.StaticDraw);

            //ElementBufferObject = GL.GenBuffer();
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);
            //GL.BufferData(BufferTarget.ElementArrayBuffer, indices.Length * sizeof(uint), indices, BufferUsageHint.StaticDraw);

            ////shader.vert has been modified. Take a look at it after the explanation in OnRenderFrame.
            //shader = new Shader("shader.vert", "shader.frag");
            //shader.Use();

            //texture = new Texture("container.png");
            //texture.Use(TextureUnit.Texture0);

            //texture2 = new Texture("awesomeface.png");
            //texture2.Use(TextureUnit.Texture1);

            //shader.SetInt("texture0", 0);
            //shader.SetInt("texture1", 1);

            //VertexArrayObject = GL.GenVertexArray();
            //GL.BindVertexArray(VertexArrayObject);

            //GL.BindBuffer(BufferTarget.ArrayBuffer, VertexArrayObject);
            //GL.BindBuffer(BufferTarget.ElementArrayBuffer, ElementBufferObject);


            //int vertexLocation = shader.GetAttribLocation("aPosition");
            //GL.EnableVertexAttribArray(vertexLocation);
            //GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);


            //int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            //GL.EnableVertexAttribArray(texCoordLocation);
            //GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));


            camera = new Camera(this, new Vector3(0, 0, 3f), new Vector3(0, 1, 0));

            //Now, head over to OnRenderFrame to see how we setup the model matrix

            base.OnLoad(e);
        }


        protected override void OnRenderFrame(FrameEventArgs e)
        {
            //We add the time elapsed since last frame, times 4.0 to speed up animation, to the total amount of time passed.
            time += 4.0 * e.Time;

            //We clear the depth buffer in addition to the color buffer
            GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);

            //GL.BindVertexArray(VertexArrayObject);

            //texture.Use(TextureUnit.Texture0);
            //texture2.Use(TextureUnit.Texture1);
            //shader.Use();

            ////Finally, we have the model matrix. This determines the position of the model.
            //Matrix4 model = Matrix4.Identity * Matrix4.CreateRotationX((float)MathHelper.DegreesToRadians(time));

            ////Then, we pass all of these matrices to the vertex shader.
            ////You could also multiply them here and then pass, which is faster, but having the separate matrices available is used for some advanced effects

            ////IMPORTANT: OpenTK's matrix types are transposed from what OpenGL would expect - rows and columns are reversed.
            ////They are then transposed properly when passed to the shader.
            ////If you pass the individual matrices to the shader and multiply there, you have to do in the order "model, view, projection",
            ////but if you do it here and then pass it to the vertex, you have to do it in order "projection, view, model".
            //shader.SetMatrix4("model", model);
            //shader.SetMatrix4("view", camera.GetViewMatrix());
            //shader.SetMatrix4("projection", camera.GetProjectionMatrix());

            //GL.DrawElements(PrimitiveType.Triangles, indices.Length, DrawElementsType.UnsignedInt, 0);

            sprite.Draw(Vector3.Zero, 0, camera.GetViewMatrix(), camera.GetProjectionMatrix());

            Context.SwapBuffers();

            base.OnRenderFrame(e);
        }


        protected override void OnUpdateFrame(FrameEventArgs e)
        {
            KeyboardState input = Keyboard.GetState();

            if (input.IsKeyDown(Key.Escape))
            {
                Exit();
            }

            if(input.IsAnyKeyDown)
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

        protected override void OnMouseMove(MouseMoveEventArgs e)
        {
            Console.WriteLine("{0},{1}", e.XDelta, e.YDelta);
            camera.ProcessMouseMovement(e.XDelta, e.YDelta);

            base.OnMouseMove(e);
        }

        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            camera.ProcessMouseScroll(e.DeltaPrecise);

            base.OnMouseWheel(e);
        }

        protected override void OnResize(EventArgs e)
        {
            GL.Viewport(0, 0, Width, Height);
            base.OnResize(e);
        }

        protected override void OnUnload(EventArgs e)
        {
            GL.BindBuffer(BufferTarget.ArrayBuffer, 0);
            GL.BindVertexArray(0);
            GL.UseProgram(0);

            //GL.DeleteBuffer(VertexBufferObject);
            //GL.DeleteVertexArray(VertexArrayObject);

            //shader.Dispose();
            //texture.Dispose();
            //texture2.Dispose();


            sprite.Dispose();
            
            base.OnUnload(e);
        }
    }
}
