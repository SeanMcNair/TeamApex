using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishController : PlayerController
{
    [SerializeField] private float fallSpeed = 1;
    [SerializeField] private float inflatedSpeed = 2;
    [SerializeField] private Vector2 inflatedColliderSize = Vector2.one;
    private Vector2 startSize;
    [SerializeField] private AnimatorStateProperty animProp;
    [SerializeField] private bool holdToInflate;
    [SerializeField] private float inflateTime = 3;
    [SerializeField] private bool useAddedForce;
    [SerializeField] private bool consistentRiseForce;
    private bool inflated;
    public bool IsInflated { get { return inflated; } }

    private Animator animator;
    private float vertSpeed;

    private Coroutine inflateCoroutine;

    public override void Start()
    {
        base.Start();
        startSize = conCollider.size;
        vertSpeed = -fallSpeed;
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    public override void Update ()
    {
        GetInputs();
	}

    public override void FixedUpdate()
    {
        Move();
    }

    public override void Move()
    {
        move = new Vector2(inputHor * curSpeed, vertSpeed) * Time.deltaTime;
        transform.Translate(move);
    }

    public void Inflate(bool _inflate)
    {
        if (inflateCoroutine != null)
            StopCoroutine(inflateCoroutine);
        inflateCoroutine = StartCoroutine(StartInflate(_inflate));
        
    }

    IEnumerator StartInflate(bool _inflate)
    {
        var state = animProp.stateToPlay;
        var targetSpeed = inflatedSpeed;
        var targetSize = inflatedColliderSize;
        if (!_inflate)
        {
            state = animProp.exitState;
            targetSpeed = -fallSpeed;
            targetSize = startSize;
        }  
        animator.CrossFade(state, animProp.crossfadeTime);
        var size = conCollider.size;
        var speed = velocityDirection.y;
        float timer = 0;
        while (timer < animProp.crossfadeTime)
        {
            timer += Time.deltaTime;
            var perc = timer / animProp.crossfadeTime;

            vertSpeed = Mathf.Lerp(speed, targetSpeed, perc);
            conCollider.size = Vector2.Lerp(size, targetSize, perc);
            conCollider.offset = new Vector2(conCollider.offset.x, conCollider.size.y / 2);
            yield return new WaitForFixedUpdate();
        }
        if (!_inflate)
            inflated = false;
    }

}
