import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link, Navigate } from 'react-router-dom';
import { AuthProvider, useAuth } from './contexts/AuthContext';
import ProtectedRoute from './components/ProtectedRoute';
import AdminRoute from './components/AdminRoute';
import Login from './components/Login';
import './App.css';
import BooksList from './components/BooksList';
import BookForm from './components/BookForm';
import LoansList from './components/LoansList';
import CreateLoan from './components/CreateLoan';
import Register from './components/Register';
import UsersList from './components/UsersList';
import AuthorsList from './components/AuthorsList';
import CategoriesList from './components/CategoriesList';
import PublishersList from './components/PublishersList';
import BookCopiesList from './components/BookCopiesList';
import FinesList from './components/FinesList';
import NotificationsList from './components/NotificationsList';
import ReviewsList from './components/ReviewsList';
import Dashboard from './components/Dashboard';

function Navbar() {
  const { user, logout, isAuthenticated, isAdmin } = useAuth();

  if (!isAuthenticated) {
    return null;
  }

  return (
    <nav className="navbar">
      <div className="nav-container">
        <h1 className="nav-logo">📚 Biblioteka</h1>
        <ul className="nav-menu">
          <li><Link to="/dashboard">Dashboard</Link></li>
          <li><Link to="/books">Books</Link></li>
          {isAdmin && <li><Link to="/books/new">Add Book</Link></li>}
          {isAdmin && <li><Link to="/bookcopies">Book Copies</Link></li>}
          {isAdmin && <li><Link to="/authors">Authors</Link></li>}
          {isAdmin && <li><Link to="/categories">Categories</Link></li>}
          {isAdmin && <li><Link to="/publishers">Publishers</Link></li>}
          <li><Link to="/loans">Loans</Link></li>
          {isAdmin && <li><Link to="/loans/new">New Loan</Link></li>}
          {isAdmin && <li><Link to="/fines">Fines</Link></li>}
          <li><Link to="/notifications">Notifications</Link></li>
          <li><Link to="/reviews">Reviews</Link></li>
          {isAdmin && <li><Link to="/users">Users</Link></li>}
          <li className="user-info">
            <span>{user?.firstName} {user?.lastName} {isAdmin && <span style={{color: '#e74c3c'}}>(Admin)</span>}</span>
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
          <Route path="/register" element={<Register />} />
          <Route
            path="/dashboard"
            element={
              <ProtectedRoute>
                <Dashboard />
              </ProtectedRoute>
            }
          />
          <Route
            path="/users"
            element={
              <AdminRoute>
                <UsersList />
              </AdminRoute>
            }
          />
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
              <AdminRoute>
                <BookForm />
              </AdminRoute>
            }
          />
          <Route
            path="/bookcopies"
            element={
              <AdminRoute>
                <BookCopiesList />
              </AdminRoute>
            }
          />
          <Route
            path="/authors"
            element={
              <AdminRoute>
                <AuthorsList />
              </AdminRoute>
            }
          />
          <Route
            path="/categories"
            element={
              <AdminRoute>
                <CategoriesList />
              </AdminRoute>
            }
          />
          <Route
            path="/publishers"
            element={
              <AdminRoute>
                <PublishersList />
              </AdminRoute>
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
              <AdminRoute>
                <CreateLoan />
              </AdminRoute>
            }
          />
          <Route
            path="/fines"
            element={
              <AdminRoute>
                <FinesList />
              </AdminRoute>
            }
          />
          <Route
            path="/notifications"
            element={
              <ProtectedRoute>
                <NotificationsList />
              </ProtectedRoute>
            }
          />
          <Route
            path="/reviews"
            element={
              <ProtectedRoute>
                <ReviewsList />
              </ProtectedRoute>
            }
          />
          <Route path="/" element={<Navigate to="/dashboard" replace />} />
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

