using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PigMovement : MonoBehaviour
{
    private GameObject target;
    private Rigidbody2D rb;
    private PigController controller;

    public Transform player;

    private SetupFinding setupFinding;
    private Find finder;
    private List<Action> actions = new List<Action>();
    private int curIndex;
    
    public bool isCharging;
    
    [SerializeField] private float recalcInterval = 0.5f;
    private float recalcTimer;

    public void Initialize(SetupFinding setupFindingRef, GameObject targetRef)
    {
        setupFinding = setupFindingRef != null ? setupFindingRef : SetupFinding.Instance;
        
        finder = setupFinding == null ? GetComponent<Find>() : null;
        
        if (finder == null)
            finder = FindObjectOfType<Find>();
        
        target = targetRef;
        
        
        rb = GetComponent<Rigidbody2D>();
        controller = GetComponent<PigController>();
    }

    public void StartPath()
    {
        actions = finder.Findd(transform.position, target.transform.position);
        curIndex = 0;
        recalcTimer = recalcInterval;
    }

    public void Movement()
    {
        recalcTimer -= Time.fixedDeltaTime;
        if (recalcTimer <= 0f)
        {
            actions = finder.Findd(transform.position, target.transform.position);
            curIndex = 0;
            recalcTimer = recalcInterval;
        }
        
        if (actions == null || curIndex >= actions.Count) return;
        
        var action = actions[curIndex];
        switch (action.StateAction)
        {
            case StateAction.move:
                HandleMove(action);
                break;
            case StateAction.jump:
                HandleJump(action);
                break;
            case StateAction.fall:
                curIndex++;
                break;
            default:
                curIndex++;
                break;
        }
    }

    private void HandleMove(Action action)
    {
        Vector2 targetPos = action.Target;
        Vector2 direction = (targetPos - (Vector2)transform.position).normalized;
        
        FlipSprite(direction.x);
        rb.velocity = new Vector2(direction.x * controller.enemyType.moveSpeed, rb.velocity.y);
        
        if (Vector2.Distance(transform.position, targetPos) < 0.1f)
            curIndex++;
    }

    private void HandleJump(Action action)
    {
        Vector2 landingPos = action.Target;
        bool groundCheck = Physics2D.OverlapCircle(landingPos, controller.enemyType.groundCheckRadius
            , controller.enemyType.groundLayer);
        
        if (!groundCheck)
        {
            curIndex++;
            return;
        }
        
        controller.Jump.Jump(action.ForceJump);
        curIndex++;
    }

    private void FlipSprite(float directionX)
    {
        Vector3 scale = transform.localScale;
        scale.x = Mathf.Abs(scale.x) * (directionX < 0 ? 1 : -1);
        transform.localScale = scale;
    }
}
