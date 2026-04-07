import { useEffect, useState } from 'react';
import { actionItemsApi } from '../services/api';
import { ActionItem } from '../types';

export default function ActionItemsPage() {
  const [actionItems, setActionItems] = useState<ActionItem[]>([]);
  const [loading, setLoading] = useState(true);

  useEffect(() => {
    const fetchActionItems = async () => {
      try {
        const response = await actionItemsApi.getAll();
        if (response.data.success) {
          setActionItems(response.data.data);
        }
      } finally {
        setLoading(false);
      }
    };
    fetchActionItems();
  }, []);

  const handleUpdateStatus = async (id: string, status: string) => {
    try {
      const newStatus = status === 'Completed' ? 'Pending' : 'Completed';
      await actionItemsApi.update(id, { status: newStatus });
      setActionItems(items =>
        items.map(item =>
          item.id === id ? { ...item, status: newStatus } : item
        )
      );
    } catch (e) {
      console.error('Update failed');
    }
  };

  return (
    <div>
      <h1 className="text-2xl font-bold mb-6">Action Items</h1>

      {loading ? (
        <div className="animate-pulse">Loading...</div>
      ) : actionItems.length === 0 ? (
        <div className="text-center py-12 text-gray-500">
          No action items yet
        </div>
      ) : (
        <div className="bg-white rounded-lg shadow overflow-hidden">
          <table className="min-w-full">
            <thead className="bg-gray-50">
              <tr>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Task</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Owner</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Priority</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Status</th>
                <th className="px-6 py-3 text-left text-xs font-medium text-gray-500 uppercase">Actions</th>
              </tr>
            </thead>
            <tbody className="divide-y divide-gray-200">
              {actionItems.map((item) => (
                <tr key={item.id}>
                  <td className="px-6 py-4">{item.task}</td>
                  <td className="px-6 py-4 text-gray-600">{item.ownerName || '-'}</td>
                  <td className="px-6 py-4">
                    <span className={`px-2 py-1 rounded text-xs ${
                      item.priority === 'High' ? 'bg-red-100 text-red-700' :
                      item.priority === 'Medium' ? 'bg-yellow-100 text-yellow-700' :
                      'bg-gray-100 text-gray-700'
                    }`}>
                      {item.priority}
                    </span>
                  </td>
                  <td className="px-6 py-4">
                    <span className={`px-2 py-1 rounded text-xs ${
                      item.status === 'Completed' ? 'bg-green-100 text-green-700' :
                      item.status === 'Pending' ? 'bg-gray-100 text-gray-700' :
                      'bg-blue-100 text-blue-700'
                    }`}>
                      {item.status}
                    </span>
                  </td>
                  <td className="px-6 py-4">
                    <button
                      onClick={() => handleUpdateStatus(item.id, item.status)}
                      className="text-blue-600 hover:underline text-sm"
                    >
                      {item.status === 'Completed' ? 'Mark Pending' : 'Mark Complete'}
                    </button>
                  </td>
                </tr>
              ))}
            </tbody>
          </table>
        </div>
      )}
    </div>
  );
}
