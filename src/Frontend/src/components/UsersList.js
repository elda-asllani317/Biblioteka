import React, { useState, useEffect } from 'react';
import { usersAPI } from '../services/api';
import './UsersList.css';

function UsersList() {
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showForm, setShowForm] = useState(false);
  const [editingUser, setEditingUser] = useState(null);

  useEffect(() => {
    loadUsers();
  }, []);

  const loadUsers = async () => {
    try {
      setLoading(true);
      const response = await usersAPI.getAll();
      setUsers(response.data);
    } catch (error) {
      console.error('Error loading users:', error);
      alert('Error loading users');
    } finally {
      setLoading(false);
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Jeni të sigurt që dëshironi të fshini këtë përdorues?')) {
      return;
    }

    try {
      await usersAPI.delete(id);
      loadUsers();
    } catch (error) {
      console.error('Error deleting user:', error);
      alert('Error deleting user');
    }
  };

  const handleEdit = (user) => {
    setEditingUser(user);
    setShowForm(true);
  };

  const handleFormClose = () => {
    setShowForm(false);
    setEditingUser(null);
    loadUsers();
  };

  if (loading) {
    return <div className="loading">Duke u ngarkuar...</div>;
  }

  return (
    <div>
      <div className="page-header">
        <h1>Menaxhimi i Përdoruesve</h1>
        <button className="btn btn-primary" onClick={() => setShowForm(true)}>
          + Shto Përdorues të Ri
        </button>
      </div>

      {showForm && (
        <UserForm
          user={editingUser}
          onClose={handleFormClose}
        />
      )}

      <div className="card">
        <table className="table">
          <thead>
            <tr>
              <th>ID</th>
              <th>Emri</th>
              <th>Mbiemri</th>
              <th>Email</th>
              <th>Telefoni</th>
              <th>Adresa</th>
              <th>Data Regjistrimit</th>
              <th>Aktiv</th>
              <th>Veprime</th>
            </tr>
          </thead>
          <tbody>
            {users.length === 0 ? (
              <tr>
                <td colSpan="9" style={{ textAlign: 'center' }}>Nuk ka përdorues</td>
              </tr>
            ) : (
              users.map((user) => (
                <tr key={user.id}>
                  <td>{user.id}</td>
                  <td>{user.firstName}</td>
                  <td>{user.lastName}</td>
                  <td>{user.email}</td>
                  <td>{user.phone}</td>
                  <td>{user.address}</td>
                  <td>{new Date(user.registrationDate).toLocaleDateString()}</td>
                  <td>{user.isActive ? '✅' : '❌'}</td>
                  <td>
                    <button
                      className="btn btn-primary"
                      onClick={() => handleEdit(user)}
                      style={{ marginRight: '0.5rem' }}
                    >
                      Edit
                    </button>
                    <button
                      className="btn btn-danger"
                      onClick={() => handleDelete(user.id)}
                    >
                      Fshi
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

function UserForm({ user, onClose }) {
  const [formData, setFormData] = useState({
    firstName: user?.firstName || '',
    lastName: user?.lastName || '',
    email: user?.email || '',
    password: '',
    phone: user?.phone || '',
    address: user?.address || ''
  });
  const [loading, setLoading] = useState(false);
  const [error, setError] = useState('');

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    try {
      if (user) {
        // Update existing user
        await usersAPI.update(user.id, {
          firstName: formData.firstName,
          lastName: formData.lastName,
          phone: formData.phone,
          address: formData.address,
          password: formData.password || undefined
        });
      } else {
        // Create new user
        await usersAPI.create(formData);
      }
      onClose();
    } catch (error) {
      console.error('Error saving user:', error);
      setError(error.response?.data?.message || 'Gabim në ruajtjen e përdoruesit');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div className="modal-overlay" onClick={onClose}>
      <div className="modal-content" onClick={(e) => e.stopPropagation()}>
        <div className="modal-header">
          <h2>{user ? 'Përditëso Përdoruesin' : 'Shto Përdorues të Ri'}</h2>
          <button className="btn-close" onClick={onClose}>×</button>
        </div>

        {error && <div className="error-message">{error}</div>}

        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <div className="form-group">
              <label>Emri *</label>
              <input
                type="text"
                name="firstName"
                value={formData.firstName}
                onChange={handleChange}
                required
              />
            </div>

            <div className="form-group">
              <label>Mbiemri *</label>
              <input
                type="text"
                name="lastName"
                value={formData.lastName}
                onChange={handleChange}
                required
              />
            </div>
          </div>

          <div className="form-group">
            <label>Email *</label>
            <input
              type="email"
              name="email"
              value={formData.email}
              onChange={handleChange}
              required
              disabled={!!user}
            />
          </div>

          <div className="form-group">
            <label>Password {user ? '(lëreni bosh për të mos e ndryshuar)' : '*'}</label>
            <input
              type="password"
              name="password"
              value={formData.password}
              onChange={handleChange}
              required={!user}
              minLength={user ? undefined : 6}
            />
          </div>

          <div className="form-group">
            <label>Telefoni</label>
            <input
              type="tel"
              name="phone"
              value={formData.phone}
              onChange={handleChange}
            />
          </div>

          <div className="form-group">
            <label>Adresa</label>
            <input
              type="text"
              name="address"
              value={formData.address}
              onChange={handleChange}
            />
          </div>

          <div className="form-actions">
            <button type="submit" className="btn btn-primary" disabled={loading}>
              {loading ? 'Duke u ruajtur...' : 'Ruaj'}
            </button>
            <button type="button" className="btn" onClick={onClose} style={{ background: '#95a5a6', color: 'white' }}>
              Anulo
            </button>
          </div>
        </form>
      </div>
    </div>
  );
}

export default UsersList;

