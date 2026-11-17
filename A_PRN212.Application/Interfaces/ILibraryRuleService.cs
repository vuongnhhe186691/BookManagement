using A_PRN212.Domain.Entities;

namespace A_PRN212.Application.Interfaces
{
    public interface ILibraryRuleService
    {
        IEnumerable<LibraryRule> GetAllRules();
    }
}
