import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import { meetingsApi } from '../services/api';
import { Meeting } from '../types';

export default function MeetingsPage() {
  const [meetings, setMeetings] = useState<Meeting[]>([]);
  const [page, setPage] = useState(1);
  const [totalPages, setTotalPages] = useState(1);
  const [loading, setLoading] = useState(true);
  const [search, setSearch] = useState('');

  useEffect(() => {
    const fetchMeetings = async () => {
      setLoading(true);
      try {
        const response = await meetingsApi.getAll(page);
        if (response.data.success) {
          setMeetings(response.data.data.items);
          setTotalPages(response.data.data.totalPages);
        }
      } finally {
        setLoading(false);
      }
    };
    fetchMeetings();
  }, [page]);

  const handleSearch = async (e: React.FormEvent) => {
    e.preventDefault();
    if (!search.trim()) return;
    setLoading(true);
    try {
      const response = await meetingsApi.search(search);
      if (response.data.success) {
        setMeetings(response.data.data);
        setTotalPages(1);
      }
    } finally {
      setLoading(false);
    }
  };

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <h1 className="text-2xl font-bold">Meetings</h1>
        <Link to="/meetings/new" className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700">
          Upload Meeting
        </Link>
      </div>

      <form onSubmit={handleSearch} className="mb-6">
        <div className="flex gap-2">
          <input
            type="text"
            value={search}
            onChange={(e) => setSearch(e.target.value)}
            placeholder="Search meetings..."
            className="flex-1 px-4 py-2 border rounded-md"
          />
          <button type="submit" className="bg-gray-800 text-white px-4 py-2 rounded-md">
            Search
          </button>
        </div>
      </form>

      {loading ? (
        <div className="animate-pulse">Loading...</div>
      ) : meetings.length === 0 ? (
        <div className="text-center py-12 text-gray-500">
          No meetings found. Upload a meeting to get started.
        </div>
      ) : (
        <div className="grid grid-cols-1 md:grid-cols-2 lg:grid-cols-3 gap-4">
          {meetings.map((meeting) => (
            <Link
              key={meeting.id}
              to={`/meetings/${meeting.id}`}
              className="bg-white p-4 rounded-lg shadow hover:shadow-md transition"
            >
              <h3 className="font-semibold text-lg mb-2">{meeting.title}</h3>
              <p className="text-gray-600 text-sm mb-2 line-clamp-2">
                {meeting.summary || meeting.description || 'No description'}
              </p>
              <div className="flex justify-between items-center text-sm">
                <span className={`px-2 py-1 rounded ${
                  meeting.status === 'Completed' ? 'bg-green-100 text-green-700' :
                  meeting.status === 'Processing' ? 'bg-yellow-100 text-yellow-700' :
                  'bg-gray-100 text-gray-700'
                }`}>
                  {meeting.status}
                </span>
                <span className="text-gray-500">
                  {new Date(meeting.createdAt).toLocaleDateString()}
                </span>
              </div>
            </Link>
          ))}
        </div>
      )}

      {totalPages > 1 && (
        <div className="flex justify-center gap-2 mt-6">
          <button
            onClick={() => setPage(p => Math.max(1, p - 1))}
            disabled={page === 1}
            className="px-4 py-2 border rounded disabled:opacity-50"
          >
            Previous
          </button>
          <span className="px-4 py-2">Page {page} of {totalPages}</span>
          <button
            onClick={() => setPage(p => Math.min(totalPages, p + 1))}
            disabled={page === totalPages}
            className="px-4 py-2 border rounded disabled:opacity-50"
          >
            Next
          </button>
        </div>
      )}
    </div>
  );
}
