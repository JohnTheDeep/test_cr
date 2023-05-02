using Blazor.Shared.Models;

namespace Blazor.Server.Helper
{
    public static class DistinctHelper
    {
        public static Dictionary<int, Wallet> GetDistinctValuesAsync(Dictionary<int, Wallet> compareFrom, Dictionary<int, Wallet> compareTo)
        {
            var result = compareTo
                .ExceptBy(compareFrom.Select(from => from.Key), key => key.Key)
                .ToDictionary(el => el.Key, el => el.Value);
            return result;
        }
    }
}
