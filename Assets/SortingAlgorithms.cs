using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class QuickSort
{
    public void Sort(Particle[] particles, int low, int high)
    {
        if(low < high)
        {
            int pivot = Partition(particles, low, high);
            Sort(particles, low, pivot-1);
            Sort(particles, pivot + 1, high);
        }
    }
    private int Partition(Particle[] particles, int low, int high)
    {
        int mid = low + (high - low) / 2;
        float pivotValue = particles[mid].left;
        Swap(particles, mid, high);

        int i = low - 1;
        for (int j = low; j < high; j++)
        {
            if (particles[j].left <= pivotValue)
            {
                i++;
                Swap(particles, i, j);
            }
        }
        Swap(particles,i+1, high);
        return i + 1;
    }
    private void Swap(Particle[] particles, int i, int j)
    {
        Particle t = particles[i];
        particles[i] = particles[j];
        particles[j] = t;
    }
}
public class InsertionSort
{
    public void Sort(Particle[] particles)
    {
        int n = particles.Length;
        for(int i = 1; i < n; i++)
        {
            Particle key = particles[i];
            int j = i- 1;
            while(j >= 0 && particles[j].left > key.left)
            {
                particles[j + 1] = particles[j];
                j--;
            }
            particles[j + 1] = key;
        }
    }
}

