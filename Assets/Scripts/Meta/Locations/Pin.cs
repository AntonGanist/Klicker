using TMPro;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

namespace Meta.Locations
{
    public class Pin : MonoBehaviour
    {
        [SerializeField] private Button _button;
        [SerializeField] private Image _image;
        [SerializeField] private TextMeshProUGUI _text;

        [SerializeField] private bool _isBoss;

        [SerializeField] private Sprite _currentLevelSprite;
        [SerializeField] private Sprite _passedLevelSprite;
        [SerializeField] private Sprite _closedLevelSprite;
        //[SerializeField] private Animator _animator;

        [SerializeField] private Color _currentLevelColor;
        [SerializeField] private Color _passedLevelColor;
        [SerializeField] private Color _closedLevelColor;

        public void Initialize(int levelNumber, ProgressState progressState, UnityAction clickCallback)
        {
            _text.text = $"Óð. {levelNumber}";

            if(_isBoss == false)
            {
                _image.sprite = progressState switch
                {
                    ProgressState.Current => _currentLevelSprite,
                    ProgressState.Closed => _closedLevelSprite,
                    ProgressState.Passed => _passedLevelSprite
                };
            }
            else
            {
                _image.color = progressState switch
                {
                    ProgressState.Current => _currentLevelColor,
                    ProgressState.Closed => _closedLevelColor,
                    ProgressState.Passed => _passedLevelColor
                };
            }
            //_animator.SetFloat("speed", 1);

            _button.onClick.AddListener(() => clickCallback?.Invoke());
        }
    }
}