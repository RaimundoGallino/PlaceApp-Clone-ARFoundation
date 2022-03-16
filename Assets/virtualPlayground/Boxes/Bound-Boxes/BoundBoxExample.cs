using UnityEngine;
using UnityEngine.UI;

namespace DimBoxes
{
    [RequireComponent(typeof(BoundBox))]
    public class BoundBoxExample : MonoBehaviour
    {
        private BoundBox boundBox;


        // Start is called before the first frame update
        void Start()
        {
            boundBox = GetComponent<BoundBox>();
        }

        // Update is called once per frame
        public void EnableLines(bool val)
        {
            boundBox.line_renderer = val;
            boundBox.Init();
        }

        public void EnableWires(bool val)
        {
            boundBox.wire_renderer = val;
            boundBox.Init();
        }
        public void SetLineWidth(Slider widthSlider)
        {
            boundBox.lineWidth = widthSlider.value;
            boundBox.Init();
        }
        public void SetNumCapVertices(Slider numCapVerticesSlider)
        {
            boundBox.numCapVertices = (int)numCapVerticesSlider.value;
            boundBox.Init();
        }
    }
}