using System.Text.Json.Nodes;
using Umbraco.Cms.Core.Models;

namespace Perplex.ContentBlocks.Migrations.M001_MigrateFromNestedContentToBlockList;

/// <summary>
/// Migrates a property value inside Perplex.ContentBlocks from Nested Content to Block List format.
/// Can be used if the default migration does not properly migrate a property value, for example with a custom property editor.
/// </summary>
public interface IContentBlocksPropertyValueMigrator
{
    /// <summary>
    /// Migrates a property value inside Perplex.ContentBlocks from Nested Content to Block List format.
    /// Return a new JsonNode value in <paramref name="migratedValue"/>, do not modify <paramref name="originalValue"/>.
    /// When you performed a migration, return <c>true</c> and set the <paramref name="migratedValue"/> to the new value.
    /// If you did not perform a migration, return <c>false</c> and set <paramref name="migratedValue"/> to <c>null</c>.
    /// </summary>
    /// <param name="originalValue">The original property value inside the Nested Content object without any migrations applied to it.</param>
    /// <param name="propertyType">The property type</param>
    /// <param name="contentType">The content type</param>
    /// <param name="migratedValue">The migrated property value</param>
    /// <returns><c>true</c> if the value was migrated, otherwise <c>false</c>.</returns>
    bool MigratePropertyValue(JsonNode? originalValue, IPropertyType propertyType, IContentType contentType, out JsonNode? migratedValue);
}
