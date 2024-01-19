using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Mover : MonoBehaviour
{
    [SerializeField] float Speed = 6f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float verticalVelocity = 0;
        float horizontalVelocity = 0;

        if (Input.GetKey(KeyCode.W))
        {
            verticalVelocity = 1;
        }
        else if (Input.GetKey(KeyCode.S))
        {
            verticalVelocity = -1;
        }

        if (Input.GetKey(KeyCode.A))
        {
            horizontalVelocity = -1;
        }
        else if (Input.GetKey(KeyCode.D))
        {
            horizontalVelocity = 1;
        }

        transform.Translate(new Vector3(horizontalVelocity, verticalVelocity, 0).normalized * Speed * Time.deltaTime);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Collision");
        if(collision.GetComponent<Encounter>() != null)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("CombatScene");
        }
    }
}
