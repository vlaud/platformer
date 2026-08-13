using System.Linq;
using UnityEngine;

public class WorldMessageManager : MonoBehaviour
{
    [Header("메시지")]
    [SerializeField] private Transform _messages;
    [SerializeField] private TMPro.TMP_Text showText;

    private void Awake()
    {
        var iObjectActions = FindObjectsByType<MonoBehaviour>().OfType<IObjectAction>();
        foreach (var iObjectAction in iObjectActions)
        {
            iObjectAction.GetTextObject(_messages, showText);
        }
        showText.gameObject.SetActive(false);
    }
}
