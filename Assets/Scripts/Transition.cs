using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Transition : MonoBehaviour
{
    [SerializeField]
    private Image transition;

    public IEnumerator DoIn()
    {
        yield return transition.DOFade(0, 1).SetEase(Ease.Linear).WaitForCompletion();
    }

    public IEnumerator DoOut()
    {
        yield return transition.DOFade(1f, 1).SetEase(Ease.Linear).WaitForCompletion();
    }

}
