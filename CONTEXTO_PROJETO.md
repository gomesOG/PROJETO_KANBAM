# FlowBoard — Contexto do Projeto

## Visão geral

Aplicação Kanban chamada **FlowBoard** com:
- **Backend:** ASP.NET Core C# (.NET 8) — Clean Architecture + CQRS
- **Frontend:** React (Vite + TypeScript) — **não iniciado ainda**
- **Banco de dados:** SQL Server com EF Core 8 (Code-First)

Repositório local: `C:\Users\User\Desktop\Repositórios\PROJETO_KANBAM`  
Branch atual: `Inicio`

---

## Estrutura de pastas

```
PROJETO_KANBAM/
├── FlowBoard.sln
├── database/
│   └── FlowBoard_Schema.sql          ← script SQL idempotente, pronto para rodar
├── src/
│   ├── FlowBoard.Domain/
│   ├── FlowBoard.Application/
│   ├── FlowBoard.Infrastructure/
│   └── FlowBoard.API/
└── (frontend React — criar aqui: flowboard-web/)
```

---

## Estado do backend — **Completo, build limpo (0 erros, 0 avisos)**

### Domain Layer (`src/FlowBoard.Domain/`)

**Common:**
- `Entity.cs` — base com `Guid Id`, `CreatedAt`, `UpdatedAt`, domain events, igualdade por Id
- `AggregateRoot.cs` — herda `Entity`, marcador para raízes de agregado
- `ValueObject.cs` — igualdade por `GetEqualityComponents()`
- `IDomainEvent.cs` — `DateTime OccurredOn`

**Enums:** `Priority` (Low=1..Critical=4), `CardStatus` (Active=1, Archived=2), `ProjectRole` (Viewer=1..Owner=4)

**ValueObjects:** `Email` — valida, normaliza para lowercase

**Entities:**
- `User` — `Name`, `Email` (VO), `PasswordHash`, `AvatarUrl`, `IsActive`; `static Create(name, email, passwordHash)`
- `Project` — possui `List<ProjectMember>` e `List<Board>`; `AddMember()`, `RemoveMember()` (owner protegido), `Archive()`
- `ProjectMember` — `internal static Create()`, `ChangeRole()`
- `Board` — `AddColumn()` (calcula posição), `ReorderColumns(IEnumerable<Guid>)`, `Archive()`
- `Column` — `AddCard()` com enforcement de WIP limit; `internal SetPosition()`
- `Card` — `MoveToColumn()`, `AddTag()`, `RemoveTag()`, `AssignUser()`, `UnassignUser()`, `AddChecklistItem()`, `ToggleChecklistItem()`, `Archive()`, `public SetPosition()`
- `CardTag`, `CardAssignee`, `ChecklistItem` — suporte, `internal static Create()`

**Interfaces:** `IRepository<T>`, `IUnitOfWork`, `IUserRepository`, `IProjectRepository`, `IBoardRepository`, `ICardRepository`

---

### Application Layer (`src/FlowBoard.Application/`)

**Infraestrutura comum:**
- `Result<T>` / `Result` — pattern de sucesso/falha tipado
- Exceções: `NotFoundException`, `ForbiddenException`, `ValidationException`
- `ValidationBehavior` — pipeline MediatR, roda FluentValidation antes de cada handler
- `LoggingBehavior` — loga tempo de execução, avisa se >500ms

**DTOs:** `UserDto`, `ProjectDto` + `ProjectMemberDto`, `BoardDto` + `BoardDetailDto`, `ColumnDto` (com cards), `CardSummaryDto`, `CardDetailDto`, `CardTagDto`, `CardAssigneeDto`, `ChecklistItemDto`

**Interfaces:** `ITokenService`, `ICurrentUserService`, `IPasswordHasher`

**Use Cases implementados:**

| Área | Commands | Queries |
|---|---|---|
| Auth | `RegisterUser`, `LoginUser` | — |
| Projects | `CreateProject`, `UpdateProject`, `ArchiveProject`, `AddProjectMember` | `GetProjects`, `GetProjectById` |
| Boards | `CreateBoard`, `AddColumn`, `ReorderColumns` | `GetBoardById` |
| Cards | `CreateCard`, `UpdateCard`, `MoveCard`, `AssignUser`, `ToggleChecklistItem` | `GetCardById` |

Cada use case tem: `Command/Query` + `Handler` + `Validator` (FluentValidation)

