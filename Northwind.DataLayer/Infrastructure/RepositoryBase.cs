using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Entity;
using System.Data;
using System.Linq.Expressions;
using System.ServiceModel;
using LinqKit;
using Northwind.DataLayer;
namespace NorthWind.DataLayer.Infrastructure
{
    public abstract class RepositoryBase<T> : IRepository<T> where T : class
    {
        private NorthwindData _dataContext;
        private readonly IDbSet<T> _dbset;
        protected RepositoryBase(IDatabaseFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            _dbset = DataContext.Set<T>();
        }

        protected IDatabaseFactory DatabaseFactory
        {
            get;
            private set;
        }

        protected NorthwindData DataContext
        {
            get { return _dataContext ?? (_dataContext = DatabaseFactory.Get()); }
        }
        public virtual void Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            _dbset.Add(entity);

        }

        public virtual void Delete(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException("entity");

            _dbset.Remove(entity);

        }
        public virtual void Delete(Expression<Func<T, bool>> where)
        {

            IEnumerable<T> objects = _dbset.Where(where).AsEnumerable();
            foreach (T obj in objects)
                _dbset.Remove(obj);

        }
        public virtual T GetById(int id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            T result;

            result = _dbset.Find(id);

            return result;
        }
        public virtual T GetById(string id)
        {
            if (id == null)
                throw new ArgumentNullException("id");
            var result = _dbset.Find(id);
            return result;
        }
        public virtual IEnumerable<T> GetAll()
        {
            return _dbset.ToList();
        }
        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
        {
            //var entities = dbset.ToList();
            return _dbset.AsExpandable().Where<T>(where.Compile()).AsEnumerable();
        }
        public T Get(Expression<Func<T, bool>> where)
        {
            return _dbset.Where(where).FirstOrDefault<T>();
        }
    }
}
