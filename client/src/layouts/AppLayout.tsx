import { Outlet, Navigate } from 'react-router-dom';
import { useAuthStore } from '../hooks/useAuth';

export default function AppLayout() {
  const { isAuthenticated } = useAuthStore();

  if (!isAuthenticated) {
    return <Navigate to="/login" replace />;
  }

  return (
    <div className="flex min-h-screen bg-gray-50">
      <aside className="w-64 bg-white border-r border-gray-200">
        <div className="p-6">
          <h1 className="text-xl font-bold text-gray-900">AI Summarizer</h1>
        </div>
        <nav className="px-4 space-y-1">
          <a href="/dashboard" className="block px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-md">Dashboard</a>
          <a href="/meetings" className="block px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-md">Meetings</a>
          <a href="/action-items" className="block px-4 py-2 text-gray-700 hover:bg-gray-100 rounded-md">Action Items</a>
        </nav>
      </aside>
      <main className="flex-1 p-8">
        <Outlet />
      </main>
    </div>
  );
}
