using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerDie : DeathBehaviour
{
    public override void Die ( Vector3 direction )
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().name);
    }

}