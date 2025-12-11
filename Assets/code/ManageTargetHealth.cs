using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ManageTargetHealth : MonoBehaviour
{
    [Header("Enemy / Target Health")]
    [Range(0, 1000)]
    public int health = 100;

    // Flash effect variables
    float hitTimer;
    bool hitFlash;
    float alpha;

    void Start()
    {
        alpha = 0f;
        hitFlash = false;
        hitTimer = 0f;

        // Make sure the sphere starts transparent
        Transform sphere = transform.Find("Sphere");
        if (sphere != null)
        {
            Renderer r = sphere.GetComponent<Renderer>();
            if (r != null)
            {
                Color c = new Color(1f, 0f, 0f, alpha); // red with 0 transparency
                r.material.color = c;
            }
        }
    }

    void Update()
    {
        if (!hitFlash)
            return;

        hitTimer += Time.deltaTime;

        // Fade alpha toward 0 over 1 second
        alpha -= Time.deltaTime;

        if (alpha <= 0f)
        {
            alpha = 0f;
            hitFlash = false;
            hitTimer = 0f;
        }

        // Apply new alpha to sphere
        Transform sphere = transform.Find("Sphere");
        if (sphere != null)
        {
            Renderer r = sphere.GetComponent<Renderer>();
            if (r != null)
            {
                Color c = r.material.color;
                c.a = alpha;
                r.material.color = c;
            }
        }
    }

    // Set health safely
    public void SetHealth(int newHealth)
    {
        health = newHealth;

        if (health <= 0)
        {
            health = 0;
            DestroyTarget();
        }
    }

    // Read current health if needed
    public int GetHealth()
    {
        return health;
    }

    // Called when the sword hits the target
    public void DecreaseHealth(int increment)
    {
        SetHealth(this.health - increment);

        // Start flash effect
        hitFlash = true;
        alpha = 0.5f;   // initial flash opacity
        hitTimer = 0f;
    }

    void DestroyTarget()
    {
        // small delay so it looks like it dies
        Destroy(gameObject, 0.2f);
    }
}
