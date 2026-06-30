using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using static MutagenConfig;

public class DropMutagen : MonoBehaviour
{
    [SerializeField] private Rigidbody2D _rb;
    [SerializeField] private Button _button;
    [SerializeField] private RectTransform _rectTransform;
    [SerializeField] private float _speed;
    [SerializeField] private Animator _animator;
    [SerializeField] private Image _image;

    private SkillCounter _skillCounter;
    private string _id;
    private Transform _position;
    private int _location;
    public void Initialize(string id, SkillCounter skillCounter, Transform pos, MutagenViewData ViewData,
        int location)
    {
        StartMove(ViewData);
        _id = id;
        _location = location;
        _skillCounter = skillCounter;
        _position = pos;
        _button.onClick.AddListener(CollectMutagen);
        StartCoroutine(Disappearing());
    }
    private void StartMove(MutagenViewData ViewData)
    {
        _rb.bodyType = RigidbodyType2D.Dynamic;
        _image.sprite = ViewData.Sprite;
        transform.localScale = ViewData.Scale;
        Vector3 pos = Vector3.zero;
        _rectTransform.anchoredPosition3D = pos;
        _rb.mass = ViewData.Mass;
    }
    private void CollectMutagen()
    {
        _rb.bodyType = RigidbodyType2D.Kinematic;
        StartCoroutine(MoveToWallet());
    }
    private IEnumerator MoveToWallet()
    {
        while (Vector3.Distance(transform.position, _position.position) > 0.1f)
        {
            transform.position = Vector3.MoveTowards(
                transform.position,
                _position.position,
                _speed * Time.deltaTime
            );
            yield return null;
        }
        _skillCounter.TakeMutagen(_id, _location);
        gameObject.SetActive(false);
    }
    private IEnumerator Disappearing()
    {
        yield return new WaitForSeconds(5);
        _animator.SetTrigger("изчезает");
        yield return new WaitForSeconds(0.5f);
        float animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);
        gameObject.SetActive(false);
    }
    private void OnDisable()
    {
        _button.onClick.RemoveListener(CollectMutagen);
    }

}