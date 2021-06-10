﻿// Aaron Grincewicz Veganimus@icloud.com 6/5/2021
using UnityEngine;
namespace Veganimus.Platformer
{
    public class Character : MonoBehaviour
    {
        private CharacterController _controller;
        private float _horizontal;
        private float _vertical;
        private float _runSpeed = 10.0f;
        private float _defaultSpeed;
        private float _yVelocity;
        private bool _canDoubleJump;
        [SerializeField] private bool _canWallJump;
        [SerializeField] private bool _isWallJumping;
        private Vector3 _direction;
        private Vector3 _velocity;
        private Vector3 _wallSurfaceNormal;
        [SerializeField] private float _speed = 5f;
        [SerializeField] private float _gravity = 1.0f;
        [SerializeField] private float _jumpHeight = 15.0f;
        [SerializeField] private float _collectibleDetectionRadius;
        [SerializeField] private GameObject _characterModel;
        [SerializeField] private bool _hanging;
        [SerializeField] private LayerMask _detectSurfaceLayers;
        [SerializeField] private LayerMask _collectibleLayerMask;

        private void Start()
        {
            _controller = GetComponent<CharacterController>();
            _defaultSpeed = _speed;
        }
        private void Update()
        {
            Movement();
            Run();
            FaceDirection();
            DetectSurface();
            DetectCollectible();
        }
        private void Movement()
        {
            _horizontal = Input.GetAxis("Horizontal");
            _vertical = Input.GetAxis("Vertical");
            _direction = new Vector3(_horizontal, 0, 0);
            _velocity = _direction * _speed;

            if (_controller.isGrounded)
            {
                if (Input.GetButtonDown("Jump"))
                {
                    _yVelocity = _jumpHeight;
                    _canDoubleJump = true;
                }
            }
            else
            {
                if (Input.GetButtonDown("Jump"))
                {
                    if (_canDoubleJump || _canWallJump)
                    {
                        if (_canWallJump && !_isWallJumping)
                        {
                            _isWallJumping = true;
                            _velocity = _wallSurfaceNormal * (_speed * 3);
                            _canDoubleJump = false;
                            _canWallJump = false;
                        }
                        _yVelocity = _jumpHeight;
                        _canDoubleJump = false;
                    }
                }
                if (_hanging)
                {
                    _gravity = 0;
                    _canDoubleJump = false;
                    _canWallJump = false;
                }
                if (_vertical < 0 && _hanging)
                {
                    _hanging = false;
                    _gravity = 1;
                }
                _yVelocity -= _gravity;
            }

            _velocity.y = _yVelocity;
            _controller.Move(_velocity * Time.deltaTime);
        }
        private void Run()
        {
            if (Input.GetKey(KeyCode.LeftShift) && _controller.isGrounded)
                _speed = _runSpeed;
            else
                _speed = _defaultSpeed;
        }
        private void FaceDirection()
        {
            if (_horizontal < 0)
                _characterModel.transform.localRotation = new Quaternion(0, -180, 0, 0);
            else if (_horizontal > 0)
                _characterModel.transform.localRotation = new Quaternion(0, 0, 0, 0);
        }

        private void OnControllerColliderHit(ControllerColliderHit hit)
        {
            if (!_controller.isGrounded && !_canWallJump)
            {
                var wall = hit.collider.GetComponent<IWall>();
                if (wall != null)
                {
                    _wallSurfaceNormal = hit.normal;
                    _canWallJump = true;
                    _canDoubleJump = false;
                }
            }
            else
            {
                _canWallJump = false;
                _isWallJumping = false;
            }
        }
        private void DetectSurface()
        {
            RaycastHit hitInfo;
            if (Physics.Raycast(_characterModel.transform.position, Vector3.up, out hitInfo, 2.5f, _detectSurfaceLayers))
            {
                var hangable = hitInfo.collider.GetComponent<IHang>();
                if (hangable != null)
                    _hanging = true;
            }
            else
            {
                _hanging = false;
                _gravity = 1.0f;
            }
        }
        
        private void DetectCollectible()
        {
            int maxColliders = 5;
            Collider[] results = new Collider[maxColliders];
            int numberColliders = Physics.OverlapSphereNonAlloc(transform.position,
                                                                _collectibleDetectionRadius,
                                                                results,
                                                                _collectibleLayerMask);

            for (int i = 0; i < numberColliders; i++)
            {
                results[i].transform.position = Vector3.Lerp(results[i].transform.position, transform.position, 3f * Time.deltaTime);

                if (Vector3.Distance(transform.position, results[i].transform.position) < 0.5f)
                 Destroy(results[i].gameObject);
            }
        }
    }
}