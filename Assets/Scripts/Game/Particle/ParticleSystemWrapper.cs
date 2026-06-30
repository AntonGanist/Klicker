using UnityEngine;

public class ParticleSystemWrapper : MonoBehaviour
{
    [SerializeField] private ParticleSystem _particleSystem;
    [SerializeField] private Vector2 _xPos;
    [SerializeField] private Vector2 _yPos;

    public void PlayAtPosition()
    {
        Vector3 pos = new Vector3(
            Random.Range(_xPos.x, _xPos.y),
            Random.Range(_yPos.x, _yPos.y),
            -10f
        );

        transform.localPosition = pos;
        transform.localScale = new Vector3(2, 2, 2);

        float rotationAngle = GetRotationAngle(pos.x, pos.y);
        transform.rotation = Quaternion.Euler(rotationAngle, 90, 0);
        _particleSystem.Play();
    }
    public void PlayAtPosition(Vector3 pos, bool begien)
    {
        transform.localPosition = pos;
        if(begien == false) _particleSystem.Play();
    }
    private float GetRotationAngle(float x, float y)
    {
        float rand = Random.Range(-10f, 10f);
        if (x > 0 && y > 0) return -45f + rand;
        if (x < 0 && y > 0) return -125f + rand;
        if (x > 0 && y < 0) return 45f + rand;
        return 125f + rand;
    }

}