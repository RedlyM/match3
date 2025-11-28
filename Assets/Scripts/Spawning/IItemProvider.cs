using UnityEngine;

using MatchThree.Core;

namespace MatchThree.Spawning
{
    public interface IItemProvider<out T> where T : MonoBehaviour
    {
        T GetItem(IObjectIdentifier target);
    }
}