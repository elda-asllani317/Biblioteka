import React, { useEffect, useState } from 'react';
import { finesAPI, usersAPI } from '../services/api';
import './BooksList.css';

function FinesList() {
  const [fines, setFines] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [editingId, setEditingId] = useState(null);
  const [filters, setFilters] = useState({
    userId: '',
    status: '',
  });
  const [formData, setFormData] = useState({
    amount: '',
    reason: '',
    userId: '',
    loanId: '',
    status: 'Pending',
    paymentDate: '',
  });

  const loadData = async () => {
    try {
      setLoading(true);
      const [finesRes, usersRes] = await Promise.all([
        finesAPI.getAll(),
        usersAPI.getAll(),
      ]);
      setFines(finesRes.data);
      setUsers(usersRes.data);
    } catch (err) {
      console.error('Error loading fines or users', err);
      alert('Error loading fines or users');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadData();
  }, []);

  const handleFilterChange = (e) => {
    const { name, value } = e.target;
    setFilters((prev) => ({ ...prev, [name]: value }));
  };

  const applyFilters = (list) => {
    return list.filter((fine) => {
      let ok = true;
      if (filters.userId) {
        ok = ok && fine.userId === parseInt(filters.userId);
      }
      if (filters.status) {
        ok = ok && fine.status.toLowerCase() === filters.status.toLowerCase();
      }
      return ok;
    });
  };

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({
      ...prev,
      [name]: value,
    }));
  };

  const resetForm = () => {
    setEditingId(null);
    setFormData({
      amount: '',
      reason: '',
      userId: '',
      loanId: '',
      status: 'Pending',
      paymentDate: '',
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();

    const amount = parseFloat(formData.amount);
    if (isNaN(amount) || amount <= 0) {
      alert('Amount duhet të jetë më i madh se 0');
      return;
    }
    if (!formData.reason.trim()) {
      alert('Reason është i detyrueshëm');
      return;
    }
    if (!formData.userId) {
      alert('User është i detyrueshëm');
      return;
    }

    const payload = {
      amount,
      reason: formData.reason,
      userId: parseInt(formData.userId),
      loanId: formData.loanId ? parseInt(formData.loanId) : null,
    };

    if (editingId) {
      payload.status = formData.status;
      payload.paymentDate = formData.paymentDate || null;

      if (payload.status === 'Paid' && !payload.paymentDate) {
        alert('PaymentDate duhet të vendoset kur statusi është Paid');
        return;
      }
    }

    try {
      if (editingId) {
        await finesAPI.update(editingId, payload);
      } else {
        await finesAPI.create(payload);
      }
      resetForm();
      loadData();
    } catch (err) {
      console.error('Error saving fine', err);
      const msg =
        err.response?.data ||
        err.message ||
        'Error saving fine';
      alert(typeof msg === 'string' ? msg : 'Error saving fine');
    }
  };

  const handleEdit = (fine) => {
    setEditingId(fine.id);
    setFormData({
      amount: fine.amount,
      reason: fine.reason,
      userId: fine.userId,
      loanId: fine.loanId || '',
      status: fine.status,
      paymentDate: fine.paymentDate ? fine.paymentDate.substring(0, 10) : '',
    });
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this fine?')) return;
    try {
      await finesAPI.delete(id);
      loadData();
    } catch (err) {
      console.error('Error deleting fine', err);
      alert('Error deleting fine');
    }
  };

  const filteredFines = applyFilters(fines);

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div>
      <h1>Fines Management</h1>

      <div className="card" style={{ marginBottom: '1.5rem' }}>
        <h2 style={{ marginBottom: '1rem' }}>Filters</h2>
        <div className="form-row">
          <div className="form-group">
            <label>User</label>
            <select
              name="userId"
              value={filters.userId}
              onChange={handleFilterChange}
            >
              <option value="">All Users</option>
              {users.map((u) => (
                <option key={u.id} value={u.id}>
                  {u.firstName} {u.lastName}
                </option>
              ))}
            </select>
          </div>
          <div className="form-group">
            <label>Status</label>
            <select
              name="status"
              value={filters.status}
              onChange={handleFilterChange}
            >
              <option value="">All</option>
              <option value="Pending">Pending</option>
              <option value="Paid">Paid</option>
            </select>
          </div>
        </div>
      </div>

      <div className="card" style={{ marginBottom: '1.5rem' }}>
        <h2 style={{ marginBottom: '1rem' }}>{editingId ? 'Edit Fine' : 'Add New Fine'}</h2>
        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <div className="form-group">
              <label>Amount *</label>
              <input
                type="number"
                name="amount"
                value={formData.amount}
                onChange={handleChange}
                min="0.01"
                step="0.01"
                required
              />
            </div>
            <div className="form-group">
              <label>User *</label>
              <select
                name="userId"
                value={formData.userId}
                onChange={handleChange}
                required
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
              <label>Loan Id (optional)</label>
              <input
                type="number"
                name="loanId"
                value={formData.loanId}
                onChange={handleChange}
                min="1"
              />
            </div>
          </div>

          <div className="form-row">
            <div className="form-group" style={{ flex: 2 }}>
              <label>Reason *</label>
              <textarea
                name="reason"
                value={formData.reason}
                onChange={handleChange}
                required
              />
            </div>
            {editingId && (
              <>
                <div className="form-group">
                  <label>Status</label>
                  <select
                    name="status"
                    value={formData.status}
                    onChange={handleChange}
                  >
                    <option value="Pending">Pending</option>
                    <option value="Paid">Paid</option>
                  </select>
                </div>
                <div className="form-group">
                  <label>Payment Date</label>
                  <input
                    type="date"
                    name="paymentDate"
                    value={formData.paymentDate}
                    onChange={handleChange}
                  />
                </div>
              </>
            )}
          </div>

          <div className="form-actions">
            <button type="submit" className="btn btn-primary">
              {editingId ? 'Update Fine' : 'Add Fine'}
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
              <th>User</th>
              <th>Amount</th>
              <th>Reason</th>
              <th>Issue Date</th>
              <th>Payment Date</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {filteredFines.length === 0 ? (
              <tr>
                <td colSpan="7" style={{ textAlign: 'center' }}>No fines found</td>
              </tr>
            ) : (
              filteredFines.map((fine) => (
                <tr key={fine.id}>
                  <td>{fine.userName}</td>
                  <td>{fine.amount.toFixed(2)}</td>
                  <td>{fine.reason}</td>
                  <td>{new Date(fine.issueDate).toLocaleDateString()}</td>
                  <td>{fine.paymentDate ? new Date(fine.paymentDate).toLocaleDateString() : '-'}</td>
                  <td>
                    <span
                      className="status-badge"
                      style={{
                        backgroundColor: fine.status === 'Paid' ? '#27ae60' : '#e67e22',
                        color: 'white',
                      }}
                    >
                      {fine.status}
                    </span>
                  </td>
                  <td>
                    <button
                      className="btn"
                      style={{ marginRight: '0.5rem' }}
                      onClick={() => handleEdit(fine)}
                    >
                      Edit
                    </button>
                    <button
                      className="btn btn-danger"
                      onClick={() => handleDelete(fine.id)}
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

export default FinesList;


