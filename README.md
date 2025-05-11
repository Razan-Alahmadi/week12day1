
# Image Processing API

This project is a secure and cache-enabled Image Processing Web API built with ASP.NET Core. It supports basic image filters like grayscale and sepia, implements API key-based authentication, and enforces rate limiting.

## Features

- 📷 Upload and process images
- 🎨 Apply filters (grayscale, sepia)
- 🔐 Secure API using API Key Authentication
- 📉 Rate limiting to prevent abuse
- ⚡ In-memory caching for performance


## API Usage

### 🔑 Generate API Key

`POST /api/apikeys/generate`

- Returns a new API key.

### 🖼️ Process Image

`POST /api/images/process?filter=grayscale`

