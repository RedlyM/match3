using System.Collections.Generic;
using Match;
using MatchThree.Core;

namespace MatchThree
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
                if (item.Identifier.Id.Equals(target.Id))
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