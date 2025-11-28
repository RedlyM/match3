using System.Collections.Generic;
using System.Linq;
using System.Threading;

using UnityEngine;

using Cysharp.Threading.Tasks;

using MatchThree.Core;

namespace MatchThree.Spawning
{
    public class PoolWithFactory<T> where T : MonoBehaviour, IObjectIdentifier, IPoolable
    {
        private readonly FactoryByIdentifier<T> _factoryByIdentifier;
        private readonly List<T> _pool;
        private readonly HashSet<T> _busy;

        public PoolWithFactory(FactoryByIdentifier<T> factoryByIdentifier)
        {
            _factoryByIdentifier = factoryByIdentifier;
            _pool = new List<T>(64);
            _busy = new HashSet<T>(64);
        }

        public async UniTask<T> GetAsync(IObjectIdentifier id, CancellationToken token)
        {
            T item = _pool.FirstOrDefault(item => item.Id.Equals(id.Id) && !_busy.Contains(item));

            if (item == null)
            {
                item = await _factoryByIdentifier.CreateAsync(id, token);
                _pool.Add(item);
            }

            _busy.Add(item);

            item.Prepare();
            return item;
        }

        public void ReturnToPool(T item)
        {
            _busy.Remove(item);
            item.Release();
        }
    }
}