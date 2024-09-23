using System.Reflection;
using SpecBox.Domain.Lib;

public static class EntityUpdater
{
    public static bool ApplyChanges<TEntity, TRequest>(TEntity entity, TRequest request)
        where TEntity : class
        where TRequest : class
    {
        var hasChanges = false;

        var entityProperties = typeof(TEntity).GetProperties(BindingFlags.Public | BindingFlags.Instance);
        var requestProperties = typeof(TRequest).GetProperties(BindingFlags.Public | BindingFlags.Instance);

        foreach (var requestProp in requestProperties)
        {
            var entityProp = Array.Find(entityProperties, ep => ep.Name == requestProp.Name && ep.CanWrite);

            if (entityProp == null)
                continue;

            // Skip RowVersion Field, first we have to ensure that model has changes
            if (entity is IConcurrencyControl && entityProp.Name == nameof(IConcurrencyControl.RowVersion))
                continue;

            var entityValue = entityProp.GetValue(entity);
            var requestValue = requestProp.GetValue(request);
            if (requestValue != null && !requestValue.Equals(entityValue))
            {
                entityProp.SetValue(entity, requestValue);
                hasChanges = true;
            }
        }

        if (hasChanges &&
            entity is IConcurrencyControl ucEntity &&
            request is IConcurrencyControl ucRequest)
        {
            ucEntity.RowVersion = ucRequest.RowVersion;
        }

        return hasChanges;
    }
}
