import { Navigate, Route, Routes } from 'react-router-dom';
import Layout from './components/Layout';
import LoginPage from './pages/LoginPage';
import DashboardPage from './pages/DashboardPage';
import AgentsPage from './pages/AgentsPage';
import AgentEditorPage from './pages/AgentEditorPage';
import KnowledgePage from './pages/KnowledgePage';
import ChatPage from './pages/ChatPage';
import SettingsPage from './pages/SettingsPage';
import UsersPage from './pages/UsersPage';
import AuditLogPage from './pages/AuditLogPage';

const isAuthenticated = () => Boolean(localStorage.getItem('token'));

function ProtectedRoute({ children }) {
  return isAuthenticated() ? children : <Navigate to="/login" replace />;
}

export default function App() {
  return (
    <Routes>
      <Route path="/login" element={<LoginPage />} />
      <Route
        path="/"
        element={
          <ProtectedRoute>
            <Layout />
          </ProtectedRoute>
        }
      >
        <Route index element={<DashboardPage />} />
        <Route path="agents" element={<AgentsPage />} />
        <Route path="agents/new" element={<AgentEditorPage />} />
        <Route path="agents/:id" element={<AgentEditorPage />} />
        <Route path="knowledge" element={<KnowledgePage />} />
        <Route path="conversations" element={<ChatPage />} />
        <Route path="settings" element={<SettingsPage />} />
        <Route path="users" element={<UsersPage />} />
        <Route path="audit-logs" element={<AuditLogPage />} />
      </Route>
    </Routes>
  );
}
