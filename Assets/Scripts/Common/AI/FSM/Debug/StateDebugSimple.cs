using System;
using UnityEngine;
using TMPro;

namespace VD.FSM.Debug
{
    public class StateDebugSimple : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private GameObject stateDebugObj;

        private void Start()
        {
            IStateGetter stateGetter = stateDebugObj.GetComponent<IStateGetter>();

            if (stateGetter != null)
            {
                stateGetter.GetStateController().OnChangeState += state =>
                {
                    stateText.text = state.stateName;
                };
            }
        }
    }
}