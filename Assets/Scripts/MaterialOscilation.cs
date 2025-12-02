using UnityEngine;

public class MaterialOscilation : MonoBehaviour
{
    private Renderer renderComp;
    private Material materialInstancia;
    private Material oldMaterial;
    private Color colorOriginal;
    public float velocidad = 5f; // velocidad de oscilación
    public bool transparent = false;
    [Range(0f, 1f)] public float alphaMin = 0.1f;
    [Range(0f, 1f)] public float alphaMax = 0.8f;

    void Start()
    {
        renderComp = GetComponent<Renderer>();

        if (renderComp != null)
        {
            // Guardamos el material original, usando sharedMaterial para no crear una nueva instancia
            oldMaterial = renderComp.material;

            // Clonamos el material para no modificar el original
            materialInstancia = new Material(oldMaterial); // Usamos el material compartido original
            renderComp.material = materialInstancia; // Asignamos la instancia clonada

            colorOriginal = materialInstancia.color;

            // Asegurar que el material permite transparencia
            materialInstancia.SetFloat("_Mode", 3); // Transparent
            materialInstancia.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
            materialInstancia.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
            materialInstancia.SetInt("_ZWrite", 0);
            materialInstancia.DisableKeyword("_ALPHATEST_ON");
            materialInstancia.EnableKeyword("_ALPHABLEND_ON");
            materialInstancia.DisableKeyword("_ALPHAPREMULTIPLY_ON");
            materialInstancia.renderQueue = 3000;
        }
    }

    void Update()
    {
        if (materialInstancia == null) return;

        float t = (Mathf.Sin(Time.time * velocidad) + 1f) / 2f;

        // Mezclar color original con amarillo
        Color amarillo = Color.yellow;
        Color nuevoColor = Color.Lerp(colorOriginal, amarillo, t);

        // Oscilar la transparencia también
        if (transparent)
        {
            float alpha = Mathf.Lerp(alphaMin, alphaMax, t);
            nuevoColor.a = alpha;
        }

        materialInstancia.color = nuevoColor;
    }

    private void OnDisable()
    {
        // Restaurar el material original de la escena
        renderComp = GetComponent<Renderer>();
        if (renderComp != null)
        {
            renderComp.material = oldMaterial;
        }
    }

    private void OnEnable()
    {
        Start();
    }
}
