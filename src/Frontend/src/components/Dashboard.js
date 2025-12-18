import React from 'react';
import { Link } from 'react-router-dom';
import { useAuth } from '../contexts/AuthContext';
import './Dashboard.css';

function Dashboard() {
  const { user, isAdmin } = useAuth();

  const adminCards = [
    { title: 'Books', link: '/books', icon: '📚', description: 'Menaxho librat' },
    { title: 'Authors', link: '/authors', icon: '✍️', description: 'Menaxho autorët' },
    { title: 'Categories', link: '/categories', icon: '📂', description: 'Menaxho kategoritë' },
    { title: 'Publishers', link: '/publishers', icon: '🏢', description: 'Menaxho botuesit' },
    { title: 'Book Copies', link: '/bookcopies', icon: '📖', description: 'Menaxho kopjet e librave' },
    { title: 'Loans', link: '/loans', icon: '📋', description: 'Menaxho huazimet' },
    { title: 'Fines', link: '/fines', icon: '💰', description: 'Menaxho gjobat' },
    { title: 'Users', link: '/users', icon: '👥', description: 'Menaxho përdoruesit' },
    { title: 'Notifications', link: '/notifications', icon: '🔔', description: 'Menaxho njoftimet' },
    { title: 'Reviews', link: '/reviews', icon: '⭐', description: 'Menaxho recensionet' },
  ];

  const userCards = [
    { title: 'Books', link: '/books', icon: '📚', description: 'Shiko librat' },
    { title: 'My Loans', link: '/loans', icon: '📋', description: 'Huazimet e mia' },
    { title: 'My Reviews', link: '/reviews', icon: '⭐', description: 'Recensionet e mia' },
    { title: 'Notifications', link: '/notifications', icon: '🔔', description: 'Njoftimet e mia' },
  ];

  const cards = isAdmin ? adminCards : userCards;

  return (
    <div className="dashboard">
      <div className="dashboard-header">
        <h1>Dashboard</h1>
        <div className="welcome-message">
          <p>Mirë se vini, <strong>{user?.firstName} {user?.lastName}</strong>!</p>
          <span className={`role-badge ${isAdmin ? 'admin' : 'user'}`}>
            {isAdmin ? '👑 Admin' : '👤 Përdorues'}
          </span>
        </div>
      </div>

      <div className="dashboard-stats">
        <div className="stat-card">
          <div className="stat-icon">📚</div>
          <div className="stat-info">
            <h3>Librat</h3>
            <p>Shiko dhe menaxho librat</p>
          </div>
        </div>
        <div className="stat-card">
          <div className="stat-icon">📋</div>
          <div className="stat-info">
            <h3>Huazimet</h3>
            <p>Menaxho huazimet</p>
          </div>
        </div>
        {isAdmin && (
          <>
            <div className="stat-card">
              <div className="stat-icon">👥</div>
              <div className="stat-info">
                <h3>Përdoruesit</h3>
                <p>Menaxho përdoruesit</p>
              </div>
            </div>
            <div className="stat-card">
              <div className="stat-icon">💰</div>
              <div className="stat-info">
                <h3>Gjobat</h3>
                <p>Menaxho gjobat</p>
              </div>
            </div>
          </>
        )}
      </div>

      <div className="dashboard-grid">
        {cards.map((card, index) => (
          <Link key={index} to={card.link} className="dashboard-card">
            <div className="card-icon">{card.icon}</div>
            <div className="card-content">
              <h3>{card.title}</h3>
              <p>{card.description}</p>
            </div>
            <div className="card-arrow">→</div>
          </Link>
        ))}
      </div>
    </div>
  );
}

export default Dashboard;

