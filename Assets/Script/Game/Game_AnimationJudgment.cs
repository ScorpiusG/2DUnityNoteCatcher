using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Game_AnimationJudgment : MonoBehaviour
{
    public SpriteRenderer spriteRendererJudgment;
    public Animator animatorJudgment;

    public void Despawn()
    {
        gameObject.SetActive(false);
    }
}
