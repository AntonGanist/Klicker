using UnityEngine;

public class VisualDialog : MonoBehaviour
{
    [SerializeField] private Animator _animator;
    private RuntimeAnimatorController _animatorController;

    public void Initialize(RuntimeAnimatorController animatorController)
    {
        _animatorController = animatorController;
        _animator.runtimeAnimatorController = _animatorController;
    }

    public void PlayAnimationForEnemy(int enemyNumber, int justNumber)
    {
        _animator.Rebind();
        _animator.Update(0f);
        if(justNumber == 0) 
            _animator.SetInteger("do", enemyNumber);
        else
            _animator.SetInteger("od", justNumber);
    }
}