using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_AnimationJudgment : MonoBehaviour
{
    public SpriteRenderer spriteRendererJudgment;
    public Animator animatorJudgment;
    public ParticleSystem particleSystemJudgment;

    private void OnEnable()
    {
        StartCoroutine("LifeSpan", 0.42f);
    }
    private IEnumerator LifeSpan(float timer)
    {
        yield return new WaitForSeconds(timer);
        Despawn();
    }
    public void Despawn()
    {
        gameObject.SetActive(false);
    }

    private void LateUpdate()
    {
        spriteRendererJudgment.gameObject.layer = gameObject.layer;
        particleSystemJudgment.gameObject.layer = gameObject.layer;
    }
}
