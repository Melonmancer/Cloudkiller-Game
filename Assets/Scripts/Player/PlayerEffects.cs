using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerEffects : MonoBehaviour
{
    [SerializeField] private Transform character;

    [SerializeField] private Material deathMaterial; 
    [SerializeField] private Material respawnMaterial;

    [SerializeField] private ParticleSystem deathFlames; 
    [SerializeField] private ParticleSystem embers; 
    [SerializeField] private ParticleSystem respawnBurst;

    [SerializeField] private Renderer fireWall;

    [SerializeField] private float deathDuration = 1.6f; 
    [SerializeField] private float respawnDuration = 0.9f; 
    [SerializeField] private float fireWallHeight = 0.6f;

    private const float EdgeStart = -0.15f; 
    private const float EdgeEnd = 1.05f;

    private readonly List<Renderer> renderers = new List<Renderer>();
    private readonly List<Material[]> normal = new List<Material[]>(); 
    private readonly List<Material[]> death = new List<Material[]>(); 
    private readonly List<Material[]> respawn = new List<Material[]>();
    
    private Material deathCopy; 
    private Material respawnCopy; 
    private Material wallCopy;
    
    private bool isDead;

    private float bottom; 
    private float top; 
    private float wallFeet;
    private float flameRate; 
    private float emberRate;
    private Coroutine running;

    void Awake()
    {
        deathCopy = new Material(deathMaterial);
        respawnCopy = new Material(respawnMaterial);

        var bounds = new Bounds(character.position, Vector3.zero);
        foreach (var r in character.GetComponentsInChildren<Renderer>(true))
        {
            if (r is ParticleSystemRenderer || r == fireWall) 
            {
                continue;
            }
            renderers.Add(r);
            normal.Add(r.sharedMaterials);
            death.Add(Filled(r.sharedMaterials.Length, deathCopy));
            respawn.Add(Filled(r.sharedMaterials.Length, respawnCopy));
            if (r.gameObject.activeInHierarchy) 
            {
                bounds.Encapsulate(r.bounds);
            }
        }
        bottom = bounds.min.y - character.position.y;
        top = bounds.max.y - character.position.y;
        wallFeet = fireWall.transform.localPosition.y;
        wallCopy = fireWall.material;
        fireWall.enabled = false;
        flameRate = deathFlames.emission.rateOverTime.constant;
        emberRate = embers.emission.rateOverTime.constant;
    }

    static Material[] Filled(int slots, Material material)
    {
        var materials = new Material[slots];
        for (int i = 0; i < slots; i++) 
        {
            materials[i] = material;
        }
        return materials;
    }

    public void Die()
    {
        if (isDead)
        {
            return;
        }
        isDead = true;
        Play(DeathRoutine());
    }

    public void Respawn()
    {
        isDead = false;
        Play(RespawnRoutine());
    }

    void Play(IEnumerator routine)
    {
        if (running != null) 
        {
            StopCoroutine(running);
        }
        running = StartCoroutine(routine);
    }

    IEnumerator DeathRoutine()
    {
        Show(death);
        deathFlames.Play();
        embers.Play();
        for (float t = 0; t < 1; t += Time.deltaTime / deathDuration)
        {
            Set(deathCopy, "_Dissolve", Mathf.Lerp(EdgeStart, EdgeEnd, t));
            var flames = deathFlames.emission;
            flames.rateOverTime = flameRate * Mathf.Clamp01((1 - t) * 1.6f);
            var sparks = embers.emission;
            sparks.rateOverTime = emberRate * (1 - t);
            yield return null;
        }
        deathFlames.Stop();
        embers.Stop();
        foreach (var r in renderers) 
        {
            r.enabled = false;
        }
    }

    IEnumerator RespawnRoutine()
    {
        deathFlames.Stop();
        embers.Stop();
        Show(respawn);
        respawnBurst.Play();
        embers.Emit(40);
        fireWall.enabled = true;
        var wall = fireWall.transform;
        for (float t = 0; t < 1; t += Time.deltaTime / respawnDuration)
        {
            Set(respawnCopy, "_Reveal", Mathf.Lerp(EdgeStart, EdgeEnd, Mathf.Clamp01((t - 0.1f) / 0.75f)));
            float height = Mathf.Max(fireWallHeight * Mathf.SmoothStep(0, 1, t / 0.2f), 0.001f);
            wall.localScale = new Vector3(wall.localScale.x, height, wall.localScale.z);
            wall.localPosition = new Vector3(wall.localPosition.x, wallFeet + height, wall.localPosition.z);
            wallCopy.SetFloat("_Intensity", 1 - Mathf.Clamp01((t - 0.6f) / 0.4f));
            yield return null;
        }
        fireWall.enabled = false;
        Show(normal);
    }

    void Show(List<Material[]> materials)
    {
        for (int i = 0; i < renderers.Count; i++)
        {
            renderers[i].sharedMaterials = materials[i];
            renderers[i].enabled = true;
        }
    }

    void Set(Material material, string property, float value)
    {
        float y = character.position.y;
        material.SetFloat(property, value);
        material.SetVector("_HeightRange", new Vector4(y + bottom, y + top));
    }
}
