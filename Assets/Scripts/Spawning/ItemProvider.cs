using System.Collections.Generic;

using MatchThree.Core;
using MatchThree.Element;

namespace MatchThree.Spawning
{
    public class ItemProvider : IItemProvider<MatchElement>
    {
        private IEnumerable<MatchElement> _fruits;

        public ItemProvider(IEnumerable<MatchElement> fruits)
        {
            _fruits = fruits;
        }

        public MatchElement GetItem(IObjectIdentifier target)
        {
            MatchElement result = null;

            foreach (var item in _fruits)
            {
                if (item.Id.Equals(target.Id))
                {
                    result = item;
                    break;
                }
            }

            if (result == null)
            {
                throw new KeyNotFoundException($"Item by id:'{target.Id}' not found");
            }

            return result;
        }
    }
}