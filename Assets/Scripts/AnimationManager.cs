using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class AnimationManager : MonoBehaviour
{
    public Animator animator; // Reference to the Animator component
    public Rigidbody2D rb; // Reference to the Rigidbody2D component
    public Button idleButton; // Reference to the Idle button
    public Button stopButton; // Reference to the Stop Animation button
    public Button walkButton;
    public Button jumpButton;

    public float speed = 2f; // Speed of movement
    private bool isAnimating = false; // To check if the animation loop is running
    private Coroutine animationCoroutine; // To store the coroutine

    private bool isWalkingLeft = true; // Determine the direction of movement

    private void Start()
    {
        // Assign listeners to the buttons
        idleButton.onClick.AddListener(OnIdleButtonClick);
        walkButton.onClick.AddListener(OnWalkButtonClick);
        jumpButton.onClick.AddListener(OnJumpButtonClick);
        stopButton.onClick.AddListener(OnStopButtonClick);

        //rb = GetComponent<Rigidbody2D>(); // Get Rigidbody2D for movement
    }

    private void Update()
    {
        // // Move the object if it's walking
        // if (isAnimating && animator.GetCurrentAnimatorStateInfo(0).IsName("WalkSpecial"))
        // {
        //     if (isWalkingLeft)
        //     {
        //         rb.velocity = new Vector2(-speed, rb.velocity.y); // Move left
        //         Debug.Log("Moving Left with velocity: " + rb.velocity.x); // Debugging movement
        //     }
        //     else
        //     {
        //         rb.velocity = new Vector2(speed, rb.velocity.y); // Move right
        //     }
        // }
        // else
        // {
        //     // If not walking, stop any movement
        //     rb.velocity = Vector2.zero;
        // }
    }

    private IEnumerator WalkLeft()
    {
        while (isAnimating)
        {
            // Move the character to the left
            Vector2 newPosition = rb.position + Vector2.left * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);

            yield return new WaitForFixedUpdate(); // Ensure movement occurs in sync with physics
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        Debug.Log("Collision Detected with: " + collision.gameObject.name);
        // Check if the collision is with the plant object
        if (collision.gameObject.CompareTag("Plant"))
        {
            Debug.Log("Collision with Plant!");
            // Flip the direction when colliding with the plant
            isWalkingLeft = !isWalkingLeft;

            // Flip the character's sprite by scaling it
            Vector3 localScale = transform.localScale;
            localScale.x *= -1;
            transform.localScale = localScale;

            // Reverse the walking direction
            StartCoroutine(WalkInOppositeDirection());
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        Debug.Log("Trigger Detected with: " + collision.gameObject.name); // Debugging trigger
    }

    private void OnJumpButtonClick()
    {
        isAnimating = true;
        animator.SetBool("Idle", false);
        animator.ResetTrigger("IdleSwitch");
        animator.ResetTrigger("Walk");
        animator.SetBool("isWalking", false);
        animationCoroutine = StartCoroutine(JumpAnimation());
    }

    private void OnWalkButtonClick()
    {
        isAnimating = true;
        animator.SetBool("isWalking", true);
        animator.SetBool("Idle", false);
        animator.ResetTrigger("IdleSwitch");
        animator.ResetTrigger("Jump");
        animationCoroutine = StartCoroutine(WalkAnimation());
        StartCoroutine(WalkLeft());
    }

    private void OnIdleButtonClick()
    {
        // Start the animation loop if it's not already running
        isAnimating = true;
        animator.SetBool("Idle", true);
        animator.ResetTrigger("IdleSwitch");
        animator.ResetTrigger("Walk");
        animator.SetBool("isWalking", false);
        animator.ResetTrigger("Jump");
        animationCoroutine = StartCoroutine(SwitchAnimations());
    }

    private void OnStopButtonClick()
    {
        if (isAnimating)
        {
            // Stop the animation loop
            isAnimating = false;

            // Stop the animation coroutine
            StopAllCoroutines();

            // Reset Animator parameters and return to a non-animated state
            animator.SetBool("Idle", false);
            animator.ResetTrigger("IdleSwitch");
            animator.ResetTrigger("Walk");
            animator.SetBool("isWalking", false);
            animator.ResetTrigger("Jump");

            animator.SetTrigger("Stop");
        }
    }

    private IEnumerator SwitchAnimations()
    {
        // Start in Idle mode
        animator.SetBool("Idle", true);

        while (isAnimating)
        {
            yield return new WaitForSeconds(Random.Range(6f, 12f));

            // Switch to IdleSpecial
            animator.SetTrigger("IdleSwitch");

            yield return new WaitForSeconds(1f);
            animator.ResetTrigger("IdleSwitch");
        }
    }

    private IEnumerator WalkAnimation()
    {
        animator.SetTrigger("Walk");
        yield return new WaitForEndOfFrame();
        animator.ResetTrigger("Walk");
    }

    private IEnumerator JumpAnimation()
    {
        animator.SetTrigger("Jump");
        yield return new WaitForEndOfFrame();
        animator.ResetTrigger("Jump");
    }

    private IEnumerator WalkInOppositeDirection()
    {
        while (isAnimating)
        {
            // Move in the opposite direction (right if initially left)
            Vector2 newPosition = rb.position + (isWalkingLeft ? Vector2.left : Vector2.right) * speed * Time.fixedDeltaTime;
            rb.MovePosition(newPosition);

            yield return new WaitForFixedUpdate(); // Ensure movement syncs with physics
        }
    }
}
