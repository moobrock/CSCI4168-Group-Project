using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyController : MonoBehaviour
{
    private NavMeshAgent navMeshAgent;
    private Vector3 destination;

    private float moveDist = 1f;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();

        destination = GameManager.gameManager.barnAttackPosition.position;

        SetDestination();
    }

    private void SetDestination()
    {
        SetDestination(destination);
    }

    private void SetDestination(Vector3 position)
    {
        destination = position;
        navMeshAgent.SetDestination(position);
    }

    private void Update()
    {
        DevelopmentControls();
    }

    // manually control enemy for development
    private void DevelopmentControls()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            navMeshAgent.Move(new Vector3(0, 0, moveDist));
        }

        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            navMeshAgent.Move(new Vector3(0, 0, -moveDist));
        }

        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            navMeshAgent.Move(new Vector3(-moveDist, 0, 0));
        }

        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            navMeshAgent.Move(new Vector3(moveDist, 0, 0));
        }
    }
}
