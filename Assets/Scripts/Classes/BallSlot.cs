﻿// Aaron Grincewicz Veganimus@icloud.com 6/5/2021
using UnityEngine;
namespace Veganimus.Platformer
{
    public class BallSlot : MonoBehaviour
    {
        [SerializeField] private bool _ballInSlot;
        [SerializeField] private bool _isGoal;
        [SerializeField] private GameObject _centerPos;
        [SerializeField] private Goal _goal;
        private bool _conditionsChecked;
        
        private Rigidbody _insertedRB;

        private void OnTriggerEnter(Collider other)
        {
            var ballMode = other.GetComponent<BallModeController>();
            if (ballMode != null)
            {
                _insertedRB = other.GetComponent<Rigidbody>();
                if (_insertedRB != null)
                {
                    _insertedRB.useGravity = false;
                    if (!_insertedRB.useGravity)
                    {
                        _insertedRB.constraints = RigidbodyConstraints.FreezeAll;
                    }
                    other.transform.position = Vector3.Lerp(other.transform.position, _centerPos.transform.position, 5.0f);
                    _ballInSlot = true;
                }
            }
        }
        private void OnTriggerStay(Collider other)
        {
            if (_insertedRB != null)
            {
                if (!_insertedRB.useGravity)
                {
                    _insertedRB.constraints = RigidbodyConstraints.FreezeAll;
                }
                else
                {
                    _insertedRB.constraints = RigidbodyConstraints.FreezePositionZ;
                    _insertedRB.constraints = RigidbodyConstraints.FreezeRotationX;
                    _insertedRB.constraints = RigidbodyConstraints.FreezeRotationY;
                }
            }
            if (_isGoal && !_conditionsChecked)
            {
                _conditionsChecked = true;
                _goal.CheckWinConditions();
            }
        }
        private void OnTriggerExit(Collider other)
        {
            if (_insertedRB != null)
            {
                _insertedRB.useGravity = true;
                _insertedRB.constraints = RigidbodyConstraints.FreezePositionZ;
                _insertedRB.constraints = RigidbodyConstraints.FreezeRotationX;
                _insertedRB.constraints = RigidbodyConstraints.FreezeRotationY;
                _ballInSlot = false;
                _conditionsChecked = false;
            }
        }
    }
}