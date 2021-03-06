﻿using System;
using System.Linq;
using SMS.CORE;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Collections.Generic;

namespace SMS.DATA
{
    /// <summary>
    /// base repository
    /// </summary>
    /// <typeparam name="T">entity</typeparam>
    public class GenericRepository<T> where T: BaseEntity
    {
        private readonly SMSContext _context;
        private IDbSet<T> _Entities;

        /// <summary>
        /// get all entities
        /// </summary>
        public virtual IDbSet<T> Entities
        {
            get
            {
                if (this._Entities == null)
                    this._Entities = _context.Set<T>();
                return this._Entities;
            }
        }

        /// <summary>
        /// get dbcontext
        /// </summary>
        public virtual SMSContext DbContext
        {
            get
            {
                return this._context;
            }
        }

        /// <summary>
        /// get a table
        /// </summary>
        public virtual IQueryable<T> Table
        {
            get
            {
                return this.Entities;
            }
        }

        /// <summary>
        /// constructor
        /// </summary>
        /// <param name="dbContext">dbcontext</param>
        public GenericRepository(SMSContext dbContext)
        {
            this._context = dbContext;
        }

        /// <summary>
        /// Get entity by ids
        /// </summary>
        /// <param name="ids">entity primary key</param>
        /// <returns>entity</returns>
        public virtual T GetEntityById(params object[] ids)
        {
            T entity;
            try
            {
                this._context.Configuration.AutoDetectChangesEnabled = false;
                entity = this.Entities.Find(ids);
            }
            finally
            {
                this._context.Configuration.AutoDetectChangesEnabled = true;
            }
            return entity;
        }

        /// <summary>
        /// Insert entity
        /// </summary>
        /// <param name="entity">entity</param>
        public virtual void Insert(T entity)
        {
            try
            {
                if (entity == null)
                    throw new ArgumentNullException("entity is null");

                this.Entities.Add(entity);
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

                var fail = new Exception(msg, dbEx);
                throw fail;
            }
            catch (Exception ex)
            {
                var fail = new Exception(ex.Message, ex);
                throw fail;
            }
        }

        /// <summary>
        /// Insert entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Insert(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                foreach (var entity in entities)
                    this.Entities.Add(entity);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        /// <summary>
        /// update entity
        /// </summary>
        /// <param name="entity">entity</param>
        public virtual void Update(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity is null");

            this._context.Entry<T>(entity).State = EntityState.Modified;
            this._context.SaveChanges();
        }

        /// <summary>
        /// Update entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Update(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                foreach (var entity in entities)
                    this._context.Entry(entity).State = EntityState.Modified;
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage) + Environment.NewLine;

                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        /// <summary>
        /// delete entity
        /// </summary>
        /// <param name="entity">entity</param>
        public virtual void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity is null");
            this.Entities.Attach(entity);
            this.Entities.Remove(entity);
            this._context.SaveChanges();
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void Delete(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                foreach (var entity in entities)
                    this.Entities.Remove(entity);

                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }

        /// <summary>
        /// delete entity by ids
        /// </summary>
        /// <param name="ids">entity's primary key</param>
        public virtual void DeleteById(params object[] ids)
        {
            var entity = GetEntityById(ids);
            if (entity != null)
                Delete(entity);
        }

        /// <summary>
        /// InActive entity
        /// </summary>
        /// <param name="entity">entity</param>
        public virtual void FakeDelete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity is null");

            entity.Active = false;
            this._context.Entry<T>(entity).State = EntityState.Modified;
            this._context.SaveChanges();
        }

        /// <summary>
        /// Delete entities
        /// </summary>
        /// <param name="entities">Entities</param>
        public virtual void FakeDelete(IEnumerable<T> entities)
        {
            try
            {
                if (entities == null)
                    throw new ArgumentNullException("entities");

                foreach (var entity in entities)
                {
                    entity.Active = false;
                    this._context.Entry<T>(entity).State = EntityState.Modified;
                }
                this._context.SaveChanges();
            }
            catch (DbEntityValidationException dbEx)
            {
                var msg = string.Empty;

                foreach (var validationErrors in dbEx.EntityValidationErrors)
                    foreach (var validationError in validationErrors.ValidationErrors)
                        msg += Environment.NewLine + string.Format("Property: {0} Error: {1}", validationError.PropertyName, validationError.ErrorMessage);

                var fail = new Exception(msg, dbEx);
                throw fail;
            }
        }
        
        /// <summary>
        /// InActive entity
        /// </summary>
        /// <param name="ids">entity's primary key</param>
        public void FakeDeleteById(params object[] ids)
        {
            var entity = GetEntityById(ids);
            if (entity != null)
                FakeDelete(entity);
        }
    }
}
