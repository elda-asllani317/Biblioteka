import React, { useState, useEffect } from 'react';
import { useNavigate, Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import './Login.css';

function Login() {
  const [email, setEmail] = useState('');
  const [password, setPassword] = useState('');
  const [error, setError] = useState('');
  const [loading, setLoading] = useState(false);
  const { login, isAuthenticated } = useAuth();
  const navigate = useNavigate();

  // Redirect if already logged in
  useEffect(() => {
    if (isAuthenticated) {
      navigate('/dashboard');
    }
  }, [isAuthenticated, navigate]);

  const handleSubmit = async (e) => {
    e.preventDefault();
    setError('');
    setLoading(true);

    const result = await login(email, password);
    
    if (result.success) {
      // Redirect to dashboard after successful login
      navigate('/dashboard');
    } else {
      setError(result.message);
    }
    
    setLoading(false);
  };

  return (
    <div className="login-container">
      <div className="login-card">
        <h1>📚 Biblioteka</h1>
        <h2>Hyr në Sistem</h2>
        <div style={{ 
          fontSize: '0.85rem', 
          color: '#666', 
          marginBottom: '1rem', 
          textAlign: 'center',
          padding: '0.75rem',
          backgroundColor: '#f8f9fa',
          borderRadius: '6px'
        }}>
          <strong>Test Credentials:</strong><br />
          Admin: admin / admin<br />
          User: john.doe@example.com / password123
        </div>
        
        {error && <div className="error-message">{error}</div>}
        
        <form onSubmit={handleSubmit}>
          <div className="form-group">
            <label>Email ose Username</label>
            <input
              type="text"
              value={email}
              onChange={(e) => setEmail(e.target.value)}
              required
              placeholder="shkruani email-in ose username"
            />
          </div>

          <div className="form-group">
            <label>Password</label>
            <input
              type="password"
              value={password}
              onChange={(e) => setPassword(e.target.value)}
              required
              placeholder="shkruani password-in"
            />
          </div>

          <button type="submit" className="btn btn-primary btn-block" disabled={loading}>
            {loading ? 'Duke u kyçur...' : 'Kyçu'}
          </button>
        </form>

        <div className="login-footer">
          <p>Nuk keni llogari? <Link to="/register">Regjistrohuni këtu</Link></p>
        </div>
      </div>
    </div>
  );
}

export default Login;

