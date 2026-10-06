using UnityEngine;

public class WaypointWalker : MonoBehaviour
{
    public Transform[] waypoints;
    public float moveSpeed = 2f;
    public float turnSpeed = 8f; 
    
    private int currentPoint = 0;
    private Animator anim;

    void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void OnEnable()
    {
        currentPoint = 0;
        
        if (waypoints.Length > 0 && waypoints[0] != null)
        {
            transform.position = waypoints[0].position;
            transform.rotation = waypoints[0].rotation;
        }

        if (anim != null)
        {
            anim.SetBool("isIdle", false);
        }
    }

    void Update()
    {
        if (waypoints.Length == 0 || currentPoint >= waypoints.Length) return;

        Transform target = waypoints[currentPoint];
        
        transform.position = Vector3.MoveTowards(transform.position, target.position, moveSpeed * Time.deltaTime);

        Vector3 direction = (target.position - transform.position).normalized;
        if (direction != Vector3.zero)
        {
            Quaternion lookRotation = Quaternion.LookRotation(new Vector3(direction.x, 0f, direction.z));
            transform.rotation = Quaternion.Slerp(transform.rotation, lookRotation, Time.deltaTime * turnSpeed);
        }

        if (Vector3.Distance(transform.position, target.position) < 0.05f)
        {
            currentPoint++;

            if (currentPoint >= waypoints.Length)
            {
                if (anim != null)
                {
                    anim.SetBool("isIdle", true);
                }
            }
        }
    }
}