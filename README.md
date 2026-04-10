# 🧠 AI Meeting Summariser

A full-stack production-quality SaaS application for AI-powered meeting transcription, summarization, and action item extraction.

---

## 🚀 Project Repositories

* 🟢 **Frontend (React + TypeScript)**
  https://github.com/shreyaskhamkar/ai-meeting-client

* 🔵 **Backend (.NET Web API)**
  https://github.com/shreyaskhamkar/ai-meeting-server

---

## ✨ Features

* User Authentication (JWT-based)
* Upload meeting audio/video
* AI Transcription
* AI Summarization
* Action Item Extraction
* Decision Tracking
* AI Chat with meetings
* Dashboard analytics

---

## 🏗️ Architecture

This project follows a **micro-repo architecture**:

* Frontend → React (deployed on Vercel)
* Backend → .NET Web API (deployed on Render/Azure)
* Database → PostgreSQL (Neon)
* AI → OpenAI / Azure AI

💡 Initially built as a monorepo, later split using **git subtree** for:

* Independent deployments
* Scalable CI/CD pipelines
* Better team collaboration

---

## ⚙️ Tech Stack

### Frontend

* React 19 + TypeScript
* Vite
* Tailwind CSS
* TanStack Query
* Zustand

### Backend

* .NET 9 Web API
* Entity Framework Core
* PostgreSQL
* MediatR (CQRS)
* FluentValidation
* Serilog

---

## 📡 API Overview

### Auth

* POST `/api/auth/register`
* POST `/api/auth/login`

### Meetings

* CRUD + Search

### AI

* Process meeting
* Chat with transcript

### Dashboard

* Overview stats

### Action Items

* Track and update tasks

---

## 🚀 Deployment

### Frontend

* Hosted on Vercel

### Backend

* Hosted on Render / Azure

### Database

* Neon PostgreSQL

---

## 🔮 Future Improvements

* OpenAI / Whisper integration
* Cloud storage (S3 / Azure Blob)
* Team collaboration
* Slack / Notion integration
* Calendar sync

---

## 👨‍💻 Author

**Shreyas Khamkar**
Full Stack Developer (.NET + React)
