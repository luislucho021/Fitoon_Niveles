using UnityEngine;

public class BotIA : MonoBehaviour
{
    public bool network;
    public float rayLength = 6.0f;  // Longitud del rayo
    public float sideRayAngle = 20.0f;
    public LayerMask obstacleLayer;  // Capa de obstáculos

    public string wallTag = "Wall";
    public string cpTag = "Checkpoint";

    public float speedVariance = 0.8f;
    public float rotationVariance = 0.95f;


    private float moveV = 1f;
    private float moveH = 0f;
    private BotController controller;
    private BotNoNetwork controller_noNetwork;
    private bool blockedFront, blockedLeft, blockedRight;
    private float randomRotDir;
    private Color frontRayColor = Color.green;  // Color del rayo frontal
    private Color leftRayColor = Color.green;  // Color del rayo izquierdo
    private Color rightRayColor = Color.green;  // Color del rayo derecho

    private void Start()
    {
        controller = GetComponent<BotController>();
        controller_noNetwork = GetComponent<BotNoNetwork>();
        moveV = 1f * Random.Range(speedVariance, 1f);
        randomRotDir = (Random.Range(0, 2) == 0) ? -1f : 1f;
    }

    void Update()
    {
        // Configurar el raycast
        RaycastHit hitFront, hitLeft, hitRight;

        Vector3 left = Quaternion.Euler(0, -sideRayAngle, 0) * transform.forward;
        Vector3 right = Quaternion.Euler(0, sideRayAngle, 0) * transform.forward;

        if (Physics.Raycast(transform.position, transform.forward, out hitFront, rayLength, obstacleLayer))
        {
            if (hitFront.collider.CompareTag(wallTag) || (hitFront.collider.CompareTag(cpTag) && CheckAngle(hitFront.collider)))
            {
                blockedFront = true;
                frontRayColor = Color.red;
            }
            else
            {
                blockedFront = false;
                frontRayColor = Color.green;
                moveH = 0;
            }
        }
        else
        {
            blockedFront = false;
            frontRayColor = Color.green;
            moveH = 0;
        }

        if (Physics.Raycast(transform.position, left, out hitLeft, rayLength, obstacleLayer))
        {
            if (hitLeft.collider.CompareTag(wallTag) || (hitLeft.collider.CompareTag(cpTag) && CheckAngle(hitLeft.collider)))
            {
                blockedLeft = true;
                leftRayColor = Color.red;
            }
            else blockedLeft = false;
        }
        else blockedLeft = false;

        if (Physics.Raycast(transform.position, right, out hitRight, rayLength, obstacleLayer))
        {
            if (hitRight.collider.CompareTag(wallTag) || (hitRight.collider.CompareTag(cpTag) && CheckAngle(hitRight.collider)))
            {
                blockedRight = true;
                rightRayColor = Color.red;
            }
            else blockedRight = false;
        }
        else blockedRight = false;


        if (blockedFront && blockedLeft && blockedRight)
        {
            if (hitRight.distance < hitLeft.distance) moveH -= 1f * Random.Range(rotationVariance, 1);
            else moveH += 1f * Random.Range(rotationVariance, 1);
        }

        if (blockedLeft && !blockedRight)
        {
            moveH += 1f * Random.Range(rotationVariance, 1);
        }

        if (!blockedLeft && blockedRight)
        {
            moveH -= 1f * Random.Range(rotationVariance, 1);
        }

        if (blockedFront && !blockedLeft && !blockedRight)
        {
            moveH += randomRotDir * Random.Range(rotationVariance, 1);
        }

        if (!blockedRight) rightRayColor = Color.green;
        if (!blockedLeft) leftRayColor = Color.green;
        if (!blockedFront) frontRayColor = Color.green;


        // Aplicar movimiento
        
        if (network)
        {
            controller.SetMovement(moveV, moveH);
        }
        else
        {
            controller_noNetwork.SetMovement(moveV, moveH);
        }
    }

    private bool CheckAngle(Collider collider)
    {
        if (Vector3.Angle(transform.forward, collider.transform.forward) < 90) return false; //Ignore wall
        else return true; //Collide with wall
    }

    void OnDrawGizmos()
    {
        // Dibuja los rayos en la escena con sus respectivos colores
        DrawRayWithColor(transform.position, transform.forward * rayLength, frontRayColor);
        DrawRayWithColor(transform.position, Quaternion.Euler(0, -sideRayAngle, 0) * transform.forward * rayLength, leftRayColor);
        DrawRayWithColor(transform.position, Quaternion.Euler(0, sideRayAngle, 0) * transform.forward * rayLength, rightRayColor);
    }

    void DrawRayWithColor(Vector3 origin, Vector3 direction, Color color)
    {
        Gizmos.color = color;
        Gizmos.DrawRay(origin, direction);
    }


}