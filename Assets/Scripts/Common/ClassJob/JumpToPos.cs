namespace CE
{
    using UnityEngine;

    public class TransformJumpJob
    {
        public bool isFinish;
        public Vector3 endPos;
        public Vector3 startPos;
        public float distance;
        public float duration;
        public float counter;

        public Transform unit;

        public TransformJumpJob(Transform unit)
        {
            isFinish = false;
            this.unit = unit;
            startPos = unit.position;
            counter = 0f;
        }

        public void PerformJump(float delta)
        {
            counter += delta;

            if (counter >= duration)
            {
                isFinish = true;
                counter = duration;
                unit.gameObject.SetActive(false);

                return;
            }

            Vector3 pos = Vector3.Lerp(startPos, endPos, counter / duration);

            pos.y += Mathf.Lerp(0.0f, distance * 0.5f, Mathf.Sin(counter / duration * 180.0f * Mathf.Deg2Rad));
            unit.position = pos;
        }
    }
}
