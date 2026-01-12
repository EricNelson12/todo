
# Todo App

## Prereqs
- .NET SDK 10
- Node 18+

---

## Run server

```bash
cd server/eztask-server
dotnet restore
dotnet run
```

- Swagger UI: http://localhost:5114/swagger  
- OpenAPI JSON: http://localhost:5114/swagger/v1/swagger.json  

---

## Run client

```bash
cd client
npm install
```

Create `client/.env`:

```env
API_BASE_URL=http://localhost:5114
```

Start dev server:

```bash
npm run dev
```

---

## Generate typed API from Swagger

```bash
cd client
npm i openapi-fetch
npm i -D openapi-typescript
npx openapi-typescript http://localhost:5114/swagger/v1/swagger.json -o src/api/schema.d.ts
```

The client wrapper lives in `src/api/client.ts` and uses the generated types.

Regenerate types whenever the .NET DTOs or routes change.
