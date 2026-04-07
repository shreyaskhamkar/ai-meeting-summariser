import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { dashboardApi } from '../services/api';
import { DashboardStats } from '../types';

export default function DashboardPage() {
  const [stats, setStats] = useState<DashboardStats | null>(null);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchStats = async () => {
      try {
        const response = await dashboardApi.getStats();
        if (response.data.success) {
          setStats(response.data.data);
        }
      } finally {
        setLoading(false);
      }
    };
    fetchStats();
  }, []);

  if (loading) {
    return <div className="animate-pulse">Loading...</div>;
  }

  return (
    <div>
      <h1 className="text-2xl font-bold mb-6">Dashboard</h1>
      
      <div className="grid grid-cols-1 md:grid-cols-4 gap-6 mb-8">
        <div className="bg-white p-6 rounded-lg shadow">
          <div className="text-3xl font-bold text-blue-600">{stats?.totalMeetings || 0}</div>
          <div className="text-gray-600">Total Meetings</div>
        </div>
        <div className="bg-white p-6 rounded-lg shadow">
          <div className="text-3xl font-bold text-green-600">{stats?.completedMeetings || 0}</div>
          <div className="text-gray-600">Completed</div>
        </div>
        <div className="bg-white p-6 rounded-lg shadow">
          <div className="text-3xl font-bold text-yellow-600">{stats?.pendingActionItems || 0}</div>
          <div className="text-gray-600">Pending Tasks</div>
        </div>
        <div className="bg-white p-6 rounded-lg shadow">
          <div className="text-3xl font-bold text-purple-600">{stats?.completedActionItems || 0}</div>
          <div className="text-gray-600">Completed Tasks</div>
        </div>
      </div>

      <div className="grid grid-cols-1 md:grid-cols-2 gap-6">
        <div className="bg-white p-6 rounded-lg shadow">
          <h2 className="text-lg font-semibold mb-4">Recent Meetings</h2>
          {stats?.recentMeetings.length === 0 ? (
            <p className="text-gray-500">No meetings yet</p>
          ) : (
            <ul className="space-y-3">
              {stats?.recentMeetings.map((meeting) => (
                <li key={meeting.id}>
                  <Link to={`/meetings/${meeting.id}`} className="text-blue-600 hover:underline">
                    {meeting.title}
                  </Link>
                  <p className="text-sm text-gray-500">{new Date(meeting.createdAt).toLocaleDateString()}</p>
                </li>
              ))}
            </ul>
          )}
        </div>

        <div className="bg-white p-6 rounded-lg shadow">
          <h2 className="text-lg font-semibold mb-4">Upcoming Deadlines</h2>
          {stats?.upcomingDeadlines.length === 0 ? (
            <p className="text-gray-500">No upcoming deadlines</p>
          ) : (
            <ul className="space-y-3">
              {stats?.upcomingDeadlines.map((item) => (
                <li key={item.id} className="flex justify-between items-center">
                  <span>{item.task}</span>
                  <span className={`text-sm px-2 py-1 rounded ${
                    item.priority === 'High' ? 'bg-red-100 text-red-700' : 'bg-gray-100'
                  }`}>
                    {item.priority}
                  </span>
                </li>
              ))}
            </ul>
          )}
        </div>
      </div>
    </div>
  );
}
