using UnityEngine;

namespace CityZero.Core.Utilities
{
    public static class UnityVersionCompat
    {
        public static Vector3 GetPlanarVelocity(Rigidbody body)
        {
#if UNITY_6000_0_OR_NEWER
            Vector3 velocity = body.linearVelocity;
#else
            Vector3 velocity = body.velocity;
#endif
            velocity.y = 0f;
            return velocity;
        }

        public static void SetVelocity(Rigidbody body, Vector3 velocity)
        {
#if UNITY_6000_0_OR_NEWER
            body.linearVelocity = velocity;
#else
            body.velocity = velocity;
#endif
        }
    }
}
