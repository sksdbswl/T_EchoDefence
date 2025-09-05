using UnityEngine;

public class Notepad : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Debug.Log(TestManager.Instance.gameObject.name);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
