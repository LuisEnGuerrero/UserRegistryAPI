-- Procedimiento almacenado para crear un usuario si no existe
CREATE OR REPLACE PROCEDURE sp_create_user(
    p_nombre TEXT,
    p_telefono TEXT,
    p_direccion TEXT,
    p_pais_id INT,
    p_departamento_id INT,
    p_municipio_id INT
) 
LANGUAGE plpgsql AS $$
BEGIN
    -- Verificar si el usuario ya existe
    IF EXISTS (SELECT 1 FROM usuario WHERE nombre = p_nombre AND telefono = p_telefono) THEN
        RAISE NOTICE 'El usuario ya existe.';
    ELSE
        INSERT INTO usuario (nombre, telefono, direccion, pais_id, departamento_id, municipio_id)
        VALUES (p_nombre, p_telefono, p_direccion, p_pais_id, p_departamento_id, p_municipio_id);
    END IF;
END;
$$;
