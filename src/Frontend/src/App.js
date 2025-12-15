import React from 'react';
import { BrowserRouter as Router, Routes, Route, Link } from 'react-router-dom';
import './App.css';
import BooksList from './components/BooksList';
import BookForm from './components/BookForm';
import LoansList from './components/LoansList';
import CreateLoan from './components/CreateLoan';

function App() {
  return (
    <Router>
      <div className="App">
        <nav className="navbar">
          <div className="nav-container">
            <h1 className="nav-logo">📚 Biblioteka</h1>
            <ul className="nav-menu">
              <li><Link to="/books">Books</Link></li>
              <li><Link to="/books/new">Add Book</Link></li>
              <li><Link to="/loans">Loans</Link></li>
              <li><Link to="/loans/new">New Loan</Link></li>
            </ul>
          </div>
        </nav>

        <main className="main-content">
          <Routes>
            <Route path="/books" element={<BooksList />} />
            <Route path="/books/new" element={<BookForm />} />
            <Route path="/loans" element={<LoansList />} />
            <Route path="/loans/new" element={<CreateLoan />} />
            <Route path="/" element={<BooksList />} />
          </Routes>
        </main>
      </div>
    </Router>
  );
}

export default App;

