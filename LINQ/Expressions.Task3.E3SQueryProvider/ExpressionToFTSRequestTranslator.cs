using System;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace Expressions.Task3.E3SQueryProvider
{
    public class ExpressionToFtsRequestTranslator : ExpressionVisitor
    {
        readonly StringBuilder _resultStringBuilder;

        public ExpressionToFtsRequestTranslator()
        {
            _resultStringBuilder = new StringBuilder();
        }

        public string Translate(Expression exp)
        {
            Visit(exp);

            return _resultStringBuilder.ToString();
        }

        #region protected methods

        protected override Expression VisitMethodCall(MethodCallExpression node)
        {
            if (node.Method.DeclaringType == typeof(Queryable)
                && node.Method.Name == "Where")
            {
                var predicate = node.Arguments[1];
                Visit(predicate);

                return node;
            }

            if (node.Method.DeclaringType == typeof(string) && (node.Method.Name == "Contains" || node.Method.Name == "StartsWith" || node.Method.Name == "EndsWith" || node.Method.Name == "Equals"))
            {
                var leftBracket = node.Method.Name == "Contains" || node.Method.Name == "EndsWith" ? "(*" : "(";
                var rightBracket = node.Method.Name == "Contains" || node.Method.Name == "StartsWith" ? "*)" : ")";

                var leftNode = node.Object;
                Visit(leftNode);
                _resultStringBuilder.Append(leftBracket);
                var rigthNode = node.Arguments[0];
                Visit(rigthNode);
                _resultStringBuilder.Append(rightBracket);
                return node;
            }

            return base.VisitMethodCall(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            switch (node.NodeType)
            {
                case ExpressionType.Equal:

                    var isRigthOrder = node.Left.NodeType == ExpressionType.MemberAccess;
                    var leftNode = isRigthOrder ? node.Left : node.Right;
                    var rightNode = isRigthOrder ? node.Right : node.Left;

                    Visit(leftNode);
                    _resultStringBuilder.Append("(");
                    Visit(rightNode);
                    _resultStringBuilder.Append(")");
                    break;
                case ExpressionType.AndAlso:
                    Visit(node.Left);
                    _resultStringBuilder.Append('&');
                    Visit(node.Right);
                    break;

                default:
                    throw new NotSupportedException($"Operation '{node.NodeType}' is not supported");
            };

            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            _resultStringBuilder.Append(node.Member.Name).Append(":");

            return base.VisitMember(node);
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            _resultStringBuilder.Append(node.Value);

            return node;
        }

        #endregion
    }
}
