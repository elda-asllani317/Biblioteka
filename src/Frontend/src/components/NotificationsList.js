import React, { useEffect, useState } from 'react';
import { notificationsAPI, usersAPI } from '../services/api';
import { useAuth } from '../contexts/AuthContext';
import './BooksList.css';

function NotificationsList() {
  const { user } = useAuth();
  const [notifications, setNotifications] = useState([]);
  const [users, setUsers] = useState([]);
  const [loading, setLoading] = useState(true);
  const [editingId, setEditingId] = useState(null);
  const [filterType, setFilterType] = useState('all'); // all, unread, Info, Warning, Error
  const [selectedUserId, setSelectedUserId] = useState(null);
  const [error, setError] = useState('');
  const [formData, setFormData] = useState({
    title: '',
    message: '',
    type: 'Info',
    userId: user?.id || null,
  });

  useEffect(() => {
    loadUsers();
    if (user?.id) {
      setSelectedUserId(user.id);
      loadNotifications(user.id);
    }
  }, [user]);

  useEffect(() => {
    if (selectedUserId) {
      loadNotifications(selectedUserId);
    }
  }, [selectedUserId, filterType]);

  const loadUsers = async () => {
    try {
      const res = await usersAPI.getAll();
      setUsers(res.data);
    } catch (err) {
      console.error('Error loading users', err);
    }
  };

  const loadNotifications = async (userId) => {
    try {
      setLoading(true);
      setError('');
      const unreadOnly = filterType === 'unread';
      const res = await notificationsAPI.getByUser(userId, unreadOnly);
      let filtered = res.data;

      // Apply type filter if not 'all' or 'unread'
      if (filterType !== 'all' && filterType !== 'unread') {
        filtered = filtered.filter(n => n.type === filterType);
      }

      setNotifications(filtered);
    } catch (err) {
      console.error('Error loading notifications', err);
      setError(err.response?.data?.message || 'Error loading notifications');
    } finally {
      setLoading(false);
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
      title: '',
      message: '',
      type: 'Info',
      userId: user?.id || null,
    });
    setError('');
  };

  const validateForm = () => {
    if (!formData.title.trim()) {
      setError('Title është i detyrueshëm');
      return false;
    }
    if (!formData.message.trim()) {
      setError('Message është i detyrueshëm');
      return false;
    }
    if (!formData.userId) {
      setError('UserId është i detyrueshëm');
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
        title: formData.title.trim(),
        message: formData.message.trim(),
        type: formData.type,
        userId: formData.userId,
      };

      if (editingId) {
        await notificationsAPI.update(editingId, payload);
      } else {
        await notificationsAPI.create(payload);
      }

      resetForm();
      if (selectedUserId) {
        loadNotifications(selectedUserId);
      }
    } catch (err) {
      console.error('Error saving notification', err);
      setError(err.response?.data?.message || 'Error saving notification');
    }
  };

  const handleEdit = (notification) => {
    setEditingId(notification.id);
    setFormData({
      title: notification.title,
      message: notification.message,
      type: notification.type,
      userId: notification.userId,
    });
    setError('');
  };

  const handleMarkAsRead = async (id) => {
    try {
      setError('');
      await notificationsAPI.markAsRead(id);
      if (selectedUserId) {
        loadNotifications(selectedUserId);
      }
    } catch (err) {
      console.error('Error marking as read', err);
      setError(err.response?.data?.message || 'Error marking as read');
    }
  };

  const handleDelete = async (id) => {
    if (!window.confirm('A jeni të sigurt që dëshironi të fshini këtë njoftim?')) return;
    try {
      setError('');
      await notificationsAPI.delete(id);
      if (selectedUserId) {
        loadNotifications(selectedUserId);
      }
    } catch (err) {
      console.error('Error deleting notification', err);
      setError(err.response?.data?.message || 'Error deleting notification');
    }
  };

  const getTypeColor = (type) => {
    switch (type) {
      case 'Info':
        return '#3498db';
      case 'Warning':
        return '#f39c12';
      case 'Error':
        return '#e74c3c';
      default:
        return '#95a5a6';
    }
  };

  if (loading && notifications.length === 0) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div>
      <h1>Notifications Management</h1>

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
        <h2>{editingId ? 'Edit Notification' : 'Add New Notification'}</h2>
        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <div className="form-group" style={{ flex: 2 }}>
              <label>Title *</label>
              <input
                type="text"
                name="title"
                value={formData.title}
                onChange={handleChange}
                required
                placeholder="Titulli i njoftimit"
              />
            </div>
            <div className="form-group">
              <label>Type *</label>
              <select
                name="type"
                value={formData.type}
                onChange={handleChange}
                required
                style={{ width: '100%', padding: '0.5rem', border: '1px solid #ddd', borderRadius: '4px' }}
              >
                <option value="Info">Info</option>
                <option value="Warning">Warning</option>
                <option value="Error">Error</option>
              </select>
            </div>
          </div>

          <div className="form-row">
            <div className="form-group" style={{ flex: 1 }}>
              <label>Message *</label>
              <textarea
                name="message"
                value={formData.message}
                onChange={handleChange}
                required
                rows="3"
                placeholder="Mesazhi i njoftimit"
              />
            </div>
          </div>

          <div className="form-row">
            <div className="form-group" style={{ flex: 1 }}>
              <label>User *</label>
              <select
                name="userId"
                value={formData.userId || ''}
                onChange={(e) => setFormData(prev => ({ ...prev, userId: parseInt(e.target.value) }))}
                required
                style={{ width: '100%', padding: '0.5rem', border: '1px solid #ddd', borderRadius: '4px' }}
              >
                <option value="">Select User</option>
                {users.map((u) => (
                  <option key={u.id} value={u.id}>
                    {u.firstName} {u.lastName} ({u.email})
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="form-actions">
            <button type="submit" className="btn btn-primary">
              {editingId ? 'Update Notification' : 'Add Notification'}
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
        <div style={{ marginBottom: '1rem', display: 'flex', gap: '1rem', alignItems: 'center', flexWrap: 'wrap' }}>
          <div style={{ flex: 1, minWidth: '200px' }}>
            <label style={{ display: 'block', marginBottom: '0.5rem' }}>Select User:</label>
            <select
              value={selectedUserId || ''}
              onChange={(e) => setSelectedUserId(parseInt(e.target.value))}
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
          <div style={{ flex: 1, minWidth: '200px' }}>
            <label style={{ display: 'block', marginBottom: '0.5rem' }}>Filter:</label>
            <select
              value={filterType}
              onChange={(e) => setFilterType(e.target.value)}
              style={{ width: '100%', padding: '0.5rem', border: '1px solid #ddd', borderRadius: '4px' }}
            >
              <option value="all">All</option>
              <option value="unread">Unread Only</option>
              <option value="Info">Info</option>
              <option value="Warning">Warning</option>
              <option value="Error">Error</option>
            </select>
          </div>
        </div>

        <div style={{ overflowX: 'auto' }}>
          <table className="table">
            <thead>
              <tr>
                <th>Title</th>
                <th>Message</th>
                <th>Type</th>
                <th>User</th>
                <th>Date</th>
                <th>Status</th>
                <th>Actions</th>
              </tr>
            </thead>
            <tbody>
              {notifications.length === 0 ? (
                <tr>
                  <td colSpan="7" style={{ textAlign: 'center' }}>
                    {loading ? 'Loading...' : 'No notifications found'}
                  </td>
                </tr>
              ) : (
                notifications.map((n) => (
                  <tr key={n.id} style={{ backgroundColor: n.isRead ? '#f9f9f9' : '#fff' }}>
                    <td>{n.title}</td>
                    <td style={{ maxWidth: '300px', overflow: 'hidden', textOverflow: 'ellipsis', whiteSpace: 'nowrap' }}>
                      {n.message}
                    </td>
                    <td>
                      <span style={{
                        display: 'inline-block',
                        padding: '0.25rem 0.5rem',
                        borderRadius: '4px',
                        backgroundColor: getTypeColor(n.type),
                        color: 'white',
                        fontSize: '0.85rem',
                        fontWeight: '500',
                      }}>
                        {n.type}
                      </span>
                    </td>
                    <td>{n.userName}</td>
                    <td>{new Date(n.createdDate).toLocaleDateString()}</td>
                    <td>
                      {n.isRead ? (
                        <span style={{ color: '#27ae60' }}>✓ Read</span>
                      ) : (
                        <span style={{ color: '#e74c3c', fontWeight: 'bold' }}>● Unread</span>
                      )}
                    </td>
                    <td>
                      {!n.isRead && (
                        <button
                          className="btn"
                          style={{ marginRight: '0.5rem', fontSize: '0.85rem' }}
                          onClick={() => handleMarkAsRead(n.id)}
                        >
                          Mark Read
                        </button>
                      )}
                      <button
                        className="btn"
                        style={{ marginRight: '0.5rem' }}
                        onClick={() => handleEdit(n)}
                      >
                        Edit
                      </button>
                      <button
                        className="btn btn-danger"
                        onClick={() => handleDelete(n.id)}
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
    </div>
  );
}

export default NotificationsList;

