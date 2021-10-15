using UnityEngine;

namespace Untitled.PlayerSystem
{
    public class PlayerCamera : MonoBehaviour
    {
        public static PlayerCamera instance;

        [SerializeField]
        private Transform target;

        /// <summary>
        /// Maximum distance from target
        /// </summary>
        [SerializeField]
        private float maxDistanceFromTarget;

        /// <summary>
        /// Lerp factor for a smoooooooth movement
        /// </summary>
        [SerializeField]
        private float lerpFactor = 2f;

        private void Awake()
        {
            instance = this;
        }

        public void SetTransformTarget(Transform target)
        {
            this.target = target;
        }

        private void Update()
        {
            if(target && Vector3.Distance(target.position, transform.position) > maxDistanceFromTarget)
            {
                transform.position = Vector3.Lerp(transform.position, target.position, Time.deltaTime * lerpFactor);
            }
        }
    }
}