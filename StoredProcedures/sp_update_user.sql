CREATE OR REPLACE PROCEDURE sp_update_user(
    IN p_id INT,
    IN p_nombre VARCHAR,
    IN p_telefono VARCHAR,
    IN p_direccion VARCHAR,
    IN p_pais_id INT,
    IN p_departamento_id INT,
    IN p_municipio_id INT
)
LANGUAGE plpgsql AS $$
BEGIN
    UPDATE usuario
    SET nombre = p_nombre,
        telefono = p_telefono,
        direccion = p_direccion,
        pais_id = p_pais_id,
        departamento_id = p_departamento_id,
        municipio_id = p_municipio_id
    WHERE id = p_id;
END;
$$;
