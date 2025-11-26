using MatchThree;
using UnityEngine;

namespace MatchThree.Core
{
    [System.Serializable]
    public class Identifier : IObjectIdentifier
    {
        public string Id => _id;

        [SerializeField] private string _id;

        public Identifier(string id)
        {
            _id = id;
        }
    }
}