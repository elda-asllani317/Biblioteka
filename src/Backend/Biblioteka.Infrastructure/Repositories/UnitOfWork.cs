using Biblioteka.Core.Entities;
using Biblioteka.Core.Interfaces;
using Biblioteka.Infrastructure.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Biblioteka.Infrastructure.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly BibliotekaDbContext _context;
    private IDbContextTransaction? _transaction;
    private bool _disposed = false;

    private IRepository<User>? _users;
    private IRepository<Author>? _authors;
    private IRepository<Book>? _books;
    private IRepository<Category>? _categories;
    private IRepository<Publisher>? _publishers;
    private IRepository<BookCopy>? _bookCopies;
    private IRepository<Loan>? _loans;
    private IRepository<Review>? _reviews;
    private IRepository<Fine>? _fines;
    private IRepository<Notification>? _notifications;

    public UnitOfWork(BibliotekaDbContext context)
    {
        _context = context;
    }

    public IRepository<User> Users => _users ??= new Repository<User>(_context);
    public IRepository<Author> Authors => _authors ??= new Repository<Author>(_context);
    public IRepository<Book> Books => _books ??= new Repository<Book>(_context);
    public IRepository<Category> Categories => _categories ??= new Repository<Category>(_context);
    public IRepository<Publisher> Publishers => _publishers ??= new Repository<Publisher>(_context);
    public IRepository<BookCopy> BookCopies => _bookCopies ??= new Repository<BookCopy>(_context);
    public IRepository<Loan> Loans => _loans ??= new Repository<Loan>(_context);
    public IRepository<Review> Reviews => _reviews ??= new Repository<Review>(_context);
    public IRepository<Fine> Fines => _fines ??= new Repository<Fine>(_context);
    public IRepository<Notification> Notifications => _notifications ??= new Repository<Notification>(_context);

    public async Task<int> SaveChangesAsync()
    {
        return await _context.SaveChangesAsync();
    }

    public async Task BeginTransactionAsync()
    {
        _transaction = await _context.Database.BeginTransactionAsync();
    }

    public async Task CommitTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.CommitAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    public async Task RollbackTransactionAsync()
    {
        if (_transaction != null)
        {
            await _transaction.RollbackAsync();
            await _transaction.DisposeAsync();
            _transaction = null;
        }
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!_disposed)
        {
            if (disposing)
            {
                _transaction?.Dispose();
                _context.Dispose();
            }
            _disposed = true;
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }
}

