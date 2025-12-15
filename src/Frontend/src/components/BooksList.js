import React, { useState, useEffect } from 'react';
import { booksAPI } from '../services/api';
import './BooksList.css';

function BooksList() {
  const [books, setBooks] = useState([]);
  const [searchTerm, setSearchTerm] = useState('');
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    loadBooks();
  }, []);

  const loadBooks = async () => {
    try {
      setLoading(true);
      const response = await booksAPI.getAll();
      setBooks(response.data);
    } catch (error) {
      console.error('Error loading books:', error);
      alert('Error loading books');
    } finally {
      setLoading(false);
    }
  };

  const handleSearch = async () => {
    if (!searchTerm.trim()) {
      loadBooks();
      return;
    }

    try {
      setLoading(true);
      const response = await booksAPI.search(searchTerm);
      setBooks(response.data);
    } catch (error) {
      console.error('Error searching books:', error);
      alert('Error searching books');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this book?')) {
      return;
    }

    try {
      await booksAPI.delete(id);
      loadBooks();
    } catch (error) {
      console.error('Error deleting book:', error);
      alert('Error deleting book');
    }
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div>
      <h1>Books Management</h1>
      
      <div className="search-bar">
        <input
          type="text"
          placeholder="Search books by title, ISBN, or description..."
          value={searchTerm}
          onChange={(e) => setSearchTerm(e.target.value)}
          onKeyPress={(e) => e.key === 'Enter' && handleSearch()}
        />
        <button className="btn btn-primary" onClick={handleSearch} style={{ marginTop: '0.5rem' }}>
          Search
        </button>
      </div>

      <div className="card">
        <table className="table">
          <thead>
            <tr>
              <th>Title</th>
              <th>ISBN</th>
              <th>Author</th>
              <th>Category</th>
              <th>Publisher</th>
              <th>Year</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {books.length === 0 ? (
              <tr>
                <td colSpan="7" style={{ textAlign: 'center' }}>No books found</td>
              </tr>
            ) : (
              books.map((book) => (
                <tr key={book.id}>
                  <td>{book.title}</td>
                  <td>{book.isbn}</td>
                  <td>{book.authorName}</td>
                  <td>{book.categoryName}</td>
                  <td>{book.publisherName}</td>
                  <td>{book.publicationYear}</td>
                  <td>
                    <button
                      className="btn btn-danger"
                      onClick={() => handleDelete(book.id)}
                    >
                      Delete
                    </button>
                  </td>
                </tr>
              ))
            )}
          </tbody>
        </table>
      </div>
    </div>
  );
}

export default BooksList;

