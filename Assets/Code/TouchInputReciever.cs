using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TouchInputReciever : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        // Iterate through touch input (should work for web too!)
        foreach (Touch touch in Input.touches)
        {
            // If the touch is on the left side of the screen
            if (touch.position.x < Screen.width / 2)
            {
                // Move the player left
                transform.Translate(Vector3.left * Time.deltaTime);
            }
            // If the touch is on the right side of the screen
            else if (touch.position.x > Screen.width / 2)
            {
                // Move the player right
                transform.Translate(Vector3.right * Time.deltaTime);
            }
        }

        // // Iterate through mouse input (should work for web too!)
        // if (Input.GetMouseButton(0))
        // {
        //     // If the mouse is on the left side of the screen
        //     if (Input.mousePosition.x < Screen.width / 2)
        //     {
        //         // Move the player left
        //         transform.Translate(Vector3.left * Time.deltaTime);
        //     }
        //     // If the mouse is on the right side of the screen
        //     else if (Input.mousePosition.x > Screen.width / 2)
        //     {
        //         // Move the player right
        //         transform.Translate(Vector3.right * Time.deltaTime);
        //     }
        // }
    }
}
