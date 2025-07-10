using Repositories;

namespace WebApi.Services
{
    public interface ITestSubmissionService
    {
        
    }

    public class TestSubmissionService : BaseService, ITestSubmissionService
    {
        public TestSubmissionService(IUnitOfWork unitOfWork) : base(unitOfWork) 
        {
        }
    }
}
