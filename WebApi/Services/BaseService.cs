using Repositories.Interfaces;

namespace ApiService.Services
{
    public interface IBaseService
    {
    }

    public class BaseService : IBaseService
    {
        internal readonly IUnitOfWork unitOfWork;

        public BaseService(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork ?? throw new ArgumentNullException(nameof(unitOfWork));
        }
    }
}
