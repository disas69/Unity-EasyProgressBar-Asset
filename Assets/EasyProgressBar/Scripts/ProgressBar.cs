using UnityEngine;
using UnityEngine.UI;

namespace EasyProgressBar
{
    [ExecuteInEditMode]
    [RequireComponent(typeof(Image))]
    public class ProgressBar : MonoBehaviour
    {
        private const string ShaderName = "Unlit/Easy Progress Bar";
        
        private static int _mainTexPropertyID;
        private static int _mainColorPropertyID;
        private static int _startColorPropertyID;
        private static int _endColorPropertyID;
        private static int _backColorPropertyID;
        private static int _gradientPropertyID;
        private static int _roundnessSizePropertyID;
        private static int _borderSizePropertyID;
        private static int _fillAmountPropertyID;
        private static int _sizePropertyID;

        private Image _image;
        private Material _material;
        private bool _useGradient;

        [SerializeField] private Color _startColor = Color.white;
        [SerializeField] private Color _endColor = Color.white;
        [SerializeField] private Color _backColor = Color.black;
        [SerializeField] [Range(0f, 1f)] private float _gradient = 0f;
        [SerializeField] [Range(0f, 1f)] private float _roundness = 0.5f;
        [SerializeField] [Range(0f, 1f)] private float _borderSize = 0.15f;
        [SerializeField] [Range(0f, 1f)] private float _fillAmount = 1f;

        public float Gradient
        {
            get => _gradient;
            set => _gradient = value;
        }
        
        public float BorderSize
        {
            get => _borderSize;
            set => _borderSize = value;
        }
        
        public float Roundness
        {
            get => _roundness;
            set => _roundness = value;
        }
        
        public float FillAmount
        {
            get => _fillAmount;
            set => _fillAmount = value;
        }

        private void Awake()
        {
            _mainTexPropertyID = Shader.PropertyToID("_MainTex");
            _mainColorPropertyID = Shader.PropertyToID("_MainColor");
            _startColorPropertyID = Shader.PropertyToID("_StartColor");
            _endColorPropertyID = Shader.PropertyToID("_EndColor");
            _backColorPropertyID = Shader.PropertyToID("_BackColor");
            _gradientPropertyID = Shader.PropertyToID("_Gradient");
            _roundnessSizePropertyID = Shader.PropertyToID("_Roundness");
            _borderSizePropertyID = Shader.PropertyToID("_BorderSize");
            _fillAmountPropertyID = Shader.PropertyToID("_FillAmount");
            _sizePropertyID = Shader.PropertyToID("_Size");

            _image = GetComponent<Image>();
            _image.material = _material = new Material(Shader.Find(ShaderName));

            UpdateView();
        }

        private void Update()
        {
            UpdateView();
        }

        private void UpdateView()
        {
            if (_image != null && _material != null)
            {
                var texture = _material.GetTexture(_mainTexPropertyID);
                if (texture != null)
                {
                    if (_image.sprite != null && _image.sprite.texture != texture)
                    {
                        _material.SetTexture(_mainTexPropertyID, _image.sprite.texture);
                    }
                    else
                    {
                        _material.SetTexture(_mainTexPropertyID, null);
                    }
                }
            
                _material.SetColor(_mainColorPropertyID, _image.color);
                _material.SetColor(_startColorPropertyID, _startColor);
                _material.SetColor(_endColorPropertyID, _endColor);
                _material.SetColor(_backColorPropertyID, _backColor);
                _material.SetFloat(_gradientPropertyID, _gradient);
                _material.SetFloat(_roundnessSizePropertyID, _roundness);
                _material.SetFloat(_borderSizePropertyID, _borderSize);
                _material.SetFloat(_fillAmountPropertyID, _fillAmount);
            
                var scale = transform.lossyScale;
                var rect = _image.rectTransform.rect;
            
                _material.SetVector(_sizePropertyID, new Vector4(scale.x * rect.width, scale.y * rect.height, 0, 0));
            }
        }

        private void OnDestroy()
        {
            if (_material != null)
            {
                if (Application.isPlaying)
                {
                    Destroy(_material);
                }
                else
                {
                    DestroyImmediate(_material);
                }
            }
        }
    }
}