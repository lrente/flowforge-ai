import { createContext, useContext, useEffect, useState } from 'react';
import api from './api';

const AuthContext = createContext({ user: null, loading: true, can: () => false });

export function AuthProvider({ children }) {
  const [user, setUser] = useState(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    if (!localStorage.getItem('token')) { setLoading(false); return; }
    api.get('/auth/me').then(({ data }) => setUser(data)).catch(() => localStorage.removeItem('token')).finally(() => setLoading(false));
  }, []);

  return <AuthContext.Provider value={{ user, loading, can: (permission) => user?.permissions?.includes(permission) }}>{children}</AuthContext.Provider>;
}

export const useAuth = () => useContext(AuthContext);
