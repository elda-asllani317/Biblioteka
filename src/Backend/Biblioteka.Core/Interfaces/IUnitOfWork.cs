using Biblioteka.Core.Entities;

namespace Biblioteka.Core.Interfaces;

public interface IUnitOfWork : IDisposable
{
    IRepository<User> Users { get; }
    IRepository<Author> Authors { get; }
    IRepository<Book> Books { get; }
    IRepository<Category> Categories { get; }
    IRepository<Publisher> Publishers { get; }
    IRepository<BookCopy> BookCopies { get; }
    IRepository<Loan> Loans { get; }
    IRepository<Review> Reviews { get; }
    IRepository<Fine> Fines { get; }
    IRepository<Notification> Notifications { get; }
    
    Task<int> SaveChangesAsync();
    Task BeginTransactionAsync();
    Task CommitTransactionAsync();
    Task RollbackTransactionAsync();
}

