import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { loansAPI, usersAPI, bookCopiesAPI } from '../services/api';
import './CreateLoan.css';

function CreateLoan() {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    userId: '',
    bookCopyId: '',
    dueDate: '',
  });

  const [users, setUsers] = useState([]);
  const [bookCopies, setBookCopies] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    const loadData = async () => {
      try {
        const [usersRes, copiesRes] = await Promise.all([
          usersAPI.getAll(),
          bookCopiesAPI.getAvailable(),
        ]);
        setUsers(usersRes.data);
        setBookCopies(copiesRes.data);
      } catch (error) {
        console.error('Error loading users/book copies:', error);
        alert('Error loading users/book copies');
      }
    };

    loadData();
  }, []);

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value,
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    if (!formData.dueDate) {
      alert('Due date is required');
      return;
    }

    const selectedDate = new Date(formData.dueDate);
    const today = new Date();
    if (selectedDate < new Date(today.toDateString())) {
      alert('Due date must be today or in the future');
      return;
    }

    setLoading(true);

    try {
      await loansAPI.create({
        userId: parseInt(formData.userId),
        bookCopyId: parseInt(formData.bookCopyId),
        dueDate: formData.dueDate,
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
            <label>Due Date *</label>
            <input
              type="date"
              name="dueDate"
              value={formData.dueDate}
              onChange={handleChange}
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

