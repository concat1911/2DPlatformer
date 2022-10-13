namespace CE
{
    using UnityEngine;

    public class MaterialFlashJob
    {
        public bool isDone;
        private float duration;
        
        public float timer = 0f;
        public Material mat;
        Color flashColor;
        Color originalColor;

        bool lerpToFinished;
        bool lerpBackFinished;

        public MaterialFlashJob(Material mat2Flash, float duration, Color flashColor)
        {
            timer = 0f;
            mat = mat2Flash;
            this.duration = duration;
            lerpToFinished = false;
            lerpBackFinished = false;
            originalColor = mat2Flash.color;
            this.flashColor = flashColor;
        }

        public bool IsDone => isDone;

        public void PerformFlash(float delta)
        {
            if (isDone) return;

            timer += delta;

            if( timer >= duration * 0.5f )
            {
                if( lerpToFinished ) isDone = true;
                lerpToFinished = true;
                timer = 0f;
            }

            if(lerpToFinished)
            {
                mat.color = Color.Lerp(mat.color, originalColor, timer / (duration * 0.5f));
            }
            else
            {
                mat.color = Color.Lerp(mat.color, flashColor, timer / ( duration * 0.5f ));
            }
        }

        public void PerformPingPong(float delta, float duration)
        {
            timer += delta;

            if (timer >= duration)
            {
                lerpToFinished = !lerpToFinished;
                lerpBackFinished = !lerpBackFinished;
                timer = 0f;
            }

            if( !lerpToFinished && lerpBackFinished )
            {
                mat.color = Color.Lerp(mat.color, flashColor, timer / duration);
            }

            if (!lerpBackFinished && lerpToFinished)
            {
                mat.color = Color.Lerp(mat.color, originalColor, timer / duration);
            }
        }

        public void Reset()
        {
            timer = 0f;
            isDone = false;

            lerpToFinished = false;
            lerpBackFinished = false;

            mat.color = originalColor;
        }
    }
}
