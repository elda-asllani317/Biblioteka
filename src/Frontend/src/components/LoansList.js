import React, { useState, useEffect } from 'react';
import { loansAPI } from '../services/api';
import './LoansList.css';

function LoansList() {
  const [loans, setLoans] = useState([]);
  const [loading, setLoading] = useState(true);
  const [showOverdue, setShowOverdue] = useState(false);

  useEffect(() => {
    loadLoans();
  }, [showOverdue]);

  const loadLoans = async () => {
    try {
      setLoading(true);
      const response = showOverdue
        ? await loansAPI.getOverdue()
        : await loansAPI.getAll();
      setLoans(response.data);
    } catch (error) {
      console.error('Error loading loans:', error);
      alert('Error loading loans');
    } finally {
      setLoading(false);
    }
  };

  const handleReturn = async (loanId) => {
    if (!window.confirm('Are you sure you want to return this book?')) {
      return;
    }

    try {
      await loansAPI.returnLoan(loanId);
      loadLoans();
    } catch (error) {
      console.error('Error returning loan:', error);
      alert('Error returning loan');
    }
  };

  const handleDelete = async (loanId) => {
    if (!window.confirm('Are you sure you want to delete this loan?')) {
      return;
    }

    try {
      await loansAPI.delete(loanId);
      loadLoans();
    } catch (error) {
      console.error('Error deleting loan:', error);
      alert('Error deleting loan');
    }
  };

  if (loading) {
    return <div className="loading">Loading...</div>;
  }

  return (
    <div>
      <div style={{ display: 'flex', justifyContent: 'space-between', alignItems: 'center', marginBottom: '1.5rem' }}>
        <h1>Loans Management</h1>
        <button
          className={`btn ${showOverdue ? 'btn-primary' : ''}`}
          onClick={() => setShowOverdue(!showOverdue)}
          style={!showOverdue ? { background: '#95a5a6', color: 'white' } : {}}
        >
          {showOverdue ? 'Show All Loans' : 'Show Overdue Only'}
        </button>
      </div>

      <div className="card">
        <table className="table">
          <thead>
            <tr>
              <th>User</th>
              <th>Book</th>
              <th>Loan Date</th>
              <th>Due Date</th>
              <th>Return Date</th>
              <th>Status</th>
              <th>Actions</th>
            </tr>
          </thead>
          <tbody>
            {loans.length === 0 ? (
              <tr>
                <td colSpan="7" style={{ textAlign: 'center' }}>No loans found</td>
              </tr>
            ) : (
              loans.map((loan) => (
                <tr key={loan.id} className={loan.status === 'Active' && new Date(loan.dueDate) < new Date() ? 'overdue' : ''}>
                  <td>{loan.userName}</td>
                  <td>{loan.bookTitle}</td>
                  <td>{new Date(loan.loanDate).toLocaleDateString()}</td>
                  <td>{new Date(loan.dueDate).toLocaleDateString()}</td>
                  <td>{loan.returnDate ? new Date(loan.returnDate).toLocaleDateString() : '-'}</td>
                  <td>
                    <span className={`status-badge status-${loan.status.toLowerCase()}`}>
                      {loan.status}
                    </span>
                  </td>
                  <td>
                    {loan.status === 'Active' && (
                      <button
                        className="btn btn-success"
                        onClick={() => handleReturn(loan.id)}
                      >
                        Return
                      </button>
                    )}
                    <button
                      className="btn btn-danger"
                      style={{ marginLeft: '0.5rem' }}
                      onClick={() => handleDelete(loan.id)}
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

export default LoansList;

