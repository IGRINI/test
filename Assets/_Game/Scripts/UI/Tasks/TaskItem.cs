using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Tasks
{
    public class TaskItem : MonoBehaviour
    {
        [SerializeField] private TMP_Text _taskNameText;
        [SerializeField] private TMP_Text _counterText;
        [SerializeField] private Image _fillImage;

        private float _targetCount;

        public void Initialize(TaskData data)
        {
            
        }

        public void SetProgress(int currentCount)
        {
            
        }
    }

    public struct TaskData
    {
        
    }
}

