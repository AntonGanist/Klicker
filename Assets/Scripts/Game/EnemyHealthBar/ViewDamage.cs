using System.Collections;
using TMPro;
using UnityEngine;

public class ViewDamage : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private Animator _animator;

    public void Initialize(Vector3 pos, string damage, AttackType attackType)
    {
        gameObject.SetActive(true);
        transform.localPosition = pos;
        Vector3 scale = Vector3.one * 2.5f;
        if (attackType == AttackType.Critical)
            scale *= 1.5f;
        transform.localScale = scale;
        _text.text = "-" + damage;
        _animator.Rebind();
        _animator.Update(0f);
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        yield return new WaitUntil(() => _animator.GetCurrentAnimatorStateInfo(0).length > 0);

        float animationLength = _animator.GetCurrentAnimatorStateInfo(0).length;
        yield return new WaitForSeconds(animationLength);

        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        StopAllCoroutines();
    }
}