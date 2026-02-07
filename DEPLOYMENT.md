# Nafes API - Deployment Guide

## Prerequisites

- .NET 9.0 SDK installed
- SQL Server Management Studio (optional, for database verification)

## Step 1: Update Configuration

1. Open `appsettings.json`
2. Replace placeholders with actual Somee credentials:
   - `SERVER_PLACEHOLDER` → Your Somee SQL Server address
   - `DATABASE_PLACEHOLDER` → Your Somee database name
   - `USERNAME_PLACEHOLDER` → Your Somee database username
   - `PASSWORD_PLACEHOLDER` → Your Somee database password

3. Open `Program.cs`
4. Replace `VERCEL_URL_PLACEHOLDER` with your actual Vercel frontend URL

## Step 2: Build for Production

**Windows:**
```batch
publish-production.bat
```

**Linux/Mac:**
```bash
./publish-production.sh
```

**Manual:**
```bash
dotnet publish -c Release -o ./publish
```

## Step 3: Run Database Migrations

Before uploading, migrate your database:

```batch
migrate-production.bat
```

Or manually:
```bash
dotnet ef database update
```

## Step 4: Upload to Somee

1. **Files to upload:** Everything in `publish/` folder
2. **Upload destination:** `wwwroot/` or `wwwroot/api/`
3. **Method:** FTP (FileZilla) or Somee File Manager

## Step 5: Test

- **Test endpoint:** `https://yoursite.somee.com/api/health`
- **Check logs:** `wwwroot/logs/stdout.log`
- **Verify frontend can connect**

## Troubleshooting

| Issue | Solution |
|-------|----------|
| 500 Error | Check connection string and database migrations |
| CORS Error | Verify Vercel URL in CORS policy |
| 404 Error | Ensure web.config is in root of deployed app |
