using UnityEngine;

namespace UI
{
    public class Bar : MonoBehaviour
    {
        private bool _isFull = true;
        private bool _isEmpty = false;
        private bool _isIncreasing;
        private float _actualSize = 1;
        private float _increasePerSecond;
        private float _decreasePerSecond;
        private bool _isDecreasing;

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
                if (_actualSize < 0)
                {
                    _actualSize = 0;
                    BarObject.localScale = new Vector3(_actualSize, BarObject.localScale.y);
                }
                if (_actualSize <= 0)
                {
                    _isEmpty = true;
                }
                else
                {
                    _isEmpty = false;
                }
            }
            else
            {
                if (_actualSize > 1)
                {
                    _actualSize = 1;
                    BarObject.localScale = new Vector3(_actualSize, BarObject.localScale.y);
                }
                _isIncreasing = false;
                _isFull = true;
            }
            if (_isEmpty)
            {
                _isIncreasing = true;
                _isDecreasing = false;
            }
            if (!_isFull && _isIncreasing && !_isDecreasing)
            {
                IncreaseBar();
            }
            else if (!_isEmpty && _isDecreasing && !_isIncreasing)
            {
                DecreaseBar();
            }

            
        }

        private void IncreaseBar()
        {
            _actualSize += _increasePerSecond * Time.deltaTime;
            BarObject.localScale = new Vector3(_actualSize, BarObject.localScale.y);
        }

        private void DecreaseBar()
        {
            _actualSize -= _decreasePerSecond * Time.deltaTime;
            BarObject.localScale = new Vector3(_actualSize, BarObject.localScale.y);
        }

        public void SetIncreasingDuration(float duration)
        {
            _increasePerSecond = 1 / duration;
        }

        public void SetDecreasingDuration(float duration)
        {
            _decreasePerSecond = 1 / duration;
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

        public bool IsDecreasing
        {
            get => _isDecreasing;
            set => _isDecreasing = value;
        }

        public float ActualSize
        {
            get => _actualSize;
            set => _actualSize = value;
        }
    }
}
