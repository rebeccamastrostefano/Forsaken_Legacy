using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FlowerEnemy : MonoBehaviour
{
    public GameObject player;

    public Collider areaOfAttack;
    private Animator _animator;

    private bool canSee;
    public GameObject bulletPrefab;
    private GameObject bullet;
    public GameObject indicatorPrefab;

    public float bulletSpeed = 5f;
    public float curveForce = 20f;

    // Start is called before the first frame update
    void Start()
    {
        _animator = GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canSee = true;
            Debug.Log("Player entered the area of attack");
            InvokeRepeating("StartShoot", 0.5f, 5f);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            canSee = false;
            Debug.Log("Player exited the area of attack");
            CancelInvoke();
        }
    }

    private void StartShoot()
    {
        if (canSee && GetComponentInChildren<Renderer>().isVisible)
        {
            Debug.Log("Shooting!");
            _animator.SetTrigger("Shoot");
        }
        else
        {
            return;
        }
    }

    // <<--- Method Called in Animation Events --->>
    private void CreateBullet()
    {
        Vector3 bulletPos = transform.position;
        bulletPos.y += 0.5f;
        bullet = Instantiate(bulletPrefab, bulletPos, transform.rotation);
    }

    private void Shoot()
    {
        if (bullet)
        {
            Rigidbody rb = bullet.GetComponent<Rigidbody>();

            // Calculate the direction to the player
            Vector3 directionToPlayer = (player.transform.position - bullet.transform.position).normalized;

            // Calculate the distance to the player
            float distanceToPlayer = Vector3.Distance(transform.position, player.transform.position);

            // Adjust speed and force based on distance
            float speed = Mathf.Clamp(distanceToPlayer / bulletSpeed, 0, 2);

            // Spawn Indicator
            float timeOfFlight = distanceToPlayer / speed;
            Vector3 landingPosition = bullet.transform.position + directionToPlayer * speed * timeOfFlight + Physics.gravity * timeOfFlight * timeOfFlight * 0.5f;
            SpawnIndicator(landingPosition);

            // Apply the adjusted initial velocity
            rb.velocity = directionToPlayer * speed;

            // Apply a force to curve the bullet
            rb.AddForce(Vector3.up * curveForce, ForceMode.Impulse);
            rb.useGravity = true;
        }
    }

    private void SpawnIndicator(Vector3 landingPosition)
    {
        landingPosition.y = player.transform.position.y - 0.3f;
        // Instantiate the indicator at the landing position
        GameObject indicator = Instantiate(indicatorPrefab, landingPosition, Quaternion.identity);
    }
}