namespace Chloe
{
    /// <summary>
    /// Usage: Case.When(a.Gender == Gender.Male).Then("Male").When(a.Gender == Gender.Female).Then("Female").Else(null)
    /// </summary>
    public class Case
    {
        bool _condition;
        internal Case(bool condition)
        {
            this._condition = condition;
        }
        public static Case When(bool condition)
        {
            return new Case(condition);
        }
        public Then<TResult> Then<TResult>(TResult result)
        {
            return new Then<TResult>(this._condition, result, null);
        }
    }
    public class Case<TResult>
    {
        bool _condition;
        Then<TResult> _prevCase;
        internal Case(bool condition, Then<TResult> prevCase)
        {
            this._condition = condition;
            this._prevCase = prevCase;
        }
        public Then<TResult> Then(TResult result)
        {
            Then<TResult> ret = new Then<TResult>(this._condition, result, this._prevCase);
            return ret;
        }
    }
    public class Then<TResult>
    {
        bool _condition;
        TResult _result;
        Then<TResult> _prevCase;

        internal Then(bool condition, TResult result, Then<TResult> prevCase)
        {
            this._condition = condition;
            this._result = result;
            this._prevCase = prevCase;
        }

        public Case<TResult> When(bool condition)
        {
            return new Case<TResult>(condition, this);
        }
        public TResult Else(TResult result)
        {
            TResult ret;
            if (this.IsMap(out ret))
            {
                return ret;
            }

            return result;
        }

        bool IsMap(out TResult result)
        {
            result = default(TResult);
            if (this._prevCase != null)
            {
                if (this._prevCase.IsMap(out result))
                {
                    return true;
                }
            }

            if (this._condition)
            {
                result = this._result;
                return true;
            }

            return false;
        }
    }
}
