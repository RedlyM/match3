using UnityEngine;
using UnityEngine.UI;

using TMPro;

namespace MatchThree.Menu
{
    public class MenuView : MonoBehaviour
    {
        [SerializeField]
        private Button _play;

        [SerializeField]
        private TMP_Text _gridSize;

        [SerializeField]
        private Button _increaseRows;

        [SerializeField]
        private Button _decreaseRows;

        [SerializeField]
        private Button _increaseColumns;

        [SerializeField]
        private Button _decreaseColumns;

        public Button Play => _play;

        public TMP_Text GridSize => _gridSize;

        public Button IncreaseRows => _increaseRows;

        public Button DecreaseRows => _decreaseRows;

        public Button IncreaseColumns => _increaseColumns;

        public Button DecreaseColumns => _decreaseColumns;
    }
}