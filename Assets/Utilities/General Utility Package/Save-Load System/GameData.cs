using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace GeneralUtility
{
    namespace SaveLoadSystem
    {
        [System.Serializable]
        public class GameData
        {
            public long lastUpdated;

            public SerializableDictionary<string, bool> allMissions;
            public SerializableDictionary<string, bool> allObjectives;
            public Dictionary<string, int> missionScores;
            public SerializableDictionary<string, bool> allResearch;
            public List<string> primaryLoadout, secondaryLoadout, turretLoadout, passivesLoadout;

            public SerializableDictionary<string, bool> allTutorialFlags;

            public int researchPoints;

            public GameData()
            {//Constructor: sets default values for new games and such

                /* DATA THAT NEEDS TO BE SAVED:
                 *
                 *  - Planets
                 *      - Wouldn't the completed missions simply recognize which planets to unlock?
                 *  - Missions
                 *      - Completed Missions
                 *          - List<string> for names?
                 *      - Scores
                 *          - Dictionary<string key, int score value>
                 *          
                 *  - Research
                 *      - Unlocked Research
                 *          - List<string> for research names
                 *      - Primary/Secondary/Turret loadouts
                 *          - List<string> for research names, *with null for empty*?
                 *
                */

                allMissions = new();
                allObjectives = new();
                missionScores = new();
                allResearch = new();
                primaryLoadout = new();
                secondaryLoadout = new();
                turretLoadout = new();
                passivesLoadout = new();
                allTutorialFlags = new();

                researchPoints = 0;
            }

            public int GetPercentageComplete()
            {
                int totalChecks = 0;
                int completedChecks = 0;

                foreach (bool completed in allMissions.Values)
                {
                    totalChecks++;
                    if (completed)
                    {
                        completedChecks++;
                    }
                }
                foreach (bool completed in allObjectives.Values)
                {
                    totalChecks++;
                    if (completed)
                    {
                        completedChecks++;
                    }
                }
                foreach (bool unlocked in allResearch.Values)
                {
                    totalChecks++;
                    if (unlocked)
                    {
                        completedChecks++;
                    }
                }

                if (totalChecks <= 0)
                    return 0;
                return Mathf.FloorToInt(completedChecks / totalChecks);
            }
        }
    }
}