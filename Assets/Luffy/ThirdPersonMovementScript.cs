using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonMovement : MonoBehaviour
{
    public CharacterController controller;
    public Transform cam;

    public float speed = 6f;
    public float gravity = -9.81f;
    public float jumpHeight = 3f;


    public float turnSmoothTime = 0.1f;
    float turnSmoothVelocity;

    public Transform groundCheck;
    public float groundDistance = 0.4f;
    public LayerMask groundMask;

    Vector3 velocity;
    bool isGrounded;

    Animator animator;




    void Start()
    {
    Cursor.lockState = CursorLockMode.Locked;

    animator = GetComponent<Animator>();
    }



    // Update is called once per frame
    void Update()
    {
        // Gravity https://www.youtube.com/watch?v=_QajrabyTJc&list=WL&index=4&t=53s
        isGrounded = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        if(isGrounded && velocity.y < 0)
        {
            velocity.y = -2f;
        }


        // Third Person Movement
        float horizontal = Input.GetAxisRaw("Horizontal");
        float vertical = Input.GetAxisRaw("Vertical");
        Vector3 direction = new Vector3(horizontal, 0f, vertical).normalized;

        if(direction.magnitude >= 0.1f)
        {
            float targetAngle = Mathf.Atan2(direction.x, direction.z) * Mathf.Rad2Deg + cam.eulerAngles.y;
            float angle = Mathf.SmoothDampAngle(transform.eulerAngles.y, targetAngle, ref turnSmoothVelocity, turnSmoothTime );
            transform.rotation = Quaternion.Euler(0f, angle, 0f);

            Vector3 moveDir = Quaternion.Euler(0f, targetAngle, 0f) * Vector3.forward;
            controller.Move(moveDir.normalized * speed * Time.deltaTime);

             // Update the "Speed" parameter in the Animator based on character speed
            animator.SetFloat("Speed", speed * direction.magnitude);
            animator.SetBool("IsIdle", false); // Character is not idle
            // ... (existing movement code)
        }
        else
        {
            // If not moving, set the "Speed" parameter to 0 and indicate idle
            animator.SetFloat("Speed", 0f);
            animator.SetBool("IsIdle", true); // Character is idle
        }

        // Jump https://www.youtube.com/watch?v=_QajrabyTJc&list=WL&index=4&t=53s
        if(Input.GetButtonDown("Jump") && isGrounded)
        {
            velocity.y = Mathf.Sqrt(jumpHeight * -2f * gravity);
        }

        velocity.y += gravity * Time.deltaTime;

        controller.Move(velocity * Time.deltaTime);
    }
}
