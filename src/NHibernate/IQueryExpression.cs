using NHibernate.Engine;
using NHibernate.Hql.Ast.ANTLR.Tree;
using NHibernate.Impl;

namespace NHibernate
{
    public interface IQueryExpression
    {
        IASTNode Translate(ISessionFactory sessionFactory);
        string Key { get; }
        System.Type Type { get; }
        void SetQueryParametersPriorToExecute(QueryImpl impl);
    }
}