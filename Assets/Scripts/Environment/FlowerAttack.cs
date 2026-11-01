using System.Collections;
using UnityEngine;
using UnityEngine.Animations.Rigging;

public class FlowerAttack : MonoBehaviour
{
    [SerializeField] private Transform stemRoot;
    [SerializeField] private Transform head;
    [SerializeField] private Transform strikeTarget;
    [SerializeField] private Transform lookTarget;
    [SerializeField] private Vector3 faceAxis = Vector3.up;
    [SerializeField] private Rig rig;
    [SerializeField] private float rigBlendTime = 0.4f;

    [SerializeField] private float detectRange = 30f;
    [SerializeField] private float aimHeight = 1f;
    [SerializeField] private LayerMask sightBlockers;
    [SerializeField] private bool ignoreDisguised = true;

    [SerializeField] private float lean = 0.2f;
    [SerializeField] private float followSpeed = 5f;

    [SerializeField] private float damage = 100f;
    [SerializeField] private float cooldown = 2f;
    [SerializeField] private float hitRadius = 2f;
    [SerializeField] private float windupTime = 0.5f;
    [SerializeField] private float strikeTime = 0.1f;
    [SerializeField] private float holdTime = 0.1f;
    [SerializeField] private float recoverTime = 0.5f;
    [SerializeField] private float windupPull = 0.3f;
    [SerializeField] private float leadAmount = 1f;

    private Transform player;
    private PlayerController playerController;
    private Rigidbody playerRigidbody;
    private Vector3 restHeadLocal;
    private Vector3 restLookLocal;
    private float reach;
    private float cooldownTimer;
    private bool attacking;
    private bool strikeLocked;
    private Vector3 strikeDirection;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        playerController = player.GetComponent<PlayerController>();
        playerRigidbody = player.GetComponent<Rigidbody>();

        restHeadLocal = transform.InverseTransformPoint(head.position);
        restLookLocal = transform.InverseTransformPoint(head.position + head.TransformDirection(faceAxis) * 2f);
        strikeTarget.position = head.position;
        lookTarget.position = transform.TransformPoint(restLookLocal);

        reach = StemLength();
        rig.weight = 0f;
    }

    void Update()
    {
        cooldownTimer -= Time.deltaTime;

        Vector3 aim = AimPoint();
        Vector3 restHead = transform.TransformPoint(restHeadLocal);
        bool sees = CanSeePlayer(aim, restHead);
        float smooth = 1f - Mathf.Exp(-followSpeed * Time.deltaTime);

        float rigTarget = sees || attacking ? 1f : 0f;
        rig.weight = Mathf.MoveTowards(rig.weight, rigTarget, Time.deltaTime / rigBlendTime);

        if (attacking)
        {
            if (!strikeLocked)
            {
                strikeDirection = (aim - restHead).normalized;
            }
            lookTarget.position = strikeTarget.position + strikeDirection * reach;
            return;
        }

        Vector3 lookGoal = sees ? aim : transform.TransformPoint(restLookLocal);
        lookTarget.position = Vector3.Lerp(lookTarget.position, lookGoal, smooth * 2f);

        Vector3 idleGoal = sees ? ClampReach(Vector3.Lerp(restHead, aim, lean)) : restHead;
        strikeTarget.position = Vector3.Lerp(strikeTarget.position, idleGoal, smooth);

        if (sees && cooldownTimer <= 0f && Vector3.Distance(stemRoot.position, aim) <= reach)
        {
            StartCoroutine(Attack());
        }
    }

    bool CanSeePlayer(Vector3 aim, Vector3 restHead)
    {
        if (Vector3.Distance(restHead, aim) > detectRange || (ignoreDisguised && playerController.GetIsDisguised()) || PlayerDead())
        {
            return false;
        }
        return sightBlockers == 0 || !Physics.Linecast(head.position, aim, sightBlockers);
    }

    IEnumerator Attack()
    {
        attacking = true;
        strikeLocked = false;
        Vector3 restHead = transform.TransformPoint(restHeadLocal);

        Vector3 coil = ClampReach(restHead + (restHead - AimPoint()).normalized * reach * windupPull);
        yield return MoveTarget(strikeTarget.position, coil, windupTime, true);

        Vector3 lead = playerRigidbody != null ? playerRigidbody.velocity * strikeTime * leadAmount : Vector3.zero;
        Vector3 hit = ClampReach(AimPoint() + lead);
        strikeDirection = (hit - coil).normalized;
        strikeLocked = true;
        yield return MoveTarget(coil, hit, strikeTime, false);

        if (!PlayerDead() && Vector3.Distance(hit, AimPoint()) <= hitRadius)
        {
            playerController.DamagePlayer(damage);
        }
        yield return new WaitForSeconds(holdTime);

        yield return MoveTarget(hit, restHead, recoverTime, true);
        cooldownTimer = cooldown;
        attacking = false;
    }


    IEnumerator MoveTarget(Vector3 from, Vector3 to, float time, bool eased)
    {
        for (float t = 0; t < 1; t += Time.deltaTime / time)
        {
            strikeTarget.position = Vector3.Lerp(from, to, eased ? Mathf.SmoothStep(0, 1, t) : t * t);
            yield return null;
        }
        strikeTarget.position = to;
    }

    Vector3 AimPoint()
    {
        return player.position + Vector3.up * aimHeight;
    }

    bool PlayerDead()
    {
        return playerController.GetHealthValues()[0] <= 0;
    }

    Vector3 ClampReach(Vector3 point)
    {
        return stemRoot.position + Vector3.ClampMagnitude(point - stemRoot.position, reach * 0.98f);
    }

    float StemLength()
    {
        float length = 0f;
        for (Transform t = head; t != stemRoot && t.parent != null; t = t.parent)
        {
            length += Vector3.Distance(t.position, t.parent.position);
        }
        return length;
    }
}
