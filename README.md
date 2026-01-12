
# Eric's Todo App 😊

## Setup

### 1. Check Prerequisites
- .NET SDK 10
- dotnet-ef ( `dotnet tool install --global dotnet-ef`)
- Node 18+

### 2. Run backend (dot net api)

```bash
cd server/eztask-server
dotnet ef database update
dotnet restore
dotnet run
```

- Swagger UI: http://localhost:5114/swagger  


### 3. Run frontend (vue via vite)

```bash
cd client
npm install
```

~Create `client/.env`:~ (i left `.env` in the repo to make one less step for this demo)

```env
API_BASE_URL=http://localhost:5114
```

Start dev server:

```bash
npm run dev
```
### 4. Open app

http://localhost:5173/


## Useful development commands

### Generate the client side sdk from openApi docs

```bash
cd client
npm i openapi-fetch
npm i -D openapi-typescript
npx openapi-typescript http://localhost:5114/swagger/v1/swagger.json -o src/api/schema.d.ts
```

The client wrapper lives in `src/api/client.ts` and uses the generated types.

Regenerate types whenever the .NET DTOs or routes change.
