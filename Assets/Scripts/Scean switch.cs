using UnityEngine;
using UnityEngine.SceneManagement;
public class Sceanswitch : MonoBehaviour
{
    public void StartGame()
    {
        SceneManager.LoadScene("NewScene"); // zameni sa stvarnim imenom scene
    }
    public void Restart()
    {
        SceneManager.LoadScene("Start screen"); // zameni sa stvarnim imenom scene
    }
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
