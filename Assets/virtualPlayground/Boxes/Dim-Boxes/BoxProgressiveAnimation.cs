using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DimBoxes
{
    [RequireComponent(typeof(BoxProgressive))]
    public class BoxProgressiveAnimation : MonoBehaviour
    {
        [SerializeField]
        private float speed = 0.05f;
        private bool coroutineRunning = false;
        private BoxProgressive bp;
        public float Speed { get { return speed; } set { speed = value; } }
        // Start is called before the first frame update
        void Start()
        {
            bp = GetComponent<BoxProgressive>();
            bp.SetProgress(bp.IsEnabled() ? 1 : 0);
        }

        void OnMouseDown()
        {
            if (coroutineRunning) return;
            StartCoroutine(Fade());
        }

        private IEnumerator Fade()
        {
            bool enabledAtStart = bp.IsEnabled();
            bp.IsEnabled(true);
            coroutineRunning = true;
            float t = enabledAtStart ? 1 : 0;
            while (enabledAtStart ? (t > 0) : (t < 1))
            {
                float incr = (enabledAtStart ? -1 : 1) * speed;
                t += incr;
                t = Mathf.Clamp01(t);
                bp.SetProgress(t);
                yield return new WaitForEndOfFrame();
            }
            bp.IsEnabled(!enabledAtStart);
            coroutineRunning = false;
            yield return null;
        }
    }
}
