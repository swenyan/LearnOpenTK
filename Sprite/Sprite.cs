using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;

namespace LearnOpenGL_TK
{
    class Sprite : IDisposable
    {
        private const int U_MIN_IDX = 0;
        private const int V_MIN_IDX = 1;
        private const int U_MAX_IDX = 2;
        private const int V_MAX_IDX = 3;

        private const int U_11 = 3;
        private const int V_11 = 4;
        private const int U_10 = 8;
        private const int V_10 = 9;
        private const int U_00 = 13;
        private const int V_00 = 14;
        private const int U_01 = 18;
        private const int V_01 = 19;

        private float[] quad_vertices =
        {
            //Position          Texture coordinates
             0.5f,  0.5f, 0.0f, 1.0f, 1.0f, // top right
             0.5f, -0.5f, 0.0f, 1.0f, 0.0f, // bottom right
            -0.5f, -0.5f, 0.0f, 0.0f, 0.0f, // bottom left
            -0.5f,  0.5f, 0.0f, 0.0f, 1.0f  // top left 
        };

        private uint[] quad_indices =
        {
            0, 1, 3,
            1, 2, 3
        };

        private Mesh quad;

        private Shader shader = new Shader("shader.vert", "shader.frag");

        private Texture texture;

        private int VertexArrayObject;

        public Sprite(string texturePath, float[] uv)
        {
            if(uv != null)
            {
                quad_vertices[U_00] = uv[U_MIN_IDX];
                quad_vertices[V_00] = uv[V_MIN_IDX];
                quad_vertices[U_01] = uv[U_MIN_IDX];
                quad_vertices[V_01] = uv[V_MAX_IDX];
                quad_vertices[U_10] = uv[U_MAX_IDX];
                quad_vertices[V_10] = uv[V_MIN_IDX];
                quad_vertices[U_11] = uv[U_MAX_IDX];
                quad_vertices[V_11] = uv[V_MAX_IDX];
            }

            quad = new Mesh(quad_vertices, quad_indices);

            shader.Use();

            texture = new Texture(texturePath);
            texture.Use(TextureUnit.Texture0);

            shader.SetInt("texture0", 0);

            VertexArrayObject = GL.GenVertexArray();
            GL.BindVertexArray(VertexArrayObject);

            GL.BindBuffer(BufferTarget.ArrayBuffer, VertexArrayObject);
            GL.BindBuffer(BufferTarget.ElementArrayBuffer, quad.ElementBufferObject);

            int vertexLocation = shader.GetAttribLocation("aPosition");
            GL.EnableVertexAttribArray(vertexLocation);
            GL.VertexAttribPointer(vertexLocation, 3, VertexAttribPointerType.Float, false, 5 * sizeof(float), 0);

            int texCoordLocation = shader.GetAttribLocation("aTexCoord");
            GL.EnableVertexAttribArray(texCoordLocation);
            GL.VertexAttribPointer(texCoordLocation, 2, VertexAttribPointerType.Float, false, 5 * sizeof(float), 3 * sizeof(float));
        }

        public void Draw(Vector3 position, float rotationZ, Matrix4 view, Matrix4 projection)
        {
            GL.BindVertexArray(VertexArrayObject);

            texture.Use(TextureUnit.Texture0);
            shader.Use();

            Matrix4 model = Matrix4.CreateTranslation(position) * Matrix4.CreateRotationZ(MathHelper.DegreesToRadians(rotationZ));

            //Then, we pass all of these matrices to the vertex shader.
            //You could also multiply them here and then pass, which is faster, but having the separate matrices available is used for some advanced effects

            //IMPORTANT: OpenTK's matrix types are transposed from what OpenGL would expect - rows and columns are reversed.
            //They are then transposed properly when passed to the shader.
            //If you pass the individual matrices to the shader and multiply there, you have to do in the order "model, view, projection",
            //but if you do it here and then pass it to the vertex, you have to do it in order "projection, view, model".
            shader.SetMatrix4("model", model);
            shader.SetMatrix4("view", view);
            shader.SetMatrix4("projection", projection);

            GL.DrawElements(PrimitiveType.Triangles, quad_indices.Length, DrawElementsType.UnsignedInt, 0);
        }

        ~Sprite()
        {
            Console.WriteLine("Dispose sprite");
            GL.DeleteVertexArray(VertexArrayObject);
            texture.Dispose();
            shader.Dispose();
            quad.Dispose();
        }

        private bool disposedValue = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects).
                }

                GL.DeleteVertexArray(VertexArrayObject);
                texture.Dispose();
                shader.Dispose();
                quad.Dispose();
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
