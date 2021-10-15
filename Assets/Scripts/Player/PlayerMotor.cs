using UnityEngine;
using Mirror;

namespace Untitled.PlayerSystem
{
    public class PlayerMotor : NetworkBehaviour
    {
        /// <summary>
        /// Character Controller reference
        /// </summary>
        [SerializeField]
        private CharacterController controller;

        /// <summary>
        /// Player model animator
        /// </summary>
        [SerializeField]
        private Animator animator;

        /// <summary>
        /// Gravity applied to player controller
        /// </summary>
        [SerializeField]
        private float gravity = -9.81f;

        /// <summary>
        /// Player speed
        /// </summary>
        [SerializeField]
        private float speed = 5f;

        /// <summary>
        /// Ground mask
        /// </summary>
        [SerializeField]
        private LayerMask groundMask;

        /// <summary>
        /// Current velocity applied to character controller
        /// </summary>
        private Vector3 velocity;
        /// <summary>
        /// Current animator speed 
        /// </summary>
        private float animatorSpeed;

        private void Update()
        {
            if (!hasAuthority)
                return;

            if (velocity.y < 0)
            {
                velocity.y = -2f;
            }

            float x = Input.GetAxis("Horizontal");
            float z = Input.GetAxis("Vertical");

            if (x != 0 || z != 0)
            {
                animatorSpeed = Mathf.Lerp(animatorSpeed, 1f, Time.deltaTime * 10f);
            }
            else
            {
                animatorSpeed = Mathf.Lerp(animatorSpeed, 0f, Time.deltaTime * 10f);
            }

            animator.SetFloat("Speed", animatorSpeed);

            Vector3 move = Vector3.right * x + Vector3.forward * z;

            controller.Move(move * speed * Time.deltaTime);

            velocity.y += gravity * Time.deltaTime;

            controller.Move(velocity * Time.deltaTime);

            // Look towards
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit, 100f, groundMask))
            {
                Vector3 targetPostition = new Vector3(hit.point.x,
                                       transform.position.y,
                                       hit.point.z);
                transform.LookAt(targetPostition);
            }
        }
    }
}