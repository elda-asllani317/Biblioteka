import React, { useEffect, useState } from 'react';
import { reviewsAPI, booksAPI, usersAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import './BooksList.css';

function ReviewsList() {
  const { user } = useAuth();
  const [reviews, setReviews] = useState([]);
  const [books, setBooks] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [editingId, setEditingId] = useState(null);
  const [filterBookId, setFilterBookId] = useState(null);
  const [filterUserId, setFilterUserId] = useState(null);
  const [averageRating, setAverageRating] = useState(0);
  const [error, setError] = useState('');
  const [formData, setFormData] = useState({
    rating: 5,
    comment: '',
    userId: user?.id || null,
    bookId: null,
  });

  useEffect(() => {
    loadBooks();
    loadUsers();
    loadReviews();
  }, []);

  useEffect(() => {
    if (filterBookId) {
      loadReviews();
      loadAverageRating(filterBookId);
    }
  }, [filterBookId]);

  useEffect(() => {
    loadReviews();
  }, [filterUserId]);

  const loadBooks = async () => {
    try {
      const res = await booksAPI.getAll();
      setBooks(res.data);
    } catch (err) {
      console.error('Error loading books', err);
    }
  };

  const loadUsers = async () => {
    try {
      const res = await usersAPI.getAll();
      setUsers(res.data);
    } catch (err) {
      console.error('Error loading users', err);
    }
  };

  const loadReviews = async () => {
    try {
      setLoading(true);
      setError('');
      let res;

      if (filterBookId) {
        res = await reviewsAPI.getByBook(filterBookId);
      } else if (filterUserId) {
        res = await reviewsAPI.getByUser(filterUserId);
      } else {
        // Load all reviews (we'll need to get them from books)
        res = { data: [] };
        // For simplicity, we'll show reviews when a book is selected
      }

      setReviews(res.data);
    } catch (err) {
      console.error('Error loading reviews', err);
      setError(err.response?.data?.message || 'Error loading reviews');
    } finally {
      setLoading(false);
    }
  };

  const loadAverageRating = async (bookId) => {
    try {
      const res = await reviewsAPI.getAverageRating(bookId);
      setAverageRating(res.data.averageRating || 0);
    } catch (err) {
      console.error('Error loading average rating', err);
    }
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    setError('');
  };

  const resetForm = () => {
    setEditingId(null);
    setFormData({
      rating: 5,
      comment: '',
      userId: user?.id || null,
      bookId: filterBookId || null,
    });
    setError('');
  };

  const validateForm = async () => {
    if (!formData.rating || formData.rating < 1 || formData.rating > 5) {
      setError('Rating duhet të jetë midis 1 dhe 5');
      return false;
    }
    if (!formData.userId) {
      setError('UserId është i detyrueshëm');
      return false;
    }
    if (!formData.bookId) {
      setError('BookId është i detyrueshëm');
      return false;
    }

    // Check if user has already reviewed this book (only for new reviews)
    if (!editingId) {
      try {
        const checkRes = await reviewsAPI.checkUserHasReviewed(formData.userId, formData.bookId);
        if (checkRes.data.hasReviewed) {
          setError('Ju keni dhënë tashmë një recension për këtë libër');
          return false;
        }
      } catch (err) {
        console.error('Error checking review', err);
      }
    }

    return true;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!(await validateForm())) return;

    try {
      setError('');
      const payload = {
        rating: parseInt(formData.rating),
        comment: formData.comment.trim() || '',
        userId: formData.userId,
        bookId: formData.bookId,
      };

      if (editingId) {
        await reviewsAPI.update(editingId, payload);
      } else {
        await reviewsAPI.create(payload);
      }

      resetForm();
      loadReviews();
      if (filterBookId) {
        loadAverageRating(filterBookId);
      }
    } catch (err) {
      console.error('Error saving review', err);
      setError(err.response?.data?.message || 'Error saving review');
    }
  };

  const handleEdit = (review) => {
    // Only allow editing if it's the current user's review
    if (review.userId !== user?.id) {
      setError('Ju mund të modifikoni vetëm recensionet tuaja');
      return;
    }

    setEditingId(review.id);
    setFormData({
      rating: review.rating,
      comment: review.comment || '',
      userId: review.userId,
      bookId: review.bookId,
    });
    setError('');
  };

  const handleDelete = async (id, reviewUserId) => {
    // Only allow deletion if it's the current user's review
    if (reviewUserId !== user?.id) {
      setError('Ju mund të fshini vetëm recensionet tuaja');
      return;
    }

    if (!window.confirm('A jeni të sigurt që dëshironi të fshini këtë recension?')) return;
    try {
      setError('');
      await reviewsAPI.delete(id);
      loadReviews();
      if (filterBookId) {
        loadAverageRating(filterBookId);
      }
    } catch (err) {
      console.error('Error deleting review', err);
      setError(err.response?.data?.message || 'Error deleting review');
    }
  };

  const renderStars = (rating) => {
    return '★'.repeat(rating) + '☆'.repeat(5 - rating);
  };

  const getRatingDistribution = () => {
    const distribution = { 5: 0, 4: 0, 3: 0, 2: 0, 1: 0 };
    reviews.forEach(r => {
      distribution[r.rating] = (distribution[r.rating] || 0) + 1;
    });
    return distribution;
  };

  const ratingDistribution = getRatingDistribution();
  const totalReviews = reviews.length;

  if (loading && reviews.length === 0) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div>
      <h1>Reviews Management</h1>

      {error && (
        <div style={{
          padding: '1rem',
          marginBottom: '1rem',
          backgroundColor: '#fee',
          color: '#c33',
          borderRadius: '4px'
        }}>
          {error}
        </div>
      )}

      <div className="card" style={{ marginBottom: '1.5rem' }}>
        <h2>{editingId ? 'Edit Review' : 'Add New Review'}</h2>
        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <div className="form-group" style={{ flex: 1 }}>
              <label>Book *</label>
              <select
                name="bookId"
                value={formData.bookId || ''}
                onChange={(e) => setFormData(prev => ({ ...prev, bookId: parseInt(e.target.value) }))}
                required
                disabled={!!editingId}
                style={{ width: '100%', padding: '0.5rem', border: '1px solid #ddd', borderRadius: '4px' }}
              >
                <option value="">Select Book</option>
                {books.map((b) => (
                  <option key={b.id} value={b.id}>
                    {b.title}
                  </option>
                ))}
              </select>
            </div>
            <div className="form-group" style={{ flex: 1 }}>
              <label>User *</label>
              <select
                name="userId"
                value={formData.userId || ''}
                onChange={(e) => setFormData(prev => ({ ...prev, userId: parseInt(e.target.value) }))}
                required
                disabled={!!editingId}
                style={{ width: '100%', padding: '0.5rem', border: '1px solid #ddd', borderRadius: '4px' }}
              >
                <option value="">Select User</option>
                {users.map((u) => (
                  <option key={u.id} value={u.id}>
                    {u.firstName} {u.lastName}
                  </option>
                ))}
              </select>
            </div>
            <div className="form-group">
              <label>Rating *</label>
              <select
                name="rating"
                value={formData.rating}
                onChange={handleChange}
                required
                style={{ width: '100%', padding: '0.5rem', border: '1px solid #ddd', borderRadius: '4px' }}
              >
                <option value={1}>1 ★</option>
                <option value={2}>2 ★★</option>
                <option value={3}>3 ★★★</option>
                <option value={4}>4 ★★★★</option>
                <option value={5}>5 ★★★★★</option>
              </select>
            </div>
          </div>

          <div className="form-row">
            <div className="form-group" style={{ flex: 1 }}>
              <label>Comment</label>
              <textarea
                name="comment"
                value={formData.comment}
                onChange={handleChange}
                rows="3"
                placeholder="Shkruani komentin tuaj (opsionale)"
              />
            </div>
          </div>

          <div className="form-actions">
            <button type="submit" className="btn btn-primary">
              {editingId ? 'Update Review' : 'Add Review'}
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

      <div className="card" style={{ marginBottom: '1.5rem' }}>
        <h2>Filters</h2>
        <div style={{ display: 'flex', gap: '1rem', alignItems: 'center', flexWrap: 'wrap' }}>
          <div style={{ flex: 1, minWidth: '200px' }}>
            <label style={{ display: 'block', marginBottom: '0.5rem' }}>Filter by Book:</label>
            <select
              value={filterBookId || ''}
              onChange={(e) => {
                const bookId = e.target.value ? parseInt(e.target.value) : null;
                setFilterBookId(bookId);
                setFilterUserId(null);
              }}
              style={{ width: '100%', padding: '0.5rem', border: '1px solid #ddd', borderRadius: '4px' }}
            >
              <option value="">All Books</option>
              {books.map((b) => (
                <option key={b.id} value={b.id}>
                  {b.title}
                </option>
              ))}
            </select>
          </div>
          <div style={{ flex: 1, minWidth: '200px' }}>
            <label style={{ display: 'block', marginBottom: '0.5rem' }}>Filter by User:</label>
            <select
              value={filterUserId || ''}
              onChange={(e) => {
                const userId = e.target.value ? parseInt(e.target.value) : null;
                setFilterUserId(userId);
                setFilterBookId(null);
              }}
              style={{ width: '100%', padding: '0.5rem', border: '1px solid #ddd', borderRadius: '4px' }}
            >
              <option value="">All Users</option>
              {users.map((u) => (
                <option key={u.id} value={u.id}>
                  {u.firstName} {u.lastName}
                </option>
              ))}
            </select>
          </div>
        </div>

        {filterBookId && averageRating > 0 && (
          <div style={{ marginTop: '1rem', padding: '1rem', backgroundColor: '#f0f0f0', borderRadius: '4px' }}>
            <strong>Average Rating: </strong>
            <span style={{ fontSize: '1.2rem', color: '#f39c12' }}>
              {averageRating.toFixed(1)} / 5.0
            </span>
            <span style={{ marginLeft: '0.5rem', fontSize: '1.2rem' }}>
              {renderStars(Math.round(averageRating))}
            </span>
          </div>
        )}
      </div>

      {totalReviews > 0 && (
        <div className="card" style={{ marginBottom: '1.5rem' }}>
          <h3>Rating Distribution</h3>
          <div style={{ display: 'flex', flexDirection: 'column', gap: '0.5rem' }}>
            {[5, 4, 3, 2, 1].map((rating) => {
              const count = ratingDistribution[rating] || 0;
              const percentage = totalReviews > 0 ? (count / totalReviews) * 100 : 0;
              return (
                <div key={rating} style={{ display: 'flex', alignItems: 'center', gap: '1rem' }}>
                  <span style={{ minWidth: '60px' }}>{rating} ★</span>
                  <div style={{ flex: 1, height: '20px', backgroundColor: '#e0e0e0', borderRadius: '4px', position: 'relative' }}>
                    <div
                      style={{
                        width: `${percentage}%`,
                        height: '100%',
                        backgroundColor: '#3498db',
                        borderRadius: '4px',
                        transition: 'width 0.3s'
                      }}
                    />
                  </div>
                  <span style={{ minWidth: '80px', textAlign: 'right' }}>{count} ({percentage.toFixed(1)}%)</span>
                </div>
              );
            })}
          </div>
        </div>
      )}

      <div className="card">
        <div style={{ overflowX: 'auto' }}>
          <table className="table">
            <thead>
              <tr>
                <th>Book</th>
                <th>User</th>
                <th>Rating</th>
                <th>Comment</th>
                <th>Date</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {reviews.length === 0 ? (
                <tr>
                  <td colSpan="6" style={{ textAlign: 'center' }}>
                    {loading ? 'Loading...' : 'No reviews found. Select a book or user to filter reviews.'}
                  </td>
                </tr>
              ) : (
                reviews.map((r) => (
                  <tr key={r.id}>
                    <td>{r.bookTitle}</td>
                    <td>{r.userName}</td>
                    <td>
                      <span style={{ color: '#f39c12', fontSize: '1.2rem' }}>
                        {renderStars(r.rating)}
                      </span>
                      <span style={{ marginLeft: '0.5rem' }}>({r.rating}/5)</span>
                    </td>
                    <td style={{ maxWidth: '300px', overflow: 'hidden', textOverflow: 'ellipsis' }}>
                      {r.comment || '-'}
                    </td>
                    <td>{new Date(r.reviewDate).toLocaleDateString()}</td>
                    <td>
                      {r.userId === user?.id && (
                        <>
                          <button
                            className="btn"
                            style={{ marginRight: '0.5rem' }}
                            onClick={() => handleEdit(r)}
                          >
                            Edit
                          </button>
                          <button
                            className="btn btn-danger"
                            onClick={() => handleDelete(r.id, r.userId)}
                          >
                            Delete
                          </button>
                        </>
                      )}
                      {r.userId !== user?.id && (
                        <span style={{ color: '#999', fontSize: '0.9rem' }}>-</span>
                      )}
                    </td>
                  </tr>
                ))
              )}
            </tbody>
          </table>
        </div>
      </div>
    </div>
  );
}

export default ReviewsList;

