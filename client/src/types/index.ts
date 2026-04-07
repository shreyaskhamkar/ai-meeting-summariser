/// <reference types="vite/client" />

export interface User {
  id: string;
  fullName: string;
  email: string;
  createdAt: string;
}

export interface AuthResponse {
  token: string;
  user: User;
}

export interface Meeting {
  id: string;
  title: string;
  description?: string;
  fileUrl?: string;
  originalFileName?: string;
  durationInMinutes?: number;
  status: string;
  meetingDate?: string;
  createdAt: string;
  summary?: string;
  actionItemCount: number;
  decisionCount: number;
}

export interface Transcript {
  id: string;
  fullText: string;
  language: string;
  createdAt: string;
}

export interface Summary {
  id: string;
  shortSummary: string;
  detailedSummary: string;
  keyDiscussionPoints?: string;
  risksOrBlockers?: string;
  createdAt: string;
}

export interface MeetingDetail extends Omit<Meeting, 'summary'> {
  transcript?: Transcript;
  summary?: Summary;
  actionItems: ActionItem[];
  decisions: Decision[];
  participants: Participant[];
}

export interface ActionItem {
  id: string;
  task: string;
  ownerName?: string;
  deadline?: string;
  priority: string;
  status: string;
}

export interface Decision {
  id: string;
  decisionText: string;
}

export interface Participant {
  id: string;
  name: string;
  email?: string;
  role: string;
}

export interface ChatMessage {
  id: string;
  role: string;
  message: string;
  createdAt: string;
}

export interface DashboardStats {
  totalMeetings: number;
  completedMeetings: number;
  pendingActionItems: number;
  completedActionItems: number;
  recentMeetings: Meeting[];
  upcomingDeadlines: ActionItem[];
}

export interface PaginatedList<T> {
  items: T[];
  totalCount: number;
  page: number;
  pageSize: number;
  totalPages: number;
}

export interface ResponseWrapper<T> {
  success: boolean;
  message?: string;
  data?: T;
  errors?: string[];
}
