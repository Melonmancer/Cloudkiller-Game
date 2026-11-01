using UnityEngine;
using UnityEngine.Serialization;

public class SkyBlend : MonoBehaviour
{
    [System.Serializable]
    public class SkyZone
    {
        public Collider area; 
        [FormerlySerializedAs("zenith")] public Color top = new Color(0.36f, 0.55f, 0.78f);
        public Color horizon = new Color(0.95f, 0.90f, 0.75f);
        [FormerlySerializedAs("cloudSeaShade")] public Color bottom = new Color(0.72f, 0.80f, 0.82f);
        public Color sun = new Color(1f, 0.92f, 0.70f);
    }

    [SerializeField] private Material skyMaterial;
    [SerializeField] private SkyZone[] zones;
    [SerializeField] private float transitionTime = 3f;
    [SerializeField] private bool blendFog = true;
    [SerializeField] private bool updateAmbient = true;
    [SerializeField] private float ambientInterval = 0.5f;

    private static readonly int[] ColourIds =
    {
        Shader.PropertyToID("_TopColour"),
        Shader.PropertyToID("_HorizonColour"),
        Shader.PropertyToID("_BottomColour"),
        Shader.PropertyToID("_SunColour"),
    };

    private Transform player;
    private Material sky;
    private SkyZone current;
    private Color[] from = new Color[4], to = new Color[4], shown = new Color[4];
    private float progress = 1f;
    private float ambientTimer;

    void Start()
    {
        player = GameObject.FindWithTag("Player").transform;
        if (skyMaterial == null) { skyMaterial = RenderSettings.skybox; }

        sky = new Material(skyMaterial);
        RenderSettings.skybox = sky;

        if (zones.Length == 0) { return; }

        current = FindZone() ?? zones[0];
        GetColours(current, shown);
        ApplyColours();
        DynamicGI.UpdateEnvironment();
    }

    void Update()
    {
        if (zones.Length == 0) { return; }

        SkyZone inside = FindZone();
        if (inside != null && inside != current)
        {
            current = inside;
            shown.CopyTo(from, 0);
            GetColours(current, to);
            progress = 0f;
        }
        if (progress >= 1f) { return; }

        progress = Mathf.MoveTowards(progress, 1f, Time.deltaTime / Mathf.Max(transitionTime, 0.01f));
        float t = Mathf.SmoothStep(0f, 1f, progress);
        for (int i = 0; i < shown.Length; i++) { shown[i] = Color.Lerp(from[i], to[i], t); }
        ApplyColours();

        if (updateAmbient)
        {
            ambientTimer -= Time.deltaTime;
            if (ambientTimer <= 0f || progress >= 1f)
            {
                DynamicGI.UpdateEnvironment();
                ambientTimer = ambientInterval;
            }
        }
    }

    SkyZone FindZone()
    {
        if (current != null && Contains(current.area)) { return current; }
        foreach (SkyZone zone in zones)
        {
            if (Contains(zone.area)) { return zone; }
        }
        return null;
    }

    bool Contains(Collider area)
    {
        if (area == null || !area.enabled || !area.gameObject.activeInHierarchy) { return false; }
        return (area.ClosestPoint(player.position) - player.position).sqrMagnitude < 0.0001f;
    }

    void GetColours(SkyZone zone, Color[] colours)
    {
        colours[0] = zone.top;
        colours[1] = zone.horizon;
        colours[2] = zone.bottom;
        colours[3] = zone.sun;
    }

    void ApplyColours()
    {
        for (int i = 0; i < ColourIds.Length; i++) { sky.SetColor(ColourIds[i], shown[i]); }
        if (blendFog) { RenderSettings.fogColor = shown[1]; }
    }

    void OnDestroy()
    {
        if (sky != null)
        {
            RenderSettings.skybox = skyMaterial;
            Destroy(sky);
        }
    }
}
