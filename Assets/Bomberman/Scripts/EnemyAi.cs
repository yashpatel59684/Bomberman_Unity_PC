using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public sealed class EnemyAi : MonoBehaviour
{
    [Range(0, 10)]
    [SerializeField] float runSpeed = 2.5f;
    List<Vector3> nextTargetDirections;
    Vector3 previousNextTargetDirection, nextTargetPosition, nextTargetDirection;
    bool isPosChecking;
    EnemyAi() { }
    void Start()
    {
        previousNextTargetDirection = nextTargetDirection = Vector3.zero;
        nextTargetPosition = transform.position;
        InvokeRepeating(nameof(CheckPos), 1f, .2f);
    }
    void CheckPos()
    {
        if (isArrived)
        {
            StartCoroutine(GetNextTargetPos());
        }
    }
    bool isArrived => Vector3.Distance(transform.position, nextTargetPosition) <= 0f;
    void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Brick") || collision.collider.CompareTag("Bomb"))
        {
            Start();
        }
    }
    void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Wall"))
        {
            Start();
        }
        if (collision.collider.CompareTag("Player"))
        {
            GameManager.Instance?.PlayerDie();
        }
    }
    void FixedUpdate()
    {
        MoveEnemyAi(nextTargetPosition);
    }
    void OnDestroy()
    {
        CancelInvoke();
    }
    void MoveEnemyAi(Vector3 pos)
    {
        transform.position = Vector3.MoveTowards(transform.position, pos, runSpeed * Time.deltaTime);
    }
    IEnumerator GetNextTargetPos()
    {
        yield return new WaitForEndOfFrame();
        if (isPosChecking) yield break;
        isPosChecking = true;
        nextTargetDirections = new List<Vector3>();
        nextTargetDirection = Vector3.zero;
        SetNextTargetDirection(Vector3.up);
        SetNextTargetDirection(Vector3.right);
        SetNextTargetDirection(Vector3.down);
        SetNextTargetDirection(Vector3.left);
        if (nextTargetDirections.Contains(previousNextTargetDirection))
        {
            if (previousNextTargetDirection.Equals(Vector3.up))
            {
                RemoveNextTargetDirections(Vector3.down);
            }
            else if (previousNextTargetDirection.Equals(Vector3.down))
            {
                RemoveNextTargetDirections(Vector3.up);
            }
            else if (previousNextTargetDirection.Equals(Vector3.right))
            {
                RemoveNextTargetDirections(Vector3.left);
            }
            else if (previousNextTargetDirection.Equals(Vector3.left))
            {
                RemoveNextTargetDirections(Vector3.right);
            }
        }
        else
        {
            yield return new WaitForSeconds(.2f);
        }
        if (nextTargetDirections.Count > 0)
        {
            if (GameManager.Instance.EnemyObj.Count <= GameManager.Instance.MinEnemyCriticalCount && GameManager.Instance.PlayerObj != null)
            {
                Vector3 playerPos = GameManager.Instance.PlayerObj.position;
                float maxDistance = 0f;
                foreach (var direction in nextTargetDirections)
                {
                    float distance = Vector3.Distance(transform.position + direction, playerPos);
                    if (distance > maxDistance)
                    {
                        maxDistance = distance;
                        previousNextTargetDirection = nextTargetDirection = direction;
                    }
                }
            }
            else
            {
                var getRandomDirection = UnityEngine.Random.Range(0, nextTargetDirections.Count);
                previousNextTargetDirection = nextTargetDirection = nextTargetDirections[getRandomDirection];
            }
            nextTargetPosition = transform.position + nextTargetDirection;
            //to avoid Issue with ai that it recieved nextTargetPosition to wall position so it stuck.
            nextTargetPosition = GameManager.Instance.GetNearestAbsoluteLocation(nextTargetPosition);
        }
        isPosChecking = false;
    }

    void SetNextTargetDirection(Vector3 direction, bool extra = true)
    {
        if (IsWayToGo(direction) && extra)
        {
            AddNextTargetDirections(direction);
        }
    }
    bool IsWayToGo(Vector3 towords)
    {
        RaycastHit2D[] raycastHit2D = Physics2D.RaycastAll(transform.position, towords, 1f);
        if (raycastHit2D.Length > 0)
        {
            foreach (var item in raycastHit2D)
            {
                if (item.collider.CompareTag("Wall") || item.collider.CompareTag("Brick"))
                {
                    return false;
                }
            }
        }
        return true;
    }
    void AddNextTargetDirections(Vector3 direction)
    {
        if (!nextTargetDirections.Contains(direction))
        {
            nextTargetDirections.Add(direction);
        }
    }
    void RemoveNextTargetDirections(Vector3 direction)
    {
        if (nextTargetDirections.Contains(direction))
        {
            nextTargetDirections.Remove(direction);
        }
    }
}
