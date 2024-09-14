# API de Registro de Usuarios v1.1

### Desarrollador: Luis Enrique Guerrero Ibarra  
Fecha: 13 de septiembre de 2024  

## Descripción del Proyecto

La API de Registro de Usuarios (v1.1) es una API RESTful diseñada en **ASP.NET Core** que permite la gestión segura de usuarios mediante roles diferenciados (usuarios-roll), proporcionando control de acceso y funcionalidades CRUD sobre una base de datos PostgreSQL. Esta versión incluye autenticación con **JWT** y registro/login mediante **Google OAuth 2.0**.

El objetivo de la API es gestionar usuarios con roles específicos, asegurando que solo usuarios autorizados puedan acceder a las funcionalidades según su rol. Además, se garantiza que los datos de los usuarios estén protegidos mediante encriptación y autenticación segura.

## Tecnologías Utilizadas
- **Lenguaje de Programación**: C# (ASP.NET Core)
- **Base de Datos**: PostgreSQL
- **ORM / Micro-ORM**: Dapper
- **Autenticación**: JWT y Google OAuth 2.0
- **Dependencias**: NuGet, BCrypt.Net, CsvHelper

## Funcionalidades Principales
1. **Usuarios-Roll**: 
   - Roles: `AdminMaster`, `Viewer`, `CreatorAdmin`, `EditorAdmin`.
   - Gestión de usuarios por roles, permitiendo acciones específicas para cada tipo de rol.
   
2. **Autenticación con JWT**:
   - Login y registro con email y contraseña.
   - Generación y validación de tokens JWT para proteger las solicitudes.
   
3. **Autenticación con Google OAuth 2.0**:
   - Login/registro mediante Google.
   - Creación automática de usuarios no autorizados, pendientes de aprobación por el `AdminMaster`.
   
4. **Operaciones CRUD para Usuarios-Roll**:
   - Crear, modificar, visualizar y eliminar usuarios con roles.

5. **Validaciones de Seguridad**:
   - Contraseñas encriptadas con BCrypt.
   - Autorización basada en roles.
   - Tokens JWT con tiempo de expiración.

## Endpoints Implementados
### Usuarios-Roll
- `POST /api/authuser/login`: Autenticación de usuarios-roll con email y contraseña.
- `POST /api/authuser/signin-google`: Autenticación de usuarios con Google OAuth.
- `GET /api/authuser/get-auth-users`: Obtener todos los usuarios con rol.
- `POST /api/authuser/create-auth-user`: Crear un nuevo usuario con rol (solo `AdminMaster`).
- `PUT /api/authuser/update-auth-user/{id}`: Actualizar un usuario con rol (solo `AdminMaster` y `EditorAdmin`).
- `PUT /api/authuser/authorize-login/{id}`: Autorizar el acceso de un usuario con rol (solo `AdminMaster`).
- `DELETE /api/authuser/delete-auth-user/{id}`: Eliminar un usuario con rol (solo `AdminMaster`).
- `PUT /api/authuser/change-role/{id}`: Cambiar el rol de un usuario (solo `AdminMaster`).

### Usuarios-Data
- `POST /api/user/`: Crear un nuevo usuario-registro (solo `AdminMaster` y `CreatorAdmin`).
- `GET /api/user/`: Obtener todos los usuarios-registro.
- `GET /api/user/{id}`: Obtener un usuario por su ID.
- `PUT /api/user/{id}`: Actualizar un usuario-registro (solo `AdminMaster` y `EditorAdmin`).
- `DELETE /api/user/{id}`: Eliminar un usuario-registro (solo `AdminMaster`).

## Base de Datos
La estructura de la base de datos incluye tablas relacionadas para usuarios, roles, países, departamentos y municipios. Los campos principales son:
- **auth_users**: Información de los usuarios con rol, integrando la autenticación de Google.
- **pais, departamento, municipio**: Información geográfica normalizada.
- **usuario**: Información de los usuarios registrados sin roles.

### Unicidades y Restricciones
- Se implementaron restricciones **UNIQUE** en campos clave como `google_id` y en combinaciones de `nombre` con `departamento_id` y `pais_id`.

## Instrucciones de Instalación
1. Clonar el repositorio.
2. Configurar las variables de entorno (.env) para la conexión a la base de datos y las credenciales de Google.
3. Ejecutar las migraciones y/o aplicar los cambios en la base de datos mediante el script `create_tables.sql`.
4. Ejecutar la aplicación usando `dotnet run`.

## Conclusión
Este proyecto ha sido diseñado con un enfoque en la seguridad y la escalabilidad, implementando tecnologías críticas como JWT, Google OAuth 2.0 y BCrypt para la gestión segura de usuarios. Además, la gestión de roles diferenciados permite un control granular de las funcionalidades accesibles para cada usuario, lo que lo convierte en una solución ideal para la gestión de usuarios en entornos corporativos.

## Contacto
Luis Enrique Guerrero Ibarra  
luisenguerrero@yahoo.com  
WhatsApp: 3208172936
