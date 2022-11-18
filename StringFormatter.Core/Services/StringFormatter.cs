using StringFormatter.Core.Interfaces;
using System.Collections.Concurrent;

namespace StringFormatter.Core.Services
{
    public class StringFormatter : IStringFormatter
    {
        public static readonly StringFormatter Shared = new();
        public readonly ConcurrentDictionary<string, Func<object, string>> _cache = new();
        public readonly ConcurrentDictionary<string, Func<object, int, string>> _collectionCache = new();

        public string Format(string template, object target)
        {
            CheckParenthesis(template);

            throw new NotImplementedException();
        }

        private static void CheckParenthesis(string template)
        {
            var count = 0;
            for (int i = 0; i < template.Length; ++i)
            {
                if (template[i] == '{')
                {
                    ++count;
                }
                else if (template[i] == '}')
                {
                    --count;
                }

                if (count > 2 || count < 0)
                {
                    throw new ArgumentException($"Invalid parenthesis count (near position {i})");
                }
            }
        }
    }
}
