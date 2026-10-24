using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;


public class GrassScatterer : MonoBehaviour
{
    [SerializeField] private MeshFilter ground;
    [SerializeField] private MeshFilter grassSource;
    [SerializeField] private Material grassMaterial;
    [SerializeField] private Camera targetCamera;

    [SerializeField] private Texture2D densityMask;

    [SerializeField, Min(0f)] private float density = 3f;
    private int seed = 12345;
    private float maximumSlope = 60f;
    [SerializeField] private Vector2 scaleRange = new Vector2(0.8f, 1.2f);

    private float grassSize = 1f;
    private float groundOffset = 0f;

    private float normalAlignment = 0.25f;

    private int maximumInstances = 200000;

    private float chunkSize = 10f;
    [SerializeField, Min(1f)] private float drawDistance = 50f;

    private float windBoundsPadding = 0.5f;

    private const int BatchSize = 500;
    private readonly Plane[] frustumPlanes = new Plane[6];
    private readonly List<Chunk> chunks = new List<Chunk>();
    private Mesh renderedMesh;
    private bool ready;

    private sealed class Chunk
    {
        public Matrix4x4[] matrices;
        public Bounds bounds;
    }

    private sealed class ChunkBuilder
    {
        public readonly List<Matrix4x4> matrices = new List<Matrix4x4>();
        public Bounds bounds;

        public void Add(Matrix4x4 matrix, Bounds instanceBounds)
        {
            if (matrices.Count == 0)
            {
                bounds = instanceBounds;
            }
                
            else
            {
                bounds.Encapsulate(instanceBounds);
            }             
            matrices.Add(matrix);
        }
    }

    private void Start()
    {
        GenerateGrass();
    }

    private void LateUpdate()
    {
        RenderGrass();
    }

    private void GenerateGrass()
    {
        ready = false;
        chunks.Clear();

        Mesh groundMesh = ground.sharedMesh;
        renderedMesh = grassSource.sharedMesh;

        Vector2[] uvs = null;
        if (densityMask != null)
        {
            uvs = groundMesh.uv;
        }

        float cellSize = Mathf.Max(1f, chunkSize);
        float minScale = Mathf.Max(0.001f, Mathf.Min(scaleRange.x, scaleRange.y));
        float maxScale = Mathf.Max(minScale, Mathf.Max(scaleRange.x, scaleRange.y));
        float size = Mathf.Max(0.001f, grassSize);
        float minUpDot = Mathf.Cos(Mathf.Clamp(maximumSlope, 0f, 90f) * Mathf.Deg2Rad);
        int limit = Mathf.Max(1, maximumInstances);
        var random = new System.Random(seed);

        var maskRandom = new System.Random(unchecked(seed ^ 0x5A17B39D));
        var builders = new Dictionary<Vector2Int, ChunkBuilder>();
        Vector3[] vertices = groundMesh.vertices;
        int[] triangles = groundMesh.triangles;
        Matrix4x4 groundToWorld = ground.transform.localToWorldMatrix;
        int total = 0;
        int accepted = 0;

        for (int i = 0; i < vertices.Length; i++)
        {
            vertices[i] = groundToWorld.MultiplyPoint3x4(vertices[i]);
        }
            

        for (int i = 0; i < triangles.Length; i += 3)
        {
            Vector3 a = vertices[triangles[i]];
            Vector3 b = vertices[triangles[i + 1]];
            Vector3 c = vertices[triangles[i + 2]];
            Vector3 cross = Vector3.Cross(b - a, c - a);
            float twiceArea = cross.magnitude;
            if (twiceArea <= 0.000001f)
            {
                continue;
            }
                

            Vector3 normal = cross / twiceArea;
            if (Vector3.Dot(normal, Vector3.up) < minUpDot)
            {
                continue;
            }
                

            double expected = 0.5 * twiceArea * Mathf.Max(0f, density);

            int count = (int)Math.Floor(expected);
            if (random.NextDouble() < expected - count)
            {
                count++;
            }
               

            Vector3 up = Vector3.Slerp(Vector3.up, normal, Mathf.Clamp01(normalAlignment));
            Quaternion tilt = Quaternion.FromToRotation(Vector3.up, up);

            for (int j = 0; j < count; j++)
            {
                float s = Mathf.Sqrt((float)random.NextDouble());
                float r = (float)random.NextDouble();
                Vector3 position = (1f - s) * a + s * (1f - r) * b + s * r * c;
                position += normal * groundOffset;
                float yaw = (float)random.NextDouble() * 360f;
                float scale = Mathf.Lerp(minScale, maxScale, (float)random.NextDouble()) * size;

                float maskRoll = (float)maskRandom.NextDouble();
                if (densityMask != null)
                {
                    Vector2 uv = (1f - s) * uvs[triangles[i]] + s * (1f - r) * uvs[triangles[i + 1]] + s * r * uvs[triangles[i + 2]];
                    float coverage = Mathf.Clamp01(densityMask.GetPixelBilinear(Mathf.Clamp01(uv.x), Mathf.Clamp01(uv.y)).r);

                    if (coverage <= 0f || (coverage < 1f && maskRoll >= coverage))
                    {
                        continue;
                    }
                        
                }

                Quaternion rotation = tilt * Quaternion.Euler(0f, yaw, 0f);
                Matrix4x4 matrix = Matrix4x4.TRS(position, rotation, Vector3.one * scale);
                var key = new Vector2Int(Mathf.FloorToInt(position.x / cellSize), Mathf.FloorToInt(position.z / cellSize));

                if (!builders.TryGetValue(key, out ChunkBuilder builder))
                {
                    builder = new ChunkBuilder();
                    builders.Add(key, builder);
                }

                builder.Add(matrix, TransformBounds(renderedMesh.bounds, matrix));
                accepted++;
            }

            total += count;
        }

        foreach (ChunkBuilder builder in builders.Values)
        {
            Bounds bounds = builder.bounds;
            bounds.Expand(Mathf.Max(0f, windBoundsPadding) * 2f);
            chunks.Add(new Chunk { matrices = builder.matrices.ToArray(), bounds = bounds });
        }

        ready = true;
    }

