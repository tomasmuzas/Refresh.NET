using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Refresh.Components.Migrations
{
    public class FullType
    {
        public IEnumerable<FullType> GenericArguments = new List<FullType>();

        public string Namespace { get; }

        public string ClassName { get; }

        private string FullValue { get;  }

        public FullType(string type)
        {
            FullValue = type;

            var parts = type.Split(".");
            Namespace = string.Join('.', parts.Take(parts.Length - 1));
            
            var className = parts.Last();
            var GenericPartMatch = Regex.Match(className, @"<([\w\.\s,]*)>");

            ClassName = className;

            if (GenericPartMatch.Success)
            {
                ClassName = ClassName.Replace(GenericPartMatch.Value, "");

                GenericArguments = Regex
                    .Split(GenericPartMatch.Groups[1].Value, @",\s?")
                    .Select(a => new FullType(a));
            }
        }

        public static implicit operator FullType(string type)
        {
            return type == null ? null : new FullType(type);
        }

        public static implicit operator string(FullType type)
        {
            return type?.FullValue;
        }

        public static bool operator == (FullType obj, string type)
        {
            if (obj is null)
            {
                return type == null;
            }

            return obj.FullValue == type;
        }

        public static bool operator ==(FullType obj1, FullType obj2)
        {
            return (string) obj1 == (string) obj2;
        }

        public static bool operator !=(FullType obj1, FullType obj2)
        {
            return (string)obj1 != (string)obj2;
        }

        public static bool operator !=(FullType obj, string type)
        {
            return (string)obj != type;
        }
    }
}
