using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_ActionPrompt : MonoBehaviour
{
    // public GameObject buttonVerbMasterGO;

    private enum controllerType { ps4, xbox };
    private controllerType currentControllerType = controllerType.ps4;

    public enum verbTypes { jump, pickup, drop, grabCart, releaseCart, crouch, sprint, checkout };
    public enum buttonTypes { top_button, bottom_button, left_button, right_button,  leftBumper, rightBumber, leftTrigger, rightTrigger, start, select, home };


    // public List<buttonAndVerb> UI_actionPrompts = new List<buttonAndVerb>();

}
