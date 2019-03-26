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
        const float SENSITIVTY = 0.1f;
        const float ZOOM = 45.0f;

        // Camera Attributes
        public vec3 Position;
        public vec3 Front;
        public vec3 Up;
        public vec3 Right;
        public vec3 WorldUp;
        // Eular Angles
        public float Yaw;
        public float Pitch;
        // Camera options
        public float MovementSpeed;
        public float MouseSensitivity;
        public float Zoom;

        // Constructor with vectors
        public Camera(vec3 position, vec3 up, float yaw = YAW, float pitch = PITCH)
        {
            Position = position;
            WorldUp = up;
            Yaw = yaw;
            Pitch = pitch;

            Front = new vec3(0, 0, -1f);
            MovementSpeed = SPEED;
            MouseSensitivity = SENSITIVTY;
            Zoom = ZOOM;
            updateCameraVectors();
        }

        // Constructor with scalar values
        public Camera(float posX, float posY, float posZ, float upX, float upY, float upZ, float yaw, float pitch)
        {
            Position = new vec3(posX, posY, posZ);
            WorldUp = new vec3(upX, upY, upZ);
            Yaw = yaw;
            Pitch = pitch;
            Front = new vec3(0, 0, -1f);
            MovementSpeed = SPEED;
            MouseSensitivity = SENSITIVTY;
            Zoom = ZOOM;

            updateCameraVectors();
        }

        // Returns the view matrix calculated using Eular Angles and the LookAt Matrix
        public mat4 GetViewMatrix()
        {
            return glm.lookAt(Position, Position + Front, Up);
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
        }

        // Processes input received from a mouse input system. Expects the offset value in both the x and y direction.
        public void ProcessMouseMovement(float xoffset, float yoffset, bool constrainPitch = true)
        {
            xoffset *= MouseSensitivity;
            yoffset *= MouseSensitivity;

            Yaw += xoffset;
            Pitch += yoffset;

            // Make sure that when pitch is out of bounds, screen doesn't get flipped
            if (constrainPitch)
            {
                if (Pitch > 89.0f)
                    Pitch = 89.0f;
                if (Pitch < -89.0f)
                    Pitch = -89.0f;
            }

            // Update Front, Right and Up Vectors using the updated Eular angles
            updateCameraVectors();
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
        }

        // Calculates the front vector from the Camera's (updated) Eular Angles
        private void updateCameraVectors()
        {
            // Calculate the new Front vector
            vec3 front;
            front.x = glm.cos(glm.radians(Yaw)) * glm.cos(glm.radians(Pitch));
            front.y = glm.sin(glm.radians(Pitch));
            front.z = glm.sin(glm.radians(Yaw)) * glm.cos(glm.radians(Pitch));
            Front = glm.normalize(front);
            // Also re-calculate the Right and Up vector
            Right = glm.normalize(glm.cross(Front, WorldUp));  // Normalize the vectors, because their length gets closer to 0 the more you look up or down which results in slower movement.
            Up = glm.normalize(glm.cross(Right, Front));
        }
    };
}