    private void RenderGrass()
    {
        if (!ready || targetCamera == null || !targetCamera.isActiveAndEnabled || renderedMesh == null || grassMaterial == null)
        {
            return;
        }
            

        int layer = gameObject.layer;
        if ((targetCamera.cullingMask & (1 << layer)) == 0)
        {
            return;
        }
            

        GeometryUtility.CalculateFrustumPlanes(targetCamera, frustumPlanes);
        Vector3 cameraPosition = targetCamera.transform.position;
        float distance = Mathf.Max(0f, drawDistance);
        float distanceSquared = distance * distance;

        foreach (Chunk chunk in chunks)
        {
            if (chunk.bounds.SqrDistance(cameraPosition) > distanceSquared)
            {
                continue;
            }
                
            if (!GeometryUtility.TestPlanesAABB(frustumPlanes, chunk.bounds))
            {
                continue;
            }
               
            var rp = new RenderParams(grassMaterial)
            {
                camera = targetCamera,
                layer = layer,
                worldBounds = chunk.bounds,
                shadowCastingMode = ShadowCastingMode.Off,
                receiveShadows = true,
                lightProbeUsage = LightProbeUsage.Off
            };

            for (int start = 0; start < chunk.matrices.Length; start += BatchSize)
            {
                int count = Mathf.Min(BatchSize, chunk.matrices.Length - start);
                Graphics.RenderMeshInstanced(rp, renderedMesh, 0, chunk.matrices, count, start);
            }
        }
    }

    private static Bounds TransformBounds(Bounds localBounds, Matrix4x4 matrix)
    {
        Vector3 e = localBounds.extents;
        Vector3 x = matrix.MultiplyVector(new Vector3(e.x, 0f, 0f));
        Vector3 y = matrix.MultiplyVector(new Vector3(0f, e.y, 0f));
        Vector3 z = matrix.MultiplyVector(new Vector3(0f, 0f, e.z));
        Vector3 extents = new Vector3(Mathf.Abs(x.x) + Mathf.Abs(y.x) + Mathf.Abs(z.x), Mathf.Abs(x.y) + Mathf.Abs(y.y) + Mathf.Abs(z.y), Mathf.Abs(x.z) + Mathf.Abs(y.z) + Mathf.Abs(z.z));
        return new Bounds(matrix.MultiplyPoint3x4(localBounds.center), extents * 2f);
    }
}
