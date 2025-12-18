import React, { useEffect, useState } from 'react';
import { publishersAPI } from '../services/api';
import './BooksList.css';

function PublishersList() {
  const [publishers, setPublishers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [editingId, setEditingId] = useState(null);
  const [searchTerm, setSearchTerm] = useState('');
  const [currentPage, setCurrentPage] = useState(1);
  const [pageSize] = useState(10);
  const [totalPages, setTotalPages] = useState(1);
  const [error, setError] = useState('');
  const [formData, setFormData] = useState({
    name: '',
    address: '',
    phone: '',
    email: '',
  });

  const loadPublishers = async (page = 1, search = '') => {
    try {
      setLoading(true);
      setError('');
      const params = {
        page,
        pageSize,
        ...(search && { search }),
      };
      const res = await publishersAPI.getAll(params);
      setPublishers(res.data);
      
      // Extract pagination info from headers
      const totalCount = parseInt(res.headers['x-total-count'] || '0');
      const totalPagesCount = parseInt(res.headers['x-total-pages'] || '1');
      setTotalPages(totalPagesCount);
    } catch (err) {
      console.error('Error loading publishers', err);
      setError(err.response?.data?.message || 'Error loading publishers');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadPublishers(currentPage, searchTerm);
  }, [currentPage, searchTerm]);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
    setError('');
  };

  const handleSearchChange = (e) => {
    setSearchTerm(e.target.value);
    setCurrentPage(1); // Reset to first page on search
  };

  const resetForm = () => {
    setEditingId(null);
    setFormData({
      name: '',
      address: '',
      phone: '',
      email: '',
    });
    setError('');
  };

  const validateForm = () => {
    if (!formData.name.trim()) {
      setError('Name është i detyrueshëm');
      return false;
    }
    if (!formData.email.trim()) {
      setError('Email është i detyrueshëm');
      return false;
    }
    // Basic email validation
    const emailRegex = /^[^\s@]+@[^\s@]+\.[^\s@]+$/;
    if (!emailRegex.test(formData.email)) {
      setError('Email duhet të jetë në format të vlefshëm');
      return false;
    }
    if (!formData.phone.trim()) {
      setError('Phone është i detyrueshëm');
      return false;
    }
    if (!formData.address.trim()) {
      setError('Address është e detyrueshme');
      return false;
    }
    return true;
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    if (!validateForm()) return;

    try {
      setError('');
      const payload = {
        name: formData.name.trim(),
        address: formData.address.trim(),
        phone: formData.phone.trim(),
        email: formData.email.trim(),
      };

      if (editingId) {
        await publishersAPI.update(editingId, payload);
      } else {
        await publishersAPI.create(payload);
      }

      resetForm();
      loadPublishers(currentPage, searchTerm);
    } catch (err) {
      console.error('Error saving publisher', err);
      setError(err.response?.data?.message || 'Error saving publisher');
    }
  };

  const handleEdit = (publisher) => {
    setEditingId(publisher.id);
    setFormData({
      name: publisher.name,
      address: publisher.address || '',
      phone: publisher.phone || '',
      email: publisher.email || '',
    });
    setError('');
  };

  const handleDelete = async (id) => {
    if (!window.confirm('A jeni të sigurt që dëshironi të fshini këtë botues?')) return;
    try {
      setError('');
      await publishersAPI.delete(id);
      loadPublishers(currentPage, searchTerm);
    } catch (err) {
      console.error('Error deleting publisher', err);
      setError(err.response?.data?.message || 'Error deleting publisher');
      alert(err.response?.data?.message || 'Error deleting publisher');
    }
  };

  const exportToCSV = () => {
    const headers = ['Name', 'Address', 'Phone', 'Email', 'Book Count'];
    const rows = publishers.map((p) => [
      p.name,
      p.address,
      p.phone,
      p.email,
      p.bookCount || 0,
    ]);

    const csvContent = [
      headers.join(','),
      ...rows.map((row) => row.map((cell) => `"${cell}"`).join(',')),
    ].join('\n');

    const blob = new Blob([csvContent], { type: 'text/csv;charset=utf-8;' });
    const link = document.createElement('a');
    const url = URL.createObjectURL(blob);
    link.setAttribute('href', url);
    link.setAttribute('download', `publishers_${new Date().toISOString().split('T')[0]}.csv`);
    link.style.visibility = 'hidden';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  };

  if (loading && publishers.length === 0) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <h1>Publishers Management</h1>
        <div>
          <button onClick={exportToCSV} className="btn" style={{ marginRight: '1rem' }}>
            Export to CSV
          </button>
        </div>
      </div>

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
        <h2>{editingId ? 'Edit Publisher' : 'Add New Publisher'}</h2>
        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <div className="form-group" style={{ flex: 2 }}>
              <label>Name *</label>
              <input
                type="text"
                name="name"
                value={formData.name}
                onChange={handleChange}
                required
                placeholder="Emri i botuesit"
              />
            </div>
            <div className="form-group" style={{ flex: 2 }}>
              <label>Email *</label>
              <input
                type="email"
                name="email"
                value={formData.email}
                onChange={handleChange}
                required
                placeholder="email@example.com"
              />
            </div>
          </div>

          <div className="form-row">
            <div className="form-group" style={{ flex: 2 }}>
              <label>Phone *</label>
              <input
                type="tel"
                name="phone"
                value={formData.phone}
                onChange={handleChange}
                required
                placeholder="+355 69 1234567"
              />
            </div>
            <div className="form-group" style={{ flex: 3 }}>
              <label>Address *</label>
              <input
                type="text"
                name="address"
                value={formData.address}
                onChange={handleChange}
                required
                placeholder="Adresa e plotë"
              />
            </div>
          </div>

          <div className="form-actions">
            <button type="submit" className="btn btn-primary">
              {editingId ? 'Update Publisher' : 'Add Publisher'}
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
        <div style={{ marginBottom: '1rem', display: 'flex', justifyContent: 'space-between', alignItems: 'center' }}>
          <div style={{ flex: 1, maxWidth: '300px' }}>
            <input
              type="text"
              placeholder="Search by name..."
              value={searchTerm}
              onChange={handleSearchChange}
              style={{
                width: '100%',
                padding: '0.5rem',
                border: '1px solid #ddd',
                borderRadius: '4px',
              }}
            />
          </div>
          <div style={{ fontSize: '0.9rem', color: '#666' }}>
            Total: {publishers.length} publisher(s)
          </div>
        </div>

        <div style={{ overflowX: 'auto' }}>
          <table className="table">
            <thead>
              <tr>
                <th>Name</th>
                <th>Address</th>
                <th>Phone</th>
                <th>Email</th>
                <th>Books</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {publishers.length === 0 ? (
                <tr>
                  <td colSpan="6" style={{ textAlign: 'center' }}>
                    {loading ? 'Loading...' : 'No publishers found'}
                  </td>
                </tr>
              ) : (
                publishers.map((p) => (
                  <tr key={p.id}>
                    <td>{p.name}</td>
                    <td>{p.address}</td>
                    <td>{p.phone}</td>
                    <td>{p.email}</td>
                    <td>
                      <span style={{
                        display: 'inline-block',
                        padding: '0.25rem 0.5rem',
                        borderRadius: '12px',
                        backgroundColor: p.bookCount > 0 ? '#e8f5e9' : '#f5f5f5',
                        color: p.bookCount > 0 ? '#2e7d32' : '#666',
                        fontSize: '0.85rem',
                        fontWeight: '500',
                      }}>
                        {p.bookCount || 0}
                      </span>
                    </td>
                    <td>
                      <button
                        className="btn"
                        style={{ marginRight: '0.5rem' }}
                        onClick={() => handleEdit(p)}
                      >
                        Edit
                      </button>
                      <button
                        className="btn btn-danger"
                        onClick={() => handleDelete(p.id)}
                        disabled={p.bookCount > 0}
                        title={p.bookCount > 0 ? 'Cannot delete publisher with books' : 'Delete publisher'}
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

        {totalPages > 1 && (
          <div style={{ 
            marginTop: '1rem', 
            display: 'flex', 
            justifyContent: 'center', 
            alignItems: 'center',
            gap: '0.5rem'
          }}>
            <button
              className="btn"
              onClick={() => setCurrentPage((prev) => Math.max(1, prev - 1))}
              disabled={currentPage === 1}
            >
              Previous
            </button>
            <span>
              Page {currentPage} of {totalPages}
            </span>
            <button
              className="btn"
              onClick={() => setCurrentPage((prev) => Math.min(totalPages, prev + 1))}
              disabled={currentPage === totalPages}
            >
              Next
            </button>
          </div>
        )}
      </div>
    </div>
  );
}

export default PublishersList;

