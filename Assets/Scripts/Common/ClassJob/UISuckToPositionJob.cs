using UnityEngine;

public class UISuckToPositionJob
{
    public float delay = 0;
    public float duration;
    float timer = 0f;

    public RectTransform rectObject;
    public Vector2 targetPos;

    public bool IsDelayFinish { get; private set; }
    public bool IsFinish { get; private set; }

    public UISuckToPositionJob(float _duration)
    {
        IsFinish = false;
        timer = 0f;

        duration = _duration;
    }

    public void Perform(float delta, Vector2 target)
    {
        if (IsFinish) return;

        timer += delta;

        if (timer >= delay && !IsDelayFinish)
        {
            IsDelayFinish = true;
            timer = 0f;
        }

        if (!IsDelayFinish) return;

        if ( Vector2.Distance(rectObject.position, target) <= 1f )
        {
            IsFinish = true;
            timer = 0f;
        }

        rectObject.position = Vector2.Lerp(rectObject.position, target, timer / duration);
    }

    public void Perform(float delta)
    {
        if (IsFinish) return;

        timer += delta;

        if (timer >= delay && !IsDelayFinish)
        {
            IsDelayFinish = true;
            timer = 0f;
        }

        if (!IsDelayFinish) return;

        if (timer >= duration)
        {
            IsFinish = true;
            timer = 0f;
        }

        rectObject.position = Vector2.Lerp(rectObject.position, targetPos, timer / duration);
    }
}
