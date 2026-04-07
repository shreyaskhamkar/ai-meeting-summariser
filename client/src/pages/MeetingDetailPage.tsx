import { useEffect, useState } from 'react';
import { useParams, Link } from 'react-router-dom';
import { meetingsApi, aiApi } from '../services/api';
import { MeetingDetail, ChatMessage } from '../types';

export default function MeetingDetailPage() {
  const { id } = useParams<{ id: string }>();
  const [meeting, setMeeting] = useState<MeetingDetail | null>(null);
  const [loading, setLoading] = useState(true);
  const [activeTab, setActiveTab] = useState('overview');
  const [chatMessages, setChatMessages] = useState<ChatMessage[]>([]);
  const [chatInput, setChatInput] = useState('');
  const [chatLoading, setChatLoading] = useState(false);

  useEffect(() => {
    const fetchMeeting = async () => {
      if (!id) return;
      try {
        const response = await meetingsApi.getById(id);
        if (response.data.success) {
          setMeeting(response.data.data);
        }
      } finally {
        setLoading(false);
      }
    };
    fetchMeeting();
  }, [id]);

  useEffect(() => {
    if (activeTab === 'chat' && id) {
      const fetchChat = async () => {
        try {
          const response = await aiApi.getChatHistory(id);
          if (response.data.success) {
            setChatMessages(response.data.data);
          }
        } catch (e) {
          console.log('No chat history');
        }
      };
      fetchChat();
    }
  }, [activeTab, id]);

  const handleSendMessage = async () => {
    if (!chatInput.trim() || !id) return;
    setChatLoading(true);
    try {
      const response = await aiApi.chat(id, chatInput);
      if (response.data.success) {
        setChatMessages([...chatMessages, response.data.data]);
        setChatInput('');
      }
    } finally {
      setChatLoading(false);
    }
  };

  const handleProcessMeeting = async () => {
    if (!id) return;
    setLoading(true);
    try {
      await aiApi.processMeeting(id);
      const response = await meetingsApi.getById(id);
      if (response.data.success) {
        setMeeting(response.data.data);
      }
    } finally {
      setLoading(false);
    }
  };

  if (loading) {
    return <div className="animate-pulse">Loading...</div>;
  }

  if (!meeting) {
    return <div>Meeting not found</div>;
  }

  const tabs = [
    { id: 'overview', label: 'Overview' },
    { id: 'transcript', label: 'Transcript' },
    { id: 'actionItems', label: 'Action Items' },
    { id: 'decisions', label: 'Decisions' },
    { id: 'chat', label: 'AI Chat' },
  ];

  return (
    <div>
      <div className="flex justify-between items-center mb-6">
        <div>
          <Link to="/meetings" className="text-blue-600 hover:underline mb-2 block">← Back to Meetings</Link>
          <h1 className="text-2xl font-bold">{meeting.title}</h1>
          <span className={`inline-block mt-2 px-2 py-1 rounded ${
            meeting.status === 'Completed' ? 'bg-green-100 text-green-700' :
            meeting.status === 'Processing' ? 'bg-yellow-100 text-yellow-700' :
            'bg-gray-100 text-gray-700'
          }`}>
            {meeting.status}
          </span>
        </div>
        {meeting.status === 'Uploaded' && (
          <button
            onClick={handleProcessMeeting}
            className="bg-blue-600 text-white px-4 py-2 rounded-md hover:bg-blue-700"
          >
            Process Meeting
          </button>
        )}
      </div>

      <div className="border-b mb-6">
        <nav className="flex gap-4">
          {tabs.map((tab) => (
            <button
              key={tab.id}
              onClick={() => setActiveTab(tab.id)}
              className={`py-2 px-1 border-b-2 ${
                activeTab === tab.id
                  ? 'border-blue-600 text-blue-600'
                  : 'border-transparent text-gray-600'
              }`}
            >
              {tab.label}
            </button>
          ))}
        </nav>
      </div>

      {activeTab === 'overview' && (
        <div className="space-y-6">
          {meeting.description && (
            <div className="bg-white p-6 rounded-lg shadow">
              <h2 className="font-semibold mb-2">Description</h2>
              <p className="text-gray-600">{meeting.description}</p>
            </div>
          )}
          {meeting.summary && (
            <div className="bg-white p-6 rounded-lg shadow">
              <h2 className="font-semibold mb-2">Summary</h2>
              <p className="text-gray-600">{meeting.summary.shortSummary}</p>
              <h3 className="font-medium mt-4 mb-2">Detailed Summary</h3>
              <p className="text-gray-600 whitespace-pre-wrap">{meeting.summary.detailedSummary}</p>
            </div>
          )}
          {meeting.summary?.keyDiscussionPoints && (
            <div className="bg-white p-6 rounded-lg shadow">
              <h2 className="font-semibold mb-2">Key Discussion Points</h2>
              <p className="text-gray-600 whitespace-pre-wrap">{meeting.summary.keyDiscussionPoints}</p>
            </div>
          )}
          {meeting.summary?.risksOrBlockers && (
            <div className="bg-white p-6 rounded-lg shadow">
              <h2 className="font-semibold mb-2">Risks & Blockers</h2>
              <p className="text-gray-600 whitespace-pre-wrap">{meeting.summary.risksOrBlockers}</p>
            </div>
          )}
        </div>
      )}

      {activeTab === 'transcript' && (
        <div className="bg-white p-6 rounded-lg shadow">
          <h2 className="font-semibold mb-4">Transcript</h2>
          {meeting.transcript ? (
            <p className="text-gray-600 whitespace-pre-wrap">{meeting.transcript.fullText}</p>
          ) : (
            <p className="text-gray-500">No transcript available</p>
          )}
        </div>
      )}

      {activeTab === 'actionItems' && (
        <div className="bg-white p-6 rounded-lg shadow">
          <h2 className="font-semibold mb-4">Action Items</h2>
          {meeting.actionItems.length === 0 ? (
            <p className="text-gray-500">No action items</p>
          ) : (
            <ul className="space-y-3">
              {meeting.actionItems.map((item) => (
                <li key={item.id} className="flex justify-between items-center p-3 bg-gray-50 rounded">
                  <div>
                    <p className="font-medium">{item.task}</p>
                    <p className="text-sm text-gray-500">
                      {item.ownerName && `Owner: ${item.ownerName}`}
                      {item.deadline && ` • Due: ${new Date(item.deadline).toLocaleDateString()}`}
                    </p>
                  </div>
                  <span className={`px-2 py-1 rounded text-sm ${
                    item.priority === 'High' ? 'bg-red-100 text-red-700' :
                    item.priority === 'Medium' ? 'bg-yellow-100 text-yellow-700' :
                    'bg-gray-100 text-gray-700'
                  }`}>
                    {item.priority}
                  </span>
                </li>
              ))}
            </ul>
          )}
        </div>
      )}

      {activeTab === 'decisions' && (
        <div className="bg-white p-6 rounded-lg shadow">
          <h2 className="font-semibold mb-4">Decisions</h2>
          {meeting.decisions.length === 0 ? (
            <p className="text-gray-500">No decisions recorded</p>
          ) : (
            <ul className="space-y-3">
              {meeting.decisions.map((decision) => (
                <li key={decision.id} className="p-3 bg-gray-50 rounded">
                  {decision.decisionText}
                </li>
              ))}
            </ul>
          )}
        </div>
      )}

      {activeTab === 'chat' && (
        <div className="bg-white rounded-lg shadow flex flex-col h-[500px]">
          <div className="flex-1 p-4 overflow-y-auto">
            {chatMessages.length === 0 ? (
              <p className="text-gray-500 text-center">Ask questions about this meeting...</p>
            ) : (
              <div className="space-y-4">
                {chatMessages.map((msg) => (
                  <div key={msg.id} className={`${msg.role === 'User' ? 'ml-auto bg-blue-100' : 'mr-auto bg-gray-100'} p-3 rounded-lg max-w-[80%]`}>
                    <p className="text-gray-800">{msg.message}</p>
                  </div>
                ))}
              </div>
            )}
          </div>
          <div className="p-4 border-t">
            <div className="flex gap-2">
              <input
                type="text"
                value={chatInput}
                onChange={(e) => setChatInput(e.target.value)}
                onKeyDown={(e) => e.key === 'Enter' && handleSendMessage()}
                placeholder="Ask about this meeting..."
                className="flex-1 px-4 py-2 border rounded-md"
                disabled={chatLoading}
              />
              <button
                onClick={handleSendMessage}
                disabled={chatLoading || !chatInput.trim()}
                className="bg-blue-600 text-white px-4 py-2 rounded-md disabled:opacity-50"
              >
                {chatLoading ? '...' : 'Send'}
              </button>
            </div>
          </div>
        </div>
      )}
    </div>
  );
}
