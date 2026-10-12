using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace abc.App.Game.Code.Unity.View
{
    public class UpgradesScreen : MonoBehaviour
    {
        [SerializeField] private List<GameObject> toggleTargets;
        [SerializeField] private Button openCloseButton;

        private void Awake()
        {
            foreach (var toggleTarget in toggleTargets)
                toggleTarget.SetActive(false);
            
            openCloseButton.onClick.AddListener(() =>
            {
                foreach (var toggleTarget in toggleTargets)
                    toggleTarget.SetActive(!toggleTarget.activeSelf);
            });
        }
    }
}