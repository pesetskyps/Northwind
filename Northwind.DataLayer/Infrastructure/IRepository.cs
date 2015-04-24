﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace NorthWind.DataLayer.Infrastructure
{
public interface IRepository<T> where T : class
{
    void Add(T entity);
    void Delete(T entity);
    void Delete(Expression<Func<T, bool>> where);
    T GetById(int Id);
    T GetById(string Id);
    T Get(Expression<Func<T, bool>> where);
    IEnumerable<T> GetAll();
    IEnumerable<T> GetMany(Expression<Func<T, bool>> where);   
}
}
