using System.Collections;
using UnityEngine;

public class Focus : MonoBehaviour
{
    Vector3 _currentFocus;

    void Update()
    {
        _currentFocus = Main.GetInstance.GetCurrentFocus.transform.position - transform.position;

        _currentFocus.y = 0f;

        transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(_currentFocus), 1f);
    }
}