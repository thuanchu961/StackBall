using System.Collections.Generic;
using UnityEngine;

namespace CBGames
{
    public enum IngameState
    {
        Ingame_Playing,
        Ingame_Revive,
        Ingame_GameOver,
        Ingame_CompletedLevel,
    }

    public enum PlayerState
    {
        Player_Prepare,
        Player_Living,
        Player_Died,
        Player_CompletedLevel,
    }


    public enum StackType
    {
        STACK_6_PARTS,
        STACK_8_PARTS,
        STACK_10_PARTS,
        STACK_12_PARTS,
        STACK_16_PARTS,
    }


    public enum ItemType
    {
        COIN,
    }


    public enum RotateDirection
    {
        LEFT,
        RIGHT,
    }

    [System.Serializable]
    public class LevelConfig
    {
        [Header("Level Number Config")]
        [SerializeField] private int minLevel = 0;
        public int MinLevel { get { return minLevel; } }
        [SerializeField] private int maxLevel = 10;
        public int MaxLevel { get { return maxLevel; } }


        [Header("Colors Config")]
        [SerializeField] private Color playerColor = Color.white;
        public Color PlayerColor { get { return playerColor; } }
        [SerializeField] private Color deadlyPartColor = Color.white;
        public Color DeadlyPartColor { get { return deadlyPartColor; } }
        [SerializeField] private Color backgroundTopColor = Color.white;
        public Color BackgroundTopColor { get { return backgroundTopColor; } }
        [SerializeField] private Color backgroundBottomColor = Color.white;
        public Color BackgroundBottomColor { get { return backgroundBottomColor; } }


        [Header("Center Pillar Config")]
        [SerializeField] private float minCenterPillarRotatingSpeed = 30f;
        public float MinCenterPillarRotatingSpeed { get { return minCenterPillarRotatingSpeed; } }
        [SerializeField] private float maxCenterPillarRotatingSpeed = 100f;
        public float MaxCenterPillarRotatingSpeed { get { return maxCenterPillarRotatingSpeed; } }
        [SerializeField] private float minCenterPillarRotatingTime = 30f;
        public float MinCenterPillarRotatingTime { get { return minCenterPillarRotatingTime; } }
        [SerializeField] private float maxCenterPillarRotatingTime = 60f;
        public float MaxCenterPillarRotatingTime { get { return maxCenterPillarRotatingTime; } }


        [Header("Stack Configs")]

        [SerializeField] private List<StackConfig> listStackConfig = new List<StackConfig>();
        public List<StackConfig> ListStackConfig { get { return listStackConfig; } }

    }


    [System.Serializable]
    public class StackConfig
    {
        [Space(10)]
        [SerializeField] private int minStackNumber = 20;
        public int MinStackNumber { get { return minStackNumber; } }
        [SerializeField] private int maxStackNumber = 20;
        public int MaxStackNumber { get { return maxStackNumber; } }
        [SerializeField] private StackType stackType = StackType.STACK_8_PARTS;
        public StackType StackType { get { return stackType; } }
        [SerializeField] private Color stackColor = Color.white;
        public Color StackColor { get { return stackColor; } }

        [Space(10)]
        [SerializeField] [Range(0f, 360f)] private float firstStackAngle = 0f;
        public float FirstStackAngle { get { return firstStackAngle; } }
        [SerializeField] private int rotationChangeAmount = 2;
        public int RotationChangeAmount { get { return rotationChangeAmount; } }


        [Space(10)]
        [SerializeField] private List<int> listIndexOfDeadlyPart = new List<int>();
        public List<int> ListIndexOfDeadlyPart { get { return listIndexOfDeadlyPart; } }

    }


    public class StackData
    {
        public int StackNumber { get; private set; }
        public void SetStackNumber(int stackNumber)
        {
            StackNumber = stackNumber;
        }


        public StackType StackType { get; private set; }
        public void SetStackType(StackType stackType)
        {
            StackType = stackType;
        }

        public Color StackColor { get; private set; }
        public void SetStackColor(Color stackColor)
        {
            StackColor = stackColor;
        }

        public List<int> ListIndexOfDeadlyPart { get; private set; }
        public void SetListIndexOfDeadlyPart(List<int> listIndexOfDeadlyPart)
        {
            ListIndexOfDeadlyPart = listIndexOfDeadlyPart;
        }


        public float StackAngle { get; private set; }
        public void SetStackAngle(float stackAngle)
        {
            StackAngle = stackAngle;
        }


        public float RotationChangeAmount { get; private set; }
        public void SetRotationChangeAmount(float rotationChangeAmount)
        {
            RotationChangeAmount = rotationChangeAmount;
        }
    }


    public class CenterPillarData
    {
        public float MinRotatingSpeed { get; private set; }
        public void SetMinRotatingSpeed(float minRotatingSpeed)
        {
            MinRotatingSpeed = minRotatingSpeed;
        }

        public float MaxRotatingSpeed { get; private set; }
        public void SetMaxRotatingSpeed(float maxRotatingSpeed)
        {
            MaxRotatingSpeed = maxRotatingSpeed;
        }

        public float MinRotatingTime { get; private set; }
        public void SetMinRotatingTime(float minRotatingTime)
        {
            MinRotatingTime = minRotatingTime;
        }

        public float MaxRotatingTime { get; private set; }
        public void SetMaxRotatingTime(float maxRotatingTime)
        {
            MaxRotatingTime = maxRotatingTime;
        }
    }
}
