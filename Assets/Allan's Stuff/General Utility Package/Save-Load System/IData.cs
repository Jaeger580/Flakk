using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralUtility
{
    namespace SaveLoadSystem
    {
        public interface IData
        {
            public void LoadData(GameData data);
            public void SaveData(GameData data);
        }
    }
}