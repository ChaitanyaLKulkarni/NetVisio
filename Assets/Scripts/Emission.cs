using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Emission : MonoBehaviour {


    private ParticleSystem PSystem;
    private ParticleCollisionEvent[] CollisionEvents;

    private void Start()
    {
        CollisionEvents = new ParticleCollisionEvent[8];
        PSystem = GetComponent<ParticleSystem>();
    }

    public void OnParticleCollision(GameObject other)
    {
        int collCount = PSystem.GetSafeCollisionEventSize();

        if (collCount > CollisionEvents.Length)
            CollisionEvents = new ParticleCollisionEvent[collCount];

        int eventCount = PSystem.GetCollisionEvents(other, CollisionEvents);

        for (int i = 0; i < eventCount; i++)
        {
            //other.GetComponent<Info>().Collide();
        }
    }
}