**DI:** `services.AddApplication()` registra MediatR 12.4.1, validators, pipeline behaviors

---

### Infrastructure Layer (`src/FlowBoard.Infrastructure/`)

- `AppDbContext` — 9 DbSets; `ApplyConfigurationsFromAssembly`; `SaveChangesAsync` despacha domain events
- **Configurações EF Core:**
  - `UserConfiguration` — `OwnsOne(Email)` → coluna `Email nvarchar(200)` com índice único; `PasswordHash nvarchar(100)`
  - `ProjectConfiguration` — cascade delete em Members e Boards
  - `ColumnConfiguration` — `DeleteBehavior.Restrict` em Cards (protege cards ao reorganizar colunas)
  - `CardConfiguration` — Priority/Status como `nvarchar(20)`; índices em ColumnId, BoardId, DueDate, IsArchived
- **Repositórios:** `Repository<T>` (base com `protected AppDbContext Context`), `UserRepository`, `ProjectRepository`, `BoardRepository`, `CardRepository`
- `UnitOfWork` — suporte a transações com `IDbContextTransaction`
- `TokenService` — JWT `HmacSha256`; lê `Jwt:SecretKey`, `Jwt:Issuer`, `Jwt:Audience`, `Jwt:ExpirationMinutes`
- `PasswordHasher` — `BCrypt.Net.BCrypt.EnhancedHashPassword(password, workFactor: 12)`
- `CurrentUserService` — lê claims JWT via `IHttpContextAccessor`, usa `FindFirst()?.Value`
- Usa `<FrameworkReference Include="Microsoft.AspNetCore.App" />` (não pacote avulso)

**DI:** `services.AddInfrastructure(configuration)`

---

### API Layer (`src/FlowBoard.API/`)

- `ExceptionHandlingMiddleware` — `NotFoundException→404`, `ForbiddenException→403`, `ValidationException→400 ValidationProblemDetails`, `UnauthorizedAccessException→401`, default→500 (todos como `application/problem+json`)
- `ApiController` — abstrato, `[ApiController]`, `[Authorize]`, `[Produces("application/json")]`, injeta `ISender`
- **Controllers:**
  - `AuthController` — `[AllowAnonymous]`; POST `/api/auth/register` (201), POST `/api/auth/login` (200)
  - `ProjectsController` — CRUD + POST `/{id}/members`
  - `BoardsController` — GET, POST board, POST column, PATCH reorder
  - `CardsController` — GET, POST, PUT, PATCH move, PATCH assignees, PATCH checklist toggle
- `Program.cs` — JWT Bearer, Swagger com Bearer security, CORS via config, middleware registrado primeiro
- `appsettings.json` — connection string, `Jwt:SecretKey` (placeholder), `Cors:AllowedOrigins: ["http://localhost:5173"]`
- `appsettings.Development.json` — JWT expiry 480min, CORS inclui porta 3000

---

### Pacotes NuGet (versões fixas)

| Projeto | Pacote | Versão |
|---|---|---|
| Application | MediatR | 12.4.1 |
| Application | FluentValidation.DependencyInjectionExtensions | 11.11.0 |
| Application | Microsoft.Extensions.Logging.Abstractions | 8.0.2 |
| Infrastructure | Microsoft.EntityFrameworkCore.SqlServer | 8.0.10 |
| Infrastructure | Microsoft.EntityFrameworkCore.Tools | 8.0.10 |
| Infrastructure | BCrypt.Net-Next | 4.0.3 |
| Infrastructure | System.IdentityModel.Tokens.Jwt | 8.1.2 |
| API | Swashbuckle.AspNetCore | 6.9.0 |
| API | Microsoft.AspNetCore.Authentication.JwtBearer | 8.0.10 |
| API | Microsoft.EntityFrameworkCore.Design | 8.0.10 |

---

### Banco de dados

- Migration gerada: `src/FlowBoard.Infrastructure/Persistence/Migrations/20260713174409_InitialCreate.cs`
- Script SQL idempotente: `database/FlowBoard_Schema.sql` — 9 tabelas, 13 índices, guards `IF NOT EXISTS`
- **Para aplicar no banco:** `dotnet ef database update --project src\FlowBoard.Infrastructure --startup-project src\FlowBoard.API`
- Ferramenta global instalada: `dotnet-ef` versão 10.0.9

---

