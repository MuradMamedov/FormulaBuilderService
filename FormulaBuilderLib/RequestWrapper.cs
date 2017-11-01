using System.Linq;

namespace FormulaBuilderLib
{
    /// <summary>
    ///     Excpecting Valid Request
    /// </summary>
    public class RequestWrapper
    {
        private Request _request;
        private string _stringForm = null;

        public RequestWrapper(Request request)
        {
            _request = request;
        }

        public string StringForm => _stringForm ?? (_stringForm = GetStringForm(_request.expression));

        private string GetStringForm(object formulaUnit)
        {
            string result;
            if (formulaUnit is decimal)
            {
                return formulaUnit.ToString();
            }
            var r = new ExpressionWrapper(formulaUnit as Expression);
            result = "(" + GetStringForm(r.Operand1.Item);
            switch (r.Operation)
            {
                case Operation.div:
                    result += " / ";
                    break;
                case Operation.mul:
                    result += " * ";
                    break;
                case Operation.minus:
                    result += " - ";
                    break;
                case Operation.plus:
                    result += " + ";
                    break;
            }
            result += GetStringForm(r.Operand2.Item) + ")";
            return result;
        }

        private class ExpressionWrapper
        {
            private Expression _expression;

            public ExpressionWrapper(Expression expression)
            {
                _expression = expression;
            }

            public Operation Operation
            {
                get { return (Operation) _expression.Items.LastOrDefault(item => item.GetType() == typeof(Operation)); }
            }

            public Operand Operand1
            {
                get { return _expression.Items.FirstOrDefault(item => item.GetType() == typeof(Operand)) as Operand; }
            }

            public Operand Operand2
            {
                get { return _expression.Items.LastOrDefault(item => item.GetType() == typeof(Operand)) as Operand; }
            }
        }
    }
}