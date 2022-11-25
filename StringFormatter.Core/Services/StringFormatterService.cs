using StringFormatter.Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;

namespace StringFormatter.Core.Services
{
    public class StringFormatterService : IStringFormatter
    {
        public static readonly StringFormatterService Shared = new();
        
        private readonly ConcurrentDictionary<Type, Dictionary<string, Func<object, string>>> _cache = new();
        private readonly ConcurrentDictionary<Type, Dictionary<string, Func<object, int, string>>> _collectionCache = new();

        public string Format(string template, object target)
        {
            CheckCurlyBraces(template);
            var sb = new StringBuilder(template);


            for (var i = 0; i < sb.Length; ++i)
            {
                var openCurlyBracesCount = 0;
                while (i < sb.Length && sb[i] == '{')
                {
                    ++openCurlyBracesCount;
                    ++i;
                }

                if (openCurlyBracesCount > 0)
                {
                    if (openCurlyBracesCount > 1)
                    {
                        var countToRemove = openCurlyBracesCount / 2;
                        sb.Remove(i - openCurlyBracesCount, countToRemove);
                        i -= countToRemove;
                    }

                    if (openCurlyBracesCount % 2 == 1)
                    {
                        ReplaceWithValue(sb, ref i, target);
                    }
                }

                var closeCurlyBraces = 0;
                while (i < sb.Length && sb[i] == '}')
                {
                    ++closeCurlyBraces;
                    ++i;
                }

                if (closeCurlyBraces > 0)
                {
                    var countToRemove = closeCurlyBraces / 2;
                    sb.Remove(i - closeCurlyBraces, countToRemove);
                    i -= countToRemove;
                }
            }            

            var result = sb.ToString();
            return result;
        }

        private void ReplaceWithValue(StringBuilder sb, ref int index, object target)
        {
            var startIndex = index;
            var substring = GetSubstring(sb, startIndex, ref index).Trim();
            var posOfSquareBracket = substring.IndexOf('[');

            var valueString = "";
            if (posOfSquareBracket == -1)
            {

            }
            else
            {
                var memberName = substring.Substring(0, posOfSquareBracket).Trim();

                ++posOfSquareBracket;
                var startPos = posOfSquareBracket;
                while (posOfSquareBracket < substring.Length && substring[posOfSquareBracket] != ']')
                {
                    ++posOfSquareBracket;
                }

                if (posOfSquareBracket >= substring.Length)
                {
                    throw new ArgumentException($"Square bracket at {index + startPos} should be closed");
                }

                var collectionIndexString = substring.Substring(startPos, posOfSquareBracket - startPos);
                if (!int.TryParse(collectionIndexString, out var collectionIndex))
                {
                    throw new ArgumentException($"Cannot parse collection index {collectionIndexString}");
                }

                if (_collectionCache.TryGetValue(target.GetType(), out var dict) 
                    && dict.TryGetValue(memberName, out var func))
                {
                    valueString = func(target, collectionIndex);
                }
                else
                {

                }
            }
        }

        private static Func<object, string> GetValueConverter(Type target, string memberName)
        {
            var members = target.GetMembers(BindingFlags.Instance);

            if (!members.Any(m => m.Name == memberName))
            {
                throw new ArgumentException($"Cannot find public member with such name: {memberName}");
            }

            var parameter = Expression.Parameter(target, "p");
            var memberExpression = Expression.PropertyOrField(Expression.TypeAs(parameter, target.GetType()), memberName);
            var toStringExpression = Expression.Call(memberExpression, "ToString", null, null);
            var func = Expression.Lambda<Func<object, string>>(toStringExpression, parameter).Compile();

            return func;
        }

        private static Func<object, int, string> GetCollectionValueConverter(Type target, string memberName)
        {
            var members = target.GetMembers(BindingFlags.Instance);

            if (!members.Any(m => m.Name == memberName))
            {
                throw new ArgumentException($"Cannot find public member with such name: {memberName}");
            }

            var parameter = Expression.Parameter(target, "p");
            var memberExpression = Expression.PropertyOrField(Expression.TypeAs(parameter, target.GetType()), memberName);
            var indexParameter = Expression.Parameter(typeof(int), "i");
            var arrayAccess = Expression.ArrayAccess(memberExpression, indexParameter);
            var toStringExpression = Expression.Call(arrayAccess, "ToString", null, null);
            var func = Expression.Lambda<Func<object, int, string>>(toStringExpression, parameter, indexParameter).Compile();
            
            return func;
        }

        private static string GetSubstring(StringBuilder sb, int startIndex, ref int index) 
        {
            while (sb[index] != '}')
            {
                ++index;
            }

            var count = index - startIndex;
            var chars = new char[index - startIndex];
            sb.CopyTo(startIndex, chars, 0, count);

            var substring = new string(chars);
            return substring;
        }

        private static void CheckCurlyBraces(string template)
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

                if (count < 0)
                {
                    throw new ArgumentException($"Invalid parenthesis count (near position {i})");
                }
            }
        }
    }
}
