import React, { useEffect, useState } from 'react';
import { bookCopiesAPI, booksAPI } from '../services/api';
import './BooksList.css';

function BookCopiesList() {
  const [copies, setCopies] = useState([]);
  const [books, setBooks] = useState([]);
  const [loading, setLoading] = useState(true);
  const [editingId, setEditingId] = useState(null);
  const [formData, setFormData] = useState({
    bookId: '',
    copyNumber: '',
    condition: 'New',
    purchaseDate: '',
    isAvailable: true,
  });

  const loadData = async () => {
    try {
      setLoading(true);
      const [copiesRes, booksRes] = await Promise.all([
        bookCopiesAPI.getAll(),
        booksAPI.getAll(),
      ]);
      setCopies(copiesRes.data);
      setBooks(booksRes.data);
    } catch (err) {
      console.error('Error loading book copies or books', err);
      const message =
        err.response?.data?.message ||
        (typeof err.response?.data === 'string' ? err.response.data : null) ||
        err.message ||
        'Error loading book copies or books';
      alert(message);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleChange = (e) => {
    const { name, value, type, checked } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: type === 'checkbox' ? checked : value,
    }));
  };

  const resetForm = () => {
    setEditingId(null);
    setFormData({
      bookId: '',
      copyNumber: '',
      condition: 'New',
      purchaseDate: '',
      isAvailable: true,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!formData.bookId || !formData.copyNumber || !formData.condition || !formData.purchaseDate) {
      alert('All required fields must be filled');
      return;
    }

    const purchase = new Date(formData.purchaseDate);
    const today = new Date();
    if (purchase > today) {
      alert('Purchase date cannot be in the future');
      return;
    }

    const payload = {
      bookId: parseInt(formData.bookId),
      copyNumber: formData.copyNumber,
      condition: formData.condition,
      purchaseDate: formData.purchaseDate,
      isAvailable: formData.isAvailable,
    };

    try {
      if (editingId) {
        // Update lejon vetëm disa fusha sipas backend-it
        await bookCopiesAPI.update(editingId, {
          condition: payload.condition,
          purchaseDate: payload.purchaseDate,
          isAvailable: payload.isAvailable,
        });
      } else {
        await bookCopiesAPI.create(payload);
      }
      resetForm();
      loadData();
    } catch (err) {
      console.error('Error saving book copy', err);
      alert(err.response?.data || 'Error saving book copy');
    }
  };

  const handleEdit = (copy) => {
    setEditingId(copy.id);
    setFormData({
      bookId: copy.bookId,
      copyNumber: copy.copyNumber,
      condition: copy.condition || 'New',
      purchaseDate: copy.purchaseDate ? copy.purchaseDate.substring(0, 10) : '',
      isAvailable: copy.isAvailable,
    });
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this copy?')) return;

    try {
      await bookCopiesAPI.delete(id);
      loadData();
    } catch (err) {
      console.error('Error deleting book copy', err);
      alert(err.response?.data || 'Error deleting book copy');
    }
  };

  const getBookTitle = (bookId) => {
    const book = books.find((b) => b.id === bookId);
    return book ? book.title : '';
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div>
      <h1>Book Copies Management</h1>

      <div className="card" style={{ marginBottom: '1.5rem' }}>
        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <div className="form-group">
              <label>Book *</label>
              <select
                name="bookId"
                value={formData.bookId}
                onChange={handleChange}
                required
              >
                <option value="">Select Book</option>
                {books.map((b) => (
                  <option key={b.id} value={b.id}>
                    {b.title}
                  </option>
                ))}
              </select>
            </div>
            <div className="form-group">
              <label>Copy Number *</label>
              <input
                type="text"
                name="copyNumber"
                value={formData.copyNumber}
                onChange={handleChange}
                required
                disabled={!!editingId}
              />
            </div>
            <div className="form-group">
              <label>Condition *</label>
              <input
                type="text"
                name="condition"
                value={formData.condition}
                onChange={handleChange}
                required
              />
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label>Purchase Date *</label>
              <input
                type="date"
                name="purchaseDate"
                value={formData.purchaseDate}
                onChange={handleChange}
                required
              />
            </div>
            <div className="form-group" style={{ display: 'flex', alignItems: 'center' }}>
              <label style={{ marginRight: '0.5rem' }}>Available</label>
              <input
                type="checkbox"
                name="isAvailable"
                checked={formData.isAvailable}
                onChange={handleChange}
              />
            </div>
          </div>

          <div className="form-actions">
            <button type="submit" className="btn btn-primary">
              {editingId ? 'Update Copy' : 'Add Copy'}
            </button>
            {editingId && (
              <button
                type="button"
                className="btn"
                style={{ marginLeft: '1rem', background: '#95a5a6', color: 'white' }}
                onClick={resetForm}
              >
                Cancel
              </button>
            )}
          </div>
        </form>
      </div>

      <div className="card">
        <table className="table">
          <thead>
            <tr>
              <th>Copy Number</th>
              <th>Book</th>
              <th>Condition</th>
              <th>Purchase Date</th>
              <th>Available</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {copies.length === 0 ? (
              <tr>
                <td colSpan="6" style={{ textAlign: 'center' }}>No copies found</td>
              </tr>
            ) : (
              copies.map((c) => (
                <tr key={c.id}>
                  <td>{c.copyNumber}</td>
                  <td>{c.bookTitle || getBookTitle(c.bookId)}</td>
                  <td>{c.condition}</td>
                  <td>{c.purchaseDate ? new Date(c.purchaseDate).toLocaleDateString() : ''}</td>
                  <td>
                    <span
                      className="status-badge"
                      style={{
                        backgroundColor: c.isAvailable ? '#2ecc71' : '#e74c3c',
                      }}
                    >
                      {c.isAvailable ? 'Available' : 'Not available'}
                    </span>
                  </td>
                  <td>
                    <button
                      className="btn"
                      style={{ marginRight: '0.5rem' }}
                      onClick={() => handleEdit(c)}
                    >
                      Edit
                    </button>
                    <button
                      className="btn btn-danger"
                      onClick={() => handleDelete(c.id)}
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

export default BookCopiesList;


