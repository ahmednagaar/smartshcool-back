# Pre-Deployment Checklist

## Configuration Files

- [ ] `appsettings.json` - Connection string updated
- [ ] `appsettings.json` - Logging levels set to Warning/Error
- [ ] `Program.cs` - CORS configured for Vercel URL
- [ ] `web.config` - Created and configured for IIS

## Code Changes

- [ ] Development-only features disabled in production
- [ ] Swagger disabled (or secured) for production
- [ ] All secrets removed from code
- [ ] Database migrations created and tested

## Build

- [ ] Project builds without errors: `dotnet build`
- [ ] Project publishes successfully: `dotnet publish -c Release -o ./publish`
- [ ] `web.config` present in publish folder
- [ ] All DLL files present in publish folder

## Database

- [ ] Somee database created
- [ ] Connection string tested locally
- [ ] Migrations run successfully: `dotnet ef database update`
- [ ] All tables created in Somee database

## Testing

- [ ] Local build works: `dotnet run`
- [ ] API endpoints respond correctly
- [ ] Authentication/Authorization works
- [ ] CORS allows Vercel domain

## Deployment

- [ ] Files uploaded to Somee (FTP or File Manager)
- [ ] Backend URL tested: `https://yoursite.somee.com/api`
- [ ] Frontend connected to backend
- [ ] End-to-end testing complete
