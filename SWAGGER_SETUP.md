# Swagger UI Setup - Complete Guide

## ✅ What's Been Implemented:

### 1. **Swagger Package Added**
   - Added `Swashbuckle.AspNetCore` package to the project
   - Configured Swagger/OpenAPI in `Program.cs`

### 2. **Beautiful Custom UI**
   - Custom CSS styling with gradient themes
   - Modern color scheme (purple/blue gradients)
   - Smooth animations and transitions
   - Responsive design
   - Enhanced buttons and operation blocks

### 3. **JWT Authentication Integration**
   - Swagger UI includes JWT Bearer token authentication
   - "Authorize" button in Swagger UI
   - Easy token input for testing protected endpoints

### 4. **Features Enabled**
   - Deep linking
   - Filter functionality
   - Request duration display
   - Validator enabled
   - Custom JavaScript enhancements

## 🚀 How to Access:

1. **Start the backend:**
   ```bash
   cd RecruitmentSystem.API
   dotnet restore
   dotnet run
   ```

2. **Open Swagger UI:**
   - Navigate to: `http://localhost:5000` (or the port shown in terminal)
   - Swagger UI will be displayed at the root URL

## 🎨 UI Features:

- **Beautiful gradient header** with purple/blue theme
- **Color-coded HTTP methods:**
  - GET: Blue
  - POST: Green
  - PUT: Orange
  - DELETE: Red
- **Smooth animations** when expanding/collapsing operations
- **Enhanced buttons** with hover effects
- **Custom scrollbars** matching the theme
- **Responsive design** for mobile devices

## 🔐 Using JWT Authentication:

1. Click the **"Authorize"** button (green button at top)
2. In the "Value" field, enter: `Bearer YOUR_TOKEN_HERE`
   - Example: `Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...`
3. Click **"Authorize"**
4. Click **"Close"**
5. Now all protected endpoints will include the token automatically

## 📝 Testing Endpoints:

1. **Login first:**
   - Use `/api/auth/login` endpoint
   - Enter credentials:
     - Email: `hr@recruitment.com`
     - Password: `Hr@123456`
     - Role: `HR`
   - Copy the token from response

2. **Authorize in Swagger:**
   - Click "Authorize" button
   - Paste token with "Bearer " prefix
   - Test protected endpoints

## 🎯 API Endpoints Available:

- **Authentication:**
  - `POST /api/auth/login` - Login (HR or Candidate)

- **Jobs (HR only):**
  - `GET /api/jobs` - Get all jobs (paginated)
  - `GET /api/jobs/{id}` - Get job by ID
  - `POST /api/jobs` - Create job
  - `PUT /api/jobs/{id}` - Update job
  - `DELETE /api/jobs/{id}` - Delete job

- **Candidates (HR only):**
  - `GET /api/candidates` - Get all candidates (paginated)
  - `GET /api/candidates/{id}` - Get candidate by ID
  - `POST /api/candidates` - Create candidate
  - `PUT /api/candidates/{id}` - Update candidate
  - `DELETE /api/candidates/{id}` - Delete candidate

- **Applications:**
  - `GET /api/applications` - Get applications (with filters)
  - `GET /api/applications/{id}` - Get application by ID
  - `POST /api/applications` - Create application (Candidate only)
  - `PUT /api/applications/{id}/status` - Update status (HR only)

- **Public:**
  - `GET /api/public/jobs` - Get active jobs (no auth required)

## 🎨 Customization:

The custom styling is in:
- `wwwroot/swagger-ui/custom.css` - All styling
- `wwwroot/swagger-ui/custom.js` - JavaScript enhancements

You can modify these files to change colors, fonts, or add features.

## 📋 Notes:

- Swagger UI is available at the root URL (`/`)
- All endpoints are documented with request/response schemas
- JWT authentication is fully integrated
- Beautiful, modern UI with smooth animations

Enjoy your beautiful Swagger UI! 🎉

