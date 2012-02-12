using System;
using System.Reflection;
using System.Collections.Generic;
using System.Collections;

namespace Simulation
{
    public class AdvancedPredicate<T> where T : class
    {
        public AdvancedPredicate()
        {
        }
        public AdvancedPredicate(Dictionary<string, object> initialParameters)
        {
            _parameters = initialParameters;
        }

        private Dictionary<string, object> _parameters = new Dictionary<string, object>();
        public void AddParameter(string parameter, object value)
        {
            _parameters.Add(parameter, value);
        }
        public void RemoveParameter(string parameter) { _parameters.Remove(parameter); }
        public Dictionary<string, object> Parameters { get { return _parameters; } }

        public Predicate<T> Predicate { get { return PredicateFunction; } }

        protected virtual bool PredicateFunction(T input)
        {
            foreach (string parameter in _parameters.Keys)
                if (!input.GetType().GetProperty(parameter).GetValue(input, null).Equals(_parameters[parameter]))
                    return false;
            return true;
        }

        public static implicit operator Predicate<T> (AdvancedPredicate<T> advancedPredicate)
        {
            return advancedPredicate.Predicate;
        }
    }
}
