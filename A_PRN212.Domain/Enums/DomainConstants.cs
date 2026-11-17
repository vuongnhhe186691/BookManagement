namespace A_PRN212.Domain.Enums
{
    public static class UserRole
    {
        public const string Admin = "Admin";
        public const string Librarian = "Librarian";
        public const string Reader = "Reader";
    }

    public static class BorrowStatus
    {
        public const string NotBorrowed = "Not Borrowed";
        public const string Borrowed = "Borrowed";
        public const string Returned = "Returned";
        public const string Overdue = "Overdue";
    }

    public static class CardStatus
    {
        public const string Active = "Active";
        public const string Expired = "Expired";
        public const string Revoked = "Revoked";
    }
}
