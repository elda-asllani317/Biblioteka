import React, { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import './Register.css';

function Register() {
  const [formData, setFormData] = useState({
    firstName: '',
    lastName: '',
    email: '',
    password: '',
    confirmPassword: '',
    phone: '',
    address: ''
  });
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { register, isAuthenticated } = useAuth();
  const navigate = useNavigate();

  // Redirect if already logged in
  useEffect(() => {
    if (isAuthenticated) {
      navigate('/dashboard');
    }
  }, [isAuthenticated, navigate]);

  const handleChange = (e) => {
    setFormData({
      ...formData,
      [e.target.name]: e.target.value
    });
  };

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');

    if (formData.password !== formData.confirmPassword) {
      setError('Password-et nuk përputhen');
      return;
    }

    if (formData.password.length < 6) {
      setError('Password-i duhet të jetë të paktën 6 karaktere');
      return;
    }

    setLoading(true);

    const result = await register({
      firstName: formData.firstName,
      lastName: formData.lastName,
      email: formData.email,
      password: formData.password,
      phone: formData.phone,
      address: formData.address
    });

    if (result.success) {
      // Redirect to dashboard after successful registration
      navigate('/dashboard');
    } else {
      setError(result.message);
    }

    setLoading(false);
  };

  return (
    <div className="register-container">
      <div className="register-card">
        <h1>📚 Biblioteka</h1>
        <h2>Regjistrohu</h2>
        <div style={{ 
          fontSize: '0.85rem', 
          color: '#666', 
          marginBottom: '1rem', 
          textAlign: 'center',
          padding: '0.75rem',
          backgroundColor: '#e8f4f8',
          borderRadius: '6px',
          border: '1px solid #bee5eb'
        }}>
          <strong>ℹ️ Informacion:</strong><br />
          Regjistrimi krijon një llogari <strong>Përdorues</strong>. Vetëm administratori mund të menaxhojë sistemin plotësisht.
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
                placeholder="Emri"
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
                placeholder="Mbiemri"
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
              placeholder="email@example.com"
            />
          </div>

          <div className="form-row">
            <div className="form-group">
              <label>Password *</label>
              <input
                type="password"
                name="password"
                value={formData.password}
                onChange={handleChange}
                required
                placeholder="Minimum 6 karaktere"
                minLength="6"
              />
            </div>

            <div className="form-group">
              <label>Konfirmo Password *</label>
              <input
                type="password"
                name="confirmPassword"
                value={formData.confirmPassword}
                onChange={handleChange}
                required
                placeholder="Konfirmo password-in"
                minLength="6"
              />
            </div>
          </div>

          <div className="form-group">
            <label>Telefoni</label>
            <input
              type="tel"
              name="phone"
              value={formData.phone}
              onChange={handleChange}
              placeholder="+355 69 1234567"
            />
          </div>

          <div className="form-group">
            <label>Adresa</label>
            <input
              type="text"
              name="address"
              value={formData.address}
              onChange={handleChange}
              placeholder="Adresa e plotë"
            />
          </div>

          <button type="submit" className="btn btn-primary btn-block" disabled={loading}>
            {loading ? 'Duke u regjistruar...' : 'Regjistrohu'}
          </button>
        </form>

        <div className="register-footer">
          <p>Keni tashmë llogari? <Link to="/login">Kyçuni këtu</Link></p>
        </div>
      </div>
    </div>
  );
}

export default Register;

