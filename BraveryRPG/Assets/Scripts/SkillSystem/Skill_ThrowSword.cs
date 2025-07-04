using UnityEngine;

public class Skill_ThrowSword : Skill_Base
{
    private SkillObject_Sword currentSword;
    private float currentThrowPower;

    [Header("Regular Sword Uprgade")]
    [SerializeField] private GameObject regularSwordPrefab;
    [Range(0, 10)]
    [SerializeField] private float regularThrowPower = 3f;

    [Header("Pierce Sword Upgrade")]
    [SerializeField] private GameObject pierceSwordPrefab;
    [Tooltip("How many enemies the sword can pierce.")]
    public int amountToPierce = 2;
    [Range(0, 10)]
    [SerializeField] private float pierceThrowPower = 5f;

    [Header("Spin Sword Upgrade")]
    [SerializeField] private GameObject spinSwordPrefab;
    public int maxDistance = 6;
    public float attacksPerSecond = 4f;
    [Tooltip("How many seconds the sword will spin in its deployed position.")]
    public float maxSpinDuration = 3f;
    [Range(0, 10)]
    [SerializeField] private float spinThrowPower = 5f;

    [Header("Bounce Sword Upgrade")]
    [SerializeField] private GameObject bounceSwordPrefab;
    [Tooltip("Maximum number of times the Bounce Sword upgrade will bounce between enemies to deal damage.")]
    public int bounceCount = 5;
    [Tooltip("Movement speed of the Bounce Sword skill object.")]
    public float bounceSpeed = 18f;
    [Range(0, 10)]
    [SerializeField] private float bounceThrowPower = 5f;

    [Header("Trajectory prediction")]
    [Tooltip("Trajectory prediction dot SkillObject prefab.")]
    [SerializeField] private GameObject predictionDot;
    [SerializeField] private int numberOfDots = 20;
    [Tooltip("Distance between the trajectory dots.")]
    [SerializeField] private float spaceBetweenDots = 0.05f;

    private float swordGravity = 4f;
    private Transform[] dots;

    // Uused when we confirm direction and create the sword object and give it the direction to fly.
    private Vector2 confirmedDirection;

    protected override void Awake()
    {
        base.Awake();
        swordGravity = regularSwordPrefab.GetComponent<Rigidbody2D>().gravityScale;
        dots = GenerateDots();
    }

    public override bool CanUseSkill()
    {
        UpdateThrowPower();

        if (currentSword != null)
        {
            currentSword.EnableSwordFlyBackToPlayer();
            return false;
        }

        return base.CanUseSkill();
    }

    public void ThrowSword()
    {
        GameObject swordPrefab = GetSwordPrefab();

        Debug.Log("Skill_ThrowSword -> ThrowSword() -> Prefab = " + swordPrefab);

        // Don't create the sword right in the middle of the player.
        Vector3 spawnPoint = dots[1].position;
        GameObject newSword = Instantiate(swordPrefab, spawnPoint, Quaternion.identity);

        currentSword = newSword.GetComponent<SkillObject_Sword>();
        currentSword.SetupSword(this, GetThrowVelocity());

        SetSkillOnCooldown();
    }

    private GameObject GetSwordPrefab()
    {
        if (Unlocked(SkillUpgradeType.SwordThrow)) return regularSwordPrefab;

        if (Unlocked(SkillUpgradeType.SwordThrow_Pierce)) return pierceSwordPrefab;

        if (Unlocked(SkillUpgradeType.SwordThrow_Spin)) return spinSwordPrefab;

        if (Unlocked(SkillUpgradeType.SwordThrow_Bounce)) return bounceSwordPrefab;

        Debug.Log("GetSwordPrefab() -> No valid sword upgrade selected!");
        return null;
    }

    private void UpdateThrowPower()
    {
        switch (upgradeType)
        {
            case SkillUpgradeType.SwordThrow:
                currentThrowPower = regularThrowPower;
                break;

            case SkillUpgradeType.SwordThrow_Pierce:
                currentThrowPower = pierceThrowPower;
                break;

            case SkillUpgradeType.SwordThrow_Spin:
                currentThrowPower = spinThrowPower;
                break;

            case SkillUpgradeType.SwordThrow_Bounce:
                currentThrowPower = bounceThrowPower;
                break;

            default:
                Debug.Log($"UpdateThrowPower() -> Upgrade type {upgradeType} not supported!");
                break;
        }
    }

    private Vector2 GetThrowVelocity() => confirmedDirection * (currentThrowPower * 10);

    /// <summary>
    /// Moves the dots to positions along a predicted trajectory parabola by
    /// calculating the effect of gravity over distance from the player.
    /// </summary>
    /// <param name="direction">A position away from the player, such as the mouse cursor coordinates.</param>
    public void PredictTrajectory(Vector2 direction)
    {
        for (int i = 0; i < dots.Length; i++)
        {
            dots[i].position = GetTrajectoryPoint(direction, i * spaceBetweenDots);
        }
    }

    /// <param name="direction">A position away from the player.</param>
    /// <param name="t">Time.</param>
    /// <returns>Predicted trajectory point with factored time as distance traveled.</returns>
    private Vector2 GetTrajectoryPoint(Vector2 direction, float t)
    {
        float scaledThrowPower = currentThrowPower * 10;

        // This gives us the initial velocity — the starting speed and direction of the throw.
        Vector2 initialVelocity = direction * scaledThrowPower;

        // Gravity pulls the sword down over time. The longer it's in the air, the more it drops.
        // Original formula: 0.5f * Physics2D.gravity * swordGravity * (t * t)
        Vector2 gravityEffect = 0.5f * Physics2D.gravity * swordGravity * (t * t);

        // We calculate how far the sword will travel after time 't',
        // by combining the initial throw direction with the gravity pull.
        Vector2 predictedPoint = (initialVelocity * t) + gravityEffect;

        // Transform.root is the root game object, such as the "Player".
        Vector2 playerPosition = transform.root.position;

        return playerPosition + predictedPoint;
    }

    public void ConfirmTrajectory(Vector2 direction) => confirmedDirection = direction;

    public void EnableDots(bool enable)
    {
        foreach (Transform dot in dots)
        {
            dot.gameObject.SetActive(enable);
        }
    }

    private Transform[] GenerateDots()
    {
        Transform[] newDots = new Transform[numberOfDots];

        for (int i = 0; i < numberOfDots; i++)
        {
            newDots[i] = Instantiate(predictionDot, transform.position, Quaternion.identity, transform).transform;
            // The dot is not immediately visible to the player.
            newDots[i].gameObject.SetActive(false);
        }

        return newDots;
    }
}
