using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetGameSpeed : MonoBehaviour
{
    [SerializeField] float gameSpeed = 1.0f;

    void Start()
    {
        Time.timeScale = gameSpeed;
    }
}
