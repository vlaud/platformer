using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Inst;
    private void Awake() => Inst = this;

    public PlayerMovement Player;
    public Controller Controller;
    public CameraController CameraController;
    public Transform Mask;

    [Range(0f, 1f)]
    [SerializeField] private float gameTimeScale = 1f;
    [Range(0f, 0.02f)]
    [SerializeField] private float gameFixedDeltaTime = 0.02f;
    public float GameTimeScale => gameTimeScale;
    public float GameFixedDeltaTime => gameFixedDeltaTime;
    public float GameUnscaledDeltaTime => Time.unscaledDeltaTime;
    public float GameFixedUnscaledDeltaTime => Time.fixedUnscaledDeltaTime;
    public float GameDeltaTime => Time.deltaTime;

    private void Update()
    {
        DoSlowmotion();
    }

    public void DoSlowmotion()
    {
        gameTimeScale = Mathf.Clamp(gameTimeScale, 0f, 1f);
        Time.timeScale = gameTimeScale;
        gameFixedDeltaTime = Time.fixedDeltaTime = Time.timeScale * 0.02f;
    }

    public void PauseGame()
    {
        gameTimeScale = 0f;
    }

    public void StartGame()
    {
        gameTimeScale = 1f;
    }
}
