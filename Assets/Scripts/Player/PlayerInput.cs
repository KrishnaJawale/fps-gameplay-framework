using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerInput : MonoBehaviour
{
    [Header("Keybinds")]
    [SerializeField]
    private KeyCode restartKey = KeyCode.V;

    void Update()
    {
        if (Input.GetKeyDown(restartKey)) SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }
}
