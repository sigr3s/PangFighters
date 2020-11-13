﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(CharacterController))]
[RequireComponent(typeof(PlayerInput))]
public class PlayerController : MonoBehaviour
{
    private PlayerInput _playerInput;
    private PlayerInput m_PlayerInput {
        get{
            if(_playerInput == null){
                _playerInput = GetComponent<PlayerInput>();
            }

            return _playerInput;
        }
    }
    private InputAction m_MoveAction {
        get{
           return m_PlayerInput.actions["move"];
        }
    }

    private InputAction m_JumpAction {
        get{
           return m_PlayerInput.actions["jump"];
        }
    }

    private InputAction m_FireAction{
        get{
            return m_PlayerInput.actions["fire"];
        }
    }

    // A player can:
    //  Move wasd arrows etc
    //  Attack something
    //  Block stun
    //  A player always faces the other player bc you know fgc

    [SerializeField] private float jumpSpeed = 18.0F;
    [SerializeField] private float moveSpeed = 8.0F;
    [SerializeField] private float gravity = 40.0F;
    private Vector3 moveDirection = Vector3.zero;
    private CharacterController _charaterController = null;
    
    void Awake()
    {
        _charaterController = GetComponent<CharacterController>();
    }

    void Start()
    {
        
    }

    void Update()
    {
        var move = m_MoveAction.ReadValue<Vector2>();
        if (_charaterController.isGrounded && move.y > 0.4f) {
            moveDirection.y = jumpSpeed;
        }
        moveDirection.y -= gravity * Time.deltaTime;
        moveDirection.x = moveSpeed * move.x;
        _charaterController.Move(moveDirection * Time.deltaTime);
    }
}
