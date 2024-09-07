-- Procedimiento almacenado para crear un usuario
CREATE OR REPLACE PROCEDURE sp_create_user(
    nombre VARCHAR(100),
    telefono VARCHAR(20),
    direccion VARCHAR(255),
    pais_id INT,
    departamento_id INT,
    municipio_id INT
) 
LANGUAGE plpgsql AS $$
BEGIN
    INSERT INTO usuario (nombre, telefono, direccion, pais_id, departamento_id, municipio_id)
    VALUES (nombre, telefono, direccion, pais_id, departamento_id, municipio_id);
END;
$$;
