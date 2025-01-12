using UnityEngine;

public class Shadow3DPlatform : MonoBehaviour
{
    public Light lightSource; // The light source (can be directional, point, or spot)
    public GameObject shadowSurface; // Surface where the shadow will be projected
    public Material shadowMaterial; // Material for the shadow
    public string walkableLayer = "Water"; // Layer for walkable surfaces
    public float shadowOffset = 0.01f; // Small offset to prevent blending with the surface

    private GameObject shadowObject; // GameObject for the shadow
    private Mesh shadowMesh; // Mesh for the shadow
    private MeshCollider shadowCollider; // Collider for the shadow

    void Start()
    {
        InitializeShadow();
    }

    void Update()
    {
        if (lightSource == null || shadowSurface == null)
        {
            Debug.LogError("Light Source or Shadow Surface is not assigned!");
            return;
        }

        GenerateShadowMesh();
    }

    private void InitializeShadow()
    {
        // Create a new GameObject for the shadow
        shadowObject = new GameObject($"{gameObject.name}_Shadow");
        shadowObject.transform.parent = shadowSurface.transform;
        shadowObject.transform.position = shadowSurface.transform.position;
        shadowObject.transform.rotation = shadowSurface.transform.rotation;

        // Set the layer for the shadow object
        int layerIndex = LayerMask.NameToLayer(walkableLayer);
        if (layerIndex == -1)
        {
            Debug.LogError($"Layer '{walkableLayer}' does not exist! Please create it.");
        }
        else
        {
            shadowObject.layer = layerIndex;
        }

        // Add MeshFilter and Renderer for the shadow
        MeshFilter shadowMeshFilter = shadowObject.AddComponent<MeshFilter>();
        shadowMesh = new Mesh();
        shadowMeshFilter.mesh = shadowMesh;

        MeshRenderer shadowRenderer = shadowObject.AddComponent<MeshRenderer>();
        shadowRenderer.material = shadowMaterial != null ? shadowMaterial : new Material(Shader.Find("Unlit/Color"));

        // Add MeshCollider for the shadow
        shadowCollider = shadowObject.AddComponent<MeshCollider>();
        shadowCollider.convex = false; // Ensure accurate mesh representation
        shadowCollider.sharedMesh = shadowMesh;
    }

    private void GenerateShadowMesh()
    {
        MeshFilter objectMeshFilter = GetComponent<MeshFilter>();
        if (objectMeshFilter == null || objectMeshFilter.sharedMesh == null)
        {
            Debug.LogError("Original object is missing a MeshFilter or valid Mesh!");
            return;
        }

        Mesh objectMesh = objectMeshFilter.sharedMesh;
        Vector3[] originalVertices = objectMesh.vertices;
        int[] originalTriangles = objectMesh.triangles;

        if (originalVertices == null || originalVertices.Length == 0)
        {
            Debug.LogError("Object mesh vertices are invalid or empty!");
            return;
        }

        // Prepare arrays for shadow vertices
        Vector3[] shadowVertices = new Vector3[originalVertices.Length];
        Matrix4x4 localToWorld = transform.localToWorldMatrix;

        // Determine the light direction
        Vector3 lightDirection;
        if (lightSource.type == LightType.Directional)
        {
            // Correctly calculate the directional light's world space direction
            lightDirection = lightSource.transform.forward.normalized;
        }
        else
        {
            // For point or spot lights, calculate the direction from the light's position
            lightDirection = (transform.position - lightSource.transform.position).normalized;
        }

        // Project each vertex onto the shadow plane
        Plane shadowPlane = new Plane(shadowSurface.transform.up, shadowSurface.transform.position);
        for (int i = 0; i < originalVertices.Length; i++)
        {
            Vector3 worldVertex = localToWorld.MultiplyPoint3x4(originalVertices[i]);
            Ray projectionRay = new Ray(worldVertex, lightDirection);

            if (shadowPlane.Raycast(projectionRay, out float distance))
            {
                // Apply the shadow offset after projection
                shadowVertices[i] = projectionRay.GetPoint(distance) + shadowSurface.transform.up * shadowOffset;
            }
            else
            {
                // Fallback to a default position if the projection fails
                shadowVertices[i] = worldVertex;
            }
        }

        // Update shadow mesh
        shadowMesh.Clear();
        shadowMesh.vertices = shadowVertices;
        shadowMesh.triangles = originalTriangles;
        shadowMesh.RecalculateNormals();
        shadowMesh.RecalculateBounds();

        // Update shadow collider
        shadowCollider.sharedMesh = null; // Reset collider
        shadowCollider.sharedMesh = shadowMesh; // Apply new shadow mesh
    }
}
