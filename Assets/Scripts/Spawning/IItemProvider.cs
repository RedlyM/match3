using MatchThree.Core;
using UnityEngine;

namespace MatchThree
{
    public interface IItemProvider<out T> where T : MonoBehaviour
    {
        T GetItem(IObjectIdentifier target);
    }
}