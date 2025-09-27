
using System;
using UnityEngine;

public class DebugLifecycle : MonoBehaviour
{
    void Awake()        { Debug.Log($"{name} Awake  activeSelf={gameObject.activeSelf}"); }
    void OnEnable()     { Debug.Log($"{name} OnEnable"); }
    void Start()        { Debug.Log($"{name} Start   activeSelf={gameObject.activeSelf}"); }
    void OnDisable()    { Debug.Log($"{name} OnDisable  (誰かが SetActive(false) した)"); }
    void OnDestroy()    { Debug.Log($"{name} OnDestroy\n{Environment.StackTrace}"); }
    void OnBecameInvisible() { Debug.Log($"{name} OnBecameInvisible"); }
    void OnTriggerEnter2D(Collider2D other)
    {
        Debug.Log($"{name} Trigger with {other.tag}/{other.name}");
    }
}