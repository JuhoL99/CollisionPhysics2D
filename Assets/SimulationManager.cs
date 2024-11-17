using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using UnityEngine;

public class SimulationManager : MonoBehaviour
{
    [SerializeField] private GameObject visualizerObject;
    [SerializeField] private int particleAmount = 2;
    [SerializeField] private SortingMode sortingMode;
    [SerializeField,Header("velocity min/max")] private Vector2 velocityRange;
    private GameObject[] visualizers;
    private Particle[] particles;
    private QuickSort quickSort = new QuickSort();
    private InsertionSort insertionSort = new InsertionSort();
    private Vector2 boxBounds = new Vector2(-50, 50);
    private int checksPerFrame;

    void Start()
    {
        InitializeParticles();
    }
    void FixedUpdate()
    {
        checksPerFrame = 0;
        MoveParticles();
        SortAndSweep();
        Debug.Log(checksPerFrame);
        
    }
    // change velocity and position of a particle each frame
    private void MoveParticles()
    {
        for (int i = 0; i < particleAmount; i++)
        {
            particles[i].v += particles[i].a * Time.fixedDeltaTime;
            particles[i].p += particles[i].v * Time.fixedDeltaTime;
            HandleBoxCollision(i, true);
            Visualize(i);
        }
        //CollisionDetectionNaive(); //1000 particles around 500000 checks, with sort 6000 checks
    }
    private void SortAndSweep()
    {
        if(sortingMode == SortingMode.InsertionSort)
        {
            insertionSort.Sort(particles);
        }
        else if(sortingMode == SortingMode.QuickSort)
        {
            quickSort.Sort(particles, 0, particleAmount - 1);
        }
        for (int i = 0; i < particleAmount; i++)
        {
            Particle p1 = particles[i];
            for (int j = i + 1; j < particleAmount; j++)
            {
                Particle p2 = particles[j];
                if (p2.left > p1.right)
                    break;
                if (p2.top < p1.bottom || p2.bottom > p1.top)
                    continue;
                if (IntersectionCheck(p1, p2))
                    HandleParticleCollisionsElastic(p1, p2);
            }
        }
    }
    // check for collision with bounding box edges
    private void HandleBoxCollision(int i)
    {
        Particle p = particles[i];
        if (p.right >= boxBounds[1] || p.left <= boxBounds[0])
            p.v.x = -p.v.x;
        if (p.top >= boxBounds[1] || p.bottom <= boxBounds[0])
            p.v.y = -p.v.y;
    }
    // keeps particles inside the box and moves them back the distance they overlap the bounding box
    private void HandleBoxCollision(int i, bool overlapCompensation)
    {
        Particle p = particles[i];
        if (p.right >= boxBounds[1])
        {
            p.v.x = -p.v.x;
            p.p.x = boxBounds[1] - p.r;
        }
        else if(p.left <= boxBounds[0])
        {
            p.v.x = -p.v.x;
            p.p.x = boxBounds[0] + p.r;
        }
        if(p.top >= boxBounds[1])
        {
            p.v.y = -p.v.y;
            p.p.y = boxBounds[1] - p.r;
        }
        else if(p.bottom <= boxBounds[0])
        {
            p.v.y = -p.v.y;
            p.p.y = boxBounds[0] + p.r;
        }
    }
    // check every pair
    private void CollisionDetectionNaive()
    {
        for(int i = 0; i < particleAmount;i++)
        {
            Particle p1 = particles[i];
            for (int j = i+1; j < particleAmount; j++)
            {
                Particle p2 = particles[j];
                HandleParticleCollisionsElastic(p1, p2);
            }
        }
    }
    // using circles, checks if 2 are overlapping
    private bool IntersectionCheck(Particle p1, Particle p2)
    {
        checksPerFrame++;
        return Vector2.Distance(p1.p, p2.p) <= p1.r + p2.r;
    }
    // chances particles velocities on collision
    private void HandleParticleCollisionsElastic(Particle p1, Particle p2)
    {
        if(IntersectionCheck(p1,p2))
        {
            Vector2 p1v = p1.v - (2 * p2.m / (p1.m + p2.m) * Vector2.Dot(p1.v - p2.v, p1.p - p2.p) / Mathf.Pow(Vector2.Distance(p1.p, p2.p),2) * (p1.p - p2.p));
            Vector2 p2v = p2.v - (2 * p1.m / (p1.m + p2.m) * Vector2.Dot(p2.v - p1.v, p2.p - p1.p) / Mathf.Pow(Vector2.Distance(p2.p, p1.p),2) * (p2.p - p1.p));
            p1.v = p1v;
            p2.v = p2v;
            FixOverlap(p1, p2);
        }
    }
    // moves particles by the amount they overlap on a frame to fix them sticking together
    private void FixOverlap(Particle p1, Particle p2)
    {
        Vector2 difference = p1.p - p2.p;
        float distance = difference.magnitude;
        float overlap = (p1.r + p2.r) - distance;

        Vector2 correction = difference.normalized * overlap / 2;

        p1.p += correction;
        p2.p -= correction;

        //proportional to mass maybe more realistic looking
        //p1.p += correction * (p2.m / (p1.m + p2.m));
        //p2.p -= correction * (p1.m / (p1.m + p2.m));
    }
    // move the visualizer object to its particle position
    private void Visualize(int i)
    {
        visualizers[particles[i].vIndex].transform.position = particles[i].p;
    }
    // creates a grid and place particles on it with initial values
    private void InitializeParticles()
    {
        particles = new Particle[particleAmount];
        visualizers = new GameObject[particleAmount];

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
                Vector2 p = new Vector2(x, y);

                Vector2 dir = Random.insideUnitCircle.normalized;
                float magnitude = Random.Range(velocityRange[0], velocityRange[1]);   //default for testing 0.5 5
                Vector2 v = dir * magnitude;

                Vector2 a = Vector2.zero;
                float r = Random.Range(0.2f, 0.8f);
                particles[i] = new Particle(p, v, a, r, i);
                visualizers[i] = Instantiate(visualizerObject);
                visualizers[i].transform.localScale = new Vector3(r * 2, r * 2, 1);
                visualizers[i].GetComponent<SpriteRenderer>().color = RandomColor();

                i++;
            }
        }
    }
    private Color RandomColor()
    {
        return new Color(Random.value, Random.value, Random.value);
    }
}
public enum SortingMode
{
    QuickSort,
    InsertionSort
}
