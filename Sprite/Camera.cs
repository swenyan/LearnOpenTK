using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL4;
using OpenTK.Input;
using GlmNet;

namespace LearnOpenGL_TK
{
    // Defines several possible options for camera movement. Used as abstraction to stay away from window-system specific input methods
    enum Camera_Movement
    {
        FORWARD,
        BACKWARD,
        LEFT,
        RIGHT
    };

    // An abstract camera class that processes input and calculates the corresponding Eular Angles, Vectors and Matrices for use in OpenGL
    class Camera
    {
        // Default camera values
        const float YAW = -90.0f;
        const float PITCH = 0.0f;
        const float SPEED = 2.5f;
        const float SENSITIVTY = 0.2f;
        const float ZOOM = 45.0f;

        private GameWindow window;

        // Camera Attributes
        public Vector3 Position;
        public Vector3 Front;
        public Vector3 Up;
        public Vector3 Right;
        public Vector3 WorldUp;
        // Eular Angles
        public float Yaw;
        public float Pitch;
        // Camera options
        public float MovementSpeed;
        public float MouseSensitivity;
        public float Zoom;

        private bool isFirstFrame = true;

        // View Matrix
        private Matrix4 view;

        // Projection Matrix
        private Matrix4 projection;

        // Constructor with vectors
        public Camera(GameWindow window, Vector3 position, Vector3 up, float yaw = YAW, float pitch = PITCH)
        {
            this.window = window;

            Position = position;
            WorldUp = up;
            Yaw = yaw;
            Pitch = pitch;

            Front = new Vector3(0, 0, -1f);
            MovementSpeed = SPEED;
            MouseSensitivity = SENSITIVTY;
            Zoom = ZOOM;
            UpdateCameraVectors();

            view = new Matrix4();
            projection = new Matrix4();

            UpdateViewMatrix();
            UpdateProjectionMatrix();
        }

        // Constructor with scalar values
        public Camera(GameWindow window, float posX, float posY, float posZ, float upX, float upY, float upZ, float yaw, float pitch) : 
            this(window, new Vector3(posX, posY, posZ), new Vector3(upX, upY, upZ), yaw, pitch)
        {

        }

        // Returns the view matrix calculated using Eular Angles and the LookAt Matrix
        public Matrix4 GetViewMatrix()
        {
            return view;
        }

        public Matrix4 GetProjectionMatrix()
        {
            return projection;
        }

        private void UpdateViewMatrix()
        {
            view = Matrix4.LookAt(Position, Position + Front, Up);
        }

        private void UpdateProjectionMatrix()
        {
            projection = Matrix4.CreatePerspectiveFieldOfView((float)MathHelper.DegreesToRadians(Zoom), window.Width / window.Height, 0.1f, 100.0f);
        }

        // Processes input received from any keyboard-like input system. Accepts input parameter in the form of camera defined ENUM (to abstract it from windowing systems)
        public void ProcessKeyboard(Camera_Movement direction, float deltaTime)
        {
            float velocity = MovementSpeed * deltaTime;
            if (direction == Camera_Movement.FORWARD)
                Position += Front * velocity;
            if (direction == Camera_Movement.BACKWARD)
                Position -= Front * velocity;
            if (direction == Camera_Movement.LEFT)
                Position -= Right * velocity;
            if (direction == Camera_Movement.RIGHT)
                Position += Right * velocity;

            UpdateViewMatrix();
        }

        // Processes input received from a mouse input system. Expects the offset value in both the x and y direction.
        public void ProcessMouseMovement(float xoffset, float yoffset, bool constrainPitch = true)
        {
            if(isFirstFrame)
            {
                isFirstFrame = false;
                return;
            }

            xoffset *= MouseSensitivity;
            yoffset *= MouseSensitivity;

            Yaw += xoffset;
            Pitch -= yoffset;

            // Make sure that when pitch is out of bounds, screen doesn't get flipped
            if (constrainPitch)
            {
                if (Pitch > 89.0f)
                    Pitch = 89.0f;
                if (Pitch < -89.0f)
                    Pitch = -89.0f;
            }

            // Update Front, Right and Up Vectors using the updated Eular angles
            UpdateCameraVectors();

            UpdateViewMatrix();
        }

        // Processes input received from a mouse scroll-wheel event. Only requires input on the vertical wheel-axis
        public void ProcessMouseScroll(float yoffset)
        {
            if (Zoom >= 1.0f && Zoom <= 45.0f)
                Zoom -= yoffset;
            if (Zoom <= 1.0f)
                Zoom = 1.0f;
            if (Zoom >= 45.0f)
                Zoom = 45.0f;

            Console.WriteLine(Zoom);
            UpdateProjectionMatrix();
        }

        // Calculates the front vector from the Camera's (updated) Eular Angles
        private void UpdateCameraVectors()
        {
            //Console.WriteLine("Yaw:{0},Pitch{1},Front:{2}", Yaw, Pitch, Front);
            // Calculate the new Front vector
            Vector3 front;
            front.X = glm.cos(glm.radians(Yaw)) * glm.cos(glm.radians(Pitch));
            front.Y = glm.sin(glm.radians(Pitch));
            front.Z = glm.sin(glm.radians(Yaw)) * glm.cos(glm.radians(Pitch));
            Front = Vector3.Normalize(front);
            // Also re-calculate the Right and Up vector
            Right = Vector3.Normalize(Vector3.Cross(Front, WorldUp));  // Normalize the vectors, because their length gets closer to 0 the more you look up or down which results in slower movement.
            Up = Vector3.Normalize(Vector3.Cross(Right, Front));
        }
    };
}
