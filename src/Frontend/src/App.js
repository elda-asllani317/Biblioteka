import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import Login from './components/Login';
import './App.css';
import BooksList from './components/BooksList';
import BookForm from './components/BookForm';
import LoansList from './components/LoansList';
import CreateLoan from './components/CreateLoan';

function Navbar() {
  const { user, logout, isAuthenticated } = useAuth();

  if (!isAuthenticated) {
    return null;
  }

  return (
    <nav className="navbar">
      <div className="nav-container">
        <h1 className="nav-logo">📚 Biblioteka</h1>
        <ul className="nav-menu">
          <li><Link to="/books">Books</Link></li>
          <li><Link to="/books/new">Add Book</Link></li>
          <li><Link to="/loans">Loans</Link></li>
          <li><Link to="/loans/new">New Loan</Link></li>
          <li className="user-info">
            <span>{user?.firstName} {user?.lastName}</span>
            <button onClick={logout} className="btn-logout">Dil</button>
          </li>
        </ul>
      </div>
    </nav>
  );
}

function AppContent() {
  return (
    <div className="App">
      <Navbar />
      <main className="main-content">
        <Routes>
          <Route path="/login" element={<Login />} />
          <Route
            path="/books"
            element={
              <ProtectedRoute>
                <BooksList />
              </ProtectedRoute>
            }
          />
          <Route
            path="/books/new"
            element={
              <ProtectedRoute>
                <BookForm />
              </ProtectedRoute>
            }
          />
          <Route
            path="/loans"
            element={
              <ProtectedRoute>
                <LoansList />
              </ProtectedRoute>
            }
          />
          <Route
            path="/loans/new"
            element={
              <ProtectedRoute>
                <CreateLoan />
              </ProtectedRoute>
            }
          />
          <Route path="/" element={<Navigate to="/books" replace />} />
        </Routes>
      </main>
    </div>
  );
}

function App() {
  return (
    <Router>
      <AuthProvider>
        <AppContent />
      </AuthProvider>
    </Router>
  );
}

export default App;

