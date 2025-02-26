using Microsoft.EntityFrameworkCore;
using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore.Metadata;

namespace EFCore.Tips.Extensions;

public static class ModelBuilderExtension
{
    public static void ToSnakeCaseNames(this ModelBuilder modelBuilder)
    {
        foreach (var entity in modelBuilder.Model.GetEntityTypes())
        {
            var tableName = entity.GetTableName().ToSnakeCase();

            entity.SetTableName(tableName);

            foreach (var property in entity.GetProperties())
            {
                var objectIdentifier = StoreObjectIdentifier.Table(tableName, null);
                property.SetColumnName(property.GetColumnName(objectIdentifier).ToSnakeCase());
            }

            foreach (var key in entity.GetKeys())
                key.SetName(key.GetName().ToSnakeCase());

            foreach (var key in entity.GetForeignKeys())
                key.SetConstraintName(key.GetConstraintName().ToSnakeCase());

            foreach (var index in entity.GetIndexes())
                index.SetDatabaseName(index.GetDatabaseName().ToSnakeCase());
        }
    }

    public static string ToSnakeCase(this string name)
        => Regex.Replace(name, @"([a-z0-9])([A-Z])", "$1_$2").ToLower();
}