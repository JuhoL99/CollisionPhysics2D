using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Particle
{
    public Vector2 p;
    public Vector2 v;
    public Vector2 a;
    public float r;
    public int vIndex;

    public Particle(Vector2 p, Vector2 v, Vector2 a, float r, int vIndex)
    {
        this.p = p;
        this.v = v;
        this.a = a;
        this.r = r;
        this.vIndex = vIndex;
    }
    public float m
    {
        get { return Mathf.PI * r * r; }
    }
    public float right
    {
        get { return p.x + r; }
    }
    public float left
    {
        get { return p.x - r; }
    }
    public float top
    {
        get { return p.y + r; }
    }
    public float bottom
    {
        get { return p.y - r; }
    }
}
