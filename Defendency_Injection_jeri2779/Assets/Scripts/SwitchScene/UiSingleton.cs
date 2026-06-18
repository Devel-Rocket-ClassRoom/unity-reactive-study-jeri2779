using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;
using UnityEngine;
using VContainer;

public class UiSingleton : MonoBehaviour
{
    [SerializeField] TMP_Text m_InfoText;

    [SerializeField] Button m_addButton;
    [SerializeField] Button m_sceneButton;

    [SerializeField] string m_targetSceneName;

    private GlobalCounterService m_Counter;

    [Inject]
    public void Construct(GlobalCounterService counter)
    {
         
        m_Counter = counter;
    }

    public void Start()
    {
        m_addButton.onClick.AddListener(OnClickAdd);
        m_sceneButton.onClick.AddListener(OnClickScene);

    }

    public void OnClickAdd()
    {
        m_Counter.Add(1);
        m_InfoText.text = $"{m_Counter.Id} - {m_Counter.Count}";
    }
    public void OnClickScene()
    {
        SceneManager.LoadScene(m_targetSceneName);
    }
}