using UnityEngine;

namespace UI
{
    public class Bar : MonoBehaviour
    {
        private float _duration;
        private bool _isFull = true;
        private bool _isIncreasing;
        private float _actualSize = 1;
        private float _increasePerSecond;

        public RectTransform BarObject;
        
        
        // Start is called before the first frame update
        void Start()
        {
        
        }

        // Update is called once per frame
        void Update()
        {
            if (_actualSize < 1)
            {
                _isFull = false;
                _isIncreasing = true;
            }
            else
            {
                if (_actualSize > 1)
                {
                    _actualSize = 1;
                }
                _isIncreasing = false;
                _isFull = true;
            }

            if (_isIncreasing)
            {
                IncreaseBar();
            }
        }

        private void IncreaseBar()
        {
            _actualSize += _increasePerSecond * Time.deltaTime;
            BarObject.localScale = new Vector3(_actualSize, BarObject.localScale.y);
        }

        public void SetDuration(float duration)
        {
            _duration = duration;
            _increasePerSecond = 1 / duration;
        }

        public bool IsFull
        {
            get => _isFull;
            set => _isFull = value;
        }

        public bool IsIncreasing
        {
            get => _isIncreasing;
            set => _isIncreasing = value;
        }

        public float ActualSize
        {
            get => _actualSize;
            set => _actualSize = value;
        }
    }
}
