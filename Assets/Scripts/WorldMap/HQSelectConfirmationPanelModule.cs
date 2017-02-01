using UnityEngine;
using System.Collections;

public class HQSelectConfirmationPanelModule : MonoBehaviour
{

    public void ConfirmHQPanelHasSlidOut()
    {
        MapController.instance.ConfirmPanelSlideOutEvent();
    }
}
