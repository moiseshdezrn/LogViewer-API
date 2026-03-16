
using System.Linq.Expressions;

namespace LogViewer.Infrastructure.Repositories.Interfaces
{
    /// <summary>
    /// Generic repository interface
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public interface IBaseRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Creates a new registry on database
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="saveChanges">
        /// Whether to save changes after the operation. Defaults to true.
        /// </param>
        /// <param name="autoDetectChanges">Detect changes in entity. Defaults to true.</param>
        void Add(TEntity entity, bool saveChanges = true, bool autoDetectChanges = true);

        /// <summary>
        /// Creates registries on database
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="saveChanges">
        /// Whether to save changes after the operation. Defaults to true.
        /// </param>
        /// <param name="autoDetectChanges">Detect changes in entity. Defaults to true.</param>
        void Add(IEnumerable<TEntity> entities, bool saveChanges = true, bool autoDetectChanges = true);

        /// <summary>
        /// Deletes a registry in database.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="saveChanges">
        /// Whether to save changes after the operation. Defaults to true.
        /// </param>
        void Delete(TEntity entity, bool saveChanges = true);

        /// <summary>
        /// Detach entity from the context
        /// </summary>
        /// <param name="entity"></param>
        void Detach(TEntity entity);

        /// <summary>
        /// Find an entity by its primary key.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity Get(int id);

        /// <summary>
        /// Find an entity by its primary key.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        TEntity Get(long id);

        /// <summary>
        /// Get all the entities that match the criteria.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Get all the entities that match the criteria.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="include">Navigation property path</param>
        /// <returns></returns>
        IEnumerable<TEntity> Get<TProperty>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProperty>> include);

        /// <summary>
        /// Get all the entities that match the criteria.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="include">Navigation property path</param>
        /// <returns></returns>
        IEnumerable<TEntity> Get<TProperty>(Expression<Func<TEntity, bool>> predicate, List<Expression<Func<TEntity, TProperty>>> include);

        /// <summary>
        /// Gets all the entities.
        /// </summary>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll();

        /// <summary>
        /// Gets all the entities.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="include">Navigation property path</param>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll<TProperty>(Expression<Func<TEntity, TProperty>> include);

        /// <summary>
        /// Get existing entities according to the filters
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="propertySelectors"></param>
        /// <returns></returns>
        IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors);

        /// <summary>
        /// Gets all the entities
        /// </summary>
        /// <returns></returns>
        IQueryable<TEntity> GetAllAsQueryable();

        /// <summary>
        /// Checks for existing entities
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        bool GetAny(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Get the Single or Default entity that match the criteria.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        TEntity GetSingle(Expression<Func<TEntity, bool>> predicate);

        /// <summary>
        /// Get the Single or Default entity that match the criteria.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="include">Navigation property path</param>
        /// <returns></returns>
        TEntity GetSingle<TProperty>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProperty>> include);

        /// <summary>
        /// Commit the changes to the database.
        /// </summary>
        /// <param name="autoDetectChanges">Detect changes in entity. Defaults to true.</param>
        int SaveChanges(bool autoDetectChanges = true);

        /// <summary>
        /// Updates a row in database.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="saveChanges">
        /// Whether to save changes after the operation. Defaults to true.
        /// </param>
        void Update(TEntity entity, bool saveChanges = true);

        /// <summary>
        /// Updates registries in the database.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="saveChanges"></param>
        void Update(IEnumerable<TEntity> entities, bool saveChanges = true);
    }
}
