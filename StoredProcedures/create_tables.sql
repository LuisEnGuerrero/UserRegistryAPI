CREATE TABLE IF NOT EXISTS pais (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL
);

CREATE TABLE IF NOT EXISTS departamento (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    pais_id INT REFERENCES pais(id)
);

CREATE TABLE IF NOT EXISTS municipio (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    departamento_id INT REFERENCES departamento(id)
);

CREATE TABLE IF NOT EXISTS usuario (
    id SERIAL PRIMARY KEY,
    nombre VARCHAR(100) NOT NULL,
    telefono VARCHAR(15) NOT NULL,
    direccion VARCHAR(200),
    pais_id INT REFERENCES pais(id),
    departamento_id INT REFERENCES departamento(id),
    municipio_id INT REFERENCES municipio(id)
);

