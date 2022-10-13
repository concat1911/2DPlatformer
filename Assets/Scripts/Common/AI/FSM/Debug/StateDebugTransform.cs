using System;
using UnityEngine;
using TMPro;

namespace VD.FSM.Debug
{
    public class StateDebugTransform : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI stateText;
        [SerializeField] private GameObject stateDebugObj;
        [SerializeField] private Vector3 offset;
        
        private RectTransform canvasRect;
        private Camera mainCam;
        private Transform target;

        private void Awake()
        {
            canvasRect = GetComponentInParent<Canvas>().transform as RectTransform;
            mainCam = Camera.main;
            target = stateDebugObj.transform;
        }

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

        private void Update()
        {
            Vector3 targetPosition = target.position + offset;
            Vector2 viewPortPosition = mainCam.WorldToViewportPoint(targetPosition);

            // Calculate screen position
            Vector2 screenPosition = new Vector2(
                ((viewPortPosition.x * canvasRect.sizeDelta.x) - (canvasRect.sizeDelta.x * 0.50f)),
                ((viewPortPosition.y * canvasRect.sizeDelta.y) - (canvasRect.sizeDelta.y * 0.50f)));
                
            stateText.rectTransform.anchoredPosition = screenPosition;
        }
    }
}