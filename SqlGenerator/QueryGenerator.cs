using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Query.SqlExpressions;
using Microsoft.EntityFrameworkCore.SqlServer.Query.Internal;
using Microsoft.EntityFrameworkCore.SqlServer.Infrastructure.Internal;

namespace EFCore.Tips.SqlGenerator;

public class QueryGenerator : SqlServerQuerySqlGenerator
{
    public QueryGenerator(
        QuerySqlGeneratorDependencies dependencies,
        IRelationalTypeMappingSource typeMappingSource,
        ISqlServerSingletonOptions sqlServerSingletonOptions) : base(dependencies, typeMappingSource, sqlServerSingletonOptions)
    {

    }

    protected override Expression VisitTable(TableExpression tableExpression)
    {
        var table = base.VisitTable(tableExpression);

        Sql.Append(" WITH (NOLOCK)");

        return table;
    }
}

public class QueryGeneratorFactory : SqlServerQuerySqlGeneratorFactory
{
    private readonly QuerySqlGeneratorDependencies _dependencies;

    public QueryGeneratorFactory(
        QuerySqlGeneratorDependencies dependencies, 
        IRelationalTypeMappingSource typeMappingSource, 
        ISqlServerSingletonOptions sqlServerSingletonOptions) : base(dependencies, typeMappingSource, sqlServerSingletonOptions)
    {
        _dependencies = dependencies;
    }


    public override QuerySqlGenerator Create()
    {
        return new QuerySqlGenerator(_dependencies);
    }

}
