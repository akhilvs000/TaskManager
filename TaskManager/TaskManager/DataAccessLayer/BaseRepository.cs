using Realms;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using TaskManager.Interfaces;


namespace TaskManager.DataAccessLayer
{
    public class BaseRepository<T> : IRepository<T> where T : RealmObject, new()
    {
        private Realm dbInstance;
        public BaseRepository()
        {
            dbInstance = Realm.GetInstance();
        }

        Task<int> IRepository<T>.Delete(T entity)
        {
            try
            {
                dbInstance.Write(() =>
                {
                    dbInstance.Remove(entity);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Delete failed" + ex.Message);
                return Task.FromResult(-1);
            }
            return Task.FromResult(0);
        }

        Task<int> IRepository<T>.DeleteAll()
        {
            try
            {
                dbInstance.Write(() =>
                {
                    dbInstance.RemoveAll<T>();
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Delete All failed" + ex.Message);
                return Task.FromResult(-1);
            }
            return Task.FromResult(0);
        }

        public List<T> Get() =>
          dbInstance.All<T>().ToList();
       
        T IRepository<T>.Get(int id) =>
             dbInstance.Find<T>(id);

        Task<int> IRepository<T>.Insert(T entity)
        {
            try
            {
                dbInstance.Write(() =>
                {
                    dbInstance.Add(entity);
                });
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Insert failed" + ex.Message);
                return Task.FromResult(-1);
            }
            return Task.FromResult(0);
        }

        Task<int> IRepository<T>.InsertAll(List<T> entityList)
        {
            try
            {
                dbInstance.Write(() =>
                {
                    foreach(var entity in entityList)
                        dbInstance.Add(entity);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("InsertAll failed" + ex.Message);
                return Task.FromResult(-1);
            }
            return Task.FromResult(0);
        }

        Task<int> IRepository<T>.Update(T entity)
        {
            try
            {
                dbInstance.Write(() =>
                {
                    dbInstance.Add(entity,true);
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Insert failed" + ex.Message);
                return Task.FromResult(-1);
            }
            return Task.FromResult(0);
        }
    }
}

