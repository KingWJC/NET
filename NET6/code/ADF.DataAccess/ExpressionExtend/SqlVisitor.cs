using System.Linq.Expressions;

namespace ADF.DataAccess
{
    /// <summary>
    /// 解读表达式目录树
    /// </summary>
    public class SqlVisitor : ExpressionVisitor
    {
        private Stack<string> _ConditionStack = new Stack<string>();

        public override Expression? Visit(Expression? node)
        {
            Console.WriteLine($"Visit入口：{node.NodeType} {node.Type} {node.ToString()}");

            return base.Visit(node);
        }

        protected override Expression VisitBinary(BinaryExpression node)
        {
            Console.WriteLine($"VisitBinary：{node.NodeType} {node.Type} {node.ToString()}");

            // 从右往左解析
            _ConditionStack.Push(" ) ");
            this.Visit(node.Right);
            _ConditionStack.Push(node.NodeType.ToSqlOperator());
            this.Visit(node.Left);
            _ConditionStack.Push(" ( ");
            return node;
        }

        protected override Expression VisitMember(MemberExpression node)
        {
            Console.WriteLine($"VisitMember：{node.NodeType} {node.Type} {node.ToString()}");

            string prop = node.Member.Name;
            this._ConditionStack.Push(prop);
            return node;
        }

        protected override Expression VisitConstant(ConstantExpression node)
        {
            Console.WriteLine($"VisitConstant：{node.NodeType} {node.Type} {node.ToString()}");
            object value = node.Value;
            this._ConditionStack.Push(value.ToString());
            return node;
        }

        protected override Expression VisitMethodCall(MethodCallExpression mc)
        {
            if (mc == null)
                throw new ArgumentNullException("MethodCallExpression");

            Console.WriteLine($"VisitMethodCall {mc.NodeType} {mc.Object} {mc.Arguments[0]}");

            string format;
            switch (mc.Method.Name)
            {
                case "StartsWith":
                    format = "({0} LIKE '{1}%')";
                    break;
                case "Contains":
                    format = "({0} LIKE '%{1}%')";
                    break;
                case "EndsWith":
                    format = "({0} LIKE '%{1}')";
                    break;
                default:
                    throw new NotSupportedException(mc.NodeType + " is not supported!");
            }
            this.Visit(mc.Object);
            this.Visit(mc.Arguments[0]);
            string right = this._ConditionStack.Pop();
            string left = this._ConditionStack.Pop();
            this._ConditionStack.Push(String.Format(format, left, right));

            return mc;
        }

        public string GetWhere()
        {
            string where = string.Concat(_ConditionStack.ToArray());
            _ConditionStack.Clear();
            return where;
        }
    }
}