## Decisões técnicas importantes

| Decisão | Motivo |
|---|---|
| Construtores privados + factory methods | Entidades nunca em estado inválido |
| `Email` como ValueObject com `OwnsOne` | Encapsula validação sem tabela separada |
| `DeleteBehavior.Restrict` em Column→Cards | Evita deleção em cascata ao reorganizar colunas |
| `ICurrentUserService` injetado nos handlers | `UserId` nunca no corpo da request — previne spoofing |
| Enums persistidos como `nvarchar` | Legibilidade em queries SQL diretas |
| `FindFirst()?.Value` em vez de `FindFirstValue` | `FindFirstValue` não está disponível sem extensão ASP.NET específica |
| `protected AppDbContext Context` na base | Resolve CS9107 (primary constructor + passagem para base) |
| `public SetPosition()` em `Card` | `internal` impedia acesso cross-assembly da Application |
| `workFactor: 12` no BCrypt | Custo computacional adequado para produção |
| `ClockSkew = Zero` no JWT | Sem tolerância de desvio de relógio |

---

## Pendências do backend (menores)

- [ ] `PasswordHash` mapeado como `nvarchar(100)` — BCrypt gera ~60 chars, pode ajustar
- [ ] Fluxo de **refresh token** — `GenerateRefreshToken()` existe mas não há armazenamento/rotação
- [ ] Command `RemoveProjectMember` — não criado (apenas `AddProjectMember`)
- [ ] Command `ArchiveCard` — `Card.Archive()` existe no Domain mas sem command/endpoint
- [ ] Commands para **tags** — `Card.AddTag()` / `Card.RemoveTag()` sem command/endpoint
- [ ] Command `AddChecklistItem` via API — método no Domain existe mas sem endpoint
- [ ] `GET /api/projects/{id}/boards` — endpoint de listagem de boards por projeto
- [ ] Aplicar migration no banco (`dotnet ef database update`)
- [ ] Projetos de testes (xUnit + Moq sugeridos)

---

## Frontend React — **NÃO INICIADO**

### O que precisa ser criado

```
flowboard-web/          ← criar com: npm create vite@latest flowboard-web -- --template react-ts
├── src/
│   ├── features/
│   │   ├── auth/       ← login, register, JWT storage
│   │   ├── board/      ← Board, Column, Card, drag-and-drop
│   │   ├── project/    ← listagem e criação de projetos
│   │   └── card/       ← detalhe do card, checklist, assignees
│   ├── shared/
│   │   ├── api/        ← axios instance, interceptors JWT
│   │   ├── store/      ← Zustand
│   │   └── components/ ← UI base (Button, Modal, Input...)
│   └── router/         ← React Router v6, rotas protegidas
```

### Stack frontend sugerida

| Responsabilidade | Lib sugerida |
|---|---|
| Bundler | Vite + TypeScript |
| Roteamento | React Router v6 |
| Estado global | Zustand |
| Chamadas API | Axios + React Query (TanStack Query) |
| Drag and drop | @dnd-kit/core + @dnd-kit/sortable |
| Estilo | Tailwind CSS |
| Ícones | Lucide React |

### URL da API em desenvolvimento
`http://localhost:5000` (ou porta definida no `launchSettings.json`)

### Autenticação
- JWT armazenado no `localStorage` (ou `sessionStorage`)
- Axios interceptor adiciona `Authorization: Bearer <token>` em toda request
- React Router v6 com `<Navigate>` em rotas protegidas

---

## Como rodar o backend localmente

```bash
# Restaurar pacotes e compilar
cd C:\Users\User\Desktop\Repositórios\PROJETO_KANBAM
dotnet build

# Aplicar migration no SQL Server
dotnet ef database update --project src\FlowBoard.Infrastructure --startup-project src\FlowBoard.API

# Rodar a API
dotnet run --project src\FlowBoard.API
```

Configurar a string de conexão em `src/FlowBoard.API/appsettings.Development.json` antes de rodar.

---

## Próximo passo imediato

Iniciar o **frontend React**:

```bash
cd C:\Users\User\Desktop\Repositórios\PROJETO_KANBAM
npm create vite@latest flowboard-web -- --template react-ts
cd flowboard-web
npm install
npm install react-router-dom zustand axios @tanstack/react-query @dnd-kit/core @dnd-kit/sortable tailwindcss @tailwindcss/vite lucide-react
```
