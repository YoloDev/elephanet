using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;


namespace Elephanet
{
    // http://blogs.msdn.com/b/mattwar/archive/2007/08/01/linq-building-an-iqueryable-provider-part-iii.aspx

    public static class ExpressionEvaluator
    {
        public static Expression EvaluateSubtrees(Expression expression, Func<Expression, bool> canBeEvaluated)
        {
            return new SubtreeEvaluator(canBeEvaluated).Evaluate(expression);
        }

        public static Expression EvaluateSubtrees(Expression expression)
        {
            return new SubtreeEvaluator(CanBeEvaluatedLocally).Evaluate(expression);
        }

        private static bool CanBeEvaluatedLocally(Expression expression)
        {
            return expression.NodeType != ExpressionType.Parameter;
        }

        private sealed class SubtreeEvaluator : ExpressionVisitor
        {
            private readonly SubtreeNominator _nominator;
            private HashSet<Expression> _candidates;

            internal SubtreeEvaluator(Func<Expression, bool> canBeEvaluated)
            {
                _nominator = new SubtreeNominator(canBeEvaluated);
            }

            internal Expression Evaluate(Expression node)
            {
                _candidates = _nominator.NominateSubtrees(node);

                return Visit(node);
            }

            public override Expression Visit(Expression node)
            {
                if (node == null)
                {
                    return node;
                }
                else if (_candidates.Contains(node))
                {
                    return TryEvaluateCandidate(node);
                }
                else
                {
                    return base.Visit(node);
                }
            }

            private static Expression TryEvaluateCandidate(Expression node)
            {
                return node.NodeType == ExpressionType.Constant ? node : EvaluateCandidate(node);
            }

            private static Expression EvaluateCandidate(Expression node)
            {
                var lambda = Expression.Lambda(node);

                var function = lambda.Compile();

                return Expression.Constant(function.DynamicInvoke(null), node.Type);
            }
        }

        private sealed class SubtreeNominator : ExpressionVisitor
        {
            private readonly Func<Expression, bool> _canBeEvaluated;
            private HashSet<Expression> _candidates;
            private bool _cannotBeEvaluated;

            internal SubtreeNominator(Func<Expression, bool> canBeEvaluated)
            {
                _canBeEvaluated = canBeEvaluated;
            }

            internal HashSet<Expression> NominateSubtrees(Expression node)
            {
                _candidates = new HashSet<Expression>();

                Visit(node);

                return _candidates;
            }

            public override Expression Visit(Expression node)
            {
                if (node != null)
                {
                    TryNominateSubtree(node);
                }

                return node;
            }

            private void TryNominateSubtree(Expression node)
            {
                var priorCannotBeEvaluated = _cannotBeEvaluated;

                _cannotBeEvaluated = false;

                base.Visit(node);

                if (!_cannotBeEvaluated)
                {
                    if (_canBeEvaluated(node))
                    {
                        _candidates.Add(node);
                    }
                    else
                    {
                        _cannotBeEvaluated = true;
                    }
                }

                _cannotBeEvaluated |= priorCannotBeEvaluated;
            }
        }
    }
}
