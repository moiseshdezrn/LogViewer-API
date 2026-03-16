using LogViewer.Infrastructure.Repositories.Interfaces;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace LogViewer.Infrastructure.Repositories
{
    /// <summary>
    /// Generic Base Repository implementation
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    /// <typeparam name="TContext">/typeparam>
    public class BaseRepository<TEntity, TContext> : IBaseRepository<TEntity>
        where TEntity : class
        where TContext : DbContext
    {
        /// <summary>
        /// Identity Server Context
        /// </summary>
        protected readonly TContext _context;

        /// <summary>
        /// Base Repository Constructor
        /// </summary>
        /// <param name="dbContext">Identity Server Context</param>
        public BaseRepository(TContext dbContext)
        {
            _context = dbContext;
        }

        /// <inheritdoc cref="Add(IEnumerable{TEntity}, bool, bool)"/>
        [ExcludeFromCodeCoverage]
        public virtual void Add(IEnumerable<TEntity> entities, bool saveChanges = true, bool autoDetectChanges = true)
        {
            _context.Set<TEntity>().AddRange(entities);
            if (saveChanges)
                SaveChanges(autoDetectChanges);
        }

        /// <inheritdoc cref="Add(TEntity, bool ,bool)"/>
        [ExcludeFromCodeCoverage]
        public virtual void Add(TEntity entity, bool saveChanges = true, bool autoDetectChanges = true)
        {
            _context.Set<TEntity>().Add(entity);
            if (saveChanges)
                SaveChanges(autoDetectChanges);
        }

        /// <summary>
        /// Deletes a registry in database.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="saveChanges">
        /// Whether to save changes after the operation. Defaults to true.
        /// </param>
        [ExcludeFromCodeCoverage]
        public virtual void Delete(TEntity entity, bool saveChanges = true)
        {
            _context.Set<TEntity>().Remove(entity);
            if (saveChanges)
                SaveChanges();
        }

        /// <summary>
        /// Detach entity from the context
        /// </summary>
        /// <param name="entity"></param>
        [ExcludeFromCodeCoverage]
        public void Detach(TEntity entity)
        {
            _context.Entry(entity).State = EntityState.Detached;
        }

        /// <summary>
        /// Get all the entities that match the criteria.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().Where(predicate).ToList();
        }

        /// <summary>
        /// Get all the entities that match the criteria.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="include">Navigation property path</param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Get<TProperty>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProperty>> include)
        {
            return _context.Set<TEntity>().Include(include).Where(predicate).AsEnumerable();
        }

        /// <summary>
        /// Get all the entities that match the criteria.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="include">Navigation property path</param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> Get<TProperty>(Expression<Func<TEntity, bool>> predicate, List<Expression<Func<TEntity, TProperty>>> include)
        {
            var entity = _context.Set<TEntity>();

            foreach (var criteria in include)
            {
                entity.Include(criteria);
            }

            return entity.Where(predicate).AsEnumerable();
        }

        /// <summary>
        /// Find an entity by its primary key.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity Get(int id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        /// <summary>
        /// Find an entity by its primary key.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public virtual TEntity Get(long id)
        {
            return _context.Set<TEntity>().Find(id);
        }

        /// <summary>
        /// Gets all the entities.
        /// </summary>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetAll()
        {
            return _context.Set<TEntity>();
        }

        /// <summary>
        /// Gets all the entities.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="include">Navigation property path</param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetAll<TProperty>(Expression<Func<TEntity, TProperty>> include)
        {
            return _context.Set<TEntity>().Include(include).AsEnumerable();
        }

        /// <summary>
        /// Get existing entities according to the filters
        /// </summary>
        /// <param name="predicate"></param>
        /// <param name="propertySelectors"></param>
        /// <returns></returns>
        public virtual IEnumerable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate, params Expression<Func<TEntity, object>>[] propertySelectors)
        {
            var entity = _context.Set<TEntity>().AsQueryable();

            if (propertySelectors != null && propertySelectors.Any())
            {
                foreach (var propertySelector in propertySelectors)
                {
                    entity = entity.Include(propertySelector);
                }
            }

            if (predicate == null)
            {
                return entity.AsEnumerable();
            }

            return entity.Where(predicate).AsEnumerable();
        }

        /// <summary>
        /// Gets all the entities as queryable tree
        /// </summary>
        /// <returns></returns>
        public IQueryable<TEntity> GetAllAsQueryable()
        {
            return _context.Set<TEntity>();
        }

        /// <summary>
        /// Checks for existing entities
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual bool GetAny(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().Where(predicate).Any();
        }

        /// <summary>
        /// Get the Single or Default entity that match the criteria.
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public virtual TEntity GetSingle(Expression<Func<TEntity, bool>> predicate)
        {
            return _context.Set<TEntity>().AsNoTracking().SingleOrDefault(predicate);
        }

        /// <summary>
        /// Get the Single or Default entity that match the criteria.
        /// </summary>
        /// <typeparam name="TProperty"></typeparam>
        /// <param name="predicate"></param>
        /// <param name="include">Navigation property path</param>
        /// <returns></returns>
        public virtual TEntity GetSingle<TProperty>(Expression<Func<TEntity, bool>> predicate, Expression<Func<TEntity, TProperty>> include)
        {
            return _context.Set<TEntity>().Include(include).AsNoTracking().SingleOrDefault(predicate);
        }

        /// <inheritdoc cref="SaveChanges(bool)"/>
        public int SaveChanges(bool autoDetectChanges = true)
        {
            int result;

            if (autoDetectChanges)
            {
                result = _context.SaveChanges();
            }
            else
            {
                try
                {
                    _context.ChangeTracker.AutoDetectChangesEnabled = false;
                    result = _context.SaveChanges();
                }
                finally
                {
                    _context.ChangeTracker.AutoDetectChangesEnabled = true;
                }
            }

            return result;
        }

        /// <summary>
        /// Updates a row in database.
        /// </summary>
        /// <param name="entity"></param>
        /// <param name="saveChanges">
        /// Whether to save changes after the operation. Defaults to true.
        /// </param>
        [ExcludeFromCodeCoverage]
        public virtual void Update(TEntity entity, bool saveChanges = true)
        {
            _context.Set<TEntity>().Update(entity);
            if (saveChanges)
                SaveChanges();
        }

        /// <summary>
        /// Updates registries in the database.
        /// </summary>
        /// <param name="entities"></param>
        /// <param name="saveChanges"></param>
        [ExcludeFromCodeCoverage]
        public virtual void Update(IEnumerable<TEntity> entities, bool saveChanges = true)
        {
            _context.Set<TEntity>().UpdateRange(entities);
            if (saveChanges)
                SaveChanges();
        }
    }
}
