using UnityEngine;

public class RotationAnimator : MonoBehaviour
{
    public float speed;
    void Update()
    {
        transform.Rotate(new Vector3(0, speed * Time.deltaTime, 0));
    }
}
