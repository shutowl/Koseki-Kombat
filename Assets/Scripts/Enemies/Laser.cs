using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using VolumetricLines;

public class Laser : MonoBehaviour
{
    private VolumetricLineBehavior line;
    private EdgeCollider2D hurtbox;
    public float lifeTime = 2f;
    private float lifeTimeTimer = 0f;
    public float width = 3f;
    public float delay = 0f;
    private float delayTime = 0f;
    public Vector3 startPos;
    public Vector3 endPos;
    public bool explode = false;    //creates a fountain of bullets at contact with ground
    public bool indicator = false;
    bool soundPlayed = false;
    Color color = Color.red;

    void Start()
    {
        line = GetComponent<VolumetricLineBehavior>();
        hurtbox = GetComponent<EdgeCollider2D>();
        line.SetStartAndEndPoints(startPos, endPos);
        hurtbox.SetPoints(new List<Vector2>() { startPos, endPos });

        lifeTimeTimer = lifeTime;
        delayTime = delay;
        soundPlayed = false;

        if (indicator)
        {
            line.LineColor = new Color(0f, 0.5f, 0.5f, 1f);
            line.LightSaberFactor = 0.9f;
            line.LineWidth = 0.5f;
            hurtbox.enabled = false;
        }
        else
        {
            line.LineColor = color;
            hurtbox.enabled = true;
        }

        if (delayTime > 0)
        {
            line.LineWidth = 0;
            hurtbox.edgeRadius = 0;
            hurtbox.enabled = false;
        }
    }

    void Update()
    {
        if (delayTime > 0)
        {
            delayTime -= Time.deltaTime;
            line.LineWidth = 0;
            hurtbox.enabled = false;
        }
        else
        {
            lifeTimeTimer -= Time.deltaTime;

            if (indicator)
            {
                line.LineColor = new Color(0f, 0.5f, 0.5f, 1f);
                line.LightSaberFactor = 0.9f;
                line.LineWidth = 0.5f;
            }
            else
            {
                hurtbox.enabled = true;
                line.LineWidth = (1 - Mathf.Pow(1 - lifeTimeTimer / lifeTime, 5)) * width;
                hurtbox.edgeRadius = (1 - Mathf.Pow(1 - lifeTimeTimer / lifeTime, 5)) * width * 0.15f;
            }

            line.SetStartAndEndPoints(startPos, endPos);

            if (lifeTimeTimer <= 0)
            {
                Destroy(this.gameObject);
            }

            if (!soundPlayed && !indicator)
            {
                AudioManager.Instance.Play("Laser1");
                soundPlayed = true;
            }
        }
    }

    public void SetPositions(Vector3 start, Vector3 end)
    {
        startPos = start;
        endPos = end;
    }

    public void SetLifeTime(float lifeTime)
    {
        this.lifeTime = lifeTime;
    }

    public void SetWidth(float width)
    {
        this.width = width;
    }

    public void SetLightFactor(float light)
    {
        line.LightSaberFactor = light;
    }

    public void SetColor(Color color)
    {
        this.color = color;
    }

}
