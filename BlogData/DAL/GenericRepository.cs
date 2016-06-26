using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using BlogData.Data;

namespace BlogData.DAL
{
    public class GenericRepository<TEntity> where TEntity : class
    {

        internal BlogContext Context;
        internal DbSet<TEntity> DbSet;

        private string _errorMessage = string.Empty;
        
        public GenericRepository(BlogContext c)
        {
            Context = c;
            DbSet = c.Set<TEntity>(); 
        }

        public virtual IEnumerable<TEntity> GetAll()
        {
            return DbSet.ToList();
        }

        public virtual void Insert(TEntity entity)
        {
            try
            {
                if(entity == null) throw new ArgumentNullException();
                
                DbSet.Add(entity);
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationError in dbEx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors))
                {
                    _errorMessage += $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}";
                }

                throw new Exception(_errorMessage, dbEx);
            }
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            try
            {
                if (entityToUpdate == null) throw new ArgumentNullException();

                DbSet.Attach(entityToUpdate);
                Context.Entry(entityToUpdate).State = EntityState.Modified;
            }
            catch (DbEntityValidationException dbEx)
            {
                foreach (var validationError in dbEx.EntityValidationErrors.SelectMany(validationErrors => validationErrors.ValidationErrors))
                {
                    _errorMessage += $"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}";
                }

                throw new Exception(_errorMessage, dbEx);
            }
        }

        public virtual TEntity GetById(object id)
        {
            return DbSet.Find(id);
        }


    }
}
