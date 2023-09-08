using Newtonsoft.Json;
using NUnit.Framework.Constraints;

namespace SFA.DAS.AdminService.Web.UnitTests.Constraints
{
    public class JsonEquivalentConstraint : Constraint
    {
        private readonly object _expected;
        private string _expectedJson;
        private string _actualJson;

        public JsonEquivalentConstraint(object expected)
        {
            _expected = expected;
        }

        public override ConstraintResult ApplyTo<TActual>(TActual actual)
        {
            _expectedJson = JsonConvert.SerializeObject(_expected);
            _actualJson = JsonConvert.SerializeObject(actual);

            bool isSuccess = _expectedJson == _actualJson;

            return new ConstraintResult(this, actual, isSuccess);
        }

        public override string Description
        {
            get { return $"Objects to be JSON equivalent. Expected:<{_expectedJson}>. Actual:<{_actualJson}>."; }
        }
    }

    public static class JsonIs
    {
        public static Constraint EquivalentTo(object expected)
        {
            return new JsonEquivalentConstraint(expected);
        }
    }
}
