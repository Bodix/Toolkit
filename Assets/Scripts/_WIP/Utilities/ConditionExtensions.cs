namespace _WIP.Utilities
{
    /// <summary>
    /// This is a bool predicate called "Condition".
    /// <br/>Like a Predicate class but without input parameters.
    ///
    /// <para/>Often it's useful to use with "Try-" prefix on methods to reduce non-obviousness.
    ///
    /// <para/>More about bool predicates you can read
    /// <a href="https://en.wikipedia.org/wiki/Predication_(computer_architecture)">here</a> or 
    /// <a href="https://www.sciencedirect.com/topics/computer-science/boolean-predicate">here</a>.
    /// </summary>
    public delegate bool Condition();

    public static class ConditionExtensions
    {
        public static bool IsTrueOrNull(this Condition action)
        {
            return action?.Invoke() ?? true;
        }
        
        public static bool IsFalseOrNull(this Condition action)
        {
            return !action?.Invoke() ?? true;
        }
        
        public static bool IsTrue(this Condition condition)
        {
            return condition?.Invoke() ?? false;
        }
        
        public static bool IsFalse(this Condition action)
        {
            return !action?.Invoke() ?? false;
        }
    }
}