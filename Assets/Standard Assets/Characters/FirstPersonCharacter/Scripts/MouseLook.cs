using System;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.FirstPerson
{
    [Serializable]
    public class MouseLook
    {
        public PlayerInput input;

        public float XSensitivity = 2f;
        public float YSensitivity = 2f;
        public bool clampVerticalRotation = true;
        public float MinimumX = -90F;
        public float MaximumX = 90F;
        public bool smooth;
        public float smoothTime = 5f;
        public bool lockCursor = true;
        public static bool canMove;

        private Quaternion m_CharacterTargetRot;
        private Quaternion m_CameraTargetRot;
        private bool m_cursorIsLocked = true;
        private Vector2 moveVector;
        private float gravityX;
        private float inputDecel = 6f;
        private float inputAccel = 6f;
        private float gravityY;

        public void Init(Transform character, Transform camera)
        {
            m_CharacterTargetRot = character.localRotation;
            m_CameraTargetRot = camera.localRotation;

            canMove = true;
        }


        public void LookRotation(Transform character, Transform camera)
        {
            if(canMove)
            {
                if(input.currentControlScheme != "KB/M")
                {
                    moveVector = new Vector2(input.actions.FindAction("RotX").ReadValue<float>(), input.actions.FindAction("RotY").ReadValue<float>());

                    if (moveVector.x == 0)
                    {
                        gravityX = Mathf.MoveTowards(gravityX, 0f, Time.deltaTime * inputDecel);
                    }
                    else
                        gravityX = Mathf.MoveTowards(gravityX, moveVector.x, Time.deltaTime * inputAccel);

                    if (moveVector.y == 0)
                        gravityY = Mathf.MoveTowards(gravityY, 0f, Time.deltaTime * inputDecel);
                    else
                        gravityY = Mathf.MoveTowards(gravityY, moveVector.y, Time.deltaTime * inputAccel);

                    gravityX = Mathf.Clamp(gravityX, -1, 1);
                    gravityY = Mathf.Clamp(gravityY, -1, 1);
                }
                else
                {
                    gravityX = input.actions.FindAction("RotX").ReadValue<float>();
                    gravityY = input.actions.FindAction("RotY").ReadValue<float>();
                }

                float yRot = gravityX * XSensitivity;
                float xRot = gravityY * YSensitivity;

                m_CharacterTargetRot *= Quaternion.Euler(0f, yRot, 0f);
                m_CameraTargetRot *= Quaternion.Euler(-xRot, 0f, 0f);

                if (clampVerticalRotation)
                    m_CameraTargetRot = ClampRotationAroundXAxis(m_CameraTargetRot);

                if (smooth)
                {
                    character.localRotation = Quaternion.Slerp(character.localRotation, m_CharacterTargetRot,
                        smoothTime * Time.deltaTime);
                    camera.localRotation = Quaternion.Slerp(camera.localRotation, m_CameraTargetRot,
                        smoothTime * Time.deltaTime);
                }
                else
                {
                    character.localRotation = m_CharacterTargetRot;
                    camera.localRotation = m_CameraTargetRot;
                }

                UpdateCursorLock();
            }
        }

        public void SetCursorLock(bool value)
        {
            lockCursor = value;
            if(!lockCursor)
            {//we force unlock the cursor if the user disable the cursor locking helper
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        public void UpdateCursorLock()
        {
            //if the user set "lockCursor" we check & properly lock the cursos
            if (lockCursor)
                InternalLockUpdate();
        }

        private void InternalLockUpdate()
        {
            /*if(Input.GetKeyUp(KeyCode.Escape))
            {
                m_cursorIsLocked = false;
            }
            else if(Input.GetMouseButtonUp(0))
            {
                m_cursorIsLocked = true;
            }*/

            if (m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.Locked;
                Cursor.visible = false;
            }
            else if (!m_cursorIsLocked)
            {
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;
            }
        }

        Quaternion ClampRotationAroundXAxis(Quaternion q)
        {
            q.x /= q.w;
            q.y /= q.w;
            q.z /= q.w;
            q.w = 1.0f;

            float angleX = 2.0f * Mathf.Rad2Deg * Mathf.Atan (q.x);

            angleX = Mathf.Clamp (angleX, MinimumX, MaximumX);

            q.x = Mathf.Tan (0.5f * Mathf.Deg2Rad * angleX);

            return q;
        }

    }
}
