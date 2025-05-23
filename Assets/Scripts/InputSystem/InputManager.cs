﻿using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [field: Header("타깃")]
    // controlTarget variable
    [SerializeField]
    private Controller controller;
    // variable to store keycodes with icommand
    private Dictionary<KeyCode, List<ICommand>> keyMapping = new Dictionary<KeyCode, List<ICommand>>();
    // variable to store reset keymapping
    private Dictionary<KeyCode, List<ICommand>> resetKeyMapping = new Dictionary<KeyCode, List<ICommand>>();

    // Axis variable for horizontal and vertical
    [SerializeField]
    private Axis horizontal = new Axis(KeyCode.A, KeyCode.D, 0.001f, 3f, 3f);
    [SerializeField]
    private Axis vertical = new Axis(KeyCode.S, KeyCode.W, 0.001f, 3f, 3f);

    [field: Header("움직임 값")]
    // Vector2 variable to store horizontal and vertical axis
    [SerializeField]
    private Vector2 moveAmount;

    // function to add keymapping
    public void AddKeyMapping(KeyCode key, ICommand command, ICommand resetCommand)
    {
        if (!keyMapping.ContainsKey(key))
        {
            keyMapping.Add(key, new List<ICommand>());
            resetKeyMapping.Add(key, new List<ICommand>());
        }

        keyMapping[key].Add(command);
        resetKeyMapping[key].Add(resetCommand);
    }

    public void RemoveKeyMapping(KeyCode key) { keyMapping.Remove(key); }

    // function to reset keymapping to default
    public void ResetKeyMapping()
    {
        keyMapping = new Dictionary<KeyCode, List<ICommand>>(resetKeyMapping);
    }

    // function to execute actions every keys in keymapping foreach loop
    public void ExecuteKeyMapping()
    {
        foreach (var key in keyMapping)
        {
            //if (Input.GetKeyDown(key.Key))
        }
    }

    private void Awake()
    {
        // when controlTarget is null, set controlTarget to player
        if (controller == null)
            controller = FindFirstObjectByType<Controller>();

        //var player = FindObjectOfType<PlayerMovement>();

        // add keymapping
        //AddKeyMapping(KeyCode.E, new InteractCommand(controller.controlTarget), new InteractCommand(controller.controlTarget));
        //AddKeyMapping(KeyCode.Space, new JumpCommand(controller.controlTarget), new JumpCommand(controller.controlTarget));

        //controlables = FindObjectsOfType<Controlable>();

        //foreach (var controlable in controlables)
        //{
        //    AddKeyMapping(KeyCode.E, new InteractCommand(controlable), new InteractCommand(controlable));
        //    AddKeyMapping(KeyCode.Space, new JumpCommand(controlable), new JumpCommand(controlable));
        //}
    }

    private void Update()
    {
        moveAmount = new Vector2(horizontal.GetAxis(), vertical.GetAxis());

        controller.controlTarget?.Move(moveAmount);

        ExecuteKeyMapping();
    }
}

// struct to store GetAxis variables
[Serializable]
public struct Axis
{
    public KeyCode negative;
    public KeyCode positive;
    public float Dead;
    // 아무런 버튼도 누르지 않으면 축이 중립쪽으로 떨어지는 속도(초 단위) 변수
    public float autoReturnSpeed;
    // 축이 대상의 값으로 향하기 위한 속도(초 단위)입니다. 디지털 장치 전용입니다.
    public float digitalReturnSpeed;
    [SerializeField]
    private float value;

    public Axis(KeyCode negative, KeyCode positive, float dead, float autoReturnSpeed, float digitalReturnSpeed)
    {
        this.negative = negative;
        this.positive = positive;
        Dead = dead;
        this.autoReturnSpeed = autoReturnSpeed;
        this.digitalReturnSpeed = digitalReturnSpeed;
        value = 0f;
    }

    // function to get axis value, if digitalReturnSpeed is higher, value changes faster
    // if abs value of value is lower than dead, return 0f
    // value is clamp between -1f and 1f, value changes speed of digitalReturnSpeed
    // return float value
    public float GetAxis()
    {
        if (Input.GetKeyDown(negative) || Input.GetKeyDown(positive))
        {
            value = 0f;
        }

        if (Input.GetKey(negative))
        {
            value -= digitalReturnSpeed * Time.deltaTime;
        }
        else if (Input.GetKey(positive))
        {
            value += digitalReturnSpeed * Time.deltaTime;
        }
        else
        {
            if (value > 0)
            {
                value -= autoReturnSpeed * Time.deltaTime;
                if (value < 0)
                    value = 0;
            }
            else if (value < 0)
            {
                value += autoReturnSpeed * Time.deltaTime;
                if (value > 0)
                    value = 0;
            }
        }

        if (Mathf.Abs(value) < Dead)
            value = 0f;

        value = Mathf.Clamp(value, -1f, 1f);

        return value;
    }
}
