using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;

namespace MatchThree
{
    public class PoolWithFactory<T> where T : MonoBehaviour
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
            T result;

            if (_pool.Count == _busy.Count)
            {
                result = await _factoryByIdentifier.CreateAsync(id, token);
            }
            else
            {
                result = _pool.First(item => !_busy.Contains(item));
            }

            _pool.Add(result);
            _busy.Add(result);

            return result;
        }

        public void ReturnToPool(T item)
        {
            _busy.Remove(item);
        }
    }
}