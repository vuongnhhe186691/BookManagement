using A_PRN212.Domain.Entities;

namespace A_PRN212.Domain.Interfaces
{
    public interface ICardRepository : IGenericRepository<Card>
    {
        IEnumerable<Card> GetByUserId(int userId);
    }
}
