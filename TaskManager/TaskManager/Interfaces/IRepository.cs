using System.Collections.Generic;
using System.Threading.Tasks;

namespace TaskManager.Interfaces
{
     public interface IRepository<T> where T : class, new()
    {
		 
        List<T> Get();

		T Get(int id);

        Task<int> Insert(T entity);

        Task<int> InsertAll(List<T> entityList);

        Task<int> Update(T entity);

        Task<int> Delete(T entity);

        Task<int> DeleteAll();
    }
}
