﻿using UnityEngine;

namespace Develop
{
    public class FreeFlyController : MonoBehaviour
    {
        #region UI

        [Space]
        [SerializeField]
        private bool _active = true;

        [Space]
        [SerializeField]
        private CursorLockMode _cursorMode = CursorLockMode.Locked;

        [Space]
        [SerializeField]
        private bool _enableRotation = true;

        [SerializeField]
        private float _mouseSense = 1.8f;

        [Space]
        [SerializeField]
        private bool _enableTranslation = true;

        [SerializeField]
        private float _translationSpeed = 55f;

        [Space]
        [SerializeField]
        private bool _enableMovement = true;

        [SerializeField]
        private float _movementSpeed = 10f;

        [SerializeField]
        private float _boostedSpeed = 50f;

        [Space]
        [SerializeField]
        private bool _enableSpeedAcceleration = true;

        [SerializeField]
        private float _speedAccelerationFactor = 1.5f;

        [Space]
        [SerializeField]
        private KeyCode _initPositonButton = KeyCode.R;

        #endregion UI

        private CursorLockMode _currentCursorMode = CursorLockMode.None;

        private float _currentIncrease = 1;
        private float _currentIncreaseMem = 0;

        private Vector3 _initPosition;
        private Vector3 _initRotation;

        private Rigidbody _rigidbody;

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (_boostedSpeed < _movementSpeed)
                _boostedSpeed = _movementSpeed;
        }
#endif

        private void Awake()
        {
            _rigidbody = GetComponent<Rigidbody>();
            _rigidbody.freezeRotation = true;

            if (_cursorMode == CursorLockMode.Locked)
                _currentCursorMode = CursorLockMode.Locked;
        }

        private void Start()
        {
            _initPosition = transform.position;
            _initRotation = transform.eulerAngles;
        }

        // Apply requested cursor state
        private void SetCursorState()
        {
            if (_cursorMode == CursorLockMode.None)
                return;

            if (Input.GetKeyDown(KeyCode.Escape))
            {
                Cursor.lockState = _currentCursorMode = CursorLockMode.None;
            }

            if (Input.GetMouseButtonDown(0))
            {
                _currentCursorMode = CursorLockMode.Locked;
            }

            // Apply cursor state
            Cursor.lockState = _currentCursorMode;
            // Hide cursor when locking
            Cursor.visible = CursorLockMode.Locked != _currentCursorMode;
        }

        private void CalculateCurrentIncrease(bool moving)
        {
            _currentIncrease = Time.deltaTime;

            if (!_enableSpeedAcceleration || _enableSpeedAcceleration && !moving)
            {
                _currentIncreaseMem = 0;
                return;
            }

            _currentIncreaseMem += Time.deltaTime * (_speedAccelerationFactor - 1);
            _currentIncrease = Time.deltaTime + Mathf.Pow(_currentIncreaseMem, 3) * Time.deltaTime;
        }

        private void Update()
        {
            if (!_active)
                return;

            SetCursorState();

            if (_cursorMode == CursorLockMode.Locked && _currentCursorMode != CursorLockMode.Locked)
                return;

            // Translation
            if (_enableTranslation)
            {
                transform.Translate(Vector3.forward * (Input.mouseScrollDelta.y * Time.deltaTime * _translationSpeed));
            }

            // Movement
            if (_enableMovement)
            {
                Vector3 deltaPosition = Vector3.zero;
                float currentSpeed = _movementSpeed;

                if (Input.GetKey(KeyCode.LeftShift))
                    currentSpeed = _boostedSpeed;

                if (Input.GetKey(KeyCode.W))
                    deltaPosition += transform.forward;

                if (Input.GetKey(KeyCode.S))
                    deltaPosition -= transform.forward;

                if (Input.GetKey(KeyCode.A))
                    deltaPosition -= transform.right;

                if (Input.GetKey(KeyCode.D))
                    deltaPosition += transform.right;

                // Calc acceleration
                CalculateCurrentIncrease(deltaPosition != Vector3.zero);

                //transform.position += deltaPosition * (currentSpeed * _currentIncrease);

                Vector3 targetPosition = _rigidbody.position + deltaPosition * (currentSpeed * _currentIncrease);
                Vector3 targetVelocity = targetPosition - transform.position;
                _rigidbody.linearVelocity = targetVelocity;
            }

            // Rotation
            if (_enableRotation)
            {
                // Pitch
                transform.rotation *= Quaternion.AngleAxis(
                    -Input.GetAxis("Mouse Y") * _mouseSense,
                    Vector3.right
                );

                // Paw
                transform.rotation = Quaternion.Euler(
                    transform.eulerAngles.x,
                    transform.eulerAngles.y + Input.GetAxis("Mouse X") * _mouseSense,
                    transform.eulerAngles.z
                );
            }

            // Return to init position
            if (Input.GetKeyDown(_initPositonButton))
            {
                transform.position = _initPosition;
                transform.eulerAngles = _initRotation;
            }
        }
    }
}