import React, { useEffect, useState } from 'react';
import { authorsAPI } from '../services/api';
import './BooksList.css';

function AuthorsList() {
  const [authors, setAuthors] = useState([]);
  const [loading, setLoading] = useState(true);
  const [editingId, setEditingId] = useState(null);
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    biography: '',
    dateOfBirth: '',
    nationality: '',
  });

  const loadAuthors = async () => {
    try {
      setLoading(true);
      const res = await authorsAPI.getAll();
      setAuthors(res.data);
    } catch (err) {
      console.error('Error loading authors', err);
      alert('Error loading authors');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadAuthors();
  }, []);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const resetForm = () => {
    setEditingId(null);
    setFormData({
      firstName: '',
      lastName: '',
      biography: '',
      dateOfBirth: '',
      nationality: '',
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const payload = {
        firstName: formData.firstName,
        lastName: formData.lastName,
        biography: formData.biography,
        nationality: formData.nationality,
        dateOfBirth: formData.dateOfBirth ? new Date(formData.dateOfBirth).toISOString() : null,
      };

      if (editingId) {
        await authorsAPI.update(editingId, payload);
      } else {
        await authorsAPI.create(payload);
      }

      resetForm();
      loadAuthors();
    } catch (err) {
      console.error('Error saving author', err);
      alert('Error saving author');
    }
  };

  const handleEdit = (author) => {
    setEditingId(author.id);
    setFormData({
      firstName: author.firstName,
      lastName: author.lastName,
      biography: author.biography || '',
      nationality: author.nationality || '',
      dateOfBirth: author.dateOfBirth ? author.dateOfBirth.substring(0, 10) : '',
    });
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this author?')) return;
    try {
      await authorsAPI.delete(id);
      loadAuthors();
    } catch (err) {
      console.error('Error deleting author', err);
      alert('Error deleting author');
    }
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div>
      <h1>Authors Management</h1>

      <div className="card" style={{ marginBottom: '1.5rem' }}>
        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <div className="form-group">
              <label>First Name *</label>
              <input
                type="text"
                name="firstName"
                value={formData.firstName}
                onChange={handleChange}
                required
              />
            </div>
            <div className="form-group">
              <label>Last Name *</label>
              <input
                type="text"
                name="lastName"
                value={formData.lastName}
                onChange={handleChange}
                required
              />
            </div>
            <div className="form-group">
              <label>Nationality</label>
              <input
                type="text"
                name="nationality"
                value={formData.nationality}
                onChange={handleChange}
              />
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label>Date of Birth</label>
              <input
                type="date"
                name="dateOfBirth"
                value={formData.dateOfBirth}
                onChange={handleChange}
              />
            </div>
            <div className="form-group" style={{ flex: 2 }}>
              <label>Biography</label>
              <textarea
                name="biography"
                value={formData.biography}
                onChange={handleChange}
              />
            </div>
          </div>

          <div className="form-actions">
            <button type="submit" className="btn btn-primary">
              {editingId ? 'Update Author' : 'Add Author'}
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
              <th>First Name</th>
              <th>Last Name</th>
              <th>Nationality</th>
              <th>Date of Birth</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {authors.length === 0 ? (
              <tr>
                <td colSpan="5" style={{ textAlign: 'center' }}>No authors found</td>
              </tr>
            ) : (
              authors.map((a) => (
                <tr key={a.id}>
                  <td>{a.firstName}</td>
                  <td>{a.lastName}</td>
                  <td>{a.nationality}</td>
                  <td>{a.dateOfBirth ? a.dateOfBirth.substring(0, 10) : ''}</td>
                  <td>
                    <button
                      className="btn"
                      style={{ marginRight: '0.5rem' }}
                      onClick={() => handleEdit(a)}
                    >
                      Edit
                    </button>
                    <button
                      className="btn btn-danger"
                      onClick={() => handleDelete(a.id)}
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

export default AuthorsList;


