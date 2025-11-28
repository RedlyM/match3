using UnityEngine;

namespace MatchThree.Match
{
    public class MatchView : MonoBehaviour
    {
        [SerializeField]
        private Transform _spawnParent;

        public Transform SpawnParent => _spawnParent;
    }
}