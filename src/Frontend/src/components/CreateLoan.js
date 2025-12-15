import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { loansAPI, booksAPI } from '../services/api';
import './CreateLoan.css';

function CreateLoan() {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    userId: '',
    bookCopyId: '',
    daysToLoan: 14,
  });

  const [users, setUsers] = useState([]);
  const [bookCopies, setBookCopies] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    // In a real app, you would fetch these from API
    // For now, we'll use placeholder data
    setUsers([
      { id: 1, firstName: 'John', lastName: 'Doe' },
      { id: 2, firstName: 'Jane', lastName: 'Smith' },
    ]);
    setBookCopies([
      { id: 1, copyNumber: 'BC001', bookId: 1, isAvailable: true },
      { id: 2, copyNumber: 'BC002', bookId: 1, isAvailable: true },
    ]);
  }, []);

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setLoading(true);

    try {
      await loansAPI.create({
        userId: parseInt(formData.userId),
        bookCopyId: parseInt(formData.bookCopyId),
        daysToLoan: parseInt(formData.daysToLoan),
      });
      navigate('/loans');
    } catch (error) {
      console.error('Error creating loan:', error);
      alert(error.response?.data?.message || 'Error creating loan. Please check all fields.');
    } finally {
      setLoading(false);
    }
  };

  const availableCopies = bookCopies.filter(copy => copy.isAvailable);

  return (
    <div>
      <h1>Create New Loan</h1>
      <div className="card">
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>User *</label>
            <select
              name="userId"
              value={formData.userId}
              onChange={handleChange}
              required
            >
              <option value="">Select User</option>
              {users.map((user) => (
                <option key={user.id} value={user.id}>
                  {user.firstName} {user.lastName}
                </option>
              ))}
            </select>
          </div>

          <div className="form-group">
            <label>Book Copy *</label>
            <select
              name="bookCopyId"
              value={formData.bookCopyId}
              onChange={handleChange}
              required
            >
              <option value="">Select Book Copy</option>
              {availableCopies.map((copy) => (
                <option key={copy.id} value={copy.id}>
                  {copy.copyNumber}
                </option>
              ))}
            </select>
            {availableCopies.length === 0 && (
              <p style={{ color: '#e74c3c', marginTop: '0.5rem' }}>
                No available book copies
              </p>
            )}
          </div>

          <div className="form-group">
            <label>Days to Loan *</label>
            <input
              type="number"
              name="daysToLoan"
              value={formData.daysToLoan}
              onChange={handleChange}
              min="1"
              max="30"
              required
            />
          </div>

          <div className="form-actions">
            <button type="submit" className="btn btn-primary" disabled={loading}>
              {loading ? 'Creating...' : 'Create Loan'}
            </button>
            <button
              type="button"
              className="btn"
              onClick={() => navigate('/loans')}
              style={{ marginLeft: '1rem', background: '#95a5a6', color: 'white' }}
            >
              Cancel
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default CreateLoan;

