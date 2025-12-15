import React, { useState, useEffect } from 'react';
import { useNavigate } from 'react-router-dom';
import { booksAPI } from '../services/api';
import './BookForm.css';

function BookForm() {
  const navigate = useNavigate();
  const [formData, setFormData] = useState({
    title: '',
    isbn: '',
    description: '',
    publicationYear: new Date().getFullYear(),
    pages: 0,
    language: 'Albanian',
    authorId: '',
    categoryId: '',
    publisherId: '',
  });

  const [authors, setAuthors] = useState([]);
  const [categories, setCategories] = useState([]);
  const [publishers, setPublishers] = useState([]);
  const [loading, setLoading] = useState(false);

  useEffect(() => {
    // In a real app, you would fetch these from API
    // For now, we'll use placeholder data
    setAuthors([
      { id: 1, firstName: 'Ismail', lastName: 'Kadare' },
      { id: 2, firstName: 'Dritëro', lastName: 'Agolli' },
    ]);
    setCategories([
      { id: 1, name: 'Fiction' },
      { id: 2, name: 'Non-Fiction' },
      { id: 3, name: 'Science' },
    ]);
    setPublishers([
      { id: 1, name: 'Onufri' },
      { id: 2, name: 'Toena' },
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
      await booksAPI.create({
        title: formData.title,
        isbn: formData.isbn,
        description: formData.description,
        publicationYear: parseInt(formData.publicationYear),
        pages: parseInt(formData.pages),
        language: formData.language,
        authorId: parseInt(formData.authorId),
        categoryId: parseInt(formData.categoryId),
        publisherId: parseInt(formData.publisherId),
      });
      navigate('/books');
    } catch (error) {
      console.error('Error creating book:', error);
      alert('Error creating book. Please check all fields are filled correctly.');
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <h1>Add New Book</h1>
      <div className="card">
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Title *</label>
            <input
              type="text"
              name="title"
              value={formData.title}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>ISBN *</label>
            <input
              type="text"
              name="isbn"
              value={formData.isbn}
              onChange={handleChange}
              required
            />
          </div>

          <div className="form-group">
            <label>Description</label>
            <textarea
              name="description"
              value={formData.description}
              onChange={handleChange}
            />
          </div>

          <div className="form-row">
            <div className="form-group">
              <label>Publication Year *</label>
              <input
                type="number"
                name="publicationYear"
                value={formData.publicationYear}
                onChange={handleChange}
                required
              />
            </div>

            <div className="form-group">
              <label>Pages *</label>
              <input
                type="number"
                name="pages"
                value={formData.pages}
                onChange={handleChange}
                required
              />
            </div>

            <div className="form-group">
              <label>Language *</label>
              <input
                type="text"
                name="language"
                value={formData.language}
                onChange={handleChange}
                required
              />
            </div>
          </div>

          <div className="form-row">
            <div className="form-group">
              <label>Author *</label>
              <select
                name="authorId"
                value={formData.authorId}
                onChange={handleChange}
                required
              >
                <option value="">Select Author</option>
                {authors.map((author) => (
                  <option key={author.id} value={author.id}>
                    {author.firstName} {author.lastName}
                  </option>
                ))}
              </select>
            </div>

            <div className="form-group">
              <label>Category *</label>
              <select
                name="categoryId"
                value={formData.categoryId}
                onChange={handleChange}
                required
              >
                <option value="">Select Category</option>
                {categories.map((category) => (
                  <option key={category.id} value={category.id}>
                    {category.name}
                  </option>
                ))}
              </select>
            </div>

            <div className="form-group">
              <label>Publisher *</label>
              <select
                name="publisherId"
                value={formData.publisherId}
                onChange={handleChange}
                required
              >
                <option value="">Select Publisher</option>
                {publishers.map((publisher) => (
                  <option key={publisher.id} value={publisher.id}>
                    {publisher.name}
                  </option>
                ))}
              </select>
            </div>
          </div>

          <div className="form-actions">
            <button type="submit" className="btn btn-primary" disabled={loading}>
              {loading ? 'Creating...' : 'Create Book'}
            </button>
            <button
              type="button"
              className="btn"
              onClick={() => navigate('/books')}
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

export default BookForm;

