using System.Threading;
using Cysharp.Threading.Tasks;
using MatchThree.Core;
using UnityEngine;
using Object = UnityEngine.Object;

namespace MatchThree
{
    public class FactoryByIdentifier<T> where T : MonoBehaviour
    {
        private readonly IItemProvider<T> _provider;

        public FactoryByIdentifier(IItemProvider<T> provider)
        {
            _provider = provider;
        }

        public async UniTask<T> CreateAsync(IObjectIdentifier id, CancellationToken token)
        {
            var item = _provider.GetItem(id);
            var op = Object.InstantiateAsync(item);
            await op;
            token.ThrowIfCancellationRequested();
            return op.Result[0].GetComponent<T>();
        }
    }
}