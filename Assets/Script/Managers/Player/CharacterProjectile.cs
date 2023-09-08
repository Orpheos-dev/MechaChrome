//using UnityEngine;
//
//public class CharacterProjectile : MonoBehaviour
//{
//    public float speed = 10f;
//    public int damage = 1;
//    public float lifetime = 3f;
//
//    private Rigidbody rb;
//
//    private void Awake()
//    {
//        rb = GetComponent<Rigidbody>();
//        Destroy(gameObject, lifetime);
//    }
//
//    private void FixedUpdate()
//    {
//        if (rb != null)
//        {
//            Vector3 movement = transform.forward * speed * Time.fixedDeltaTime;
//            rb.MovePosition(rb.position + movement);
//        }
//    }
//
//    private void OnTriggerEnter(Collider other)
//	{
//		//Debug.Log("Trigger Entered by: " + other.gameObject.name);
//		// Only proceed if the object the projectile collided with has an Enemy component
//		if (other.gameObject.GetComponent<Enemy>() != null)
//		{
//			Enemy enemy = other.GetComponent<Enemy>();
//			enemy.TakeDamage(damage);
//			Destroy(gameObject); 
//		}
//	}
//
//}
