using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralUtility
{
    namespace SaveLoadSystem
    {
        [CreateAssetMenu(menuName = "Save Slot")]
        public class SaveSlot : ScriptableObject
        {
            [Header("Profile")]
            [SerializeField] private string profileId = "";

            public string GetProfileId()
            {
                return profileId;
            }

            public void SetProfileId(string value)
            {
                profileId = value;
            }
        }
    }
}