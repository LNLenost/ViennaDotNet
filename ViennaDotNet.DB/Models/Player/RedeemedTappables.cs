using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ViennaDotNet.Common.Utils;

namespace ViennaDotNet.DB.Models.Player
{
    public sealed class RedeemedTappables
    {
        private Dictionary<string, long> tappables = new();

        public RedeemedTappables()
        {
            // empty
        }

        public bool isRedeemed(string id)
        {
            return tappables.ContainsKey(id);
        }

        public void add(string id, long expiresAt)
        {
            tappables[id] = expiresAt;
        }

        public void prune(long currentTime)
        {
            tappables.RemoveIf(entry => entry.Value < currentTime);
        }
    }
}
