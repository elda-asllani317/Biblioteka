import React, { useEffect, useState } from 'react';
import { categoriesAPI } from '../services/api';
import './BooksList.css';

function CategoriesList() {
  const [categories, setCategories] = useState([]);
  const [loading, setLoading] = useState(true);
  const [editingId, setEditingId] = useState(null);
  const [formData, setFormData] = useState({
    name: '',
    description: '',
  });

  const loadCategories = async () => {
    try {
      setLoading(true);
      const res = await categoriesAPI.getAll();
      setCategories(res.data);
    } catch (err) {
      console.error('Error loading categories', err);
      alert('Error loading categories');
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    loadCategories();
  }, []);

  const handleChange = (e) => {
    const { name, value } = e.target;
    setFormData((prev) => ({ ...prev, [name]: value }));
  };

  const resetForm = () => {
    setEditingId(null);
    setFormData({
      name: '',
      description: '',
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    try {
      const payload = {
        name: formData.name,
        description: formData.description,
      };

      if (editingId) {
        await categoriesAPI.update(editingId, payload);
      } else {
        await categoriesAPI.create(payload);
      }

      resetForm();
      loadCategories();
    } catch (err) {
      console.error('Error saving category', err);
      alert('Error saving category');
    }
  };

  const handleEdit = (category) => {
    setEditingId(category.id);
    setFormData({
      name: category.name,
      description: category.description || '',
    });
  };

  const handleDelete = async (id) => {
    if (!window.confirm('Are you sure you want to delete this category?')) return;
    try {
      await categoriesAPI.delete(id);
      loadCategories();
    } catch (err) {
      console.error('Error deleting category', err);
      alert('Error deleting category');
    }
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div>
      <h1>Categories Management</h1>

      <div className="card" style={{ marginBottom: '1.5rem' }}>
        <form onSubmit={handleSubmit}>
          <div className="form-row">
            <div className="form-group">
              <label>Name *</label>
              <input
                type="text"
                name="name"
                value={formData.name}
                onChange={handleChange}
                required
              />
            </div>
            <div className="form-group" style={{ flex: 2 }}>
              <label>Description</label>
              <textarea
                name="description"
                value={formData.description}
                onChange={handleChange}
              />
            </div>
          </div>

          <div className="form-actions">
            <button type="submit" className="btn btn-primary">
              {editingId ? 'Update Category' : 'Add Category'}
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
              <th>Name</th>
              <th>Description</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {categories.length === 0 ? (
              <tr>
                <td colSpan="3" style={{ textAlign: 'center' }}>No categories found</td>
              </tr>
            ) : (
              categories.map((c) => (
                <tr key={c.id}>
                  <td>{c.name}</td>
                  <td>{c.description}</td>
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

export default CategoriesList;


