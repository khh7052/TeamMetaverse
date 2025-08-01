using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

public class PlayerController : BaseController
{
    protected GameObject closestTarget;

    private Camera playerCamera;

    private float minDistance;

    protected override void Awake()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        lookDirection = new Vector2(1, 0);

        if (weaponPrefab != null)
        {
            weaponHandler = Instantiate(weaponPrefab, weaponPivot);
        }
        else
        {
            weaponHandler = GetComponentInChildren<WeaponHandler>();
        }
    }

    protected override void Start()
    {
        base.Start();
        playerCamera = Camera.main;
    }

    protected override void Update()
    {
        AttackCheck();
        if(movementDirection != Vector2.zero)
            lookDirection = movementDirection.normalized;
        else if(movementDirection == Vector2.zero && closestTarget != null)
            lookDirection = DirectionToTarget();
        HandleAction();
        Rotate(lookDirection);
        HandleAttackDelay();
        closestTarget = null;
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
        //Movement(moveDirection);
    }

    private Vector2 DirectionToTarget()
    {
        return (closestTarget.transform.position - transform.position).normalized;
    }

    protected override void HandleAction()
    {
        isAttacking = movementDirection == Vector2.zero ? true : false;
    }

    protected override void HandleAttackDelay()
    {
        if (weaponHandler == null)
            return;

        if (timeSinceLastAttack <= weaponHandler.Delay)
        {
            timeSinceLastAttack += Time.deltaTime;
        }

        if (isAttacking && timeSinceLastAttack > weaponHandler.Delay)
            if (closestTarget != null && isAttacking && timeSinceLastAttack > weaponHandler.Delay)
            {
                timeSinceLastAttack = 0;
                Attack();
        }
    }

    protected override void Movement(Vector2 direction)
    {
        direction = direction * 5;
        rigidBody.velocity = direction;
    }

    protected override void Attack()
    {
        if (movementDirection == Vector2.zero)
        {
            weaponHandler?.Attack();
        }
    }

    protected virtual void AttackCheck()
    {
        Collider2D[] adjObj = Physics2D.OverlapCircleAll(transform.position, weaponHandler.AttackRange);
        minDistance = weaponHandler.AttackRange;
        for (int i = 0; i < adjObj.Length; i++)
        {
            float distance;
            if (adjObj[i] != null && adjObj[i].CompareTag("Monster"))
            {
                distance = Vector2.Distance(transform.position, adjObj[i].transform.position);
                if (distance <= minDistance)
                {
                    minDistance = distance;
                    closestTarget = adjObj[i].gameObject;
                }
            }
        }
    }

    private void OnMove(InputValue inputValue)
    {
        movementDirection = inputValue.Get<Vector2>();
        movementDirection = movementDirection.normalized;
    }
}
