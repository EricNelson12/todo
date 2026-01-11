Tasko

Prereqs
	•	.NET SDK 8+
	•	Node 18+

Run server

cd server/eztask-server
dotnet restore
dotnet run

Swagger UI: http://localhost:5114/swagger
OpenAPI JSON: http://localhost:5114/swagger/v1/swagger.json

Run client

cd client
npm install

Create client/.env.local:

API_BASE_URL=http://localhost:5114

npm run dev

Generate typed API from Swagger

cd client
npm i openapi-fetch
npm i -D openapi-typescript
npx openapi-typescript http://localhost:5114/swagger/v1/swagger.json -o src/api/schema.d.ts

Client wrapper is in src/api/client.ts and uses the generated types.

Regenerate types whenever the .NET DTOs or routes change.