using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class SimulationManagerUnity : MonoBehaviour
{
    [SerializeField] private int particleAmount = 2;
    [SerializeField] private PhysicsMaterial2D pMat;
    [SerializeField] private Sprite sprite;
    [SerializeField, Header("velocity min/max")] private Vector2 velocityRange;
    private Vector2 boxBounds = new Vector2(-50, 50);
    private GameObject[] particles;

    void Start()
    {
        InitializeParticles();
    }
    private void InitializeParticles()
    {
        particles = new GameObject[particleAmount];
        int gridSize = Mathf.CeilToInt(Mathf.Sqrt(particleAmount));
        float boxWidth = boxBounds[1] - boxBounds[0];
        float boxHeight = boxBounds[1] - boxBounds[0];
        float cellWidth = boxWidth / gridSize;
        float cellHeight = boxHeight / gridSize;

        int i = 0;

        for (int row = 0; row < gridSize; row++)
        {
            for (int col = 0; col < gridSize; col++)
            {
                if (i >= particleAmount)
                    break;

                float x = boxBounds[0] + cellWidth * (col + 0.5f);
                float y = boxBounds[0] + cellHeight * (row + 0.5f);

                GameObject particle = new GameObject("Particle_" + i);
                particle.transform.position = new Vector2(x, y); ;

                CircleCollider2D collider = particle.AddComponent<CircleCollider2D>();
                float r = Random.Range(0.2f, 0.8f);
                collider.radius = r;
                collider.sharedMaterial = pMat;

                Rigidbody2D rb = particle.AddComponent<Rigidbody2D>();
                rb.gravityScale = 0;
                rb.mass = Mathf.PI * r * r;
                rb.angularDrag = 0;
                Vector2 dir = Random.insideUnitCircle.normalized;
                float magnitude = Random.Range(velocityRange[0], velocityRange[1]); //default for testing 0.5 5
                rb.velocity = dir * magnitude;
                rb.sharedMaterial = pMat;

                GameObject visualizer = new GameObject("Sprite" + i);
                visualizer.transform.SetParent(particle.transform);
                visualizer.transform.localPosition = Vector3.zero;
                visualizer.transform.localScale = new Vector3(r*2,r*2, 1);
                SpriteRenderer spriteRenderer = visualizer.AddComponent<SpriteRenderer>();
                spriteRenderer.sprite = sprite;
                spriteRenderer.color = RandomColor();

                particles[i] = particle;
                i++;
            }
        }
    }
    private Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }
}
