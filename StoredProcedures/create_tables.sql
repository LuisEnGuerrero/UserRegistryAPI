-- Crear la base de datos si no existe // Importante: Esto requiere que la extensión dblink esté instalada en PostgreSQL (puede instalarse con CREATE EXTENSION dblink; si es necesario).
DO
$$
BEGIN
   IF NOT EXISTS (
       SELECT FROM pg_database WHERE datname = 'userregistrydb'
   ) THEN
       PERFORM dblink_exec('dbname=postgres', 'CREATE DATABASE userregistrydb');
   END IF;
END
$$;

-- Conectarse a la base de datos recién creada o existente
\c userregistrydb;


-- Tabla pais con restricción de unicidad
CREATE TABLE IF NOT EXISTS pais (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL UNIQUE
);

-- Tabla departamento con unicidad de nombre por país
CREATE TABLE IF NOT EXISTS departamento (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    pais_id INT REFERENCES pais(id),
    UNIQUE (nombre, pais_id)  -- Unicidad de nombre por país
);

-- Tabla municipio con unicidad de nombre por departamento
CREATE TABLE IF NOT EXISTS municipio (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    departamento_id INT REFERENCES departamento(id),
    UNIQUE (nombre, departamento_id)  -- Unicidad de nombre por departamento
);

-- Tabla usuario con mejora en el tamaño del campo teléfono y posibles ajustes en dirección
CREATE TABLE IF NOT EXISTS usuario (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    telefono VARCHAR(20) NOT NULL,  -- Se incrementó la longitud a 20 caracteres
    direccion VARCHAR(250),  -- Se incrementó a 250 caracteres
    pais_id INT REFERENCES pais(id),
    departamento_id INT REFERENCES departamento(id),
    municipio_id INT REFERENCES municipio(id)
);

-- Tabla auth_users con correcciones y mejoras en unicidad
CREATE TABLE auth_users (
    id SERIAL PRIMARY KEY,
    username VARCHAR(255) NOT NULL,
    email VARCHAR(255) NOT NULL UNIQUE,  -- Se añadió restricción UNIQUE para email
    password_hash VARCHAR(255) NOT NULL,
    role VARCHAR(50) NOT NULL,
    is_google_auth BOOLEAN DEFAULT FALSE,
    google_id VARCHAR(255) UNIQUE,  -- Se añadió restricción UNIQUE para google_id
    is_authorized BOOLEAN DEFAULT FALSE,  -- Corregido de is_autoriced a is_authorized
    created_at TIMESTAMP DEFAULT CURRENT_TIMESTAMP
);
