using System;
using UnityEngine;
using System.Collections;

[Serializable]
struct AnimatorDirection
{
    public Animator[] dirAnimator;
}
public class VisualCastMagick : MonoBehaviour
{
    [SerializeField] private Animator _timeStop;
    [SerializeField] private AnimatorDirection[] _animatorDirection = new AnimatorDirection[3];
    [SerializeField] private Animator _mainAnimator;
    [SerializeField] private GameObject[] _icons;
    [SerializeField] private AudioSource _audioSource;
    public event Action<int> OnRealizeMagick; 
    private int number;

    public void Initialize()
    {
        _timeStop.transform.parent = null;
    }
    public void StartCast()
    {
        for (int i = 0; i < _icons.Length; i++)
        {
            _icons[i].SetActive(false);
        }
        OffDir(true);
        _timeStop.SetTrigger("старт");
        number = -1;
    }
    public void EndStartCast()
    {
        _timeStop.SetTrigger("конец");
        RealizeMagick();
    }
    public void EndAnimation(int i)
    {
        number = i;
        _mainAnimator.SetTrigger("старт");
        _icons[i].SetActive(true);

        StartCoroutine(SpellCreate());
    }
    
    public void OffDir(bool o)
    {
        foreach (var dir in _animatorDirection)
        {
            foreach (var anim in dir.dirAnimator)
            {
                anim.gameObject.SetActive(o);
            }
        }
    }
    public void DirVisual(int i, int dir)
    {
        _animatorDirection[i].dirAnimator[dir - 1].SetTrigger("старт");
        _audioSource.Play();
    }
    private IEnumerator SpellCreate()
    {
        yield return new WaitUntil(() => _mainAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
        float animationLength = _mainAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength * 0.5f);
        OffDir(false);
        yield return new WaitUntil(() => _mainAnimator.GetCurrentAnimatorStateInfo(0).normalizedTime < 1f);
        animationLength = _mainAnimator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength * 0.9f);
        EndStartCast();
    }
    private void RealizeMagick()
    {
        OnRealizeMagick?.Invoke(number);
    }
}
