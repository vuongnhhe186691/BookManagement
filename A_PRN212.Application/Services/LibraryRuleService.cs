using A_PRN212.Application.Interfaces;
using A_PRN212.Domain.Entities;
using A_PRN212.Domain.Interfaces;

namespace A_PRN212.Application.Services
{
    public class LibraryRuleService : ILibraryRuleService
    {
        private readonly IUnitOfWork _unitOfWork;

        public LibraryRuleService(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IEnumerable<LibraryRule> GetAllRules()
        {
            return _unitOfWork.LibraryRules.GetAll().ToList();
        }
    }
}
